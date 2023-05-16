using Saber.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;
using FSM;
using XianXia.Terrain;
using Saber.Camp;
using static UnityEngine.GraphicsBuffer;

namespace XianXia.Unit
{
    public class UnitSpellSystem : UnitSkillSystem<MagicOrgan,ActiveSkill>
    {
        EventSystem eventSystem;
        UnitMainSystem mainSystem;
        SaberEvent<MagicOrgan, ActiveSkill> UnitSpellBefore;
        SaberEvent<MagicOrgan, ActiveSkill> UnitSpellAfter;
        AStarPathfinding2D map;
        UnitMoveSystem moveSystem;
        UnitBodySystem bodySystem;
        TimerManagerSystem timerSystem;

        public override void Awake(WorldBase world)
        {
            base.Awake(world);
            eventSystem = world.FindSystem<EventSystem>();

            map=GameObject.FindObjectOfType<AStarPathfinding2D>();

        }
        public override void Start()
        {
            base.Start();
            mainSystem = world.FindSystem<UnitMainSystem>();
            moveSystem = world.FindSystem<UnitMoveSystem>();
            bodySystem = world.FindSystem<UnitBodySystem>();
            timerSystem = world.FindSystem<TimerManagerSystem>();
            eventSystem.GetEvent<AttackOrgan, BodyOrgan,Damage>("UnitAttackAfter").AddAction(ListenAttackRecordMagicPoint);
            eventSystem.GetEvent<BodyOrgan, UnitBase>("UnitDamagedAfter").AddAction(ListenDamageRecordMagicPoint);
            UnitSpellBefore = eventSystem.RegisterEvent<MagicOrgan, ActiveSkill>("UnitSpellBefore");
            UnitSpellAfter= eventSystem.RegisterEvent<MagicOrgan, ActiveSkill>("UnitSpellAfter");
        }

        protected override void InitializeBeforeRecycle(MagicOrgan t)
        {
            if (t != null && t.CharacterFSM != null)
                t.CharacterFSM.RemoveState(FSM_State.spell);
            //t.Destory();
            base.InitializeBeforeRecycle(t);
        }
        protected override void InitAfterSpawn(MagicOrgan t)
        {
            if (t != null && t.CharacterFSM != null)
            {
                t.CharacterFSM.AddState(new Network_Spell(t.CharacterFSM.Animator));
            }
            base.InitAfterSpawn(t);
        }
        internal static void SetSkillMagicPoint(ActiveSkill skill, MagicOrgan t,bool currIsZero=false)
        {
            IMagicPointRecover magicPointRecover = t;
            bool res = magicPointRecover.CanRecordMagicPoint;
            magicPointRecover.CanRecordMagicPoint = true;
            magicPointRecover.MagicPoint_Max = skill.NeedMagicPointMax;
            if (!currIsZero)
                magicPointRecover.MagicPoint_Curr = skill.StartMagicPoint;
            else
                magicPointRecover.MagicPoint_Curr = 0;
            magicPointRecover.MagicPoint_Attack = skill.MagicPoint_Attack;
            magicPointRecover.MagicPoint_Damaged = skill.MagicPoint_Damaged;
            magicPointRecover.CanRecordMagicPoint = res;
        }
        protected override void GainSkillAfter(ActiveSkill skill, MagicOrgan t)
        {
            //Debug.Log(skill.ToString()+"QQQ");
            //Debug.Log("QQQ" + t.StatusList.Count);

            if (skill!=null&&t.StatusList.Count==1)
            {
                SetSkillMagicPoint(skill, t);
                //Debug.Log()

            }
        }
        protected void ListenAttackRecordMagicPoint(AttackOrgan attackOrgan, BodyOrgan enemyBody,Damage damage)
        {
            if(attackOrgan==null||enemyBody==null||attackOrgan.OwnerUnit==null) return;
            IMagicPointRecover magicPointRecover = attackOrgan.OwnerUnit.FindOrganInBody<MagicOrgan>(ComponentType.magic);
            if (magicPointRecover != null&&magicPointRecover.CanRecordMagicPoint) magicPointRecover.MagicPoint_Curr += magicPointRecover.MagicPoint_Attack;
        }
        protected void ListenDamageRecordMagicPoint(BodyOrgan body, UnitBase source)
        {
            if (body == null || source == null || body.OwnerUnit == null) return;
            IMagicPointRecover magicPointRecover = body.OwnerUnit.FindOrganInBody<MagicOrgan>(ComponentType.magic);
            if (magicPointRecover != null && magicPointRecover.CanRecordMagicPoint) magicPointRecover.MagicPoint_Curr += magicPointRecover.MagicPoint_Damaged;
        }
        #region 寻敌函数
        Node FindTarget(MagicOrgan magicOrgan,ActiveSkill activeSkill)
        {
            if(magicOrgan==null||magicOrgan.OwnerUnit==null||activeSkill==null)return null;
            if (activeSkill.SpellTiggerType == SpellTiggerType.immediate) return null;
            if (activeSkill.SpellTiggerType == SpellTiggerType.unit)
            {
               // Debug.Log("RRR " + activeSkill.TargetType + "");
                Node target = AStarPathfinding2D.FindTargetInRange(mainSystem.GetGridItemByUnit(magicOrgan.OwnerUnit), activeSkill.SpellRange, (a, b) => { return mainSystem.TargetFiltration(a, b, activeSkill.TargetType); }, (a, b) => { return SystemUtility.SortFiltration(magicOrgan.OwnerUnit, mainSystem.GetUnitByGridItem(a), mainSystem.GetUnitByGridItem(b), activeSkill.TargetType); }, map, true, false,true);
                //Debug.Log(target+"找到"+"RRR");
                return target;
            }
            else if (activeSkill.SpellTiggerType == SpellTiggerType.point)
           {
                if (activeSkill is IPointSkill pointSkill)
                { Node node=pointSkill.FindTarget(); Debug.Log("CC" + node.Position);  return node; }
                else
                    return PointFindTarget(magicOrgan.OwnerUnit);
            }
            else
                return null;
        }
        Node PointFindTarget(UnitBase unit)
        {
            var res = AStarPathfinding2D.FindNearestNode(mainSystem.GetGridItemByUnit(unit), 100, 1, (a, b) =>
            {
                if (mainSystem.GetUnitByGridItem(b) == null) return true;
                else return false;
            }, map, true, true, true);
            //Debug.Log("召唤" + res.Count + res[0].Position);
            if (res != null && res.Count > 1) return res[0];
            else return null;
        }


        #endregion
        //public int SortFiltration(UnitBase spellUnit, Node unitA, UnitBase unitB, TargetType targetType)
        //{

        //}
        public void SetSpellTime(CharacterFSM magicOrgan,float time) 
        {
            //if (magicOrgan == null) return;
            //((Spell)magicOrgan.FindFSMState(FSM_State.spell)).SetFinishTime(time);
        }


        internal void SpellSkill(MagicOrgan statusOrganBase, ActiveSkill activeSkill)
        {
            if (SystemUtility.StatusOrganContains(activeSkill, statusOrganBase))
            {
                IMagicPointRecover modif = (IMagicPointRecover)statusOrganBase;
                //判断是不是有目标的技能进行目标搜寻
                //Debug.Log("寻敌RRR");
                Node target = FindTarget(statusOrganBase,activeSkill);
                Debug.Log(target + "RRR");
                if (target == null && activeSkill.SpellTiggerType != SpellTiggerType.immediate) return;
                modif.MagicPoint_Curr = 0;
                mainSystem.UnitTakeAction(statusOrganBase.OwnerUnit, FSM_State.spell, IE_UnitSpellAction(statusOrganBase, activeSkill, target));
            }
        }
        IEnumerator IE_UnitSpellAction(MagicOrgan spellBrain, ActiveSkill activeSkill, Node target)
        {
            //yield return waitForOneDelta;

            void SpellAction(MagicOrgan spellBrain, ActiveSkill activeSkill, Node target)
            {
                if (activeSkill.SpellTiggerType != SpellTiggerType.immediate)
                    activeSkill.Target=target;
                activeSkill.OnSpell();
                if (activeSkill.CDTime > 0) { activeSkill.IsCD = false; timerSystem.AddTimer(() => activeSkill.IsCD = true, activeSkill.CDTime); }
                FightLog.Record($"玩家{UnitMainSystem.GetUnitBelongPlayer(spellBrain.OwnerUnit)} 单位{spellBrain.OwnerUnit.gameObject.name} 释放了技能:{activeSkill.RealName},目标是{target?.Position}");
                //BaseUtility.ShowRisingSpaceInScreen(activeSkill.SkillInfo.SkillName, spellBrain.GetChessBuilder().GetEquipPart(CharaterPartPoint.overHead).position, Vector3.up * 1.5f + Vector3.left);
            }
            WaitUntil waitSpellFinish = new WaitUntil(() =>
            {
                return spellBrain.CharacterFSM.CurrentState != FSM_State.spell;
            });
            if (spellBrain == null || spellBrain.OwnerUnit == null) { yield break; }
            //mainSystem.CorrectionPosition(spellBrain.OwnerUnit);

            IMagicPointRecover magicPointRecover = spellBrain;
            magicPointRecover.CanRecordMagicPoint = false;
            UnitBase chess = spellBrain.OwnerUnit;

            if (target != null && target != AStarPathfinding2D.GetWorldPositionNode(chess.transform.position,map))
            {
                //转向
                Vector2 dir = AStarPathfinding2D.GetNodeWorldPositionV2(target.Position,map)- (Vector2)chess.transform.position;
                int x = dir.x > 0 ? x = 1 : x = -1;
                spellBrain.CharacterFSM.SendX(x);
            }
   

            UnitSpellBefore.Trigger(spellBrain, activeSkill);
            // yield return waitForOneDelta;
            if (activeSkill.NeedSpellAction)
            {
                spellBrain.CharacterFSM.SetCurrentState(FSM_State.spell);
                yield return new WaitForSeconds(Time.deltaTime);
                yield return waitSpellFinish;
            }


            SpellAction(spellBrain, activeSkill, target);
            //施法后摇
            yield return new WaitForSeconds(0.2f);
            magicPointRecover.CanRecordMagicPoint = true;
            //如果需要得到之前超过的蓝量在这加
            UnitSpellAfter.Trigger(spellBrain, activeSkill);
            mainSystem.UnitStopAction(chess);

        }
        public override void Update()
        {
            base.Update();
            foreach(var v in allComponents)
            {
                if (v != null &&v.StatusList.Count>0&& v.Enable == true&&v.ReadyToSpell&& v.StatusList[0].Enable && v.StatusList[0].IsCD && UnitMainSystem.ActionPriorityLevel(mainSystem.GetUnitAction(v.OwnerUnit)))
                {
                    LegOrgan leg = v.OwnerUnit.FindOrganInBody<LegOrgan>(ComponentType.leg);
                    if (leg == null || leg.IsStand) SpellSkill(v, v.StatusList[0]);
                    else moveSystem.GoBackPos(v.OwnerUnit);
                }
            }
        }
    }
}

using Saber.Base;
using Saber.Camp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;
using XianXia.Terrain;
using FSM;
using FishNet;

namespace XianXia.Unit
{
    public class UnitBodySystem : NormalSystemBase<BodyOrgan>
    {

        private bool showDamgeRisingSpace = true;
        bool isCorrectionPosition = true;


        UnitMainSystem unitPosSystem;
        EventSystem eventSystem;
        AStarPathfinding2D gridMap;
        //RisingSpaceUISystem risingSpaceSystem;

        SaberEvent<BodyOrgan, Damage> UnitDamagedBefore;
        SaberEvent<BodyOrgan, UnitBase> UnitDamagedAfter;
        SaberEvent<UnitBase> UnitDeadAfter;
        SaberEvent<BodyOrgan, UnitBase> UnitDeadBefore;
        Vector3 risingSpaceDir;

        public override void Awake(WorldBase world)
        {
            base.Awake(world);
            unitPosSystem = world.FindSystem<UnitMainSystem>();
            eventSystem= world.FindSystem<EventSystem>();
            UnitDeadBefore = eventSystem.RegisterEvent<BodyOrgan, UnitBase>("UnitDeadBefore");
            UnitDeadAfter = eventSystem.RegisterEvent<UnitBase>("UnitDeadAfter");
            UnitDamagedAfter = eventSystem.RegisterEvent<BodyOrgan, UnitBase>("UnitDamagedAfter");
            UnitDamagedBefore = eventSystem.RegisterEvent<BodyOrgan, Damage>("UnitDamagedBefore");
            //risingSpaceSystem=world.FindSystem<RisingSpaceUISystem>();
            risingSpaceDir = new Vector3(1, 1,0);
        }
        public override void Start()
        {
            base.Start();


            gridMap = Object.FindObjectOfType<AStarPathfinding2D>();
        }
        protected override void InitAfterSpawn(BodyOrgan t)
        {
            base.InitAfterSpawn(t);
            t.CharacterFSM.AddState(new Death(t.CharacterFSM.Animator));
            if (isCorrectionPosition)
            {
                //unitPosSystem.CorrectionPosition(t.OwnerUnit);
            }
        }
        public void ReceiveDamage(BodyOrgan bodyOrgan,Damage damage)
        {
            if (bodyOrgan == null || damage == null||damage.Val<=0) return;
            //闪避加在这
            if (SystemUtility.CalculatePer(bodyOrgan.Evade))
            {
                InstanceFinder.GetInstance<NormalUtility>().ORPC_ShowRisingSpace("evade", SystemUtility.GetBodyPos(bodyOrgan.OwnerUnit), risingSpaceDir, Color.white, 28);
                return;
            }

            UnitDamagedBefore.Trigger(bodyOrgan, damage);
            int val = damage.Val - bodyOrgan.Def;
            bodyOrgan.Health_Curr -= val;

            if (bodyOrgan.OwnerUnit!=null)
            {
                if (damage.IsCriticalStrike)
                    InstanceFinder.GetInstance<NormalUtility>().ORPC_ShowRisingSpace(val.ToString(), bodyOrgan.OwnerUnit.transform.position + Vector3.up * 0.5f, risingSpaceDir, Color.red, 56, TMPro.FontStyles.Bold);
                else if (showDamgeRisingSpace)
                    InstanceFinder.GetInstance<NormalUtility>().ORPC_ShowRisingSpace(val.ToString(), bodyOrgan.OwnerUnit.transform.position + Vector3.up * 0.5f, risingSpaceDir);
                Debug.Log("body" + bodyOrgan + "受到了" + damage.Val + "伤害,来源于" + damage.Source);
                FightLog.Record($"玩家:{CampManager.GetPlayerEnum(bodyOrgan.OwnerPlayer)} 单位:{bodyOrgan.OwnerUnit.gameObject.name}受到了{damage.Val}点伤害，来源于 玩家{UnitMainSystem.GetUnitBelongPlayer(damage.Source)} 单位:{damage.Source.gameObject.name}。剩余生命值:{bodyOrgan.Health_Curr}点");
            }

            UnitDamagedAfter.Trigger(bodyOrgan, damage.Source);

            if (bodyOrgan.Health_Curr<=0)
                UnitDead(bodyOrgan, damage);
        }

        public void UnitHeal(BodyOrgan bodyOrgan,int num,UnitBase source)
        {
            if (bodyOrgan == null||bodyOrgan.OwnerUnit==null) return;
            bodyOrgan.Health_Curr += num;
            InstanceFinder.GetInstance<NormalUtility>().ORPC_ShowRisingSpace(num.ToString(), bodyOrgan.OwnerUnit.transform.position + Vector3.up * 0.5f, risingSpaceDir,Color.green);
            Debug.Log("body" + bodyOrgan.OwnerUnit +"受到了来自"+source+"的治疗："+num);
            FightLog.Record($"玩家:{CampManager.GetPlayerEnum(bodyOrgan.OwnerPlayer)} 单位:{bodyOrgan.OwnerUnit.gameObject.name}受到了{num}点治疗，来源于 玩家{UnitMainSystem.GetUnitBelongPlayer(source)} 单位:{source.gameObject.name}。剩余生命值:{bodyOrgan.Health_Curr}点");

        }
        public void UnitDead(BodyOrgan bodyOrgan, Damage damage)
        {
            if (bodyOrgan == null||bodyOrgan.OwnerUnit==null) return;
            if (damage == null) damage = Damage.GodDamage;
            UnitDeadBefore.Trigger(bodyOrgan, damage.Source);
            bodyOrgan.CharacterFSM.SetCurrentState(FSM_State.death);
            Debug.Log("body" + bodyOrgan.OwnerUnit + "死亡，凶手是"+damage.Source);
            FightLog.Record($"玩家:{CampManager.GetPlayerEnum(bodyOrgan.OwnerPlayer)} 单位:{bodyOrgan.OwnerUnit.gameObject.name}死亡，凶手是 玩家{UnitMainSystem.GetUnitBelongPlayer(damage.Source)} 单位:{damage.Source.gameObject.name}。");

            unitPosSystem.RemoveUnitFromGame(bodyOrgan.OwnerUnit,bodyOrgan.DeadTime);
            UnitDeadAfter.Trigger(damage.Source);
        }

        public void SetUnitPlayer(BodyOrgan bodyOrgan,PlayerMemeber playerMemeber)
        {
            if (bodyOrgan == null) return;
            bodyOrgan.OwnerPlayer=playerMemeber;
            UIShowOrgan uIShowOrgan = bodyOrgan.OwnerUnit.FindOrganInBody<UIShowOrgan>(ComponentType.uIShow);
            if (uIShowOrgan != null) InstanceFinder.GetInstance<NormalUtility>().Server_SetUnitColor(bodyOrgan.OwnerUnit.gameObject, playerMemeber.Color);
        }
        /// <summary>
        /// 返回两个格子之间的关系
        /// 如果不存在占领棋子则返回none
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="otherGrid"></param>
        /// <returns></returns>
        public CampRelation RelationOfTwoGrids(Node grid, Node otherGrid) => SystemUtility.RelationOfTwoGrids(unitPosSystem.GetUnitByGridItem(grid), unitPosSystem.GetUnitByGridItem(otherGrid));


    }
}

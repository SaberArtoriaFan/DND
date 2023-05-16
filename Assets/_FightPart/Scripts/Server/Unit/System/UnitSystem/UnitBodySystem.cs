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
            //���ܼ�����
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
                Debug.Log("body" + bodyOrgan + "�ܵ���" + damage.Val + "�˺�,��Դ��" + damage.Source);
                FightLog.Record($"���:{CampManager.GetPlayerEnum(bodyOrgan.OwnerPlayer)} ��λ:{bodyOrgan.OwnerUnit.gameObject.name}�ܵ���{damage.Val}���˺�����Դ�� ���{UnitMainSystem.GetUnitBelongPlayer(damage.Source)} ��λ:{damage.Source.gameObject.name}��ʣ������ֵ:{bodyOrgan.Health_Curr}��");
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
            Debug.Log("body" + bodyOrgan.OwnerUnit +"�ܵ�������"+source+"�����ƣ�"+num);
            FightLog.Record($"���:{CampManager.GetPlayerEnum(bodyOrgan.OwnerPlayer)} ��λ:{bodyOrgan.OwnerUnit.gameObject.name}�ܵ���{num}�����ƣ���Դ�� ���{UnitMainSystem.GetUnitBelongPlayer(source)} ��λ:{source.gameObject.name}��ʣ������ֵ:{bodyOrgan.Health_Curr}��");

        }
        public void UnitDead(BodyOrgan bodyOrgan, Damage damage)
        {
            if (bodyOrgan == null||bodyOrgan.OwnerUnit==null) return;
            if (damage == null) damage = Damage.GodDamage;
            UnitDeadBefore.Trigger(bodyOrgan, damage.Source);
            bodyOrgan.CharacterFSM.SetCurrentState(FSM_State.death);
            Debug.Log("body" + bodyOrgan.OwnerUnit + "������������"+damage.Source);
            FightLog.Record($"���:{CampManager.GetPlayerEnum(bodyOrgan.OwnerPlayer)} ��λ:{bodyOrgan.OwnerUnit.gameObject.name}������������ ���{UnitMainSystem.GetUnitBelongPlayer(damage.Source)} ��λ:{damage.Source.gameObject.name}��");

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
        /// ������������֮��Ĺ�ϵ
        /// ���������ռ�������򷵻�none
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="otherGrid"></param>
        /// <returns></returns>
        public CampRelation RelationOfTwoGrids(Node grid, Node otherGrid) => SystemUtility.RelationOfTwoGrids(unitPosSystem.GetUnitByGridItem(grid), unitPosSystem.GetUnitByGridItem(otherGrid));


    }
}

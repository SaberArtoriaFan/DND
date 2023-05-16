using FishNet;
using Saber.Camp;
using Saber.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XianXia.Terrain;

namespace XianXia.Unit
{
    public class SummonSkill : ActiveSkill,IPointSkill
    {
        public override bool NeedSpellAction => true;
        public override TargetType TargetType { get => base.TargetType; internal set => base.TargetType = value; }
        public override SpellTiggerType SpellTiggerType => SpellTiggerType.point;

        UnitBodySystem bodySystem;
        UnitMainSystem mainSystem;
        //ObjectPoolSystem poolSystem;
        BuffSystem buffSystem;
        GameObject model;
        Saber.Base.ObjectPool<GameObject> pool;
        //GameObject[] usedModels;
        string summonerName= "Panda";
        int summonNum = 1;
        float liveTime=0;

        public override void Init(IContainerEntity magicOrgan)
        {
            base.Init(magicOrgan);
            //usedModels = new GameObject[summonNum];
            CDTime = 20f;
           // FindTargetFunc = FindNullTarget;
            bodySystem = SkillSystem.World.FindSystem<UnitBodySystem>();
            //poolSystem = SkillSystem.World.FindSystem<ObjectPoolSystem>();
            mainSystem = SkillSystem.World.FindSystem<UnitMainSystem>();
            //model = ABUtility.Load<GameObject>(ABUtility.UnitMainName + summonerName);
            //if (model == null)
            //{
            //    Debug.LogError("召唤物模型缺失");
            //    model = ABUtility.Load<GameObject>( + "TempSummoner");
            //}

            //for(int i = 0;i< usedModels.Length; i++)
            //{
            //    usedModels[i] = GameObject.Instantiate(model);
            //    usedModels[i].
            //}
            NormalUtility normalUtility = InstanceFinder.GetInstance<NormalUtility>();
            pool = normalUtility.Server_InitSpawnPool(ABUtility.UnitMainName, summonerName, null, summonNum, (u) =>
              {
                  UnitBase unit = u.GetComponent<UnitBase>();
                  if (unit != null) GameObject.Destroy(unit);
                //u.SetActive(false);
            }, null);

            //pool = poolSystem.AddPool<GameObject>(() =>
            //{
            //    TestUnitModel testUnitModel = GameObject.Instantiate(model).GetComponent<TestUnitModel>();
            //    testUnitModel.gameObject.SetActive(false);
            //    return testUnitModel.gameObject;
            //    //return mainSystem.SwapnUnit(testUnitModel, this.ownerMagicOrgan.OwnerUnit.FindOrganInBody<BodyOrgan>(ComponentType.body).OwnerPlayer).gameObject;

            //}, (u) =>
            //{
            //    UnitBase unit = u.GetComponent<UnitBase>();
            //    if (unit != null) GameObject.Destroy(unit);
            //    u.SetActive(false);
            //}, null,summonerName,summonNum);
        }

        
        public override void OnSpell()
        {
            base.OnSpell();
            if (Target == null) return;
            GameObject model;
            //if (model == null) return;
            //拿到了模型，但是是没有激活的
            //放到目标格子
            var res= AStarPathfinding2D.FindNearestNode(Target, 100,summonNum, (a, b) =>
            {
                if (mainSystem.GetUnitByGridItem(b) == null) return true;
                else return false;
            }, SkillSystem.Map, true, true, true);
            PlayerMemeber player = this.ownerMagicOrgan.OwnerUnit.FindOrganInBody<BodyOrgan>(ComponentType.body).OwnerPlayer;

            Debug.Log("召唤物的玩家类型是" + player.PlayerId);
            
            for (int i=0;i<res.Count; i++)
            {
                if (res[i] != null)
                {
                    model = pool.GetObjectInPool((g) => g.transform.position = AStarPathfinding2D.GetNodeWorldPositionV3(res[i].Position, SkillSystem.Map));
                    //model.transform.position = 
                    model.SetActive(true);
                    Debug.Log("召唤召唤物" + model.name);
                    TestUnitModel unitmodel = model.GetComponent<TestUnitModel>();
                    unitmodel.player = CampManager.GetPlayerEnum(player);
                    Action<UnitBase> action = liveTime > 0 ? (UnitBase unit) => { buffSystem.AddBuff("Summon", unit.FindOrganInBody<StatusOrgan>(ComponentType.statusHeart), null, null, (u) => bodySystem.UnitDead(u.OwnerUnit.FindOrganInBody<BodyOrgan>(ComponentType.body), null)); } : null;
                    //action = null;
                    mainSystem.SwapnUnit(unitmodel,action);
                }
            }
        }

        Node IPointSkill.FindTarget()
        {
            var res = AStarPathfinding2D.FindNearestNode(mainSystem.GetGridItemByUnit(ownerMagicOrgan.OwnerUnit), 100, 1, (a, b) =>
            {
                if (mainSystem.GetUnitByGridItem(b) == null) return true;
                else return false;
            }, SkillSystem.Map, true, true, true);
            //Debug.Log("召唤" + res.Count + res[0].Position);
            if (res != null && res.Count >= 1) return res[0];
            else return null;
        }
    }
}

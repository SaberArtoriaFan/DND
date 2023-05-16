using Saber.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;
using System.Linq;
using XianXia.Terrain;
using Saber.Camp;
using DG.Tweening.Core.Easing;
using static UnityEngine.GraphicsBuffer;
//using cfg.hero;
using UnityEngine.Rendering;
using FishNet;

namespace XianXia.Unit
{
    public class UnitMainSystem : SingletonSystemBase<UnitMainManagerModel>
    {
        public event Action<PlayerMemeber> OnPlayerFailEvent;

        public void CorrectionPosition(UnitBase unit)
        {
            if (unit == null) return;
            //GridItem gridItem = GridMap.GetGridByWorldPosition(unit.transform.position, instance.GridMap);
            Node unitItem = GetGridItemByUnit(unit);
            if (unitItem == null) unitItem = AStarPathfinding2D.GetWorldPositionNode(unit.transform.position, instance.GridMap);
            if (unitItem == null) { Debug.LogError("出现在非法位置"); return; }
            //RemoveUnitFromPos(unit);
            //if (unitItem != gridItem)
            //{
            //    gridItem = GridUtility.FindNearestGridItem(gridItem, GridType.Ground);
            //    //if (gridItem == null) return;
            //}
            //AddUnitToPos(unit, gridItem);
            unit.gameObject.transform.position = AStarPathfinding2D.GetNodeWorldPositionV3(unitItem.Position, instance.GridMap);
        }

        /// <summary>
        /// 移动时使用
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="gridItem"></param>
        public void AddUnitToPos(UnitBase unit, Node gridItem)
        {
            if (unit == null || gridItem == null) return;
            if (instance.PosUnitDict.ContainsKey(gridItem)) return;
            if (instance.UnitPosDict.TryGetValue(unit, out var node) &&node==gridItem&& instance.PosUnitDict.TryGetValue(node, out UnitBase temp) && unit == temp) return;
            RemoveUnitFromPos(unit);
            instance.UnitPosDict.Add(unit, gridItem);
            instance.PosUnitDict.Add(gridItem, unit);
            ////更新动态障碍物表
            //AStarPathfinding2D.UpdateDynamicObstacle(gridItem.Position, instance.GridMap, true);
        }
        /// <summary>
        /// 更新动态表
        /// </summary>
        public void UpdateDynamicObstacle(Node gridItem,bool isAdd=true)
        {
            //if (gridItem == null) return;
            ////更新动态障碍物表
            //AStarPathfinding2D.UpdateDynamicObstacle(gridItem.Position, instance.GridMap,isAdd);
        }
        /// <summary>
        /// 更新动态表
        /// </summary>
        public void UpdateDynamicObstacleReal(Node gridItem, bool isAdd = true)
        {
            if (gridItem == null) return;
            //更新动态障碍物表
            AStarPathfinding2D.UpdateDynamicObstacle(gridItem.Position, instance.GridMap, isAdd);
        }
        /// <summary>
        /// 玩家离开地图时使用
        /// </summary>
        /// <param name="unit"></param>
        public void RemoveUnitFromPos(UnitBase unit)
        {
            if (unit == null) return;
            if (!instance.UnitPosDict.ContainsKey(unit)) return;
            
            Node gridItem = instance.UnitPosDict[unit];
            instance.UnitPosDict.Remove(unit);
            if (gridItem != null)
            {
                if (instance.PosUnitDict.ContainsKey(gridItem))
                    instance.PosUnitDict.Remove(gridItem);
                //AStarPathfinding2D.UpdateDynamicObstacle(gridItem.Position, instance.GridMap, false);
            }

        }

        private void RemoveUnitAction(UnitBase unit)
        {
            if (unit == null) return;
            if (instance.UnitCoroutinesDict.ContainsKey(unit))
            {
                if (instance.UnitCoroutinesDict[unit].Item2 != null)
                    World.StopCoroutine(instance.UnitCoroutinesDict[unit].Item2);
            }
        }
        public Node GetGridItemByUnit(UnitBase unitBase)
        {
            Node res = null;
            if(unitBase == null) return null;
            if (instance.UnitPosDict.TryGetValue(unitBase, out res)) return res;
            else return null;
        }
        public Vector3 GetPosByUnit(UnitBase unitBase)
        {
            if(unitBase==null)return Vector3.forward*-1000;
            Node node=GetGridItemByUnit(unitBase);
            if(node==null) return Vector3.forward * -1000;
            return AStarPathfinding2D.GetNodeWorldPositionV3(node.Position, instance.GridMap);
        }
        public UnitBase GetUnitByGridItem(Node gridItem)
        {

            UnitBase res = null;
            if (gridItem == null || instance.PosUnitDict.TryGetValue(gridItem, out res)) return res;
            else return null;
        }

        //public  float CalculateTwoUnitDistance(UnitBase unit1,UnitBase unit2)
        //{
        //    return AStarPathfinding2D.GetDistance(GetGridItemByUnit(unit1),GetGridItemByUnit(unit2),instance.GridMap);
        //}

        /// <summary>
        /// 返回True,表明F1可以打断F2的行动，否则不行
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static bool ActionPriorityLevel(FSM_State f2)
        {
            if (f2 == FSM_State.idle || f2 == FSM_State.walk || f2 == FSM_State.run)
                return true;
            else
                return false;
        }
        public  bool IsUnitCanMove(UnitBase unit)
        {
            if (!instance.UnitCoroutinesDict.ContainsKey(unit)) return true;
            return ActionPriorityLevel(instance.UnitCoroutinesDict[unit].Item1);
        }


        public void UnitTakeAction(UnitBase unit,FSM_State fSM_State,IEnumerator coroutine,UnitBase target=null)
        {
            if (unit != null && coroutine != null)
            {


                if (!instance.UnitCoroutinesDict.ContainsKey(unit))
                {
                    LegOrgan leg = unit.FindOrganInBody<LegOrgan>(ComponentType.leg);
                    if (leg != null)
                    {
                        instance.MoveSystem.SetUnitLeg(leg, target);
                    }
                    instance.UnitCoroutinesDict.Add(unit, (fSM_State, unit.StartCoroutine(coroutine)));

                }
                else
                {
                    if (ActionPriorityLevel(instance.UnitCoroutinesDict[unit].Item1))
                    {
                        UnitStopAction(unit);
                        UnitTakeAction(unit, fSM_State, coroutine);
                    }
                }

            }

        }

        public void UnitStopAction(UnitBase unit)
        {
            if(unit!=null&& instance.UnitCoroutinesDict.ContainsKey(unit))
            {
                if(instance.UnitCoroutinesDict[unit].Item2!=null)
                    unit.StopCoroutine(instance.UnitCoroutinesDict[unit].Item2);
                instance.UnitCoroutinesDict.Remove(unit);

                LegOrgan leg = unit.FindOrganInBody<LegOrgan>(ComponentType.leg);
                if (leg != null)
                {
                    leg.CanFindPath = true;
                }
            }

        }
        //public FSM_State GetUnitAction(UnitBase unitBase)
        //{
        //    coroutine = null;
        //    if (unitBase == null) return FSM_State.max;
        //    if (!instance.UnitCoroutinesDict.ContainsKey(unitBase)) return FSM_State.idle;
        //    coroutine = instance.UnitCoroutinesDict[unitBase].Item2;
        //    return instance.UnitCoroutinesDict[unitBase].Item1;
        //}
        public FSM_State GetUnitAction(UnitBase unitBase)
        {
            if (unitBase == null) return FSM_State.max;
            if (!instance.UnitCoroutinesDict.ContainsKey(unitBase)) return FSM_State.idle;
            //if (!instance.UnitTimerHash.Contains(unitBase))
            //{
            //    CharacterFSM characterFSM = unitBase.GetComponent<CharacterFSM>();
            //    if (characterFSM != null && characterFSM.CurrentState == FSM_State.idle)
            //    {
            //        Coroutine coroutine = instance.UnitCoroutinesDict[unitBase].Item2;
            //        instance.UnitTimerHash.Add(unitBase);
            //        instance.TimerManagerSystem.AddTimer(() =>
            //        {
            //            if (unitBase == null) return;
            //            if (instance.UnitTimerHash.Contains(unitBase)) instance.UnitTimerHash.Remove(unitBase);
            //            if (instance.UnitCoroutinesDict[unitBase].Item2 == coroutine && characterFSM.CurrentState == FSM_State.idle)
            //            {
            //                UnitStopAction(unitBase);
            //            }
            //        }, 1f);
            //    }
            //}

            return instance.UnitCoroutinesDict[unitBase].Item1;
        }
        internal void RemoveUnitFromGame(UnitBase unitBase,float delay)
        {
            if (unitBase == null||unitBase.enabled==false) return;
            GameObject go = unitBase.gameObject;
            if (delay <= Time.deltaTime) delay = Time.deltaTime;


            RemoveUnitAction(unitBase);
            PlayerMemeber playerMemeber = unitBase.FindOrganInBody<BodyOrgan>(ComponentType.body).OwnerPlayer;
            instance.PlayersUnitsDict[playerMemeber].Remove(unitBase);

            if (instance.PlayersUnitsDict[playerMemeber].Count == 0)
                OnPlayerFailEvent?.Invoke(playerMemeber);

            UpdateDynamicObstacleReal(GetGridItemByUnit(unitBase), false);
            RemoveUnitFromPos(unitBase);
            instance.TimerManagerSystem.AddTimer(() => {
                Debug.Log("准备销毁" + go.name);
                FightLog.Record($"玩家:{CampManager.GetPlayerEnum(playerMemeber)} 单位:{unitBase.gameObject.name},被移除出游戏");
                InstanceFinder.GetInstance<NormalUtility>().Server_DespawnAndRecycleModel(go, go.name);
                /*go.SetActive(false);*/
            }, delay);


            unitBase.DestoryAllOrgans();
            unitBase.enabled = false;
            InstanceFinder.GetInstance<NormalUtility>().Server_UnitDead(go);
            GameObject.Destroy(unitBase);
            GameObject.Destroy(go.GetComponent<CharacterFSM>());
            //instance.UnitDeadQeueue.Enqueue(unitBase);
            //解除所以对这些器官的引用

            //这里写判断游戏是否已经结束的事件
        }
        /// <summary>
        /// 两具肉体素质比较，主要比较生命，防御
        /// 如果A比B弱返回-1，否则1
        /// </summary>
        /// <param name="unitA"></param>
        /// <param name="unitB"></param>
        /// <returns></returns>
        public static int UnitCompare(UnitBase unitA,UnitBase unitB)
        {
            BodyOrgan bodyOrganA = unitA.FindOrganInBody<BodyOrgan>(ComponentType.body);
            BodyOrgan bodyOrganB = unitB.FindOrganInBody<BodyOrgan>(ComponentType.body);
            if (bodyOrganA == null) return -1;
            if(bodyOrganB == null) return 1;
            return bodyOrganA.Health_Curr.CompareTo(bodyOrganB.Health_Curr);


        }
        public bool TargetFiltration(Node nodeA, Node nodeB, TargetType targetType)
        {
            // bool res = SystemUtility.TargetFiltration(GetUnitByGridItem(nodeA), GetUnitByGridItem(nodeB), targetType);
            //Debug.Log((nodeA!=null).ToString()+"ss"+(nodeB!=null).ToString() + "SSS");
            if (nodeA == null || nodeB == null) return false;
            return SystemUtility.TargetFiltration(GetUnitByGridItem(nodeA), GetUnitByGridItem(nodeB), targetType);
        }
        public UnitMainSystem() : base() 
        { 
        }

        public override void Start()
        {
            base.Start();
            instance.MoveSystem = world.FindSystem<UnitMoveSystem>();
            instance.AttackSystem = world.FindSystem<UnitAttackSystem>();
            instance.TimerManagerSystem=world.FindSystem<TimerManagerSystem>();
            //world.StartCoroutine(IE_Main());
        }
        //public  UnitBase SwapnUnit(Transform tr,Vector3 pos,Projectile projectile,float attackRange,float warningRange,PlayerMemeber player)
        //{
        //    Node node = AStarPathfinding2D.GetWorldPositionNode(pos, instance.GridMap);
        //    //Debug.Log(node + "1a1");

        //    node = AStarPathfinding2D.FindNearestNode(AStarPathfinding2D.GetNode(node.Position, instance.GridMap), 10, (u, c) => { return true; }, instance.GridMap, true, true);
        //    CharacterFSM characterFSM=tr.gameObject.AddComponent<CharacterFSM>();
        //    UnitBase unit=tr.gameObject.AddComponent<UnitBase>();
        //    world.FindSystem<UnitBodySystem>().SetUnitPlayer(unit.AddOrgan<BodyOrgan>(ComponentType.body), player);
        //    AttackOrgan attackOrgan= unit.AddOrgan<AttackOrgan>(ComponentType.attack);
        //    attackOrgan.AttackRange = attackRange;
        //    attackOrgan.WarningRange=warningRange;
        //    instance.AttackSystem.ChangeWeapon(attackOrgan, projectile);
                
        //    unit.AddOrgan<LegOrgan>(ComponentType.leg);
        //    unit.AddOrgan<UIShowOrgan>(ComponentType.uIShow);
        //    AddUnitToPos(unit,node);
        //    return unit;
        //}
        ///// <summary>
        ///// 临时使用
        ///// </summary>
        ///// <param name="testUnitModel"></param>
        ///// <param name="player"></param>
        ///// <returns></returns>
        //public UnitBase InitUnit(TestUnitModel testUnitModel, PlayerMemeber player)
        //{
        //    return Sp
        //}
        //public UnitBase InitUnit(string unitName, PlayerMemeber player)
        //{
        //    //根据名字查表得到数据
        //    return null;
        //}
        public UnitBase[] GetAllEnemy(PlayerMemeber player)
        {
            List<UnitBase> unitBases = new List<UnitBase>();
            foreach(var v in instance.PlayersUnitsDict)
            {
                if (SystemUtility.RelationOfTwoGrids(player, v.Key) == CampRelation.hostile)
                {
                    foreach(var u in v.Value)
                    {
                        if(u!=null&&unitBases.Contains(u)==false)
                        unitBases.Add(u);
                    }
                }
            }
            return unitBases.ToArray();
        }
        public int GetNumberUnitOfMine(PlayerMemeber player)
        {
            if (instance.PlayersUnitsDict.ContainsKey(player))
                return instance.PlayersUnitsDict[player].Count;
            else
                return 0;
        }
        public UnitBase[] GetAllOfMine(PlayerMemeber player)
        {
            if (instance.PlayersUnitsDict.ContainsKey(player))
                return instance.PlayersUnitsDict[player].ToArray();
            else
                return null;
        }
        public UnitBase[] GetAllUnits()
        {
            List<UnitBase> unitBases = new List<UnitBase>();
            foreach (var v in instance.PlayersUnitsDict)
            {
                    foreach (var u in v.Value)
                    {
                        if (u != null && unitBases.Contains(u) == false)
                            unitBases.Add(u);
                    }
                }           
        
            return unitBases.ToArray();
        }
        public static PlayerEnum GetUnitBelongPlayer(UnitBase unit)
        {
            if (unit == null) return default;
            return  CampManager.GetPlayerEnum(unit.FindOrganInBody<BodyOrgan>(ComponentType.body)?.OwnerPlayer);
        } 
        //public UnitBase SwapnUnit(int heroID, Vector3 pos, PlayerEnum playerEnum)
        //{
        //    Hero hero = LubanMgr.GetHeroData(heroID);

        //    if (hero == null) return null;
        //    PlayerMemeber player = CampManager.GetPlayerMemeber(playerEnum);
        //    Node node = AStarPathfinding2D.GetWorldPositionNode(pos, instance.GridMap);
        //    //加载模型
        //    GameObject heroModel = ABUtility.Load<GameObject>(ABUtility.UnitMainName + hero.HeroPrefab);

        //    //node = AStarPathfinding2D.FindNearestNode(AStarPathfinding2D.GetNode(node.Position, instance.GridMap), 10, (u, c) => { return true; }, instance.GridMap, true, true);
        //    CharacterFSM characterFSM = heroModel.gameObject.AddComponent<CharacterFSM>();
        //    UnitBase unit = heroModel.gameObject.AddComponent<UnitBase>();
        //    BodyOrgan bodyOrgan = unit.AddOrgan<BodyOrgan>(ComponentType.body);
        //    AttackOrgan attackOrgan = unit.AddOrgan<AttackOrgan>(ComponentType.attack);
        //    //技能要放在比较后面
        //    if (testUnitModel.activeSkill != "")
        //    {
        //        MagicOrgan magicOrgan = unit.AddOrgan<MagicOrgan>(ComponentType.magic);
        //        world.FindSystem<UnitSpellSystem>().GainSkill(magicOrgan, testUnitModel.activeSkill);
        //    }
        //    else if (testUnitModel.passiveSkills != null && testUnitModel.passiveSkills.Length > 0)
        //    {
        //        TalentOrgan talentOrgan = unit.AddOrgan<TalentOrgan>(ComponentType.talent);
        //        foreach (var v in testUnitModel.passiveSkills)
        //        {
        //            world.FindSystem<UnitTalentSystem>().GainSkill(talentOrgan, v);

        //        }
        //    }

        //    //MagicSkillSystem
        //    unit.transform.position = AStarPathfinding2D.GetNodeWorldPositionV3(node.Position, instance.GridMap);
        //    UIShowOrgan uIShowOrgan = unit.AddOrgan<UIShowOrgan>(ComponentType.uIShow);
        //    uIShowOrgan.UnitHight = testUnitModel.hight;
        //    //world.FindSystem<UnitUIShowSystem>().InitShow(uIShowOrgan);

        //    bodyOrgan.Health_Max = testUnitModel.health_Max;
        //    bodyOrgan.Health_Curr = testUnitModel.health_Curr;
        //    bodyOrgan.Def = testUnitModel.def;

        //    world.FindSystem<UnitBodySystem>().SetUnitPlayer(bodyOrgan, player);
        //    attackOrgan.AttackRange = testUnitModel.attackRange;
        //    attackOrgan.WarningRange = testUnitModel.warningRange;
        //    attackOrgan.AttackAnimationLong = testUnitModel.attackAnimationLong;
        //    attackOrgan.OriginAttackVal = testUnitModel.attackVal;
        //    attackOrgan.AttackTime = testUnitModel.attackTime;

        //    instance.AttackSystem.ChangeWeapon(attackOrgan, testUnitModel.projectile);
        //    instance.AttackSystem.ChangeFsm(attackOrgan);
        //    unit.AddOrgan<LegOrgan>(ComponentType.leg);//.MoveSpeed=testUnitModel.moveSpeed;
        //    AddUnitToPos(unit, node);
        //    UpdateDynamicObstacle(node);

        //    if (instance.UnitCreateActionDict.ContainsKey(unit.gameObject))
        //    {
        //        instance.UnitCreateActionDict[unit.gameObject]?.Invoke(unit);
        //        instance.UnitCreateActionDict.Remove(unit.gameObject);
        //    }
        //    if (!instance.PlayersUnitsDict.ContainsKey(player))
        //        instance.PlayersUnitsDict.Add(player, new List<UnitBase>());
        //    instance.PlayersUnitsDict[player].Add(unit);

        //    return unit;
        //}

        public void SwapnUnit(TestUnitModel testUnitModel,Action<UnitBase> finishAction=null)
        {
            if (!instance.UnitCreateQueue.Contains(testUnitModel.gameObject))
            {
                instance.UnitCreateQueue.Enqueue(testUnitModel.gameObject);
                if (finishAction != null)
                    instance.UnitCreateActionDict.Add(testUnitModel.gameObject, finishAction);

            }
        }
        /// <summary>
        /// 有些属性可能是由客户端来初始化的
        /// </summary>
        /// <param name="go"></param>
        private void ClientSpawnUnit(GameObject go)
        {
            InstanceFinder.GetInstance<NormalUtility>().ORPC_SpawnUnit(go);
        }
        private UnitBase SwapnUnitReal(TestUnitModel testUnitModel)
        {
            if (testUnitModel == null) return null;


            ClientSpawnUnit(testUnitModel.gameObject);

            PlayerMemeber player = CampManager.GetPlayerMemeber(testUnitModel.player);
            Node node = AStarPathfinding2D.GetWorldPositionNode(testUnitModel.transform.position, instance.GridMap);
            //Debug.Log(node + "1a1");

            //node = AStarPathfinding2D.FindNearestNode(AStarPathfinding2D.GetNode(node.Position, instance.GridMap), 10, (u, c) => { return true; }, instance.GridMap, true, true);
            //if(testUnitModel.gameObject.GetComponent)
            CharacterFSM characterFSM = testUnitModel.gameObject.GetComponent<CharacterFSM>();
            if (characterFSM != null) GameObject.Destroy(characterFSM);
            characterFSM = testUnitModel.gameObject.AddComponent<CharacterFSM>();

            UnitBase unit = testUnitModel.gameObject.AddComponent<UnitBase>();
            BodyOrgan bodyOrgan =unit.AddOrgan<BodyOrgan>(ComponentType.body);
            AttackOrgan attackOrgan = unit.AddOrgan<AttackOrgan>(ComponentType.attack);
            TalentOrgan talentOrgan = unit.AddOrgan<TalentOrgan>(ComponentType.talent);
            MagicOrgan magicOrgan = unit.AddOrgan<MagicOrgan>(ComponentType.magic);
            //技能要放在比较后面
            if (!String.IsNullOrEmpty(testUnitModel.activeSkill))
            {
                magicOrgan.AnimationLong = testUnitModel.spellTime;
                world.FindSystem<UnitSpellSystem>().GainSkill(magicOrgan, testUnitModel.activeSkill);
                Debug.Log(magicOrgan.OwnerUnit + "拥有" + testUnitModel.activeSkill);

            }
            if (testUnitModel.passiveSkills != null && testUnitModel.passiveSkills.Length > 0)
            {
                foreach(var v in testUnitModel.passiveSkills)
                {
                    if(!String.IsNullOrEmpty(v))
                        world.FindSystem<UnitTalentSystem>().GainSkill(talentOrgan, v);
                }
            }

            bodyOrgan.Health_Max = testUnitModel.health_Max;
            bodyOrgan.Health_Curr = testUnitModel.health_Curr;
            bodyOrgan.Def = testUnitModel.def;
            bodyOrgan.Evade = testUnitModel.evade;


            //MagicSkillSystem
            unit.transform.position = AStarPathfinding2D.GetNodeWorldPositionV3(node.Position, instance.GridMap);
            UIShowOrgan uIShowOrgan = unit.AddOrgan<UIShowOrgan>(ComponentType.uIShow);
            uIShowOrgan.UnitHight=testUnitModel.hight;
            //world.FindSystem<UnitUIShowSystem>().InitShow(uIShowOrgan);

     

            world.FindSystem<UnitBodySystem>().SetUnitPlayer(bodyOrgan, player);
            attackOrgan.AttackRange = testUnitModel.attackRange;
            attackOrgan.WarningRange = testUnitModel.warningRange;
            attackOrgan.AttackAnimationLong = testUnitModel.attackAnimationLong;
            attackOrgan.OriginAttackVal = testUnitModel.attackVal;
            attackOrgan.AttackTime = testUnitModel.attackTime;
            attackOrgan.AttackSpeed = testUnitModel.attackSpeed;
            attackOrgan.AttackHitrate = testUnitModel.attackHitrate;
            attackOrgan.AttackCriticalDamage = testUnitModel.attackCriticaldamage;
            attackOrgan.AttackCriticalChance = testUnitModel.attackCriticalchance;


            instance.AttackSystem.ChangeWeapon(attackOrgan, testUnitModel.projectile);
            instance.AttackSystem.ChangeFsm(attackOrgan);
            unit.AddOrgan<LegOrgan>(ComponentType.leg).MoveSpeed=testUnitModel.moveSpeed;//.MoveSpeed=testUnitModel.moveSpeed;
            AddUnitToPos(unit, node);
            UpdateDynamicObstacle(node);


            if (instance.UnitCreateActionDict.ContainsKey(unit.gameObject))
            {
                instance.UnitCreateActionDict[unit.gameObject]?.Invoke(unit);
                instance.UnitCreateActionDict.Remove(unit.gameObject);
            }
            if (!instance.PlayersUnitsDict.ContainsKey(player))
                instance.PlayersUnitsDict.Add(player, new List<UnitBase>());
            instance.PlayersUnitsDict[player].Add(unit);
            testUnitModel.Init(unit,world);

            FightLog.Record($"玩家:{CampManager.GetPlayerEnum(player)} 单位:{unit.gameObject.name},被添加到游戏中{unit.transform.position.ToString()}");


            return unit;
        }

        public override void Update()
        {
            base.Update();
            while (instance.UnitCreateQueue.Count > 0)
            {
                GameObject go = instance.UnitCreateQueue.Dequeue();
                SwapnUnitReal(go.GetComponent<TestUnitModel>());
            }
            //while (instance.UnitDeadQeueue.Count > 0)
            //{
            //    UnityEngine.Object @object = instance.UnitDeadQeueue.Dequeue();
            //    Debug.Log(@object.name + "被销毁！");
            //    GameObject.Destroy(@object);
            //}
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            //AStarPathfinding2D.UpdateDynamicObstacleList(instance.GridMap);
            
        }
        

    }
}

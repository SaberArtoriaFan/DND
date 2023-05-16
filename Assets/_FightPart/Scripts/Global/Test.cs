using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XianXia.Unit;
using Saber.Base;
using Saber.Camp;
using Saber.ECS;
using UnityEngine.UI;
using XianXia.Terrain;
using System.Numerics;
using FishNet;
using FishNet.Object;

namespace XianXia
{
    public class Test : NetworkBehaviour
    {
        //[SerializeField]
        //Vector3 startPos;
        [SerializeField]
        Transform enemysTR;
        [SerializeField]
        Transform playersTR;

        [SerializeField]
        UnityEngine.UI.Button button;
        [SerializeField]
        GameObject allButtonParent;
        private void Start()
        {
            //unit = FindObjectOfType<UnitBase>();
            //if (unit.FindOrganInBody<LegOrgan>(ComponentType.leg) == null)
            //if(button==null)
            //    button=FindObjectOfType<Button>();
            button.onClick.AddListener(() =>
            {
                Test1();
                allButtonParent.SetActive(false);
                button.gameObject.SetActive(false);
            });
            //AStarPathfinding2D map=FindObjectOfType<AStarPathfinding2D>();
            //Vector2Int mapInt= map.allNodeArray[99].Position;
            //UnityEngine.Vector3 pos = AStarPathfinding2D.GetNodeWorldPositionV3(mapInt, map);
            //Debug.Log("v2" + mapInt + "v3"+pos+"_v2"+AStarPathfinding2D.GetWorldPositionNodePos(pos,map));
            //Debug.Log(Application.persistentDataPath+"888");
            //#if UNITY_ANDROID
            //Test1();
            //#endif
        }
        [Button]
        [ServerRpc(RequireOwnership =false)]
        void Test1()
        {
            InstanceFinder.GetInstance<TimingSystemUI>().StartTimer(180, 30);
            UnitMainSystem mainSystem = GameManager.MainWorld.FindSystem<UnitMainSystem>();
            Projectile projectile = new Projectile("Cyclone", 2, 0);
            Projectile temp;


            playersTR.gameObject.SetActive(true);
            enemysTR.gameObject.SetActive(true);

            TestUnitModel[] players=playersTR.GetComponentsInChildren<TestUnitModel>();
            TestUnitModel[] enemy = enemysTR.GetComponentsInChildren<TestUnitModel>();

            for (int i=0;i<enemy.Length;i++)
            {
                TestUnitModel unit = enemy[i];
                unit.player = PlayerEnum.monster;
                mainSystem.SwapnUnit(unit);
                
            }
            for (int i = 0; i < players.Length; i++)
            {
                TestUnitModel unit = players[i];
                unit.player = PlayerEnum.player;

                mainSystem.SwapnUnit(unit);
            }
        }
        private void Update()
        {
            //if(Input.GetMouseButtonDown(0))
            //{
            //    Vector3 pos = Input.mousePosition;


            //    GameManager.MainWorld.FindSystem<UnitMoveSystem>().MoveTo(unit.FindOrganInBody<LegOrgan>(ComponentType.leg), Camera.main.ScreenToWorldPoint(pos), GameManager.MainMap);
            //    Debug.Log("鼠标位置" + Camera.main.ScreenToWorldPoint(pos));
            //}
            //if (Input.GetMouseButtonDown(1))
            //{
            //    Vector3 pos = Input.mousePosition;
            //   // unit.Attack();
            //    Debug.Log("鼠标位置" + Camera.main.ScreenToWorldPoint(pos));
            //}
        }
    }
}

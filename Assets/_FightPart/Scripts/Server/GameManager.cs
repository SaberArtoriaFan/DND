using FishNet;
using Saber.Camp;
using Saber.ECS;
using UnityEngine;
using XianXia.Terrain;
using XianXia.Unit;

namespace XianXia
{
    public class GameManager :GameManagerBase, IStartAfterNetwork
    {
        public static GameManager NewInstance=>(GameManager)Instance;
        [SerializeField]
        AStarPathfinding2D gridMap;
        [SerializeField]
        PlayerEnum myPlayerEnum;
        PlayerMemeber neutralPlayer;
        PlayerMemeber hostilePlayer;
        PlayerMemeber player1;
        PlayerMemeber eastPlayer;
        Camp neutralCamp;
        Camp hostileCamp;
        Camp playerCamp1;
        Camp eastCamp;

        float startPos = 0;
        public PlayerMemeber NeutralPlayer { get => neutralPlayer; }
        public PlayerMemeber HostilePlayer { get => hostilePlayer; }
        public Camp NeutralCamp { get => neutralCamp; }
        public Camp HostileCamp { get => hostileCamp; }
        public PlayerMemeber Player1 { get => player1; }
        public PlayerMemeber EastPlayer { get => eastPlayer; }
        public Camp PlayerCamp1 { get => playerCamp1; }
        public Camp EastCamp { get => eastCamp; }

        public static AStarPathfinding2D MainMap => NewInstance.gridMap;
        public static WorldBase MainWorld=>NewInstance.world;

        public PlayerEnum MyPlayerEnum { get => myPlayerEnum; }

        private PlayerMemeber GetPlayerMemeber(PlayerEnum playerEnum)
        {
            switch (playerEnum)
            {
                case PlayerEnum.monster:
                    return eastPlayer;
                case PlayerEnum.player:
                    return player1;
                default:
                    return null;
            }
        }
        private PlayerEnum GetPlayerEnum(PlayerMemeber player)
        {
            if (player == player1) return PlayerEnum.player;
            else if (player == eastPlayer) return PlayerEnum.monster;
            else return default;
        }
        void GiveGameResult(PlayerMemeber playerMemeber)
        {
            UnitMainSystem unitMainSystem = FindSystem<UnitMainSystem>();
            PlayerMemeber[] playerMemebers = playerMemeber.BelongCamp.GetAllPlayers();
            foreach(var v in playerMemebers)
            {
                if (unitMainSystem.GetNumberUnitOfMine(v) > 0) return;
            }
            FightServerClient.ConsoleWrite_Saber("Tigger Game over ，reason:UnitDead", System.ConsoleColor.DarkYellow);
            ChangeGameStatus(GameStatus.Finish);
        }
        protected override void Start()
        {
            base.Start();
            if (CampManager.Instance != null)
            {
                CampManager.Instance.GetPlayerFunc = GetPlayerMemeber;
                CampManager.Instance.GetPlayerEnumFunc = GetPlayerEnum;

            }
            if (gridMap == null) { gridMap = FindObjectOfType<AStarPathfinding2D>(); }
            hostilePlayer = new PlayerMemeber(null, Color.black, true);
            hostileCamp = new Camp(hostilePlayer, CampRelation.hostile);
            neutralPlayer = new PlayerMemeber(null, Color.grey, true);
            neutralCamp = new Camp(neutralPlayer, CampRelation.neutral);
            player1 = new PlayerMemeber(null, Color.green, false);
            playerCamp1 = new Camp(player1, CampRelation.hostile);
            eastPlayer = new PlayerMemeber(null, Color.red, true);
            eastCamp = new Camp(eastPlayer, CampRelation.hostile);
            myPlayerEnum = GetPlayerEnum(player1);
            FindSystem<UnitMainSystem>().OnPlayerFailEvent += GiveGameResult;


            InstanceFinder.GetInstance<TimingSystemUI>().OnGameOver += () => ChangeGameStatus(GameStatus.Finish);
            InstanceFinder.GetInstance<NormalUtility>().OnStartAfterNetwork += StartAfterNetwork;
            if (world != null && isPasuedWhenFinishGame) OnFinishGameEvent += () =>
            {
                FindSystem<UnitAttackSystem>()?.SetEnable(false);
                FindSystem<UnitSpellSystem>()?.SetEnable(false);
                UnitMoveSystem unitMoveSystem = FindSystem<UnitMoveSystem>();
                unitMoveSystem?.SetEnable(false);
                unitMoveSystem?.StopAll();
                InstanceFinder.GetInstance<TimingSystemUI>().StopTimer();

            };

            TimerManager.instance.AddTimer(() => XianXiaControllerInit.Request_CloseApplication(new string[] { $"游戏已结束,结果为{GameManager.instance._GameResult}" }), 10);


#if UNITY_SERVER
            //Console.WriteLine("Saber:Enter GameScene");
#else

#endif

            //UnitMoveSystem unitMoveSystem = world.FindSystem<UnitMoveSystem>();
            //unitMoveSystem.SetPlayerTarget(player1, startPos + gridMap.MapWidth * 1/6);
        }

        public void StartAfterNetwork()
        {
            //联网后初始化
            FightServerClient.ConsoleWrite_Saber("MainWord Init After Network ");
            MainWorld.StartAfterNetwork();
        }

        public override GameResult SetGameResult()
        {
            //判断时间，时间走完了就输
            if (TimingSystemUI.TimeRemaining == 0)
            {
                FightServerClient.ConsoleWrite_Saber($"Because TimeRemaining is 0，player fail！", System.ConsoleColor.Yellow);
                return GameResult.Fail;
            }

            //判断还有没有人剩下
            UnitMainSystem unitMainSystem = FindSystem<UnitMainSystem>();
            PlayerMemeber[] playerMemebers = GetPlayerMemeber(myPlayerEnum).BelongCamp.GetAllPlayers();
            bool isAlive = false;
            foreach(var v in playerMemebers)
            {
                if (unitMainSystem.GetNumberUnitOfMine(v) > 0)
                {
                    isAlive = true;
                    break;
                }
            }
            if (isAlive)
            {
                FightServerClient.ConsoleWrite_Saber($"Because fight win，player win！",System.ConsoleColor.Yellow);

                return GameResult.Success;
            }
 
            else
            {
                FightServerClient.ConsoleWrite_Saber($"Because fight fail，player fail！", System.ConsoleColor.Yellow);
                return GameResult.Fail;

            }
        }
    }
}

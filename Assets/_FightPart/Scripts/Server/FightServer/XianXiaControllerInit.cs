using FishNet;
using System;
using System.Collections;
using UnityEngine;
using FishNet.Transporting;
using Proto;
using FishNet.Connection;

namespace XianXia
{
    public static class XianXiaControllerInit 
    {
        //static Timer timer;
        public static void InitControllerManager(ControllerManager controllerManager)
        {
            controllerManager.AddRespondHandle(ActionCode.ReadyFightAction, Respond_StartFightFishNetServer);
            controllerManager.AddResultHandle(ActionCode.Login, ReturnCode.Fail, Respond_Fail_Login);
            //controllerManager.AddRespondHandle(ActionCode.BreakFight, Respond_BreakFight);
            //controllerManager.AddRespondHandle(ActionCode.Login, Respond_Login);
            //controllerManager.AddResultHandle(ActionCode.Login, ReturnCode.Success, Respond_Fail_Login);
        }
        /// <summary>
        /// 不应该收到打断战斗的消息，因为要关闭直接关闭了
        /// </summary>
        /// <param name="obj"></param>
        private static void Respond_BreakFight(MainPack obj)
        {
            //Request_CloseApplication();
        }


        public static void Respond_StartFightFishNetServer(MainPack mainPack)
        {
            FightServerManager.ConsoleWrite_Saber("收到开启客户端消息，等待Fishnet初始化中");
            FightServerManager.Instance.StartCoroutine(WaitForFishNetInit(()=>StartFightFishNetServer(mainPack)));
        }
        /// <summary>
        /// 开启战斗服务器
        /// </summary>
        /// <param name="mainPack"></param>
        static void StartFightFishNetServer(MainPack mainPack)
        {
            InstanceFinder.ServerManager.SetStartOnHeadless(false);
            //if (timer != null) timer.Stop();
            //timer = null;

            if (mainPack.IpAndPortPack == null || mainPack.IpAndPortPack.Port==0) { mainPack.ReturnCode = ReturnCode.Fail; return; }
            FightServerManager.ConsoleWrite_Saber($"Receive IP{mainPack.IpAndPortPack.Ip}andPort{mainPack.IpAndPortPack.Port},StartConnection", ConsoleColor.Green);

            InstanceFinder.NetworkManager.TransportManager.Transport.SetServerBindAddress("Any", FishNet.Transporting.IPAddressType.IPv4);
            InstanceFinder.NetworkManager.TransportManager.Transport.SetPort((ushort)mainPack.IpAndPortPack.Port);
            InstanceFinder.ServerManager.StartConnection();

            Action<ServerConnectionStateArgs> action=null;
            action= (s) =>
            {
                if (s.ConnectionState == FishNet.Transporting.LocalConnectionState.Started) 
                {
                    mainPack.ReturnCode = ReturnCode.Success; 
                    InstanceFinder.ServerManager.OnServerConnectionState -= action;
                    //注册事件，在任何时候服务器关闭时上传消息，并告知关闭原因
                    InstanceFinder.ServerManager.OnServerConnectionState += Request_BreakFight;
                    FightServerManager.Instance.SetFightIPAndPort(mainPack.IpAndPortPack.Ip, (ushort)mainPack.IpAndPortPack.Port);
                    //传给GameManager进行初始化场地，各项模组
                    //监听玩家上线，上线后开始战斗
                    //如果十秒内玩家没有上限则自动关闭
                    #region 如果一定时间内没有玩家登陆就关闭
                    NormalUtility normalUtility = InstanceFinder.GetInstance<NormalUtility>();
                    Timer l_timer =null;
                    l_timer= TimerManager.Instance.AddTimer(() =>
                    {
                        if (InstanceFinder.ServerManager.Clients.Count <= 0)
                        {
                            FightServerManager.ConsoleWrite_Saber($"Long than 15s no Client Enter!!!",ConsoleColor.Red);
                            InstanceFinder.ServerManager.StopConnection(true);
                        }
                    }, 15f);
                    #endregion
                    #region 如果玩家离线则关闭
                    Action<NetworkConnection,RemoteConnectionStateArgs> action2 = null;
                    action2 = (n,r) =>
                    {
                        //FightServerClient.ConsoleWrite_Saber($"....didididid");

                        if (r.ConnectionState == RemoteConnectionState.Stopped)
                        {
                            FightServerManager.ConsoleWrite_Saber($"Client stopConnect");
                            InstanceFinder.ServerManager.StopConnection(false);
                        }    
                    };
                    InstanceFinder.ServerManager.OnRemoteConnectionState += action2;
                    #endregion



                }
                else if (s.ConnectionState == FishNet.Transporting.LocalConnectionState.Stopped) 
                { 
                    mainPack.ReturnCode = ReturnCode.Fail; 
                    InstanceFinder.ServerManager.OnServerConnectionState -= action;
                    mainPack.Info.Add("Create Fight Server Error，Please Check Port！！！");

                    FightServerManager.ConsoleWrite_Saber("Create Fight Server Error，Please Check Port！！！");
                    Request_CloseApplication(ReturnCode.Zero,new string[] { "Create Fight Server Error，Please Check Port！！！" });
                }

            };
            InstanceFinder.ServerManager.OnServerConnectionState += action;
        }

        static IEnumerator WaitForFishNetInit(Action action)
        {
            WaitUntil waitUntil = new WaitUntil(() =>
              {
                  Debug.Log($"等待 NetworkManager:{InstanceFinder.NetworkManager != null},is Initial {InstanceFinder.NetworkManager?.Initialized}");
                  return InstanceFinder.NetworkManager != null && InstanceFinder.NetworkManager.Initialized;
              });
            yield return waitUntil;
            Debug.Log($"等待 结束，开始连接");

            action?.Invoke();

        }

        /// <summary>
        /// 登录失败直接注销
        /// </summary>
        /// <param name="mainPack"></param>
        static void Respond_Fail_Login(MainPack mainPack)
        {
            FightServerManager.ConsoleWrite_Saber("登录失败");

            //if (timer == null) return;
            //timer.Stop();
            //timer = null;
            mainPack.ReturnCode = default;
            Request_CloseApplication(ReturnCode.Zero);
            //Application.Quit();
        }
        ///// <summary>
        ///// 多次发送直到目标接收到
        ///// </summary>
        ///// <param name="mainPack"></param>
        //public static void Excess_LoginRequest()
        //{


        //    Timer timer2 = null;
        //    timer2 = TimerManager.Instance.AddTimer(() =>
        //    {
        //        if (FightServerManager.Instance.IsSocketActive && InstanceFinder.IsOffline)
        //        {
        //            timer2.Stop();
        //        }
        //    }, Time.deltaTime*3,true);
        //}
        static void Request_BreakFight(ServerConnectionStateArgs serverConnectionStateArgs)
        {
            if (serverConnectionStateArgs.ConnectionState != LocalConnectionState.Stopped) return;
            //可以根据游戏状态判定，如果正常关闭返回正常
            //如果非正常，返回错误日志
            //返回战斗日志
            FightServerManager.ConsoleWrite_Saber("Fight server(this) is Offline");
            //正常结束返回success
            //mainPack.ReturnCode=ReturnCode.Success;
            InstanceFinder.ServerManager.OnServerConnectionState -= Request_BreakFight;
            //关闭游戏进程，归还端口
            GameManager gameManager = GameManager.NewInstance;
            ReturnCode returnCode ;
            if (gameManager._GameStatus == Saber.ECS.GameManagerBase.GameStatus.Finish)
            {
                if (gameManager._GameResult == Saber.ECS.GameManagerBase.GameResult.Fail)
                    returnCode = ReturnCode.Fail;
                else if (gameManager._GameResult == Saber.ECS.GameManagerBase.GameResult.Success)
                    returnCode = ReturnCode.Success;
                else 
                    returnCode = ReturnCode.Zero;
            }
            else
                returnCode = ReturnCode.Zero;
            Request_CloseApplication(returnCode,new string[] { $"Fight Server Close,{0}" } );
        }

        /// <summary>
        /// 向服务器请求端口号开启
        /// </summary>
        public static void Request_Login()
        {
            MainPack mainPack = new MainPack();
            mainPack.ActionCode = ActionCode.Login;
            mainPack.Word = FightServerManager.Instance.ProcessId.ToString();
            FightServerManager.Instance.Send(mainPack);
            //if (timer != null) timer.Stop();
            //timer = TimerManager.Instance.AddTimer(() =>
            //{
            //    Request_CloseApplication();
            //}, 10);

        }
        /// <summary>
        /// 请求服务器关闭自己
        /// </summary>
        public static void Request_CloseApplication(ReturnCode returnCode=ReturnCode.Zero, string[] info = null)
        {
            FightServerManager.ConsoleWrite_Saber("Ask for Close this process");
            MainPack mainPack = new MainPack();
            mainPack.ActionCode = ActionCode.BreakFight;
            mainPack.Word = FightServerManager.Instance.ProcessId.ToString();
            mainPack.ReturnCode = returnCode;
            if (info != null)
            {
                foreach(var v in info)
                {
                    mainPack.Info.Add(v);
                }
            }
            mainPack.Info.Add("Fight Log Start");
            mainPack.Info.Add(FightLog.OutPut());
            mainPack.Info.Add("Fight Log End");
            FightServerManager.Instance.Send(mainPack);
        }



    }
}

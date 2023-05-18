using FishNet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Transporting;
using Proto;
using XianXia.Server;
using FishNet.Managing;
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
            controllerManager.AddRespondHandle(ActionCode.BreakFight, Respond_BreakFight);
            //controllerManager.AddRespondHandle(ActionCode.Login, Respond_Login);
            //controllerManager.AddResultHandle(ActionCode.Login, ReturnCode.Success, Respond_Fail_Login);
        }
        private static void Respond_BreakFight(MainPack obj)
        {
            Request_CloseApplication();
        }


        static void Respond_StartFightFishNetServer(MainPack mainPack)
        {
            FightServerClient.ConsoleWrite_Saber("�յ������ͻ�����Ϣ���ȴ�Fishnet��ʼ����");
            FightServerManager.Instance.StartCoroutine(WaitForFishNetInit(()=>StartFightFishNetServer(mainPack)));
        }
        /// <summary>
        /// ����ս��������
        /// </summary>
        /// <param name="mainPack"></param>
        static void StartFightFishNetServer(MainPack mainPack)
        {
            InstanceFinder.ServerManager.SetStartOnHeadless(false);
            //if (timer != null) timer.Stop();
            //timer = null;
            if (mainPack.IpAndPortPack == null || string.IsNullOrEmpty(mainPack.IpAndPortPack.Ip)) { mainPack.ReturnCode = ReturnCode.Fail; return; }

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
                    //ע���¼������κ�ʱ��������ر�ʱ�ϴ���Ϣ������֪�ر�ԭ��
                    InstanceFinder.ServerManager.OnServerConnectionState += Request_NoticeFightServerClose;
                    FightServerManager.Instance.SetFightIPAndPort(mainPack.IpAndPortPack.Ip, (ushort)mainPack.IpAndPortPack.Port);
                    //����GameManager���г�ʼ�����أ�����ģ��
                    //����������ߣ����ߺ�ʼս��
                    //���ʮ�������û���������Զ��ر�
                    #region ���һ��ʱ����û����ҵ�½�͹ر�
                    NormalUtility normalUtility = InstanceFinder.GetInstance<NormalUtility>();
                    Timer l_timer =null;
                    l_timer= TimerManager.Instance.AddTimer(() =>
                    {
                        Request_CloseApplication();
                    }, 15f);
                    Action<FishNet.Connection.NetworkConnection,RemoteConnectionStateArgs> action1 = null;
                    action1=(n,r) => {
                        //FightServerClient.ConsoleWrite_Saber($"....didididid");

                        if (r.ConnectionState == RemoteConnectionState.Started)
                        {
                            l_timer.Stop();
                            FightServerClient.ConsoleWrite_Saber($"����ҽ��룬ȡ�����ҹر�");
                            InstanceFinder.ServerManager.OnRemoteConnectionState -= action1;
                        }
                    };
                    InstanceFinder.ServerManager.OnRemoteConnectionState += action1;
                    #endregion
                    #region ������������ر�
                    Action<NetworkConnection,RemoteConnectionStateArgs> action2 = null;
                    action2 = (n,r) =>
                    {
                        //FightServerClient.ConsoleWrite_Saber($"....didididid");

                        if (r.ConnectionState == RemoteConnectionState.Stopped)
                        {
                            FightServerClient.ConsoleWrite_Saber($"�пͻ��˶Ͽ������ӶϿ�");
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
                    mainPack.Info.Add("Create Fight Server Error��Please Check Port������");
            
                    FightServerClient.ConsoleWrite_Saber("Create Fight Server Error��Please Check Port������");
                    Request_CloseApplication();
                }

            };
            InstanceFinder.ServerManager.OnServerConnectionState += action;
        }

        static IEnumerator WaitForFishNetInit(Action action)
        {
            WaitUntil waitUntil = new WaitUntil(() =>
              {
                  return InstanceFinder.NetworkManager != null && InstanceFinder.NetworkManager.Initialized;
              });
            yield return waitUntil;

            action?.Invoke();

        }

        /// <summary>
        /// ��¼ʧ��ֱ��ע��
        /// </summary>
        /// <param name="mainPack"></param>
        static void Respond_Fail_Login(MainPack mainPack)
        {
            FightServerClient.ConsoleWrite_Saber("��¼ʧ��");

            //if (timer == null) return;
            //timer.Stop();
            //timer = null;
            mainPack.ReturnCode = default;
            Request_CloseApplication();
            //Application.Quit();
        }
        ///// <summary>
        ///// ��η���ֱ��Ŀ����յ�
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
        static void Request_NoticeFightServerClose(ServerConnectionStateArgs serverConnectionStateArgs)
        {
            if (serverConnectionStateArgs.ConnectionState != LocalConnectionState.Stopped) return;
            //���Ը�����Ϸ״̬�ж�����������رշ�������
            //��������������ش�����־
            //����ս����־

            //������������success
            //mainPack.ReturnCode=ReturnCode.Success;
            InstanceFinder.ServerManager.OnServerConnectionState -= Request_NoticeFightServerClose;
            //�ر���Ϸ���̣��黹�˿�
            Request_CloseApplication(new string[] { $"Fight Server Close,{0}" } );
        }

        /// <summary>
        /// �����������˿ںſ���
        /// </summary>
        public static void Request_StartFightFishNetServer()
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
        /// ����������ر��Լ�
        /// </summary>
        public static void Request_CloseApplication(string[] info=null)
        {
            FightServerClient.ConsoleWrite_Saber("Ask for Close this process");
            MainPack mainPack = new MainPack();
            mainPack.ActionCode = ActionCode.BreakFight;
            mainPack.Word = FightServerManager.Instance.ProcessId.ToString();
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

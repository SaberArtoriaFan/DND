using FishNet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Transporting;

namespace XianXia.Client
{
    public static class Client_ConnectToFightServer 
    {
        /// <summary>
        /// 绑定一些事件，比如失败之后跳出 UI界面告知玩家
        /// </summary>
        /// <param name="success"></param>
        /// <param name="fail"></param>
        public static void SetConnectSuccessEventAndFailEvent(Action success,Action fail)
        {
            Action<ClientConnectionStateArgs> action = null;
            action=(c) =>
        {
            if (c.ConnectionState == LocalConnectionState.Stopped)
            {
                Debug.Log($"连接战斗服务器失败");
                fail?.Invoke();
                InstanceFinder.ClientManager.OnClientConnectionState -= action;
            }
            else if (c.ConnectionState == LocalConnectionState.Started)
            {
                Debug.Log($"连接战斗服务器成功");
                success?.Invoke();
                InstanceFinder.ClientManager.OnClientConnectionState-=action;
            }
        };
            InstanceFinder.ClientManager.OnClientConnectionState += action;
        }

        public static void Connect(string ip,ushort port)
        {
            if (InstanceFinder.TransportManager.Transport.GetConnectionState(false) != LocalConnectionState.Stopped)
            {
                Debug.LogError("客户端已经连接或者正在连接战斗服务器，你确定要继续连接吗？");
                return;
            }

            InstanceFinder.NetworkManager.TransportManager.Transport.SetClientAddress(ip);
            InstanceFinder.NetworkManager.TransportManager.Transport.SetPort(port);
            InstanceFinder.ClientManager.StartConnection();
        }
    }
}

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
        /// ��һЩ�¼�������ʧ��֮������ UI�����֪���
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
                Debug.Log($"����ս��������ʧ��");
                fail?.Invoke();
                InstanceFinder.ClientManager.OnClientConnectionState -= action;
            }
            else if (c.ConnectionState == LocalConnectionState.Started)
            {
                Debug.Log($"����ս���������ɹ�");
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
                Debug.LogError("�ͻ����Ѿ����ӻ�����������ս������������ȷ��Ҫ����������");
                return;
            }

            InstanceFinder.NetworkManager.TransportManager.Transport.SetClientAddress(ip);
            InstanceFinder.NetworkManager.TransportManager.Transport.SetPort(port);
            InstanceFinder.ClientManager.StartConnection();
        }
    }
}

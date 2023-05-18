using FishNet;
using FishNet.Component.Animating;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting.Tugboat;
using Saber.Base;
using Saber.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using XianXia;
using XianXia.Unit;
//using network
namespace XianXia
{
    public class NormalUtility : NetworkBehaviour
    {
        static ulong id = 0;
        Utility_ShapeShiftManager shapeShiftManager;
#if UNITY_EDITOR||!UNITY_SERVER
        RisingSpaceUI risingSpaceUI;
        Client_ProjectileManager projectileManager;
        Client_EffectSystem client_Effect;
#endif
#if UNITY_EDITOR || UNITY_SERVER
        //[SerializeField]
        //IStartAfterNetwork gameWorld;
        [SerializeField]
        float timeScale = 2;
        public event Action OnStartAfterNetwork;
        PoolManager poolManager;
        public event Action<NetworkConnection> ClientOfflineEvent;
#endif

        //NetworkUtility networkUtility;

        public event Action<NetworkConnection> OnClientEnterEvent;

        public event Action<GameObject> UnitDeadClientAction;

        public static ulong  GetId() 
        {
            if (id  == ulong.MaxValue)
                id = 0;
            return id++;
        }
        private void Awake()
        {
            if (InstanceFinder.GetInstance<NormalUtility>() != null) { Debug.LogError("Cant Find"); return; }
            InstanceFinder.RegisterInstance(this, false);
            shapeShiftManager = new Utility_ShapeShiftManager();
//#if UNITY_CLIENT
//            Debug.Log("aaaaaaaaaqqqqqqqq");
//#endif
        }
        private void Start()
        {
            //InstanceFinder.ClientManager.OnAuthenticated ;
            //InitClient();
            Debug.Log("8888888888");
#if UNITY_SERVER
            TimerManager.Instance.AddTimer(() => OnStartAfterNetwork?.Invoke(), Time.deltaTime);
#endif


        }
        private void OnDestroy()
        {
            shapeShiftManager.OnDestroy();
            OnClientEnterEvent = null;
            UnitDeadClientAction = null;
        }
        //private void SynchronizeResources(NetworkConnection conn,bool res)
        //{
        //    if (!res) return;
        //    SynchronizeResourcesEvent?.Invoke(conn);
        //    //InstanceFinder.NetworkManager.StartCoroutine(IE_WaitForInit(conn));
        //}
        //private void WaitForInit()
        //{
        //    //InstanceFinder.NetworkManager.st
        //    InstanceFinder.NetworkManager.StartCoroutine(IE_WaitForInit());
        //}
        ///// <summary>
        ///// �ڿͻ��˵��ã��ȴ���ʼ��������������������
        ///// </summary>
        ///// <param name="conn"></param>
        ///// <returns></returns>
        //IEnumerator IE_WaitForInit()
        //{
        //    WaitUntil waitUntil=new WaitUntil(()=> { Debug.Log("111"); return InstanceFinder.GetInstance<NormalUtility>() != null; });
        //    yield return waitUntil;
        //    InitClientWhenInited(ClientManager.Connection);
        //    //SynchronizeResourcesEvent?.Invoke(conn);
        //}
        /// <summary>
        /// �ͻ������Ӻ�������������������
        /// </summary>
        /// <param name="conn"></param>
        [ServerRpc(RequireOwnership =false)]
        void InitClientWhenInited(NetworkConnection conn)
        {
#if UNITY_SERVER
            //Debug.Log("ddd");
            //this.GiveOwnership(conn);
            OnClientEnterEvent?.Invoke(conn);
#endif
        }
        [Server]
        private void InitServer()
        {
#if UNITY_SERVER 

            //InstanceFinder.ServerManager.OnAuthenticationResult += SynchronizeResources;

            poolManager = PoolManager.Instance;
            //gameWorld.StartAfterNetwork();

            //Console.WriteLine($"Saber:Enter GameScene,IP:{InstanceFinder.NetworkManager.TransportManager.Transport.GetServerBindAddress(FishNet.Transporting.IPAddressType.IPv4)};Port:{InstanceFinder.NetworkManager.TransportManager.Transport.GetPort()}");
            Console.WriteLine($"Saber:Enter GameScene,IP:{InstanceFinder.NetworkManager.TransportManager.Transport.GetServerBindAddress(FishNet.Transporting.IPAddressType.IPv4)};Port:{InstanceFinder.NetworkManager.TransportManager.Transport.GetPort()}");
#else
            Debug.Log($"Saber:Enter GameScene,IP:{InstanceFinder.NetworkManager.TransportManager.Transport.GetServerBindAddress(FishNet.Transporting.IPAddressType.IPv4)};Port:{InstanceFinder.NetworkManager.TransportManager.Transport.GetPort()}");
#endif

        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            Debug.Log("9999999");
#if UNITY_SERVER
            InitServer();
            //networkUtility = InstanceFinder.GetInstance<NetworkUtility>();
            //Debug.Log(networkUtility.name + "");
            //GameManager.MainWorld.StartAfterNetwork();
#endif
        }
        public override void OnStopServer()
        {
            base.OnStopServer();
#if UNITY_SERVER
            //if (InstanceFinder.ServerManager != null)
            //    InstanceFinder.ServerManager.OnAuthenticationResult -= SynchronizeResources;
            OnClientEnterEvent = null;
#endif
        }
        public override void OnStartClient()
        {
            base.OnStartClient();
#if !UNITY_SERVER
            Client_InstanceFinder.GetInstance<Client_TimeScaleUI>().OnSetTimeScale+= ServerRpc_SetTimeScale;
            //networkUtility = InstanceFinder.GetInstance<NetworkUtility>();
            Client_InstanceFinder.StartAfterNetwork();
            shapeShiftManager.StartAfterNetwork();
            InitClient();
#endif
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            ServerRpc_ClientOfflineEvent(InstanceFinder.ClientManager.Connection);
        }
        [Client]
        void InitClient()
        {
#if !UNITY_SERVER
            //base.GiveOwnership(ClientManager.Connection);
            TimerManager.Instance.AddTimer(() => { 
                risingSpaceUI = Client_InstanceFinder.GetInstance<RisingSpaceUI>();
                projectileManager = Client_InstanceFinder.GetInstance<Client_ProjectileManager>();
                client_Effect = Client_InstanceFinder.GetInstance<Client_EffectSystem>();
                Debug.Log("Xxx");
                InitClientWhenInited(ClientManager.Connection);
            }, Time.deltaTime);
            Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
            foreach(var v in canvases)
            {
                if (v.name != "MainCanvas")
                    v.gameObject.SetActive(false);
            }
#endif
        }
       

#region ��������
#region ����
#region Spawn
        /// <summary>
        /// ��֤��ȫ����������
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="initNum"></param>
        /// <returns></returns>
        [Server]
        public GameObject Server_SpawnModel(string path,string name,Transform parent,int initNum=0, Action<GameObject> recycleAction = null, Action<GameObject> initAction = null)
        {
#if UNITY_SERVER
            _ = Server_InitSpawnPool(path, name, parent, initNum,recycleAction,initAction);
            return poolManager.GetObjectInPool<GameObject>(name);
#else
            return null;
#endif
        }
        /// <summary>
        /// �ǰ�ȫ����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Server]
        public GameObject Server_SpawnModel(string name)
        {
            //if (!poolManager.IsPoolAlive(name))
            //{
            //    GameObject model = ABUtility.Load<GameObject>(path + name);
            //    poolManager.AddPool<GameObject>(() => { return GameObject.Instantiate(model); }, (u) => { Despawn(u, DespawnType.Pool); u.SetActive(false); }, (u) => { Spawn(u); }, name);
            //}
#if UNITY_SERVER
            return poolManager.GetObjectInPool<GameObject>(name);
#else
            return null;
#endif
        }
        [Server]
        public void Server_DespawnAndRecycleModel(GameObject go,string name)
        {
#if UNITY_SERVER
            if (poolManager.IsPoolAlive(name))
                poolManager.RecycleToPool<GameObject>(go, name);
            else
                Despawn(go, DespawnType.Destroy);
#endif
        }
        [Server]
        public Saber.Base.ObjectPool<GameObject> Server_InitSpawnPool(string path, string name, Transform parent,int initNum = 0,Action<GameObject> recycleAction=null,Action<GameObject>initAction=null)
        {
#if UNITY_SERVER

            if (!poolManager.IsPoolAlive(name))
            {
                GameObject go= AddSpawnablePrefabs(InstanceFinder.NetworkManager, path, name);
                OnClientEnterEvent += (n) => { /*Debug.Log(*//*InstanceFinder.GetInstance<NetworkUtility>().*/TRPC_AddSpawnablePrefabs(n, path, name); } ;
                ORPC_AddSpawnablePrefabs(path,name);
                return poolManager.AddPool<GameObject>(() => { GameObject model = GameObject.Instantiate(go);model.name = name; return model; }, (u) => {  recycleAction?.Invoke(u); Despawn(u, DespawnType.Pool); u.SetActive(false); u.transform.SetParent(parent); }, (u) => { Spawn(u);initAction?.Invoke(u); }, name);
            }
            else
                return poolManager.GetPool<GameObject>(name);
#else
            return null;
#endif

        }
        [Server]
        public void Server_UnitDead(GameObject go)
        {
#if UNITY_SERVER

            if (go == null) return;
            Client_UnitProperty client_UnitProperty = go.GetComponent<Client_UnitProperty>();
            if (client_UnitProperty != null) client_UnitProperty.HealthPointPer = 0;
            ORPC_UnitDeadClientAction(go);
#endif

        }
        //[Server]
        //public void Server_ShapeShift(GameObject go,string modelPath,string name)
        //{

        //}
        [ObserversRpc(RunLocally =true)]
        public void ORPC_ShapeShift(GameObject go, string modelPath, string name)
        {
            //��Ϊ��Ҫͬ�������������Է�����ҲҪ����
            shapeShiftManager.ShapeShift(go, modelPath, name);
        }
        [ObserversRpc]
        private void ORPC_UnitDeadClientAction(GameObject go)
        {
#if !UNITY_SERVER
            //Client_UnitDead();
            UnitDeadClientAction?.Invoke(go);
#endif
        }



        [ObserversRpc]
        private void ORPC_AddSpawnablePrefabs(string path, string name)
        {
#if !UNITY_SERVER

            _ = AddSpawnablePrefabs(InstanceFinder.NetworkManager,path, name);
#endif

        }
        //���������﷢�ͣ���Ϊ�ͻ�����֤���¼���������������ʱ�ýű���û�м���
        [TargetRpc]
        public void TRPC_AddSpawnablePrefabs(NetworkConnection connection, string path, string name)
        {
#if !UNITY_SERVER

            AddSpawnablePrefabs(connection.NetworkManager, path, name);
#endif

        }
        private GameObject AddSpawnablePrefabs(NetworkManager networkManager,string path, string name)
        {
            NetworkObject networkObject = ABUtility.Load<GameObject>(path + name).GetComponent<NetworkObject>();

            networkManager.SpawnablePrefabs.AddObject(networkObject, true);
            return networkObject.gameObject;
            //return networkUtility.AddSpawnablePrefabs(networkManager, path, name);
        }
#endregion
        [Server]
        public Client_UnitProperty Server_AddUnitProperty(GameObject go)
        {
#if UNITY_SERVER
            NetworkObject networkObject = go.GetComponent<NetworkObject>();
            if (networkObject == null) return null;
            Client_UnitProperty unitProperty = networkObject.GetComponent<Client_UnitProperty>();

            //Client_UnitProperty unitProperty =networkObject.AddAndSerialize<Client_UnitProperty>();
            //if (unitProperty == null) unitProperty = go.AddComponent<Client_UnitProperty>();
            ORPC_AddUnitProperty(networkObject);
            return unitProperty;
#else
            return null;
#endif
        }

        [Server]
        public void Server_RemoveUnitProperty(GameObject go)
        {
#if UNITY_SERVER
            NetworkObject networkObject = go.GetComponent<NetworkObject>();
            if (networkObject == null) return;
            ORPC_RemoveUnitProperty(networkObject);
            //Client_UnitPropertyUI client_UnitPropertyUI = go.GetComponent<Client_UnitPropertyUI>();
            //if(client_UnitPropertyUI!=null)
            //    GameObject.Destroy(client_UnitPropertyUI);
#endif
        }
        [ServerRpc(RequireOwnership = false)]
        public void ServerRpc_SetTimeScale(float speed)
        {
#if UNITY_SERVER
            if (timeScale < 0) timeScale = 2;
            if (speed > timeScale) speed = timeScale;
            Time.timeScale = speed;
#endif
        }
        [Server]
        public void Server_SetUnitColor(GameObject go,Color color)
        {
#if UNITY_SERVER
            NetworkObject networkObject = go.GetComponent<NetworkObject>();
            if (networkObject == null) return;
            ORPC_ChangeUnitColor(networkObject, color);
#endif
        }
        //[Server]
        //public void Server_SetAnimatorParameter_Trigger(Client_UnitProperty c, int id)
        //{

        //    ORPC_SetAnimatorParameter_Trigger(c, id);
        //}
        [ObserversRpc]
        public void ORPC_CreateEffect(string effectName, GameObject parent,Vector3 pos, bool isAutoRecycle)
        {
#if !UNITY_SERVER
            if (parent == null) return;
            parent.transform.position = pos;
            client_Effect.CreateEffectInPool_Main(effectName, parent, isAutoRecycle);
#endif
        }
        [ObserversRpc]
        public void ORPC_CreateEffect(string effectName, string key,Vector3 pos, Vector3 rotate = default, Vector3 scale = default, bool isAutoRecycle = false)
        {
#if !UNITY_SERVER
            client_Effect.CreateEffectInPool_Main(effectName, key, pos, rotate, scale, isAutoRecycle);
#endif
            //if (parent == null) return;
        }
        [ObserversRpc]
        public void ORPC_RecycleEffect(GameObject parent)
        {
#if !UNITY_SERVER
            if (parent == null) return;
            client_Effect.RecycleEffect(parent);
#endif
        }
        [ObserversRpc]
        public void ORPC_CreateProjectile(GameObject parent,Vector3 pos,string name)
        {
#if !UNITY_SERVER
            parent.transform.position = pos;
            projectileManager.InitProjectile(parent, name);
#endif
        }
        [ObserversRpc]
        public void ORPC_RecycleProjectile(GameObject parent)
        {
#if !UNITY_SERVER
            if (parent == null) return;
            projectileManager.RecycleProjectile(parent);
#endif
        }
        [ObserversRpc]
        public void ORPC_RecycleEffect(string key)
        {
#if !UNITY_SERVER
            client_Effect.RecycleEffect(key);
#endif
            //if (parent == null) return;
        }
        [ObserversRpc]
        public void ORPC_ShowRisingSpace(string s, Vector3 worldPos, Vector3 dir, Color color = default, int size = 24, FontStyles fontStyles = FontStyles.Normal, float speed = 1, float continueTime = 1.5f)
        {
#if !UNITY_SERVER
            risingSpaceUI.ShowRisingSpace(s, worldPos, dir, color, size, fontStyles, speed, continueTime);
#endif
        }
        [ObserversRpc]
        public void ORPC_SetAnimatorParameter_Trigger(Client_UnitProperty c,int id)
        {
#if !UNITY_SERVER
            if (c != null) c.SetAnimatorParameter_Trigger(id);
#endif
        }

        [ObserversRpc]
        public void ORPC_SpawnUnit(GameObject go)
        {
#if !UNITY_SERVER
            SortingGroup sortingGroup = go.GetComponent<SortingGroup>();
            if (sortingGroup == null)
                sortingGroup = go.gameObject.AddComponent<SortingGroup>();
            sortingGroup.sortingLayerName = "Unit";
            sortingGroup.sortingOrder = 0;
#endif
        }
        //[ObserversRpc]

#endregion
#region ˽��
        [ServerRpc(RequireOwnership =false)]
        private void ServerRpc_ClientOfflineEvent(NetworkConnection conn)
        {
#if UNITY_SERVER
            ClientOfflineEvent?.Invoke(conn);

#endif
        }
        //�������
        [ObserversRpc]
        private void ORPC_AddUnitProperty(NetworkObject networkObject)
        {
#if !UNITY_SERVER
            Client_InstanceFinder.GetInstance<Client_UnitPropertyUI>().AddUnitProperty(networkObject);
#endif
        }
        [ObserversRpc]
        private void ORPC_RemoveUnitProperty(NetworkObject networkObject)
        {
#if !UNITY_SERVER
            Client_InstanceFinder.GetInstance<Client_UnitPropertyUI>().RemoveUnitProperty(networkObject);
#endif
        }
        [ObserversRpc]
        private void ORPC_ChangeUnitColor(NetworkObject networkObject, Color color)
        {
#if !UNITY_SERVER
            Client_InstanceFinder.GetInstance<Client_UnitPropertyUI>().SetUnitColor(networkObject, color);
#endif
            //if (go == null) return;
            //NetworkObject networkObject = go.GetComponent<NetworkObject>();
        }
#endregion
#endregion
    }
}

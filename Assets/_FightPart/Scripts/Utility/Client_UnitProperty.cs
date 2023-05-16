using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Saber.Base;
namespace XianXia
{
    public class Client_UnitProperty : NetworkBehaviour,IClient_UnitProperty
    {
        [ShowInInspector]
        [SyncVar(Channel = FishNet.Transporting.Channel.Unreliable,OnChange = "OnHealthChange")]
        private float healthPointPer;

        [ShowInInspector]
        [SyncVar(Channel = FishNet.Transporting.Channel.Unreliable, OnChange = "OnMagicChange")]
        private float magicPointPer;

        [ShowInInspector]
        [SyncVar]
        private float unitHigh;

        [SerializeField]
        HealthMagicPointShowUI healthMagicPointShowUI;
        // Transform target;
        Animator animator;
        private void Start()
        {
            if (IsClient)
                _=ChangeAnimator();
        }
        public float HealthPointPer { get => healthPointPer; set => healthPointPer = value; }
        public float MagicPointPer { get => magicPointPer; set => magicPointPer = value; }
        public float UnitHigh { get => unitHigh; set => unitHigh = value; }
        public HealthMagicPointShowUI HealthMagicPointShowUI { get => healthMagicPointShowUI; set => healthMagicPointShowUI = value; }
        public Animator Animator { get => animator;  }

        [Client]
        public Animator ChangeAnimator()
        {
#if !UNITY_SERVER
            animator = GetComponentInChildren<Animator>();
            return animator;
#else
            return null;
#endif
        }
        [Client]
        public void ChangeAnimator(Animator animator)
        {
#if !UNITY_SERVER

            this.animator = animator;
#endif
        }

        [Client]
        private void OnHealthChange(float pre, float next,bool asServer)
        {
#if !UNITY_SERVER

            //if (asServer||pre==next) return;
            //Debug.Log("¸üÐÂÑªÁ¿"+next);
            if (healthMagicPointShowUI == null) return;
            healthMagicPointShowUI.SetHealthPer(next,false);
            if (next <= 0) OnDisable();
#endif
        }
        [Client]
        private void OnMagicChange(float pre, float next,bool asServer)
        {
#if !UNITY_SERVER
            if (healthMagicPointShowUI==null) return;
            healthMagicPointShowUI.SetMagicPer(next,false);
#endif
        }
        [Client]
        public void SetAnimatorParameter_Bool(int id,bool val)
        {
#if !UNITY_SERVER
            animator.SetBool(id, val);
#endif
        }
        [Client]
        public void SetAnimatorParameter_Float(int id, float val)
        {
#if !UNITY_SERVER

            animator.SetFloat(id, val);
#endif
        }
        [Client]
        public void SetAnimatorParameter_Trigger(int id)
        {
#if !UNITY_SERVER
            if (animator == null||animator.isActiveAndEnabled==false) { _ = ChangeAnimator(); return; }
            animator.ResetTrigger(id);
            animator.SetTrigger(id);
#endif
        }
        [Client]
        private void UpdateUnitProperty()
        {
#if !UNITY_SERVER
            if (healthMagicPointShowUI == null || transform == null) return;
            Vector3 ItemScreenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * unitHigh * transform.localScale.x/*+ Vector3.up * 0.05f * MainCameraCtrl.Instance.scale*/);
            Vector3 RightPos = new Vector3(ItemScreenPos.x, ItemScreenPos.y, 0);
            healthMagicPointShowUI.transform.position = RightPos;
#endif
        }
        //[Client]
        //private static void Client_UnitDead(GameObject go)
        //{
        //    if (go == null) return;
        //    Client_UnitProperty client_UnitProperty = go.GetComponent<Client_UnitProperty>();
        //    if()
        //}
        private void LateUpdate()
        {
            if(IsClient)
                UpdateUnitProperty();

#if Client || UNITY_EDITOR
#endif
        }
        private void OnDisable()
        {
#if !UNITY_SERVER
            if (healthMagicPointShowUI != null&&                        
                Client_InstanceFinder.GetInstance<Client_UnitPropertyUI>() != null&& 
                InstanceFinder.NetworkManager.IsClient)
                Client_InstanceFinder.GetInstance<Client_UnitPropertyUI>().RemoveUnitProperty(gameObject);
#endif
        }
    }
}

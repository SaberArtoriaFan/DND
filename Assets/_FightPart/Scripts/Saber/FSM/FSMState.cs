using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum FSM_State
//{
//    idle,
//    walk,
//    attack,
//    spell,
//    dead
//}

//namespace FSM
//{
//    public static class FSMParameterHelper
//    {
//        public const string Walk="IsWalking";
//        public const string Run = "IsRunning";
//        public const string Attack = "Attack";
//        public const string Tied = "IsWalking";
//        public const string Spell = "Spell";
//        public const string Dead = "Dead";


//    }
//    public abstract class FSMBase
//    {
//        protected Animator animator;
//        private bool isTrigger = false;
//        event Action OnEnterEvent;
//        event Action OnExitEvent;
//        public bool IsTriggerFSM { get => isTrigger; protected set => isTrigger = value; }

//        //float ActIndex;
//        protected FSMBase(Animator animator)
//        {
//            this.animator = animator;
//        }

//       internal virtual void OnEnter()
//        {
//            OnEnterCallBack();
//            OnEnterEvent?.Invoke();
//            OnEnterEvent = null;
//        }



//        internal virtual void OnStay()
//        {

//        }
//        internal virtual void OnExit()
//        {
//            OnExitCallBack();
//            OnExitEvent?.Invoke();
//            OnExitEvent = null;


//        }

//        protected virtual void OnExitCallBack()
//        {           
//        }
//        protected virtual void OnEnterCallBack()
//        {
//        }
//        public virtual void AddOnEnterCallBackForOnce(Action action)
//        {
//            OnEnterEvent += action;
//        }
//        public virtual void AddOnExitCallBackForOnce(Action action)
//        {
//            OnExitEvent += action;
//        }
//    }
//    public abstract class TriggerFSM : FSMBase
//    {
//        bool flag = false;
//        private float finish_Time;
//        public float Finish_Time { get => finish_Time; set => finish_Time = value; }
//        private float triggered_Time;
//        public float Trigger_Time { get => triggered_Time; set => triggered_Time = value; }
//        public bool isTriggered = false;
//        public bool isFinished = false;

//        event Action OnStayEvent;


//        internal TriggerFSM(Animator animator, float _finish_Time,float triggeredTime=0) : base(animator)
//        {
//            IsTriggerFSM = true;
//            finish_Time = _finish_Time;
//            triggered_Time = triggeredTime;
//        }
//        internal override void OnStay()
//        {
//            base.OnStay();
//            if (isTriggered && !flag)
//            {
//                OnStayCallBack();
//                OnStayEvent?.Invoke();
//                OnStayEvent = null;
//                flag = true;
//            }

//        }
//        protected override void OnEnterCallBack()
//        {
//            base.OnEnterCallBack();
//            flag = false;
//            isTriggered = false;
//            isFinished = false;
//        }
//        public virtual void AddOnStayCallBackForOnce(Action action)
//        {
//            OnStayEvent += action;
//        }

//        protected virtual void OnStayCallBack()
//        {

//        }
//    }
//    public class Idle : FSMBase
//    {

//        public Idle(Animator animator) : base(animator)
//        {
//        }

//        internal override void OnEnter()
//        {
//            //animator.SetInteger("Action", 0);
//        }
//        internal override void OnStay()
//        {

//        }
//        internal override void OnExit()
//        {
//        }
//    }
//    public class Walk : FSMBase
//    {
//        private float walkSpeed =1f;
//        public Walk(Animator animator) : base(animator)
//        {
//        }

//        public void SetWalkSpeed(float speed)
//        {
//            walkSpeed= speed;
//        }
//        protected override void OnEnterCallBack()
//        {
//            base.OnEnterCallBack();
//            animator.speed = walkSpeed;
//            animator.SetBool(FSMParameterHelper.Walk, true);
//        }
//        protected override void OnExitCallBack()
//        {
//            base.OnExitCallBack();
//            animator.SetBool(FSMParameterHelper.Walk, false);
//            animator.speed = 1;
//        }
//    }
//    public class Run : FSMBase
//    {
//        private float walkSpeed = 1f;
//        public Run(Animator animator) : base(animator)
//        {
//        }

//        public void SetWalkSpeed(float speed)
//        {
//            walkSpeed = speed;
//        }
//        protected override void OnEnterCallBack()
//        {
//            base.OnEnterCallBack();
//            animator.speed = walkSpeed;
//            animator.SetBool(FSMParameterHelper.Run, true);
//        }
//        protected override void OnExitCallBack()
//        {
//            base.OnExitCallBack();
//            animator.SetBool(FSMParameterHelper.Run, false);
//            animator.speed = 1;
            
//        }
//    }

//    public class TriggerEventFSM : TriggerFSM
//    {
//        public TriggerEventFSM(Animator animator, float _finish_Time, float triggeredTime = 0) : base(animator, _finish_Time, triggeredTime)
//        {
//        }
//        protected override void OnStayCallBack()
//        {
//            base.OnStayCallBack();
//            TriggerEvent?.Invoke();
//        }

//        event Action TriggerEvent;
//        //float attackLong;
//        public void SetTriggerEvent(Action action)
//        {
//            TriggerEvent = action;
//        }
//        public void AddTriggerEvent(Action action)
//        {
//            TriggerEvent += action;
//        }
//        public void RemoveTriggerEvent(Action action)
//        {
//            TriggerEvent += action;
//        }
//    }
//    public class Attack : TriggerEventFSM
//    {
//        public Attack(Animator animator, float _finish_Time, float triggeredTime = 0) : base(animator, _finish_Time, triggeredTime)
//        {
//        }
//        protected override void OnEnterCallBack()
//        {
//            base.OnEnterCallBack();
//            animator.SetTrigger(FSMParameterHelper.Attack);
//        }
//    }

//    //public class Damaged : TriggerFSM
//    //{
//    //    public Damaged(Animator animator, float _finish_Time) : base(animator, _finish_Time)
//    //    {

//    //    }

//    //    //public event Action OnDamaged;

//    //    public override void OnEnter()
//    //    {

//    //        //animator.SetFloat("ActIndex",4,0.5f,Time.deltaTime);
//    //        //animator.SetBool("IsDamaging", true);
//    //    }
//    //    public override void OnStay()
//    //    {

//    //    }
//    //    public override void OnExit()
//    //    {
//    //        //animator.SetBool("IsDamaging", false);
//    //    }
//    //    public void Destory()
//    //    {
//    //        //OnDamaged = null;
//    //    }
//    //}
//    public class Death : TriggerEventFSM
//    {
//        public Death(Animator animator, float _finish_Time, float triggeredTime = 0) : base(animator, _finish_Time, triggeredTime)
//        {
//            IsTriggerFSM = false;
//        }
//        protected override void OnEnterCallBack()
//        {
//            base.OnEnterCallBack();
//            animator.SetBool(FSMParameterHelper.Dead, true);
//        }
//        internal override void OnExit()
//        {
//            base.OnExit();
//            animator.SetBool(FSMParameterHelper.Dead, false);
//        }
//    }

//    //示例：委托与事件的规范写法

//    public class Tied : FSMBase
//    {
//        public Tied(Animator animator) : base(animator)
//        {
//        }
//        protected override void OnEnterCallBack()
//        {
//            base.OnEnterCallBack();
//            animator.SetBool(FSMParameterHelper.Tied, true);
//        }
//        protected override void OnExitCallBack()
//        {
//            base.OnExitCallBack();
//            animator.SetBool(FSMParameterHelper.Tied, false);
//        }

//    }
//    public class Spell : TriggerEventFSM
//    {
//        public Spell(Animator animator, float _finish_Time, float triggeredTime = 0) : base(animator, _finish_Time, triggeredTime)
//        {
//        }
//        protected override void OnEnterCallBack()
//        {
//            base.OnEnterCallBack();
//            animator.SetTrigger(FSMParameterHelper.Spell);
//        }
//    }
//}

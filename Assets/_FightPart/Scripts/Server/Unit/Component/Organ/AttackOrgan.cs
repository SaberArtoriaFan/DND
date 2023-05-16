using FSM;
using Saber.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;
using System;
using FishNet;

namespace XianXia.Unit
{
    public interface IAttackOrgan
    {
        int AttackVal { get; }
        int AttackRange { get; }
    }
    public class FloatAttributeContainer
    {
        float originValue=0;
        float exValue=0;
        //T sumValue;

        public float OriginValue { get => originValue; set => originValue = value; }
        public float ExValue { get => exValue; set => exValue = value; }
        public float SumValue { get => originValue + exValue; }
        internal void Clear()
        {
            originValue = 0;
            exValue = 0;
        }
    }
    public class IntAttributeContainer
    {
        int originValue=0;
        int exValue=0;
        //T sumValue;

        public int OriginValue { get => originValue; set => originValue = value; }
        public int ExValue { get => exValue; set => exValue = value; }
        public int SumValue { get => originValue+exValue;  }

        internal void Clear()
        {
            originValue = 0;
            exValue = 0;
        }
    }
    public class Network_Attack : Attack
    {
        Client_UnitProperty unitProperty;
        public Network_Attack(Animator animator, float attackTime, Action attackEvent) : base(animator, attackTime, attackEvent)
        {
            unitProperty = animator.GetComponentInParent<Client_UnitProperty>();
        }
        public override void OnEnter()
        {
            base.OnEnter();
            InstanceFinder.GetInstance<NormalUtility>().ORPC_SetAnimatorParameter_Trigger(unitProperty, AnimatorParameters.Attack);
        }
    }
    public class AttackOrgan : OrganBase
    {
        float attackAnimationLong = 1f;
        float attackTime=0.3f;
        FloatAttributeContainer attackRange=new FloatAttributeContainer();
        IntAttributeContainer attackVal=new IntAttributeContainer() ;
        FloatAttributeContainer origin_attackSpeed = new FloatAttributeContainer();
        //float ex_attackSpeed = 0;
       // int extraAttackVal=0;
        float warningRange = 3.5f;
        FloatAttributeContainer attackHitrate =new FloatAttributeContainer();
        FloatAttributeContainer attackCriticalChance =new FloatAttributeContainer();
        FloatAttributeContainer attackCriticalDamage =new FloatAttributeContainer();

        Projectile projectile = null;
        //GameObject projectileModel=null;
        Action attackActionOnce = null;
        protected override ComponentType componentType => ComponentType.attack;
        public CharacterFSM CharacterFSM { get => characterFSM; }
        public float AttackAnimationLong { get => attackAnimationLong; set => attackAnimationLong = value; }
        public float AttackTime { get => attackTime; set => attackTime = value; }

        CharacterFSM characterFSM = null;
        Attack attackState;
        public bool IsHasTrajectory => projectile!=null;

        public float AttackRange { get => attackRange.SumValue; internal set => attackRange.OriginValue=value; }
        public float WarningRange { get => warningRange; internal set => warningRange = value; }
        public int AttackVal { get => attackVal.SumValue; }
        public int OriginAttackVal { get => attackVal.OriginValue;set=> attackVal.OriginValue=value; }
        public Projectile Projectile { get => projectile;internal set => projectile = value; }
        //public GameObject ProjectileModel { get => projectileModel; set => projectileModel = value; }
        public Action AttackActionOnce { get => attackActionOnce; set => attackActionOnce = value; }
        public int ExtraAttackVal { get => attackVal.ExValue; set => attackVal.ExValue = value; }
        public float AttackSpeed { get => origin_attackSpeed.SumValue; set => origin_attackSpeed.OriginValue = value; }
        public Attack AttackState { get => attackState; set => attackState = value; }
        public float Ex_attackSpeed { get =>origin_attackSpeed.ExValue; set => origin_attackSpeed.ExValue = value; }
        public float Origin_attackSpeed { get => origin_attackSpeed.OriginValue; set => origin_attackSpeed.OriginValue = value; }
        public float AttackHitrate { get => attackHitrate.SumValue; set => attackHitrate.OriginValue=value; }
        public float AttackCriticalChance { get => attackCriticalChance.SumValue; set => attackCriticalChance.OriginValue = value; }
        public float AttackCriticalDamage { get => attackCriticalDamage.SumValue; set => attackCriticalDamage.OriginValue=value; }
        public float Ex_AttackHitrate { get => attackHitrate.ExValue; set => attackHitrate.ExValue = value; }
        public float Ex_AttackCriticalChance { get => attackCriticalChance.ExValue; set => attackCriticalChance.ExValue = value; }
        public float Ex_AttackCriticalDamage { get => attackCriticalDamage.ExValue; set => attackCriticalDamage.ExValue = value; }
        public float Or_AttackHitrate { get => attackHitrate.OriginValue; set => attackHitrate.OriginValue = value; }
        public float Or_AttackCriticalChance { get => attackCriticalChance.OriginValue; set => attackCriticalChance.OriginValue= value; }
        public float Or_AttackCriticalDamage { get => attackCriticalDamage.OriginValue; set => attackCriticalDamage.OriginValue = value; }
        protected override void InitComponent(EntityBase owner)
        {
            base.InitComponent(owner);
            characterFSM=owner.GetComponent<CharacterFSM>();    
            //attackState=characterFSM.f
        }
        public override void Destory()
        {
            base.Destory();
             attackAnimationLong = 1f;
             attackTime = 0.3f;
            attackRange.Clear();
             attackVal.Clear();
             origin_attackSpeed.Clear();
            //float ex_attackSpeed = 0;
            // int extraAttackVal=0;
             warningRange = 3.5f;
            attackHitrate.Clear() ;
            attackCriticalChance.Clear();
            attackCriticalDamage.Clear(); ;

             projectile = null;
             //projectileModel = null;
             attackActionOnce = null;

            characterFSM = null;
            attackState=null;
        }



    }
}

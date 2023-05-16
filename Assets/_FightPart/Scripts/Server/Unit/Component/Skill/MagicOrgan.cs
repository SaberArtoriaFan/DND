using Saber.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;
using FSM;
using FishNet;

namespace XianXia.Unit
{
    internal interface IMagicPointRecover 
    {
        int MagicPoint_Max { get; set; }
        int MagicPoint_Curr { get;  set; }
        int MagicPoint_Attack { get ; set; }
        int MagicPoint_Damaged { get; set; }
        int NextMagicPoint { get; set; }
        bool CanRecordMagicPoint { get; set; }
    }
    public class Network_Spell : Spell
    {
        Client_UnitProperty unitProperty;

        public Network_Spell(Animator animator) : base(animator)
        {
            unitProperty = animator.GetComponentInParent<Client_UnitProperty>();
        }
        public override void OnEnter()
        {
            base.OnEnter();
            InstanceFinder.GetInstance<NormalUtility>().ORPC_SetAnimatorParameter_Trigger(unitProperty, AnimatorParameters.Spell);
        }
    }
    public class MagicOrgan : StatusOrganBase<ActiveSkill>,IMagicPointRecover
    {
        protected override ComponentType componentType =>ComponentType.magic;
        int magicPoint_Max=0;
        int magicPoint_Curr=0;
        int magicPoint_Attack=0;
        int magicPoint_Damaged=0;
        int nextMagicPoint = 0;
        bool canRecordMagicPoint = true;
        CharacterFSM characterFSM;
        float animationLong = 2.0f;
        public int MagicPoint_Max { get => magicPoint_Max;  }
        public int MagicPoint_Curr { get => magicPoint_Curr; }
        public int MagicPoint_Attack { get => magicPoint_Attack; }
        public int MagicPoint_Damaged { get => magicPoint_Damaged; }
        public bool ReadyToSpell=>magicPoint_Curr>=MagicPoint_Max;

        int IMagicPointRecover.MagicPoint_Max { get => magicPoint_Max; set => magicPoint_Max=value; }
        int IMagicPointRecover.MagicPoint_Curr { get => magicPoint_Curr; set { if (canRecordMagicPoint) magicPoint_Curr = value; else return; } }
        int IMagicPointRecover.MagicPoint_Attack { get => magicPoint_Attack; set => magicPoint_Attack=value; }
        int IMagicPointRecover.MagicPoint_Damaged { get => magicPoint_Damaged; set => magicPoint_Damaged=value; }
        public CharacterFSM CharacterFSM { get=>characterFSM; internal set=>characterFSM=value; }
        public int NextMagicPoint { get => nextMagicPoint; }
        public bool CanRecordMagicPoint { get => canRecordMagicPoint;  }
        int IMagicPointRecover.NextMagicPoint { get => nextMagicPoint; set => nextMagicPoint=value; }
        bool IMagicPointRecover.CanRecordMagicPoint { get => canRecordMagicPoint; set => canRecordMagicPoint=value; }
        public float AnimationLong { get => animationLong;internal set => animationLong = value; }

        protected override void InitComponent(EntityBase owner)
        {
            base.InitComponent(owner);
            characterFSM = owner.GetComponent<CharacterFSM>();

        }
        public override void Destory()
        {
            base.Destory();
            magicPoint_Max = 0;
            magicPoint_Curr = 0;
            magicPoint_Attack = 0;
            magicPoint_Damaged = 0;
            nextMagicPoint = 0;
            canRecordMagicPoint = true;
            characterFSM=null;
            animationLong = 0;
        }
    }
}

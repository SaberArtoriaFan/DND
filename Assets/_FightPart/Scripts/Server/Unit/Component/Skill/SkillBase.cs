using Saber.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;
using XianXia.Terrain;

namespace XianXia.Unit
{
    public interface IRealName
    {
        string RealName { get; }
    }
    public enum SpellTiggerType
    {
        immediate,
        point,
        unit
    }
    [Flags]
    public enum TargetType
    {
        none=0,
        enemy=2<<1,
        friend=2<<2,
        self=2 << 3,
        selfUnit=2 << 4
    }
    [Serializable]
    public abstract  class SkillBase :IComponentBase,IAllowOwnNum,IRealName
    {
        //private bool isShowInBar = true;
        private bool enable = true;
        ISkillSystem skillSystem;

        //protected ISkillBar belongSkillBar;
        //[SerializeField]
        //private SkillInfo skillInfo;

        //public SkillInfo SkillInfo { get => skillInfo; }

        public virtual string RealName { get=>"Test" ; }
        public string SkillDescription { get; }
        public uint SkillId { get ; }
        public Sprite SkillIcon { get ; }

        public ComponentType ComponentType => ComponentType.none;

        public bool Enable { get => enable; set => enable=value; }
        public IContainerEntity Owner { get => skillOrgan; set => skillOrgan = value; }

        public int AllowOwnMaxNum => 1;

        public ISkillSystem SkillSystem { get => skillSystem;internal set => skillSystem = value; }

        protected IContainerEntity skillOrgan;
        //public bool IsShowInBar { get => isShowInBar; set => isShowInBar = value; }

        //public bool Enable => isEnable;

        public SkillBase()
        {
            //OnCreate();
        }


        public virtual void Init(IContainerEntity owner)
        {
            Owner=owner;
        }
        //            protected virtual void OnCreate()
        //{

        //}

        //protected virtual void InitSelf(ISkillBar skillBar)
        //{
        //    this.belongSkillBar = skillBar;
        //    Debug.Log("获得技能信息+" + this.GetType().Name);

        //}
        //public void Init(ISkillBar skillBar)
        //{
        //    InitSelf(skillBar);
        //    skillInfo = SkillManager.Instance.GetSkillInfo(this.GetType().Name);
        //    isShowInBar = skillInfo.IsShowInBar;
        //}
        //public void Init(ISkillBar skillbar, SkillInfo skillInfo)
        //{
        //    if (skillInfo == null || this.GetType().Name != skillInfo.DataInfoName)
        //    {
        //        Init(skillbar); return;
        //    }

        //    InitSelf(skillbar);
        //    this.skillInfo = skillInfo;
        //    isShowInBar = skillInfo.IsShowInBar;
        //}

        public virtual void AcquireSkill()
        {

        }
        public virtual void LostSkill()
        {

        }

        public virtual void Destory()
        {
        }

        //public virtual void SetEnable(bool enable)
        //{
        //    if (isEnable == enable) return;
        //    isEnable = enable;
        //    if (isEnable)
        //        AcquireSkill();
        //    else
        //        LostSkill();
        //}

        //public void Init(IContainerEntity owner)
        //{
        //    throw new NotImplementedException();
        //}
    }
    internal interface IPointSkill
    {
        Node FindTarget();

    }
    public  class ActiveSkill : SkillBase
    {
        protected MagicOrgan ownerMagicOrgan;

        Node target;

        protected int startMagicPoint=10;
        protected int needMagicPointMax=100;
        protected int magicPoint_Attack = 10;
        protected int magicPoint_Damaged = 10;
        private Func<Node> findTargetFunc;

        //private SpellTiggerType spellTiggerType;
        //public virtual float spellTime
        //protected MagicOrgan magicOrgan=>()
        public virtual SpellTiggerType SpellTiggerType { get ; internal set ; }
        public Node Target { get => target; internal set => target = value; }
        public virtual bool NeedSpellAction { get;}
        public virtual TargetType TargetType { get ;internal set; }
        public virtual float SpellRange { get ;internal set ; }
        public int StartMagicPoint { get => startMagicPoint; set => startMagicPoint = value; }
        public int NeedMagicPointMax { get => needMagicPointMax; set => needMagicPointMax = value; }
        public int MagicPoint_Attack { get => magicPoint_Attack; set => magicPoint_Attack = value; }
        public int MagicPoint_Damaged { get => magicPoint_Damaged; set => magicPoint_Damaged = value; }
        public float CDTime { get => cDTime; set => cDTime = value; }
        public bool IsCD { get => isCD; set => isCD = value; }
        protected Func<Node> FindTargetFunc { get => findTargetFunc;  }

        float cDTime=0;
        bool isCD = true;

        public override void Init(IContainerEntity magicOrgan)
        {
            base.Init(magicOrgan);
            //Debug.Log((MagicOrgan)magicOrgan + "///88");
            this.ownerMagicOrgan = (MagicOrgan)magicOrgan;

        }
        public override void AcquireSkill()
        {
            base.AcquireSkill();
            FightLog.Record($"主动技能:{RealName}被添加到 玩家{UnitMainSystem.GetUnitBelongPlayer(ownerMagicOrgan.OwnerUnit)} 单位:{ownerMagicOrgan.OwnerUnit.gameObject.name}");

        }
        public override void LostSkill()
        {
            base.LostSkill();
            FightLog.Record($"主动技能:{RealName}被失去于 玩家{UnitMainSystem.GetUnitBelongPlayer(ownerMagicOrgan.OwnerUnit)} 单位:{ownerMagicOrgan.OwnerUnit.gameObject.name}");

        }
        public virtual void OnSpell()
        {
            FightLog.Record($"技能:{RealName}被释放，耗蓝{needMagicPointMax},Cd{cDTime}");
        }
        /// <summary>
        /// 释放方式为点时才会被寻找目标函数调用
        /// </summary>
        /// <returns></returns>

    }
    public  class PassiveSkill : SkillBase
    {
        protected TalentOrgan ownerTalentOrgan;
        public override void Init(IContainerEntity owner)
        {
            base.Init(owner);
            ownerTalentOrgan=(TalentOrgan)owner;
        }
        public override void AcquireSkill()
        {
            base.AcquireSkill();
            FightLog.Record($"被动技能:{RealName}被添加到 玩家{UnitMainSystem.GetUnitBelongPlayer(ownerTalentOrgan.OwnerUnit)} 单位:{ownerTalentOrgan.OwnerUnit.gameObject.name}");

        }
        public override void LostSkill()
        {
            base.LostSkill();
            FightLog.Record($"主动技能:{RealName}被失去于 玩家{UnitMainSystem.GetUnitBelongPlayer(ownerTalentOrgan.OwnerUnit)} 单位:{ownerTalentOrgan.OwnerUnit.gameObject.name}");
        }

    }
}

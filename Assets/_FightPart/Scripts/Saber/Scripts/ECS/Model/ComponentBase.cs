using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saber.ECS
{
    public enum ComponentType
    {
        none,
        system,
        body,
        attack,
        leg,
        magic,
        talent,
        statusHeart,
        uIShow
    }
    public interface IComponentBase
    {
        ComponentType ComponentType { get; }
        bool Enable { get; set; }
        IContainerEntity Owner { get; set; }
        void Init(IContainerEntity owner);
        void Destory();
    }
    public  class ComponentBase:IComponentBase
    {
        internal IContainerEntity owner;
        internal bool enable;
        internal bool isCantFindWhenDisable = false;
        public virtual ComponentType ComponentType { get; }

        public IContainerEntity Owner { get => owner;  }
        public bool Enable { get => enable;  }
        public bool Alive => enable && owner != null;

        ComponentType IComponentBase.ComponentType { get =>ComponentType;  }
        bool IComponentBase.Enable { get=>enable; set => enable=value; }
        IContainerEntity IComponentBase.Owner { get => owner; set => owner=value; }

        public ComponentBase()
        {

        }
        //internal void Init(EntityBase owner)
        //{

        //    InitComponent(owner);
        //}
        /// <summary>
        /// 被添加到Entity时调用
        /// 用来重设一些参数
        /// </summary>
        /// <param name="owner"></param>
        protected virtual void InitComponent(IContainerEntity owner)
        {
            this.owner = owner;
            enable = true;
        }

        internal void SetEnable(bool value)
        {
            SetComponentEnable(value);
        }
        protected virtual void SetComponentEnable(bool value)
        {
            enable = value;

        }

        public void Init(IContainerEntity owner)
        {
            InitComponent(owner);
        }

        public virtual void Destory()
        {
            this.owner = null;
            enable = false;
        }
    }
    public class SystemModelBase : IComponentBase
    {
        public ComponentType ComponentType => ComponentType.system;
        protected bool enable;
        public bool Enable => enable;

        public IContainerEntity Owner => null;

        bool IComponentBase.Enable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        IContainerEntity IComponentBase.Owner { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public virtual void Init(IContainerEntity owner)
        {
            //throw new System.NotImplementedException();
        }

        public void Destory()
        {
        }
    }
}

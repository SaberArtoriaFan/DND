using Saber.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;

namespace XianXia.Unit
{
    public  class StatusOrganBase<T> : OrganBase, IContainerEntity where T:IComponentBase,IRealName
    {
        List<T> m_BuffList = new List<T>();
        Dictionary<string, List<T>> m_BuffNumDict = new Dictionary<string, List<T>>();
        public List<T> StatusList { get => m_BuffList; }
        public Dictionary<string, List<T>> StatusNumDict { get => m_BuffNumDict; }

        protected override ComponentType componentType => ComponentType.none;

        //protected virtual ComponentType componentType { get; }

        public StatusOrganBase()
        {

        }
        public override void Destory()
        {
            base.Destory();
            T[] list = m_BuffList.ToArray();
            foreach(var v in list)
            {
                SystemUtility.StatusOrganRemove<T>(v);
            }
            StatusList.Clear();
            StatusNumDict.Clear();
        }
    }

    public class StatusOrgan : StatusOrganBase<Buff>
    {
        protected override ComponentType componentType => ComponentType.statusHeart;

    }
}

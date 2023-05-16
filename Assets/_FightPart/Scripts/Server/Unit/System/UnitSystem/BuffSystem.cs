using Saber.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Saber.ECS;

namespace XianXia.Unit
{
    public class BuffSystem : SlightSystemBase<Buff,StatusOrgan>
    {
        Dictionary<Buff, (Action<StatusOrgan>, Action<StatusOrgan>)> buffEventDictionary = new Dictionary<Buff, (Action<StatusOrgan>, Action<StatusOrgan>)>();

        internal Dictionary<Buff, (Action<StatusOrgan>, Action<StatusOrgan>)> BuffEventDictionary { get => buffEventDictionary; }

        //创建Buff
        //移除Buff
        //update里面出发buff的每秒事件
        //查询Buff
        /// <summary>
        /// 把BuFF的一些属性值重置一下
        /// </summary>
        /// <param name="t"></param>
        protected override void InitAfterSpawn(Buff t)
        {
            base.InitAfterSpawn(t);

        }
        /// <summary>
        /// 把BuFF的一些属性值重置一下
        /// </summary>
        /// <param name="t"></param>
        protected override void InitializeBeforeRecycle(Buff buff)
        {
            SystemUtility.StatusOrganRemove(buff);
            if (buffEventDictionary.ContainsKey(buff))
                buffEventDictionary.Remove(buff);
            //清理buff参数
            base.InitializeBeforeRecycle(buff);
        }

        public void AddBuff(string buffName,StatusOrgan target,Action<StatusOrgan> enterAction,Action<StatusOrgan> updateAction,Action<StatusOrgan> endAction)
        {
            if (target == null) return;
            Buff buff = SpawnComponent(target);
            InitBuff(buffName, buff);
            enterAction?.Invoke(target);
            buffEventDictionary.Add(buff, (updateAction, endAction));
            SystemUtility.StatusOrganAdd(target, buffName, buff, RemoveBuff);

        }
        
        //private void RemoveBuff(string buffName)
        //{
        //    throw new NotImplementedException();
        //}
        public void RemoveBuff(Buff buff)
        {
            if (buff == null) return;
            if (buffEventDictionary.TryGetValue(buff, out var actions) && actions != default)
                actions.Item2?.Invoke((StatusOrgan)buff.Owner);
            DestoryComponent(buff);
        }

        protected void InitBuff(string name,Buff buff)
        {
            //通过名字查表得到buff数据，并配置
        }
        internal void UpdateBuff(Buff buff)
        {
            if(buff.Curr_ContinueRoundNum<=0)
            {
                RemoveBuff(buff);
                return;
            }
            buff.Curr_ContinueRoundNum--;
            if (buffEventDictionary.TryGetValue(buff, out var actions) && actions != default)
                actions.Item1?.Invoke((StatusOrgan)buff.Owner);
               
        }
    }
}

using Saber.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saber.ECS
{
    public interface ISaberEvent
    {
        public void Destory();

    }
    public class SaberEvent<T> : ISaberEvent
    {
        protected event Action<T> GridAction;
        internal SaberEvent()
        {
        }
        ~SaberEvent()
        {
            Destory();
        }
        public void AddAction(Action<T> action)
        {
            GridAction += action;
        }
        public void RemoveAction(Action<T> action)
        {
            GridAction -= action;
        }
        public void Trigger(T grid)
        {
            GridAction?.Invoke(grid);
        }
        public void Destory()
        {
            GridAction = null;
        }
    }
    public class SaberEvent<T1, T2> : ISaberEvent
    {
        protected event Action<T1, T2> GridAction;
        internal SaberEvent()
        {
        }
        ~SaberEvent()
        {
            Destory();
        }
        public void AddAction(Action<T1, T2> action)
        {
            GridAction += action;
        }
        public void RemoveAction(Action<T1, T2> action)
        {
            GridAction -= action;
        }
        public void Trigger(T1 grid, T2 t2)
        {
            GridAction?.Invoke(grid, t2);
        }
        public void Destory()
        {
            GridAction = null;
        }
    }
    public class SaberEvent<T1, T2,T3> : ISaberEvent
    {
        protected event Action<T1, T2,T3> GridAction;
        internal SaberEvent()
        {
        }
        ~SaberEvent()
        {
            Destory();
        }
        public void AddAction(Action<T1, T2,T3> action)
        {
            GridAction += action;
        }
        public void RemoveAction(Action<T1, T2,T3> action)
        {
            GridAction -= action;
        }
        public void Trigger(T1 grid, T2 t2,T3 T3)
        {
            GridAction?.Invoke(grid, t2,T3);
        }
        public void Destory()
        {
            GridAction = null;
        }
    }
    public class EventSystemModel :SystemModelBase
    {

    }
}

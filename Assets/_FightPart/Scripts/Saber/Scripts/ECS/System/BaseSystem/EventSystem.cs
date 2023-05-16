using Saber.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saber.ECS
{
    public class EventSystem : SingletonSystemBase<EventSystemModel>
    {
            Dictionary<string, object> eventDict;
            //public DNDEvent<IProp> ChessGainPropEvent;
            //public DNDEvent<IProp> ChessLostPropEvent;

            public SaberEvent<T> RegisterEvent<T>(string name = "")
            {
            SaberEvent<T> res = null;
            if (name == "") name = typeof(T).Name;
            if (!eventDict.ContainsKey(name))
            {
                res = new SaberEvent<T>();
                eventDict.Add(name, res);
                return res;
            }
            else
            {
                Debug.LogError("CantAddRepeatEvent");
                return null;
            }
        }
            public SaberEvent<T1, T2> RegisterEvent<T1, T2>(string name = "")
        {
            SaberEvent<T1, T2> res = null;
            if (name == "") name = typeof(T1).Name;
            if (!eventDict.ContainsKey(name))
            {
                res = new SaberEvent<T1, T2>();
                eventDict.Add(name, res);
                return res;
            }
            else
            {
                Debug.LogError("CantAddRepeatEvent");
                return null;
            }
        }
            public SaberEvent<T> GetEvent<T>(string name = "")
            {
                if (name == "") name = typeof(T).Name;
                if (eventDict.ContainsKey(name) && eventDict[name] is SaberEvent<T>) return eventDict[name] as SaberEvent<T>;
                return null;
            }
            public SaberEvent<T1, T2> GetEvent<T1, T2>(string name = "")
            {
                if (name == "") name = typeof(T1).Name;
                if (eventDict.ContainsKey(name) && eventDict[name] is SaberEvent<T1, T2>) return eventDict[name] as SaberEvent<T1, T2>;
                return null;
            }

        public SaberEvent<T1, T2,T3> RegisterEvent<T1, T2,T3>(string name = "")
        {
            SaberEvent<T1, T2, T3> res = null;
            if (name == "") name = typeof(T1).Name;
            if (!eventDict.ContainsKey(name))
            {
                res = new SaberEvent<T1, T2, T3>();
                eventDict.Add(name, res);
                return res;
            }
            else
            {
                Debug.LogError("CantAddRepeatEvent");
                return null;
            }
        }
        public SaberEvent<T1, T2,T3> GetEvent<T1, T2,T3>(string name = "")
        {
            if (name == "") name = typeof(T1).Name;
            if (eventDict.ContainsKey(name) && eventDict[name] is SaberEvent<T1, T2, T3>) return eventDict[name] as SaberEvent<T1, T2, T3>;
            return null;
        }

        public override void Awake(WorldBase world)
        {
            base.Awake(world);
            eventDict = new Dictionary<string, object>();
        }

        public override void OnDestory()
        {
            base.OnDestory();
            foreach (var v in eventDict.Values)
            {
                ((ISaberEvent)v).Destory();
            }
            eventDict.Clear();
            eventDict = null;
        }
    }
}

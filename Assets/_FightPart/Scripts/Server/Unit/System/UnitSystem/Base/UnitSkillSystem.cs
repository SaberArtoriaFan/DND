using Saber.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;
using System.Reflection;
using System;

namespace XianXia.Unit
{

    public class UnitSkillSystem<T,T1> : NormalSystemBase<T> where T:StatusOrganBase<T1>,new() where T1:SkillBase,new()
    {
        protected SkillSystemBase<T1> skillSystem;


        public override void Start()
        {
            base.Start();
            skillSystem=world.FindSystem<SkillSystemBase<T1>>();
            //Debug.Log((skillSystem == null) + "//88");
        }
        protected override void InitializeBeforeRecycle(T t)
        {
            SystemUtility.ClearStatusOrgan(t);
            base.InitializeBeforeRecycle(t);
        }
        public void GainSkill(T t,T1 skill)
        {
            skillSystem.GainSkill(t, skill);
            GainSkillAfter(skill, t);

        }
        public T1 GainSkill(T t,string skillName)
        {
            T1 s= skillSystem.GainSkill(t,skillName);
            GainSkillAfter(s, t);
            return s;

        }
        public void SortFristSkill(T t,T1 skill)
        {
            if (t == null || skill == null||t.StatusList.Contains(skill)==false) return;
            t.StatusList.Remove(skill);
            t.StatusList.Insert(0, skill);
        }
        protected virtual void GainSkillAfter(T1 s,T organ)
        {

        }
        public  void GainSkill<T2>(T t)where T2:T1,new ()
        {
            T1 s= skillSystem.GainSkill<T2>(t);
            GainSkillAfter(s, t);
        }
        public virtual void LostSkill(T1 skill)
        {
            skillSystem.LostSkill(skill);

        }
        public virtual void LostSkill<T2>(string skill,StatusOrganBase<T2> bar)where T2:SkillBase
        {
            skillSystem.LostSkill(skill,bar);

        }
    }
}

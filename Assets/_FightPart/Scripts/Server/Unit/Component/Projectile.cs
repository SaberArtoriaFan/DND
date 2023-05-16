using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    [Serializable]
    public class Projectile 
    {
        //发射的模型名词，在包里去找模型
        [SerializeField]
        string modelName;
        //飞行弹道速度
        [SerializeField]
        float flySpeed;
        //飞行弧度
        [SerializeField]
        float arc;
        //动画曲线
        [SerializeField]
        AnimationCurve curve;

        public Projectile(string modelName, float flySpeed, float arc, AnimationCurve curve=null)
        {
            this.modelName = modelName;
            this.flySpeed = flySpeed;
            this.arc = arc;
            this.curve = curve;
        }

        public string ModelName { get => modelName; }
        public float FlySpeed { get => flySpeed; set => flySpeed = value; }
        public float Arc { get => arc; set => arc = value; }
        public AnimationCurve Curve { get => curve; set => curve = value; }
    }
}

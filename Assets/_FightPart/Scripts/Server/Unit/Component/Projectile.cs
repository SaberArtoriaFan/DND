using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    [Serializable]
    public class Projectile 
    {
        //�����ģ�����ʣ��ڰ���ȥ��ģ��
        [SerializeField]
        string modelName;
        //���е����ٶ�
        [SerializeField]
        float flySpeed;
        //���л���
        [SerializeField]
        float arc;
        //��������
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.Base;
using Saber.Camp;
using Saber.ECS;

namespace XianXia.Unit
{
    public interface IBodyOrgan : IComponentBase
    {
        int Health_Curr { get; }
        int Def { get; }
    }
    public class BodyOrgan : OrganBase, IBodyOrgan
    {
        public BodyOrgan() : base()
        {
        }

        IntAttributeContainer origin_health_Max = new IntAttributeContainer();
        //int ex_health_Max = 0;
        int health_Curr = 100;

        IntAttributeContainer origin_def = new IntAttributeContainer();
        //nt ex_def = 0;
        float deadTime = 1.5f;

        FloatAttributeContainer evade = new FloatAttributeContainer();
        protected PlayerMemeber ownerPlayer;

        Transform modelTR;
        public int Health_Curr
        {
            get => health_Curr; set
            {
                health_Curr = value;
                if (health_Curr > Health_Max) health_Curr = Health_Max;
                if (health_Curr <= 0) health_Curr = 0;
            }
        }
        public int Health_Max
        {
            get => origin_health_Max.SumValue; set
            {
                origin_health_Max.OriginValue = value;
                if (origin_health_Max.OriginValue <= 0) origin_health_Max.OriginValue = 1;
            }
        }

        public int Def { get => origin_def.SumValue; set => origin_def.OriginValue = value; }

        protected override ComponentType componentType => ComponentType.body;

        public PlayerMemeber OwnerPlayer { get => ownerPlayer; internal set => ownerPlayer = value; }
        public bool UnitAlive => Health_Curr > 0;

        public CharacterFSM CharacterFSM { get => characterFSM; internal set => characterFSM = value; }
        public float DeadTime { get => deadTime; set => deadTime = value; }
        public Transform ModelTR { get => modelTR; set => modelTR = value; }
        public int Ex_health_Max { get => origin_health_Max.ExValue; set => origin_health_Max.ExValue = value; }
        public int Origin_health_Max { get => origin_health_Max.OriginValue; }
        public int Ex_def { get => origin_def.ExValue; set => origin_def.ExValue = value; }
        public int Origin_def { get => origin_def.OriginValue; }
        public float Evade { get => evade.SumValue; set => evade.OriginValue = value; }
        public float Ex_Evade { get => evade.ExValue; set => evade.ExValue = value; }
        public float Or_Evade { get => evade.OriginValue; set => evade.OriginValue = value; }

        private CharacterFSM characterFSM;

        protected override void InitComponent(EntityBase unit)
        {
            base.InitComponent(unit);
            modelTR = unit.GetComponentInChildren<Animator>().transform;
            characterFSM = unit.GetComponent<CharacterFSM>();
        }
        public override void Destory()
        {
            base.Destory();
            origin_health_Max.Clear();
            //int ex_health_Max = 0;
            health_Curr = 0;

            origin_def.Clear();
            //nt ex_def = 0;
            deadTime = 1.5f;

            evade.Clear();
            ownerPlayer = null;

            modelTR = null;
            characterFSM = null;

        }
    }
}

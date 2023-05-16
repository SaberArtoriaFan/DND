using FishNet;
using Saber.ECS;
using UnityEngine;

namespace XianXia.Unit
{
    public class ShapeShiftData 
    {
        public float heath=1;
        public float attack=1;
        public float defence=1;
        public float attackRange;
        public float warningRange;
        public float attackSpeed=1;
        public float evade=1;
        public float hitrate=1;
        public float criticalchance=1;
        public float criticaldamage=1;
        public float movementspeed=1;
        public string[] passiveSkills=new string[] { "HealSkill" };
        public string activeSkill= "CriticalStrikeSkill";
        internal string prefabName;
        internal float attackTime=0.5f;
        internal float attackAnimationLong=0.7f;
        internal Projectile projectile;
        internal float spellTime=1f;
    }
    public class SkilpassiveSkill : ActiveSkill
    {


        //����Ӧ������һ����¼��Ӣ�����ݵ��࣬��Ϊս������������Ҫ���
        TestUnitModel originModel;
        ShapeShiftData shapeModel;

        //����
        public override bool NeedSpellAction => true;
        public override SpellTiggerType SpellTiggerType => SpellTiggerType.immediate;
        public override TargetType TargetType => TargetType.self;
        //
        Transform originTR;
        Transform shapeShiftTR;
        float continueTime = 5;


        //internal void InitSkill(UnitModel unitName,ShapeShiftData unitModel)
        //{
        //    //this.shapeShiftUnitModel = unitName;
        //    this.unitModel = unitModel;
        //}
        void Test()
        {
            originModel = ownerMagicOrgan.OwnerUnit.GetComponent<TestUnitModel>();
            this.shapeModel = new ShapeShiftData();
            shapeModel.heath =1.8f;
            shapeModel.attack = 2;
            shapeModel.defence = 1.5f;
            shapeModel.attackRange = 6;
            shapeModel.attackSpeed = 1.2f;
            shapeModel.warningRange = 8;
            shapeModel.movementspeed = 2;
            shapeModel.prefabName = "TestShape";
            shapeModel.evade = 3;
            shapeModel.hitrate = 1;
            shapeModel.projectile = new Projectile("Cyclone", 5, 0);
        }
        public override void OnSpell()
        {
            base.OnSpell();
            //�����õ�
            Test();

            this.Enable = false;

            UnitBase unit = ownerMagicOrgan.OwnerUnit;
            UIShowOrgan uIShowOrgan = unit.FindOrganInBody<UIShowOrgan>(ComponentType.uIShow);

            //GameObject newmModel;
            //if (shapeShiftTR == null)
            //{
            //    newmModel = GameObject.Instantiate(ABUtility.Load<GameObject>(ABUtility.UnitMainName + shapeModel.prefabName));
            //    newmModel.transform.SetParent(unit.transform);
            //    newmModel.transform.localPosition = Vector3.zero;
            //    shapeShiftTR = newmModel.transform;
            //}
            //else
            //    newmModel = shapeShiftTR.gameObject;
            //newmModel.SetActive(true);
            //originTR = bodyOrgan.ModelTR;
            //originTR.gameObject.SetActive(false);
            //SetSomeParameter(shapeShiftTR);
            //ģ���Ѿ��������
            //��������
            //Debug.Log("555");
            BodyOrgan bodyOrgan = unit.FindOrganInBody<BodyOrgan>(ComponentType.body);


            bodyOrgan.Ex_health_Max += (int)(bodyOrgan.Health_Max * (shapeModel.heath - 1));
            bodyOrgan.Health_Curr += (int)(bodyOrgan.Health_Curr * (shapeModel.heath-1));
            bodyOrgan.Ex_def += (int)(bodyOrgan.Origin_def * (shapeModel.defence - 1));
            bodyOrgan.Ex_Evade += bodyOrgan.Or_Evade * (shapeModel.evade - 1);
            AttackOrgan attackOrgan = unit.FindOrganInBody<AttackOrgan>(ComponentType.attack);

            attackOrgan.AttackRange = shapeModel.attackRange;
            attackOrgan.AttackTime = shapeModel.attackTime;
            attackOrgan.AttackAnimationLong = shapeModel.attackAnimationLong;
            attackOrgan.WarningRange = shapeModel.warningRange;

            attackOrgan.ExtraAttackVal += (int)(attackOrgan.OriginAttackVal * (shapeModel.attack - 1));
            attackOrgan.Ex_attackSpeed+= (attackOrgan.Origin_attackSpeed * (shapeModel.attackSpeed - 1));
            attackOrgan.Ex_AttackHitrate+= (shapeModel.hitrate-1)*(attackOrgan.Or_AttackHitrate);
            //Debug.Log("������" + attackOrgan.AttackHitrate);
            attackOrgan.Ex_AttackCriticalChance  += (shapeModel.criticalchance - 1) * attackOrgan.Or_AttackCriticalChance;
            attackOrgan.Ex_AttackCriticalDamage += (shapeModel.criticaldamage - 1) * attackOrgan.Or_AttackCriticalDamage;
            UnitAttackSystem attackSystem = SkillSystem.World.FindSystem<UnitAttackSystem>();
            attackSystem.ChangeWeapon(attackOrgan, shapeModel.projectile);
            attackSystem.ChangeFsm(attackOrgan);

            UnitSpellSystem unitSpellSystem = SkillSystem.World.FindSystem<UnitSpellSystem>();
            //unitSpellSystem.SetSpellTime(attackOrgan.CharacterFSM, shapeModel.spellTime);

            UnitTalentSystem unitTalentSystem = SkillSystem.World.FindSystem<UnitTalentSystem>();
            //��Ӽ���
            //
            if (shapeModel.passiveSkills != null && shapeModel.passiveSkills.Length > 0)
            {
                TalentOrgan talentOrgan = unit.FindOrganInBody<TalentOrgan>(ComponentType.talent);
                foreach (var v in shapeModel.passiveSkills)
                {
                    unitTalentSystem.GainSkill(talentOrgan, v);
                }
            }
            MagicOrgan magicOrgan = unit.FindOrganInBody<MagicOrgan>(ComponentType.magic);
            //return;
            if (shapeModel.activeSkill!=null&& shapeModel.activeSkill != "")
            {
                //MagicOrgan magicOrgan = unit.AddOrgan<MagicOrgan>(ComponentType.magic);
                //magicOrgan.AnimationLong = ;
                //unitSpellSystem.LostSkill(this);
                
                ActiveSkill skill= unitSpellSystem.GainSkill(magicOrgan, shapeModel.activeSkill);
                unitSpellSystem.SortFristSkill(ownerMagicOrgan,skill);
            }
            //unitSpellSystem.GainSkill( magicOrgan,this);
            else
            {
                //û���������ܾ͹ر�����
                IMagicPointRecover m = (IMagicPointRecover)magicOrgan;
                m.MagicPoint_Max = 0;
                m.MagicPoint_Curr = 0;

            }

            //����ģ��
            //����ģ��
            InstanceFinder.GetInstance<NormalUtility>().ORPC_ShapeShift(unit.gameObject, ABUtility.UnitMainName, shapeModel.prefabName);

            //�ж��ǲ��ǵ�ʱ����Ҫ�����
            if (continueTime > 0)
            {
                TimerManagerSystem timerManagerSystem = SkillSystem.World.FindSystem<TimerManagerSystem>();
                timerManagerSystem.AddTimer(CancelShapeShift, continueTime);
            }


            //���䷶ΧҲ��Ҫ����
            //����״̬�����  Attack�Ĺ���ʱ��ͽ���ʱ��,Spell�Ľ���ʱ��ҲҪ
            //attackOrgan.
        }

        private void SetSomeParameter(Transform tr)
        {
            UnitBase unit = ownerMagicOrgan.OwnerUnit;
            //UIShowOrgan uIShowOrgan = unit.FindOrganInBody<UIShowOrgan>(ComponentType.uIShow);
            unit.GetComponent<CharacterFSM>().ChangeAnimator(tr.GetComponentInChildren<Animator>());
            //uIShowOrgan.OriginScale = tr.localScale;
            //uIShowOrgan.UnitScale = uIShowOrgan.UnitScale;
            BodyOrgan bodyOrgan = unit.FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body);
            //originTR = bodyOrgan.ModelTR;
            //originTR.gameObject.SetActive(false);
            bodyOrgan.ModelTR = tr;
            tr.localRotation = Quaternion.identity;

        }
        private void CancelShapeShift()
        {
            //�����˾Ͳ��ñ������
            if (this.ownerMagicOrgan == null || ownerMagicOrgan.OwnerUnit == null) return;


            ////ģ�ͱ����
            //shapeShiftTR.gameObject.SetActive(false);
            //originTR.gameObject.SetActive(true);



            UnitBase unit = ownerMagicOrgan.OwnerUnit;
            BodyOrgan bodyOrgan = unit.FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body);

            bodyOrgan.Ex_health_Max -= (int)(bodyOrgan.Origin_health_Max * (shapeModel.heath - 1));
            bodyOrgan.Health_Curr = bodyOrgan.Health_Curr;
            //bodyOrgan.Health_Curr -= (int)(bodyOrgan.Health_Curr * (unitModel.heath - 1));
            bodyOrgan.Ex_def-= (int)(bodyOrgan.Origin_def * (shapeModel.defence - 1));
            bodyOrgan.Ex_Evade -= bodyOrgan.Or_Evade * (shapeModel.evade - 1);


            AttackOrgan attackOrgan = unit.FindOrganInBody<AttackOrgan>(ComponentType.attack);
            attackOrgan.AttackRange = shapeModel.attackRange;

            attackOrgan.AttackTime = shapeModel.attackTime;
            attackOrgan.AttackAnimationLong = shapeModel.attackAnimationLong;
            attackOrgan.WarningRange = shapeModel.warningRange;

            attackOrgan.ExtraAttackVal -= (int)(attackOrgan.OriginAttackVal * (shapeModel.attack - 1));
            attackOrgan.Ex_attackSpeed -=(attackOrgan.Origin_attackSpeed * (shapeModel.attackSpeed - 1));
            attackOrgan.Ex_AttackHitrate -= (shapeModel.hitrate - 1) * (attackOrgan.Or_AttackHitrate);
            attackOrgan.Ex_AttackCriticalChance -= (shapeModel.criticalchance - 1) * attackOrgan.Or_AttackCriticalChance;
            attackOrgan.Ex_AttackCriticalDamage -= (shapeModel.criticaldamage - 1) * attackOrgan.Or_AttackCriticalDamage;

            UnitAttackSystem attackSystem = SkillSystem.World.FindSystem<UnitAttackSystem>();
            attackSystem.ChangeWeapon(attackOrgan, originModel.projectile);
            attackSystem.ChangeFsm(attackOrgan);

            UnitSpellSystem unitSpellSystem = SkillSystem.World.FindSystem<UnitSpellSystem>();
            //unitSpellSystem.SetSpellTime(attackOrgan.CharacterFSM, shapeModel.spellTime);

            UnitTalentSystem unitTalentSystem = SkillSystem.World.FindSystem<UnitTalentSystem>();
            //
            if (shapeModel.passiveSkills != null && shapeModel.passiveSkills.Length > 0)
            {
                TalentOrgan talentOrgan = unit.FindOrganInBody<TalentOrgan>(ComponentType.talent);
                foreach (var v in shapeModel.passiveSkills)
                {
                    unitTalentSystem.LostSkill(v, talentOrgan);
                }
            }
            MagicOrgan magicOrgan = unit.FindOrganInBody<MagicOrgan>(ComponentType.magic);

            if (!string.IsNullOrEmpty(shapeModel.activeSkill))
            {
                //MagicOrgan magicOrgan = unit.AddOrgan<MagicOrgan>(ComponentType.magic);
                //magicOrgan.AnimationLong = ;
                //magicOrgan.StatusList.Clear();
                //Debug.Log(this);
                unitSpellSystem.LostSkill(shapeModel.activeSkill, magicOrgan);
            }
            //unitSpellSystem.GainSkill(magicOrgan, this);
            //Transform temp = originTR;
            //originTR = shapeShiftTR;
            //shapeShiftTR = temp;
            UnitSpellSystem.SetSkillMagicPoint(this, ownerMagicOrgan, true);
            this.Enable = true;

            ////ģ���Ѿ��������
            ////��������
            //SetSomeParameter(originTR);
            InstanceFinder.GetInstance<NormalUtility>().ORPC_ShapeShift(this.ownerMagicOrgan.OwnerUnit.gameObject, null, null);
        }
    }
}

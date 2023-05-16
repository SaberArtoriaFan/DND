using FishNet;
using Saber.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using XianXia.Terrain;

namespace XianXia.Unit
{
    public class BlastingSkill : ActiveSkill
    {
   
        //EffectSystem effectSystem;
        UnitMainSystem mainSystem;
        ProjectileSystem projectileSystem;
        UnitBodySystem bodySystem;
        TimerManagerSystem timerManager;

        public override float SpellRange => 100f;
        public override bool NeedSpellAction => true;
        public override SpellTiggerType SpellTiggerType => SpellTiggerType.unit;
        public override TargetType TargetType => targetType;
        //流星偏移量
        Vector3 offest = new Vector3(-5, 5, 0);
        float speed = 2;
        float influenceRange = 1.5f;
        TargetType targetType=TargetType.enemy;
        int damageVal=50;
        const string warnCircle = "MagicCircleSimpleYellow";
        const string projectEffect = "CFX2_DoubleFireBall A";
        const string explodeEffect = "CFXR2 Magic Explosion Spherical (Lit, Purple+Blue)";
        public override void Init(IContainerEntity magicOrgan)
        {
            base.Init(magicOrgan);
            //effectSystem = SkillSystem.World.FindSystem<EffectSystem>();
            projectileSystem = SkillSystem.World.FindSystem<ProjectileSystem>();
            bodySystem = SkillSystem.World.FindSystem<UnitBodySystem>();
            mainSystem = SkillSystem.World.FindSystem<UnitMainSystem>();
            timerManager = SkillSystem.World.FindSystem<TimerManagerSystem>();
        }
        public override void AcquireSkill()
        {
            base.AcquireSkill();

        }
        string SetName(StringBuilder stringBuilder,DateTime now,string name)
        {
            stringBuilder.Clear();
            stringBuilder.Append(name);
            stringBuilder.Append(now.Hour.ToString());
            stringBuilder.Append(now.Minute.ToString());
            stringBuilder.Append(now.Second.ToString());
            return stringBuilder.ToString();

        }
        public override void OnSpell()
        {
            base.OnSpell();
            NormalUtility normalUtility= InstanceFinder.GetInstance<NormalUtility>();
            DateTime now = DateTime.Now;
            StringBuilder stringBuilder = new StringBuilder();
            Vector3 target = AStarPathfinding2D.GetNodeWorldPositionV3(Target.Position, SkillSystem.Map);

            //normalUtility
            //SystemUtility
            //根据独一无二的Key创建，到时候也根据这个删除客户端的特效
            string cycleName = warnCircle + NormalUtility.GetId();//SetName(stringBuilder, now, warnCircle);
            normalUtility.ORPC_CreateEffect(warnCircle, cycleName, target);
            //string explode
            //ParticleSystem proSystem = effectSystem.GetEffectInPool_Main(projectEffect, false);
            //ParticleSystem cycle = effectSystem.GetEffectInPool_Main(warnCircle, false);
            //ParticleSystem explode = effectSystem.GetEffectInPool_Main(explodeEffect, false);
            //cycle.transform.position = target;
            //cycle.gameObject.SetActive(true);
            //cycle.Play();
            //proSystem.transform.position = target + offest;
            //proSystem.gameObject.SetActive(true);
            Node targetNode = Target;
            //ProjectileSystem.ProjectileHelper projectileHelper = projectileSystem.InitProjectile(proSystem.transform, null,target, "", speed, 0, null, () => {  Damage(targetNode);effectSystem.RecycleEffectToPool(cycle, warnCircle);effectSystem.RecycleEffectToPool(proSystem, projectEffect); PlayeExplode(explode, target, explodeEffect); }, null,false);
            ProjectileSystem.ProjectileHelper projectileHelper = null;
            projectileHelper= projectileSystem.CreateProjectile("", target + offest, 1, null,AStarPathfinding2D.GetNodeWorldPositionV3(targetNode.Position,SkillSystem.Map), speed, 0, null, () => { 
                Damage(targetNode);
                //根据Key回收
                normalUtility.ORPC_RecycleEffect(cycleName); 
                //根据父物体回收
                normalUtility.ORPC_RecycleEffect(projectileHelper.owner.gameObject); 
                //创建，但自动回收
                normalUtility.ORPC_CreateEffect(explodeEffect, null, target, default, default, true); }, null, false);
            normalUtility.ORPC_CreateEffect(projectEffect, projectileHelper.owner.gameObject, projectileHelper.owner.gameObject.transform.position, false);
            //projectileSystem.SetProjectilePathWay(projectileHelper);
        }
        //void PlayeExplode(ParticleSystem particleSystem,Vector3 pos,string name)
        //{
        //    if (particleSystem == null) Debug.LogError("！！");
        //    particleSystem.transform.position = pos;
        //    particleSystem.gameObject.SetActive(true);
        //    particleSystem.Play();
        //    //
        //    Saber.ECS.Timer timer = null;
        //    timer= timerManager.AddTimer(() => { if (!particleSystem.IsAlive()) { Debug.Log("回收动画") ; effectSystem.RecycleEffectToPool(particleSystem, name); timer.Stop(); } }, 1,true);
        //}
        void Damage(Node node)
        {
            Debug.Log("AAA"+node.Position);
            //Node unitNode = mainSystem.GetGridItemByUnit(ownerMagicOrgan.OwnerUnit);
            List<Node> targets = AStarPathfinding2D.FindTargetsInRange(node, influenceRange, (a, b) => { return SystemUtility.TargetFiltration2(ownerMagicOrgan.OwnerUnit, mainSystem.GetUnitByGridItem(b), targetType); }, SkillSystem.Map, true, false, true);
            //Debug.Log
            Debug.Log(targets.Count + "人受到了伤害");
            try
            {
                foreach (var n in targets)
                {
                    UnitBase v = mainSystem.GetUnitByGridItem(n);
                    BodyOrgan mybody;
                    if (v != null && (mybody = v.FindOrganInBody<BodyOrgan>(ComponentType.body)) != null)
                        bodySystem.ReceiveDamage(mybody, new Unit.Damage(ownerMagicOrgan.OwnerUnit, damageVal, false, true));
                }
            }catch(Exception ex)
            {
                Debug.LogError(ex.Message);
            }
 
        }
    }
}

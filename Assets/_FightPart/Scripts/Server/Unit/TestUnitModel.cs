using Saber.Camp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Saber.ECS;
using XianXia.Terrain;
using UnityEngine.Rendering;

namespace XianXia.Unit
{
    [Serializable]
    public class UnitModel
    {
        public string prefabName;
        [Header("玩家")]
        public PlayerEnum player;
        [Header("身体")]

        public float hight = 1.5f;
        public int health_Max = 100;

        public int health_Curr = 100;
        public int def = 0;

        public float moveSpeed = 200f;
        [Space]
        [Header("攻击")]
        public float attackAnimationLong = 1f;
        public float attackTime = 0.3f;
        public float attackRange = 1.6f;
        public int attackVal = 5;
        public float attackSpeed = 1f;
        public float warningRange = 3.5f;
        public Projectile projectile = null;
        //public int moveSpeed = 200;
        [Space]
        [Header("技能")]
        public float spellTime = 1f;
        public string activeSkill;
        public string[] passiveSkills;
    }
    public class TestUnitModel : MonoBehaviour
    {
        public bool ShowInTime = false;
        [Header("仅做显示窗口")]
        public UnitBase follower;
        public UnitBase AttackTarget;
        [Header("玩家")]
        public PlayerEnum player;
        [Header("身体")]

        public float hight = 2.5f;
        public int health_Max = 100;

        public int health_Curr = 100;
        public int def = 0;
        public float evade = 0.05f;
        public float moveSpeed = 200f;
        [Space]
        [Header("攻击")]
        public float attackHitrate = 0.95f;
        public float attackCriticalchance = 0.08f;
        public float attackCriticaldamage = 1.5f;
        public float attackAnimationLong = 1f;
        public float attackTime = 0.3f;
        public float attackRange = 1.6f;
        public int attackVal = 5;
        public float attackSpeed = 1f;
        public float warningRange = 3.5f;
        public Projectile projectile = null;
        //public int moveSpeed = 200;
        [Space]
        [Header("技能")]
        public float spellTime = 1f;
        public string activeSkill;
        public string[] passiveSkills;

        public void  Init(UnitBase unit, WorldBase world)
        {
            body = unit.FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body);
            attack = unit.FindOrganInBody<AttackOrgan>(Saber.ECS.ComponentType.attack);
            legOrgan = unit.FindOrganInBody<LegOrgan>(Saber.ECS.ComponentType.leg);
            this.worldBase = world;
            attackSystem = world.FindSystem<UnitAttackSystem>();
            map = FindObjectOfType<AStarPathfinding2D>();
            showOrgan = unit.FindOrganInBody<UIShowOrgan>(ComponentType.uIShow);
        }
        BodyOrgan body=null;
        AttackOrgan attack=null;
        LegOrgan legOrgan = null;
        UnitAttackSystem attackSystem;
        UIShowOrgan showOrgan = null;
        WorldBase worldBase;
        AStarPathfinding2D map;
        //R40442
        private void Update()
        {
            //if (unit == null) unit = GetComponent<UnitBase>().FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body);
            if (ShowInTime&& body != null){
                health_Curr = body.Health_Curr;
                health_Max = body.Health_Max;
                attackVal = attack.AttackVal;
                attackSpeed = attack.AttackSpeed;
                AttackTarget = attackSystem.FindAttackTarget(attack);
                follower = legOrgan.Follower;
                def = body.Def;
                player = CampManager.GetPlayerEnum(body.OwnerPlayer);
                hight = showOrgan.UnitHight;
                attackHitrate = attack.AttackHitrate;
                attackCriticalchance = attack.AttackCriticalChance;
                attackCriticaldamage = attack.AttackCriticalDamage;
                attackAnimationLong = attack.AttackAnimationLong;
                attackTime = attack.AttackTime;
                attackRange = attack.AttackRange;
                //attackVal = 5;
                //attackSpeed = 1f;
                warningRange = attack.WarningRange;
                projectile = attack.Projectile;

        //SortingGroup
    }

    //Debug.Log(gameObject.name + unit.Health_Curr + "///" + unit.Health_Max);
}
        private void OnDrawGizmosSelected()
        {
            if (attack != null && attack.Owner != null && attack.OwnerUnit.transform != null)
            {
                Gizmos.color = Color.red;
                GLUtility.DrawWireDisc(attack.OwnerUnit.transform.position, Vector3.forward, attack.AttackRange * map.NodeSize);
                Gizmos.color = Color.yellow;
                Vector2 mapY = new Vector2(map.startPos.y, map.startPos.y + map.MapHeight);
                GLUtility.DrawLine(new Vector3(attack.OwnerUnit.transform.position.x + attack.WarningRange * map.NodeSize, mapY.x), new Vector3(attack.OwnerUnit.transform.position.x + attack.WarningRange * map.NodeSize, mapY.y));
                GLUtility.DrawLine(new Vector3(attack.OwnerUnit.transform.position.x - attack.WarningRange * map.NodeSize, mapY.x), new Vector3(attack.OwnerUnit.transform.position.x - attack.WarningRange * map.NodeSize, mapY.y));

            }
        }
    }
}

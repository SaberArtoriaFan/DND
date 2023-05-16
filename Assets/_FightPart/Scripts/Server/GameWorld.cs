using Saber.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XianXia.Unit;
using Saber.ECS;
namespace XianXia
{

    public class GameWorld : BaseWorld
    {

        protected override void InitSystem()
        {
            base.InitSystem();
            //AddSystem<RisingSpaceUISystem>();
            AddSystem<ProjectileSystem>();
            AddSystem<MagicSkillSystem>();
            AddSystem<PassiveSkillSystem>();
            AddSystem<BuffSystem>();

            AddSystem<UnitMainSystem>();
            AddSystem<UnitStatusSystem>();
            AddSystem<UnitBodySystem>();
            AddSystem<UnitSpellSystem>();
            AddSystem<UnitTalentSystem>();
            AddSystem<UnitAttackSystem>();
            AddSystem<UnitMoveSystem>();


            AddSystem<UnitUIShowSystem>();
        }

    }
}

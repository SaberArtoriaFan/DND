using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;

namespace XianXia.Unit
{
    public class TalentOrgan : StatusOrganBase<PassiveSkill>
    {
        protected override ComponentType componentType => ComponentType.talent;

    }
}

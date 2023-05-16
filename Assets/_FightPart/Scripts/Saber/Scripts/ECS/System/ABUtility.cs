using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saber.ECS
{
    public abstract class ABUtility : AutoSingleton<ABUtility> 
    {

        public const string ABPackageDataPath = "Assets/HotUpdateResources";
        public const string EffectMainName = "/Main/Common/Prefab/Effect/";
        public const string ProjectMainName = "/Main/Common/Prefab/Projectile/";
        public const string UIMainName = "/Main/Common/Prefab/UI/";
        public const string UnitMainName = "/Main/Common/Prefab/Unit/";
        public static T1 Load<T1>(string path) where T1 : Object
        {
            path=ABPackageDataPath+path;
            return Instance.LoadAssest<T1>(path);
        }
       protected abstract T1 LoadAssest<T1>(string path) where T1 : Object;
    }
}

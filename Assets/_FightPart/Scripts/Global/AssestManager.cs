using JEngine.Core;
using System.Collections;
using System.Collections.Generic;
using Saber.ECS;
using Saber.Base;

public class AssestManager : ABUtility
{
    protected override T LoadAssest<T>(string path)
    {
        //Singleton
       return AssetMgr.Load<T>(path);
    }
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

}

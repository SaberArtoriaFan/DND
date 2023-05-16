using JEngine.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malee;
public class Cube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 10; i++)
        {
            GameObject cube = GameObject.Instantiate(AssetMgr.Load<GameObject>("Assets/HotUpdateResources/Main/Common/Prefab/Cube.prefab"));
            cube.transform.position = Vector3.one*i;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

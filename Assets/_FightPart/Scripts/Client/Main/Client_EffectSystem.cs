using Saber.Base;
using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XianXia
{
    public class Client_EffectSystem : Client_SingletonBase<Client_EffectSystem>
    {
        string sortLayer = "Effect";
        //ObjectPoolSystem objectPoolSystem;
        //ABManagerSystem aBManagerSystem;
        //TimerManagerSystem timerManagerSystem;
        float lastestDestroyTime;
        Transform parent;
        Dictionary<GameObject, ParticleSystem> parentAndSonDict = new Dictionary<GameObject, ParticleSystem>();
        Dictionary<string, ParticleSystem> nameAndSonDict = new Dictionary<string, ParticleSystem>();

        protected override void Start()
        {
            base.Start();
            //objectPoolSystem = world.FindSystem<ObjectPoolSystem>();
            //aBManagerSystem = world.FindSystem<ABManagerSystem>();
            //timerManagerSystem = world.FindSystem<TimerManagerSystem>();
            parent = new GameObject("Effect").transform;
            parent.parent = this.transform;
        }


        public void CreateEffectInPool_Main(string effectName, GameObject parent,bool isAutoRecycle)
        {
            if (parentAndSonDict.ContainsKey(parent)) { Debug.LogError("创建特效出错！！！"); parentAndSonDict.Remove(parent); }
            ParticleSystem particleSystem = GetEffectInPool_Main(effectName, isAutoRecycle);
            if (particleSystem == null) { Debug.LogError("CantFindPOOL"); return; }
            particleSystem.transform.SetParent(parent.transform);
            particleSystem.transform.localPosition = Vector3.zero;
            particleSystem.transform.localRotation = Quaternion.identity;
            particleSystem.transform.localScale = Vector3.one;
            particleSystem.gameObject.SetActive(true);
            particleSystem.Play();
            parentAndSonDict.Add(parent, particleSystem);
        }
        public void CreateEffectInPool_Main(string effectName, string key,Vector3 pos,Vector3 rotate=default,Vector3 scale=default, bool isAutoRecycle=false)
        {
            ParticleSystem particleSystem = GetEffectInPool_Main(effectName, isAutoRecycle);
            particleSystem.transform.position = pos;
            if(rotate!=default)
                particleSystem.transform.rotation = Quaternion.Euler(rotate);
            if(scale!=default)
                particleSystem.transform.localScale = scale;
            particleSystem.gameObject.SetActive(true);
            particleSystem.Play();
            if (!string.IsNullOrEmpty(key)&&nameAndSonDict.ContainsKey(key)==false)
                nameAndSonDict.Add(key,particleSystem);
        }
        public void RecycleEffect(string key)
        {
            if (!nameAndSonDict.ContainsKey(key)) return;
            ParticleSystem project = null;
            project = nameAndSonDict[key];
            nameAndSonDict.Remove(key);
            if (project == null) { Debug.LogError("CantFindProjectModel"); return; }
            RecycleEffectToPool(project, project.gameObject.name);
        }
        public void RecycleEffect(GameObject parent)
        {
            if (!parentAndSonDict.ContainsKey(parent)) return;
            ParticleSystem project = null;
            project = parentAndSonDict[parent];
            parentAndSonDict.Remove(parent);
            if (project == null) { Debug.LogError("CantFindProjectModel"); return; }
            RecycleEffectToPool(project,project.gameObject.name);
        }
        /// <summary>
        /// 拿出来Active是False，需要手动激活以及Play
        /// </summary>
        /// <param name="effectName"></param>
        /// <param name="isAutoRecycle"></param>
        /// <returns></returns>
        public ParticleSystem GetEffectInPool_Main(string effectName, bool isAutoRecycle)
        {
            if (!PoolManager.Instance.IsPoolAlive(effectName))
            {
                GameObject go = ABUtility.Load<GameObject>(ABUtility.EffectMainName + effectName);
                PoolManager.Instance.AddPool<ParticleSystem>(() => {
                    SortingGroup sortingGroup = GameObject.Instantiate(go).AddComponent<SortingGroup>();
                    sortingGroup.gameObject.name = effectName;
                    sortingGroup.transform.SetParent(parent);
                    sortingGroup.sortingLayerName = sortLayer;
                    return sortingGroup.GetComponent<ParticleSystem>();
                },
    (u) => { u.gameObject.SetActive(false); }, (u) => { u.gameObject.SetActive(false); }, effectName);
            }

            ParticleSystem effect = PoolManager.Instance.GetObjectInPool<ParticleSystem>(effectName);
            if (isAutoRecycle)
            {
                Timer timer = null;
                float t = 0;
                timer = TimerManager.Instance.AddTimer(() => { t += Time.deltaTime; ReadyRecycle(effect, effectName,timer); if (t >= lastestDestroyTime) {  timer.Stop(); } }, 1);

            }

            return effect;
        }
        //public ParticleSystem GetEffectInPool(string effectName)
        //{
        //    if (!objectPoolSystem.IsPoolAlive(effectName))
        //        objectPoolSystem.AddPool<ParticleSystem>(() => { return     GameObject.Instantiate(aBManagerSystem.LoadResource<GameObject>(effectABPackageName, effectName)).GetComponent<ParticleSystem>(); }, (u) => { u.gameObject.SetActive(false); u.transform.SetParent(world.transform); }, (u) => { u.gameObject.SetActive(false); }, effectName);
        //    return objectPoolSystem.GetObjectInPool<ParticleSystem>(effectName);
        //}
        public void RecycleEffectToPool(ParticleSystem effect, string effectName)
        {
            if (!PoolManager.Instance.IsPoolAlive(effectName)) return;
            PoolManager.Instance.RecycleToPool(effect, effectName);
        }
        protected void ReadyRecycle(ParticleSystem particleSystem, string name,Timer timer)
        {
            if (particleSystem.gameObject.activeSelf == true && !particleSystem.IsAlive(true)) { RecycleEffectToPool(particleSystem, name); timer.Stop(); }
        }
    }
}

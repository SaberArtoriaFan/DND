using JEngine.Core;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;
using XianXia;

public class PhoneTest : MonoBehaviour
{
    Updater updater;
    [SerializeField]
    Text text;
    [SerializeField]
    Button button;
    private void Start()
    {
        updater = GetComponent<Updater>();
        button.onClick.AddListener(ChangeMode);
        if (updater != null)
        {

#if !UNITY_EDITOR&&UNITY_SERVER
            Console.WriteLine("StartServer Succ");
            updater.mode = Updater.UpdateMode.Standalone;
#elif UNITY_EDITOR&&UNITY_SERVER
            updater.mode = Updater.UpdateMode.Simulate;
#elif (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            updater.mode = Updater.UpdateMode.Remote;
#else
            updater.mode = Updater.UpdateMode.Simulate;
#endif
#if UNITY_SERVER
            FightServerManager.ConsoleWrite_Saber($"热更新模式:{updater.mode.ToString()}");
#endif
            updater.StartUpdate();
        }

    }

    private void ChangeMode()
    {
        switch (updater.mode)
        {
            case Updater.UpdateMode.Simulate:
                updater.mode = Updater.UpdateMode.Standalone;
                break;
            case Updater.UpdateMode.Standalone:
                updater.mode = Updater.UpdateMode.Remote;
                break;
            case Updater.UpdateMode.Remote:
                updater.mode = Updater.UpdateMode.Simulate;
                break;
        }
    }

    [Button]
    void Text()
    {
        updater.StartUpdate();
    }
    
    private void Update()
    {
        if(text!=null)
            text.text = updater.mode.ToString();
    }
}

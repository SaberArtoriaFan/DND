using FishNet;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XianXia.Unit
{
    public class TimeScaleUI : MonoBehaviour
    {
        Button button;
        TMP_Text text;
        float speed;
        [SerializeField]
        float toBeSpeed = 2;
        private void Start()
        {
            Transform[] trs = GetComponentsInChildren<Transform>();
            foreach(var v in trs)
            {
                if (button == null && v.name == "TimeScaleButton")
                    button = v.gameObject.GetComponent<Button>();
                if (text == null && v.name == "TimeScaleText")
                    text = v.gameObject.GetComponent<TMP_Text>();
            }
            speed = 1;
            button.onClick.AddListener(SetTimeScale);
        }

        void SetTimeScale()
        {
            if (speed!=1)
            {
                speed = 1;
                InstanceFinder.GetInstance<NormalUtility>().ServerRpc_SetTimeScale(1);
                //Time.timeScale = speed;
                text.text = "二倍速";
            }
            else
            {
                speed = toBeSpeed;
                InstanceFinder.GetInstance<NormalUtility>().ServerRpc_SetTimeScale(toBeSpeed);
                //Time.timeScale = toBeSpeed;
                text.text = "正常速";
            }
        }
    }
}

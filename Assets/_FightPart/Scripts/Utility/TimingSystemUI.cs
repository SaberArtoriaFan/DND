using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace XianXia.Unit
{
    public class TimingSystemUI : NetworkBehaviour
    {
        // Start is called before the first frame update
        [SyncObject]
        private readonly SyncTimer syncTimer=new SyncTimer();
        [SyncVar]
        [ShowInInspector]
        protected float pastedTime;
        [SyncVar]
        [ShowInInspector]
        bool time_switch = false;


        public event Action OnGameOver;
        public float Remaining => syncTimer.Duration - pastedTime;

        [SerializeField]
        RectTransform root;
        const string RootName = "TimingSystemPanel";
        int minute;
        int second;
        [ShowInInspector]
        int urgentTime = 0;
        TMP_Text text;
        GameObject bg;
        StringBuilder stringBuilder;
        [ShowInInspector]
        //float timer = 0;
        int lastTimer=0;

        private void Awake()
        {
            if (InstanceFinder.GetInstance<TimingSystemUI>() != null) { Debug.LogError("Cant Find"); return; }
            InstanceFinder.RegisterInstance(this, false);
        }
        public override void OnStartClient()
        {
            base.OnStartClient();
            if (root == null)
            {
                Canvas mianCanvas = null;
                Canvas[] canvas = FindObjectsOfType<Canvas>();
                foreach(Canvas c in canvas)
                {
                    if (c.tag == "MainCanvas")
                    {
                        mianCanvas = c;
                        break;
                    }
                }
                if (mianCanvas != null)
                    root = mianCanvas.transform.Find(RootName).GetComponent<RectTransform>();
            }

            stringBuilder = new StringBuilder();
            Transform[] transforms = root.GetComponentsInChildren<Transform>();
            foreach (var v in transforms)
            {
                if (v.name == "TimingSystemText")
                {
                    text = v.GetComponent<TMP_Text>();
                    //break;
                }
                if (v.name == "TimingSystemBG")
                    bg = v.gameObject;
            }
            if (bg != null) bg.SetActive(false);
        }
        [Button]
        [Server]
        public void StartTimer(int time,int urgentTime)
        {
            syncTimer.StartTimer(time);
            syncTimer.OnChange += GameOver;
            time_switch = true;
            ORPC_StartTimer(time, urgentTime);

        }
        [ObserversRpc]
        private void ORPC_StartTimer(int time, int urgentTime)
        {
            second = time % 60;
            minute = (time - second) / 60;
            this.urgentTime = time - urgentTime;
            lastTimer = 0;
            text.color = Color.black;
            //Update();
            stringBuilder.Clear();
            stringBuilder.Append(minute.ToString().PadLeft(2, '0'));
            stringBuilder.Append(":");
            stringBuilder.Append(second.ToString().PadLeft(2, '0'));
            text.text = stringBuilder.ToString();
            stringBuilder.Clear();
            bg.SetActive(true);
        }
        [Server]
        public void StopTimer()
        {
            time_switch = false;
            syncTimer.StopTimer();
        }
        [Server]
        private void GameOver(SyncTimerOperation op, float prev, float next, bool asServer)
        {
            Debug.Log("next" + next);
            if (next == 0)
            {
                OnGameOver?.Invoke();
                OnGameOver = null;
                syncTimer.OnChange -= GameOver;
            }


        }
        private void Update()
        {
            if (IsServer)
            {
                syncTimer.Update(Time.deltaTime);
                pastedTime = syncTimer.Elapsed;
                //Debug.Log(syncTimer.Elapsed + "TT");
            }
            if (IsClient)
            {
                if (time_switch)
                {
                    float timer = pastedTime;
                    //Debug.Log(syncTimer.Elapsed+"TT");
                    if (timer >= urgentTime && urgentTime > 0)
                    {
                        urgentTime = -1;
                        text.color = Color.red;
                    }

                    if (timer >= lastTimer)
                        lastTimer++;
                    else
                        return;
                    stringBuilder.Clear();
                    stringBuilder.Append(minute.ToString().PadLeft(2, '0'));
                    stringBuilder.Append(":");
                    stringBuilder.Append(second.ToString().PadLeft(2, '0'));
                    text.text = stringBuilder.ToString();
                    stringBuilder.Clear();
                    second--;
                    if (second < 0)
                    {
                        second = 59;
                        minute--;
                        if (minute < 0)
                            EndTimer();
                    }
                }
            }


        }


        private void EndTimer()
        {
            time_switch = false;
            //syncTimer.StopTimer();
            urgentTime = 0;
        }

        /// <summary>
        /// 服务器使用的
        /// </summary>
        public static float TimeRemaining => InstanceFinder.GetInstance<TimingSystemUI>().syncTimer.Remaining;
        public static float TimePast => InstanceFinder.GetInstance<TimingSystemUI>().syncTimer.Elapsed;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Saber.ECS
{
    public abstract class GameManagerBase : Singleton<GameManagerBase>
    {
        public enum GameStatus
        {
            None,
            Starting,
            Finish
        }
        public enum GameResult
        {
            None,
            Success,
            Fail
        }

        [SerializeField]
        protected WorldBase world;
        [SerializeField]
        protected bool isPasuedWhenFinishGame = true;
        protected GameStatus gameStatus = GameStatus.None;
        protected GameResult gameResult = GameResult.None;



        public event Action OnFinishGameEvent;
        public event Action OnSuccessGameEvent;
        public event Action OnFailGameEvent;

        public GameStatus _GameStatus { get => gameStatus;  }
        public GameResult _GameResult { get => gameResult;  }
        public abstract GameResult SetGameResult();
        public virtual void ChangeGameStatus(GameStatus gameStatus)
        {
            this.gameStatus = gameStatus;
            if (this.gameStatus == GameStatus.Finish)
            {
                OnFinishGameEvent?.Invoke();
                gameResult= SetGameResult();
                switch (gameResult)
                {
                    case GameResult.Success:
                        OnSuccessGameEvent?.Invoke();
                        break;
                    case GameResult.Fail:
                        OnFailGameEvent?.Invoke();
                        break;
                }
            }

        }

        public static T FindSystem<T>() where T : class, IMono, new()
        {
            if (instance == null || instance.world == null) return null;
            return instance.world.FindSystem<T>();
        }
        public static NormalSystemBase<T> FindSystemByComponent<T>() where T : ComponentBase,new()
        {
            if (instance == null || instance.world == null) return null;
            return instance.world.FindSystemByComponent<T>();
        }
        public static NormalSystemBase<T> FindSystemByComponent<T>(T component) where T : ComponentBase, new()
        {
            if (instance == null || instance.world == null) return null;
            return instance.world.FindSystemByComponent(component);
        }
        public static void DesotryComponent<T>(T component) where T : ComponentBase, new()
        {
            if (instance == null || instance.world == null) return;
            instance.world.DesotryComponent(component);
        }
        protected virtual void Start()
        {
            if (world == null) world = FindObjectOfType<WorldBase>();

        }
    }
}

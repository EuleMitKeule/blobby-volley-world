using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game.Entities;
using Blobby.Models;
using Blobby.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeardedManStudios.Forge.Networking.Generated;
using Blobby.Game.Timing;
using UnityEngine;
using UnityEngine.AI;

namespace Blobby.Game
{
    public class OnlineMatchComponent : MatchComponent, IMatch
    {
        public event Action MatchStarted;
        public event Action MatchStopped;

        public List<PlayerData> PlayerDataList { get; set; }
        public List<GameObject> PlayerObjList { get; set; }
        
        protected override void Awake()
        {
            base.Awake();

            Debug.Log("OnlineMatchComponent.Awake()");

            NetworkManager.Instance.objectInitialized += OnObjectInitialized;

            PlayerDataList = new List<PlayerData>();
            PlayerObjList = new List<GameObject>();
            
            SubscribeEventHandler();
        }

        public void StartMatch()
        {
            Debug.Log("OnlineMatchComponent.StartMatch()");
            MainThreadManager.Run(() =>
            {
                ServerConnection.SendSound(SoundHelper.SoundClip.Whistle);

                var ballObj = NetworkManager.Instance.InstantiateBall(IsBomb ? 1 : 0).gameObject;
                ballObj.transform.SetParent(transform);

                if (MatchData.JumpMode != JumpMode.NoJump) SetState(ReadyState);
                else AutoDropTimer.Start();

                MatchStarted?.Invoke();
            });
        }

        public void Restart()
        {
            MainThreadManager.Run(() =>
            {
                for (int i = 0; i < MatchData.PlayerCount; i++)
                {
                    ServerConnection.SendPlayerPosition(Players[i].Position, i);
                    if (MatchData.PlayerCount == 4) InvokeAlpha(i, false);
                }
            
                if (!IsSingle) InvokeAlpha(2, true);
            
                LeftSwitched = false;
                RightSwitched = false;
                ScoreLeft = 0;
                ScoreRight = 0;
                HitCounts = new int[] { 0, 0, 1, 0, 0, 0 };
                LastWinner = Side.None;
            
                MatchTimer = new MatchTimer();
                ResetBallTimer = new ResetBallTimer();
                AutoDropTimer = new AutoDropTimer();
                BombTimer = new BombTimer();
            
                SubscribeTimerEvents();

                if (BallComponent) Destroy(BallComponent.gameObject);

                var ballObj = NetworkManager.Instance.InstantiateBall(IsBomb ? 1 : 0).gameObject;
                ballObj.transform.SetParent(transform);
            
                ServerConnection.SendTime(MatchTimer.MatchTime);
                ServerConnection.SendScore(ScoreLeft, ScoreRight, LastWinner);
            });
        }
        
        public void OnPlayerJoined(PlayerData playerData)
        {
            MainThreadManager.Run(() =>
            {
                var playerBehavior = NetworkManager.Instance.InstantiatePlayer();
                playerBehavior.transform.SetParent(transform);

                PlayerObjList.Add(playerBehavior.gameObject);
                PlayerDataList.Add(playerData);
            });
        }

        void OnObjectInitialized(INetworkBehavior networkBehavior, NetworkObject networkObject)
        {
            Debug.Log("OnlineMatchComponent.OnObjectInitialized");

            if (networkBehavior is PlayerBehavior playerBehavior)
            {
                
            }
            else if (networkBehavior is BallBehavior ballBehavior)
            {
                
            }
        }

        protected override void OnPlayer(PlayerComponent playerComponent)
        {
            base.OnPlayer(playerComponent);
            
            ServerConnection.SendSound(SoundHelper.SoundClip.WallHit);
        }

        protected override void OnGround()
        {
            base.OnGround();
            
            ServerConnection.SendSound(SoundHelper.SoundClip.WallHit);
        }

        protected override void OnWall()
        {
            base.OnWall();
            
            ServerConnection.SendSound(SoundHelper.SoundClip.WallHit);
        }

        protected override void OnNet()
        {
            base.OnNet();
            
            ServerConnection.SendSound(SoundHelper.SoundClip.WallHit);
        }

        protected override void OnMatchTimerTicked(int time)
        {
            MainThreadManager.Run(() =>
            {
                ServerConnection.SendTime(time);
            });
        }

        protected override void OnBombTick()
        {
            base.OnBombTick();
            
            ServerConnection.SendSound(SoundHelper.SoundClip.Bomb);
        }

        protected override void OnBombTimerStopped()
        {
            base.OnBombTimerStopped();
            
            ServerConnection.SendSound(SoundHelper.SoundClip.Explosion);
        }

        protected override async Task OnResetBallTimerStopped()
        {
            await base.OnResetBallTimerStopped();
            
            ServerConnection.SendBallPosition(BallComponent.Position);
        }

        protected override void OnScore(Side winner)
        {
            base.OnScore(winner);
            
            ServerConnection.SendScore(ScoreLeft, ScoreRight, LastWinner);
        }

        protected override void OnStop()
        {
            base.OnStop();
            
            ServerConnection.SendSound(SoundHelper.SoundClip.Whistle);
        }

        protected override void OnOver(Side winner, int scoreLeft, int scoreRight, int time)
        {
            base.OnOver(winner, scoreLeft, scoreRight, time);
            
            ServerConnection.SendOver(winner, scoreLeft, scoreRight, time);
        }

        protected override void OnAlpha(int playerNum, bool value)
        {
            base.OnAlpha(playerNum, value);
            
            ServerConnection.SendAlpha(playerNum, value);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Debug.Log("OnlineMatchComponent.OnDestroy()");

            MatchStopped?.Invoke();
        }
    }
}

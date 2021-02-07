using BeardedManStudios.Forge.Networking.Unity;
using Blobby.UserInterface;
using Blobby.Game.Entities;
using Blobby.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blobby.Game.Timing;
using UnityEngine;

namespace Blobby.Game
{
    public class LocalMatchComponent : MatchComponent, IClientMatch
    {
        [SerializeField] bool _isAiGame;

        public override MatchData MatchData => MatchHandler.MatchData;

        public event Action MatchStarted;
        public event Action<int, int, Side> ScoreChanged;
        public event Action<int> TimeChanged;
        public event Action MatchStopped;

        public bool Switched { get { return false; } }

        MatchScore _matchScore;

        protected override void Awake()
        {
            base.Awake();

            MainThreadManager.Run(() =>
            {
                MapHelper.ChangeMap(MatchData.Map);

                if (!_isAiGame)
                {
                    for (int i = 0; i < MatchData.PlayerCount; i++)
                    {
                        var playerData = new PlayerData(i, "", PanelSettings.SettingsData.Colors[i]);
                        var playerObject = Instantiate(PrefabHelper.LocalPlayer, transform);
                        var playerComponent = playerObject.GetComponent<PlayerComponent>();
                        playerComponent.PlayerData = playerData;
                        Players.Add(playerComponent);
                    }
                }
                else
                {
                    var playerData = new PlayerData(0, "", PanelSettings.SettingsData.Colors[0]);
                    var playerObject = Instantiate(PrefabHelper.LocalPlayer, transform);
                    var playerComponent = playerObject.GetComponent<PlayerComponent>();
                    playerComponent.PlayerData = playerData;
                    Players.Add(playerComponent);

                    for (var i = 1; i < MatchData.PlayerCount; i++)
                    {
                        playerData = new PlayerData(i, $"COM_{i}", PanelSettings.SettingsData.Colors[i]);
                        playerObject = Instantiate(PrefabHelper.AiPlayer, transform);
                        playerComponent = playerObject.GetComponent<PlayerComponent>();
                        playerComponent.PlayerData = playerData;
                        Players.Add(playerComponent);
                    }
                }

                var ballObject = Instantiate(IsBomb ? PrefabHelper.Bomb : PrefabHelper.Ball, transform);
                BallComponent = ballObject.GetComponent<BallComponent>();

                _matchScore = new MatchScore(this, Players[0].PlayerData, Players[1].PlayerData);

                SubscribeEventHandler();
                SubscribeBallEvents();
            });
        }

        public void StartMatch()
        {
            MainThreadManager.Run(() =>
            {
                MatchStarted?.Invoke();

                SoundHelper.PlayAudio(SoundHelper.SoundClip.Whistle);

                if (MatchData.JumpMode != JumpMode.NoJump) SetState(ReadyState);
                else AutoDropTimer.Start();

                Time.timeScale = MatchData.TimeScale;
            });
        }

        public void Restart()
        {
            MainThreadManager.Run(() =>
            {
                for (int i = 0; i < MatchData.PlayerCount; i++)
                {
                    if (MatchData.PlayerCount == 4) InvokeAlpha(i, false);
                }

                if (MatchData.PlayerCount == 4) InvokeAlpha(2, true);

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

                _matchScore = new MatchScore(this, Players[0].PlayerData, Players[1].PlayerData);

                if (BallComponent) Destroy(BallComponent.gameObject);
                var ballObject = Instantiate(PrefabHelper.Ball, transform);
                BallComponent = ballObject.GetComponent<BallComponent>();

                SubscribeBallEvents();
                SubscribeTimerEvents();

                SetState(ReadyState);
                if (MatchData.JumpMode == JumpMode.NoJump) AutoDropTimer?.Start();
            });
        }

        protected override void OnPlayer(PlayerComponent playerComponent)
        {
            base.OnPlayer(playerComponent);

            SoundHelper.PlayAudio(SoundHelper.SoundClip.WallHit);
        }

        protected override void OnGround()
        {
            base.OnGround();

            SoundHelper.PlayAudio(SoundHelper.SoundClip.WallHit);
        }

        protected override void OnWall()
        {
            base.OnWall();

            SoundHelper.PlayAudio(SoundHelper.SoundClip.WallHit);
        }

        protected override void OnNet()
        {
            base.OnNet();

            SoundHelper.PlayAudio(SoundHelper.SoundClip.WallHit);
        }

        protected override void OnMatchTimerTicked(int time)
        {
            MainThreadManager.Run(() =>
            {
                TimeChanged?.Invoke(time);
            });
        }

        protected override void OnBombTick()
        {
            base.OnBombTick();

            MainThreadManager.Run(() =>
            {
                SoundHelper.PlayAudio(SoundHelper.SoundClip.Bomb);
            });
        }

        protected override void OnBombTimerStopped()
        {
            base.OnBombTimerStopped();

            MainThreadManager.Run(() =>
            {
                SoundHelper.PlayAudio(SoundHelper.SoundClip.Explosion);
            });
        }

        protected override void OnScore(Side winner)
        {
            base.OnScore(winner);

            ScoreChanged?.Invoke(ScoreLeft, ScoreRight, LastWinner);
        }

        protected override void OnStop()
        {
            base.OnStop();

            SoundHelper.PlayAudio(SoundHelper.SoundClip.Whistle);
        }

        protected override void OnOver(Side winner, int scoreLeft, int scoreRight, int time)
        {
            base.OnOver(winner, scoreLeft, scoreRight, time);
        }

        void OnDestroy()
        {
            MatchStopped?.Invoke();
        }
    }
}

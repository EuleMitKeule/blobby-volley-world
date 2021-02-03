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
    public class LocalMatch : Match, IClientMatch
    {
        public event Action MatchStarted;
        public event Action<int, int, Side> ScoreChanged;
        public event Action<int> TimeChanged;
        public event Action MatchStopped;

        public bool Switched { get { return false; } }

        MatchScore _matchScore;

        public LocalMatch(MatchData matchData, bool isAiGame = false) : base(matchData)
        {
            MainThreadManager.Run(() =>
            {
                Time.timeScale = matchData.TimeScale;
                MapHelper.ChangeMap(matchData.Map);

                if (!isAiGame)
                {
                    for (int i = 0; i < matchData.PlayerCount; i++)
                    {
                        var playerData = new PlayerData(i, "", PanelSettings.SettingsData.Colors[i]);
                        Players.Add(new LocalPlayer(this, playerData));
                    }
                }
                else
                {
                    var playerData = new PlayerData(0, "", PanelSettings.SettingsData.Colors[0]);
                    Players.Add(new LocalPlayer(this, playerData));

                    for (int i = 1; i < matchData.PlayerCount; i++)
                    {
                        playerData = new PlayerData(i, $"COM_{i}", PanelSettings.SettingsData.Colors[i]);
                        Players.Add(new AiPlayer(this, playerData, matchData));
                    }
                }

                Ball = new LocalBall(this, matchData);

                _matchScore = new MatchScore(this, Players[0].PlayerData, Players[1].PlayerData);

                SubscribeEventHandler();
                SubscribeBallEvents();
                SubscribeTimerEvents();
            });
        }

        public void Start()
        {
            MainThreadManager.Run(() =>
            {
                MatchStarted?.Invoke();

                SoundHelper.PlayAudio(SoundHelper.SoundClip.Whistle);

                if (MatchData.JumpMode != JumpMode.NoJump) SetState(ReadyState);
                else AutoDropTimer.Start();
            });
        }

        public void Restart()
        {
            MainThreadManager.Run(() =>
            {
                for (int i = 0; i < MatchData.PlayerCount; i++)
                {
                    // Players[i].Position = MatchData.SpawnPoints[i];
                    // Players[i].LeftLimit = MatchData.LeftLimits[i];
                    // Players[i].RightLimit = MatchData.RightLimits[i];

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

                Ball?.Dispose();
                Ball = new LocalBall(this, MatchData);

                SubscribeBallEvents();
                SubscribeTimerEvents();

                SetState(ReadyState);
                if (MatchData.JumpMode == JumpMode.NoJump) AutoDropTimer?.Start();
            });
        }

        protected override void OnPlayer(Player player)
        {
            base.OnPlayer(player);

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

        public override void Dispose()
        {
            base.Dispose();

            MatchStopped?.Invoke();
        }
    }
}

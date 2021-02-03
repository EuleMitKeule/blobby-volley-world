using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game.Entities;
using Blobby.Models;
using Blobby.Networking;
using System;
using System.Threading.Tasks;
using Blobby.Game.Timing;
using UnityEngine;

namespace Blobby.Game
{
    public class OnlineMatchComponent : MatchComponent, IMatch
    {
        public event Action MatchStarted;
        public event Action MatchStopped;

        public OnlineMatchComponent(MatchData matchData) : base(matchData)
        {
            Time.timeScale = matchData.TimeScale;
            ServerConnection.SendMap(matchData.Map);

            SubscribeEventHandler();
        }

        public void Start()
        {
            MainThreadManager.Run(() =>
            {
                ServerConnection.SendSound(SoundHelper.SoundClip.Whistle);

                BallComponent = new OnlineBallComponent(this, MatchData);
                SubscribeBallEvents();

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
                    // Players[i].Position = MatchData.SpawnPoints[i];
                    // Players[i].LeftLimit = MatchData.LeftLimits[i];
                    // Players[i].RightLimit = MatchData.RightLimits[i];

                    ServerConnection.SendPlayerPosition(Players[i].Position, i);
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

                BallComponent?.Dispose();
                BallComponent = new OnlineBallComponent(this, MatchData);

                SubscribeBallEvents();
                SubscribeTimerEvents();

                SetState(ReadyState);
                if (MatchData.JumpMode == JumpMode.NoJump) AutoDropTimer?.Start();

                ServerConnection.SendTime(MatchTimer.MatchTime);
                ServerConnection.SendScore(ScoreLeft, ScoreRight, LastWinner);
            });
        }

        /// <summary>
        /// Invoked on the server when a new player joined the game 
        /// </summary>
        /// <param name="username">The players username</param>
        /// <param name="elo">The players elo</param>
        /// <param name="color">The players color</param>
        /// <param name="networkingPlayer">The associated networking player object</param>
        public void OnPlayerJoined(string username, int elo, Color color, NetworkingPlayer networkingPlayer)
        {
            var playerNum = Players.Count;
            Debug.Log($"player {playerNum} joined");

            var playerData = new PlayerData(playerNum, username, color);
            Players.Add(new OnlinePlayer(this, playerData, MatchData));

            ServerConnection.SendInfo(networkingPlayer, playerNum, username, color);
            ServerConnection.SendPlayerNum(networkingPlayer, playerNum);

            SubscribeTimerEvents();
        }

        protected override void OnPlayer(Player player)
        {
            base.OnPlayer(player);

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

        protected override void OnPlayerCounted(Player player)
        {
            base.OnPlayerCounted(player);
        }

        protected override void OnReady(Side side)
        {
            base.OnReady(side);
        }

        protected override void OnStop()
        {
            base.OnStop();

            if (MatchData.PlayerMode == PlayerMode.DoubleFixed)
            {
                foreach (var player in Players)
                {
                    ServerConnection.SendPlayerPosition(player.Position, player.PlayerData.PlayerNum);
                }
            }
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

        protected override void SubscribeEventHandler()
        {
            base.SubscribeEventHandler();
        }

        public override void Dispose()
        {
            base.Dispose();

            MatchStopped?.Invoke();
        }
    }
}

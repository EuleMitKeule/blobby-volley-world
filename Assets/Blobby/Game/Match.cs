using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Components;
using Blobby.Game;
using Blobby.Game.Entities;
using Blobby.Game.States;
using Blobby.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blobby.Game.Timing;
using UnityEngine;

namespace Blobby.Game
{
    public abstract class Match : IDisposable
    {
        public MatchData MatchData { get; private set; }
        public PhysicsSettings PhysicsSettings { get; private set; }

        public Ball Ball { get; protected set; }
        public List<Player> Players { get; protected set; } = new List<Player>();

        public MatchTimer MatchTimer { get; protected set; }
        public ResetBallTimer ResetBallTimer { get; protected set; }
        public AutoDropTimer AutoDropTimer { get; protected set; }
        public BombTimer BombTimer { get; protected set; }

        public int ScoreLeft { get; protected set; }
        public int ScoreRight { get; protected set; }

        public bool LeftSwitched { get; set; }
        public bool RightSwitched { get; set; }

        public int[] HitCounts { get; set; } = { 0, 0, 1, 0, 0, 0 };
        public float LastHit { get; set; }

        public Side LastWinner { get; protected set; } = Side.None;

        public Vector2[] SpawnPoints => MatchData.SpawnPoints;
        public float[] LeftLimits => MatchData.LeftLimits;
        public float[] RightLimits => MatchData.RightLimits;
        public bool IsSingle => MatchData.PlayerCount == 2;
        public JumpMode JumpMode => MatchData.JumpMode;
        public bool IsPogo => MatchData.JumpMode == JumpMode.Pogo;
        public bool IsJumpOverNet => MatchData.JumpOverNet;
        
        protected IMatchState _matchState;
        
        public readonly IMatchState InitState;
        public readonly IMatchState ReadyState;
        public readonly IMatchState RunningState;
        public readonly IMatchState RunningTennisState;
        public readonly IMatchState StoppedState;
        public readonly IMatchState OverState;

        public event Action<Side> Score;
        public event Action<Player> PlayerCounted;
        public event Action<Side> Ready;
        public event Action Stop;
        public event Action<Side, int, int, int> Over;
        public event Action<int, bool> Alpha;

        public Match(MatchData matchData)
        {
            MatchTimer = new MatchTimer();
            MatchData = matchData;
            PhysicsSettings = Resources.Load<PhysicsSettings>("Settings/Physics/eule");

            ResetBallTimer = new ResetBallTimer();
            AutoDropTimer = new AutoDropTimer();
            BombTimer = new BombTimer();

            #region Init States

            InitState = new MatchInitState();
            ReadyState = new MatchReadyState(this);
            RunningState = new MatchRunningState(this, matchData);
            RunningTennisState = new MatchRunningTennisState(this, matchData);
            StoppedState = new MatchStoppedState(this, matchData);
            OverState = new MatchOverState(this);

            #endregion

            SetState(InitState);
        }

        public virtual void FixedUpdate()
        {
            foreach (var player in Players)
            {
                player?.FixedUpdate();
            }
            Ball?.FixedUpdate();
        }

        public void SetState(IMatchState newState)
        {
            if (newState == null) return;

            _matchState?.ExitState();

            _matchState = newState;
            _matchState.EnterState();
        }

        protected virtual void OnPlayer(Player player) 
        {
            _matchState.OnPlayer(player);
        }

        protected virtual void OnGround()
        {
            _matchState.OnGround();
        }

        protected virtual void OnWall() 
        {

        }

        protected virtual void OnNet() 
        {

        }

        void OnSideChanged(Side newSide)
        {
            _matchState.OnSideChanged(newSide);
        }

        protected virtual void OnMatchTimerTicked(int time) { }

        protected virtual void OnBombTick() { }

        protected virtual void OnBombTimerStopped() 
        {
            _matchState.OnBombTimerStopped();
        }

        protected virtual void OnScore(Side winner)
        {
            if (winner == LastWinner)
            {
                if (winner == Side.Left) ScoreLeft++;
                else ScoreRight++;
            }

            LastWinner = winner;
        }

        protected virtual void OnPlayerCounted(Player player) 
        {
            LastHit = Time.fixedTime;

            HitCounts[player.PlayerData.PlayerNum]++;

            if (Players.Count == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    Alpha?.Invoke(i, false);
                }

                HitCounts[player.PlayerData.PlayerNum % 2 + 4]++;

                Alpha?.Invoke(player.PlayerData.PlayerNum, true);
            }
        }

        protected virtual async Task OnResetBallTimerStopped()
        {
            await WaitForGrounded();

            //MainThreadManager.Run(() =>
            //{
                SetState(ReadyState);
                Ball.SetState(Ball.Ready);
            //});
        }

        async Task WaitForGrounded()
        {
            do
            {
                if (MatchData.PlayerCount == 2)
                {
                    if (Players[LastWinner == Side.Left ? 0 : 1].IsGrounded) return;
                }
                else
                {
                    if (Players[LastWinner == Side.Left ? 0 : 1].IsGrounded && Players[LastWinner == Side.Left ? 2 : 3].IsGrounded) return;
                }

                await Task.Delay(100);
            }
            while (true);
        }

        protected virtual void OnAutoDropTimerStopped()
        {
            MainThreadManager.Run(() =>
            {
                if (MatchData.JumpMode == JumpMode.NoJump) SetState(RunningState);
            });
        }

        protected virtual void OnReady(Side side) { }

        protected virtual void OnStop()
        {
            SetState(StoppedState);
            Ball.SetState(Ball.Stopped);
        }

        protected virtual void OnOver(Side winner, int scoreLeft, int scoreRight, int time)
        {
            SetState(OverState);
            Ball?.SetState(Ball.Stopped);
        }

        protected virtual void OnAlpha(int playerNum, bool value) { }

        public void SetHitCounts(Side side, int value)
        {
            int x = side == Side.Left ? 0 : 1;

            for (int i = 0; i < 6; i++)
            {
                if (x % 2 == i % 2) HitCounts[i] = value;
            }
        }

        public bool CanHit()
        {
            return Time.fixedTime >= LastHit + MatchData.HitTreshold;
        }

        protected virtual void SubscribeEventHandler()
        {
            TimeComponent.FixedUpdateTicked += FixedUpdate;

            Ready += OnReady;
            PlayerCounted += OnPlayerCounted;
            Alpha += OnAlpha;
            Score += OnScore;
            Stop += OnStop;
            Over += OnOver;
        }

        protected void SubscribeBallEvents()
        {
            Ball.PlayerHit += OnPlayer;
            Ball.GroundHit += OnGround;
            Ball.WallHit += OnWall;
            Ball.NetHit += OnNet;
            Ball.SideChanged += OnSideChanged;
        }

        protected void SubscribeTimerEvents()
        {
            MatchTimer.MatchTimerTicked += OnMatchTimerTicked;
            BombTimer.BombTimerTicked += OnBombTick;
            BombTimer.BombTimerStopped += OnBombTimerStopped;
            ResetBallTimer.ResetBallTimerStopped += () => Task.Run(OnResetBallTimerStopped);
            AutoDropTimer.AutoDropTimerStopped += OnAutoDropTimerStopped;
        }

        public virtual void Dispose()
        {
            TimeComponent.FixedUpdateTicked -= FixedUpdate;

            Ball?.Dispose();
            foreach (var player in Players) player?.Dispose();
        }

        public void InvokeReady(Side beginningSide) => Ready?.Invoke(beginningSide);
        public void InvokePlayerCounted(Player player) => PlayerCounted?.Invoke(player);
        public void InvokeAlpha(int playerNum, bool isTransparent) => Alpha?.Invoke(playerNum, isTransparent);
        public void InvokeScore(Side lastWinner) => Score?.Invoke(lastWinner);
        public void InvokeStop() => Stop?.Invoke();
        public void InvokeOver(Side winner) => Over?.Invoke(winner, ScoreLeft, ScoreRight, MatchTimer.MatchTime);
    }
}


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
    public abstract class MatchComponent : MonoBehaviour
    {
        public const int WIN_SCORE = 16;

        public virtual MatchData MatchData => null;

        public BallComponent BallComponent { get; protected set; }
        public List<PlayerComponent> Players { get; protected set; } = new List<PlayerComponent>();

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
        public PlayerMode PlayerMode => MatchData.PlayerMode;
        public bool IsSingle => MatchData.PlayerCount == 2;
        public JumpMode JumpMode => MatchData.JumpMode;
        public bool IsPogo => MatchData.JumpMode == JumpMode.Pogo;
        public bool IsJumpOverNet => MatchData.JumpOverNet;
        
        protected IMatchState _matchState;
        
        public IMatchState InitState {get; private set; }
        public IMatchState ReadyState {get; private set; }
        public IMatchState RunningState {get; private set; }
        public IMatchState RunningTennisState {get; private set; }
        public IMatchState StoppedState {get; private set; }
        public IMatchState OverState {get; private set; }

        public event Action<Side> Score;
        public event Action<PlayerComponent> PlayerCounted;
        public event Action<Side> Ready;
        public event Action Stop;
        public event Action<Side, int, int, int> Over;
        public event Action<int, bool> Alpha;

        protected virtual void Awake()
        {
            MatchTimer = new MatchTimer();
            ResetBallTimer = new ResetBallTimer();
            AutoDropTimer = new AutoDropTimer();
            BombTimer = new BombTimer();

            InitState = new MatchInitState();
            ReadyState = new MatchReadyState(this);
            RunningState = new MatchRunningState(this);
            RunningTennisState = new MatchRunningTennisState(this);
            StoppedState = new MatchStoppedState(this);
            OverState = new MatchOverState(this);

            SetState(InitState);
        }

        public void SetState(IMatchState newState)
        {
            if (newState == null) return;

            _matchState?.ExitState();

            _matchState = newState;
            _matchState.EnterState();
        }

        protected virtual void OnPlayer(PlayerComponent playerComponent)
        {
            _matchState.OnPlayer(playerComponent);
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

        protected virtual void OnPlayerCounted(PlayerComponent playerComponent)
        {
            LastHit = Time.fixedTime;

            HitCounts[playerComponent.PlayerData.PlayerNum]++;

            if (Players.Count != 4) return;

            for (var i = 0; i < 4; i++)
            {
                Alpha?.Invoke(i, false);
            }

            HitCounts[playerComponent.PlayerData.PlayerNum % 2 + 4]++;

            Alpha?.Invoke(playerComponent.PlayerData.PlayerNum, true);
        }

        protected virtual async Task OnResetBallTimerStopped()
        {
            await WaitForGrounded();

            SetState(ReadyState);
            BallComponent.SetState(BallComponent.Ready);
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

        void OnAutoDropTimerStopped()
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
            BallComponent.SetState(BallComponent.Stopped);
        }

        protected virtual void OnOver(Side winner, int scoreLeft, int scoreRight, int time)
        {
            SetState(OverState);
            BallComponent?.SetState(BallComponent.Stopped);
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
            Ready += OnReady;
            PlayerCounted += OnPlayerCounted;
            Alpha += OnAlpha;
            Score += OnScore;
            Stop += OnStop;
            Over += OnOver;
        }

        protected void SubscribeBallEvents()
        {
            BallComponent.PlayerHit += OnPlayer;
            BallComponent.GroundHit += OnGround;
            BallComponent.WallHit += OnWall;
            BallComponent.NetHit += OnNet;
            BallComponent.SideChanged += OnSideChanged;
        }

        protected void SubscribeTimerEvents()
        {
            MatchTimer.MatchTimerTicked += OnMatchTimerTicked;
            BombTimer.BombTimerTicked += OnBombTick;
            BombTimer.BombTimerStopped += OnBombTimerStopped;
            ResetBallTimer.ResetBallTimerStopped += () => Task.Run(OnResetBallTimerStopped);
            AutoDropTimer.AutoDropTimerStopped += OnAutoDropTimerStopped;
        }

        void OnDestroy()
        {
            Destroy(BallComponent.gameObject);
            foreach (var player in Players) Destroy(player.gameObject);
        }

        public void InvokeReady(Side beginningSide) => Ready?.Invoke(beginningSide);
        public void InvokePlayerCounted(PlayerComponent playerComponent) => PlayerCounted?.Invoke(playerComponent);
        public void InvokeAlpha(int playerNum, bool isTransparent) => Alpha?.Invoke(playerNum, isTransparent);
        public void InvokeScore(Side lastWinner) => Score?.Invoke(lastWinner);
        public void InvokeStop() => Stop?.Invoke();
        public void InvokeOver(Side winner) => Over?.Invoke(winner, ScoreLeft, ScoreRight, MatchTimer.MatchTime);
    }
}


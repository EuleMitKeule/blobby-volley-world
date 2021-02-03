using Blobby.Game.Entities.States;
using Blobby.Models;
using Blobby.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Blobby.Game.Entities
{
    public class AiPlayer : Player
    {
        public AiData AiData { get; private set; }

        public AiState Defensive { get; private set; }
        public AiState Offensive { get; private set; }
        public AiState Idle { get; private set; }

        public AiState AiState { get; private set; }

        public bool[] IsTransparent { get; private set; } = { true, true, true, true };

        Animator _animator;
        LocalMatch _localMatch;

        static GameObject Prefab => PrefabHelper.LocalPlayer;
     
        public AiPlayer(LocalMatch match, PlayerData playerData, MatchData matchData) : base(match, playerData, Prefab)
        {
            _localMatch = match;

            AiData = new AiData()
            {
                DefensivePositionX = PlayerData.Side == Side.Left ? -12f : 12f,
                Threshold = 0.5f,
                OffensiveOffset = PlayerData.Side == Side.Left ? -0.75f : 0.75f
            };

            Defensive = new AiDefensiveState(this, match);
            Offensive = new AiOffensiveState(this, match);
            Idle = new AiIdleState(this, match);
            
            if (matchData.PlayerMode == PlayerMode.Ghost && PlayerData.Side == Side.Right)
            {
                PlayerGraphics.SetInvisible(true);
            }

            _animator = PlayerObj.GetComponent<Animator>();

            SubscribeEventHandler();
        }

        protected override void OnStop()
        {
            SetState(Idle);
        }

        protected override void OnOver(Side side, int scoreLeft, int scoreRight, int time)
        {
            base.OnOver(side, scoreLeft, scoreRight, time);

            SetState(Idle);
        }

        protected override void OnReady(Side side)
        {
            base.OnReady(side);

            if (Match.IsSingle)
            {
                if (side == PlayerData.Side)
                {
                    SetState(Offensive);
                }
                else SetState(Defensive);
            }
            else
            {
                if (!IsTransparent[PlayerData.PlayerNum] && IsTransparent[(PlayerData.PlayerNum + 2) % 4]) SetState(Offensive);
                else SetState(Defensive);
            }
        }

        void OnAlpha(int playerNum, bool isAlpha)
        {
            IsTransparent[playerNum] = isAlpha;

            if (playerNum == PlayerData.PlayerNum)
            {
                PlayerGraphics.SetAlpha(isAlpha);
            }
        }

        void OnPlayerCounted(Player player)
        {
            if (player == this && !Match.IsSingle)
            {
                SetState(Defensive);
            }
            else if (player is AiPlayer aiPlayer)
            {
                var playerNum = aiPlayer.PlayerData.PlayerNum;
                if (playerNum == (PlayerData.PlayerNum + 2) % 4)
                {
                    SetState(Offensive);
                }
            }
            else if (player is LocalPlayer localPlayer)
            {
                var playerNum = localPlayer.PlayerData.PlayerNum;
                if (playerNum == (PlayerData.PlayerNum + 2) % 4)
                {
                    SetState(Offensive);
                }
            }
        }

        public void SetState(AiState state)
        {
            if (state == null) return;

            AiState?.ExitState();

            AiState = state;
            AiState?.EnterState();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            AiState?.FixedUpdate();
        }

        void SubscribeEventHandler()
        {
            _localMatch.Alpha += OnAlpha;
            _localMatch.PlayerCounted += OnPlayerCounted;
        }

        public override void Dispose()
        {
            base.Dispose();

            _localMatch.Alpha -= OnAlpha;
            _localMatch.PlayerCounted -= OnPlayerCounted;
        }
    }
}

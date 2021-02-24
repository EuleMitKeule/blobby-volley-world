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
    public class AiPlayerComponent : PlayerComponent
    {
        public AiData AiData { get; private set; }

        public AiState Defensive { get; private set; }
        public AiState Offensive { get; private set; }
        public AiState Idle { get; private set; }

        public AiState AiState { get; private set; }

        public bool[] IsTransparent { get; private set; } = { true, true, true, true };
        public override bool IsInvisible => MatchComponent.PlayerMode == PlayerMode.Ghost && OwnSide == Side.Right;

        static GameObject Prefab => PrefabHelper.LocalPlayer;

        protected override void Awake()
        {
            base.Awake();

            SubscribeEventHandler();

            Defensive = new AiDefensiveState(this, MatchComponent);
            Offensive = new AiOffensiveState(this, MatchComponent);
            Idle = new AiIdleState(this, MatchComponent);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            AiState?.FixedUpdate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            MatchComponent.PlayerCounted -= OnPlayerCounted;
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

        protected override void OnReady(Side givingSide)
        {
            base.OnReady(givingSide);

            if (MatchComponent.IsSingle)
            {
                if (givingSide == PlayerData.Side)
                {
                    SetState(Offensive);
                }
                else SetState(Defensive);
            }
            else
            {
                switch (PlayerData.PlayerNum)
                {
                    case 1:
                        if (givingSide == PlayerData.Side) SetState(MatchComponent.IsRightSwitched ? Idle : Offensive);
                        else SetState(Defensive);
                        
                        break;
                    
                    case 2:
                        if (givingSide == PlayerData.Side) SetState(MatchComponent.IsLeftSwitched ? Offensive : Idle);
                        else SetState(Defensive);
                            
                        break;

                    case 3:
                        if (givingSide == PlayerData.Side) SetState(MatchComponent.IsRightSwitched ? Offensive : Idle);
                        else SetState(Defensive);
                            
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override void OnAlpha(int playerNum, bool isTransparent)
        {
            base.OnAlpha(playerNum, isTransparent);
            
            IsTransparent[playerNum] = isTransparent;
        }

        void OnPlayerCounted(PlayerComponent playerComponent)
        {
            if (playerComponent == this && !MatchComponent.IsSingle)
            {
                SetState(Defensive);
            }
            else if (playerComponent is AiPlayerComponent aiPlayer)
            {
                var playerNum = aiPlayer.PlayerData.PlayerNum;
                if (playerNum == (PlayerData.PlayerNum + 2) % 4)
                {
                    SetState(Offensive);
                }
            }
            else if (playerComponent is LocalPlayerComponent localPlayer)
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

        void OnPlayerDataChanged(PlayerData playerData)
        {
            AiData = new AiData()
            {
                DefensivePositionX = PlayerData.Side == Side.Left ? -12f : 12f,
                Threshold = 0.5f,
                OffensiveOffset = PlayerData.Side == Side.Left ? -0.75f : 0.75f,
            };
        }

        void SubscribeEventHandler()
        {
            PlayerDataChanged += OnPlayerDataChanged;
            MatchComponent.PlayerCounted += OnPlayerCounted;
        }
    }
}

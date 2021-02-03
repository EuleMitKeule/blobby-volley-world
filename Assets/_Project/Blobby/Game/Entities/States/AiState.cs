using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobby.Game.Entities.States
{
    public abstract class AiState
    {
        protected AiPlayer _aiPlayer;
        protected MatchComponent MatchComponent;

        public AiState(AiPlayer aiPlayer, MatchComponent matchComponent)
        {
            _aiPlayer = aiPlayer;
            MatchComponent = matchComponent;
        }

        public virtual void FixedUpdate()
        {

        }

        public virtual void EnterState()
        {
            MatchComponent.BallComponent.SideChanged += OnBallSideChanged;
        }

        public virtual void ExitState()
        {
            MatchComponent.BallComponent.SideChanged -= OnBallSideChanged;
        }

        protected virtual void OnBallSideChanged(Side side)
        {
            if (side == _aiPlayer.PlayerData.Side)
            {
                if (_aiPlayer.PlayerData.PlayerNum > 1)
                {
                    if (!_aiPlayer.IsTransparent[_aiPlayer.PlayerData.PlayerNum] && !_aiPlayer.IsTransparent[(_aiPlayer.PlayerData.PlayerNum + 2) % 4])
                    {
                        _aiPlayer.SetState(_aiPlayer.Defensive);
                        return;
                    }
                    else _aiPlayer.SetState(_aiPlayer.Offensive);
                }
                else _aiPlayer.SetState(_aiPlayer.Offensive);
            }
            else _aiPlayer.SetState(_aiPlayer.Defensive);
        }
    }
}

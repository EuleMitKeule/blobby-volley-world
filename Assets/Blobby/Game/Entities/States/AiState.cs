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
        protected Match _match;

        public AiState(AiPlayer aiPlayer, Match match)
        {
            _aiPlayer = aiPlayer;
            _match = match;
        }

        public virtual void FixedUpdate()
        {

        }

        public virtual void EnterState()
        {
            _match.Ball.SideChanged += OnBallSideChanged;
        }

        public virtual void ExitState()
        {
            _match.Ball.SideChanged -= OnBallSideChanged;
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

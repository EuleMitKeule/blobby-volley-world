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
        protected AiPlayerComponent AiPlayerComponent;
        protected MatchComponent MatchComponent;

        public AiState(AiPlayerComponent aiPlayerComponent, MatchComponent matchComponent)
        {
            AiPlayerComponent = aiPlayerComponent;
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
            if (side == AiPlayerComponent.PlayerData.Side)
            {
                if (AiPlayerComponent.PlayerData.PlayerNum > 1)
                {
                    if (!AiPlayerComponent.IsTransparent[AiPlayerComponent.PlayerData.PlayerNum] && !AiPlayerComponent.IsTransparent[(AiPlayerComponent.PlayerData.PlayerNum + 2) % 4])
                    {
                        AiPlayerComponent.SetState(AiPlayerComponent.Defensive);
                        return;
                    }
                    else AiPlayerComponent.SetState(AiPlayerComponent.Offensive);
                }
                else AiPlayerComponent.SetState(AiPlayerComponent.Offensive);
            }
            else AiPlayerComponent.SetState(AiPlayerComponent.Defensive);
        }
    }
}

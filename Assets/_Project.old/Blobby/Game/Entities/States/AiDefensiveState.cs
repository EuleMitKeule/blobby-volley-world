using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blobby.Game.Entities.States
{
    public class AiDefensiveState : AiState
    {
        public AiDefensiveState(AiPlayerComponent aiPlayerComponent, MatchComponent matchComponent) : base(aiPlayerComponent, matchComponent)
        {
            
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (AiPlayerComponent.Position.x < AiPlayerComponent.AiData.DefensivePositionX - AiPlayerComponent.AiData.Threshold)
            {
                AiPlayerComponent.KeyPressed[0] = false;
                AiPlayerComponent.KeyPressed[1] = false;
                AiPlayerComponent.KeyPressed[2] = true;
            }
            else if (AiPlayerComponent.Position.x > AiPlayerComponent.AiData.DefensivePositionX + AiPlayerComponent.AiData.Threshold)
            {
                AiPlayerComponent.KeyPressed[0] = false;
                AiPlayerComponent.KeyPressed[1] = true;
                AiPlayerComponent.KeyPressed[2] = false;
            }
            else
            {
                AiPlayerComponent.KeyPressed[0] = false;
                AiPlayerComponent.KeyPressed[1] = false;
                AiPlayerComponent.KeyPressed[2] = false;
            }
        }
    }
}

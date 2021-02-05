using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blobby.Game.Entities.States
{
    public class AiOffensiveState : AiState
    {
        public AiOffensiveState(AiPlayerComponent aiPlayerComponent, MatchComponent matchComponent) : base(aiPlayerComponent, matchComponent)
        {

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (AiPlayerComponent.Position.x < MatchComponent.BallComponent.Position.x + AiPlayerComponent.AiData.OffensiveOffset - AiPlayerComponent.AiData.Threshold)
            {
                //Debug.Log("links");
                //_aiPlayer.KeyPressed[0] = false;
                AiPlayerComponent.KeyPressed[1] = false;
                AiPlayerComponent.KeyPressed[2] = true;
            }
            else if (AiPlayerComponent.Position.x > MatchComponent.BallComponent.Position.x + AiPlayerComponent.AiData.OffensiveOffset + AiPlayerComponent.AiData.Threshold)
            {
                //Debug.Log("rechts");
                //_aiPlayer.KeyPressed[0] = false;
                AiPlayerComponent.KeyPressed[1] = true;
                AiPlayerComponent.KeyPressed[2] = false;
            }
            else
            {
                //Debug.Log("sprung");
                AiPlayerComponent.KeyPressed[0] = true;
                AiPlayerComponent.KeyPressed[1] = false;
                AiPlayerComponent.KeyPressed[2] = false;
            }
        }
    }
}

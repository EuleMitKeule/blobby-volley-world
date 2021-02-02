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
        public AiOffensiveState(AiPlayer aiPlayer, Match match) : base(aiPlayer, match)
        {

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_aiPlayer.Position.x < _match.Ball.Position.x + _aiPlayer.AiData.OffensiveOffset - _aiPlayer.AiData.Threshold)
            {
                //Debug.Log("links");
                //_aiPlayer.KeyPressed[0] = false;
                _aiPlayer.KeyPressed[1] = false;
                _aiPlayer.KeyPressed[2] = true;
            }
            else if (_aiPlayer.Position.x > _match.Ball.Position.x + _aiPlayer.AiData.OffensiveOffset + _aiPlayer.AiData.Threshold)
            {
                //Debug.Log("rechts");
                //_aiPlayer.KeyPressed[0] = false;
                _aiPlayer.KeyPressed[1] = true;
                _aiPlayer.KeyPressed[2] = false;
            }
            else
            {
                //Debug.Log("sprung");
                _aiPlayer.KeyPressed[0] = true;
                _aiPlayer.KeyPressed[1] = false;
                _aiPlayer.KeyPressed[2] = false;
            }
        }
    }
}

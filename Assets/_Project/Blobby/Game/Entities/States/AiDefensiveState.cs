using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobby.Game.Entities.States
{
    public class AiDefensiveState : AiState
    {
        public AiDefensiveState(AiPlayer aiPlayer, Match match) : base(aiPlayer, match)
        {
            
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_aiPlayer.Position.x < _aiPlayer.AiData.DefensivePositionX - _aiPlayer.AiData.Threshold)
            {
                _aiPlayer.KeyPressed[0] = false;
                _aiPlayer.KeyPressed[1] = false;
                _aiPlayer.KeyPressed[2] = true;
            }
            else if (_aiPlayer.Position.x > _aiPlayer.AiData.DefensivePositionX + _aiPlayer.AiData.Threshold)
            {
                _aiPlayer.KeyPressed[0] = false;
                _aiPlayer.KeyPressed[1] = true;
                _aiPlayer.KeyPressed[2] = false;
            }
            else
            {
                _aiPlayer.KeyPressed[0] = false;
                _aiPlayer.KeyPressed[1] = false;
                _aiPlayer.KeyPressed[2] = false;
            }
        }
    }
}

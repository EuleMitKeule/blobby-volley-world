using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobby.Game.Entities.States
{
    public class AiIdleState : AiState
    {
        public AiIdleState(AiPlayer aiPlayer, Match match) : base(aiPlayer, match) { }

        public override void EnterState()
        {
            _aiPlayer.KeyPressed[0] = false;
            _aiPlayer.KeyPressed[1] = false;
            _aiPlayer.KeyPressed[2] = false;
        }

        public override void ExitState()
        {

        }

        public override void FixedUpdate()
        {

        }

        protected override void OnBallSideChanged(Side side)
        {

        }
    }
}

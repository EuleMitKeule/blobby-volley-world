using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blobby.Game.Entities.States
{
    public class AiIdleState : AiState
    {
        public AiIdleState(AiPlayerComponent aiPlayerComponent, MatchComponent matchComponent) : base(aiPlayerComponent, matchComponent) { }

        public override void EnterState()
        {
            AiPlayerComponent.KeyPressed[0] = false;
            AiPlayerComponent.KeyPressed[1] = false;
            AiPlayerComponent.KeyPressed[2] = false;
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

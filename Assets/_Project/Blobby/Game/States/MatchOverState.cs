using Blobby.Game.Entities;
using Blobby.Models;

namespace Blobby.Game.States
{
    public class MatchOverState : IMatchState
    {
        MatchComponent _matchComponent;

        public MatchOverState(MatchComponent matchComponent)
        {
            _matchComponent = matchComponent;
        }

        public void EnterState()
        {
            _matchComponent.MatchTimer?.Stop();
            _matchComponent.BombTimer?.Stop();
            _matchComponent.ResetBallTimer?.Stop();
        }

        public void ExitState()
        {

        }

        public void OnBombTimerStopped() 
        {

        }

        public void OnGround()
        {

        }

        public void OnPlayer(PlayerComponent playerComponent)
        {

        }

        public void OnSideChanged(Side newSide)
        {

        }
    }
}
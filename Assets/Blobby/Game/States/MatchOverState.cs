using Blobby.Game.Entities;
using Blobby.Models;

namespace Blobby.Game.States
{
    public class MatchOverState : IMatchState
    {
        Match _match;

        public MatchOverState(Match match)
        {
            _match = match;
        }

        public void EnterState()
        {
            _match.MatchTimer?.Stop();
            _match.BombTimer?.Stop();
            _match.ResetBallTimer?.Stop();
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

        public void OnPlayer(Player player)
        {

        }

        public void OnSideChanged(Side newSide)
        {

        }
    }
}
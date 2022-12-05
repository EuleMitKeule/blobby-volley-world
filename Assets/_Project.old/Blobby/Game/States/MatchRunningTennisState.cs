using Blobby.Game.Entities;

namespace Blobby.Game.States
{
    public class MatchRunningTennisState : IMatchState
    {
        MatchComponent _matchComponent;

        public MatchRunningTennisState(MatchComponent matchComponent) => (_matchComponent) = (matchComponent);

        public void OnPlayer(PlayerComponent playerComponent)
        {
            //set other sides hit count to 0
            _matchComponent.SetHitCounts(playerComponent.EnemySide, 0);

            if (!_matchComponent.IsSingle)
            {
                for (var i = 0; i < _matchComponent.Players.Count; i++)
                {
                    _matchComponent.InvokeAlpha(i, _matchComponent.HitCounts[i] != 0);
                }
            }

            //check if hit can count again
            if (!_matchComponent.CanHit()) return;

            _matchComponent.InvokePlayerCounted(playerComponent);

            //check if hit count exceeds max allowed hit count
            for (int i = 0; i < 6; i++)
            {
                if (_matchComponent.HitCounts[i] > _matchComponent.MatchData.AllowedHits[i])
                {
                    _matchComponent.InvokeStop();
                    break;
                }
            }

            //make ground deadly
            if (_matchComponent.BallComponent.Position.x <= 0f && playerComponent.PlayerData.PlayerNum % 2 == 0 ||
                _matchComponent.BallComponent.Position.x > 0f && playerComponent.PlayerData.PlayerNum % 2 == 1)
            {
                _matchComponent.SetState(_matchComponent.RunningState);
                _matchComponent.BallComponent.SetState(_matchComponent.BallComponent.Running);
            }
        }

        public void OnGround()
        {
            _matchComponent.SetState(_matchComponent.RunningState);
        }

        public void OnSideChanged(Side newSide)
        {
            var otherSide = (Side)(((int)newSide + 1) % 2);

            _matchComponent.SetHitCounts(otherSide, 0);
        }

        public void OnBombTimerStopped()
        {
            _matchComponent.SetState(_matchComponent.StoppedState);
        }

        public void EnterState()
        {

        }

        public void ExitState()
        {

        }
    }
}

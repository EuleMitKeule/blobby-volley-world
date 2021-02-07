using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game.Entities;
using Blobby.Models;
using UnityEngine;

namespace Blobby.Game.States
{
    public class MatchRunningState : IMatchState
    {
        MatchComponent _matchComponent;

        public MatchRunningState(MatchComponent matchComponent) => (_matchComponent) = (matchComponent);

        public void OnPlayer(PlayerComponent playerComponent)
        {
            _matchComponent.SetHitCounts(playerComponent.EnemySide, 0);

            //set other players hit count to 0
            if (_matchComponent.Players.Count == 4) _matchComponent.HitCounts[playerComponent.TeamBlobNum] = 0;

            if (!_matchComponent.IsSingle)
            {
                for (int i = 0; i < _matchComponent.Players.Count; i++)
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
        }

        public void OnGround()
        {
            _matchComponent.InvokeStop();
        }

        public void OnSideChanged(Side newSide)
        {
            var otherSide = (Side)(((int)newSide + 1) % 2);

            _matchComponent.SetHitCounts(otherSide, 0);

            //change to tennis state
            if (_matchComponent.MatchData.GameMode == GameMode.Tennis)
            {
                _matchComponent.SetState(_matchComponent.RunningTennisState);
                _matchComponent.BallComponent.SetState(_matchComponent.BallComponent.RunningTennis);
            }
        }

        public void EnterState()
        {
            _matchComponent.MatchTimer?.Start();
        
            if (_matchComponent.MatchData.GameMode == GameMode.Bomb)
                _matchComponent.BombTimer?.Start();
        }

        public void OnBombTimerStopped()
        {
            MainThreadManager.Run(() =>
            {
                _matchComponent.SetState(_matchComponent.StoppedState);
            });
        }

        public void ExitState()
        {

        }
    }
}
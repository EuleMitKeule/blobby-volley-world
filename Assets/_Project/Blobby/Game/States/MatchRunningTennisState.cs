using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game.Entities;
using Blobby.Models;
using UnityEngine;

namespace Blobby.Game.States
{
    public class MatchRunningTennisState : IMatchState
    {
        Match _match;
        MatchData _matchData;

        public MatchRunningTennisState(Match match, MatchData matchData) => (_match, _matchData) = (match, matchData);

        public void OnPlayer(Player player)
        {
            //set other sides hit count to 0
            _match.SetHitCounts(player.EnemySide, 0);

            //check if hit can count again
            if (!_match.CanHit()) return;

            _match.InvokePlayerCounted(player);

            //check if hit count exceeds max allowed hit count
            for (int i = 0; i < 6; i++)
            {
                if (_match.HitCounts[i] > _match.MatchData.AllowedHits[i])
                {
                    _match.InvokeStop();
                    break;
                }
            }

            //make ground deadly
            if (_match.Ball.Position.x <= 0f && player.PlayerData.PlayerNum % 2 == 0 || 
                _match.Ball.Position.x > 0f && player.PlayerData.PlayerNum % 2 == 1)
            {
                _match.SetState(_match.RunningState);
                _match.Ball.SetState(_match.Ball.Running);
            }
        }

        public void OnGround()
        {
            _match.SetState(_match.RunningState);
        }

        public void OnSideChanged(Side newSide)
        {
            var otherSide = (Side)(((int)newSide + 1) % 2);

            _match.SetHitCounts(otherSide, 0);
        }

        public void OnBombTimerStopped()
        {
            MainThreadManager.Run(() =>
            {
                _match.SetState(_match.StoppedState);
            });
        }

        public void EnterState()
        {

        }

        public void ExitState()
        {

        }
    }
}

using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game.Entities;
using Blobby.Models;
using UnityEngine;

namespace Blobby.Game.States
{
    public class MatchRunningState : IMatchState
    {
        Match _match;
        MatchData _matchData;

        public MatchRunningState(Match match, MatchData matchData) => (_match, _matchData) = (match, matchData);

        public void OnPlayer(Player player)
        {
            _match.SetHitCounts(player.EnemySide, 0);

            //set other players hit count to 0
            if (_match.Players.Count == 4) _match.HitCounts[(player.PlayerData.PlayerNum + 2) % 4] = 0;

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
        }

        public void OnGround()
        {
            _match.InvokeStop();
        }

        public void OnSideChanged(Side newSide)
        {
            var otherSide = (Side)(((int)newSide + 1) % 2);

            _match.SetHitCounts(otherSide, 0);

            //change to tennis state
            if (_match.MatchData.GameMode == GameMode.Tennis)
            {
                _match.SetState(_match.RunningTennisState);
                _match.Ball.SetState(_match.Ball.RunningTennis);
            }
        }

        public void EnterState()
        {
            _match.MatchTimer?.Start();
        
            if (_match.MatchData.GameMode == GameMode.Bomb)
                _match.BombTimer?.Start();
        }

        public void OnBombTimerStopped()
        {
            MainThreadManager.Run(() =>
            {
                _match.SetState(_match.StoppedState);
            });
        }

        public void ExitState()
        {

        }
    }
}
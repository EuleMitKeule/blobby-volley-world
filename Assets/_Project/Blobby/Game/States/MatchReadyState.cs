using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game.Entities;
using Blobby.Models;
using UnityEngine;

namespace Blobby.Game.States
{
    public class MatchReadyState : IMatchState
    {
        Match _match;

        public MatchReadyState(Match match) => _match = match;

        public void EnterState()
        {
            _match.InvokeReady(_match.LastWinner);
        }

        public void ExitState()
        {

        }

        public void OnPlayer(Player player)
        {
            if (_match.MatchData.PlayerMode == PlayerMode.Double ||
                _match.MatchData.PlayerMode == PlayerMode.DoubleFixed)
            {
                _match.HitCounts[(player.PlayerData.PlayerNum + 2) % 4] = 0;
            }

            _match.InvokePlayerCounted(player);

            for (int i = 0; i < 6; i++)
            {
                if (_match.HitCounts[i] > _match.MatchData.AllowedHits[i])
                {
                    _match.InvokeStop();
                    return;
                }
            }

            _match.SetState(_match.RunningState);
        }

        public void OnBombTimerStopped()
        {
            MainThreadManager.Run(() =>
            {
                _match.SetState(_match.StoppedState);
            });
        }

        public void OnGround()
        {

        }

        public void OnSideChanged(Side side)
        {

        }
    }
}

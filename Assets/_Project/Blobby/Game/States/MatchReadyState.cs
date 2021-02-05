using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game.Entities;
using Blobby.Models;
using UnityEngine;

namespace Blobby.Game.States
{
    public class MatchReadyState : IMatchState
    {
        MatchComponent _matchComponent;

        public MatchReadyState(MatchComponent matchComponent) => _matchComponent = matchComponent;

        public void EnterState()
        {
            _matchComponent.InvokeReady(_matchComponent.LastWinner);
        }

        public void ExitState()
        {

        }

        public void OnPlayer(PlayerComponent playerComponent)
        {
            if (_matchComponent.MatchData.PlayerMode == PlayerMode.Double ||
                _matchComponent.MatchData.PlayerMode == PlayerMode.DoubleFixed)
            {
                _matchComponent.HitCounts[(playerComponent.PlayerData.PlayerNum + 2) % 4] = 0;
            }

            _matchComponent.InvokePlayerCounted(playerComponent);

            for (int i = 0; i < 6; i++)
            {
                if (_matchComponent.HitCounts[i] > _matchComponent.MatchData.AllowedHits[i])
                {
                    _matchComponent.InvokeStop();
                    return;
                }
            }

            _matchComponent.SetState(_matchComponent.RunningState);
        }

        public void OnBombTimerStopped()
        {
            MainThreadManager.Run(() =>
            {
                _matchComponent.SetState(_matchComponent.StoppedState);
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

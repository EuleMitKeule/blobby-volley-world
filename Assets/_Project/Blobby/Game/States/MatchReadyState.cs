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
            //double mode alpha and side switch
            if (_matchComponent.Players.Count == 4)
            {
                if (_matchComponent.CurrentWinner != _matchComponent.LastWinner && _matchComponent.LastWinner != Side.None)
                {
                    if (_matchComponent.CurrentWinner == Side.Left) _matchComponent.IsLeftSwitched = !_matchComponent.IsLeftSwitched;
                    else _matchComponent.IsRightSwitched = !_matchComponent.IsRightSwitched;
                }

                int notGivingPlayerNum = _matchComponent.CurrentWinner == Side.Left ? (_matchComponent.IsLeftSwitched ? 0 : 2) : (_matchComponent.IsRightSwitched ? 1 : 3);
                _matchComponent.HitCounts[notGivingPlayerNum] = 1;

                for (int i = 0; i < 4; i++) _matchComponent.InvokeAlpha(i, false);

                _matchComponent.InvokeAlpha(notGivingPlayerNum, true);
            }

            _matchComponent.LastWinner = _matchComponent.CurrentWinner;
            _matchComponent.CurrentWinner = Side.None;
            
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

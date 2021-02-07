using Blobby.Game.Entities;
using Blobby.Models;
using UnityEngine;

namespace Blobby.Game.States
{
    public class MatchStoppedState : IMatchState
    {
        MatchComponent _matchComponent;

        public MatchStoppedState(MatchComponent matchComponent) => (_matchComponent) = (matchComponent);

        public void EnterState()
        {
            _matchComponent.MatchTimer?.Stop();
            _matchComponent.BombTimer?.Stop();

            var winner = Side.None;

            //determine winner by hit counts
            for (int i = 0; i < _matchComponent.HitCounts.Length; i++)
            {
                if (_matchComponent.HitCounts[i] > _matchComponent.MatchData.AllowedHits[i])
                {
                    winner = i % 2 == 0 ? Side.Right : Side.Left;
                    break;
                }
            }

            //determine winner by ground
            if (winner == Side.None)
            {
                winner = _matchComponent.BallComponent.Position.x > 0f
                    ? Side.Left : Side.Right;
            }

            _matchComponent.HitCounts = new int[6];

            //double mode alpha and side switch
            if (_matchComponent.Players.Count == 4)
            {
                if (winner != _matchComponent.LastWinner && _matchComponent.LastWinner != Side.None)
                {
                    if (winner == Side.Left) _matchComponent.LeftSwitched = !_matchComponent.LeftSwitched;
                    else _matchComponent.RightSwitched = !_matchComponent.RightSwitched;
                }

                int notGivingPlayerNum = winner == Side.Left ? (_matchComponent.LeftSwitched ? 0 : 2) : (_matchComponent.RightSwitched ? 1 : 3);
                _matchComponent.HitCounts[notGivingPlayerNum] = 1;

                for (int i = 0; i < 4; i++) _matchComponent.InvokeAlpha(i, false);

                _matchComponent.InvokeAlpha(notGivingPlayerNum, true);
            }

            _matchComponent.InvokeScore(winner);

            if (_matchComponent.ScoreLeft >= _matchComponent.WinningScore) _matchComponent.InvokeOver(Side.Left);
            else if (_matchComponent.ScoreRight >= _matchComponent.WinningScore) _matchComponent.InvokeOver(Side.Right);
            else
            {
                _matchComponent.ResetBallTimer?.Start();
                if (_matchComponent.MatchData.JumpMode == JumpMode.NoJump) _matchComponent.AutoDropTimer?.Start();
            }
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

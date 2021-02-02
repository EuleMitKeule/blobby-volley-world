using Blobby.Game.Entities;
using Blobby.Models;
using UnityEngine;

namespace Blobby.Game.States
{
    public class MatchStoppedState : IMatchState
    {
        Match _match;
        MatchData _matchData;

        public MatchStoppedState(Match match, MatchData matchData) => (_match, _matchData) = (match, matchData);

        public void EnterState()
        {
            _match.MatchTimer?.Stop();
            _match.BombTimer?.Stop();

            var winner = Side.None;

            //determine winner by hit counts
            for (int i = 0; i < _match.HitCounts.Length; i++)
            {
                if (_match.HitCounts[i] > _match.MatchData.AllowedHits[i])
                {
                    winner = i % 2 == 0 ? Side.Right : Side.Left;
                    break;
                }
            }

            //determine winner by ground
            if (winner == Side.None)
            {
                winner = _match.Ball.Position.x > 0f
                    ? Side.Left : Side.Right;
            }

            _match.HitCounts = new int[6];

            //double mode alpha and side switch
            if (_match.Players.Count == 4)
            {
                if (winner != _match.LastWinner && _match.LastWinner != Side.None)
                {
                    if (winner == Side.Left) _match.LeftSwitched = !_match.LeftSwitched;
                    else _match.RightSwitched = !_match.RightSwitched;

                    // if (winner == Side.Left)
                    // {
                    //     _match.Players[0].Position = _matchData.SpawnPoints[_match.LeftSwitched ? 2 : 0];
                    //     _match.Players[0].LeftLimit = _matchData.LeftLimits[_match.LeftSwitched ? 2 : 0];
                    //     _match.Players[0].RightLimit = _matchData.RightLimits[_match.LeftSwitched ? 2 : 0];
                    //     _match.Players[2].Position = _matchData.SpawnPoints[_match.LeftSwitched ? 0 : 2];
                    //     _match.Players[2].LeftLimit = _matchData.LeftLimits[_match.LeftSwitched ? 0 : 2];
                    //     _match.Players[2].RightLimit = _matchData.RightLimits[_match.LeftSwitched ? 0 : 2];
                    // }
                    // else
                    // {
                    //     _match.Players[1].Position = _matchData.SpawnPoints[_match.RightSwitched ? 3 : 1];
                    //     _match.Players[1].LeftLimit = _matchData.LeftLimits[_match.RightSwitched ? 3 : 1];
                    //     _match.Players[1].RightLimit = _matchData.RightLimits[_match.RightSwitched ? 3 : 1];
                    //     _match.Players[3].Position = _matchData.SpawnPoints[_match.RightSwitched ? 1 : 3];
                    //     _match.Players[3].LeftLimit = _matchData.LeftLimits[_match.RightSwitched ? 1 : 3];
                    //     _match.Players[3].RightLimit = _matchData.RightLimits[_match.RightSwitched ? 1 : 3];
                    // }
                }

                int notGivingPlayerNum = winner == Side.Left ? (_match.LeftSwitched ? 0 : 2) : (_match.RightSwitched ? 1 : 3);
                _match.HitCounts[notGivingPlayerNum] = 1;

                for (int i = 0; i < 4; i++) _match.InvokeAlpha(i, false);

                _match.InvokeAlpha(notGivingPlayerNum, true);
            }

            _match.InvokeScore(winner);

            if (_match.ScoreLeft >= _matchData.WinningScore) _match.InvokeOver(Side.Left);
            else if (_match.ScoreRight >= _matchData.WinningScore) _match.InvokeOver(Side.Right);
            else
            {
                _match.ResetBallTimer?.Start();
                if (_match.MatchData.JumpMode == JumpMode.NoJump) _match.AutoDropTimer?.Start();
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

        public void OnPlayer(Player player)
        {

        }

        public void OnSideChanged(Side newSide)
        {

        }
    }
}

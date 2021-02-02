using System.Linq;
using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game.Entities;
using Blobby.Game.Physics;
using Blobby.Models;
using UnityEngine;
using static Blobby.Game.Entities.Ball;

namespace Blobby.Game.Entities
{
    public class BallReadyState : IBallState
    {
        Ball _ball;
        Match _match;
        MatchData _matchData;

        public BallReadyState(Ball ball, Match match, MatchData matchData) => (_ball, _match, _matchData) = (ball, match, matchData);

        public void EnterState()
        {
            MainThreadManager.Run(() =>
            {
                _ball.BallObj.layer = 6;

                var ballPos = _match.LastWinner == Side.Left ||
                              _match.LastWinner == Side.None
                    ? _matchData.BallSpawnPoints[0]
                    : _matchData.BallSpawnPoints[1];

                _ball.Position = ballPos;
                _ball.BallObj.transform.position = ballPos;
                _ball.Velocity = Vector2.zero;
                _ball.Rotation = 0f;
                _ball.AngularVelocity = 0f;
                _ball.Gravity = 0f;
                _ball.Side = _match.LastWinner;

                _ball.InvokeSideChanged(_ball.Side);
            });
        }

        public void ExitState()
        {

        }

        public void FixedUpdate()
        {
            // var involuntaryHappened = _ball.HandleInvoluntaryCollision(); //nicht im stopped
            //
            // var voluntaryPlayerHappened = false;
            // var voluntaryWallHappened = false;
            // var voluntaryNetHappened = false;
            // if (!involuntaryHappened)
            // {
            //     voluntaryPlayerHappened = _ball.HandleVoluntaryPlayerCollision(); //nicht im stopped
            //     voluntaryWallHappened = _ball.HandleVoluntaryWallCollision();
            //     voluntaryNetHappened = _ball.HandleVoluntaryNetCollision();
            // }
            //
            // var voluntaries = new bool[] {voluntaryPlayerHappened, voluntaryNetHappened, voluntaryWallHappened };
            //
            // if (involuntaryHappened || voluntaries.Any(val => val))
            // {
            //     _ball.SetState(_ball.Running);
            // }

            //_ball.HandleMapCollision();
        }

        public void OnPlayerHit(Player player, Vector2 centroid, Vector2 normal)
        {
            _ball.Position = centroid;
            _ball.Velocity = normal * Ball.BALL_SHOT_VELOCITY;

            _ball.InvokePlayerHit(player);
            _ball.SetState(_ball.Running);
        }

        public void OnWallHit()
        {
            
        }

        public void OnNetHit()
        {
            
        }

        public void OnGroundHit()
        {

        }

        public void OnBombTimerStopped() { }
    }
}
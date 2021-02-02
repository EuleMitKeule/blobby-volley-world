using System.Linq;
using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game.Physics;
using Blobby.Models;
using UnityEngine;
using static Blobby.Game.Entities.Ball;

namespace Blobby.Game.Entities
{
    public class BallRunningTennisState : IBallState
    {
        Ball _ball;
        Match _match;
        MatchData _matchData;

        public BallRunningTennisState(Ball ball, Match match, MatchData matchData) => (_ball, _match, _matchData) = (ball, match, matchData);

        public void EnterState()
        {
            MainThreadManager.Run(() =>
            {
                _ball.BallObj.layer = 6;
                _ball.Gravity = _match.PhysicsSettings.ballGravity;
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
            // if (!involuntaryHappened && voluntaries.All(val => !val))
            // {
            //     _ball.Position += Time.fixedDeltaTime * _ball.Velocity;
            // }

            _ball.HandleMapCollisionTennis();
        }

        public void OnPlayerHit(Player player, Vector2 centroid, Vector2 normal)
        {
            _ball.Position = centroid;
            _ball.Velocity = normal * Ball.BALL_SHOT_VELOCITY;

            _ball.InvokePlayerHit(player);
        }

        public void OnWallHit()
        {
            
        }

        public void OnGroundHit()
        {
            _ball.InvokeGroundHit();
            _ball.SetState(_ball.Stopped);
        }

        public void OnNetHit()
        {
            
        }

        public void OnBombTimerStopped()
        {

        }
    }
}
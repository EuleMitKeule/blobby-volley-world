using System.Linq;
using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game.Physics;
using Blobby.Models;
using UnityEngine;
using static Blobby.Game.Entities.Ball;

namespace Blobby.Game.Entities
{
    public class BallStoppedState : IBallState
    {
        Ball _ball;
        Match _match;
        MatchData _matchData;

        public BallStoppedState(Ball ball, Match match, MatchData matchData) => (_ball, _match, _matchData) = (ball, match, matchData);

        public void EnterState()
        {
            MainThreadManager.Run(() =>
            {
                _ball.BallObj.layer = 7;
            });
        }

        public void ExitState()
        {

        }

        public void FixedUpdate()
        {

            // //var involuntaryHappened = _ball.HandleInvoluntaryCollision(); //nicht im stopped
            //
            // // var voluntaryPlayerHappened = false;
            // var voluntaryWallHappened = false;
            // var voluntaryNetHappened = false;
            // // if (!involuntaryHappened)
            // // {
            //     //voluntaryPlayerHappened = _ball.HandleVoluntaryPlayerCollision(); //nicht im stopped
            //     voluntaryWallHappened = _ball.HandleVoluntaryWallCollision();
            //     voluntaryNetHappened = _ball.HandleVoluntaryNetCollision();
            // // }
            //
            // var voluntaries = new bool[] { voluntaryNetHappened, voluntaryWallHappened };
            //
            // if (voluntaries.All(val => !val))
            // {
            //     _ball.Position += Time.fixedDeltaTime * _ball.Velocity;
            // }

            _ball.HandleMapCollision();
        }

        public void OnPlayerHit(Player player, Vector2 centroid, Vector2 normal) { }

        public void OnWallHit()
        {
            
        }

        public void OnNetHit()
        {
            
        }

        public void OnGroundHit()
        {
            if (Mathf.Abs(_ball.Velocity.y) > _matchData.GroundVelocityTreshold) _ball.InvokeGroundHit();
        }

        public void OnBombTimerStopped()
        {

        }
    }
}
using UnityEngine;
using static Blobby.Game.Entities.BallComponent;

namespace Blobby.Game.Entities.States
{
    public class BallReadyState : IBallState
    {
        BallComponent _ballComponent;
        MatchComponent _matchComponent;

        public BallReadyState(BallComponent ballComponent, MatchComponent matchComponent) => (_ballComponent, _matchComponent) = (ballComponent, matchComponent);

        public void EnterState()
        {
            _ballComponent.gameObject.layer = 6;

            var ballPos = _matchComponent.LastWinner == Side.Left ||
                          _matchComponent.LastWinner == Side.None
                ? _ballComponent.SpawnPoints[0]
                : _ballComponent.SpawnPoints[1];

            _ballComponent.Position = ballPos;
            _ballComponent.transform.position = ballPos;
            _ballComponent.Velocity = Vector2.zero;
            _ballComponent.Rotation = 0f;
            _ballComponent.AngularVelocity = 0f;
            _ballComponent.Gravity = 0f;
            _ballComponent.Side = _matchComponent.LastWinner;
        }

        public void ExitState()
        {

        }

        public void FixedUpdate()
        {
        }

        public void OnPlayerHit(PlayerComponent playerComponent, Vector2 centroid, Vector2 normal)
        {
            var oldBallFrictionVelocity = _ballComponent.Velocity.RotateClockwise(normal).x;
            var newBallFrictionVelocity = (normal * SHOT_VELOCITY).RotateClockwise(normal).x;
            var blobFrictionVelocity = playerComponent.Velocity.RotateClockwise(normal).x;

            _ballComponent.AngularVelocity =
                (newBallFrictionVelocity - oldBallFrictionVelocity + blobFrictionVelocity) * _ballComponent.AngularVelocityMultiplier / _ballComponent.Radius;    
            _ballComponent.Position = centroid;
            _ballComponent.Velocity = normal * SHOT_VELOCITY;

            _ballComponent.InvokePlayerHit(playerComponent);
            _ballComponent.SetState(_ballComponent.Running);
        }

        public void OnCollision(RaycastHit2D result)
        {

        }

        public void OnBombTimerStopped() { }
    }
}
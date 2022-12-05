using Blobby.Game.Physics;
using UnityEngine;
using static Blobby.Game.Entities.BallComponent;

namespace Blobby.Game.Entities.States
{
    public class BallStoppedState : IBallState
    {
        BallComponent _ballComponent;
        MatchComponent _matchComponent;

        public BallStoppedState(BallComponent ballComponent, MatchComponent matchComponent) =>
            (_ballComponent, _matchComponent) = (ballComponent, matchComponent);

        public void EnterState()
        {
            _ballComponent.gameObject.layer = 7;
        }

        public void ExitState()
        {

        }

        public void FixedUpdate()
        {
            _ballComponent.HandleMapCollision();
        }

        public void OnCollision(RaycastHit2D result)
        {
            if (result.collider == PhysicsWorld.GroundCollider)
            {
                if (Mathf.Abs(_ballComponent.Velocity.y) > GROUND_VELOCITY_THRESHOLD) _ballComponent.InvokeGroundHit();
            }
            else if (result.collider == PhysicsWorld.NetEdgeCollider)
            {
                _ballComponent.InvokeNetHit();
            }
            else
            {
                _ballComponent.InvokeWallHit();
            }
        }

        public void OnPlayerHit(PlayerComponent playerComponent, Vector2 centroid, Vector2 normal)
        {
        }

        public void OnBombTimerStopped()
        {

        }
    }
}
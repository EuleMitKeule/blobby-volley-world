using System.Linq;
using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game.Physics;
using Blobby.Models;
using UnityEngine;
using static Blobby.Game.Entities.BallComponent;

namespace Blobby.Game.Entities
{
    public class BallRunningState : IBallState
    {
        BallComponent _ballComponent;

        public BallRunningState(BallComponent ballComponent) => _ballComponent = ballComponent;

        public void EnterState()
        {
            MainThreadManager.Run(() =>
            {
                _ballComponent.gameObject.layer = 6;
                _ballComponent.Gravity = BallComponent.GRAVITY;
            });
        }

        public void ExitState()
        {

        }

        public void FixedUpdate()
        {
            _ballComponent.HandleMapCollision();
        }

        public void OnPlayerHit(PlayerComponent playerComponent, Vector2 centroid, Vector2 normal)
        {
            _ballComponent.Position = centroid;
            _ballComponent.Velocity = normal * SHOT_VELOCITY;

            _ballComponent.InvokePlayerHit(playerComponent);
        }

        public void OnCollision(RaycastHit2D result)
        {
            if (result.collider == PhysicsWorld.GroundCollider)
            {
                _ballComponent.InvokeGroundHit();
                _ballComponent.SetState(_ballComponent.Stopped);
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

        public void OnBombTimerStopped()
        {
            MainThreadManager.Run(() =>
            {
                _ballComponent.SetState(_ballComponent.Stopped);
            });
        }
    }
}
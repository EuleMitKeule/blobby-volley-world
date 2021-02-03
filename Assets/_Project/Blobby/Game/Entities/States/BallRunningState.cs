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
        MatchComponent _matchComponent;

        public BallRunningState(BallComponent ballComponent, MatchComponent matchComponent) => (_ballComponent, _matchComponent) = (ballComponent, matchComponent);

        public void EnterState()
        {
            MainThreadManager.Run(() =>
            {
                _ballComponent.BallObj.layer = 6;
                _ballComponent.Gravity = _matchComponent.PhysicsSettings.ballGravity;
            });
        }

        public void ExitState()
        {

        }

        public void FixedUpdate()
        {
            _ballComponent.HandleMapCollision();
        }

        public void OnPlayerHit(Player player, Vector2 centroid, Vector2 normal)
        {
            _ballComponent.Position = centroid;
            _ballComponent.Velocity = normal * BALL_SHOT_VELOCITY;

            _ballComponent.InvokePlayerHit(player);
        }

        public void OnWallHit()
        {
            
        }
        public void OnNetHit()
        {
            
        }

        public void OnGroundHit()
        {
            _ballComponent.InvokeGroundHit();
            _ballComponent.SetState(_ballComponent.Stopped);
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
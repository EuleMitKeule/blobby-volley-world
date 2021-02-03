using System.Linq;
using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game.Physics;
using Blobby.Models;
using UnityEngine;
using static Blobby.Game.Entities.BallComponent;

namespace Blobby.Game.Entities
{
    public class BallStoppedState : IBallState
    {
        BallComponent _ballComponent;
        MatchComponent _matchComponent;

        public BallStoppedState(BallComponent ballComponent, MatchComponent matchComponent) =>
            (_ballComponent, _matchComponent) = (ballComponent, matchComponent);

        public void EnterState()
        {
            MainThreadManager.Run(() =>
            {
                _ballComponent.BallObj.layer = 7;
            });
        }

        public void ExitState()
        {

        }

        public void FixedUpdate()
        {
            _ballComponent.HandleMapCollision();
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
            if (Mathf.Abs(_ballComponent.Velocity.y) > GROUND_VELOCITY_THRESHOLD) _ballComponent.InvokeGroundHit();
        }

        public void OnBombTimerStopped()
        {

        }
    }
}
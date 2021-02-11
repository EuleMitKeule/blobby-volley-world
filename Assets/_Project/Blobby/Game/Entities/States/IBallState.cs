using Blobby.Game.Physics;
using UnityEngine;
using static Blobby.Game.Entities.BallComponent;

namespace Blobby.Game.Entities
{
    public interface IBallState
    {
        void EnterState();

        void ExitState();

        void FixedUpdate();

        void OnCollision(RaycastHit2D result);

        void OnPlayerHit(PlayerComponent playerComponent, Vector2 centroid, Vector2 normal);

        void OnBombTimerStopped();
    }
}

using UnityEngine;

namespace Blobby.Game.Entities.States
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

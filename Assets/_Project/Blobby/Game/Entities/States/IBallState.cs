using Blobby.Game.Physics;
using UnityEngine;
using static Blobby.Game.Entities.Ball;

namespace Blobby.Game.Entities
{
    public interface IBallState
    {
        void FixedUpdate();

        void EnterState();

        void ExitState();

        void OnPlayerHit(Player player, Vector2 centroid, Vector2 normal);

        void OnWallHit();

        void OnNetHit();

        void OnGroundHit();

        void OnBombTimerStopped();
    }
}

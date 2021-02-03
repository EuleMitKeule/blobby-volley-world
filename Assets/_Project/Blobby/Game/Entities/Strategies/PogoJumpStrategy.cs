using UnityEngine;

namespace Blobby.Game.Entities
{
    public class PogoJumpStrategy : IJumpStrategy
    {
        Player _player;
        MatchComponent _matchComponent;

        public PogoJumpStrategy(Player player, MatchComponent matchComponent)
        {
            _player = player;
            _matchComponent = matchComponent;
        }

        public void OnJumpDown()
        {
            _player.KeyPressed[0] = true;
        }

        public void OnJumpUp()
        {

        }

        public void OnJump()
        {
            _player.IsGrounded = false;
            _player.Velocity = new Vector2(_player.Velocity.x, _matchComponent.PhysicsSettings.playerJumpVelocity);
        }

        public void OnJumpHold()
        {
            _player.Velocity += Vector2.up * Time.fixedDeltaTime * _matchComponent.PhysicsSettings.playerJumpDrift;
        }
    }
}
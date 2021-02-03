using UnityEngine;

namespace Blobby.Game.Entities
{
    public class StandardJumpStrategy : IJumpStrategy
    {
        Player _player;
        MatchComponent _matchComponent;

        public StandardJumpStrategy(Player player, MatchComponent matchComponent)
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
            _player.KeyPressed[0] = false;
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
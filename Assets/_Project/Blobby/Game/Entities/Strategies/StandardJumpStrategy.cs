using Blobby.Game.Entities.Strategies;
using UnityEngine;

namespace Blobby.Game.Entities
{
    public class StandardJumpStrategy : IJumpStrategy
    {
        PlayerComponent _playerComponent;

        public StandardJumpStrategy(PlayerComponent playerComponent)
        {
            _playerComponent = playerComponent;
        }

        public void OnJumpDown()
        {
            _playerComponent.KeyPressed[0] = true;
        }

        public void OnJumpUp()
        {
            _playerComponent.KeyPressed[0] = false;
        }

        public void OnJump()
        {
            _playerComponent.IsGrounded = false;
            _playerComponent.Velocity = new Vector2(_playerComponent.Velocity.x, PlayerComponent.JUMP_VELOCITY);
        }

        public void OnJumpHold()
        {
            _playerComponent.Velocity += Vector2.up * Time.fixedDeltaTime * PlayerComponent.JUMP_DRIFT;
        }
    }
}
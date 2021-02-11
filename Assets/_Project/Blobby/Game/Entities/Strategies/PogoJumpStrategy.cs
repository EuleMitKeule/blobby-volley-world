using Blobby.Game.Entities.Strategies;
using UnityEngine;

namespace Blobby.Game.Entities
{
    public class PogoJumpStrategy : IJumpStrategy
    {
        PlayerComponent PlayerComponent { get; set; }

        public PogoJumpStrategy(PlayerComponent playerComponent) => PlayerComponent = playerComponent;

        public void OnJumpDown()
        {
            PlayerComponent.KeyPressed[0] = true;
        }

        public void OnJumpUp()
        {

        }

        public void OnJump()
        {
            PlayerComponent.IsGrounded = false;
            PlayerComponent.Velocity = new Vector2(PlayerComponent.Velocity.x, PlayerComponent.JUMP_VELOCITY);
        }

        public void OnJumpHold()
        {
            PlayerComponent.Velocity += Vector2.up * Time.fixedDeltaTime * PlayerComponent.JUMP_DRIFT;
        }
    }
}
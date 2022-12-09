using BlobbyVolleyWorld.Entities.Input;
using BlobbyVolleyWorld.Game;
using UnityEngine;

namespace BlobbyVolleyWorld.Entities.Physics.Jumping
{
    public class SpringJumpStrategy : IJumpStrategy
    {
        InputComponent InputComponent { get; }
        PlayerMovementComponent PlayerMovementComponent { get; }
        GameComponent GameComponent { get; }

        public SpringJumpStrategy(
            PlayerMovementComponent playerMovementComponent,
            InputComponent inputComponent)
        {
            GameComponent = Object.FindObjectOfType<GameComponent>();
            PlayerMovementComponent = playerMovementComponent;
            InputComponent = inputComponent;
        }

        public void OnJumpStart() { }
        public void OnJumpHold() { }
        
        public void OnJumpOver()
        {
            PlayerMovementComponent.IsGrounded = false;
            PlayerMovementComponent.Velocity = 
                new Vector2(
                    PlayerMovementComponent.Velocity.x, 
                    InputComponent.JumpCharge * GameComponent.PhysicsAsset.JumpSpeed);
        }
    }
}
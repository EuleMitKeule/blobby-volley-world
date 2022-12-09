using BlobbyVolleyWorld.Entities.Input;
using BlobbyVolleyWorld.Game;
using UnityEngine;

namespace BlobbyVolleyWorld.Entities.Physics.Jumping
{
    public class StandardJumpStrategy : IJumpStrategy
    {
        PlayerMovementComponent PlayerMovementComponent { get; }
        GameComponent GameComponent { get; }
        
        public StandardJumpStrategy(
            PlayerMovementComponent playerMovementComponent)
        {
            GameComponent = Object.FindObjectOfType<GameComponent>();
            PlayerMovementComponent = playerMovementComponent;
        }
        
        public void OnJumpStart()
        {
            PlayerMovementComponent.IsGrounded = false;
            PlayerMovementComponent.Velocity = 
                new Vector2(
                    PlayerMovementComponent.Velocity.x, 
                    GameComponent.PhysicsAsset.JumpSpeed);
        }

        public void OnJumpHold()
        {
            PlayerMovementComponent.Velocity += 
                Vector2.up * (Time.fixedDeltaTime * GameComponent.PhysicsAsset.JumpDrift);
        }

        public void OnJumpOver() { }
    }
}
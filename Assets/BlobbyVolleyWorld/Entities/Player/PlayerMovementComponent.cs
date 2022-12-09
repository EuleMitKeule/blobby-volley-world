using BlobbyVolleyWorld.Entities.Input;
using BlobbyVolleyWorld.Entities.Physics.Jumping;
using BlobbyVolleyWorld.Game;
using BlobbyVolleyWorld.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace BlobbyVolleyWorld.Entities.Physics
{
    public class PlayerMovementComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [ReadOnly]
        public Vector2 Velocity { get; set; }

        [ShowInInspector]
        [ReadOnly]
        public bool IsGrounded { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public bool IsJumping { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        IJumpStrategy JumpStrategy { get; set; }
        
        GameComponent GameComponent { get; set; }
        InputComponent InputComponent { get; set; }
        PlayerComponent PlayerComponent { get; set; }
        PlayerGeometryComponent PlayerGeometryComponent { get; set; }
        MapGeometryComponent MapGeometryComponent { get; set; }
        
        void Awake()
        {
            GameComponent = FindObjectOfType<GameComponent>();
            MapGeometryComponent = FindObjectOfType<MapGeometryComponent>();
            InputComponent = GetComponent<InputComponent>();
            PlayerComponent = GetComponent<PlayerComponent>();
            PlayerGeometryComponent = GetComponent<PlayerGeometryComponent>();
            
            PlayerComponent.FieldPositionChanged += OnFieldPositionChanged;
            
            JumpStrategy = JumpStrategyFactory.Create(
                this,
                InputComponent,
                GameComponent.MatchSettings.JumpMode);
        }

        void OnFieldPositionChanged(object sender, FieldPosition fieldPosition)
        {
            transform.position = GameComponent.PhysicsAsset.PlayerSpawnPositions[fieldPosition];
        }

        void FixedUpdate()
        {
            var newPosition = (Vector2)transform.position;
            
            var newVelocityX = InputComponent.HorizontalInput * GameComponent.PhysicsAsset.PlayerSpeed;
            var newVelocityY = Velocity.y - GameComponent.PhysicsAsset.Gravity * Time.fixedDeltaTime;

            Velocity = new Vector2(newVelocityX, newVelocityY);

            if (InputComponent.IsUpPressed)
            {
                if (IsGrounded)
                {
                    IsJumping = true;
                    JumpStrategy.OnJumpStart();
                }
                JumpStrategy.OnJumpHold();
            }
            
            if (IsJumping && !InputComponent.IsUpPressed)
            {
                IsJumping = false;
                JumpStrategy.OnJumpOver();
            }

            newPosition += Velocity * Time.fixedDeltaTime;

            IsGrounded = newPosition.y <= PlayerGeometryComponent.GroundLimit;
            
            var clampedVelocity = new Vector2(
                Velocity.x,
                IsGrounded ? 0f : Velocity.y);

            var clampedPosition = new Vector2(
                Mathf.Clamp(newPosition.x, PlayerGeometryComponent.LeftLimit, PlayerGeometryComponent.RightLimit),
                Mathf.Max(newPosition.y, PlayerGeometryComponent.GroundLimit));
            
            Velocity = clampedVelocity;
            newPosition = clampedPosition;
            
            // if (MatchComponent.BallComponent)
            // {
            //     var ballCollisionResult = BallCollision;
            //
            //     if (ballCollisionResult.HasValue)
            //     {
            //         var largeRadius = ballCollisionResult.Value.Radius + MatchComponent.BallComponent.Radius;
            //         var ballCentroid = Position + ballCollisionResult.Value.Offset -
            //                            ballCollisionResult.Value.Result.normal * largeRadius;
            //         var ballNormal = -ballCollisionResult.Value.Result.normal;
            //         MatchComponent.BallComponent.ResolvePlayerCollision(this, ballCentroid, ballNormal);
            //     }
            // }

            transform.position = newPosition;
        }
    }
}
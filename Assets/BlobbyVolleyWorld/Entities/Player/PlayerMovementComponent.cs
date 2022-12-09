using System;
using BlobbyVolleyWorld.Entities.Ball;
using BlobbyVolleyWorld.Entities.Input;
using BlobbyVolleyWorld.Entities.Physics.Jumping;
using BlobbyVolleyWorld.Entities.Player;
using BlobbyVolleyWorld.Game;
using BlobbyVolleyWorld.Maps;
using BlobbyVolleyWorld.Match;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace BlobbyVolleyWorld.Entities.Physics
{
    public class PlayerMovementComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [Required]
        LayerMask BallLayerMask { get; set; }
        
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
        SideComponent SideComponent { get; set; }
        PlayerGeometryComponent PlayerGeometryComponent { get; set; }
        MapGeometryComponent MapGeometryComponent { get; set; }
        BallMovementComponent BallMovementComponent { get; set; }
        BallGeometryComponent BallGeometryComponent { get; set; }
        
        void Awake()
        {
            GameComponent = FindObjectOfType<GameComponent>();
            MapGeometryComponent = FindObjectOfType<MapGeometryComponent>();
            InputComponent = GetComponent<InputComponent>();
            PlayerComponent = GetComponent<PlayerComponent>();
            SideComponent = GetComponent<SideComponent>();
            PlayerGeometryComponent = GetComponent<PlayerGeometryComponent>();
            
            PlayerComponent.FieldPositionChanged += OnFieldPositionChanged;
            
            JumpStrategy = JumpStrategyFactory.Create(
                this,
                InputComponent,
                GameComponent.MatchSettings.JumpMode);
        }

        void Start()
        {
            BallMovementComponent = FindObjectOfType<BallMovementComponent>();
            BallGeometryComponent = FindObjectOfType<BallGeometryComponent>();
        }

        void OnFieldPositionChanged(object sender, FieldPosition fieldPosition)
        {
            SideComponent.SetSide(fieldPosition.ToSide());
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

            var (result, colliderType) = HandleBallCollision(newPosition);

            if (result.HasValue && colliderType.HasValue)
            {
                var radius = colliderType switch
                {
                    PlayerColliderType.Upper => PlayerGeometryComponent.UpperRadius,
                    PlayerColliderType.Lower => PlayerGeometryComponent.LowerRadius,
                    _ => throw new ArgumentOutOfRangeException()
                };

                var offset = colliderType switch
                {
                    PlayerColliderType.Upper => PlayerGeometryComponent.UpperCenterOffset,
                    PlayerColliderType.Lower => PlayerGeometryComponent.LowerCenterOffset,
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                var largeRadius = radius + BallGeometryComponent.Radius;
                var ballNormal = -result.Value.normal;
                var ballCentroid = newPosition + offset + ballNormal * largeRadius;
                BallMovementComponent.ResolvePlayerCollision(this, PlayerComponent, ballNormal, ballCentroid);   
            }

            transform.position = newPosition;
        }

        (RaycastHit2D?, PlayerColliderType?) HandleBallCollision(Vector2 newPosition)
        {
            var upperCenterPosition = newPosition + PlayerGeometryComponent.UpperCenterOffset;
            var lowerCenterPosition = newPosition + PlayerGeometryComponent.LowerCenterOffset;
            
            var upperResult = Physics2D.CircleCast(
                upperCenterPosition, 
                PlayerGeometryComponent.UpperRadius, 
                BallMovementComponent.Velocity,
                BallMovementComponent.Velocity.magnitude, 
                BallLayerMask);
            
            var lowerResult = Physics2D.CircleCast(
                lowerCenterPosition, 
                PlayerGeometryComponent.LowerRadius, 
                BallMovementComponent.Velocity,
                BallMovementComponent.Velocity.magnitude, 
                BallLayerMask);

            if (!lowerResult.collider && !upperResult.collider) return (null, null);

            var upperDistance = (newPosition - upperResult.point).magnitude;
            var lowerDistance = (newPosition - lowerResult.point).magnitude;

            if (!upperResult.collider) return (lowerResult, PlayerColliderType.Lower);
            if (!lowerResult.collider) return (upperResult, PlayerColliderType.Upper);
            
            return upperDistance < lowerDistance
                ? (upperResult, PlayerColliderType.Upper)
                : (lowerResult, PlayerColliderType.Lower);
        }
    }
}
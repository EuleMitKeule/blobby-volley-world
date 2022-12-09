using BlobbyVolleyWorld.Entities.Ball;
using BlobbyVolleyWorld.Entities.Physics;
using BlobbyVolleyWorld.Extensions;
using BlobbyVolleyWorld.Game;
using BlobbyVolleyWorld.Maps;
using BlobbyVolleyWorld.Match;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace BlobbyVolleyWorld.Entities
{
    public class BallMovementComponent : StateContextComponent<BallStateComponent>
    {
        [OdinSerialize]
        [Required]
        LayerMask MapLayerMask { get; set; }
        
        [OdinSerialize]
        [Required]
        LayerMask PlayerLayerMask { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public Vector2 Velocity { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        float AngularVelocity { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        Vector2? CollisionVelocity { get; set; }
        
        Transform Transform { get; set; }
        BallGeometryComponent BallGeometryComponent { get; set; }
        GameComponent GameComponent { get; set; }
        MapGeometryComponent MapGeometryComponent { get; set; }
        SideComponent SideComponent { get; set; }
        MatchComponent MatchComponent { get; set; }
        
        protected override void Awake()
        {
            base.Awake();
            
            GameComponent = FindObjectOfType<GameComponent>();
            MapGeometryComponent = FindObjectOfType<MapGeometryComponent>();
            Transform = GetComponent<Transform>();
            BallGeometryComponent = GetComponent<BallGeometryComponent>();
            SideComponent = GetComponent<SideComponent>();
            MatchComponent = FindObjectOfType<MatchComponent>();

            MatchComponent.GivingSideChanged += OnGivingSideChanged;
        }

        void FixedUpdate()
        {
            var position = Transform.position;
            var oldPosition = (Vector2)position;
            var newPosition = (Vector2)position;
            var newRotation = Transform.eulerAngles.z;

            if (StateComponent.IsGravityEnabled)
            {
                Velocity -= Vector2.up * (Time.fixedDeltaTime * GameComponent.PhysicsAsset.BallGravity);
            }
            
            Velocity = Vector2.ClampMagnitude(Velocity, GameComponent.PhysicsAsset.BallMaxSpeed);
            
            newPosition += Velocity * Time.fixedDeltaTime;
            newRotation += AngularVelocity * Time.fixedDeltaTime;
            
            HandleCollision(ref newPosition, oldPosition);

            Transform.position = newPosition;
            Transform.eulerAngles = new Vector3(0, 0, newRotation);
        }

        public void ResolvePlayerCollision(PlayerMovementComponent playerMovementComponent, PlayerComponent playerComponent, Vector2 normal, Vector2 centroid)
        {
            if (!StateComponent.IsPlayerCollisionEnabled) return;
            
            var oldBallFrictionVelocity = Velocity.RotateClockwise(normal).x;
            var newBallFrictionVelocity = (normal * GameComponent.PhysicsAsset.BallSpeed).RotateClockwise(normal).x;
            var blobFrictionVelocity = playerMovementComponent.Velocity.RotateClockwise(normal).x;

            AngularVelocity = 
                (newBallFrictionVelocity - oldBallFrictionVelocity + blobFrictionVelocity) * 
                GameComponent.PhysicsAsset.BallAngularVelocityFactor / BallGeometryComponent.Radius;
            
            transform.position = centroid;
            Velocity = normal * GameComponent.PhysicsAsset.BallSpeed;
            
            StateComponent.OnPlayerCollision();
        }

        void HandleCollision(ref Vector2 newPosition, Vector2 oldPosition)
        {
            if (StateComponent.IsMapCollisionEnabled)
            {
                HandleMapCollision(newPosition, oldPosition);
            }
            
            if (StateComponent.IsPlayerCollisionEnabled)
            {
                HandlePlayerCollision(oldPosition);
            }

            if (!CollisionVelocity.HasValue) return;
            
            Velocity = CollisionVelocity.Value;

            CollisionVelocity = null;
        }

        void HandleMapCollision(Vector2 newPosition, Vector2 oldPosition)
        {
            var speed = Velocity.magnitude;
            var traveledDistance = speed * Time.fixedDeltaTime;
            
            var result = Physics2D.CircleCast(
                oldPosition, 
                BallGeometryComponent.Radius, 
                Velocity, 
                traveledDistance, 
                MapLayerMask);
            
            if (!result.collider) return;

            if (result.collider == MapGeometryComponent.GroundCollider)
            {
                HandleGroundCollision();
                return;
            }

            if (result.collider == MapGeometryComponent.LeftWallCollider ||
                result.collider == MapGeometryComponent.RightWallCollider ||
                result.collider == MapGeometryComponent.LeftNetCollider ||
                result.collider == MapGeometryComponent.RightNetCollider)
            {
                HandleWallCollision(newPosition);
                return;
            }

            if (result.collider == MapGeometryComponent.NetEdgeCollider)
            {
                HandleNetEdgeCollision(result);
            }
        }

        void HandleGroundCollision()
        {
            CollisionVelocity = new Vector2(Velocity.x, Mathf.Abs(Velocity.y)) * GameComponent.PhysicsAsset.BallGroundBounceFactor;
            AngularVelocity *= GameComponent.PhysicsAsset.BallGroundAngularVelocityFactor;
        }

        void HandleWallCollision(Vector2 newPosition)
        {
            var positionSign = Mathf.Sign(newPosition.x);
            var quarterSide = 
                newPosition.x < positionSign * MapGeometryComponent.FieldWidth / 4f ? 
                    Side.Left : Side.Right;
            
            CollisionVelocity = new Vector2(
                Mathf.Abs(Velocity.x) * -quarterSide.ToSign(),
                Velocity.y);
        }

        void HandleNetEdgeCollision(RaycastHit2D result)
        {
            CollisionVelocity = Velocity - 2f * Vector2.Dot(Velocity, -result.normal) * -result.normal;
        }

        void HandlePlayerCollision(Vector2 oldPosition)
        {
            CollisionVelocity ??= Velocity;
            var speed = CollisionVelocity.Value.magnitude;
            var traveledDistance = speed * Time.fixedDeltaTime;
            
            var result = Physics2D.CircleCast(
                oldPosition, 
                BallGeometryComponent.Radius, 
                CollisionVelocity.Value, 
                traveledDistance, 
                PlayerLayerMask);
            
            if (!result.collider) return;

            var playerCollisionVelocity = result.normal * GameComponent.PhysicsAsset.BallSpeed;
            
            CollisionVelocity = (CollisionVelocity.HasValue ? 
                (CollisionVelocity.Value + playerCollisionVelocity).normalized * GameComponent.PhysicsAsset.BallSpeed :
                playerCollisionVelocity);
            
            StateComponent.OnPlayerCollision();
        }

        void OnGivingSideChanged(object sender, Side side)
        {
            transform.position = GameComponent.PhysicsAsset.BallSpawnPositions[side];
        }
        
        // public Vector2 HandleMapCollision(Vector2 position)
        // {
        //     var mapResult = Physics2D.CircleCast(
        //         position, 
        //         BallGeometryComponent.Radius, 
        //         Velocity, 
        //         0f, 
        //         MapLayerMask);
        //     
        //     if (!mapResult.collider) return position;
        //
        //     StateComponent.OnMapCollision(mapResult);
        //
        //     var normal = CalculateMapCollisionNormal(mapResult, position);
        //     var speed = Velocity.magnitude;
        //
        //     var playerResult = Physics2D.CircleCast(
        //         position, 
        //         BallGeometryComponent.Radius, 
        //         Velocity, 
        //         0f, 
        //         PlayerLayerMask);
        //     
        //     if (!HasState<BallStoppedStateComponent>() && playerResult.collider)
        //     {
        //         normal = (normal * speed + playerResult.normal).normalized;
        //         speed = GameComponent.PhysicsAsset.BallSpeed;
        //     }
        //
        //     Velocity = normal * speed;
        //
        //     return mapResult.centroid + normal * (speed * 0.02f);
        // }

        // Vector2 CalculateMapCollisionNormal(RaycastHit2D result, Vector2 position)
        // {
        //     if (!result.collider) return Vector2.zero;
        //
        //     if (result.collider == MapGeometryComponent.GroundCollider)
        //     {
        //         AngularVelocity *= 0.5f;
        //         
        //         return HasState<BallRunningTennisStateComponent>() ? 
        //             new Vector2(Velocity.x * 0.5f, Mathf.Abs(Velocity.y * 0.5f)).normalized * 0.5f : 
        //             new Vector2(Velocity.x, Mathf.Abs(Velocity.y * 0.95f)).normalized * 0.95f;
        //     }
        //
        //     if (result.collider == MapGeometryComponent.NetEdgeCollider)
        //     {
        //         return (Velocity - 2 * Vector2.Dot(Velocity, -result.normal) * -result.normal).normalized;
        //     }
        //
        //     var quarterSide = 
        //         position.x < SideComponent.Side.ToSign() * MapGeometryComponent.FieldWidth / 4f ? 
        //         Side.Left : Side.Right;
        //     return new Vector2(Mathf.Abs(Velocity.x) * -quarterSide.ToSign(), Velocity.y).normalized;
        // }
    }
}
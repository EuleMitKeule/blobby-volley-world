using Blobby.Models;
using System;
using Blobby.Game.Entities.Strategies;
using Blobby.Game.Physics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Blobby.Game.Entities
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerGraphicsComponent))]
    public class PlayerComponent : MonoBehaviour, IPlayerGraphicsProvider
    {
        GameObject MatchObject => transform.parent.gameObject;
        protected MatchComponent MatchComponent { get; private set; }
        BallComponent BallComponent => MatchComponent.BallComponent;

        PlayerData _playerData;
        public PlayerData PlayerData
        {
            get => _playerData;
            set
            {
                _playerData = value;
                PlayerDataChanged?.Invoke(value);
            }
        }
        public event Action<PlayerData> PlayerDataChanged;

        #region Physics Member

        public Vector2 Position { get; private set; }
        public Vector2 Velocity { get; set; }
        Vector2 TransformPosition => transform.position;

        #region Constants

        public const int LAYER = 8;
        const float GRAVITY = 406f;
        const float RUN_VELOCITY = 16.9f;
        public const float JUMP_VELOCITY = 62f;
        public const float JUMP_DRIFT = 216f;
        public const float MIN_CHARGE = 0.85f;
        public const float MAX_CHARGE = 1.85f;
        public const float CHARGE_INCREASE = 0.14f;

        #endregion

        #region Collider

        CircleCollider2D[] Colliders => GetComponents<CircleCollider2D>();
        protected CircleCollider2D UpperCollider { get; set; }
        protected CircleCollider2D LowerCollider { get; set; }
        protected EdgeCollider2D GroundCollider { get; set; }
        public float UpperRadius => UpperCollider.radius;
        public float LowerRadius => LowerCollider.radius;
        Vector2 UpperOffset => UpperCollider.offset;
        Vector2 LowerOffset => LowerCollider.offset;
        public Vector2 UpperCenter => TransformPosition + UpperOffset;
        public Vector2 LowerCenter => TransformPosition + LowerOffset;
        Vector2 Top => Position + UpperOffset + Vector2.up * UpperRadius;
        float TopOffset => UpperOffset.y + UpperRadius;
        Vector2 Bottom => Position + LowerOffset + Vector2.down * LowerRadius;
        public float BottomOffset => GroundCollider.offset.y;

        #endregion

        #region Map

        Vector2 MapCenter => Vector2.zero;
        protected Vector2 SpawnPoint => MatchComponent.SpawnPoints[BlobNum];
        float LeftLimit => MatchComponent.LeftLimits[BlobNum];
        float RightLimit => MatchComponent.RightLimits[BlobNum];
        bool IsOnLeftSide => Position.IsLeftOf(MapCenter);

        #endregion

        #region Movement

        Vector2 DeltaGravity => Vector2.up * GRAVITY * Time.fixedDeltaTime;
        Vector2 DeltaVelocity => Velocity * Time.fixedDeltaTime;
        Vector2 MoveVelocity => new Vector2(MoveSign * RUN_VELOCITY, Velocity.y);
        Vector2 ClampedPosition => new Vector2
        (
            CanCollideWithNet ? ClampedNetPosition.x : Mathf.Clamp(Position.x, LeftLimit, RightLimit),
            ClampedNetPosition.y
        );
        Vector2 ClampedNetPosition => new Vector2
        (
            Mathf.Clamp(Position.x,
                IsOnLeftSide ? LeftLimit : PhysicsWorld.RightNetCollider.offset.x + LowerRadius,
                IsOnLeftSide ? PhysicsWorld.LeftNetCollider.offset.x - LowerRadius : RightLimit),
            Mathf.Clamp(Position.y, PhysicsWorld.Ground - BottomOffset, Position.y)
        );
        Vector2 ClampedVelocity => new Vector2
        (
            Velocity.x,
            IsCollidingBelow ? 0f : Velocity.y
        );

        #endregion

        #region Collision

        Vector2 BallDeltaDirection => BallComponent.Velocity.DirectionTo(Velocity);
        float BallDeltaDistance => BallComponent.Velocity.To(Velocity).magnitude * Time.fixedDeltaTime;
        bool IsCollidingBelow => Bottom.y <= PhysicsWorld.Ground;
        bool IsCollidingLeft => Position.x < LeftLimit;
        bool IsCollidingRight => Position.x > RightLimit;
        bool IsBelowNetEdge => Bottom.IsBelow(PhysicsWorld.NetEdgeTop);
        bool CanCollideWithNet => MatchComponent.IsJumpOverNet && IsBelowNetEdge;

        #region Ball Collision

        int BallLayerMask => 1 << BallComponent.LAYER;
        struct BallCollisionResult
        {
            public RaycastHit2D Result;
            public Vector2 Offset;
            public float Radius;

            public BallCollisionResult(RaycastHit2D result, Vector2 offset, float radius) =>
                (Result, Offset, Radius) = (result, offset, radius);
        }
        RaycastHit2D UpperBallCollision => Physics2D.CircleCast(UpperCenter, UpperRadius, BallDeltaDirection,
            BallDeltaDistance, BallLayerMask);
        RaycastHit2D LowerBallCollision => Physics2D.CircleCast(LowerCenter, LowerRadius, BallDeltaDirection,
            BallDeltaDistance, BallLayerMask);

        BallCollisionResult? BallCollision
        {
            get
            {
                var upperResult = UpperBallCollision;
                var lowerResult = LowerBallCollision;

                if (!lowerResult.collider && !upperResult.collider) return null;

                var upperDistance = TransformPosition.To(upperResult.point).magnitude;
                var lowerDistance = TransformPosition.To(lowerResult.point).magnitude;

                var result = new RaycastHit2D();
                var offset = Vector2.zero;
                var radius = 0f;

                if (lowerResult.collider)
                {
                    result = lowerResult;
                    offset = LowerOffset;
                    radius = LowerRadius;
                }

                if (upperResult.collider && upperDistance < lowerDistance)
                {
                    result = upperResult;
                    offset = UpperOffset;
                    radius = UpperRadius;
                }

                return new BallCollisionResult(result, offset, radius);
            }
        }

        #endregion

        #endregion

        #endregion

        #region Input Member

        public bool[] KeyPressed { get; } = new bool[3];
        public bool IsGrounded { get; set; }
        public bool IsRunning => KeyPressed[1] || KeyPressed[2];
        Side MoveSide => KeyPressed[1] switch
        {
            true when !KeyPressed[2] => Side.Left,
            false when KeyPressed[2] => Side.Right,
            _ => Side.None
        };
        int MoveSign => MoveSide.GetSign();
        bool IsJumping => KeyPressed[0];
        IJumpStrategy JumpStrategy { get; set; }

        #endregion

        #region Logic Member

        public Side OwnSide => DefaultBlobNum % 2 == 0 ? Side.Left : Side.Right;
        public Side EnemySide => OwnSide.Other();
        bool IsOnLeftTeam => PlayerData.Side == Side.Left;
        public bool IsSwitched => IsOnLeftTeam ? MatchComponent.LeftSwitched : MatchComponent.RightSwitched;
        public int BlobNum => IsSwitched ? TeamBlobNum : DefaultBlobNum;
        public int DefaultBlobNum => PlayerData.PlayerNum;
        public int TeamBlobNum => (PlayerData.PlayerNum + 2) % 4;
        public virtual bool IsInvisible => false;

        public event Action<int, bool> AlphaChanged;

        #endregion

        protected virtual void Awake()
        {
            MatchComponent = MatchObject.GetComponent<MatchComponent>();

            UpperCollider = Colliders[0];
            LowerCollider = Colliders[1];
            GroundCollider = GetComponent<EdgeCollider2D>();

            JumpStrategy = JumpStrategyFactory.Create(this, MatchComponent.JumpMode);

            SubscribeEventHandler();
        }

        protected virtual void FixedUpdate()
        {
            Velocity -= DeltaGravity;
            Velocity = MoveVelocity;

            if (IsJumping)
            {
                if (IsGrounded) JumpStrategy.OnJump();
                JumpStrategy.OnJumpHold();
            }

            Position += DeltaVelocity;

            IsGrounded = IsCollidingBelow;
            Velocity = ClampedVelocity;
            Position = ClampedPosition;

            if (MatchComponent.BallComponent)
            {
                var ballCollisionResult = BallCollision;

                if (ballCollisionResult.HasValue)
                {
                    var largeRadius = ballCollisionResult.Value.Radius + MatchComponent.BallComponent.Radius;
                    var ballCentroid = Position + ballCollisionResult.Value.Offset -
                                     ballCollisionResult.Value.Result.normal * largeRadius;
                    var ballNormal = -ballCollisionResult.Value.Result.normal;
                    MatchComponent.BallComponent.ResolvePlayerCollision(this, ballCentroid, ballNormal);
                }
            }

            transform.position = Position;
        }

        void OnPlayerDataChanged(PlayerData playerData)
        {
            Position = SpawnPoint;
        }

        protected virtual void OnDestroy()
        {
            MatchComponent.Ready -= OnReady;
            MatchComponent.Stop -= OnStop;
            MatchComponent.Over -= OnOver;
            MatchComponent.Alpha -= OnAlpha;
        }

        #region Match Methods

        protected virtual void OnReady(Side side)
        {
        }

        protected virtual void OnAlpha(int playerNum, bool isTransparent) =>
            AlphaChanged?.Invoke(playerNum, isTransparent);

        protected virtual void OnStop()
        {
            if (MatchComponent.IsPogo) KeyPressed[0] = false;

            if (!MatchComponent.IsJumpOverNet) return;

            Position = SpawnPoint;
        }

        protected virtual void OnOver(Side side, int scoreLeft, int scoreRight, int time) { }

        #endregion

        #region Input Methods

        protected void OnJumpDown() => JumpStrategy.OnJumpDown();
        protected void OnJumpUp() => JumpStrategy.OnJumpUp();
        protected void OnLeftDown() => KeyPressed[1] = true;
        protected void OnLeftUp() => KeyPressed[1] = false;
        protected void OnRightDown() => KeyPressed[2] = true;
        protected void OnRightUp() => KeyPressed[2] = false;

        #endregion

        void SubscribeEventHandler()
        {
            PlayerDataChanged += OnPlayerDataChanged;
            
            MatchComponent.Ready += OnReady;
            MatchComponent.Stop += OnStop;
            MatchComponent.Over += OnOver;
            MatchComponent.Alpha += OnAlpha;
        }
    }
}
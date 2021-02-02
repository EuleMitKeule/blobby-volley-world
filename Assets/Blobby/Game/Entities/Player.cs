using Blobby.Models;
using System;
using Blobby.Game.Physics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Blobby.Game.Entities
{
    public class Player
    {
        public GameObject PlayerObj { get; protected set; }
        public PlayerData PlayerData { get; }
        protected PlayerGraphics PlayerGraphics { get; set; }
        protected Match Match { get; }
        Ball Ball => Match.Ball;

        #region Physics Member

        public Vector2 Position { get; private set; }
        public Vector2 Velocity { get; set; }
        Vector2 TransformPosition => PlayerObj.transform.position;

        #region Constants

        public const int LAYER = 8;
        const float GRAVITY = 406f;
        const float RUN_VELOCITY = 16.9f;

        #endregion

        #region Collider

        CircleCollider2D[] Colliders => PlayerObj.GetComponents<CircleCollider2D>();
        protected CircleCollider2D UpperCollider { get; set; }
        protected CircleCollider2D LowerCollider { get; set; }
        float UpperRadius => UpperCollider.radius;
        float LowerRadius => LowerCollider.radius;
        Vector2 UpperOffset => UpperCollider.offset;
        Vector2 LowerOffset => LowerCollider.offset;
        Vector2 UpperCenter => TransformPosition + UpperOffset;
        Vector2 LowerCenter => TransformPosition + LowerOffset;
        Vector2 Top => Position + UpperOffset + Vector2.up * UpperRadius;
        float TopOffset => UpperOffset.y + UpperRadius;
        Vector2 Bottom => Position + LowerOffset + Vector2.down * LowerRadius;
        public float BottomOffset => LowerOffset.y - LowerRadius;

        #endregion

        #region Map

        Vector2 MapCenter => Vector2.zero;
        protected Vector2 SpawnPoint => Match.SpawnPoints[BlobNum];
        float LeftLimit => Match.LeftLimits[BlobNum];
        float RightLimit => Match.RightLimits[BlobNum];
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

        Vector2 BallDeltaDirection => Ball.Velocity.DirectionTo(Velocity);
        float BallDeltaDistance => Ball.Velocity.To(Velocity).magnitude * Time.fixedDeltaTime;
        bool IsCollidingBelow => Bottom.y <= PhysicsWorld.Ground;
        bool IsCollidingLeft => Position.x < LeftLimit;
        bool IsCollidingRight => Position.x > RightLimit;
        bool IsBelowNetEdge => Bottom.IsBelow(PhysicsWorld.NetEdgeTop);
        bool CanCollideWithNet => Match.IsJumpOverNet && IsBelowNetEdge;

        #region Ball Collision

        int BallLayerMask => 1 << Ball.LAYER;
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
        protected bool IsRunning => KeyPressed[1] || KeyPressed[2];
        Side MoveSide => KeyPressed[1] switch
        {
            true when !KeyPressed[2] => Side.Left,
            false when KeyPressed[2] => Side.Right,
            _ => Side.None
        };
        int MoveSign => MoveSide.GetSign();
        bool IsJumping => KeyPressed[0];
        IJumpStrategy JumpStrategy { get; }

        #endregion

        #region Logic Member

        public Side OwnSide => DefaultBlobNum % 2 == 0 ? Side.Left : Side.Right;
        public Side EnemySide => OwnSide.Other();
        bool IsOnLeftTeam => PlayerData.Side == Side.Left;
        bool IsSwitched => IsOnLeftTeam ? Match.LeftSwitched : Match.RightSwitched;
        int BlobNum => IsSwitched ? TeamBlobNum : DefaultBlobNum;
        int DefaultBlobNum => PlayerData.PlayerNum;
        int TeamBlobNum => (PlayerData.PlayerNum + 2) % 4;

        #endregion

        #region Graphics Member

        public const float NAME_LABEL_OFFSET = -9.3f;
        public const float SHADOW_X = 1.588f;
        public const float SHADOW_Y = -1.73f;
        public const float SHADOW_MOD = 0.1f;
        
        static readonly int IsRunningAnim = Animator.StringToHash("isRunning");
        static readonly int IsGroundedAnim = Animator.StringToHash("grounded");
        
        Animator Animator { get; }
        
        #endregion
        
        protected Player(Match match, PlayerData playerData, GameObject prefab = null)
        {
            Match = match;
            PlayerData = playerData;

            if (prefab)
            {
                PlayerObj = Object.Instantiate(prefab, SpawnPoint, Quaternion.identity);
                PlayerGraphics = new PlayerGraphics(this, PlayerData);
                Animator = PlayerObj.GetComponent<Animator>();
            }

            UpperCollider = Colliders[0];
            LowerCollider = Colliders[1];
            
            JumpStrategy = JumpStrategyFactory.Create(this, match);

            Position = SpawnPoint;

            SubscribeEventHandler();
        }

        public virtual void FixedUpdate()
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

            if (Match.Ball != null)
            {
                var ballCollisionResult = BallCollision;

                if (ballCollisionResult.HasValue)
                {
                    var largeRadius = ballCollisionResult.Value.Radius + Match.Ball.Radius;
                    var ballCentroid = Position + ballCollisionResult.Value.Offset -
                                     ballCollisionResult.Value.Result.normal * largeRadius;
                    var ballNormal = -ballCollisionResult.Value.Result.normal;
                    Match.Ball.ResolvePlayerCollision(this, ballCentroid, ballNormal);
                }
            }

            if (!PlayerObj) return;

            PlayerObj.transform.position = Position;

            Animator.SetBool(IsRunningAnim, IsRunning);
            Animator.SetBool(IsGroundedAnim, IsGrounded);

            PlayerGraphics.SetShadow();
            PlayerGraphics.SetNameLabelPos();
        }

        #region Match Methods

        protected virtual void OnReady(Side side)
        {
        }

        protected virtual void OnAlpha(int playerNum, bool isTransparent)
        {
            if (playerNum == DefaultBlobNum)
            {
                PlayerGraphics?.SetAlpha(isTransparent);
            }
        }

        protected virtual void OnStop()
        {
            if (Match.IsPogo) KeyPressed[0] = false;

            if (!Match.IsJumpOverNet) return;

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
            Match.Ready += OnReady;
            Match.Alpha += OnAlpha;
            Match.Stop += OnStop;
            Match.Over += OnOver;
        }

        public virtual void Dispose()
        {
            Match.Ready -= OnReady;
            Match.Alpha -= OnAlpha;
            Match.Stop -= OnStop;
            Match.Over -= OnOver;

            if (PlayerObj) Object.Destroy(PlayerObj);
        }
    }
}
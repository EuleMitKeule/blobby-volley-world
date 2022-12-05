using System;
using Blobby.Game.Entities.States;
using Blobby.Game.Physics;
using UnityEngine;

namespace Blobby.Game.Entities
{
    public class BallComponent : MonoBehaviour
    {
        GameObject MatchObject => transform.parent.gameObject;
        MatchComponent MatchComponent { get; set; }

        public bool IsSwitched { get; set; }
        public Vector2[] SpawnPoints => new[] { new Vector2(-10, -1), new Vector2(10, -1) };

        #region Physics Member

        public const int LAYER = 6;
        const int MAP_LAYERMASK = 1 << PhysicsWorld.MAP_LAYER;
        const int PLAYER_LAYERMASK = 1 << PlayerComponent.LAYER;
        public const float SHOT_VELOCITY = 45.4f;
        const float MAX_VELOCITY = 52.2f;
        public const float GROUND_VELOCITY_THRESHOLD = 3.5f;
        public const float GRAVITY = 78f;

        public Vector2 Position { get; set; }
        Vector2 TransformPosition => transform.position;
        public Vector2 Velocity { get; set; }
        public float Gravity { get; set; }
        public float Rotation { get; set; }
        public float AngularVelocity { get; set; }
        public float AngularVelocityMultiplier = 25f;

        Vector2 DeltaVelocity => Vector2.up * (UnityEngine.Time.fixedDeltaTime * Gravity);
        Vector2 DeltaPosition => Velocity * UnityEngine.Time.fixedDeltaTime;
        float DeltaRotation => UnityEngine.Time.fixedDeltaTime * AngularVelocity;
        Vector2 ClampedVelocity =>
            Velocity.magnitude > MAX_VELOCITY ? Velocity.normalized * MAX_VELOCITY : Velocity;
        float TraveledDistance => (Velocity * UnityEngine.Time.fixedDeltaTime).magnitude;

        bool IsOnLeftSide => Position.IsLeftOf(0f);
        Side CurrentSide => IsOnLeftSide ? Side.Left : Side.Right;
        bool HasSideChanged => CurrentSide != Side;
        public Side Side { get; set; }

        CircleCollider2D Collider { get; set; }
        public float Radius => Collider.radius;
        public Vector2 Top => Position + Vector2.up * Radius;
        public Vector2 Bottom => Position + Vector2.down * Radius;

        RaycastHit2D MapCollision =>
            Physics2D.CircleCast(TransformPosition, Radius, Velocity, TraveledDistance, MAP_LAYERMASK);
        RaycastHit2D PlayerCollision =>
            Physics2D.CircleCast(TransformPosition, Radius, Velocity, TraveledDistance, PLAYER_LAYERMASK);
        bool IsCollidingWithPlayer => PlayerCollision.collider;
        bool CanCollideWithPlayer => State != Stopped;

        Vector2 GroundNormal => new Vector2(Velocity.x * 0.5f, Mathf.Abs(Velocity.y * 0.5f)).normalized * 0.5f;
        Vector2 GroundTennisNormal => new Vector2(Velocity.x, Mathf.Abs(Velocity.y * 0.95f)).normalized * 0.95f;

        #endregion

        #region State Member

        public IBallState Ready { get; private set; }
        public IBallState Running { get; private set; }
        public IBallState RunningTennis { get; private set; }
        public IBallState Stopped { get; private set; }

        IBallState State { get; set; }

        #endregion

        #region Event Member

        public event Action NetHit;
        public event Action GroundHit;
        public event Action<PlayerComponent> PlayerHit;
        public event Action WallHit;
        public event Action<Side> SideChanged;

        #endregion
        
        protected void Awake()
        {
            Collider = GetComponent<CircleCollider2D>();
            MatchComponent = MatchObject.GetComponent<MatchComponent>();
            
            Ready = new BallReadyState(this, MatchComponent);
            Running = new BallRunningState(this);
            RunningTennis = new BallRunningTennisState(this);
            Stopped = new BallStoppedState(this, MatchComponent);

            SetState(Ready);

            SubscribeEventHandler();
        }

        void FixedUpdate()
        {
            Velocity -= DeltaVelocity;
            Velocity = ClampedVelocity;

            Position += DeltaPosition;
            Rotation += DeltaRotation;

            State.FixedUpdate();

            if (HasSideChanged && State == Running)
            {
                Side = CurrentSide;
                InvokeSideChanged(Side);
            }

            transform.position = Position;
            transform.rotation = Rotation.ToQuaternion();
            
        }

        public void SetState(IBallState newState)
        {
            if (newState == null) return;

            State?.ExitState();

            State = newState;
            State.EnterState();
        }

        public void HandleMapCollision()
        {
            var mapResult = MapCollision;

            if (!mapResult.collider) return;

            State.OnCollision(mapResult);

            var normal = CalculateMapCollisionNormal(mapResult);
            var speed = Velocity.magnitude;

            if (CanCollideWithPlayer && IsCollidingWithPlayer)
            {
                var playerResult = PlayerCollision;
                normal = (normal * speed + playerResult.normal).normalized;
                speed = SHOT_VELOCITY;
            }

            Velocity = normal * speed;
            Position = mapResult.centroid + normal * (speed * 0.02f);
        }

        Vector2 CalculateMapCollisionNormal(RaycastHit2D result)
        {
            if (!result.collider) return Vector2.zero;

            if (result.collider == PhysicsWorld.GroundCollider)
            {
                AngularVelocity *= 0.5f;
                return State == RunningTennis ? GroundTennisNormal : GroundNormal;
            }

            if (result.collider == PhysicsWorld.NetEdgeCollider)
            {
                return (Velocity - 2 * Velocity.Dot(-result.normal) * -result.normal).normalized;
            }

            var quarterSide = TransformPosition.IsLeftOf(Side.GetSign() * 9.6f) ? Side.Left : Side.Right;
            return new Vector2(Mathf.Abs(Velocity.x) * -quarterSide.GetSign(), Velocity.y).normalized;
        }
        
        public void ResolvePlayerCollision(PlayerComponent playerComponent, Vector2 centroid, Vector2 normal)
        {
            State.OnPlayerHit(playerComponent, centroid, normal);
        }

        void OnBombTimerStopped()
        {
            State.OnBombTimerStopped();
        }

        void OnAutoDropTimerStopped()
        {
            if (MatchComponent.MatchData.JumpMode == JumpMode.NoJump) SetState(Running);
        }

        void SubscribeEventHandler()
        {
            MatchComponent.AutoDropTimer.AutoDropTimerStopped += OnAutoDropTimerStopped;
            MatchComponent.BombTimer.BombTimerStopped += OnBombTimerStopped;
        }

        public void OnDestroy()
        {
            MatchComponent.AutoDropTimer.AutoDropTimerStopped -= OnAutoDropTimerStopped;
            MatchComponent.BombTimer.BombTimerStopped -= OnBombTimerStopped;
        }

        public void InvokeNetHit() => NetHit?.Invoke();
        public void InvokeGroundHit() => GroundHit?.Invoke();
        public void InvokePlayerHit(PlayerComponent playerComponent) => PlayerHit?.Invoke(playerComponent);
        public void InvokeWallHit() => WallHit?.Invoke();
        public void InvokeSideChanged(Side side) => SideChanged?.Invoke(side);
    }
}

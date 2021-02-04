using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blobby.Game.Physics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Blobby.Game.Entities
{
    public abstract class BallComponent : MonoBehaviour, IDisposable
    {
        GameObject MatchObject => transform.parent.gameObject;

        public CircleCollider2D Collider { get; protected set; }
        public float Radius => Collider.radius;

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        public float Rotation { get; set; }
        public float AngularVelocity { get; set; }
        
        public float Gravity { get; set; }
        public Side Side { get; set; }
        public bool IsSwitched { get; set; }

        public const int LAYER = 6;

        public const float BALL_SHOT_VELOCITY = 45.4f;

        public Vector2[] SpawnPoints
        {
            get { return new[] {new Vector2(-10, -1), new Vector2(10, -1)}; }
        }

        public const float GROUND_VELOCITY_THRESHOLD = 3.5f;

        #region States

        public IBallState Ready { get; private set; }
        public IBallState Running { get; private set; }
        public IBallState RunningTennis { get; private set; }
        public IBallState Stopped { get; private set; }

        #endregion

        public event Action NetHit;
        public event Action NetEdgeHit;
        public event Action GroundHit;
        public event Action<Player> PlayerHit;
        public event Action WallHit;
        public event Action<Side> SideChanged;

        protected MatchComponent MatchComponent;
        protected MatchData _matchData;
        IBallState _state;

        #region Graphics Member

        public const float SHADOW_MOD = 0.1f;

        #endregion

        void Awake()
        {
            Collider = GetComponent<CircleCollider2D>();

            MatchComponent = MatchObject.GetComponent<MatchComponent>();
            
            Ready = new BallReadyState(this, MatchComponent);
            Running = new BallRunningState(this, MatchComponent);
            RunningTennis = new BallRunningTennisState(this, MatchComponent);
            Stopped = new BallStoppedState(this, MatchComponent);

            SetState(Ready);
        }

        public void SetState(IBallState newState)
        {
            if (newState == null) return;

            _state?.ExitState();

            _state = newState;
            _state.EnterState();
        }

        protected virtual void FixedUpdate()
        {
            Velocity -= Vector2.up * (Time.fixedDeltaTime * Gravity);

            if (Velocity.magnitude >= MatchComponent.PhysicsSettings.ballMaxVelocity)
                Velocity = Velocity.normalized * MatchComponent.PhysicsSettings.ballMaxVelocity;
            
            Position += Velocity * Time.fixedDeltaTime;

            _state.FixedUpdate();
            
            Rotation += Time.fixedDeltaTime * AngularVelocity;

            var curSide = Position.x > 0f ? Side.Right : Side.Left;

            if (Side != curSide)
            {
                Side = curSide;
                InvokeSideChanged(curSide);
            }

            transform.position = Position;
            transform.rotation = Quaternion.Euler(0, 0, Rotation);
        }

        public void HandleMapCollision()
        {
            var layerMask = 1 << PhysicsWorld.MAP_LAYER;
            var playerLayerMask = 1 << Player.LAYER;
            
            var lastPosition = (Vector2)transform.position;
            var traveledDistance = (Velocity * Time.fixedDeltaTime).magnitude;
            
            var result = Physics2D.CircleCast(lastPosition, Radius, Velocity, traveledDistance, layerMask);
            
            var playerResult = Physics2D.CircleCast(lastPosition, Radius, Velocity, traveledDistance, playerLayerMask);
            var isCollidingWithPlayer = playerResult.collider != null;
            
            if (result.collider == null) return;
            
            var normal = -result.normal;
            
            if (result.collider == PhysicsWorld.GroundCollider)
            {
                normal = new Vector2(Velocity.x * 0.5f, Velocity.y * -0.5f);
                _state.OnGroundHit();
            }
            else if (result.collider == PhysicsWorld.NetEdgeCollider)
            {
                normal = (Velocity - 2 * Vector2.Dot(Velocity, normal) * normal).normalized * Velocity.magnitude;
                _state.OnNetHit();
            }
            else
            {
                var leftWall = BallObj.transform.position.x < -9.6f || 0f < BallObj.transform.position.x && BallObj.transform.position.x < 9.6f;
                var sign = leftWall ? 1f : -1f;
            
                normal = new Vector2(Mathf.Abs(Velocity.x) * sign, Velocity.y);
                _state.OnWallHit();
            }
                        
            if (isCollidingWithPlayer && _state != Stopped)
            {
                normal = (normal + playerResult.normal).normalized * BALL_SHOT_VELOCITY;
            }
            Velocity = normal;
            Position = result.centroid + normal * 0.02f;
        }

        public void HandleMapCollisionTennis()
        {
            var layerMask = 1 << PhysicsWorld.MAP_LAYER;

            var lastPosition = (Vector2)BallObj.transform.position;
            var traveledDistance = (Velocity * Time.fixedDeltaTime).magnitude;

            var result = Physics2D.CircleCast(lastPosition, Radius, Velocity, traveledDistance, layerMask);

            if (result.collider == null) return;

            Position = result.centroid;

            if (result.collider == PhysicsWorld.GroundCollider)
            {
                InvokeGroundHit();
                Velocity = new Vector2(Velocity.x, -0.95f * Velocity.y);
            }
            else if (result.collider == PhysicsWorld.NetEdgeCollider)
            {
                InvokeNetHit();
                var resetNormal = (Velocity - 2 * Vector2.Dot(Velocity, -result.normal) * -result.normal).normalized;
                Velocity = resetNormal * Velocity.magnitude;
            }
            else
            {
                InvokeWallHit();
                Velocity = new Vector2(-Velocity.x, Velocity.y);
            }
        }
        
        public void ResolvePlayerCollision(Player player, Vector2 centroid, Vector2 normal)
        {
            _state.OnPlayerHit(player, centroid, normal);
        }

        void OnBombTimerStopped()
        {
            _state.OnBombTimerStopped();
        }

        protected virtual void OnAutoDropTimerStopped()
        {
            if (MatchComponent.MatchData.JumpMode == JumpMode.NoJump) SetState(Running);
        }

        protected virtual void SubscribeEventHandler()
        {
            MatchComponent.AutoDropTimer.AutoDropTimerStopped += OnAutoDropTimerStopped;
            MatchComponent.BombTimer.BombTimerStopped += OnBombTimerStopped;
        }

        public virtual void Dispose()
        {
            MatchComponent.AutoDropTimer.AutoDropTimerStopped -= OnAutoDropTimerStopped;
            MatchComponent.BombTimer.BombTimerStopped -= OnBombTimerStopped;

            Object.Destroy(BallObj);
        }

        public void InvokeNetHit() => NetHit?.Invoke();
        public void InvokeGroundHit() => GroundHit?.Invoke();
        public void InvokePlayerHit(Player player) => PlayerHit?.Invoke(player);
        public void InvokeWallHit() => WallHit?.Invoke();
        public void InvokeSideChanged(Side side) => SideChanged?.Invoke(side);
    }
}

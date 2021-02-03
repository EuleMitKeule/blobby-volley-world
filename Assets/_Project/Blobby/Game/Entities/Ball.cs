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
    public abstract class Ball : IDisposable
    {
        public enum NetHitResult { None, Wall, Edge}

        public GameObject BallObj { get; protected set; }

        public CircleCollider2D Collider { get; protected set; }
        public float Radius => Collider.radius;

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        public float Rotation { get; set; }
        public float AngularVelocity { get; set; }
        
        public float Gravity { get; set; }
        public Side Side { get; set; }

        public const int LAYER = 6;

        public const float BALL_SHOT_VELOCITY = 45.4f;

        const float LEFT_LIMIT = -18.01f;
        const float RIGHT_LIMIT = 18.01f;

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

        protected Match _match;
        protected MatchData _matchData;
        IBallState _state;

        #region Graphics Member

        public const float SHADOW_MOD = 0.1f;

        #endregion
        
        public Ball(Match match, MatchData matchData)
        {
            _match = match;
            _matchData = matchData;

            Ready = new BallReadyState(this, match, matchData);
            Running = new BallRunningState(this, match, matchData);
            RunningTennis = new BallRunningTennisState(this, match, matchData);
            Stopped = new BallStoppedState(this, match, matchData);
        }

        public void SetState(IBallState newState)
        {
            if (newState == null) return;

            _state?.ExitState();

            _state = newState;
            _state.EnterState();
        }

        public virtual void FixedUpdate()
        {
            Velocity -= Vector2.up * (Time.fixedDeltaTime * Gravity);

            if (Velocity.magnitude >= _match.PhysicsSettings.ballMaxVelocity)
                Velocity = Velocity.normalized * _match.PhysicsSettings.ballMaxVelocity;
            
            Position += Velocity * Time.fixedDeltaTime;

            _state.FixedUpdate();
            
            Rotation += Time.fixedDeltaTime * AngularVelocity;

            var curSide = Position.x > 0f ? Side.Right : Side.Left;

            if (Side != curSide)
            {
                Side = curSide;
                InvokeSideChanged(curSide);
            }
        }

        public void HandleMapCollision()
        {
            var layerMask = 1 << PhysicsWorld.MAP_LAYER;
            var playerLayerMask = 1 << Player.LAYER;
            
            var lastPosition = (Vector2)BallObj.transform.position;
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
            if (_match.MatchData.JumpMode == JumpMode.NoJump) SetState(Running);
        }

        protected virtual void SubscribeEventHandler()
        {
            _match.AutoDropTimer.AutoDropTimerStopped += OnAutoDropTimerStopped;
            _match.BombTimer.BombTimerStopped += OnBombTimerStopped;
        }

        public virtual void Dispose()
        {
            _match.AutoDropTimer.AutoDropTimerStopped -= OnAutoDropTimerStopped;
            _match.BombTimer.BombTimerStopped -= OnBombTimerStopped;

            Object.Destroy(BallObj);
        }

        public void InvokeNetHit() => NetHit?.Invoke();
        public void InvokeGroundHit() => GroundHit?.Invoke();
        public void InvokePlayerHit(Player player) => PlayerHit?.Invoke(player);
        public void InvokeWallHit() => WallHit?.Invoke();
        public void InvokeSideChanged(Side side) => SideChanged?.Invoke(side);
    }
}

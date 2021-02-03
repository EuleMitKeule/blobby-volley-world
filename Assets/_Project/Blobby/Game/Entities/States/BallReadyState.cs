using System.Linq;
using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game.Entities;
using Blobby.Game.Physics;
using Blobby.Models;
using UnityEngine;
using static Blobby.Game.Entities.BallComponent;

namespace Blobby.Game.Entities
{
    public class BallReadyState : IBallState
    {
        BallComponent _ballComponent;
        MatchComponent _matchComponent;

        public BallReadyState(BallComponent ballComponent, MatchComponent matchComponent) => (_ballComponent, _matchComponent) = (ballComponent, matchComponent);

        public void EnterState()
        {
            MainThreadManager.Run(() =>
            {
                _ballComponent.BallObj.layer = 6;

                var ballPos = _matchComponent.LastWinner == Side.Left ||
                              _matchComponent.LastWinner == Side.None
                    ? _ballComponent.SpawnPoints[0]
                    : _ballComponent.SpawnPoints[1];

                _ballComponent.Position = ballPos;
                _ballComponent.BallObj.transform.position = ballPos;
                _ballComponent.Velocity = Vector2.zero;
                _ballComponent.Rotation = 0f;
                _ballComponent.AngularVelocity = 0f;
                _ballComponent.Gravity = 0f;
                _ballComponent.Side = _matchComponent.LastWinner;

                _ballComponent.InvokeSideChanged(_ballComponent.Side);
            });
        }

        public void ExitState()
        {

        }

        public void FixedUpdate()
        {
        }

        public void OnPlayerHit(Player player, Vector2 centroid, Vector2 normal)
        {
            _ballComponent.Position = centroid;
            _ballComponent.Velocity = normal * BALL_SHOT_VELOCITY;

            _ballComponent.InvokePlayerHit(player);
            _ballComponent.SetState(_ballComponent.Running);
        }

        public void OnWallHit()
        {
            
        }

        public void OnNetHit()
        {
            
        }

        public void OnGroundHit()
        {

        }

        public void OnBombTimerStopped() { }
    }
}
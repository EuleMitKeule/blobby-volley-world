using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Blobby.Components
{
    public class OnlinePlayerComponent : PlayerBehavior
    {
        public MatchData MatchData { get; set; }
        public bool Switched { get; set; }

        Animator _animator;

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void PlayerUpdate(Vector2 position, bool isGrounded, bool isRunning)
        {
            networkObject.position = position;
            networkObject.grounded = isGrounded;
            networkObject.isRunning = isRunning;
        }

        public void OnPlayerPositionReceived(Vector2 position)
        {
            MainThreadManager.Run(() =>
            {
                Debug.Log($"position update received {position}");
                networkObject.positionInterpolation.target = position;
                transform.position = position;
            });
        }

        void FixedUpdate()
        {
            if (!networkObject.IsServer)
            {
                var wantedPosition = new Vector2((Switched ? -1 : 1) * networkObject.position.x, networkObject.position.y);

                transform.position = wantedPosition;

                _animator.SetBool("isRunning", networkObject.isRunning);
                _animator.SetBool("grounded", networkObject.grounded);
            }
        }
    }
}

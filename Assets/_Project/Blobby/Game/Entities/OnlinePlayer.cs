using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Models;
using Blobby.Networking;
using Blobby.Components;

namespace Blobby.Game.Entities
{
    public class OnlinePlayer : Player
    {
        OnlinePlayerComponent OnlinePlayerComponent { get; }

        public OnlinePlayer(MatchComponent matchComponent, PlayerData playerData, MatchData matchData) : base(matchComponent, playerData)
        {
            PlayerObj = NetworkManager.Instance.InstantiatePlayer(0, SpawnPoint).gameObject;

            var colliders = PlayerObj.GetComponents<CircleCollider2D>();
            UpperCollider = colliders[0];
            LowerCollider = colliders[1];
            
            OnlinePlayerComponent = PlayerObj.GetComponent<OnlinePlayerComponent>();
            OnlinePlayerComponent.MatchData = matchData;

            SubscribeControls();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            PlayerObj.transform.position = Position;
            
            OnlinePlayerComponent.PlayerUpdate(Position, IsGrounded, IsRunning);
        }

        void SubscribeControls()
        {
            ServerConnection.ButtonDownReceived += OnButtonDownReceived;
            ServerConnection.ButtonUpReceived += OnButtonUpReceived;
        }

        void UnsubscribeControls()
        {
            ServerConnection.ButtonDownReceived -= OnButtonDownReceived;
            ServerConnection.ButtonUpReceived -= OnButtonUpReceived;
        }

        void OnButtonDownReceived(int playerNum, int button)
        {
            if (playerNum != PlayerData.PlayerNum) return;

            switch (button)
            {
                case 0:
                    OnJumpDown();
                    break;
                case 1:
                    OnLeftDown();
                    break;
                case 2:
                    OnRightDown();
                    break;
            }
        }

        void OnButtonUpReceived(int playerNum, int button)
        {
            if (playerNum != PlayerData.PlayerNum) return;

            switch (button)
            {
                case 0:
                    OnJumpUp();
                    break;
                case 1:
                    OnLeftUp();
                    break;
                case 2:
                    OnRightUp();
                    break;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            
            UnsubscribeControls();
        }
    }
}

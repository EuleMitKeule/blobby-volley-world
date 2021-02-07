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
    public class OnlinePlayerComponent : PlayerComponent
    {
        protected override void Awake()
        {
            base.Awake();

            SubscribeControls();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnsubscribeControls();
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
    }
}

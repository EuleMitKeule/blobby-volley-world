using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Components;
using Blobby.Models;
using Blobby.Networking;
using Blobby.UserInterface;
using System;
using UnityEngine;

namespace Blobby.Game.Entities
{
    public class ClientPlayer : IDisposable
    {
        public GameObject PlayerObj { get; private set; }
        public PlayerData PlayerData { get; private set; }
        public PlayerGraphics PlayerGraphics { get; private set; }

        ClientMatch _clientMatch;
        MatchData _matchData;
        OnlinePlayerComponent _onlinePlayerComponent;

        public ClientPlayer(ClientMatch clientMatch, MatchData matchData, GameObject playerObj, PlayerData playerData)
        {
            _clientMatch = clientMatch;
            _matchData = matchData;
            PlayerObj = playerObj;
            PlayerData = playerData;

            // PlayerGraphics = new PlayerGraphics(this, playerData);

            if (matchData.PlayerMode == PlayerMode.Ghost)
            {
                var enemySide = PanelSettings.SettingsData.Side == Side.Left ? Side.Right : Side.Left;
                if (PlayerData.Side == enemySide) PlayerGraphics.SetInvisible(true);
            }

            _onlinePlayerComponent = PlayerObj.GetComponent<OnlinePlayerComponent>();
            _onlinePlayerComponent.MatchData = _matchData;

            _onlinePlayerComponent.Switched = _clientMatch.Switched;
            SubscribeEventHandler();
        }

        void FixedUpdate()
        {
            PlayerGraphics.SetShadow();
            PlayerGraphics.SetNameLabelPos();
        }

        public void SetSwitched(bool value)
        {
            _onlinePlayerComponent.Switched = value;
        }

        void OnAlpha(int playerNum, bool isAlpha)
        {
            if (playerNum == PlayerData.PlayerNum)
            {
                PlayerGraphics.SetAlpha(isAlpha);
            }
        }

        void OnPlayerPositionReceived(Vector2 position, int playerNum)
        {
            MainThreadManager.Run(() =>
            {
                if (playerNum == PlayerData.PlayerNum) _onlinePlayerComponent.OnPlayerPositionReceived(position);
            });
        }

        void SubscribeEventHandler()
        {
            TimeComponent.FixedUpdateTicked += FixedUpdate;
            ClientConnection.AlphaReceived += OnAlpha;
            ClientConnection.PlayerPositionReceived += OnPlayerPositionReceived;
        }

        public void Dispose()
        {
            TimeComponent.FixedUpdateTicked -= FixedUpdate;
            ClientConnection.AlphaReceived -= OnAlpha;
            ClientConnection.PlayerPositionReceived -= OnPlayerPositionReceived;

            GameObject.Destroy(PlayerObj);
        }
    }
}

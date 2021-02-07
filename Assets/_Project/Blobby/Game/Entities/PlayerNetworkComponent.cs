using System;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using Blobby.Models;
using Blobby.Networking;
using Blobby.UserInterface;
using UnityEngine;

namespace Blobby.Game.Entities
{
    public class PlayerNetworkComponent : PlayerBehavior, IPlayerGraphicsProvider
    {
        PlayerComponent PlayerComponent { get; set; }
        GameObject MatchObj => transform.parent.gameObject;
        OnlineMatchComponent OnlineMatchComponent { get; set; }
        public ClientMatchComponent ClientMatchComponent { get; set; }
        
        Vector2 Position => PlayerComponent.Position;
        bool IsServer => networkObject.IsServer;

        Vector2 NetworkPosition => new Vector2
        (
            SwitchModifier * networkObject.position.x,
            networkObject.position.y
        );

        public bool IsGrounded => networkObject.isGrounded;
        public bool IsRunning => networkObject.isRunning;
        public bool IsInvisible => false;
        bool IsSwitched => OwnSide != PanelSettings.SettingsData.Side;
        PlayerData _playerData;

        public PlayerData PlayerData
        {
            get => _playerData;
            set
            {
                _playerData = value;
                PlayerDataChanged?.Invoke(_playerData);
            }
        }

        public event Action<PlayerData> PlayerDataChanged;
        public event Action<int, bool> AlphaChanged;

        Action NetAwake => IsServer ? ServerAwake : new Action(ClientAwake);
        Action NetFixedUpdate => IsServer ? ServerFixedUpdate : new Action(ClientFixedUpdate);
        Side OwnSide => ClientMatchComponent.NetPlayerNum % 2 == 0 ? Side.Left : Side.Right;
        public Side EnemySide => OwnSide.Other();
        bool IsOnLeftTeam => OwnSide == Side.Left;
        float SwitchModifier => IsSwitched ? -1 : 1;

        protected override void NetworkStart()
        {
            base.NetworkStart();
            NetAwake();
        }
        
        void FixedUpdate() => NetFixedUpdate();

        void ServerAwake()
        {
            PlayerComponent = gameObject.AddComponent<OnlinePlayerComponent>();
            OnlineMatchComponent = MatchObj.GetComponent<OnlineMatchComponent>();
            
            var index = OnlineMatchComponent.PlayerObjList.IndexOf(gameObject);
            var playerData = OnlineMatchComponent.PlayerDataList[index];

            networkObject.SendRpc(PlayerBehavior.RPC_SET_PLAYER_DATA, Receivers.All,
                playerData.PlayerNum, playerData.Name, playerData.Color);
            
            OnlineMatchComponent.Players.Add(PlayerComponent);

            if (OnlineMatchComponent.Players.Count < OnlineMatchComponent.MatchData.PlayerCount) return;

            OnlineMatchComponent.StartMatch();
            ServerHandler.SetState(ServerHandler.RunningState);
        }

        void ClientAwake()
        {
            ClientConnection.AlphaReceived += OnAlphaReceived;
        }

        void OnAlphaReceived(int playerNum, bool isTransparent) => AlphaChanged?.Invoke(playerNum, isTransparent);

        void ServerFixedUpdate()
        {
            networkObject.position = Position;
            networkObject.isGrounded = PlayerComponent.IsGrounded;
            networkObject.isRunning = PlayerComponent.IsRunning;
        }

        void ClientFixedUpdate()
        {
            transform.position = NetworkPosition;
        }

        public override void SetPlayerData(RpcArgs args)
        {
            var playerNum = args.GetNext<int>();
            var username = args.GetNext<string>();
            var color = args.GetNext<Color>();

            PlayerData = new PlayerData(playerNum, username, color);
            
            if (IsServer)
            {
               PlayerComponent.PlayerData = PlayerData;
            }
        }
    }
}
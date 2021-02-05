using System;
using BeardedManStudios.Forge.Networking.Generated;
using Blobby.Models;
using UnityEngine;

namespace Blobby.Game.Entities
{
    public class PlayerNetworkComponent : PlayerBehavior, IPlayerAnimProvider
    {
        PlayerComponent PlayerComponent { get; set; }
        Vector2 Position => PlayerComponent.Position;
        bool IsServer => networkObject.IsServer;

        Vector2 NetworkPosition => new Vector2
        (
            SwitchModifier * networkObject.position.x,
            networkObject.position.y
        );

        public bool IsGrounded => networkObject.grounded;
        public bool IsRunning => networkObject.isRunning;

        PlayerData _playerData; //TODO add networkObject.name und playerNum und color
        PlayerData PlayerData
        {
            get => _playerData;
            set
            {
                _playerData = value;
                PlayerDataChanged?.Invoke(value);
            }
        }
        public event Action<PlayerData> PlayerDataChanged;

        int DefaultBlobNum => PlayerData.PlayerNum;
        public bool IsInvisible => false; //TODO add networkObject.isInvisible;

        Action NetAwake => IsServer ? ServerAwake : new Action(ClientAwake);
        Action NetFixedUpdate => IsServer ? ServerFixedUpdate : new Action(ClientFixedUpdate);

        public Side OwnSide => DefaultBlobNum % 2 == 0 ? Side.Left : Side.Right;
        public Side EnemySide => OwnSide.Other();
        bool IsOnLeftTeam => OwnSide == Side.Left;
        public bool IsSwitched => false;//IsOnLeftTeam ? MatchComponent.LeftSwitched : MatchComponent.RightSwitched;
        float SwitchModifier => 1f;//MatchHandler ? -1 : 1;

        void Awake()
        {
            NetAwake();
        }
        void FixedUpdate() => NetFixedUpdate();

        void ServerAwake()
        {
            PlayerComponent = gameObject.AddComponent<PlayerComponent>();
        }

        void ClientAwake()
        {

        }

        void ServerFixedUpdate()
        {
            networkObject.position = Position;
            networkObject.grounded = PlayerComponent.IsGrounded;
            networkObject.isRunning = PlayerComponent.IsRunning;
        }

        void ClientFixedUpdate()
        {
            transform.position = NetworkPosition;
        }
    }
}
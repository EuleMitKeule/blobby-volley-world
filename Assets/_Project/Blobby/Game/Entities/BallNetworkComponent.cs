using System;
using BeardedManStudios.Forge.Networking.Generated;
using Blobby.UserInterface;
using UnityEngine;

namespace Blobby.Game.Entities
{
    public class BallNetworkComponent : BallBehavior
    {
        GameObject MatchObj => transform.parent.gameObject; 
        MatchComponent MatchComponent { get; set; }
        public ClientMatchComponent ClientMatchComponent { get; set; }
        BallComponent BallComponent { get; set; }
        Vector2 Position => BallComponent.Position;
        float Rotation => BallComponent.Rotation;

        bool IsServer => networkObject.IsServer;
        Vector2 NetworkPosition => new Vector2
        (
            SwitchModifier * networkObject.pos.x, 
            networkObject.pos.y
        );
        Quaternion NetworkRotation => Quaternion.Euler(0f, 0f, networkObject.rot);
        Side OwnSide => ClientMatchComponent.NetPlayerNum % 2 == 0 ? Side.Left : Side.Right;
        bool IsSwitched => OwnSide != PanelSettings.SettingsData.Side;
        float SwitchModifier => IsSwitched ? -1 : 1;
        Action NetAwake => IsServer ? ServerAwake : new Action(ClientAwake);
        Action NetFixedUpdate => IsServer ? ServerFixedUpdate : new Action(ClientFixedUpdate);
        
        protected override void NetworkStart()
        {
            base.NetworkStart();
            
            NetAwake();
        }

        void FixedUpdate() => NetFixedUpdate();

        void ServerAwake()
        {
            MatchComponent = MatchObj.GetComponent<MatchComponent>();
            BallComponent = gameObject.AddComponent<BallComponent>();
            
            MatchComponent.BallComponent = BallComponent;
            Debug.Log($"Ballcomponent existiert? {BallComponent != null}");
            MatchComponent.SubscribeBallEvents();
        }
        void ClientAwake()
        {
            
        }

        void ServerFixedUpdate()
        {
            networkObject.pos = Position;
            networkObject.rot = Rotation;
        }
        void ClientFixedUpdate()
        {
            var cachedTransform = GetComponent<Transform>();
            cachedTransform.position = NetworkPosition;
            cachedTransform.rotation = NetworkRotation;
        }
    }
}
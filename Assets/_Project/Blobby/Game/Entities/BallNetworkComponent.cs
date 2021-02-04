using System;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

namespace Blobby.Game.Entities
{
    public class BallNetworkComponent : BallBehavior
    {
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
        Action NetAwake => IsServer ? ServerAwake : new Action(ClientAwake);
        Action NetFixedUpdate => IsServer ? ServerFixedUpdate : new Action(ClientFixedUpdate);

        float SwitchModifier => BallComponent.IsSwitched ? -1 : 1;
        
        void Awake() => NetAwake();
        void FixedUpdate() => NetFixedUpdate();

        void ServerAwake()
        {
            BallComponent = gameObject.AddComponent<BallComponent>();
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
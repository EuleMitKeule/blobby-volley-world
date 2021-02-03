using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Models;
using Blobby.Networking;
using Blobby.Components;
using UnityEditor.PackageManager;

namespace Blobby.Game.Entities
{
    public class OnlineBallComponent : BallComponent
    {
        public MatchData MatchData { get; set; }
        public bool Switched { get; set; }

        BallNetworkComponent BallNetworkComponent { get; set; }

        static GameObject _shadowObj;
        static GameObject _arrowObj;
        static SpriteRenderer _arrowSR;

        void Awake()
        {
            Collider = BallObj.GetComponent<CircleCollider2D>();

            SubscribeEventHandler();

            SetState(Ready);

            _shadowObj = transform.GetChild(0).gameObject;
            _arrowObj = transform.GetChild(1).gameObject;
            _arrowSR = _arrowObj.GetComponent<SpriteRenderer>();
            _arrowSR.enabled = false;

            BallNetworkComponent = gameObject.AddComponent<BallNetworkComponent>();
            BallNetworkComponent.BallComponent = this;
        }
    }

    public class BallNetworkComponent : BallBehavior
    {
        BallComponent BallComponent { get; set; }
        Vector2 Position => BallComponent.Position;
        float Rotation => BallComponent.Rotation;

        bool IsServer => networkObject.IsServer;
        Action NetAwake => IsServer ? ServerAwake : new Action(ClientAwake);
        Action NetFixedUpdate => IsServer ? ServerFixedUpdate : new Action(ClientFixedUpdate);

        void Awake() => NetAwake();
        void FixedUpdate() => NetFixedUpdate();

        void ServerAwake()
        {
            BallComponent = gameObject.AddComponent<OnlineBallComponent>();
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
            var wantedPosition = new Vector2((BallComponent.Switched ? -1 : 1) * networkObject.pos.x, networkObject.pos.y);

            transform.position = wantedPosition;
            transform.rotation = Quaternion.Euler(0f, 0f, networkObject.rot);

            BallComponent.SetArrow(wantedPosition);
            BallComponent.SetShadow(wantedPosition);
        }

        void SetArrow(Vector2 ballPosition)
        {
            if (_arrowObj == null || MatchData == null) return;

            _arrowObj.transform.position = new Vector2(ballPosition.x, MatchData.ArrowHeight);
            _arrowObj.transform.rotation = Quaternion.identity;

            _arrowSR.enabled = ballPosition.y > MatchData.ArrowLimit;
        }

        void SetShadow(Vector2 ballPosition)
        {
            if (_shadowObj == null || MatchData == null) return;

            var shadowX = ballPosition.x +
                          Mathf.Abs(ballPosition.y - MatchData.BallGround) * MatchData.ShadowModifier.x;
            var shadowY = MatchData.BallGround - Ball.Radius +
                          Mathf.Abs(ballPosition.y - MatchData.BallGround) * MatchData.ShadowModifier.y;

            _shadowObj.transform.position = new Vector2(shadowX, shadowY);
            _shadowObj.transform.rotation = Quaternion.identity;
        }
    }
}

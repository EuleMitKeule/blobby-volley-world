using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Models;
using Blobby.Networking;
using System.Collections;
using System.Collections.Generic;
using Blobby.Game.Entities;
using UnityEngine;

namespace Blobby.Components
{
    public class OnlineBallComponent : BallBehavior
    {
        public MatchData MatchData { get; set; }
        public bool Switched { get; set; }

        static GameObject _shadowObj;
        static GameObject _arrowObj;
        static SpriteRenderer _arrowSR;
        
        void Awake()
        {
            _shadowObj = transform.GetChild(0).gameObject;
            _arrowObj = transform.GetChild(1).gameObject;
            _arrowSR = _arrowObj.GetComponent<SpriteRenderer>();
            _arrowSR.enabled = false;

            ClientConnection.BallPositionReceived += OnBallPositionReceived;
        }

        void OnDestroy()
        {
            ClientConnection.BallPositionReceived -= OnBallPositionReceived;
        }

        public void BallUpdate(Vector2 position, float rotation)
        {
            networkObject.pos = position;
            networkObject.rot = rotation;
        }

        void OnBallPositionReceived(Vector2 position)
        {
            MainThreadManager.Run(() =>
            {
                if (gameObject == null) return;

                //Debug.Log($"ball position received {position}");
                //networkObject.posInterpolation.current = position;
                networkObject.posInterpolation.target = position;
                //networkObject.SnapInterpolations();
                transform.position = position;
            });
        }

        void FixedUpdate()
        {
            if (!networkObject.IsServer)
            {
                var wantedPosition = new Vector2((Switched ? -1 : 1) * networkObject.pos.x, networkObject.pos.y);

                //bool shouldSnap = ((Vector2)transform.position - wantedPosition).magnitude > 0.5f;
                
                transform.position = wantedPosition;
                transform.rotation = Quaternion.Euler(0f, 0f, networkObject.rot);

                //if (shouldSnap)
                //{
                //    Debug.Log(networkObject.posInterpolation.target);
                //    networkObject.posInterpolation.Enabled = false;
                //    networkObject.SnapInterpolations();
                //    networkObject.posInterpolation.current = networkObject.posInterpolation.target;
                //    networkObject.posInterpolation.Enabled = true;
                //}

                SetArrow(wantedPosition);
                SetShadow(wantedPosition);
            }
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
            // if (_shadowObj == null || MatchData == null) return;
            //
            // var shadowX = ballPosition.x + Mathf.Abs(ballPosition.y - MatchData.BallGround) * MatchData.ShadowModifier.x;
            // var shadowY = MatchData.BallGround - Ball.Radius + Mathf.Abs(ballPosition.y - MatchData.BallGround) * MatchData.ShadowModifier.y;
            //
            // _shadowObj.transform.position = new Vector2(shadowX, shadowY);
            // _shadowObj.transform.rotation = Quaternion.identity;
        }
    }
}

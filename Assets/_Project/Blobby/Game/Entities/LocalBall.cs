using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blobby.Models;

namespace Blobby.Game.Entities
{
    public class LocalBall : Ball
    {
        GameObject _arrowObj, _shadowObj;
        SpriteRenderer _arrowSR;

        LocalMatch _localMatch;

        public LocalBall(LocalMatch match, MatchData matchData) : base(match, matchData)
        {
            _localMatch = match;

            BallObj = Object.Instantiate(matchData.BallPrefab, new Vector2(-10, 5), Quaternion.identity);

            Collider = BallObj.GetComponent<CircleCollider2D>();

            _shadowObj = BallObj.transform.GetChild(0).gameObject;
            _arrowObj = BallObj.transform.GetChild(1).gameObject;
            _arrowSR = _arrowObj.GetComponent<SpriteRenderer>();
            _arrowSR.enabled = false;

            SubscribeEventHandler();

            SetState(Ready);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            BallObj.transform.position = Position;
            BallObj.transform.rotation = Quaternion.Euler(0, 0, Rotation);

            SetArrow();
            SetShadow();
        }

        void SetArrow()
        {
            if (_arrowObj != null)
            {
                _arrowObj.transform.position = new Vector2(Position.x, _matchData.ArrowHeight);
                _arrowObj.transform.rotation = Quaternion.identity;

                _arrowSR.enabled = Position.y > _matchData.ArrowLimit;
            }
        }

        void SetShadow()
        {
            var shadowX = Position.x + Mathf.Abs(Position.y - _matchData.BallGround) * SHADOW_MOD;
            var shadowY = _matchData.BallGround - Radius + Mathf.Abs(Position.y - _matchData.BallGround) * SHADOW_MOD;

            if (_shadowObj != null)
            {
                _shadowObj.transform.position = new Vector2(shadowX, shadowY);
                _shadowObj.transform.rotation = Quaternion.identity;
            }
        }
    }
}

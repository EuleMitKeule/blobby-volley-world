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
    public class OnlineBall : Ball
    {
        OnlineBallComponent _onlineBallComponent;

        public OnlineBall(Match match, MatchData matchData) : base(match, matchData)
        {
            if (matchData.GameMode != GameMode.Bomb)
            {
                BallObj = NetworkManager.Instance.InstantiateBall(0, match.MatchData.BallSpawnPoints[0]).gameObject;
            }
            else
            {
                BallObj = NetworkManager.Instance.InstantiateBall(1, match.MatchData.BallSpawnPoints[0]).gameObject;
            }

            Collider = BallObj.GetComponent<CircleCollider2D>();

            _onlineBallComponent = BallObj.GetComponent<OnlineBallComponent>();
            _onlineBallComponent.MatchData = matchData;

            SubscribeEventHandler();

            SetState(Ready);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            
            BallObj.transform.position = Position;
            BallObj.transform.rotation = Quaternion.Euler(0, 0, Rotation);

            _onlineBallComponent.BallUpdate(Position, Rotation);
        }
    }
}

using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Components;
using Blobby.Models;
using Blobby.Networking;
using Blobby.UserInterface;
using System;
using UnityEngine;

namespace Blobby.Game.Entities
{
    public class ClientBall : IDisposable
    {
        ClientMatch _clientMatch;
        MatchData _matchData;
        GameObject _ballObj;

        Components.OnlineBallComponent _onlineBallComponent;

        public ClientBall(ClientMatch clientMatch, MatchData matchData, GameObject ballObj)
        {
            _clientMatch = clientMatch;
            _matchData = matchData;
            _ballObj = ballObj;

            _onlineBallComponent = _ballObj.GetComponent<Components.OnlineBallComponent>();
            _onlineBallComponent.MatchData = _matchData;

            _onlineBallComponent.Switched = _clientMatch.Switched;

            SubscribeEventHandler();
        }

        void SubscribeEventHandler()
        {

        }

        public void Dispose()
        {
            GameObject.Destroy(_ballObj);
        }
    }
}

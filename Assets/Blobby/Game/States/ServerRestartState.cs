using BeardedManStudios.Forge.Networking;
using Blobby.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blobby.Game.States
{
    public class ServerRestartState : IServerState
    {
        public void EnterState()
        {
            ServerConnection.Stop();
        }

        public void ExitState()
        {
            
        }

        public async void OnServerDisconnected()
        {
            await Task.Delay(2000);
            ServerHandler.Start();
        }

        public void OnPlayerJoined(string username, int elo, Color color, NetworkingPlayer networkingPlayer)
        {

        }

        public void OnAllPlayersConnected()
        {

        }

        public void OnMatchOver(Side winner)
        {

        }

        public void OnSurrenderReceived(int playerNum)
        {

        }

        public void OnPlayerDisconnected(int playernum)
        {

        }

        public void OnAllPlayersDisconnected()
        {

        }

        public void OnRevancheReceived(int playerNum, bool isRevanche)
        {

        }

        public void OnServerCloseTimerStopped()
        {

        }

        public void OnMatchStopped()
        {

        }
    }
}

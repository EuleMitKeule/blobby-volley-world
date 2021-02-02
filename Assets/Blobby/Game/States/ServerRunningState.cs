using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blobby.Game.States
{
    public class ServerRunningState : IServerState
    {
        public void EnterState()
        {

        }

        public void ExitState()
        {

        }

        public void OnServerDisconnected()
        {
            Application.Quit(); //TODO restart?
        }

        public void OnPlayerJoined(string username, int elo, Color color, NetworkingPlayer networkingPlayer)
        {

        }

        public void OnAllPlayersConnected()
        {

        }

        public void OnMatchOver(Side winner)
        {
            ServerHandler.Winner = winner;

            ServerHandler.SetState(ServerHandler.OverState);
        }

        public void OnSurrenderReceived(int playerNum)
        {
            var winner = playerNum % 2 == 0 ? Side.Right : Side.Left;

            Debug.Log($"Received surrender, winner: {winner}");

            ServerConnection.SendSound(SoundHelper.SoundClip.Whistle);

            ServerHandler.Winner = winner;
            ServerHandler.SetState(ServerHandler.OverState);
        }

        public void OnPlayerDisconnected(int playerNum)
        {
            var winner = playerNum % 2 == 0 ? Side.Left : Side.Right;

            Debug.Log($"Received surrender, winner: {winner}");

            ServerHandler.Winner = winner;
            ServerHandler.SetState(ServerHandler.OverState);
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

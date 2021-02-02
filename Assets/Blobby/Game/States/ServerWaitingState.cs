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
    public class ServerWaitingState : IServerState
    {
        public void EnterState()
        {

        }

        public void ExitState()
        {

        }

        public void OnServerDisconnected()
        {
            Application.Quit();
        }

        public void OnPlayerJoined(string username, int elo, Color color, NetworkingPlayer networkingPlayer)
        {
            Debug.Log($"Player \"{username}\" joined");

            MainThreadManager.Run(() =>
            {
                ServerHandler.Match?.OnPlayerJoined(username, elo, color, networkingPlayer);

                if (ServerHandler.Match.Players.Count >= ServerHandler.Match.MatchData.PlayerCount)
                {
                    OnAllPlayersJoined();
                }
            });
        }

        public void OnAllPlayersConnected()
        {
            Debug.Log("all Players connected");

            ServerConnection.StopAccepting();
            ServerConnection.Unlist();
        }

        void OnAllPlayersJoined()
        {
            Debug.Log("all Players joined");

            ServerHandler.Match?.Start();
            ServerHandler.SetState(ServerHandler.RunningState);
        }

        public void OnMatchOver(Side winner)
        {

        }

        public void OnSurrenderReceived(int playerNum)
        {
            var winner = Side.None;

            Debug.Log($"Received surrender, winner: {winner}");

            ServerHandler.Winner = winner;
            ServerHandler.SetState(ServerHandler.OverState);
        }

        public void OnPlayerDisconnected(int playernum)
        {
            var winner = Side.None;

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

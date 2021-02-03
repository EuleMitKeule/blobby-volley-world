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
    public class ServerOverState : IServerState
    {
        public void EnterState()
        {
            ServerHandler.MatchComponent.InvokeOver(ServerHandler.Winner);

            Debug.Log($"Match is over; {ServerHandler.Winner} side won!");

            ServerConnection.StopAccepting();
            ServerConnection.Unlist();
            ServerConnection.SendOver(ServerHandler.Winner, ServerHandler.MatchComponent.ScoreLeft, ServerHandler.MatchComponent.ScoreRight, ServerHandler.MatchComponent.MatchTimer.MatchTime);

            ServerHandler.RevancheWanted = new bool[ServerHandler.ServerData.MatchData.PlayerCount];

            ServerHandler.ServerCloseTimer?.Start();
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

        public void OnPlayerDisconnected(int playerNum)
        {

        }

        public void OnAllPlayersDisconnected()
        {
            ServerHandler.ServerCloseTimer?.Stop();

            MainThreadManager.Run(() =>
            {
                ServerHandler.MatchComponent?.Dispose();
            });
        }

        public void OnRevancheReceived(int playerNum, bool isRevanche)
        {
            ServerHandler.RevancheWanted[playerNum] = isRevanche;

            if (isRevanche) Debug.Log($"Player {playerNum} wants a revanche!");
            else Debug.Log($"Player {playerNum} does not want a revanche");

            if (ServerHandler.RevancheWanted.All(val => val))
            {
                Debug.Log("Revanche!");

                ServerHandler.ServerCloseTimer?.Stop();

                ServerConnection.SendRematch();

                MainThreadManager.Run(() =>
                {
                    ServerHandler.MatchComponent?.Restart();
                });

                ServerHandler.SetState(ServerHandler.RunningState);
            }
        }

        public void OnServerCloseTimerStopped()
        {
            Debug.Log("Server close timer stopped");

            MainThreadManager.Run(() =>
            {
                ServerHandler.MatchComponent?.Dispose();
            });
        }

        public void OnMatchStopped()
        {
            ServerHandler.SetState(ServerHandler.RestartState);
        }
    }
}

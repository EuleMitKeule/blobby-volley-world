using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blobby.Models;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Blobby.Game.States
{
    public class ServerWaitingState : IServerState
    {
        public void EnterState()
        {
            Debug.Log("Waiting state");
            MainThreadManager.Run(() =>
            {
                var matchObj = Object.Instantiate(PrefabHelper.OnlineMatch);
                var matchComponent = matchObj.GetComponent<OnlineMatchComponent>();
                var matchData = ServerHandler.ServerData.MatchData;

                Time.timeScale = matchData.TimeScale;
                ServerConnection.SendMap(matchData.Map);
                
                ServerHandler.MatchComponent = matchComponent;
                ServerHandler.MatchComponent.MatchData = matchData;
                ServerHandler.SubscribeMatchEvents();
            });
        }

        public void ExitState()
        {

        }

        public void OnServerDisconnected()
        {
            Application.Quit();
        }

        public void OnPlayerJoined(PlayerData playerData)
        {
            Debug.Log($"Player \"{playerData.Name}\" joined");

            MainThreadManager.Run(() =>
            {
                ServerHandler.MatchComponent.OnPlayerJoined(playerData);
            });
        }

        public void OnAllPlayersConnected()
        {
            Debug.Log("all Players connected");

            ServerConnection.StopAccepting();
            ServerConnection.Unlist();
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

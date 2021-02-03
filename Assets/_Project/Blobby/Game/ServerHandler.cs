using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game.States;
using Blobby.Models;
using Blobby.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blobby.Game
{
    public static class ServerHandler
    {
        public static bool IsServer { get; private set; }
        public static OnlineMatchComponent MatchComponent { get; private set; }
        public static ServerData ServerData { get; private set; }
        public static bool[] RevancheWanted { get; set; }
        public static ServerCloseTimer ServerCloseTimer { get; private set; }
        public static Side Winner { get; set; }

        public static IServerState WaitingState;
        public static IServerState RunningState;
        public static IServerState OverState;
        public static IServerState RestartState;

        static IServerState _state;

        public static void SetState(IServerState state)
        {
            if (state == null) return;

            Debug.Log(state.GetType().Name);

            if (_state != null) _state.ExitState();

            _state = state;
            _state.EnterState();
        }

        public static void Initialize(ServerData serverData)
        {
            IsServer = true;

            ServerData = serverData;

            WaitingState = new ServerWaitingState();
            RunningState = new ServerRunningState();
            OverState = new ServerOverState();
            RestartState = new ServerRestartState();

            ServerCloseTimer = new ServerCloseTimer();
            ServerCloseTimer.ServerCloseTimerStopped += OnServerCloseTimerStopped;

            ServerConnection.Initialize(serverData, serverData.MatchData);

            ServerConnection.ServerStartSuccess += OnServerStartSuccess;
            ServerConnection.ServerStartFailed += OnServerStartFailed;
            ServerConnection.PlayerJoined += OnPlayerJoined;
            ServerConnection.PlayerDisconnected += OnPlayerDisconnected;
            ServerConnection.SurrenderReceived += OnSurrenderReceived;
            ServerConnection.RevancheReceived += OnRevancheReceived;
            ServerConnection.AllPlayersJoined += OnAllPlayersConnected;
            ServerConnection.AllPlayersDisconnected += OnAllPlayersDisconnected;
            ServerConnection.ServerDisconnected += OnServerDisconnected;

            Start();
        }

        public static void Start()
        {
            RevancheWanted = new bool[ServerData.MatchData.PlayerCount];
            Winner = Side.None;

            ServerConnection.Start();
        }

        static void OnServerStartSuccess()
        {
            Debug.Log("Server Start successful");

            MatchComponent = new OnlineMatchComponent(ServerData.MatchData);

            MatchComponent.Over += OnMatchOver;
            MatchComponent.MatchStopped += OnMatchStopped;

            SetState(WaitingState);

            ServerConnection.List();
        }

        static void OnServerStartFailed()
        {
            Debug.Log("Server Start failed");

            Application.Quit();
        }

        private static void OnPlayerJoined(string username, int elo, Color color, NetworkingPlayer networkingPlayer)
        {
            _state.OnPlayerJoined(username, elo, color, networkingPlayer);
        }

        static void OnAllPlayersConnected()
        {
            _state.OnAllPlayersConnected();
        }

        static void OnServerDisconnected()
        {
            _state.OnServerDisconnected();
        }

        static void OnMatchOver(Side winner, int scoreLeft, int scoreRight, int time)
        {
            _state.OnMatchOver(winner);
        }

        static void OnSurrenderReceived(int playerNum)
        {
            _state.OnSurrenderReceived(playerNum);
        }

        static void OnPlayerDisconnected(int playerNum)
        {
            _state.OnPlayerDisconnected(playerNum);
        }

        static void OnAllPlayersDisconnected()
        {
            _state.OnAllPlayersDisconnected();
        }

        static void OnRevancheReceived(int playerNum, bool isRevanche)
        {
            _state.OnRevancheReceived(playerNum, isRevanche);
        }

        static void OnServerCloseTimerStopped()
        {
            _state.OnServerCloseTimerStopped();
        }

        static void OnMatchStopped()
        {
            _state.OnMatchStopped();
        }
    }
}

﻿using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.SimpleJSON;
using Blobby.Game;
using Blobby.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace Blobby.Networking
{
    public static class ServerConnection
    {
        static UDPServer _udpServer;
        static ServerData _serverData;
        static MatchData _matchData;

        public static event Action ServerStartSuccess;
        public static event Action ServerStartFailed;
        public static event Action<string, int, Color, NetworkingPlayer> PlayerJoined;
        public static event Action AllPlayersJoined;
        public static event Action<int> PlayerDisconnected;
        public static event Action AllPlayersDisconnected;
        public static event Action<int, int> ButtonDownReceived;
        public static event Action<int, int> ButtonUpReceived;
        public static event Action<int> SurrenderReceived;
        public static event Action<int, bool> RevancheReceived;
        public static event Action ServerDisconnected;

        public const int CLIENT_HANDSHAKE = MessageGroupIds.START_OF_GENERIC_IDS + 1;
        public const int MAP = MessageGroupIds.START_OF_GENERIC_IDS + 2;
        public const int INFO = MessageGroupIds.START_OF_GENERIC_IDS + 3;
        public const int PLAYER_NUM = MessageGroupIds.START_OF_GENERIC_IDS + 4;
        public const int SOUND = MessageGroupIds.START_OF_GENERIC_IDS + 5;
        public const int TIME = MessageGroupIds.START_OF_GENERIC_IDS + 6;
        public const int ALPHA = MessageGroupIds.START_OF_GENERIC_IDS + 7;
        public const int BUTTON_DOWN = MessageGroupIds.START_OF_GENERIC_IDS + 8;
        public const int BUTTON_UP = MessageGroupIds.START_OF_GENERIC_IDS + 9;
        public const int SCORE = MessageGroupIds.START_OF_GENERIC_IDS + 10;
        public const int SURRENDER = MessageGroupIds.START_OF_GENERIC_IDS + 11;
        public const int OVER = MessageGroupIds.START_OF_GENERIC_IDS + 12;
        public const int REVANCHE = MessageGroupIds.START_OF_GENERIC_IDS + 13;
        public const int REMATCH = MessageGroupIds.START_OF_GENERIC_IDS + 14;
        public const int BALL_POSITION = MessageGroupIds.START_OF_GENERIC_IDS + 15;
        public const int PLAYER_POSITION = MessageGroupIds.START_OF_GENERIC_IDS + 16;

        //static string _masterServerHost = "89.163.134.176";
        //static ushort _masterServerPort = 15940;

        public static void Initialize(ServerData serverData, MatchData matchData)
        {
            _serverData = serverData;
            _matchData = matchData;

            NetworkManager.Instance.objectInitialized += OnObjectInitialized;
        }

        public static void Start()
        {
            Debug.Log("START   Server");

            try
            {
                _udpServer = new UDPServer(_matchData.PlayerCount + 1);

                _udpServer.bindSuccessful += OnBindSuccessful;
                _udpServer.bindFailure += OnBindFailed;
                _udpServer.playerAccepted += OnPlayerAccepted;
                _udpServer.playerRejected += OnPlayerRejected;
                _udpServer.playerDisconnected += OnPlayerDisconnected;
                _udpServer.playerTimeout += OnPlayerDisconnected;
                _udpServer.binaryMessageReceived += OnBinaryMessage;
                _udpServer.disconnected += OnServerDisconnected;

                _udpServer.Connect(_serverData.Host, _serverData.Port);

                ServerStartSuccess?.Invoke();
            }
            catch (Exception) { }
        }

        public static void Stop()
        {
            Debug.Log("STOP   Server shutting down");

            _udpServer.bindSuccessful -= OnBindSuccessful;
            _udpServer.bindFailure -= OnBindFailed;
            _udpServer.playerAccepted -= OnPlayerAccepted;
            _udpServer.playerRejected -= OnPlayerRejected;
            _udpServer.playerDisconnected -= OnPlayerDisconnected;
            _udpServer.playerTimeout -= OnPlayerDisconnected;
            _udpServer.binaryMessageReceived -= OnBinaryMessage;
            _udpServer.disconnected -= OnServerDisconnected;

            try
            {
                _udpServer.CommitDisconnects();
                _udpServer.Disconnect(false);
            }
            catch (Exception)
            {
                Application.Quit();
                throw;
            }

            ServerDisconnected?.Invoke();
        }

        public static void List()
        {
            Debug.Log("Was anderes");
            var serverId = "Blobby Volley World";
            var serverName = _serverData.Name;
            var type = _serverData.Token != "" ? "official" : "custom";
            var mode = $"{_matchData.Map} {_matchData.GameMode} {_matchData.PlayerMode} {_matchData.JumpMode} {_matchData.JumpOverNet} {_matchData.TimeScale}";
            var comment = "none";
            var useElo = false;
            var eloRequired = 0;

            var masterServerData = NetworkManager.Instance.MasterServerRegisterData(_udpServer, serverId, serverName, type, mode, comment, useElo, eloRequired);
            NetworkManager.Instance.Initialize(_udpServer, _serverData.MasterServerHost, _serverData.MasterServerPort, masterServerData);

            Debug.Log($"Listed Server on MasterServer ({_serverData.MasterServerHost}:{_serverData.MasterServerPort})");

            NetworkObject.Flush(_udpServer);
            Debug.Log(_serverData.Port);

        }

        public static void Unlist()
        {
            if (!NetworkManager.Instance.MasterServerNetworker.Disposed)
            {
                NetworkManager.Instance.MasterServerNetworker.Disconnect(false);
            }
            else Debug.Log("clever, Lennard!\nWärst du damals bloß nicht über die römische Mauer gesprungen!");
        }

        public static void StopAccepting()
        {
            _udpServer?.StopAcceptingConnections();
        }

        #region Sending

        public static void SendMap(Map map)
        {
            _udpServer.Send(Receivers.OthersBuffered, null, MAP, true, (int)map);
        }

        public static void SendInfo(NetworkingPlayer player, int playerNum, string username, Color color)
        {
            _udpServer.Send(Receivers.OthersBuffered, null, INFO, true, playerNum, username, color.r, color.g, color.b);
        }

        public static void SendSound(SoundHelper.SoundClip sound)
        {
            Debug.Log($"SendSound({sound})");

            _udpServer.Send(Receivers.Others, null, SOUND, true, (int)sound);
        }

        public static void SendAlpha(int playerNum, bool value)
        {
            _udpServer.Send(Receivers.Others, null, ALPHA, true, playerNum, value);
        }

        public static void SendPlayerNum(NetworkingPlayer player, int playerNum)
        {
            _udpServer.Send(player, PLAYER_NUM, true, playerNum);
        }

        public static void SendScore(int scoreLeft, int scoreRight, Side lastWinner)
        {
            Debug.Log($"SendScore({scoreLeft}, {scoreRight}, {lastWinner})");

            _udpServer.Send(Receivers.Others, null, SCORE, true, scoreLeft, scoreRight, (int)lastWinner);
        }

        public static void SendTime(int time)
        {
            Debug.Log($"SendTime({time})");

            _udpServer.Send(Receivers.Others, null, TIME, true, time);
        }

        public static void SendOver(Side winner, int scoreLeft, int scoreRight, int time)
        {
            _udpServer.Send(Receivers.Others, null, OVER, true, (int)winner, scoreLeft, scoreRight, time);
        }

        public static void SendRematch()
        {
            _udpServer.Send(Receivers.Others, null, REMATCH, true);
        }

        public static void SendBallPosition(Vector2 position)
        {
            _udpServer.Send(Receivers.Others, null, BALL_POSITION, true, position.x, position.y);
        }

        public static void SendPlayerPosition(Vector2 position, int playerNum)
        {
            _udpServer.Send(Receivers.Others, null, PLAYER_POSITION, true, position.x, position.y, playerNum);
        }

        #endregion

        #region NetEventHandler

        static void OnBindSuccessful(NetWorker sender)
        {
            
        }

        static void OnBindFailed(NetWorker sender)
        {
            ServerStartFailed?.Invoke();
        }

        static void OnServerDisconnected(NetWorker sender)
        {
            ServerDisconnected?.Invoke();
        }

        static void OnObjectInitialized(INetworkBehavior behavior, NetworkObject networkObject)
        {
            if (behavior is BallBehavior ballBehavior)
            {
                networkObject.UpdateInterval = 1000 / 128;
            }
            else if (behavior is PlayerBehavior playerBehavior)
            {
                networkObject.UpdateInterval = 1000 / 128;
            }
        }

        static void OnPlayerAccepted(NetworkingPlayer player, NetWorker sender)
        {
            player.PingInterval = 2000;
            player.TimeoutMilliseconds = 15000;

            if (_udpServer.Players.Count - 1 >= _serverData.MatchData.PlayerCount)
            {
                AllPlayersJoined?.Invoke();
            }
        }

        static void OnPlayerRejected(NetworkingPlayer player, NetWorker sender)
        {
            Debug.Log("rejected");
        }

        static void OnPlayerDisconnected(NetworkingPlayer player, NetWorker sender)
        {
            var playerNum = _udpServer.Players.IndexOf(player);
            PlayerDisconnected?.Invoke(playerNum);

            if (_udpServer.Players.Count == 1)
            {
                AllPlayersDisconnected?.Invoke();
            }
        }

        static void OnBinaryMessage(NetworkingPlayer networkingPlayer, Binary frame, NetWorker sender)
        {
            MainThreadManager.Run(() =>
            {
                switch (frame.GroupId)
                {
                    case CLIENT_HANDSHAKE:

                        Debug.Log("handshake received");
                        string username = frame.StreamData.GetBasicType<string>();
                        string token = frame.StreamData.GetBasicType<string>();

                        //if (token != "") GetUserData(username, token, networkingPlayer);
                        MainThreadManager.Run(() => PlayerJoined?.Invoke(username, 0, Color.gray, networkingPlayer)); //TODO: Später nicht mehr ohne token!

                        break;
                    case BUTTON_DOWN:

                        var playerNum = frame.StreamData.GetBasicType<int>();
                        var button = frame.StreamData.GetBasicType<int>();

                        ButtonDownReceived?.Invoke(playerNum, button);

                        break;

                    case BUTTON_UP:

                        playerNum = frame.StreamData.GetBasicType<int>();
                        button = frame.StreamData.GetBasicType<int>();

                        ButtonUpReceived?.Invoke(playerNum, button);

                        break;
                    case SURRENDER:

                        playerNum = _udpServer.Players.IndexOf(networkingPlayer) - 1;

                        SurrenderReceived?.Invoke(playerNum);

                        break;
                    case REVANCHE:

                        var isRevanche = frame.StreamData.GetBasicType<bool>();

                        if (!isRevanche) break;

                        playerNum = _udpServer.Players.IndexOf(networkingPlayer) - 1;

                        RevancheReceived?.Invoke(playerNum, isRevanche);

                        break;
                    default:
                        break;
                }
            });
        }

        #endregion

        #region WebRequests

        static async void GetUserData(string username, string token, NetworkingPlayer player)
        {
            var url = $"https://api.blobnet.de/api/user/{username}?token={token}";

            using var client = new HttpClient();
            var response = await client.GetAsync(url);

            if (response != null)
            {
                var json = await response.Content.ReadAsStringAsync();
                var dic = SimpleJSON.JSON.Parse(json);

                var color = new Color(dic["color_r"], dic["color_g"], dic["color_b"]);
                MainThreadManager.Run(() => PlayerJoined?.Invoke(username, dic["elo"], color, player));
            }
            else
            {
                _udpServer.BanPlayer(player.NetworkId, 1);
                return;
            }
        }

        #endregion
    }
}

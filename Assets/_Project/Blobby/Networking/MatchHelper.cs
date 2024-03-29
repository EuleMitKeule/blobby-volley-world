﻿using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.SimpleJSON;
using Blobby.Game;
using Blobby.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static BeardedManStudios.Forge.Networking.MasterServerResponse;

namespace Blobby.Networking
{
    public class MatchHelper : MonoBehaviour
    {
        public static List<Server> Servers { get; private set; } = new List<Server>();

        public static event Action<List<Server>> ServersChanged;

        static TCPMasterClient _client;

        static List<(string, ushort)> _bannedServers = new List<(string, ushort)>();

        static bool _isOffline;

        void Awake()
        {
            if (ServerHandler.IsServer) return;

            ClientConnection.Connecting += OnConnecting;
            ClientConnection.Connected += OnConnected;

            _client = new TCPMasterClient();

            _client.connectAttemptFailed += OnMasterServerFailed;
            _client.textMessageReceived += OnMasterServerResponse;

            _client.Connect("147.185.221.180", 19994);

            StartCoroutine(UpdateServers());
        }

        IEnumerator UpdateServers()
        {
            while (gameObject.activeSelf && !_isOffline)
            {
                yield return new WaitForSeconds(5);
                RequestServers();
            }
        }

        #region ServerList

        /// <summary>
        /// Send a request to the master server to get a list of all available servers
        /// </summary>
        static void RequestServers()
        {
            if (_client.Disposed || !_client.IsConnected)
            {
                Debug.Log("Master Client could not connect!");
                _isOffline = true;
                return;
            }

            Debug.Log("Master Client connected!");
            
            string gameId = "Blobby Volley World";
            string gameType = "any";
            string gameMode = "all";

            JSONNode sendData = JSONNode.Parse("{}");
            JSONClass getData = new JSONClass
            {
                { "id", gameId },
                { "type", gameType },
                { "mode", gameMode }
            };

            sendData.Add("get", getData);
            var frame = Text.CreateFromString(_client.Time.Timestep, sendData.ToString(), true, Receivers.Server, MessageGroupIds.MASTER_SERVER_GET, true);
            _client.Send(frame);
        }

        static void OnMasterServerResponse(NetworkingPlayer player, Text frame, NetWorker sender)
        {
            var data = JSONNode.Parse(frame.ToString());

            var servers = new List<Server>();

            if (data["hosts"] != null)
            {
                var response = new MasterServerResponse(data["hosts"].AsArray);

                if (response.serverResponse.Count > 0)
                {
                    for (int i = 0; i < response.serverResponse.Count; i++)
                    {
                        var server = response.serverResponse[i];
                        if (!_bannedServers.Contains((server.Address, server.Port))) servers.Add(server);
                    }
                }
            }

            if (Servers.SequenceEqual(servers)) return;
            
            Servers = servers;
            ServersChanged?.Invoke(Servers);
        }

        static void OnMasterServerFailed(NetWorker sender)
        {
            _isOffline = true;
        }

        #endregion

        /// <summary>
        /// Finds the best matching Server currently available for a ranked game
        /// </summary>
        /// <returns>Best matching Server object</returns>
        public static Server? FindBestServer()
        {
            RequestServers();
            
            Server? bestServer = null;

            foreach (var server in Servers)
            {
                if (!bestServer.HasValue) bestServer = server;
                else
                {
                    if (server.PlayerCount > bestServer.Value.PlayerCount)
                    {
                        if (!_bannedServers.Contains((server.Address, server.Port))) bestServer = server;
                    }
                }
            }

            return bestServer;
        }

        static void OnConnecting(ServerData serverData)
        {
            _bannedServers.Add((serverData.Host, serverData.Port));
        }

        static void OnConnected()
        {
            _bannedServers.RemoveAt(_bannedServers.Count - 1);
        }
    }
}

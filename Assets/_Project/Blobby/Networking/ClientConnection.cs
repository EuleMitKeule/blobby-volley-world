using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game;
using Blobby.Models;
using Blobby.UserInterface;
using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Blobby.Networking
{
    public static class ClientConnection
    {
        static UDPClient _client;
        public static UserData UserData;

        static ServerData _serverData;

        private static CancellationTokenSource _matchQueueTokenSource;
        private static CancellationTokenSource _infoQueueTokenSource;

        #region ConnectionEvents

        /// <summary>
        /// Invoked when the client starts connecting
        /// </summary>
        public static event Action<ServerData> Connecting;

        /// <summary>
        /// Invoked when the server connection started
        /// </summary>
        public static event Action Connected;

        /// <summary>
        /// Invoked when the server connection stopped
        /// </summary>
        public static event Action Disconnected;

        /// <summary>
        /// Gets invoked when the matchmaking queue is initiated
        /// </summary>
        public static event Action QueueStarted;

        /// <summary>
        /// Gets invoked when the Matchmaking queue is stopped
        /// </summary>
        public static event Action QueueStopped;

        /// <summary>
        /// Gets invoked when the matchmaking queue's time increases
        /// </summary>
        public static event Action<int> QueueTicked;

        #endregion

        #region MatchEvents

        /// <summary>
        /// Gets invoked when the client receives the map
        /// </summary>
        public static event Action<Map> MapReceived;

        /// <summary>
        /// Gets invoked when the client receives its player num
        /// </summary>
        public static event Action<int> PlayerNumReceived;

        /// <summary>
        /// Gets invoked when the client receives a player's information
        /// </summary>
        public static event Action<int, string, Color> InfoReceived;

        /// <summary>
        /// Gets invoked when the client receives the current game time
        /// </summary>
        public static event Action<int> TimeReceived;

        /// <summary>
        /// Gets invoked when the client receives the current game score
        /// </summary>
        public static event Action<int, int, Side> ScoreReceived;

        /// <summary>
        /// Gets invoked when the client receives a sound
        /// </summary>
        public static event Action<SoundHelper.SoundClip> SoundReceived;

        /// <summary>
        /// Gets invoked when the client receives an alpha value for a player
        /// </summary>
        public static event Action<int, bool> AlphaReceived;

        /// <summary>
        /// Gets invoked when the client receives game over
        /// </summary>
        public static event Action<Side, int, int, int> OverReceived;

        /// <summary>
        /// Invoked when a rematch message is received
        /// </summary>
        public static event Action RematchReceived;

        public static event Action<Vector2, int> PlayerPositionReceived;

        public static event Action<Vector2> BallPositionReceived;

        #endregion

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            if (ServerHandler.IsServer) return;

            LoginHelper.Login += OnLogin;
            LoginHelper.Logout += OnLogout;

            Start(); //TODO LÖSCHEN!
        }

        static void OnLogin(UserData userData)
        {
            UserData = userData;

            if (_client == null) Start();

            StartInfoQueue();
        }

        static void OnLogout()
        {
            UserData = null;

            StopInfoQueue();
        }

        public static void Start()
        {
            _client = new UDPClient();

            //_client.PacketLossSimulation = 0.6f;

            _client.serverAccepted += OnServerAccepted;
            _client.disconnected += OnDisconnected;
            _client.forcedDisconnect += OnDisconnected;
            _client.connectAttemptFailed += OnConnectAttemptFailed;
            _client.binaryMessageReceived += OnBinaryMessage;

            NetworkManager.Instance.Initialize(_client);
        }

        public static void Dispose()
        {
            _client.disconnected -= OnDisconnected;
            _client?.Disconnect(false);
            Disconnected?.Invoke();
        }

        public static void InitConnect(ServerData serverData)
        {
            _serverData = serverData;

            Connecting?.Invoke(serverData);
        }

        public static void Connect()
        {
            _client?.Connect(_serverData.Host, _serverData.Port);
        }

        #region Queue

        public static async void ToggleMatchQueue()
        {
            if (_matchQueueTokenSource == null || _matchQueueTokenSource.IsCancellationRequested)
            {
                _matchQueueTokenSource = new CancellationTokenSource();
                await MatchQueue();
            }
            else StopMatchQueue();
        }

        static async Task MatchQueue()
        {
            MainThreadManager.Run(() => QueueStarted?.Invoke());

            var queueTime = 0;
            MainThreadManager.Run(() => GameObject.Find("label_queue_time").GetComponent<TextMeshProUGUI>().text = "00:00");

            while (!_matchQueueTokenSource.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(1000, _matchQueueTokenSource.Token);
                }
                catch (TaskCanceledException) { break; }

                MainThreadManager.Run(() =>
                {
                    if (!_matchQueueTokenSource.IsCancellationRequested) QueueTicked?.Invoke(++queueTime);

                    var bestServer = MatchHelper.FindBestServer();

                    if (bestServer.HasValue && !_matchQueueTokenSource.IsCancellationRequested)
                    {
                        StopMatchQueue();
                        InitConnect(ParseHelper.ParseServer(bestServer.Value));
                    }
                });
            }

            MainThreadManager.Run(() => QueueStopped?.Invoke());
        }

        public static void StopMatchQueue()
        {
            if (_matchQueueTokenSource == null) return;

            _matchQueueTokenSource.Cancel();
        }

        public static async void StartInfoQueue()
        {
            if (_infoQueueTokenSource == null || _infoQueueTokenSource.IsCancellationRequested)
            {
                _infoQueueTokenSource = new CancellationTokenSource();
                await InfoQueue();
            }
        }

        static async Task InfoQueue()
        {
            while (!_infoQueueTokenSource.IsCancellationRequested)
            {
                if (UserData != null) await LoginHelper.OnlineRequest(UserData.name, UserData.token);
                if (_matchQueueTokenSource == null || !_matchQueueTokenSource.IsCancellationRequested) await LoginHelper.QueueRequest(UserData.name, UserData.token);

                await LoginHelper.PlayerRequest();
                try
                {
                    await Task.Delay(5000, _infoQueueTokenSource.Token);
                }
                catch (TaskCanceledException) { break; }
            }
        }

        static void StopInfoQueue()
        {
            _infoQueueTokenSource.Cancel();
        }

        #endregion

        #region Sending

        public static void SendButtonDown(int button, int playerNum)
        {
            _client?.Send(Receivers.Server, ServerConnection.BUTTON_DOWN, true, playerNum, button);
        }

        public static void SendButtonUp(int button, int playerNum)
        {
            _client?.Send(Receivers.Server, ServerConnection.BUTTON_UP, true, playerNum, button);
        }

        public static void SendSurrender()
        {
            _client?.Send(Receivers.Server, ServerConnection.SURRENDER, true);
        }

        public static void SendRevanche(bool revancheWanted)
        {
            _client?.Send(Receivers.Server, ServerConnection.REVANCHE, true, revancheWanted);
        }

        #endregion

        #region NetEvents

        static void OnServerAccepted(NetWorker sender)
        {
            NetworkObject.Flush(_client);

            if (UserData != null)
            {
                _client.Send(Receivers.Server, ServerConnection.CLIENT_HANDSHAKE, true, UserData.name, UserData.token);
            }
            else
            {
                _client.Send(Receivers.Server, ServerConnection.CLIENT_HANDSHAKE, true, "player", "");
            }
            
            MainThreadManager.Run(() => 
            {
                Connected?.Invoke();
            });

        }

        static void OnDisconnected(NetWorker sender)
        {
            MainThreadManager.Run(() => Disconnected?.Invoke());
        }

        static void OnConnectAttemptFailed(NetWorker sender)
        {
            MainThreadManager.Run(() => Disconnected?.Invoke());
        }

        static void OnBinaryMessage(NetworkingPlayer player, Binary frame, NetWorker sender)
        {
            MainThreadManager.Run(() =>
            {
                switch (frame.GroupId)
                {
                    case ServerConnection.MAP:

                        var map = (Map)frame.StreamData.GetBasicType<int>();

                        MapReceived?.Invoke(map);

                        break;
                    case ServerConnection.PLAYER_NUM:

                        var playerNum = frame.StreamData.GetBasicType<int>();

                        PlayerNumReceived?.Invoke(playerNum);

                        break;
                    case ServerConnection.INFO:

                        playerNum = frame.StreamData.GetBasicType<int>();
                        var userName = frame.StreamData.GetBasicType<string>();
                        var colorR = frame.StreamData.GetBasicType<float>();
                        var colorG = frame.StreamData.GetBasicType<float>();
                        var colorB = frame.StreamData.GetBasicType<float>();
                        var color = new Color(colorR, colorG, colorB);

                        InfoReceived?.Invoke(playerNum, userName, color);

                        break;
                    case ServerConnection.TIME:

                        var time = frame.StreamData.GetBasicType<int>();

                        TimeReceived?.Invoke(time);

                        break;
                    case ServerConnection.SCORE:

                        var scoreLeft = frame.StreamData.GetBasicType<int>();
                        var scoreRight = frame.StreamData.GetBasicType<int>();
                        var lastWinner = (Side)frame.StreamData.GetBasicType<int>();

                        ScoreReceived?.Invoke(scoreLeft, scoreRight, lastWinner);

                        break;
                    case ServerConnection.SOUND:

                        var sound = (SoundHelper.SoundClip)frame.StreamData.GetBasicType<int>();

                        SoundReceived?.Invoke(sound);

                        break;
                    case ServerConnection.ALPHA:

                        playerNum = frame.StreamData.GetBasicType<int>();
                        var value = frame.StreamData.GetBasicType<bool>();

                        AlphaReceived?.Invoke(playerNum, value);

                        break;
                    case ServerConnection.OVER:

                        var winner = (Side)frame.StreamData.GetBasicType<int>();
                        scoreLeft = frame.StreamData.GetBasicType<int>();
                        scoreRight = frame.StreamData.GetBasicType<int>();
                        time = frame.StreamData.GetBasicType<int>();

                        OverReceived?.Invoke(winner, scoreLeft, scoreRight, time);

                        break;
                    case ServerConnection.REMATCH:

                        RematchReceived?.Invoke();

                        break;
                    case ServerConnection.PLAYER_POSITION:

                        var posX = frame.StreamData.GetBasicType<float>();
                        var posY = frame.StreamData.GetBasicType<float>();
                        playerNum = frame.StreamData.GetBasicType<int>();

                        PlayerPositionReceived?.Invoke(new Vector2(posX, posY), playerNum);

                        break;
                    case ServerConnection.BALL_POSITION:

                        posX = frame.StreamData.GetBasicType<float>();
                        posY = frame.StreamData.GetBasicType<float>();

                        BallPositionReceived?.Invoke(new Vector2(posX, posY));

                        break;
                    default:
                        break;
                }
            });
        }

        #endregion
    }
}

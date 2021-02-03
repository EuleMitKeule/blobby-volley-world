using Blobby.UserInterface.Components;
using Blobby.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;
using Blobby.Models;
using Blobby.UserInterface;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using Blobby.Game.Entities;
using System.Linq;

namespace Blobby.Game
{
    public class ClientMatch : IClientMatch, IDisposable
    {
        public int OwnPlayerNum { get; private set; }
        public Side OwnSide { get { return OwnPlayerNum % 2 == 0 ? Side.Left : Side.Right; } }
        public bool Switched { get { return OwnSide != PanelSettings.SettingsData.Side; } }

        public List<PlayerData> PlayerDataList { get; private set; } = new List<PlayerData>();

        public List<ClientPlayer> Players { get; private set; } = new List<ClientPlayer>();
        public ClientBall Ball { get; private set; }

        public event Action MatchStarted;
        public event Action<int> TimeChanged;
        public event Action<int, int, Side> ScoreChanged;
        public event Action MatchStopped;

        MatchData _matchData;
        MatchScore _matchScore;

        public ClientMatch(ServerData serverData)
        {
            SubscribeEventHandler();

            _matchData = serverData.MatchData;
        }

        void OnObjectInitialized(INetworkBehavior behavior, NetworkObject networkObject)
        {
            MainThreadManager.Run(() =>
            {
                networkObject.UpdateInterval = 1000 / 128;

                if (behavior is BallBehavior ballBehavior)
                {
                    Ball = new ClientBall(this, _matchData, ballBehavior.gameObject);
                    MatchStarted?.Invoke();
                }
                else if (behavior is PlayerBehavior playerBehavior)
                {
                    var newPlayerNum = Players.Count;
                    var playerData = PlayerDataList[newPlayerNum];
                    var clientPlayer = new ClientPlayer(this, _matchData, playerBehavior.gameObject, playerData);

                    Players.Add(clientPlayer);

                    foreach (var player in Players) player.SetSwitched(Switched);
                }
            });
        }

        #region NetEvents

        void OnMapReceived(Map map)
        {
            MapHelper.ChangeMap(map);
            _matchScore = new MatchScore(this, null, null);
        }

        void OnInfoReceived(int playerNum, string username, Color color)
        {
            var playerData = new PlayerData(playerNum, username, color);
            PlayerDataList.Add(playerData);

            if (playerNum == 0) _matchScore.SetLeftPlayerData(playerData);
            else if (playerNum == 1) _matchScore.SetRightPlayerData(playerData);
        }

        void OnScoreReceived(int scoreLeft, int scoreRight, Side lastWinner)
        {
            ScoreChanged?.Invoke(scoreLeft, scoreRight, lastWinner);
        }

        void OnTimeReceived(int time)
        {
            TimeChanged?.Invoke(time);
        }

        void OnPlayerNumReceived(int playerNum)
        {
            OwnPlayerNum = playerNum;

            InputHelper.SetDownCallback(() => ClientConnection.SendButtonDown(0, OwnPlayerNum), 0, 0);
            InputHelper.SetDownCallback(() => ClientConnection.SendButtonDown(1, OwnPlayerNum), 0, Switched ? 2 : 1);
            InputHelper.SetDownCallback(() => ClientConnection.SendButtonDown(2, OwnPlayerNum), 0, Switched ? 1 : 2);

            InputHelper.SetUpCallback(() => ClientConnection.SendButtonUp(0, OwnPlayerNum), 0, 0);
            InputHelper.SetUpCallback(() => ClientConnection.SendButtonUp(1, OwnPlayerNum), 0, Switched ? 2 : 1);
            InputHelper.SetUpCallback(() => ClientConnection.SendButtonUp(2, OwnPlayerNum), 0, Switched ? 1 : 2);
        }

        #endregion

        void SubscribeEventHandler()
        {
            NetworkManager.Instance.objectInitialized += OnObjectInitialized;
            ClientConnection.InfoReceived += OnInfoReceived;
            ClientConnection.PlayerNumReceived += OnPlayerNumReceived;
            ClientConnection.MapReceived += OnMapReceived;
            ClientConnection.TimeReceived += OnTimeReceived;
            ClientConnection.ScoreReceived += OnScoreReceived;
        }

        public void Dispose()
        {
            Ball?.Dispose();
            foreach (var player in Players) player?.Dispose();

            MatchStopped?.Invoke();

            InputHelper.SetDownCallback(null, OwnPlayerNum, 0);
            InputHelper.SetDownCallback(null, OwnPlayerNum, 1);
            InputHelper.SetDownCallback(null, OwnPlayerNum, 2);

            InputHelper.SetUpCallback(null, OwnPlayerNum, 0);
            InputHelper.SetUpCallback(null, OwnPlayerNum, 1);
            InputHelper.SetUpCallback(null, OwnPlayerNum, 2);

            NetworkManager.Instance.objectInitialized -= OnObjectInitialized;
            ClientConnection.InfoReceived -= OnInfoReceived;
            ClientConnection.PlayerNumReceived -= OnPlayerNumReceived;
            ClientConnection.MapReceived -= OnMapReceived;
            ClientConnection.TimeReceived -= OnTimeReceived;
            ClientConnection.ScoreReceived -= OnScoreReceived;
        }
    }
}

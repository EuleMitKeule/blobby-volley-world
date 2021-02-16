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
using Object = UnityEngine.Object;

namespace Blobby.Game
{
    public class ClientMatchComponent : MonoBehaviour, IClientMatch
    {
        public int NetPlayerNum { get; private set; }
        public Side OwnSide { get { return NetPlayerNum % 2 == 0 ? Side.Left : Side.Right; } }
        public bool Switched { get { return OwnSide != PanelSettings.SettingsData.Side; } }

        public List<PlayerData> PlayerDataList { get; private set; } = new List<PlayerData>();

        public List<GameObject> Players { get; private set; } = new List<GameObject>();
        public GameObject Ball { get; private set; }

        public event Action MatchStarted;
        public event Action<int> TimeChanged;
        public event Action<int, int, Side> ScoreChanged;
        public event Action MatchStopped;

        public MatchData MatchData { get; set; }
        MatchScore _matchScore;

        void Awake()
        {
            SubscribeEventHandler();
        }

        void OnObjectInitialized(INetworkBehavior behavior, NetworkObject networkObject)
        {
            MainThreadManager.Run(() =>
            {
                networkObject.UpdateInterval = 1000 / 128;

                if (behavior is BallBehavior ballBehavior)
                {
                    Ball = ballBehavior.gameObject;
                    Ball.transform.SetParent(transform);
                    Ball.GetComponent<BallNetworkComponent>().ClientMatchComponent = this;
                    
                    MatchStarted?.Invoke();
                }
                else if (behavior is PlayerBehavior playerBehavior)
                {
                    var clientPlayer = playerBehavior.gameObject;
                    var playerNetworkComponent = clientPlayer.GetComponent<PlayerNetworkComponent>();
                        
                    clientPlayer.transform.SetParent(transform);
                    playerNetworkComponent.ClientMatchComponent = this;
                    playerNetworkComponent.PlayerData = PlayerDataList[Players.Count];

                    Players.Add(clientPlayer);
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
            NetPlayerNum = playerNum;

            InputHelper.SetDownCallback(() => ClientConnection.SendButtonDown(0, NetPlayerNum), 0, 0);
            InputHelper.SetDownCallback(() => ClientConnection.SendButtonDown(1, NetPlayerNum), 0, Switched ? 2 : 1);
            InputHelper.SetDownCallback(() => ClientConnection.SendButtonDown(2, NetPlayerNum), 0, Switched ? 1 : 2);

            InputHelper.SetUpCallback(() => ClientConnection.SendButtonUp(0, NetPlayerNum), 0, 0);
            InputHelper.SetUpCallback(() => ClientConnection.SendButtonUp(1, NetPlayerNum), 0, Switched ? 2 : 1);
            InputHelper.SetUpCallback(() => ClientConnection.SendButtonUp(2, NetPlayerNum), 0, Switched ? 1 : 2);
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

        public void OnDestroy()
        {
            if (Ball) Object.Destroy(Ball);
            foreach (var player in Players.Where(player => player)) Object.Destroy(player);

            MatchStopped?.Invoke();

            InputHelper.SetDownCallback(null, NetPlayerNum, 0);
            InputHelper.SetDownCallback(null, NetPlayerNum, 1);
            InputHelper.SetDownCallback(null, NetPlayerNum, 2);

            InputHelper.SetUpCallback(null, NetPlayerNum, 0);
            InputHelper.SetUpCallback(null, NetPlayerNum, 1);
            InputHelper.SetUpCallback(null, NetPlayerNum, 2);

            NetworkManager.Instance.objectInitialized -= OnObjectInitialized;
            ClientConnection.InfoReceived -= OnInfoReceived;
            ClientConnection.PlayerNumReceived -= OnPlayerNumReceived;
            ClientConnection.MapReceived -= OnMapReceived;
            ClientConnection.TimeReceived -= OnTimeReceived;
            ClientConnection.ScoreReceived -= OnScoreReceived;
        }
    }
}

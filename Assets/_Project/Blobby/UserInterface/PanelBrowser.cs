using Blobby.UserInterface.Components;
using Blobby.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BeardedManStudios.Forge.Networking.MasterServerResponse;
using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Models;
using Blobby.Components;
using Blobby.Game;
using Blobby.Game.States;

namespace Blobby.UserInterface
{
    public static class PanelBrowser
    {
        public static ServerData SelectedServerData { get; private set; }

        public static Sprite[] GameModeIcons = { Resources.Load<Sprite>("Graphics/UI/icons/gamemode/icon_standard"),
                                                       Resources.Load<Sprite>("Graphics/UI/icons/gamemode/icon_bomb"),
                                                       Resources.Load<Sprite>("Graphics/UI/icons/gamemode/icon_tennis"),
                                                       Resources.Load<Sprite>("Graphics/UI/icons/gamemode/icon_blitz")};

        public static Sprite[] PlayerModeIcons = { Resources.Load<Sprite>("Graphics/UI/icons/playermode/icon_single"),
                                                         Resources.Load<Sprite>("Graphics/UI/icons/playermode/icon_double"),
                                                         Resources.Load<Sprite>("Graphics/UI/icons/playermode/icon_double_fixed"),
                                                         Resources.Load<Sprite>("Graphics/UI/icons/playermode/icon_single_ghost")};

        public static Sprite[] JumpModeIcons = { Resources.Load<Sprite>("Graphics/UI/icons/jumpmode/icon_standard"),
                                                       Resources.Load<Sprite>("Graphics/UI/icons/jumpmode/icon_nojump"),
                                                       Resources.Load<Sprite>("Graphics/UI/icons/jumpmode/icon_pogo"),
                                                       Resources.Load<Sprite>("Graphics/UI/icons/jumpmode/icon_spring")};

        public static Sprite JumpOverNetIcon = Resources.Load<Sprite>("Graphics/UI/icons/jumpmode/icon_jumpovernet");


        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            if (ServerHandler.IsServer) return;

            MatchHelper.ServersChanged += OnServersChanged;

            ButtonBrowser.Clicked += OnButtonBrowser;
            ButtonBrowserConnect.Clicked += OnButtonBrowserConnect;
        }

        static void OnServersChanged(List<Server> servers)
        {
            MainThreadManager.Run(() =>
            {
                var serverDatas = ParseHelper.ParseServerList(servers);

                PopulateBrowser(serverDatas);

                // Show/hide the "no servers / offline" label
                UpdateOfflineIndicator(serverDatas.Count == 0);
            });
        }

        /// <summary>
        /// Shows or hides the offline/no-servers label in the browser panel.
        /// </summary>
        static void UpdateOfflineIndicator(bool noServers)
        {
            var panelBrowser = GameObject.Find("panel_browser");
            if (panelBrowser == null) return;

            var offlineLabel = panelBrowser.transform.Find("label_no_servers");
            if (offlineLabel == null) return;

            offlineLabel.gameObject.SetActive(noServers);

            if (noServers)
            {
                var text = offlineLabel.GetComponent<TextMeshProUGUI>();
                if (text != null)
                    text.text = OnlineStatusHelper.CurrentStatus == OnlineStatusHelper.Status.Offline
                        ? "Servers are currently offline"
                        : "No servers available";
            }
        }

        static void OnButtonBrowser()
        {
            //UnpopulateBrowser();
            ClientConnection.StopMatchQueue();
        }

        static void OnButtonBrowserConnect()
        {
            if (SelectedServerData != null)
            {
                ClientConnection.InitConnect(SelectedServerData);
                SelectedServerData = null;
            }
        }

        static void OnButtonServer(GameObject button, ServerData serverData)
        {
            //unselect all servers
            GameObject panelServers = GameObject.Find("panel_servers");

            for (int i = 0; i < panelServers.transform.childCount; i++)
                panelServers.transform.GetChild(i).GetComponent<AnimTool>().SetSelected(false);

            button.GetComponent<AnimTool>().SetSelected(true);

            SelectedServerData = serverData;
        }

        #region Populators

        static void PopulateBrowser(List<ServerData> serverList)
        {
            UnpopulateBrowser();

            var panelServer = GameObject.Find("panel_servers");

            foreach (var server in serverList)
            {
                if (server.IsRanked) continue;
                var prefab = Resources.Load<GameObject>("Prefabs/UI/button_server");
                var serverButton = GameObject.Instantiate(prefab, panelServer.transform);
                serverButton.GetComponent<Button>().onClick.AddListener(() => OnButtonServer(serverButton, server));

                PopulateServerButton(serverButton, server);
            }
        }

        static void UnpopulateBrowser()
        {
            var panelServers = GameObject.Find("panel_servers");

            for (int i = 0; i < panelServers.transform.childCount; i++)
                GameObject.Destroy(panelServers.transform.GetChild(i).gameObject);
        }

        static void PopulateServerButton(GameObject button, ServerData serverData)
        {
            TextMeshProUGUI[] labels = button.transform.GetComponentsInChildren<TextMeshProUGUI>();
            Image[] icons = button.transform.GetComponentsInChildren<Image>();

            labels[0].text = $"{serverData.Name}";
            labels[1].text = $"{serverData.MatchData.TimeScale * 100}%";
            labels[2].text = $"{serverData.PlayerCount - 1} / {serverData.MaxPlayerCount}";

            icons[1].sprite = MapHelper.Icons[(int)serverData.MatchData.Map];
            icons[2].sprite = GameModeIcons[(int)serverData.MatchData.GameMode];
            icons[3].sprite = PlayerModeIcons[(int)serverData.MatchData.PlayerMode];
            icons[4].sprite = JumpModeIcons[(int)serverData.MatchData.JumpMode];
            icons[5].sprite = serverData.MatchData.JumpOverNet ? JumpOverNetIcon : null;
        }

        #endregion
    }
}

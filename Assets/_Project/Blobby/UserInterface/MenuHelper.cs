using Blobby.UserInterface.Components;
using Blobby.Game;
using Blobby.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeardedManStudios.Forge.Networking.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Blobby.UserInterface
{
    public static class MenuHelper
    {
        static GameObject _indicatorOffline;

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            if (ServerHandler.IsServer) return;

            SetColor(Color.grey);
            SetPanelMenu(true);

            //TimeHelper.ConnectTimerStopped += (string ip, ushort port) => OnBlackoutTimerStopped();

            ButtonBrowser.Clicked += OnButtonBrowser;
            ButtonLocal.Clicked += OnButtonLocal;
            ButtonSettings.Clicked += OnButtonSettings;
            ButtonQuit.Clicked += OnButtonQuit;

            CreateOfflineIndicator();

            // Subscribe to online status changes for the offline indicator
            OnlineStatusHelper.StatusChanged += OnOnlineStatusChanged;
            // Apply initial status (may be set before MenuHelper initializes)
            OnOnlineStatusChanged(OnlineStatusHelper.CurrentStatus);
        }

        static void CreateOfflineIndicator()
        {
            var mainMenuPanel = GameObject.Find("panel_mainmenu");
            if (mainMenuPanel == null) return;

            var canvas = mainMenuPanel.GetComponentInParent<Canvas>();
            if (canvas == null) return;

            _indicatorOffline = new GameObject("indicator_offline");
            _indicatorOffline.transform.SetParent(canvas.transform, false);
            _indicatorOffline.SetActive(false);

            var rect = _indicatorOffline.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-20, -20);
            rect.sizeDelta = new Vector2(64, 64);

            var text = _indicatorOffline.AddComponent<TextMeshProUGUI>();
            text.text = "X";
            text.color = new Color(0.9f, 0.1f, 0.1f, 1f);
            text.fontSize = 48;
            text.alignment = TextAlignmentOptions.Center;
            text.fontStyle = FontStyles.Bold;
        }

        static void OnOnlineStatusChanged(OnlineStatusHelper.Status status)
        {
            bool isOffline = status == OnlineStatusHelper.Status.Offline;

            if (_indicatorOffline != null)
                _indicatorOffline.SetActive(isOffline);

            // Disable ranked button when servers are offline
            var rankedBtn = GameObject.Find("button_ranked");
            if (rankedBtn != null)
            {
                var btn = rankedBtn.GetComponent<Button>();
                if (btn != null)
                    btn.interactable = !isOffline;
            }
        }

        public static void SetColor(Color color)
        {
            bool colored = color != Color.grey;

            GameObject[] colorables = GameObject.FindGameObjectsWithTag("colorable");
            foreach (GameObject colorable in colorables) colorable.GetComponent<IColorable>().Colored = colored;

            GameObject[] panels = GameObject.FindGameObjectsWithTag("colored");

            foreach (GameObject panel in panels)
                panel.GetComponent<Image>().color = new Color(color.r, color.g, color.b);

            GameObject.Find("menu_blob").GetComponent<SpriteRenderer>().color = color;
        }

        static void OnButtonBrowser()
        {
            SetPanelLocal(false);
            SetPanelBrowser(true);
            SetPanelSettings(false);
        }

        static void OnButtonLocal()
        {
            SetPanelBrowser(false);
            SetPanelLocal(true);
            SetPanelSettings(false);
        }

        static void OnButtonSettings()
        {
            SetPanelLocal(false);
            SetPanelBrowser(false);
            SetPanelSettings(true);
        }

        static void OnButtonQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        #region UI Toggles

        public static void SetPanelPause(bool visible)
        {
            var canvasGroup = GameObject.Find("canvas_pause").GetComponent<CanvasGroup>();

            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;

            InputHelper.CursorVisible = visible;
        }

        public static void TogglePanelPause()
        {
            var canvasGroup = GameObject.Find("canvas_pause").GetComponent<CanvasGroup>();
            var visible = canvasGroup.alpha == 1;

            SetPanelPause(!visible);
        }

        public static void SetPanelMainMenu(bool visible)
        {
            var canvasGroup = GameObject.Find("panel_mainmenu").GetComponent<CanvasGroup>();

            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        public static void SetPanelMenu(bool visible)
        {
            var canvasGroup = GameObject.Find("panel_menu").GetComponent<CanvasGroup>();

            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        public static void SetPanelLocal(bool visible)
        {
            var canvasGroup = GameObject.Find("panel_local").GetComponent<CanvasGroup>();

            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        public static void SetPanelBrowser(bool visible)
        {
            var canvasGroup = GameObject.Find("panel_browser").GetComponent<CanvasGroup>();

            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        public static void SetPanelSettings(bool visible)
        {
            var canvasGroup = GameObject.Find("panel_settings").GetComponent<CanvasGroup>();

            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        #endregion
    }
}

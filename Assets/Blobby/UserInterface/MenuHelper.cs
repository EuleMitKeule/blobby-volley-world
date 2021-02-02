using Blobby.UserInterface.Components;
using Blobby.Game;
using Blobby.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Blobby.UserInterface
{
    public static class MenuHelper
    {
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            if (ServerHandler.IsServer) return;

            SetColor(Color.grey);
            SetPanelMenu(true);

            LoginHelper.Login += OnLogin;
            LoginHelper.Logout += OnLogout;
            //TimeHelper.ConnectTimerStopped += (string ip, ushort port) => OnBlackoutTimerStopped();

            ButtonBrowser.Clicked += OnButtonBrowser;
            ButtonLocal.Clicked += OnButtonLocal;
            ButtonSettings.Clicked += OnButtonSettings;
            ButtonQuit.Clicked += OnButtonQuit;
        }

        static void SetColor(Color color)
        {
            bool colored = color != Color.grey;

            GameObject[] colorables = GameObject.FindGameObjectsWithTag("colorable");
            foreach (GameObject colorable in colorables) colorable.GetComponent<IColorable>().Colored = colored;

            GameObject[] panels = GameObject.FindGameObjectsWithTag("colored");

            foreach (GameObject panel in panels)
                panel.GetComponent<Image>().color = new Color(color.r, color.g, color.b);

            GameObject.Find("menu_blob").GetComponent<SpriteRenderer>().color = color;
        }

        static void OnLogin(UserData userData)
        {
            var sprite = Resources.Load<Sprite>("Graphics/UI/canvas_menu/panel_menu/button_globe/button_globe_online");
            GameObject.Find("button_globe").GetComponent<Image>().sprite = sprite;
            GameObject.Find("button_globe").GetComponent<Animator>().SetBool("offline", false);

            GameObject.Find("button_ranked").GetComponentInChildren<Button>().interactable = true;
            GameObject.Find("button_ranked").GetComponentInChildren<EventTrigger>().enabled = true;
            GameObject.Find("button_browser").GetComponentInChildren<Button>().interactable = true;
            GameObject.Find("button_browser").GetComponentInChildren<EventTrigger>().enabled = true;

            var color = new Color(userData.colorR, userData.colorG, userData.colorB);
            SetColor(color);
        }

        static void OnLogout()
        {
            GameObject.Find("button_ranked").GetComponentInChildren<Button>().interactable = false;
            GameObject.Find("button_ranked").GetComponentInChildren<EventTrigger>().enabled = false;
            GameObject.Find("button_browser").GetComponentInChildren<Button>().interactable = false;
            GameObject.Find("button_browser").GetComponentInChildren<EventTrigger>().enabled = false;

            SetColor(Color.grey);

            var sprite = Resources.Load<Sprite>("Graphics/UI/canvas_menu/panel_menu/button_globe/button_globe_offline");
            GameObject.Find("button_globe").GetComponent<Image>().sprite = sprite;
            GameObject.Find("button_globe").GetComponent<Animator>().SetBool("offline", true);
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

using BeardedManStudios.Forge.Networking.Unity;
using Blobby.UserInterface.Components;
using Blobby.Game;
using Blobby.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Blobby.UserInterface
{
    public static class MapHelper
    {
        public static TextMeshProUGUI LabelTime { get; private set; }
        public static GameObject PanelScoreLeft { get; private set; }
        public static GameObject PanelScoreRight { get; private set; }
        public static GameObject LightScoreLeft { get; private set; }
        public static GameObject LightScoreRight { get; private set; }
        public static TextMeshProUGUI LabelScoreLeft { get; private set; }
        public static TextMeshProUGUI LabelScoreRight { get; private set; }
        public static TextMeshProUGUI ExclamationLeft { get; private set; }
        public static TextMeshProUGUI ExclamationRight { get; private set; }

        public static Sprite IconGym = Resources.Load<Sprite>("Graphics/UI/general/map_placeholder");
        public static Sprite IconBeach = Resources.Load<Sprite>("Graphics/UI/general/map_placeholder");
        public static Sprite IconMoon = Resources.Load<Sprite>("Graphics/UI/general/map_placeholder");

        public static Sprite[] Icons = { IconGym, IconBeach, IconMoon };

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            if (ServerHandler.IsServer) return;

            ChangeMap(Map.Menu);
        }

        public static void ChangeMap(Map map)
        {
            var parentMap = GameObject.Find("parent_map");
            var canvasMap = GameObject.Find("canvas_map").GetComponent<Canvas>();

            //disable all map objects
            for (int i = 0; i < parentMap.transform.childCount; i++)
                parentMap.transform.GetChild(i).gameObject.SetActive(false);

            //disable all map canvasses
            for (int i = 0; i < canvasMap.transform.childCount; i++)
                SetPanel(canvasMap.transform.GetChild(i).gameObject, false);

            var wantedMap = parentMap.transform.GetChild((int)map).gameObject;
            var wantedMapCanvas = canvasMap.transform.GetChild((int)map).gameObject;

            //enable wanted map object
            wantedMap.SetActive(true);

            var labelTimeObj = wantedMapCanvas.transform.Find("label_time");
            var labelScoreLeftObj = wantedMapCanvas.transform.Find("label_score_left");
            var labelScoreRightObj = wantedMapCanvas.transform.Find("label_score_right");
            var exclamationLeftObj = wantedMapCanvas.transform.Find("label_exclamation_left");
            var exclamationRightObj = wantedMapCanvas.transform.Find("label_exclamation_right");

            if (labelTimeObj) LabelTime = labelTimeObj.GetComponent<TextMeshProUGUI>();
            if (labelScoreLeftObj) LabelScoreLeft = labelScoreLeftObj.GetComponent<TextMeshProUGUI>();
            if (labelScoreRightObj) LabelScoreRight = labelScoreRightObj.GetComponent<TextMeshProUGUI>();
            if (exclamationLeftObj) ExclamationLeft = exclamationLeftObj.GetComponent<TextMeshProUGUI>();
            if (exclamationRightObj) ExclamationRight = exclamationRightObj.GetComponent<TextMeshProUGUI>();

            var panelScoreLeftObj = wantedMap.transform.Find("panel_score_left");
            var panelScoreRightObj = wantedMap.transform.Find("panel_score_right");
            var lightScoreLeftObj = wantedMap.transform.Find("light_score_left");
            var lightScoreRightObj = wantedMap.transform.Find("light_score_right");

            if (panelScoreLeftObj) PanelScoreLeft = panelScoreLeftObj.gameObject;
            if (panelScoreRightObj) PanelScoreRight = panelScoreRightObj.gameObject;
            if (lightScoreLeftObj) LightScoreLeft = lightScoreLeftObj.gameObject;
            if (lightScoreRightObj) LightScoreRight = lightScoreRightObj.gameObject;

            //enable wanted map canvas
            SetPanel(canvasMap.transform.GetChild((int)map).gameObject, true);
        }

        static void SetPanel(GameObject panel, bool visible)
        {
            var canvasGroup = panel.GetComponent<CanvasGroup>();

            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = panel.gameObject.name == "panel_menu" ? visible : false;
        }
    }
}

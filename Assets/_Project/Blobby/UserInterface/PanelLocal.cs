using Blobby.UserInterface.Components;
using Blobby.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blobby.UserInterface
{
    public static class PanelLocal
    {
        static int _mapIndex;

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            if (ServerHandler.IsServer) return;

            SliderSpeed.Changed += OnSliderLocalSpeed;
            ButtonLocal.Clicked += OnButtonLocal;
            ButtonPlayerModeSingle.Clicked += () => OnButtonPlayerMode(0);
            ButtonPlayerModeDouble.Clicked += () => OnButtonPlayerMode(1);
            ButtonPlayerModeDoubleFixed.Clicked += () => OnButtonPlayerMode(2);
            ButtonPlayerModeGhost.Clicked += () => OnButtonPlayerMode(3);
            ButtonGameModeStandard.Clicked += () => OnButtonGameMode(0);
            ButtonGameModeBomb.Clicked += () => OnButtonGameMode(1);
            ButtonGameModeTennis.Clicked += () => OnButtonGameMode(2);
            ButtonGameModeBlitz.Clicked += () => OnButtonGameMode(3);
            ButtonJumpModeStandard.Clicked += () => OnButtonJumpMode(0);
            ButtonJumpModeNoJump.Clicked += () => OnButtonJumpMode(1);
            ButtonJumpModePogo.Clicked += () => OnButtonJumpMode(2);
            ButtonJumpModeSpring.Clicked += () => OnButtonJumpMode(3);
            ButtonMapLeft.Clicked += () => OnButtonMap(-1);
            ButtonMapRight.Clicked += () => OnButtonMap(1);
        }

        static void OnButtonLocal()
        {
            var imageMap = GameObject.Find("image_map");
            var labelMap = GameObject.Find("label_map");
            imageMap.GetComponent<Image>().sprite = MapHelper.Icons[_mapIndex];
            labelMap.GetComponent<TextMeshProUGUI>().text = ((Map)_mapIndex).ToString();
        }

        static void OnButtonPlayerMode(int playerMode)
        {
            GameObject labelPlayerMode = GameObject.Find("label_playermode").transform.GetChild(0).gameObject;
            labelPlayerMode.GetComponent<TextMeshProUGUI>().text = ((PlayerMode)playerMode).ToString();
        }

        static void OnButtonGameMode(int gameMode)
        {
            GameObject labelGameMode = GameObject.Find("label_gamemode").transform.GetChild(0).gameObject;
            labelGameMode.GetComponent<TextMeshProUGUI>().text = ((GameMode)gameMode).ToString();
        }

        static void OnButtonJumpMode(int jumpMode)
        {
            GameObject labelJumpMode = GameObject.Find("label_jumpmode").transform.GetChild(0).gameObject;
            labelJumpMode.GetComponent<TextMeshProUGUI>().text = ((JumpMode)jumpMode).ToString();
        }

        static void OnButtonMap(int change)
        {
            _mapIndex = MathHelper.Mod(_mapIndex + change, Enum.GetNames(typeof(Map)).Length - 1);
            var imageMap = GameObject.Find("image_map");
            var labelMap = GameObject.Find("label_map");
            imageMap.GetComponent<Image>().sprite = MapHelper.Icons[_mapIndex];
            labelMap.GetComponent<TextMeshProUGUI>().text = ((Map)_mapIndex).ToString();
        }

        static void OnSliderLocalSpeed(int value)
        {
            GameObject labelSpeed = GameObject.Find("label_slider_speed");
            labelSpeed.GetComponent<TextMeshProUGUI>().text = $"{value}%";
        }
    }
}

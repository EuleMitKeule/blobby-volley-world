using Blobby.Components;
using Blobby.Game;
using Blobby.Models;
using Blobby.Networking;
using Blobby.UserInterface.Components;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Blobby.UserInterface
{
    public static class PanelSettings
    {
        public static SettingsData SettingsData { get; private set; }

        public static int SelectedPlayerNum { get; private set; }
        static Control _curControl;

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            if (ServerHandler.IsServer) return;

            SubscribeEventHandler();

            SettingsData = IoHelper.LoadSettingsData();
            SettingsData ??= new SettingsData();

            var sliderHue = GameObject.Find("slider_hue");
            var sliderHueImage = sliderHue.GetComponentInChildren<Image>();

            var colors = new Color[]
            {
                new Color(1, 0, 0),
                new Color(1, 1, 0),
                new Color(0, 1, 0),
                new Color(0, 1, 1),
                new Color(0, 0, 1),
                new Color(1, 0, 1),
                new Color(1, 0, 0)
            };
            var hueTex = new Texture2D(colors.Length, 1);
            hueTex.SetPixels(colors);
            hueTex.Apply();

            var hueSprite = Sprite.Create(hueTex, new Rect(Vector2.zero, new Vector2(colors.Length, 1)), Vector2.one * 0.5f);
            hueSprite.texture.wrapMode = TextureWrapMode.Clamp;
            hueSprite.texture.Apply();

            sliderHueImage.sprite = hueSprite;
            sliderHueImage.preserveAspect = false;
            
            //BETA
            PopulateSettings();
            MenuHelper.SetColor(SettingsData.Colors[0]);
        }

        static void OnButtonSettingsSave()
        {
            IoHelper.SaveSettingsData(SettingsData);
            if (ClientConnection.UserData != null)
            {
                if (ClientConnection.UserData.Color != SettingsData.Colors[0])
                {
                    ClientConnection.UserData.colorR = SettingsData.Colors[0].r;
                    ClientConnection.UserData.colorG = SettingsData.Colors[0].g;
                    ClientConnection.UserData.colorB = SettingsData.Colors[0].b;

                    Task.Run(() => LoginHelper.PostColor(ClientConnection.UserData));
                }
            }

            PopulateSettings();
            
            MenuHelper.SetColor(SettingsData.Colors[0]); //BETA
        }

        static void OnButtonSettings()
        {
            PopulateSettings();
        }

        static void OnButtonSettingsBack()
        {
            SettingsData = IoHelper.LoadSettingsData();
            SettingsData ??= new SettingsData();
            PopulateSettings();
        }

        static void OnButtonSideLeft()
        {
            SettingsData.Side = Side.Left;
            PopulateSettings();
        }

        static void OnButtonSideRight()
        {
            SettingsData.Side = Side.Right;
            PopulateSettings();
        }

        static void OnButtonControl(Control control)
        {
            UnsubscribeEventHandler();

            _curControl = control;

            TimeComponent.UpdateTicked += Update;
        }

        static void OnKeyInput(KeyCode keyCode)
        {
            SettingsData.Controls[SelectedPlayerNum].SetControl(_curControl, keyCode);
            SubscribeEventHandler();
            PopulateSettings();
        }

        static void OnButtonPlayer()
        {
            SelectedPlayerNum = (SelectedPlayerNum + 1) % 4;
            PopulateSettings();
        }

        static void OnSliderHueChanged(float value)
        {
            var color = Color.HSVToRGB(value, 0.9f, 0.9f);
            SettingsData.Colors[SelectedPlayerNum] = color;
            PopulateSettings();
        }

        static void OnToggleWindowed()
        {
            SettingsData.Windowed = !SettingsData.Windowed;
            PopulateSettings();
        }

        static void OnSliderVolumeChanged(float value)
        {
            SettingsData.Volume = value;
            PopulateSettings();
        }

        static void OnLogin(UserData userData)
        {
            if (ClientConnection.UserData.Color != SettingsData.Colors[0])
            {
                SettingsData.Colors[0] = userData.Color;
            }
        }

        static void OnInputUsernameChanged(string username) //BETA
        {
            SettingsData.Username = username;
        }

        static void OnInputUsernameEndEdit() //BETA
        {
            IoHelper.SaveSettingsData(SettingsData);
        }

        static void PopulateSettings()
        {
            var buttonSideLeft = GameObject.Find("button_side_left");
            var buttonSideRight = GameObject.Find("button_side_right");
            buttonSideLeft.GetComponent<Image>().color = new Color(0f, 0f, 0f, SettingsData.Side == Side.Left ? 0.35f : 0.1f);
            buttonSideRight.GetComponent<Image>().color = new Color(0f, 0f, 0f, SettingsData.Side == Side.Right ? 0.35f : 0.1f);

            var buttonControlsUp = GameObject.Find("button_controls_up");
            var buttonControlsLeft = GameObject.Find("button_controls_left");
            var buttonControlsRight = GameObject.Find("button_controls_right");
            buttonControlsUp.GetComponentInChildren<TextMeshProUGUI>().text = SettingsData.Controls[SelectedPlayerNum].Keys[0].ToString();
            buttonControlsLeft.GetComponentInChildren<TextMeshProUGUI>().text = SettingsData.Controls[SelectedPlayerNum].Keys[1].ToString();
            buttonControlsRight.GetComponentInChildren<TextMeshProUGUI>().text = SettingsData.Controls[SelectedPlayerNum].Keys[2].ToString();

            var buttonPlayer = GameObject.Find("button_player");
            buttonPlayer.GetComponentInChildren<TextMeshProUGUI>().text = (SelectedPlayerNum + 1).ToString();

            var sliderHue = GameObject.Find("slider_hue");
            Color.RGBToHSV(SettingsData.Colors[SelectedPlayerNum], out float h, out float _, out float _);
            sliderHue.GetComponent<Slider>().value = h;
            sliderHue.GetComponentsInChildren<Image>()[1].color = SettingsData.Colors[SelectedPlayerNum];
            buttonPlayer.GetComponentsInChildren<Image>()[1].color = SettingsData.Colors[SelectedPlayerNum];

            var toggleWindowed = GameObject.Find("toggle_windowed").GetComponent<Toggle>();
            toggleWindowed.SetIsOnWithoutNotify(SettingsData.Windowed);
            Screen.fullScreenMode = SettingsData.Windowed ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow;

            var sliderVolume = GameObject.Find("slider_volume").GetComponent<Slider>();
            sliderVolume.value = SettingsData.Volume;
            var labelVolume = GameObject.Find("label_volume").GetComponent<TextMeshProUGUI>();
            labelVolume.text = $"{(int)(SettingsData.Volume * 100)}%";
            AudioListener.volume = SettingsData.Volume;

            //BETA
            var inputUsername = GameObject.Find("input_username").GetComponent<TMP_InputField>();
            inputUsername.text = SettingsData.Username;
        }

        static void Update()
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(keyCode))
                    {
                        TimeComponent.UpdateTicked -= Update;

                        OnKeyInput(keyCode);
                    }
                }
            }
        }

        static void SubscribeEventHandler()
        {
            ButtonSettings.Clicked += OnButtonSettings;
            ButtonSettingsBack.Clicked += OnButtonSettingsBack;
            ButtonSettingsSave.Clicked += OnButtonSettingsSave;
            ButtonSideLeft.Clicked += OnButtonSideLeft;
            ButtonSideRight.Clicked += OnButtonSideRight;
            ButtonControlsUp.Clicked += OnButtonControl;
            ButtonControlsLeft.Clicked += OnButtonControl;
            ButtonControlsRight.Clicked += OnButtonControl;
            ButtonPlayer.Clicked += OnButtonPlayer;
            SliderHue.ValueChanged += OnSliderHueChanged;
            LoginHelper.Login += OnLogin;
            SliderVolume.ValueChanged += OnSliderVolumeChanged;
            ToggleWindowed.Toggled += OnToggleWindowed;
            InputUsername.Changed += OnInputUsernameChanged; //BETA
            InputUsername.EndEdit += OnInputUsernameEndEdit; //BETA
        }

        static void UnsubscribeEventHandler()
        {
            ButtonSettings.Clicked -= OnButtonSettings;
            ButtonSettingsBack.Clicked -= OnButtonSettingsBack;
            ButtonSettingsSave.Clicked -= OnButtonSettingsSave;
            ButtonSideLeft.Clicked -= OnButtonSideLeft;
            ButtonSideRight.Clicked -= OnButtonSideRight;
            ButtonControlsUp.Clicked -= OnButtonControl;
            ButtonControlsLeft.Clicked -= OnButtonControl;
            ButtonControlsRight.Clicked -= OnButtonControl;
            ButtonPlayer.Clicked -= OnButtonPlayer;
            SliderHue.ValueChanged -= OnSliderHueChanged;
            LoginHelper.Login -= OnLogin;
            SliderVolume.ValueChanged -= OnSliderVolumeChanged;
            ToggleWindowed.Toggled -= OnToggleWindowed;
            InputUsername.Changed -= OnInputUsernameChanged; //BETA
            InputUsername.EndEdit -= OnInputUsernameEndEdit; //BETA
        }
    }
}

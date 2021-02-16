using Blobby.Components;
using Blobby.Game;
using Blobby.Models;
using Blobby.Networking;
using Blobby.UserInterface.Components;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
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
        
        #region UI EventHandler

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
            UnsubscribeControlHandler();

            var buttonControl = control switch
            {
                Control.Up => GameObject.Find("button_controls_up"),
                Control.Left => GameObject.Find("button_controls_left"),
                Control.Right => GameObject.Find("button_controls_right"),
                _ => throw new ArgumentOutOfRangeException(nameof(control), control, null)
            };

            var keyPressedSprite =
                Resources.Load<Sprite>("Graphics/UI/canvas_menu/panel_settings/buttons/button_settings_key_pressed");
            buttonControl.GetComponent<Image>().sprite = keyPressedSprite;

            _curControl = control;

            TimeComponent.UpdateTicked += Update;
        }

        static void OnKeyInput(KeyCode key)
        {
            var buttonControl = _curControl switch
            {
                Control.Up => GameObject.Find("button_controls_up"),
                Control.Left => GameObject.Find("button_controls_left"),
                Control.Right => GameObject.Find("button_controls_right"),
                _ => throw new ArgumentOutOfRangeException()
            };

            var keySprite =
                Resources.Load<Sprite>("Graphics/UI/canvas_menu/panel_settings/buttons/button_settings_key");
            buttonControl.GetComponent<Image>().sprite = keySprite;
            
            SettingsData.Controls[SelectedPlayerNum].SetControl(_curControl, key);
            SubscribeControlHandler();
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

        static void OnInputUsernameEndEdit(string username) //BETA
        {
            SettingsData.Username = username;
            IoHelper.SaveSettingsData(SettingsData);
            
            var buttonRanked = GameObject.Find("button_ranked").GetComponent<Button>();
            buttonRanked.interactable = SettingsData.Username != "";
        }
        
        #endregion

        static void PopulateSettings()
        {
            var buttonSideLeft = GameObject.Find("button_side_left");
            var buttonSideRight = GameObject.Find("button_side_right");
            buttonSideLeft.GetComponent<Image>().color = new Color(0f, 0f, 0f, SettingsData.Side == Side.Left ? 0.35f : 0.1f);
            buttonSideRight.GetComponent<Image>().color = new Color(0f, 0f, 0f, SettingsData.Side == Side.Right ? 0.35f : 0.1f);

            var buttonControlsUp = GameObject.Find("button_controls_up").GetComponentInChildren<TextMeshProUGUI>();
            var buttonControlsLeft = GameObject.Find("button_controls_left").GetComponentInChildren<TextMeshProUGUI>();
            var buttonControlsRight = GameObject.Find("button_controls_right").GetComponentInChildren<TextMeshProUGUI>();
            buttonControlsUp.text = BlobbyKey.KeyToName[SettingsData.Controls[SelectedPlayerNum].Keys[0]];
            buttonControlsLeft.text = BlobbyKey.KeyToName[SettingsData.Controls[SelectedPlayerNum].Keys[1]];
            buttonControlsRight.text = BlobbyKey.KeyToName[SettingsData.Controls[SelectedPlayerNum].Keys[2]];

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
            var buttonRanked = GameObject.Find("button_ranked").GetComponent<Button>();
            buttonRanked.interactable = SettingsData.Username != "";
        }

        static void Update()
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(keyCode))
                    {
                        var curKeyCode = SettingsData.Controls[SelectedPlayerNum].Keys[(int) _curControl];
                        if (keyCode == KeyCode.Mouse0)
                        {
                            TimeComponent.UpdateTicked -= Update;
                        
                            OnKeyInput(curKeyCode);
                        }
                        
                        if (!BlobbyKey.KeyToName.ContainsKey(keyCode)) continue;
                        
                        for (var i = 0; i < SettingsData.Controls.Length; i++)
                        {
                            var controlsData = SettingsData.Controls[i];
                            
                            for (var j = 0; j < controlsData.Keys.Length; j++)
                            {
                                var key = controlsData.Keys[j];
                                
                                if (keyCode == key)
                                {
                                    controlsData.Keys[j] = curKeyCode;
                                }
                            }
                        }
                        
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
            InputUsername.EndEdit -= OnInputUsernameEndEdit; //BETA
        }

        static void SubscribeControlHandler()
        {
            ButtonControlsUp.Clicked += OnButtonControl;
            ButtonControlsLeft.Clicked += OnButtonControl;
            ButtonControlsRight.Clicked += OnButtonControl;
        }

        static void UnsubscribeControlHandler()
        {
            ButtonControlsUp.Clicked -= OnButtonControl;
            ButtonControlsLeft.Clicked -= OnButtonControl;
            ButtonControlsRight.Clicked -= OnButtonControl;
        }
    }
}

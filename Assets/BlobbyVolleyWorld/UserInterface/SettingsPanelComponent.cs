using System;
using System.Collections.Generic;
using BlobbyVolleyWorld.Controls;
using BlobbyVolleyWorld.Match;
using BlobbyVolleyWorld.Settings;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlobbyVolleyWorld.UserInterface
{
    public class SettingsPanelComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [Required]
        Slider HueSlider { get; set; }
        
        [OdinSerialize]
        [Required]
        Image HueSliderImage { get; set; }
        
        [OdinSerialize]
        [Required]
        Image HueSliderHandleImage { get; set; }
        
        [OdinSerialize]
        [Required]
        Slider VolumeSlider { get; set; }
        
        [OdinSerialize]
        [Required]
        TextMeshProUGUI VolumeLabel { get; set; }
        
        [OdinSerialize]
        [Required]
        Toggle WindowedToggle { get; set; }
        
        [OdinSerialize]
        [Required]
        Sprite KeyPressedSprite { get; set; }
        
        [OdinSerialize]
        [Required]
        Image LeftSideButtonImage { get; set; }
        
        [OdinSerialize]
        [Required]
        Image RightSideButtonImage { get; set; }
        
        [OdinSerialize]
        [Required]
        TMP_InputField UsernameInputField { get; set; }

        [OdinSerialize]
        [Required]
        Dictionary<ControlType, Image> ControlButtonImages { get; set; } = new();
        
        [OdinSerialize]
        [Required]
        Dictionary<ControlType, TextMeshProUGUI> ControlButtonLabels { get; set; } = new();

        [OdinSerialize]
        [Required]
        TextMeshProUGUI PlayerButtonLabel { get; set; }
        
        [OdinSerialize]
        [Required]
        Image PlayerButtonImage { get; set; }
        
        ClientSettings ClientSettings { get; set; }
        
        ClientGameComponent ClientGameComponent { get; set; }
        MenuPanelComponent MenuPanelComponent { get; set; }
        
        void Awake()
        {
            ClientGameComponent = FindObjectOfType<ClientGameComponent>();
            MenuPanelComponent = FindObjectOfType<MenuPanelComponent>();
            
            ClientSettings = ClientGameComponent.ClientSettings;

            ClientGameComponent.ClientSettingsChanged += OnClientSettingsChanged;
            
            InitializeHueSlider();
        }

        public void OnButtonSave()
        {
            ClientGameComponent.UpdateClientSettings(ClientSettings);
        }

        public void OnButtonBack()
        {
            ClientSettings = ClientGameComponent.ClientSettings;
            ApplyClientSettings(ClientSettings);
            MenuPanelComponent.MoveView(Side.Right);
        }

        public void OnButtonSideLeft()
        {
            ClientSettings = ClientSettings.WithSide(Side.Left);
            ApplyClientSettings(ClientSettings);
        }

        public void OnButtonSideRight()
        {
            ClientSettings = ClientSettings.WithSide(Side.Right);
            ApplyClientSettings(ClientSettings);
        }

        public void OnButtonPlayer()
        {
            var newPlayerNum = (ClientSettings.PlayerNum + 1) % 4;
            ClientSettings = ClientSettings.WithPlayerNum(newPlayerNum);
            ApplyClientSettings(ClientSettings);
        }

        public void OnSliderHueChanged(float value)
        {
            var color = Color.HSVToRGB(value, 0.9f, 0.9f);
            
            ClientSettings = ClientSettings.WithColor(color, ClientSettings.PlayerNum);
            ApplyClientSettings(ClientSettings);
        }

        public void OnToggleWindowed(bool value)
        {
            ClientSettings = ClientSettings.WithIsWindowed(value);
            ApplyClientSettings(ClientSettings);
        }

        public void OnSliderVolumeChanged(float value)
        {
            ClientSettings = ClientSettings.WithVolume(value);
            ApplyClientSettings(ClientSettings);
        }

        public void OnInputUsernameEndEdit(string username)
        {
            ClientSettings = ClientSettings.WithUsername(username);
            ApplyClientSettings(ClientSettings);
        }

        void ApplyClientSettings(ClientSettings clientSettings)
        {
            LeftSideButtonImage.color = new Color(
                0f, 0f, 0f, 
                clientSettings.Side == Side.Left ? 0.35f : 0.1f);
            
            RightSideButtonImage.color = new Color(
                0f, 0f, 0f, 
                clientSettings.Side == Side.Right ? 0.35f : 0.1f);

            PlayerButtonLabel.text = $"{clientSettings.PlayerNum + 1}";
            
            var color = clientSettings.GetColor(clientSettings.PlayerNum);
            Color.RGBToHSV(color, out var h, out _, out _);

            HueSlider.SetValueWithoutNotify(h);
            HueSliderHandleImage.color = color;
            PlayerButtonImage.color = color;

            WindowedToggle.SetIsOnWithoutNotify(clientSettings.IsWindowed);
            
            Screen.fullScreenMode = ClientSettings.IsWindowed ? 
                FullScreenMode.Windowed : 
                FullScreenMode.FullScreenWindow;

            VolumeSlider.SetValueWithoutNotify(ClientSettings.Volume);
            VolumeLabel.text = $"{ClientSettings.Volume * 100:0}%";
            
            AudioListener.volume = ClientSettings.Volume;

            UsernameInputField.text = ClientSettings.Username;
        }
        
        void OnClientSettingsChanged(object sender, ClientSettings clientSettings)
        {
            ClientSettings = clientSettings;
            ApplyClientSettings(clientSettings);
        }
        
        void InitializeHueSlider()
        {
            var colors = new Color[]
            {
                new(1, 0, 0),
                new(1, 1, 0),
                new(0, 1, 0),
                new(0, 1, 1),
                new(0, 0, 1),
                new(1, 0, 1),
                new(1, 0, 0)
            };
            
            var hueTex = new Texture2D(colors.Length, 1);
            hueTex.SetPixels(colors);
            hueTex.Apply();

            var hueSprite = Sprite.Create(hueTex, new Rect(Vector2.zero, new Vector2(colors.Length, 1)), Vector2.one * 0.5f);
            hueSprite.texture.wrapMode = TextureWrapMode.Clamp;
            hueSprite.texture.Apply();

            HueSliderImage.sprite = hueSprite;
            HueSliderImage.preserveAspect = false;
        }

    }
}
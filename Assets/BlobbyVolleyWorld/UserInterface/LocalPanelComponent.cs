using BlobbyVolleyWorld.Extensions;
using BlobbyVolleyWorld.Match;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine.UI;

namespace BlobbyVolleyWorld.UserInterface
{
    public class LocalPanelComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [Required]
        TextMeshProUGUI LabelPlayerMode { get; set; }
        
        [OdinSerialize]
        [Required]
        TextMeshProUGUI LabelGameMode { get; set; }
        
        [OdinSerialize]
        [Required]
        TextMeshProUGUI LabelJumpMode { get; set; }
        
        [OdinSerialize]
        [Required]
        TextMeshProUGUI LabelMap { get; set; }
        
        [OdinSerialize]
        [Required]
        TextMeshProUGUI LabelSpeed { get; set; }

        [OdinSerialize]
        [Required]
        Image ImageMap { get; set; }
        
        GameComponent GameComponent { get; set; }
        ClientGameComponent ClientGameComponent { get; set; }
        MenuPanelComponent MenuPanelComponent { get; set; }

        void Awake()
        {
            GameComponent = FindObjectOfType<GameComponent>();
            ClientGameComponent = FindObjectOfType<ClientGameComponent>();
            MenuPanelComponent = FindObjectOfType<MenuPanelComponent>();
        }
        
        public void OnButtonPlayerMode(int playerModeIndex)
        {
            var playerMode = (PlayerMode)playerModeIndex;
            LabelPlayerMode.text = playerMode.ToString();
            
            ClientGameComponent.UpdateMatchSettings(
                ClientGameComponent
                    .MatchSettings
                    .WithPlayerMode(playerMode));
        }

        public void OnButtonGameMode(int gameModeIndex)
        {
            var gameMode = (GameMode)gameModeIndex;
            LabelGameMode.text = gameMode.ToString();
            
            ClientGameComponent.UpdateMatchSettings(
                ClientGameComponent
                    .MatchSettings
                    .WithGameMode(gameMode));
        }

        public void OnButtonJumpMode(int jumpModeIndex)
        {
            var jumpMode = (JumpMode)jumpModeIndex;
            LabelJumpMode.text = jumpMode.ToString();
            
            ClientGameComponent.UpdateMatchSettings(
                ClientGameComponent
                    .MatchSettings
                    .WithJumpMode(jumpMode));
        }

        public void OnButtonMap(bool isIncrease)
        {
            var map = isIncrease ? 
                ClientGameComponent.MatchSettings.Map.Increase() : 
                ClientGameComponent.MatchSettings.Map.Decrease();
            var mapAsset = GameComponent.FindMapAsset(map);

            LabelMap.text = mapAsset.Name;
            ImageMap.sprite = mapAsset.Icon;

            ClientGameComponent.UpdateMatchSettings(
                ClientGameComponent
                    .MatchSettings
                    .WithMap(map));
        }

        public void OnSliderLocalSpeed(float value)
        {
            LabelSpeed.text = $"{value}%";
            
            ClientGameComponent.UpdateMatchSettings(
                ClientGameComponent
                    .MatchSettings
                    .WithSpeed(value));
        }
        
        public void OnToggleJumpOverNet(bool value)
        {
            ClientGameComponent.UpdateMatchSettings(
                ClientGameComponent
                    .MatchSettings
                    .WithIsJumpOverNet(value));
        }
        
        public void OnButtonBack()
        {
            MenuPanelComponent.MoveView(Side.Right);
        }

        public void OnButtonPlay()
        {
            
        }

        public void OnButtonPlayAi()
        {
            
        }
    }
}
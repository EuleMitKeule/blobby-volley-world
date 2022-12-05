using System.Collections.Generic;
using BlobbyVolleyWorld.Match;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace BlobbyVolleyWorld.UserInterface
{
    public class MenuPanelComponent : SerializedMonoBehaviour
    {
        static readonly int MoveLeft = Animator.StringToHash("moveLeft");
        static readonly int MoveRight = Animator.StringToHash("moveRight");

        [OdinSerialize]
        [Required]
        Dictionary<PanelType, GameObject> Panels { get; set; } = new();

        [OdinSerialize]
        [Required]
        List<ColorComponent> ColorComponents { get; set; } = new();

        Animator Animator { get; set; }
        
        void Awake()
        {
            Animator = GetComponent<Animator>();
            
            SetColor(Color.grey);
        }
        
        public void MoveView(Side side)
        {
            Animator.SetTrigger(side == Side.Left ? MoveLeft : MoveRight);
        }

        void SetColor(Color color)
        {
            foreach (var colorComponent in ColorComponents)
            {
                colorComponent.SetColor(color);
            }
        }

        void SetPanelVisibility(PanelType panelType, bool isVisible)
        {
            if (!Panels.ContainsKey(panelType))
            {
                Debug.LogError($"Panel {panelType} not found");
                return;
            }
            
            var panelObject = Panels[panelType];
            var canvasGroups = panelObject.GetComponentsInChildren<CanvasGroup>();
            
            foreach (var canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = isVisible ? 1 : 0;
                canvasGroup.interactable = isVisible;
                canvasGroup.blocksRaycasts = isVisible;
            }
        }

        public void OnButtonBrowser()
        {
            SetPanelVisibility(PanelType.Local, false);
            SetPanelVisibility(PanelType.Browser, true);
            SetPanelVisibility(PanelType.Settings, false);
        }

        public void OnButtonLocal()
        {
            SetPanelVisibility(PanelType.Local, true);
            SetPanelVisibility(PanelType.Browser, false);
            SetPanelVisibility(PanelType.Settings, false);
            
            MoveView(Side.Left);
        }

        public void OnButtonSettings()
        {
            SetPanelVisibility(PanelType.Local, false);
            SetPanelVisibility(PanelType.Browser, false);
            SetPanelVisibility(PanelType.Settings, true);
            
            MoveView(Side.Left);
        }

        public void OnButtonQuit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
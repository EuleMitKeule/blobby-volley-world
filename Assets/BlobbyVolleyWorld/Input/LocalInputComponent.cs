using System.Collections.Generic;
using BlobbyVolleyWorld.Entities.Input.Jumping;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BlobbyVolleyWorld.Entities.Input
{
    [RequireComponent(typeof(PlayerComponent))]
    public class LocalInputComponent : InputComponent
    {
        [OdinSerialize]
        [Required]
        InputActionAsset InputActionAsset { get; set; }
        
        [OdinSerialize]
        [Required]
        Dictionary<FieldPosition, string> InputActionMapNames { get; set; }
        
        [OdinSerialize]
        [Required]
        string JumpActionName { get; set; }
        
        [OdinSerialize]
        [Required]
        string HorizontalActionName { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        InputAction JumpAction { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        InputAction HorizontalAction { get; set; }
        
        public override float HorizontalInput => 
            HorizontalAction.ReadValue<float>();
        
        IJumpInputStrategy JumpInputStrategy { get; set; }
        
        PlayerComponent PlayerComponent { get; set; }
        
        protected override void Awake()
        {
            base.Awake();
            
            PlayerComponent = GetComponent<PlayerComponent>();
            
            var inputActionMapName = InputActionMapNames[PlayerComponent.FieldPosition];
            var inputActionMap = InputActionAsset.FindActionMap(inputActionMapName);
            
            JumpAction = inputActionMap.FindAction(JumpActionName);
            HorizontalAction = inputActionMap.FindAction(HorizontalActionName);
            
            JumpAction.Enable();
            HorizontalAction.Enable();
            
            JumpAction.started += OnJumpDown;
            JumpAction.canceled += OnJumpUp;

            JumpInputStrategy = JumpInputStrategyFactory.Create(
                this, 
                GameComponent.MatchSettings.JumpMode);
        }

        void OnJumpUp(InputAction.CallbackContext context)
        {
            JumpInputStrategy.OnJumpUp();
        }
        
        void OnJumpDown(InputAction.CallbackContext context)
        {
            JumpInputStrategy.OnJumpDown();
        }
    }
}
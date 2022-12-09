using BlobbyVolleyWorld.Game;
using UnityEngine;

namespace BlobbyVolleyWorld.Entities.Input.Jumping
{
    public class SpringJumpInputStrategy : IJumpInputStrategy
    {
        InputComponent InputComponent { get; }
        ChargeTimer ChargeTimer { get; }

        GameComponent GameComponent { get; }
        
        public SpringJumpInputStrategy(InputComponent inputComponent)
        {
            GameComponent = Object.FindObjectOfType<GameComponent>();
            
            InputComponent = inputComponent;
            ChargeTimer = new ChargeTimer();
            
            ChargeTimer.ChargeTimerTicked += OnChargeTimerTicked;
        }

        void OnChargeTimerTicked()
        {
            if (InputComponent.JumpCharge >= GameComponent.PhysicsAsset.JumpChargeMax) return;
            InputComponent.JumpCharge += GameComponent.PhysicsAsset.JumpChargeIncrease;
        }

        public void OnJumpDown()
        {
            InputComponent.JumpCharge = GameComponent.PhysicsAsset.JumpChargeMin;
            InputComponent.IsUpPressed = true;
            ChargeTimer.Start();
        }

        public void OnJumpUp()
        {
            ChargeTimer.Stop();
            InputComponent.IsUpPressed = false;
        }
    }
}
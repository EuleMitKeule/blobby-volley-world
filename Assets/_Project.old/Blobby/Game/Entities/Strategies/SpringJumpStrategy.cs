using UnityEngine;

namespace Blobby.Game.Entities.Strategies
{
    public class SpringJumpStrategy : IJumpStrategy
    {
        PlayerComponent PlayerComponent { get; }

        ChargeTimer ChargeTimer { get; }
        float JumpCharge { get; set; }

        public SpringJumpStrategy(PlayerComponent playerComponent)
        {
            PlayerComponent = playerComponent;
            ChargeTimer = new ChargeTimer();

            ChargeTimer.ChargeTimerTicked += OnChargeTimerTicked;
        }

        public void OnJumpDown()
        {
            PlayerComponent.KeyPressed[0] = true;
            ChargeTimer.Start();
        }

        public void OnJumpUp()
        {
            PlayerComponent.KeyPressed[0] = false;

            if (PlayerComponent.IsGrounded)
            {
                PlayerComponent.IsGrounded = false;
                PlayerComponent.Velocity = new Vector2(PlayerComponent.Velocity.x, JumpCharge * PlayerComponent.JUMP_VELOCITY);
                ChargeTimer.Stop();
                JumpCharge = PlayerComponent.MIN_CHARGE;
            }
        }

        public void OnJump()
        {
        }

        public void OnJumpHold()
        {

        }

        void OnChargeTimerTicked()
        {
            if (CanCharge()) JumpCharge += PlayerComponent.CHARGE_INCREASE;
        }

        bool CanCharge()
        {
            return JumpCharge < PlayerComponent.MAX_CHARGE;
        }
    }
}
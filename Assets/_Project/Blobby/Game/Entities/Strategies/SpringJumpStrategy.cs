using System;
using UnityEngine;

namespace Blobby.Game.Entities
{
    public class SpringJumpStrategy : IJumpStrategy
    {
        Player _player;
        MatchComponent _matchComponent;

        ChargeTimer _chargeTimer;
        float _jumpCharge;

        public SpringJumpStrategy(Player player, MatchComponent matchComponent)
        {
            _player = player;
            _matchComponent = matchComponent;
            _chargeTimer = new ChargeTimer();

            _chargeTimer.ChargeTimerTicked += OnChargeTimerTicked;
        }

        public void OnJumpDown()
        {
            _player.KeyPressed[0] = true;
            _chargeTimer.Start();
        }

        public void OnJumpUp()
        {
            _player.KeyPressed[0] = false;

            if (_player.IsGrounded)
            {
                _player.IsGrounded = false;
                _player.Velocity = new Vector2(_player.Velocity.x, _jumpCharge * _matchComponent.PhysicsSettings.playerJumpVelocity);
                _chargeTimer.Stop();
                _jumpCharge = _matchComponent.PhysicsSettings.playerMinCharge;
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
            if (CanCharge()) _jumpCharge += _matchComponent.PhysicsSettings.playerChargeIncrease;
        }

        bool CanCharge()
        {
            return _jumpCharge < _matchComponent.PhysicsSettings.playerMaxCharge;
        }
    }
}
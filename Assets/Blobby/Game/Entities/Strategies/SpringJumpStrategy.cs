using System;
using UnityEngine;

namespace Blobby.Game.Entities
{
    public class SpringJumpStrategy : IJumpStrategy
    {
        Player _player;
        Match _match;

        ChargeTimer _chargeTimer;
        float _jumpCharge;

        public SpringJumpStrategy(Player player, Match match)
        {
            _player = player;
            _match = match;
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
                _player.Velocity = new Vector2(_player.Velocity.x, _jumpCharge * _match.PhysicsSettings.playerJumpVelocity);
                _chargeTimer.Stop();
                _jumpCharge = _match.PhysicsSettings.playerMinCharge;
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
            if (CanCharge()) _jumpCharge += _match.PhysicsSettings.playerChargeIncrease;
        }

        bool CanCharge()
        {
            return _jumpCharge < _match.PhysicsSettings.playerMaxCharge;
        }
    }
}
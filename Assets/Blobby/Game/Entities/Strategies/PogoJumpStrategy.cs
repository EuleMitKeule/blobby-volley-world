﻿using UnityEngine;

namespace Blobby.Game.Entities
{
    public class PogoJumpStrategy : IJumpStrategy
    {
        Player _player;
        Match _match;

        public PogoJumpStrategy(Player player, Match match)
        {
            _player = player;
            _match = match;
        }

        public void OnJumpDown()
        {
            _player.KeyPressed[0] = true;
        }

        public void OnJumpUp()
        {

        }

        public void OnJump()
        {
            _player.IsGrounded = false;
            _player.Velocity = new Vector2(_player.Velocity.x, _match.PhysicsSettings.playerJumpVelocity);
        }

        public void OnJumpHold()
        {
            _player.Velocity += Vector2.up * Time.fixedDeltaTime * _match.PhysicsSettings.playerJumpDrift;
        }
    }
}
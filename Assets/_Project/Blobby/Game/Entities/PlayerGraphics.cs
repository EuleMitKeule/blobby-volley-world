using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Models;
using Blobby.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blobby.Game.Physics;
using TMPro;
using UnityEngine;

namespace Blobby.Game.Entities
{
    public class PlayerGraphics
    {
        public PlayerData PlayerData { get; private set; }

        Player _player;

        public PlayerGraphics(Player player, PlayerData playerData)
        {
            PlayerData = playerData;

            PlayerData.PlayerNumChanged += OnPlayerNumChanged;

            _player = player;

            Apply();
        }

        void OnPlayerNumChanged()
        {
            Apply();
        }

        public void SetPlayerData(PlayerData playerData)
        {
            PlayerData = playerData;
            Apply();
        }

        public void SetAlpha(bool isTransparent)
        {
            var sr = _player.PlayerObj.GetComponent<SpriteRenderer>();
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, isTransparent ? 0.5f : 1f);
        }

        public void SetNameLabelPos()
        {
            var position = _player.PlayerObj.transform.position;
            var nameLabel = _player.PlayerObj.GetComponentInChildren<TextMeshProUGUI>();
            nameLabel.transform.position = new Vector2(position.x, Player.NAME_LABEL_OFFSET);
        }

        public void SetShadow()
        {
            var position = _player.PlayerObj.transform.position;
            var shadowX = position.x + Mathf.Abs(position.y - PhysicsWorld.Ground) * Player.SHADOW_MOD + Player.SHADOW_X;
            var shadowY = PhysicsWorld.Ground - _player.BottomOffset + Player.SHADOW_Y + Mathf.Abs(position.y - PhysicsWorld.Ground - _player.BottomOffset) * Player.SHADOW_MOD;
            _player.PlayerObj.transform.GetChild(0).position = new Vector2(shadowX, shadowY);
            _player.PlayerObj.transform.GetChild(0).rotation = Quaternion.identity;
        }

        void Apply()
        {
            var spriteRenderers = _player.PlayerObj.GetComponentsInChildren<SpriteRenderer>();
            var num = (PlayerData.PlayerNum + 2) % 4;

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                var spriteRenderer = spriteRenderers[i];
                if (i == 1)
                {
                    spriteRenderer.sortingOrder = 0;
                    continue;
                }
                spriteRenderer.sortingOrder += num * spriteRenderers.Length;

            }

            _player.PlayerObj.GetComponentInChildren<SpriteRenderer>().color = PlayerData.Color;

            if (_player.PlayerObj.GetComponentInChildren<TextMeshProUGUI>() != null)
                _player.PlayerObj.GetComponentInChildren<TextMeshProUGUI>().text = PlayerData.Name;
        }

        public void SetInvisible(bool value)
        {
            MainThreadManager.Run(() =>
            {
                if (!_player.PlayerObj) return;

                var spriteRenderers = _player.PlayerObj.GetComponentsInChildren<SpriteRenderer>();

                foreach (var spriteRenderer in spriteRenderers) spriteRenderer.enabled = !value;

                var nameLabel = _player.PlayerObj.GetComponentInChildren<TextMeshProUGUI>();
                nameLabel.enabled = !value;
            });
        }
    }
}

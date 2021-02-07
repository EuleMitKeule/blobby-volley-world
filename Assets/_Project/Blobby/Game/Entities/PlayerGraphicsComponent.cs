using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Models;
using Blobby.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blobby.Game.Physics;
using Blobby.Networking;
using TMPro;
using UnityEngine;

namespace Blobby.Game.Entities
{
    [RequireComponent(typeof(Animator))]
    public class PlayerGraphicsComponent : MonoBehaviour
    {
        Animator Animator { get; set; }
        IPlayerGraphicsProvider PlayerGraphicsProvider { get; set; }
        PlayerData PlayerData { get; set; }

        Vector2 TransformPosition => transform.position;

        CircleCollider2D[] Colliders => GetComponents<CircleCollider2D>();
        CircleCollider2D LowerCollider { get; set; }
        float LowerRadius => LowerCollider.radius;
        Vector2 LowerOffset => LowerCollider.offset;
        float BottomOffset => LowerOffset.y - LowerRadius;

        const float NAME_LABEL_OFFSET = -9.3f;
        const float SHADOW_X = 1.588f;
        const float SHADOW_Y = -1.73f;
        const float SHADOW_MOD = 0.1f;

        static readonly int IsRunningAnim = Animator.StringToHash("isRunning");
        static readonly int IsGroundedAnim = Animator.StringToHash("grounded");

        void Awake()
        {
            Animator = GetComponent<Animator>();
            LowerCollider = Colliders[1];

            PlayerGraphicsProvider = GetComponent<IPlayerGraphicsProvider>();
            PlayerGraphicsProvider.PlayerDataChanged += OnPlayerDataChanged;
            PlayerGraphicsProvider.AlphaChanged += OnAlpha;
        }

        void Update()
        {
            Animator.SetBool(IsRunningAnim, PlayerGraphicsProvider.IsRunning);
            Animator.SetBool(IsGroundedAnim, PlayerGraphicsProvider.IsGrounded);

            SetShadow();
            SetNameLabelPos();
        }

        void OnPlayerDataChanged(PlayerData playerData)
        {
            PlayerData = playerData;
            MainThreadManager.Run(Apply);
        }

        void OnAlpha(int playerNum, bool isTransparent)
        {
            MainThreadManager.Run(() =>
            {
                if (playerNum != PlayerData.PlayerNum) return;

                var sr = GetComponent<SpriteRenderer>();
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, isTransparent ? 0.5f : 1f);
            });
        }

        void SetNameLabelPos()
        {
            var position = transform.position;
            var nameLabel = GetComponentInChildren<TextMeshProUGUI>();
            nameLabel.transform.position = new Vector2(position.x, NAME_LABEL_OFFSET);
        }

        void SetShadow()
        {
            var position = transform.position;
            var shadowX = position.x + Mathf.Abs(position.y - PhysicsWorld.Ground) * SHADOW_MOD + SHADOW_X;
            var shadowY = PhysicsWorld.Ground - BottomOffset + SHADOW_Y + Mathf.Abs(position.y - PhysicsWorld.Ground - BottomOffset) * SHADOW_MOD;
            transform.GetChild(0).position = new Vector2(shadowX, shadowY);
            transform.GetChild(0).rotation = Quaternion.identity;
        }

        void Apply()
        {
            SetInvisible(PlayerGraphicsProvider.IsInvisible);

            if (PlayerData.PlayerNum == 2) OnAlpha(2, true);

            var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            var num = (PlayerData.PlayerNum + 2) % 4;

            for (var i = 0; i < spriteRenderers.Length; i++)
            {
                var spriteRenderer = spriteRenderers[i];
                if (i == 1)
                {
                    spriteRenderer.sortingOrder = 0;
                    continue;
                }
                spriteRenderer.sortingOrder += num * spriteRenderers.Length;

            }

            GetComponentInChildren<SpriteRenderer>().color = PlayerData.Color;

            if (GetComponentInChildren<TextMeshProUGUI>())
                GetComponentInChildren<TextMeshProUGUI>().text = PlayerData.Name;
        }

        void SetInvisible(bool value)
        {
            MainThreadManager.Run(() =>
            {
                var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

                foreach (var spriteRenderer in spriteRenderers) spriteRenderer.enabled = !value;

                var nameLabel = GetComponentInChildren<TextMeshProUGUI>();
                nameLabel.enabled = !value;
            });
        }
    }
}

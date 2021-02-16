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
using UnityEngine.UIElements;

namespace Blobby.Game.Entities
{
    [RequireComponent(typeof(Animator))]
    public class PlayerGraphicsComponent : MonoBehaviour
    {
        Animator Animator { get; set; }
        IPlayerGraphicsProvider PlayerGraphicsProvider { get; set; }

        PlayerData _playerData;
        PlayerData PlayerData
        {
            get {return _playerData;}
            set
            {
                _playerData = value;
                Apply();
            }
        }

        Vector2 TransformPosition => transform.position;
        
        SpriteRenderer SpriteRenderer { get; set; }
        SpriteRenderer[] SpriteRenderers => GetComponentsInChildren<SpriteRenderer>();
        TextMeshProUGUI NameLabel { get; set; }
        Transform ShadowTransform => transform.GetChild(0);

        EdgeCollider2D GroundCollider { get; set; }
        float BottomOffset => GroundCollider.offset.y;

        const float NAME_LABEL_OFFSET = -9.3f;
        const float SHADOW_MOD = 0.1f;

        static readonly int IsRunningAnim = Animator.StringToHash("isRunning");
        static readonly int IsGroundedAnim = Animator.StringToHash("grounded");

        void Awake()
        {
            NameLabel = GetComponentInChildren<TextMeshProUGUI>();
            GroundCollider = GetComponent<EdgeCollider2D>();

            PlayerGraphicsProvider = GetComponent<IPlayerGraphicsProvider>();
            PlayerGraphicsProvider.PlayerDataChanged += (playerData) => { PlayerData = playerData; };
            PlayerGraphicsProvider.AlphaChanged += OnAlpha;
        }

        void Start()
        {
            Animator = GetComponent<Animator>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            Animator.SetBool(IsRunningAnim, PlayerGraphicsProvider.IsRunning);
            Animator.SetBool(IsGroundedAnim, PlayerGraphicsProvider.IsGrounded);

            SetShadow();
            SetNameLabelPos();
        }

        void SetupSortingLayers()
        {
            var num = (PlayerData.PlayerNum + 2) % 4;

            for (var i = 0; i < SpriteRenderers.Length; i++)
            {
                var spriteRenderer = SpriteRenderers[i];
                if (i == 1)
                {
                    spriteRenderer.sortingOrder = 0;
                    continue;
                }
                spriteRenderer.sortingOrder += num * SpriteRenderers.Length;
            }
        }

        void OnAlpha(int playerNum, bool isTransparent)
        {
            MainThreadManager.Run(() =>
            {
                if (playerNum != PlayerData.PlayerNum) return;

                var curColor = SpriteRenderer.color;
                SpriteRenderer.color = new Color(curColor.r, curColor.g, curColor.b, isTransparent ? 0.5f : 1f);
            });
        }

        void SetNameLabelPos()
        {
            var position = transform.position;
            NameLabel.transform.position = new Vector2(position.x, NAME_LABEL_OFFSET);
        }

        void SetShadow()
        {
            var position = transform.position;
            var groundY = PhysicsWorld.Ground + (SpriteRenderer.bounds.min.y - TransformPosition.y - BottomOffset);
            var shadowX = position.x + Mathf.Abs(position.y + BottomOffset - groundY) * SHADOW_MOD;
            var shadowY = groundY + Mathf.Abs(position.y + BottomOffset - groundY) * SHADOW_MOD;
            ShadowTransform.position = new Vector2(shadowX, shadowY);
        }

        void Apply()
        {
            MainThreadManager.Run(() =>
            {
                SetInvisible(PlayerGraphicsProvider.IsInvisible);

                if (PlayerData.PlayerNum == 2) OnAlpha(2, true);

                SpriteRenderer.color = PlayerData.Color;

                if (NameLabel)
                    NameLabel.text = PlayerData.Name;

                SetupSortingLayers();
            });
        }

        void SetInvisible(bool value)
        {
            MainThreadManager.Run(() =>
            {
                foreach (var spriteRenderer in SpriteRenderers) spriteRenderer.enabled = !value;

                NameLabel.enabled = !value;
            });
        }
    }
}

﻿using BeardedManStudios.Forge.Networking.Unity;
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
            SpriteRenderer = GetComponent<SpriteRenderer>();
            LowerCollider = Colliders[1];
            NameLabel = GetComponentInChildren<TextMeshProUGUI>();

            PlayerGraphicsProvider = GetComponent<IPlayerGraphicsProvider>();
            PlayerGraphicsProvider.PlayerDataChanged += (playerData) => { PlayerData = playerData; };
            PlayerGraphicsProvider.AlphaChanged += OnAlpha;
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
            var shadowX = position.x + Mathf.Abs(position.y - PhysicsWorld.Ground) * SHADOW_MOD;
            var shadowY = PhysicsWorld.Ground - BottomOffset + Mathf.Abs(position.y - PhysicsWorld.Ground - BottomOffset) * SHADOW_MOD;
            ShadowTransform.position = new Vector2(shadowX, shadowY);
            //transform.GetChild(0).rotation = Quaternion.identity;
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

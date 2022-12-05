using System;
using Blobby.Game.Physics;
using Blobby.Models;
using UnityEngine;

namespace Blobby.Game.Entities
{
    public class BallGraphicsComponent : MonoBehaviour
    {
        CircleCollider2D Collider { get; set; }
        float Radius => Collider.radius;
        Vector2 TransformPosition => transform.position;

        GameObject ShadowObject { get; set; }
        GameObject ArrowObject { get; set; }
        GameObject ShadingObject { get; set; }
        GameObject GlowObject { get; set; }
        SpriteRenderer ArrowRenderer { get; set; }

        Vector2 ArrowPosition =>
            new Vector2(transform.position.x, ARROW_HEIGHT);

        bool IsAboveArrowLimit => transform.position.y > ARROW_LIMIT;
        float DistanceToGround => TransformPosition.VerticalDistance(PhysicsWorld.Ground) - Radius;

        Vector2 ShadowPosition => new Vector2
        (
            TransformPosition.x + DistanceToGround * SHADOW_MODIFIER,
            PhysicsWorld.Ground + DistanceToGround * SHADOW_MODIFIER
        );

        const float ARROW_HEIGHT = 10.5f;
        const float ARROW_LIMIT = 10.85f;
        const float SHADOW_MODIFIER = 0.1f;
        const float SHADING_ROTATION = -0f;
        const float GLOW_POSITION_X = -0.125f;
        const float GLOW_POSITION_Y = -0.069f;

    void Awake()
    {
    Collider = GetComponent<CircleCollider2D>();

    ShadowObject = transform.GetChild(0).gameObject;
    ArrowObject = transform.GetChild(1).gameObject;
    ShadingObject = transform.GetChild(2).gameObject;
    GlowObject = transform.GetChild(3).gameObject;

    ArrowRenderer = ArrowObject.GetComponent<SpriteRenderer>();
    ArrowRenderer.enabled = false;
    }

    void LateUpdate()
    {
    SetArrow();
    SetShadow();
    SetShadingAndGlow();
    }

    void SetArrow()
    {
    if (!ArrowObject) return;

    ArrowObject.transform.position = ArrowPosition;
    ArrowObject.transform.rotation = Quaternion.identity;

    ArrowRenderer.enabled = IsAboveArrowLimit;
}

        void SetShadow()
        {
            if (!ShadowObject) return;

            ShadowObject.transform.position = ShadowPosition;
            ShadowObject.transform.rotation = Quaternion.identity;
        }

        void SetShadingAndGlow()
        {
            if (!ShadingObject || !GlowObject) return;

            ShadingObject.transform.rotation = Quaternion.Euler(0f, 0f, SHADING_ROTATION);

            GlowObject.transform.position = transform.position + new Vector3(GLOW_POSITION_X, GLOW_POSITION_Y);
            GlowObject.transform.rotation = Quaternion.identity;
        }
    }
}
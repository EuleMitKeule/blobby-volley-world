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
        SpriteRenderer ArrowRenderer { get; set; }

        Vector2 ArrowPosition => 
            new Vector2(transform.position.x, ARROW_HEIGHT);
        bool IsAboveArrowLimit => transform.position.y > ARROW_LIMIT;
        float DistanceToGround => TransformPosition.VerticalDistance(PhysicsWorld.Ground);
        Vector2 ShadowPosition => new Vector2
        (
            TransformPosition.x + DistanceToGround * SHADOW_MODIFIER,
            PhysicsWorld.Ground - Radius + DistanceToGround * SHADOW_MODIFIER
        );
        
        const float ARROW_HEIGHT = 10.5f;
        const float ARROW_LIMIT = 10.85f;
        const float SHADOW_MODIFIER = 0.1f;

        void Awake()
        {
            Collider = GetComponent<CircleCollider2D>();
            
            ShadowObject = transform.GetChild(0).gameObject;
            ArrowObject = transform.GetChild(1).gameObject;
            
            ArrowRenderer = ArrowObject.GetComponent<SpriteRenderer>();
            ArrowRenderer.enabled = false;
        }

        void Update()
        {
            SetArrow();
            SetShadow();
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
    }
}
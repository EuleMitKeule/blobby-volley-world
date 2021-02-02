using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blobby.Game.Physics
{
    public static class PhysicsWorld
    {
        public static EdgeCollider2D GroundCollider { get; private set; }
        public static CircleCollider2D NetEdgeCollider { get; private set; }
        public static EdgeCollider2D LeftNetCollider { get; private set; }
        public static EdgeCollider2D RightNetCollider { get; private set; }
        public static float Ground => GroundCollider.offset.y;
        public static Vector2 NetEdgeTop => NetEdgeCollider.offset + Vector2.up * NetEdgeCollider.radius;
        
        public const int MAP_LAYER = 9;

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            var parentMapObj = GameObject.Find("parent_map");
            var edgeColliders = parentMapObj.GetComponents<EdgeCollider2D>();
            NetEdgeCollider = parentMapObj.GetComponent<CircleCollider2D>();
            LeftNetCollider = edgeColliders[2];
            RightNetCollider = edgeColliders[3];
            GroundCollider = edgeColliders[4];
        }
        
        /// <summary>
        /// Solves a quadratic equation with given values p and q
        /// </summary>
        /// <param name="p">The p value of the equation</param>
        /// <param name="q">The q value of the equation</param>
        /// <returns>Two solutions if both are unique and real
        /// One solution and null if both solutions are equivalent or one is complex
        /// null and null if both solutions are complex</returns>
        public static (float?, float?) SolveQuadratic(float p, float q)
        {
            var linear = -p / 2;
            var discriminant = p.Squared() / 4 - q;

            var lambda1 = linear + Mathf.Sqrt(discriminant);
            var lambda2 = linear - Mathf.Sqrt(discriminant);
            
            if (float.IsNaN(lambda1) && float.IsNaN(lambda2)) return (null, null);
            else if (float.IsNaN(lambda1)) return (lambda2, null);
            else if (float.IsNaN(lambda2)) return (lambda1, null);
            
            if (Math.Abs(lambda1 - lambda2) < 0.00001f) return (lambda1, null);
            
            var isLess = lambda1 < lambda2;
            
            return (isLess ? lambda1 : lambda2, isLess ? lambda2 : lambda1);
        }

        public static Vector2 Mean(IEnumerable<Vector2> vectors)
        {
            var enumerable = vectors as Vector2[] ?? vectors.ToArray();
            var sum = enumerable.Aggregate(Vector2.zero, (current, vector) => current + vector);
            return sum / enumerable.Count();
        }
    }
}
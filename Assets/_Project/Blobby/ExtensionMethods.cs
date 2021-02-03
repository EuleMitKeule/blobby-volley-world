using UnityEngine;

namespace Blobby.Game
{
    public static class Vector2Extensions
    {    
        public static bool IsLeftOf(this Vector2 vector, Vector3 other) => vector.x < other.x;
        
        public static bool IsLeftOf(this Vector2 vector, float value) => vector.x < value;
        
        public static bool IsRightOf(this Vector2 vector, Vector3 other) => vector.x > other.x;
        
        public static bool IsRightOf(this Vector2 vector, float value) => vector.x > value;
        
        public static bool IsAbove(this Vector2 vector, Vector3 other) => vector.y > other.y;
        
        public static bool IsAbove(this Vector2 vector, float value) => vector.y > value;
        
        public static bool IsBelow(this Vector2 vector, Vector3 other) => vector.y < other.y;
        
        public static bool IsBelow(this Vector2 vector, float value) => vector.y < value;
        
        public static Vector2 DirectionTo(this Vector2 vector, Vector2 to) => (to - vector).normalized;
        
        public static Vector2 To(this Vector2 vector, Vector2 to) => (to - vector);

        public static Vector2 Perpendicular(this Vector2 vector) => new Vector2(vector.y, -vector.x);
        
        public static float Dot(this Vector2 vector, Vector2 other) => vector.x * other.x + vector.y * other.y;
        
        public static bool IsAcute(this Vector2 vector, Vector2 other) => vector.Dot(other) > 0f;
        
        public static float Squared(this Vector2 vector) => vector.Dot(vector);
    }
    
    public static class FloatExtensions
    {
        public static float Squared(this float value) => value * value;
    }
}
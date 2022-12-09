using UnityEngine;

namespace BlobbyVolleyWorld.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 RotateClockwise(this Vector2 vector, Vector2 around) =>
            new Vector2(vector.x * around.y - vector.y * around.x, vector.y * around.y + vector.x * around.x);   
    }
}
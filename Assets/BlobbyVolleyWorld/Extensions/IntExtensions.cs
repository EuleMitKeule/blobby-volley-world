using System;

namespace BlobbyVolleyWorld.Extensions
{
    public static class IntExtensions
    {
        public static int Mod(this int x, int m)
        {
            return (x % m + m) % m;
        }
    }
}
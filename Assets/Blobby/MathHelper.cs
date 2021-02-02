
namespace Blobby
{
    public static class MathHelper
    {
        public static int Mod(int k, int n)
        {
            return ((k %= n) < 0) ? k + n : k;
        }
    }
}

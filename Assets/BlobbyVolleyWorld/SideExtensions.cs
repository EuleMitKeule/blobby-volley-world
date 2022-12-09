using BlobbyVolleyWorld.Match;

namespace BlobbyVolleyWorld
{
    public static class SideExtensions
    {
        public static int ToSign(this Side side) => 
            side == Side.Left ? -1 : 1;
    }
}
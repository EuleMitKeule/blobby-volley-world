using System;

namespace Blobby
{
    public enum Side
    {
        Left, Right, None
    }

    public static class SideExtensions
    {
        public static int GetSign(this Side side) => side switch
        {
            Side.Left => -1,
            Side.Right => 1,
            Side.None => 0,
            _ => 0
        };

        public static Side Other(this Side side) => side switch
        {
            Side.Left => Side.Right,
            Side.Right => Side.Left,
            Side.None => Side.None,
            _ => Side.None
        };
    }
}
using System;
using System.Collections.Generic;
using BlobbyVolleyWorld.Match;

namespace BlobbyVolleyWorld
{
    public static class PlayerModeExtensions
    {
        public static int ToPlayerCount(this PlayerMode mode) =>
            mode switch
            {
                PlayerMode.Single => 2,
                PlayerMode.Double => 4,
                PlayerMode.DoubleFixed => 4,
                PlayerMode.Ghost => 2,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        
        public static List<FieldPosition> ToFieldPositions(this PlayerMode mode) =>
            mode switch
            {
                PlayerMode.Single or PlayerMode.Ghost => new List<FieldPosition>
                {
                    FieldPosition.LeftOuter, 
                    FieldPosition.RightOuter
                },
                PlayerMode.Double or PlayerMode.DoubleFixed => new List<FieldPosition>()
                {
                    FieldPosition.LeftOuter,
                    FieldPosition.RightOuter,
                    FieldPosition.LeftInner,
                    FieldPosition.RightInner
                },
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
    }
}
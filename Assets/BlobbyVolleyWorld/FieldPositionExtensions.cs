namespace BlobbyVolleyWorld.Match
{
    public static class FieldPositionExtensions
    {
        public static bool IsOuter(this FieldPosition position) => 
            position is FieldPosition.LeftOuter or FieldPosition.RightOuter;
        
        public static bool IsInner(this FieldPosition position) =>
            position is FieldPosition.LeftInner or FieldPosition.RightInner;
        
        public static bool IsLeft(this FieldPosition position) =>
            position is FieldPosition.LeftInner or FieldPosition.LeftOuter;
        
        public static bool IsRight(this FieldPosition position) =>
            position is FieldPosition.RightInner or FieldPosition.RightOuter;
        
        public static Side ToSide(this FieldPosition position) =>
            position.IsLeft() ? Side.Left : Side.Right;
    }
}
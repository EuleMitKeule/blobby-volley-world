using UnityEngine;
namespace Blobby.Game.Physics
{
    public class Line
    {
        public Vector2 Support { get; }
        public Vector2 Direction { get; }

        public Line(Vector2 support, Vector2 direction) =>
            (Support, Direction) = (support, direction);
                    
        public Vector2 PositionAt(float lambda) => Support + Direction * lambda;
        
        public Line Translate(Vector2 by) => new Line(Support + by, Direction);

        public Vector2? LineIntersect(Line other)
        {
            if (Direction.Perpendicular().Dot(other.Direction) == 0)
            {
                if (Contains(other.Support)) return Support;
                return null;
            }

            var u = Support.To(other.Support);
            var lambda = other.Direction.Perpendicular().Dot(u) / other.Direction.Perpendicular().Dot(Direction);
            var intersection = PositionAt(lambda);

            return intersection;
        }

        public bool Contains(Vector2 point)
        {
            var b = Support + Direction;
            var perpDot = (Support.x - point.x) * (b.y - point.y) - (Support.y - point.y) * (b.x - point.x);

            return perpDot == 0f;
        }
    }
}
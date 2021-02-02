using UnityEngine;
namespace Blobby.Game.Physics
{
    public struct Circle
    {
        public Vector2 Center { get; }
        public float Radius { get; }
        
        public Circle(Vector2 center, float radius) => (Center, Radius) = (center, radius);

        /// <summary>
        /// Expands the circle
        /// </summary>
        /// <param name="by">The value to add to the radius</param>
        /// <returns>The expanded circle</returns>
        public Circle Expand(float by) => new Circle(Center, Radius + by);
        
        /// <summary>
        /// Circle-Circle intersection
        /// </summary>
        /// <param name="other">The other circle</param>
        /// <returns>Whether the two circles intersect</returns>
        public bool Intersects(Circle other) => Center.To(other.Center).magnitude <= Radius + other.Radius;

        /// <summary>
        /// Line-Circle intersection
        /// </summary>
        /// <param name="line">The line object</param>
        /// <returns>Two intersection points (Vector2, Vector2) if the line is secant,
        /// one intersection point (Vector2, null) if the line is tangent and
        /// no intersection point (null, null) if the line is passersby</returns>
        public (Vector2?, Vector2?) LineIntersect(Line line, float distance = 0f)
        {
            var withDistance = distance != 0f;
            var translatedLine = line.Translate(-Center);
        
            var p = 2 * translatedLine.Support.Dot(translatedLine.Direction);
            var q = translatedLine.Support.Squared() - Radius.Squared();
            
            var (lambda1, lambda2) = PhysicsWorld.SolveQuadratic(p, q);

            if (withDistance)
            {
                if (lambda1.HasValue && lambda2.HasValue)
                {
                    var intersect1 = line.PositionAt(lambda1.Value);
                    var intersect2 = line.PositionAt(lambda2.Value);
                    var isViable1 = line.Support.To(intersect1).magnitude <= distance;
                    var isViable2 = line.Support.To(intersect2).magnitude <= distance;

                    if (isViable1 && isViable2)
                    {
                        return (intersect1, intersect2);
                    }

                    if (isViable1) return (intersect1, null);
                    if (isViable2) return (intersect2, null);

                    return (null, null);
                }

                if (lambda1.HasValue)
                {
                    var intersect1 = line.PositionAt(lambda1.Value);
                    var isViable1 = line.Support.To(intersect1).magnitude <= distance;

                    if (!isViable1) return (null, null);
                    return (intersect1, null);
                }

                if (lambda2.HasValue)
                {
                    var intersect2 = line.PositionAt(lambda2.Value);
                    var isViable2 = line.Support.To(intersect2).magnitude <= distance;

                    if (!isViable2) return (null, null);
                    return (intersect2, null);
                }

                return (null, null);
            }

            if (lambda1.HasValue && lambda2.HasValue)
            {
                var intersect1 = line.PositionAt(lambda1.Value);
                var intersect2 = line.PositionAt(lambda2.Value);
                return (intersect1, intersect2);
            }

            if (lambda1.HasValue)
            {
                var intersect1 = line.PositionAt(lambda1.Value);
                return (intersect1, null);
            }

            if (lambda2.HasValue)
            {
                var intersect2 = line.PositionAt(lambda2.Value);
                return (intersect2, null);
            }

            return (null, null);
        }
    }
}
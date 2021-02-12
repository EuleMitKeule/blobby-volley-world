using Blobby.Game.Physics;
using NUnit.Framework;
using UnityEngine;

namespace BlobbyTests
{
    public class CircleTests
    {
        [Test]
        public void ShouldCreateCircleWithCenterAndRadius()
        {
            var center = new Vector2(10, 5);
            var radius = 3f;
            var circle = new Circle(center, radius);

            Assert.AreEqual(center, circle.Center);
            Assert.AreEqual(radius, circle.Radius);
        }

        [Test]
        public void ShouldExpandCircleRadius()
        {
            Circle circle = A.Circle;
            var by = 2f;

            circle = circle.Expand(by);

            Assert.AreEqual(3, circle.Radius);
        }

        [Test]
        public void ShouldIntersectWithOtherCircle()
        {
            Circle circle1 = A.Circle.WithCenter(3, 0).WithRadius(1.5f);
            Circle circle2 = A.Circle.WithRadius(1.5f);

            var intersecting = circle1.Intersects(circle2);
            Assert.True(intersecting);
            intersecting = circle2.Intersects(circle1);
            Assert.True(intersecting);
        }

        [Test]
        public void ShouldIntersectWithSecant()
        {
            Line line = A.Line;
            Circle circle = A.Circle.WithCenter(5, 0);

            var (pointA, pointB) = circle.LineIntersect(line);

            Assert.NotNull(pointA);
            Assert.NotNull(pointB);

            Assert.AreEqual(new Vector2(4, 0), pointA);
            Assert.AreEqual(new Vector2(6, 0), pointB);
        }

        [Test]
        public void ShouldIntersectWithTangent()
        {
            Line line = A.Line.WithSupport(0, 1);
            Circle circle = A.Circle.WithCenter(5, 0);

            var (pointA, pointB) = circle.LineIntersect(line);

            Assert.NotNull(pointA);
            Assert.Null(pointB);

            Assert.AreEqual(new Vector2(5, 1), pointA);
        }

        [Test]
        public void ShouldNotIntersectWithPasserby()
        {
            Line line = A.Line.WithSupport(0, 2);
            Circle circle = A.Circle.WithCenter(5, 0);

            var (pointA, pointB) = circle.LineIntersect(line);

            Assert.Null(pointA);
            Assert.Null(pointB);
        }

        [Test]
        public void ShouldNotIntersectWithSecantWithLargerDistance()
        {
            var distance = 5f;
            Line line = A.Line;
            Circle circle = A.Circle.WithCenter(5, 0);

            var (pointA, pointB) = circle.LineIntersect(line, distance);

            Assert.NotNull(pointA);
            Assert.Null(pointB);

            Assert.AreEqual(new Vector2(4, 0), pointA.Value);
        }

        [Test]
        public void ShouldNotIntersectWithTangentWithLargerDistance()
        {
            var distance = 2.5f;
            Line line = A.Line.WithSupport(0, 1);
            Circle circle = A.Circle.WithCenter(5, 0);

            var (pointA, pointB) = circle.LineIntersect(line, distance);

            Assert.Null(pointA);
            Assert.Null(pointB);
        }

        [Test]
        public void ShouldAlwaysFail()
        {
            Assert.True(false);
        }
    }
}

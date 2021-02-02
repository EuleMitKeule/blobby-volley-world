using Blobby.Game.Physics;
using NUnit.Framework;
using UnityEngine;

namespace BlobbyTests
{
    public class LineTests
    {
        static object[][] _lineCases =
        {
            new object[]
            {
                new Vector2(3, 0),
                new Vector2(2, 0),
            },
            new object[]
            {
                new Vector2(5, 2),
                new Vector2(-1, 0),
            },
        };

        [TestCaseSource(nameof(_lineCases))]
        public void ShouldCreateLineWithSupportAndNormal(Vector2 support, Vector2 direction)
        {
            Line line = A.Line.WithSupport(support).WithDirection(direction);

            Assert.AreEqual(support, line.Support);
            Assert.AreEqual(direction, line.Direction);
        }

        [TestCaseSource(nameof(_lineCases))]
        public void ShouldCalculatePointOnLineWithLambda(Vector2 support, Vector2 direction)
        {
            Line line = A.Line.WithSupport(support).WithDirection(direction);

            var lambda = 15.6f;
            var point = line.PositionAt(lambda);

            Assert.AreEqual(support + direction * lambda, point);
        }

        [Test]
        public void ShouldCalculateIntersection()
        {
            Line lineA = A.Line;
            Line lineB = A.Line.WithDirection(64, 15);

            var point = lineA.LineIntersect(lineB);

            Assert.NotNull(point);
            Assert.AreEqual(Vector2.zero, point);
        }

        [Test]
        public void ShouldNotIntersectParallel()
        {
            Line lineA = A.Line;
            Line lineB = A.Line.WithSupport(0, 1);

            var point = lineA.LineIntersect(lineB);

            Assert.Null(point);
        }

        [Test]
        public void ShouldIntersectIdentical()
        {
            Line line = A.Line;

            var point = line.LineIntersect(line);

            Assert.AreEqual(line.Support, point);
        }

        [Test]
        public void ShouldDetectPointOnLine()
        {
            Line line = A.Line;
            var point = new Vector2(21, 0);

            var isOnLine = line.Contains(point);

            Assert.True(isOnLine);
        }

        [Test]
        public void ShouldDetectPointNotOnLine()
        {
            Line line = A.Line;
            Line lineB = A.Line.WithSupport(1, 7);

            var isOnLine = line.Contains(lineB.Support);

            Assert.False(isOnLine);
        }
    }
}
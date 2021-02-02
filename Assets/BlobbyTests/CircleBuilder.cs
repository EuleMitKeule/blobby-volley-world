using Blobby.Game.Physics;
using UnityEngine;

namespace BlobbyTests
{
    public class CircleBuilder
    {
        Circle _circle;

        public CircleBuilder()
        {
            _circle = new Circle(Vector2.zero, 1);
        }

        public CircleBuilder WithRadius(float radius)
        {
            _circle = new Circle(_circle.Center, radius);
            return this;
        }

        public CircleBuilder WithCenter(Vector2 center)
        {
            _circle = new Circle(center, _circle.Radius);
            return this;
        }

        public CircleBuilder WithCenter(float x, float y) => WithCenter(new Vector2(x, y));

        public Circle Build() => _circle;

        public static implicit operator Circle(CircleBuilder builder) => builder.Build();
    }
}
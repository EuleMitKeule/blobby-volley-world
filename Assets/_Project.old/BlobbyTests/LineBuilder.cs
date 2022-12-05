using Blobby.Game.Physics;
using UnityEngine;

namespace BlobbyTests
{
    public class LineBuilder
    {
        Line _line;
        public LineBuilder()
        {
            _line = new Line(Vector2.zero, Vector2.right);
        }

        public LineBuilder WithSupport(Vector2 support)
        {
            _line = new Line(support, _line.Direction);
            return this;
        }

        public LineBuilder WithSupport(float x, float y) => WithSupport(new Vector2(x, y));

        public LineBuilder WithDirection(Vector2 direction)
        {
            _line = new Line(_line.Support, direction);
            return this;
        }

        public LineBuilder WithDirection(float x, float y) => WithDirection(new Vector2(x, y));

        public Line Build() => _line;

        public static implicit operator Line(LineBuilder builder) => builder.Build();
    }
}
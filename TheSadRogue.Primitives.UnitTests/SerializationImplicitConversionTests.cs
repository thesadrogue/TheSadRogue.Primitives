using SadRogue.Primitives.SerializedTypes;
using Xunit;

namespace SadRogue.Primitives.UnitTests
{
    public class SerializationImplicitConversionTests
    {
        [Fact]
        public void AreaToAreaSerialized()
        {
            var original = new Area(TestUtils.Enumerable<Point>((1, 2), (3, 4), (5, 6)));
            var expressive = (AreaSerialized)original;
            var converted = (Area)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void BoundedRectangleToBoundedRectangleSerialized()
        {
            var original = new BoundedRectangle((1, 2, 9, 11), (-1, -2, 100, 101));
            var expressive = (BoundedRectangleSerialized)original;
            var converted = (BoundedRectangle)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void ColorToColorSerialized()
        {
            var original = new Color(23, 25, 27, 67);
            var expressive = (ColorSerialized)original;
            var converted = (Color)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void GradientToGradientSerialized()
        {
            var original = new Gradient(TestUtils.Enumerable(Color.AliceBlue, Color.Black, Color.Red),
                                         TestUtils.Enumerable(0.0f, 0.5f, 1.0f));
            var expressive = (GradientSerialized)original;
            var converted = (Gradient)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void PaletteToPaletteSerialized()
        {
            var original = new Palette(TestUtils.Enumerable(Color.AliceBlue, Color.Red, Color.Black, Color.Yellow));
            var expressive = (PaletteSerialized)original;
            var converted = (Palette)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void PointToPointSerialized()
        {
            var original = new Point(1, 2);
            var expressive = (PointSerialized)original;
            var converted = (Point)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void RectangleToRectangleSerialized()
        {
            var original = new Rectangle(1, 2, 34, 20);
            var expressive = (RectangleSerialized)original;
            var converted = (Rectangle)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void AdjacencyRuleToType()
        {
            var original = AdjacencyRule.Diagonals;
            var expressive = (AdjacencyRule.Types)original;
            var converted = (AdjacencyRule)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void DirectionToType()
        {
            var original = Direction.None;
            var expressive = (Direction.Types)original;
            var converted = (Direction)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void DistanceToType()
        {
            var original = Distance.Euclidean;
            var expressive = (Distance.Types)original;
            var converted = (Distance)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void RadiusToType()
        {
            var original = Radius.Circle;
            var expressive = (Radius.Types)original;
            var converted = (Radius)expressive;

            Assert.Equal(original, converted);
        }
    }
}

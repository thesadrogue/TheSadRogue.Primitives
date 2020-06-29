using SadRogue.Primitives.SerializedTypes;
using Xunit;

namespace SadRogue.Primitives.UnitTests.Serialization
{
    public class ImplicitConversionTests
    {
        [Fact]
        public void AreaToAreaSerialized()
        {
            var original = new Area(TestUtils.Enumerable<Point>((1, 2), (3, 4), (5, 6)));
            AreaSerialized expressive = (AreaSerialized)original;
            var converted = (Area)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void BoundedRectangleToBoundedRectangleSerialized()
        {
            var original = new BoundedRectangle((1, 2, 9, 11), (-1, -2, 100, 101));
            BoundedRectangleSerialized expressive = (BoundedRectangleSerialized)original;
            var converted = (BoundedRectangle)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void ColorToColorSerialized()
        {
            Color original = new Color(23, 25, 27, 67);
            ColorSerialized expressive = (ColorSerialized)original;
            Color converted = (Color)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void GradientToGradientSerialized()
        {
            var original = new Gradient(TestUtils.Enumerable(Color.AliceBlue, Color.Black, Color.Red),
                TestUtils.Enumerable(0.0f, 0.5f, 1.0f));
            GradientSerialized expressive = (GradientSerialized)original;
            var converted = (Gradient)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void PaletteToPaletteSerialized()
        {
            var original = new Palette(TestUtils.Enumerable(Color.AliceBlue, Color.Red, Color.Black, Color.Yellow));
            PaletteSerialized expressive = (PaletteSerialized)original;
            var converted = (Palette)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void PointToPointSerialized()
        {
            Point original = new Point(1, 2);
            PointSerialized expressive = (PointSerialized)original;
            Point converted = (Point)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void RectangleToRectangleSerialized()
        {
            Rectangle original = new Rectangle(1, 2, 34, 20);
            RectangleSerialized expressive = (RectangleSerialized)original;
            Rectangle converted = (Rectangle)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void AdjacencyRuleToType()
        {
            AdjacencyRule original = AdjacencyRule.Diagonals;
            AdjacencyRule.Types expressive = (AdjacencyRule.Types)original;
            AdjacencyRule converted = (AdjacencyRule)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void DirectionToType()
        {
            Direction original = Direction.None;
            Direction.Types expressive = (Direction.Types)original;
            Direction converted = (Direction)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void DistanceToType()
        {
            Distance original = Distance.Euclidean;
            Distance.Types expressive = (Distance.Types)original;
            Distance converted = (Distance)expressive;

            Assert.Equal(original, converted);
        }

        [Fact]
        public void RadiusToType()
        {
            Radius original = Radius.Circle;
            Radius.Types expressive = (Radius.Types)original;
            Radius converted = (Radius)expressive;

            Assert.Equal(original, converted);
        }
    }
}

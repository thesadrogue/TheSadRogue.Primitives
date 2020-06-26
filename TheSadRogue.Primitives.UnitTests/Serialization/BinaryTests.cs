using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SadRogue.Primitives.SerializedTypes;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests.Serialization
{
    public class BinaryTests
    {
        public static IEnumerable<object> ExpressiveTypes = new object[]
        {
            new AreaSerialized()
            {
                Positions = new List<PointSerialized>()
                {
                    new PointSerialized() { X = 1, Y = 2 },
                    new PointSerialized() { X = 6, Y = 7 },
                    new PointSerialized() { X = 10, Y = 2 }
                }
            },
            new BoundedRectangleSerialized()
            {
                Area = new RectangleSerialized() { X = 1, Y = 2, Width = 45, Height = 50 },
                Bounds = new RectangleSerialized() { X = 0, Y = -1, Width = 100, Height = 101 }
            },
            new ColorSerialized() { R = 12, G = 16, B = 45, A = 255 },
            new GradientStopSerialized()
            {
                Color = new ColorSerialized() { R = 10, G = 20, B = 30, A = 40 },
                Stop = 0.5f
            },
            new GradientSerialized()
            {
                Stops = new List<GradientStopSerialized>()
                {
                    new GradientStopSerialized()
                    {
                        Stop = 0f,
                        Color = new ColorSerialized() { R = 156, G = 24, B = 97, A = 250 }
                    },
                    new GradientStopSerialized()
                    {
                        Stop = 1f,
                        Color = new ColorSerialized() { R = 100, G = 200, B = 50, A = 25 }
                    }
                }
            },
            new PaletteSerialized()
            {
                Colors = new List<ColorSerialized>()
                {
                    new ColorSerialized() { R = 10, G = 34, B = 255, A = 100 },
                    new ColorSerialized() { R = 67, G = 87, B = 98, A = 156 }
                }
            },
            new PointSerialized() { X = 42, Y = 24 },
            new RectangleSerialized() { X = 42, Y = 24, Width = 50, Height =  100 },
            AdjacencyRule.Types.Cardinals, AdjacencyRule.Types.Diagonals, AdjacencyRule.Types.EightWay,
            Direction.Types.Up, Direction.Types.Left, Direction.Types.DownRight,
            Distance.Types.Euclidean, Distance.Types.Chebyshev, Distance.Types.Manhattan,
            Radius.Types.Square, Radius.Types.Circle, Radius.Types.Diamond
        };

        [Theory]
        [MemberDataEnumerable(nameof(ExpressiveTypes))]
        public void SerializeToDeserializeEquivalence(object objToSerialize)
        {
            Func<object, object, bool> equalityFunc = Comparisons.GetComparisonFunc(objToSerialize);

            string name = $"{objToSerialize.GetType().FullName}.bin";

            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(name, FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(stream, objToSerialize);
            }

            object reSerialized;
            using (var stream = new FileStream(name, FileMode.Open, FileAccess.Read))
                reSerialized = formatter.Deserialize(stream);

            File.Delete(name);
            Assert.True(equalityFunc(objToSerialize, reSerialized));
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SadRogue.Primitives;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    public class SerializationTests
    {
        public static IEnumerable<object> SerializeData = new object[] {
            AdjacencyRule.Cardinals, AdjacencyRule.EightWay,
            CreateArea((1, 2), (3, 4), (5,6)),
            new BoundedRectangle(new Rectangle(1, 4, 10, 14), new Rectangle(-10, -9, 100, 101)),
            new Color(.5f, .6f, .7f), new Color(120, 121, 122, 100), Color.AliceBlue,
            Direction.Down, Direction.UpRight,
            Distance.Chebyshev, Distance.Manhattan,
            new GradientStop(new Color(100, 101, 102, 103), .5f),
            new Point(-1, -5), new Point(4, 9),
            Radius.Circle, Radius.Diamond,
            new Rectangle(1, 2, 3, 4), new Rectangle(-10, -4, 56, 68)
        };

        public static IEnumerable<(object, Func<object, object, bool>)> CustomSerializeData = new (object, Func<object, object, bool>)[] {
            (new Gradient(new Color(100, 101, 102, 103), new Color(200, 201, 202, 203)), GradientCompare),
            (new Palette(new Color[] {new Color(100, 101, 102, 103), new Color(150, 151, 152, 149)}), PaletteCompare)
        };

        [Theory]
        [MemberDataTuple(nameof(CustomSerializeData))]
        public void TestSerializationCustomEquality(object objToSerialize, Func<object, object, bool> equality)
        {
            string name = $"{objToSerialize.GetType().FullName}.bin";

            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(name, FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(stream, objToSerialize);
            }

            object reSerialized = default;
            using (var stream = new FileStream(name, FileMode.Open, FileAccess.Read))
            {
                reSerialized = formatter.Deserialize(stream);
            }

            File.Delete(name);
            Assert.True(equality(objToSerialize, reSerialized));
        }

        [Theory]
        [MemberDataEnumerable(nameof(SerializeData))]
        public void TestSerialization(object objToSerialize) => TestSerializationCustomEquality(objToSerialize, (t1, t2) => t1.Equals(t2));

        private static Area CreateArea(params Point[] points)
        {
            var area = new Area();
            area.Add(points);
            return area;
        }

        private static bool GradientCompare(object o1, object o2)
        {
            Gradient g1 = (Gradient)o1;
            Gradient g2 = (Gradient)o2;

            if (g1.Stops.Length != g2.Stops.Length)
            {
                return false;
            }

            for (int i = 0; i < g1.Stops.Length; i++)
            {
                if (g1.Stops[i] != g2.Stops[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static bool PaletteCompare(object o1, object o2)
        {
            Palette p1 = (Palette)o1;
            Palette p2 = (Palette)o2;

            if (p1.Length != p2.Length)
            {
                return false;
            }

            for (int i = 0; i < p1.Length; i++)
            {
                if (p1[i] != p2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}

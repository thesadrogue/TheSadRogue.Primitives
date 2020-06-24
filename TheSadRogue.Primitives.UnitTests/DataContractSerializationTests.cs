using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SadRogue.Primitives.SerializedTypes;
using Xunit;
using Xunit.Abstractions;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    public class DataContractSerializationTests
    {
        #region Test Data
        // Types that serialize to JSON objects via JSON .NET, so we can check which fields are serialized.
        public static IEnumerable<object> SerializableValuesJsonObjects = new object[]
        {
            // AdjacencyRules
            AdjacencyRule.Cardinals, AdjacencyRule.EightWay,
            // AreaSerialized
            (AreaSerialized)CreateArea((1, 2), (3, 4), (5,6)),
            // BoundedRectangle
            new BoundedRectangle(new Rectangle(1, 4, 10, 14), new Rectangle(-10, -9, 100, 101)),
            // BoundedRectangleSerialized
            (BoundedRectangleSerialized)new BoundedRectangle(new Rectangle(1, 4, 10, 14), new Rectangle(-10, -9, 100, 101)),
            // Colors
            new Color(.5f, .6f, .7f), new Color(120, 121, 122, 100), Color.AliceBlue,
            // ColorSerialized
            (ColorSerialized)new Color(.5f, .6f, .7f), (ColorSerialized)new Color(120, 121, 122, 100), (ColorSerialized)Color.AliceBlue,
            // Directions
            Direction.Down, Direction.UpRight,
            // Distances
            Distance.Chebyshev, Distance.Manhattan,
            // GradientStop
            new GradientStop(new Color(100, 101, 102, 103), .5f),
            // GradientStopSerialized
            (GradientStopSerialized)new GradientStop(new Color(100, 101, 102, 103), .5f),
            // GradientSerialized
            (GradientSerialized)new Gradient(new Color(100, 101, 102, 103), new Color(200, 201, 202, 203)),
            // PaletteSerialized
            (PaletteSerialized)new Palette(new[] {new Color(100, 101, 102, 103), new Color(150, 151, 152, 149)}),
            // Point
            new Point(-1, -5), new Point(4, 9),
            // PointSerialized
            (PointSerialized)new Point(-1, -5), (PointSerialized)new Point(4, 9),
            // Radiuses
            Radius.Circle, Radius.Diamond,
            // Rectangles
            new Rectangle(1, 2, 3, 4), new Rectangle(-10, -4, 56, 68),
            // RectangleSerialized
            (RectangleSerialized)new Rectangle(1, 2, 3, 4), (RectangleSerialized)new Rectangle(-10, -4, 56, 68)
        };

        // Types that serialize to non-JSON objects (for example, JSON arrays), so we do NOT check for specific fields.
        public static IEnumerable<object> SerializableValuesNonJsonObjects = new object[]
        {
            // AdjacencyRule.Types
            AdjacencyRule.Types.Cardinals, AdjacencyRule.Types.Diagonals,
            // Area
            CreateArea((1, 2), (3, 4), (5,6)),
            // Direction.Types
            Direction.Types.Down, Direction.Types.Right,
            // Distance.Types
            Distance.Types.Chebyshev, Distance.Types.Euclidean,
            // Gradient
            new Gradient(new Color(100, 101, 102, 103), new Color(200, 201, 202, 203)),
            // Palette
            new Palette(new[] {new Color(100, 101, 102, 103), new Color(150, 151, 152, 149)}),
            // Radius.Types
            Radius.Types.Square, Radius.Types.Circle
        };

        // All JSON objects for which we can test serialization equality
        public static IEnumerable<object> AllSerializableObjects =
            SerializableValuesJsonObjects.Concat(SerializableValuesNonJsonObjects);

        // Dictionary of object types to an unordered but complete list of fields that each object type should have serialized
        // in its JSON object form.  
        private static Dictionary<Type, string[]> typeSerializedFields = new Dictionary<Type, string[]>
        {
            { typeof(AdjacencyRule),              new [] { "Type" } },
            { typeof(AreaSerialized),             new [] { "Positions" } },
            { typeof(BoundedRectangle),           new [] { "_area", "_boundingBox" } },
            { typeof(BoundedRectangleSerialized), new [] { "Area", "Bounds"} },
            { typeof(Color),                      new [] { "_packedValue" } },
            { typeof(ColorSerialized),            new [] {"R", "G", "B", "A" } },
            { typeof(Direction),                  new [] { "Type" } },
            { typeof(Distance),                   new [] { "Type" } },
            { typeof(GradientStop),               new [] { "Color", "Stop" } },
            { typeof(GradientStopSerialized),     new [] { "Color", "Stop" } },
            { typeof(GradientSerialized),         new [] { "Stops" } },
            { typeof(PaletteSerialized),          new [] { "Colors" } },
            { typeof(Point),                      new [] { "X", "Y" } },
            { typeof(PointSerialized),            new [] { "X", "Y" } },
            { typeof(Radius),                     new [] { "Type" } },
            { typeof(Rectangle),                  new [] { "X", "Y", "Width", "Height" } },
            { typeof(RectangleSerialized),        new [] { "X", "Y", "Width", "Height" } }
        };

        // Dictionary of object types mapping them to custom methods to use in order to determine equality.
        private static Dictionary<Type, Func<object, object, bool>> equalityMethods = new Dictionary<Type, Func<object, object, bool>>()
        {
            { typeof(AreaSerialized), AreaSerializedCompare },
            { typeof(Gradient), GradientCompare },
            { typeof(Palette), PaletteCompare },
            { typeof(GradientSerialized), GradientSerializedCompare },
            { typeof(PaletteSerialized), PaletteSerializedCompare }
        };
        #endregion

        // Useful for viewing output
        private readonly ITestOutputHelper output;

        public DataContractSerializationTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        /// <summary>
        /// Tests that if we serialize an object to JSON, then deserialize it back to an object,
        /// that the deserialized object is equal to the one that we serialized.  If a custom equality method
        /// has been specified in <see cref="equalityMethods"/>, that method is used to determine equality.
        /// Otherwise, the object's <see cref="object.Equals(object?)"/> method is used.
        /// </summary>
        /// <param name="objToSerialize"/>
        [Theory]
        [MemberDataEnumerable(nameof(AllSerializableObjects))]
        public void SerializeToDeserializeEquivalence(object objToSerialize)
        {
            var objType = objToSerialize.GetType();
            output.WriteLine($"Type is: {objType.Name}");

            // Set equality to custom comparer if we have one, otherwise default to .Equals
            Func<object, object, bool> equality =
                equalityMethods.GetValueOrDefault(objType, (o1, o2) => o1.Equals(o2));

            // Serialize to JSON string
            string json = JsonConvert.SerializeObject(objToSerialize);

            output.WriteLine($"Json: {json}");

            // Deserialize to object
            object deSerialized = JsonConvert.DeserializeObject(json, objType);

            // Make sure the deserialized object is equivalent to the one we serialized
            Assert.True(equality(objToSerialize, deSerialized));

        }

        /// <summary>
        /// For objects that are serialized to JSON objects, tests that the fields in the serialized JSON
        /// are exactly the fields that are expected to be.  This ensures that no fields are being serialized unintentionally.
        /// </summary>
        /// <param name="objToSerialize"/>
        [Theory]
        [MemberDataEnumerable(nameof(SerializableValuesJsonObjects))]
        public void ExpectedFieldsSerialized(object objToSerialize)
        {
            // Serialize to JSON string
            string json = JsonConvert.SerializeObject(objToSerialize);

            // Get fields in hash set
            var fields = JObject.Parse(json).Properties().Select(i => i.Name).ToHashSet();

            // Make hash set from specified fields that _should_ be there
            var expectedFields = typeSerializedFields[objToSerialize.GetType()].ToHashSet();

            // Ensure expected fields are what we got (in arbitrary order)
            Assert.Equal(expectedFields, fields);
        }

        // Creates an area
        private static Area CreateArea(params Point[] points)
        {
            var area = new Area();
            area.Add(points);
            return area;
        }

        // Compares to AreaSerialized instances
        private static bool AreaSerializedCompare(object o1, object o2)
            => ElementWiseEquality(((AreaSerialized)o1).Positions, ((AreaSerialized)o2).Positions);

        // Compares two gradients
        private static bool GradientCompare(object o1, object o2)
            => ElementWiseEquality(((Gradient)o1).Stops, ((Gradient)o2).Stops);

        // Compares two palettes
        private static bool PaletteCompare(object o1, object o2)
        {
            Palette p1 = (Palette)o1;
            Palette p2 = (Palette)o2;

            if (p1.Length != p2.Length)
                return false;

            for (int i = 0; i < p1.Length; i++)
            {
                if (p1[i] != p2[i])
                    return false;
            }

            return true;
        }

        private static bool PaletteSerializedCompare(object o1, object o2)
            => ElementWiseEquality(((PaletteSerialized)o1).Colors, ((PaletteSerialized)o2).Colors);

        private static bool GradientSerializedCompare(object o1, object o2)
            => ElementWiseEquality(((GradientSerialized)o1).Stops, ((GradientSerialized)o2).Stops);

        private static bool ElementWiseEquality<T>(IEnumerable<T> e1, IEnumerable<T> e2, Func<T, T, bool> compareFunc = null)
        {
            compareFunc ??= (o1, o2) => o1.Equals(o2);

            var l1 = e1.ToList();
            var l2 = e2.ToList();

            if (l1.Count != l2.Count)
                return false;

            for (int i = 0; i < l1.Count; i++)
                if (!compareFunc(l1[i], l2[i]))
                    return false;

            return true;
        }
    }
}

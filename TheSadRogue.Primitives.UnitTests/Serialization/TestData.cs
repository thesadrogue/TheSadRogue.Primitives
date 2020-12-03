using System;
using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.SerializedTypes;
using SadRogue.Primitives.SerializedTypes.GridViews;
using SadRogue.Primitives.UnitTests.Mocks;

namespace SadRogue.Primitives.UnitTests.Serialization
{
    public static class TestData
    {
        #region Original Data
        /// <summary>
        /// List of expressive versions of types.  The assumptions here are:
        ///     1. All types in this list are serializable via generic data contract serialization and binary
        ///        serialization.
        ///     2. All types in this list serialize to JSON objects (JObject) when using Newtonsoft.Json
        /// </summary>
        private static readonly IEnumerable<object> _expressiveTypes = new object[]
        {
            // ArrayViewSerialized
            new ArrayViewSerialized<int>
            {
                Width = 2,
                Data = new []{ 1, 2, 3, 4 }
            },
            // AreaSerialized
            new AreaSerialized
            {
                Positions = new List<PointSerialized>
                {
                    new PointSerialized { X = 1, Y = 2 },
                    new PointSerialized { X = 3, Y = 4 },
                    new PointSerialized { X = 5, Y = 6 }
                }
            },
            // BoundedRectangleSerialized
            new BoundedRectangleSerialized
            {
                Area = new RectangleSerialized {X = 1, Y = 4, Width = 10, Height = 14},
                Bounds = new RectangleSerialized {X = -10, Y = -1, Width = 100, Height = 101 }
            },
            // ColorSerialized
            new ColorSerialized  { R = 120, G = 121, B = 122, A = 150 },
            // DiffAwareGridViewSerialized
            /*
            new DiffAwareGridViewSerialized<int>
            {
                AutoCompress = true,
                BaseGrid = new ArrayView<int>(new[] { 1, 2, 3, 4 }, 2),
                CurrentDiffIndex = 1,
                Diffs = new List<DiffSerialized<int>>
                {
                    new DiffSerialized<int>
                    {
                        Changes = new List<ValueChangeSerialized<int>>
                        {
                            new ValueChangeSerialized<int>
                            {
                                Position = new PointSerialized{ X = 1, Y = 1 },
                                OldValue = 0,
                                NewValue = 3
                            }
                        }
                    }
                }
            },
            */
            // DiffSerialized
            new DiffSerialized<int>
            {
                Changes = new List<ValueChangeSerialized<int>>
                {
                    new ValueChangeSerialized<int>
                    {
                        Position = new PointSerialized{ X = 1, Y = 2 },
                        OldValue = 1,
                        NewValue = 2,
                    },
                    new ValueChangeSerialized<int>
                    {
                        Position = new PointSerialized{ X = 1, Y = 2 },
                        OldValue = 2,
                        NewValue = 3,
                    },
                    new ValueChangeSerialized<int>
                    {
                        Position = new PointSerialized{ X = 5, Y = 6 },
                        OldValue = 7,
                        NewValue = 9,
                    },
                    new ValueChangeSerialized<int>
                    {
                        Position = new PointSerialized{ X = 5, Y = 6 },
                        OldValue = 9,
                        NewValue = 8,
                    }
                }
            },
            // GradientStopSerialized
            new GradientStopSerialized
            {
                Color = new ColorSerialized { R = 100, G = 101, B = 102, A = 103 },
                Stop = 0.5f
            },
            // GradientSerialized
            new GradientSerialized
            {
                Stops = new List<GradientStopSerialized>
                {
                    new GradientStopSerialized
                    {
                        Color = new ColorSerialized { R = 100, G = 101, B = 102, A = 103 },
                        Stop = 0.5f
                    },
                    new GradientStopSerialized
                    {
                        Color = new ColorSerialized {R = 200, G = 201, B = 202, A = 203 },
                        Stop = 1.0f
                    }
                }
            },
            // PaletteSerialized
            new PaletteSerialized
            {
                Colors = new List<ColorSerialized>
                {
                    new ColorSerialized { R = 100, G = 101, B = 102, A = 103 },
                    new ColorSerialized {R = 200, G = 201, B = 202, A = 203 }
                }
            },
            // PointSerialized
            new PointSerialized { X = 10, Y = 20 },
            // PolarCoordinateSerialized
            new PolarCoordinateSerialized { Radius = 5.0, Theta = 3 * Math.PI / 2.0 },
            // RectangleSerialized
            new RectangleSerialized { X = 10, Y = 20, Width = 100, Height = 200 },
            // ValueChangeSerialized
            new ValueChangeSerialized<int> { Position = new PointSerialized {X = 1, Y = 2}, OldValue = 1, NewValue = 2 }
        };

        /// <summary>
        /// Any expressive types (ones that implicitly convert for the sake of serialization), that don't serialize
        /// to JSON Object (generally instead serialize to primitive types like integers).
        /// </summary>
        private static readonly IEnumerable<object> _expressivePrimitiveTypes = new object[]
        {
            // AdjacencyRule.Types
            AdjacencyRule.Types.Cardinals, AdjacencyRule.Types.Diagonals,
            // Direction.Types
            Direction.Types.Down, Direction.Types.Right,
            // Distance.Types
            Distance.Types.Chebyshev, Distance.Types.Euclidean,
            // Radius.Types
            Radius.Types.Square, Radius.Types.Circle,
        };

        /// <summary>
        /// List of all non-expressive types that serialize to JSON objects (JObject)
        /// </summary>
        private static readonly object[] _nonExpressiveJsonObjects =
        {
            // AdjacencyRules
            AdjacencyRule.Cardinals, AdjacencyRule.EightWay,
            // BoundedRectangle
            new BoundedRectangle(new Rectangle(1, 4, 10, 14), new Rectangle(-10, -9, 100, 101)),
            // Colors
            new Color(.5f, .6f, .7f), new Color(120, 121, 122, 100), Color.AliceBlue,
            // Directions
            Direction.Down, Direction.UpRight,
            // Distances
            Distance.Chebyshev, Distance.Manhattan,
            // GradientStop
            new GradientStop(new Color(100, 101, 102, 103), .5f),
            // Points
            new Point(-1, -5), new Point(4, 9),
            // Polar Coordinates
            new PolarCoordinate(10.0, 0.25), new PolarCoordinate(5.0, 3 * Math.PI / 2.0),
            // Radii
            Radius.Circle, Radius.Diamond,
            // Rectangles
            new Rectangle(1, 2, 3, 4), new Rectangle(-10, -4, 56, 68),
            // ValueChange
            new ValueChange<int>((1, 2), 1, 2)
        };

        /// <summary>
        /// Dictionary of object types to an unordered but complete list of fields that each object type should have
        /// serialized in its JSON object form.  All objects in SerializableValuesJsonObjects should have an entry here.
        /// </summary>
        public static readonly Dictionary<Type, string[]> TypeSerializedFields = new Dictionary<Type, string[]>
        {
            { typeof(AdjacencyRule), new[] { "Type" } },
            { typeof(ArrayViewSerialized<int>), new[] { "Width", "Data" } },
            { typeof(AreaSerialized), new[] { "Positions" } },
            { typeof(BoundedRectangle), new[] { "_area", "_boundingBox" } },
            { typeof(BoundedRectangleSerialized), new[] { "Area", "Bounds" } },
            { typeof(Color), new[] { "_packedValue" } },
            { typeof(ColorSerialized), new[] { "R", "G", "B", "A" } },
            // { typeof(DiffAwareGridViewSerialized<int>), new[] { "AutoCompress", "Diffs", "BaseGrid", "CurrentDiffIndex" } },
            { typeof(DiffSerialized<int>), new[] { "Changes" } },
            { typeof(Direction), new[] { "Type" } },
            { typeof(Distance), new[] { "Type" } },
            { typeof(GradientStop), new[] { "Color", "Stop" } },
            { typeof(GradientStopSerialized), new[] { "Color", "Stop" } },
            { typeof(GradientSerialized), new[] { "Stops" } },
            { typeof(PaletteSerialized), new[] { "Colors" } },
            { typeof(Point), new[] { "X", "Y" } },
            { typeof(PointSerialized), new[] { "X", "Y" } },
            { typeof(PolarCoordinate), new[] { "Radius", "Theta" } },
            { typeof(PolarCoordinateSerialized), new[] { "Radius", "Theta" } },
            { typeof(Radius), new[] { "Type" } },
            { typeof(Rectangle), new[] { "X", "Y", "Width", "Height" } },
            { typeof(RectangleSerialized), new[] { "X", "Y", "Width", "Height" } },
            { typeof(ValueChange<int>), new[] { "Position", "OldValue", "NewValue" } },
            { typeof(ValueChangeSerialized<int>), new[] { "Position", "OldValue", "NewValue" } }
        };

        /// <summary>
        /// Objects that are JSON serializable but should NOT serialize to JSON objects (instead, for example,
        /// JSON array)
        /// </summary>
        public static readonly IEnumerable<object> SerializableValuesNonJsonObjects = new object[]
        {
            // Area
            new Area((1, 2), (3, 4), (5, 6)),
            // Gradient
            new Gradient(new Color(100, 101, 102, 103), new Color(200, 201, 202, 203)),
            // Palette
            new Palette(new[] { new Color(100, 101, 102, 103), new Color(150, 151, 152, 149) }),
        };


        /// <summary>
        /// Dictionary of non-expressive types to their expressive type
        /// </summary>
        public static readonly Dictionary<Type, Type> RegularToExpressiveTypes = new Dictionary<Type, Type>
        {
            [typeof(ArrayView2D<int>)] = typeof(int[,]),
            [typeof(ArrayView<int>)] = typeof(ArrayViewSerialized<int>),
            [typeof(AdjacencyRule)] = typeof(AdjacencyRule.Types),
            [typeof(Area)] = typeof(AreaSerialized),
            [typeof(BoundedRectangle)] = typeof(BoundedRectangleSerialized),
            [typeof(Color)] = typeof(ColorSerialized),
            // [typeof(DiffAwareGridView<int>)] = typeof(DiffAwareGridViewSerialized<int>),
            [typeof(Diff<int>)] = typeof(DiffSerialized<int>),
            [typeof(Direction)] = typeof(Direction.Types),
            [typeof(Distance)] = typeof(Distance.Types),
            [typeof(GradientStop)] = typeof(GradientStopSerialized),
            [typeof(Gradient)] = typeof(GradientSerialized),
            [typeof(Palette)] = typeof(PaletteSerialized),
            [typeof(Point)] = typeof(PointSerialized),
            [typeof(PolarCoordinate)] = typeof(PolarCoordinateSerialized),
            [typeof(Radius)] = typeof(Radius.Types),
            [typeof(Rectangle)] = typeof(RectangleSerialized),
            [typeof(ValueChange<int>)] = typeof(ValueChangeSerialized<int>)
        };


        /// <summary>
        /// List of non-serializable objects that do have serializable equivalents (expressive types).
        /// </summary>
        private static readonly object[] _nonSerializableValuesWithExpressiveTypes =
        {
            // ArrayView
            new ArrayView<int>(new[] { 1, 2, 3, 4 }, 2),
            // ArrayView2D
            MockGridViews.RectangleArrayView2D(50, 40),
            // Diff
            new Diff<int>
            {
                new ValueChange<int>((1, 2), 1, 2),
                new ValueChange<int>((1, 2), 2, 3),
                new ValueChange<int>((5, 6), 7, 9),
                new ValueChange<int>((5, 6), 9, 8)
            },
            // DiffAwareGridView
            // GenerateDiffAwareGridView()
        };
        #endregion

        #region Combinatory Data

        /// <summary>
        /// All types that should serialize with binary serializers.
        /// </summary>
        public static IEnumerable<object> BinarySerializableTypes => _expressiveTypes.Concat(_expressivePrimitiveTypes);

        /// <summary>
        /// All objects that should serialize to JSON objects.  All should have entries in TypeSerializedFields
        /// </summary>
        public static IEnumerable<object> SerializableValuesJsonObjects
            => _expressiveTypes.Concat(_nonExpressiveJsonObjects);

        /// <summary>
        /// Objects that should have expressive versions of types.  Each item must have an entry in
        /// RegularToExpressiveTypes
        /// </summary>
        public static IEnumerable<object> AllNonExpressiveTypes
            => _nonExpressiveJsonObjects.Concat(SerializableValuesNonJsonObjects).Concat(_nonSerializableValuesWithExpressiveTypes);

        /// <summary>
        /// All JSON objects for which we can test serialization equality
        /// </summary>
        public static IEnumerable<object> AllSerializableObjects =>
            SerializableValuesJsonObjects.Concat(SerializableValuesNonJsonObjects);
        #endregion

        /*
        #region Instance Generation Helpers

        public static DiffAwareGridView<int> GenerateDiffAwareGridView()
        {
            var diffView = new DiffAwareGridView<int>(10, 10);
            diffView[1, 2] = 10;
            diffView[5, 6] = 12;
            diffView.FinalizeCurrentDiff();

            diffView[1, 2] = 5;
            diffView[5, 6] = 7;
            diffView[9, 8] = 9;
            diffView.FinalizeCurrentDiff();

            return diffView;
        }
        #endregion
        */
    }
}

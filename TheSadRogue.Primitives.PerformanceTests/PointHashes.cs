using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests
{
    public class CustomHashComparer : IEqualityComparer<Point>
    {
        private Func<Point, int> _hashFunc;

        public CustomHashComparer(Func<Point, int> hashFunc)
        {
            _hashFunc = hashFunc;
        }

        public bool Equals(Point x, Point y) => x.Equals(y);

        public int GetHashCode(Point obj) => _hashFunc(obj);
    }

    public class PointHashes
    {
        [Params(10, 50, 100, 175, 256)]
        public int Size;

        // Initialized by global setup so nullability is suppressed
        private Point[] _points = null!;
        private Dictionary<Point, int> _getDictCurrentPrimitives = null!;
        private Dictionary<Point, int> _getDictOldGoRogue = null!;
        private Dictionary<Point, int> _getDictRosenbergStrong = null!;
        private Dictionary<Point, int> _getDictRosenbergStrongOneLess = null!;

        // Executed once per value of parameters
        [GlobalSetup]
        public void GlobalSetup()
        {
            // Create array of appropriate size and populate
            _points = new Point[Size * Size];
            for (int i = 0; i < _points.Length; i++)
                _points[i] = Point.FromIndex(i, Size);

            // Create dictionaries with appropriate values for use with the get tests
            _getDictCurrentPrimitives = new Dictionary<Point, int>(new CustomHashComparer(CurrentPrimitivesHash));
            _getDictOldGoRogue = new Dictionary<Point, int>(new CustomHashComparer(OldGoRogueHash));
            _getDictRosenbergStrong = new Dictionary<Point, int>(new CustomHashComparer(RosenbergStrongBasedHash));
            _getDictRosenbergStrongOneLess = new Dictionary<Point, int>(new CustomHashComparer(RosenbergStrongBasedOneLessMultHash));
            foreach (var p in _points)
            {
                int value = p.ToIndex(Size);
                _getDictCurrentPrimitives[p] = value;
                _getDictOldGoRogue[p] = value;
                _getDictRosenbergStrong[p] = value;
                _getDictRosenbergStrongOneLess[p] = value;
            }
        }

        #region TimeToHash

        [Benchmark]
        public int SumPrimitivesHashing() => SumHashesAlgorithm(CurrentPrimitivesHash);

        [Benchmark]
        public int SumOldGoRogueAlgorithm() => SumHashesAlgorithm(OldGoRogueHash);

        [Benchmark]
        public int SumRosenbergStrongBasedAlgorithm() => SumHashesAlgorithm(RosenbergStrongBasedHash);

        [Benchmark]
        public int SumRosenbergStrongBasedOneLessMultAlgorithm() => SumHashesAlgorithm(RosenbergStrongBasedOneLessMultHash);

        private int SumHashesAlgorithm(Func<Point, int> hashingAlgo)
        {
            int sum = 0;
            for (int i = 0; i < _points.Length; i++)
                sum += hashingAlgo(_points[i]);

            return sum;
        }
        #endregion

        #region AddToDictionary

        [Benchmark]
        public Dictionary<Point, int> PrimitivesHashingAdd() => AddToDict(CurrentPrimitivesHash);

        [Benchmark]
        public Dictionary<Point, int> OldGoRogueHashingAdd() => AddToDict(OldGoRogueHash);

        [Benchmark]
        public Dictionary<Point, int> RosenbergStrongBasedAdd() => AddToDict(RosenbergStrongBasedHash);

        [Benchmark]
        public Dictionary<Point, int> RosenbergStrongBasedOneLessAdd() => AddToDict(RosenbergStrongBasedOneLessMultHash);

        private Dictionary<Point, int> AddToDict(Func<Point, int> hashFunc)
        {
            var dict = new Dictionary<Point, int>(new CustomHashComparer(hashFunc));
            for (int i = 0; i < _points.Length; i++)
                dict[_points[i]] = i;

            return dict;
        }
        #endregion

        #region GetFromDictionary
        [Benchmark]
        public int PrimitivesHashingGet() => GetFromDict(_getDictCurrentPrimitives);

        [Benchmark]
        public int OldGoRogueHashingGet() => GetFromDict(_getDictOldGoRogue);

        [Benchmark]
        public int RosenbergStrongBasedGet() => GetFromDict(_getDictRosenbergStrong);

        [Benchmark]
        public int RosenbergStrongBasedOneLessMultGet() => GetFromDict(_getDictRosenbergStrongOneLess);

        private int GetFromDict(Dictionary<Point, int> dict)
        {
            int sum = 0;
            for (int i = 0; i < _points.Length; i++)
                sum += dict[_points[i]];

            return sum;
        }
        #endregion

        #region Hashing Algorithms
        private int CurrentPrimitivesHash(Point p) => p.GetHashCode();
        private int OldGoRogueHash(Point p)
        {
            // Intentional overflow on both of these, part of hash-code generation
            int x2 = (int)(0x9E3779B9 * p.X), y2 = 0x632BE5AB * p.Y;
            return (int)(((uint)(x2 ^ y2) >> ((x2 & 7) + (y2 & 7))) * 0x85157AF5);
        }

        private int RosenbergStrongBasedHash(Point p)
        {
            int x = p.X + 3, y = p.Y + 3, n = (x >= y ? x * (x + 2) - y : y * y + x);
            return (int)(((n ^ (uint)n >> 1 ^ 0xD1B54A35) * 0xC13FA9AB ^ 0x7F4A7C15) * 0x91E10DA3);
        }

        private int RosenbergStrongBasedOneLessMultHash(Point p)
        {
            int x = p.X + 3, y = p.Y + 3, n = (x >= y ? x * (x + 2) - y : y * y + x);
            return (int)((n ^ (uint)n >> 1 ^ 0xD1B54A35) * 0xC13FA9AB ^ 0x7F4A7C15);
        }
        #endregion
    }
}

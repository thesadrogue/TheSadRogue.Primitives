using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;
using TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing
{
    /// <summary>
    /// A series of benchmarks that measure the amount of time it takes to add Points to a dictionary,
    /// where Points are being used as the key, when the dictionary is being passed different hashing algorithms to use.
    /// </summary>
    /// <remarks>
    /// Although dictionary add operations generally have more overhead than just the calls to GetHashCode they perform,
    /// the operation is affected by both the time it takes to compute a hash, and the number of collisions
    /// that hash generates.  This makes it a fairly well-rounded case which allows us to measure more real-world
    /// performance, which will take into account collisions as well as raw speed.
    /// </remarks>
    public class PointDictionaryAdd
    {
        public IEnumerable<int> SizeData => SharedTestParams.Sizes;

        /// <summary>
        /// An area of Size x Size will be used for the purposes of determining the series of points to add.
        /// </summary>
        [ParamsSource(nameof(SizeData))]
        public int Size;

        private Point[] _points = null!;
        private IEqualityComparer<Point> _sizeHasher = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            // Create cached list of points
            _points = new Point[Size * Size];
            for (int i = 0; i < _points.Length; i++)
                _points[i] = Point.FromIndex(i, Size);

            // Create equality comparer to ensure that the creation time isn't factored into benchmark
            // (since it is not for any other algorithms
            _sizeHasher = new KnownSizeHashing(Size);
        }

        [Benchmark]
        public Dictionary<Point, int> CurrentPrimitives() => CreateAndPopulate(EqualityComparer<Point>.Default);

        [Benchmark]
        public Dictionary<Point, int> OriginalGoRogue() => CreateAndPopulate(OriginalGoRogueAlgorithm.Instance);

        [Benchmark]
        public Dictionary<Point, int> KnownSize() => CreateAndPopulate(_sizeHasher);

        [Benchmark]
        public Dictionary<Point, int> RosenbergStrongBased() => CreateAndPopulate(RosenbergStrongBasedAlgorithm.Instance);

        [Benchmark]
        public Dictionary<Point, int> RosenbergStrongBasedMinusMultiply() => CreateAndPopulate(RosenbergStrongBasedMinusMultiplyAlgorithm.Instance);

        private Dictionary<Point, int> CreateAndPopulate(IEqualityComparer<Point> algorithm)
        {
            var dict = new Dictionary<Point, int>(algorithm);
            for (int i = 0; i < _points.Length; i++)
                dict[_points[i]] = i;

            return dict;
        }
    }
}

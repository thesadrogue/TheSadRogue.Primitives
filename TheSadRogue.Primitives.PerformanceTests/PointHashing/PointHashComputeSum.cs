using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;
using TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing
{
    /// <summary>
    /// Basic benchmarks for each hashing algorithm that are intended to simply measure the time it takes to compute
    /// the hash, without regard for collisions.
    /// </summary>
    /// <remarks>
    /// The tests actually compute the sum of the hashes for a Rectangle's worth of points, to account
    /// for cases where hashes of points with particular values may be more or less expensive than others.
    /// </remarks>
    public class PointHashComputeSum
    {
        public IEnumerable<int> SizeData => SharedTestParams.Sizes;

        /// <summary>
        /// An area of Size x Size will be used for the purposes of determining the series of points to use
        /// in the calculation.
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
        public int CurrentPrimitives() => SumHashesAlgorithm(CurrentPrimitivesAlgorithm.Instance);

        [Benchmark]
        public int OriginalGoRogue() => SumHashesAlgorithm(OriginalGoRogueAlgorithm.Instance);

        [Benchmark]
        public int KnownSize() => SumHashesAlgorithm(_sizeHasher);

        [Benchmark]
        public int RosenbergStrongBased() => SumHashesAlgorithm(RosenbergStrongBasedAlgorithm.Instance);

        [Benchmark]
        public int RosenbergStrongBasedMinusMultiply() => SumHashesAlgorithm(RosenbergStrongBasedMinusMultiplyAlgorithm.Instance);

        private int SumHashesAlgorithm(IEqualityComparer<Point> algorithm)
        {
            int sum = 0;
            for (int i = 0; i < _points.Length; i++)
                sum += algorithm.GetHashCode(_points[i]);

            return sum;
        }
    }
}

using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;
using SadRogue.Primitives.PointHashers;
using ShaiRandom.Generators;
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
        private IEqualityComparer<Point> _rangeHasher = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            // Create cached list of points.  Shuffle list to ensure cache linearity of the hash slots based on the
            // order in which we construct them isn't a factor.
            _points = SharedUtilities.PositiveArray(Size);
            new Xoshiro256StarStarRandom(1).Shuffle(_points);

            // Create equality comparers now to ensure that the creation time isn't factored into benchmark
            // (since it is not for any other algorithms)
            _sizeHasher = new KnownSizeHasher(Size);
            _rangeHasher = new KnownRangeHasher(new Point(0, 0), new Point(Size, Size));
        }

        [Benchmark]
        public int CurrentPrimitives() => SumHashesAlgorithm(EqualityComparer<Point>.Default);

        [Benchmark]
        public int OriginalGoRogue() => SumHashesAlgorithm(OriginalGoRogueAlgorithm.Instance);

        [Benchmark]
        public int KnownSize() => SumHashesAlgorithm(_sizeHasher);

        [Benchmark]
        public int KnownRange() => SumHashesAlgorithm(_rangeHasher);

        [Benchmark]
        public int RosenbergStrongBased() => SumHashesAlgorithm(RosenbergStrongBasedAlgorithm.Instance);

        [Benchmark]
        public int RosenbergStrongBasedMinusMultiply() => SumHashesAlgorithm(RosenbergStrongBasedMinusMultiplyAlgorithm.Instance);

        [Benchmark]
        public int RosenbergStrongPure() => SumHashesAlgorithm(RosenbergStrongPureAlgorithm.Instance);

        [Benchmark]
        public int CantorPure() => SumHashesAlgorithm(CantorPureAlgorithm.Instance);

        [Benchmark]
        public int BareMinimum() => SumHashesAlgorithm(BareMinimumAlgorithm.Instance);

        [Benchmark]
        public int BareMinimumSubtract() => SumHashesAlgorithm(BareMinimumSubtractAlgorithm.Instance);

        [Benchmark]
        public int BareMinimum8And24() => SumHashesAlgorithm(BareMinimum8And24Algorithm.Instance);

        [Benchmark]
        public int MultiplySum() => SumHashesAlgorithm(MultiplySumAlgorithm.Instance);

        [Benchmark]
        public int HashCodeCombine() => SumHashesAlgorithm(HashCodeCombineAlgorithm.Instance);

        private int SumHashesAlgorithm(IEqualityComparer<Point> algorithm)
        {
            int sum = 0;
            for (int i = 0; i < _points.Length; i++)
                sum += algorithm.GetHashCode(_points[i]);

            return sum;
        }
    }
}

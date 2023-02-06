﻿using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;
using SadRogue.Primitives.PointHashers;
using ShaiRandom.Generators;
using TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing
{
    /// <summary>
    /// A series of benchmarks that measure the amount of time it takes to add Points to a hash set,
    /// when the hash set is being passed different hashing algorithms to use.
    /// </summary>
    /// <remarks>
    /// Although hash set creation operations generally have more overhead than just the calls to GetHashCode they
    /// perform, the operation is affected by both the time it takes to compute a hash, and the number of collisions
    /// that hash generates.  This makes it a fairly well-rounded case which allows us to measure more real-world
    /// performance, which will take into account collisions as well as raw speed.
    ///
    /// It may be useful to compare these results to the corresponding Dictionary tests as well.
    /// </remarks>
    public class PointHashSetAdd
    {
        public IEnumerable<int> SizeData => SharedTestParams.Sizes;

        /// <summary>
        /// An area of Size x Size will be used for the purposes of determining the series of points to add.
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
        public HashSet<Point> CurrentPrimitives() => CreateAndPopulate(EqualityComparer<Point>.Default);

        [Benchmark]
        public HashSet<Point> OriginalGoRogue() => CreateAndPopulate(OriginalGoRogueAlgorithm.Instance);

        [Benchmark]
        public HashSet<Point> KnownSize() => CreateAndPopulate(_sizeHasher);

        [Benchmark]
        public HashSet<Point> KnownRange() => CreateAndPopulate(_rangeHasher);

        [Benchmark]
        public HashSet<Point> RosenbergStrongBased() => CreateAndPopulate(RosenbergStrongBasedAlgorithm.Instance);

        [Benchmark]
        public HashSet<Point> RosenbergStrongBasedMinusMultiply() => CreateAndPopulate(RosenbergStrongBasedMinusMultiplyAlgorithm.Instance);

        [Benchmark]
        public HashSet<Point> RosenbergStrongPure() => CreateAndPopulate(RosenbergStrongPureAlgorithm.Instance);

        [Benchmark]
        public HashSet<Point> CantorPure() => CreateAndPopulate(CantorPureAlgorithm.Instance);

        [Benchmark]
        public HashSet<Point> BareMinimum() => CreateAndPopulate(BareMinimumAlgorithm.Instance);

        [Benchmark]
        public HashSet<Point> BareMinimumSubtract() => CreateAndPopulate(BareMinimumSubtractAlgorithm.Instance);

        [Benchmark]
        public HashSet<Point> BareMinimum8And24() => CreateAndPopulate(BareMinimum8And24Algorithm.Instance);

        [Benchmark]
        public HashSet<Point> SimpleShift() => CreateAndPopulate(SimpleShiftAlgorithm.Instance);

        [Benchmark]
        public HashSet<Point> MultiplySum() => CreateAndPopulate(MultiplySumAlgorithm.Instance);

        [Benchmark]
        public HashSet<Point> HashCodeCombine() => CreateAndPopulate(HashCodeCombineAlgorithm.Instance);

        private HashSet<Point> CreateAndPopulate(IEqualityComparer<Point> algorithm)
        {
            var hashSet = new HashSet<Point>(algorithm);
            for (int i = 0; i < _points.Length; i++)
                hashSet.Add(_points[i]);

            return hashSet;
        }
    }
}

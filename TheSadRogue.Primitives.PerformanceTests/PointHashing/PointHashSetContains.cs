using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;
using SadRogue.Primitives.PointHashers;
using ShaiRandom.Generators;
using TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing
{
    /// <summary>
    /// A series of benchmarks that measure the amount of time it takes to see if a HashSet contains different points,
    /// when the hash set is being passed different hashing algorithms to use.
    /// </summary>
    /// <remarks>
    /// This benchmark serves a similar purpose to <see cref="PointHashSetAdd"/>; it simply provides another common
    /// real-world operation to benchmark, in order to gauge an algorithm's realistic effectiveness.
    ///
    /// It may be useful to compare these results to the corresponding Dictionary tests as well.
    /// </remarks>
    public class PointHashSetContains
    {
        public IEnumerable<int> SizeData => SharedTestParams.Sizes;

        /// <summary>
        /// An area of Size x Size will be used for the purposes of determining the series of points to check for.
        /// </summary>
        [ParamsSource(nameof(SizeData))]
        public int Size;

        private Point[] _points = null!;
        private IEqualityComparer<Point> _sizeHasher = null!;
        private IEqualityComparer<Point> _rangeHasher = null!;

        private HashSet<Point> _currentPrimitives = null!;
        private HashSet<Point> _originalGoRogue = null!;
        private HashSet<Point> _knownSizeHashing = null!;
        private HashSet<Point> _knownRangeHashing = null!;
        private HashSet<Point> _rosenbergStrong = null!;
        private HashSet<Point> _rosenbergStrongMinusMultiply = null!;
        private HashSet<Point> _rosenbergStrongPure = null!;
        private HashSet<Point> _cantorPure = null!;
        private HashSet<Point> _bareMinimum = null!;
        private HashSet<Point> _bareMinimumSubtract = null!;
        private HashSet<Point> _bareMinimum8And24 = null!;
        private HashSet<Point> _simpleShift = null!;
        private HashSet<Point> _multiplySum = null!;
        private HashSet<Point> _hashCodeCombine = null!;

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

            // Create dictionaries to retrieve from (we don't want to time this part so we will cache them)
            _currentPrimitives = CreateAndPopulate(EqualityComparer<Point>.Default);
            _originalGoRogue = CreateAndPopulate(OriginalGoRogueAlgorithm.Instance);
            _knownSizeHashing = CreateAndPopulate(_sizeHasher);
            _knownRangeHashing = CreateAndPopulate(_rangeHasher);
            _rosenbergStrong = CreateAndPopulate(RosenbergStrongBasedAlgorithm.Instance);
            _rosenbergStrongMinusMultiply = CreateAndPopulate(RosenbergStrongBasedMinusMultiplyAlgorithm.Instance);
            _rosenbergStrongPure = CreateAndPopulate(RosenbergStrongPureAlgorithm.Instance);
            _cantorPure = CreateAndPopulate(CantorPureAlgorithm.Instance);
            _bareMinimum = CreateAndPopulate(BareMinimumAlgorithm.Instance);
            _bareMinimumSubtract = CreateAndPopulate(BareMinimumSubtractAlgorithm.Instance);
            _bareMinimum8And24 = CreateAndPopulate(BareMinimum8And24Algorithm.Instance);
            _simpleShift = CreateAndPopulate(SimpleShiftAlgorithm.Instance);
            _multiplySum = CreateAndPopulate(MultiplySumAlgorithm.Instance);
            _hashCodeCombine = CreateAndPopulate(HashCodeCombineAlgorithm.Instance);
        }

        [Benchmark]
        public int CurrentPrimitivesExisting() => CheckExistingFrom(_currentPrimitives);

        [Benchmark]
        public int CurrentPrimitivesNonExisting() => CheckNonExistingFrom(_currentPrimitives);

        [Benchmark]
        public int OriginalGoRogueExisting() => CheckExistingFrom(_originalGoRogue);

        [Benchmark]
        public int OriginalGoRogueNonExisting() => CheckNonExistingFrom(_originalGoRogue);

        [Benchmark]
        public int KnownSizeExisting() => CheckExistingFrom(_knownSizeHashing);

        [Benchmark]
        public int KnownSizeNonExisting() => CheckNonExistingFrom(_knownSizeHashing);

        [Benchmark]
        public int KnownRangeExisting() => CheckExistingFrom(_knownRangeHashing);

        [Benchmark]
        public int KnownRangeNonExisting() => CheckNonExistingFrom(_knownRangeHashing);

        [Benchmark]
        public int RosenbergStrongBasedExisting() => CheckExistingFrom(_rosenbergStrong);

        [Benchmark]
        public int RosenbergStrongBasedNonExisting() => CheckNonExistingFrom(_rosenbergStrong);

        [Benchmark]
        public int RosenbergStrongBasedMinusMultiplyExisting() => CheckExistingFrom(_rosenbergStrongMinusMultiply);
        [Benchmark]
        public int RosenbergStrongBasedMinusMultiplyNonExisting() => CheckNonExistingFrom(_rosenbergStrongMinusMultiply);

        [Benchmark]
        public int RosenbergStrongPureExisting() => CheckExistingFrom(_rosenbergStrongPure);
        [Benchmark]
        public int RosenbergStrongPureNonExisting() => CheckNonExistingFrom(_rosenbergStrongPure);

        [Benchmark]
        public int CantorPureExisting() => CheckExistingFrom(_cantorPure);
        [Benchmark]
        public int CantorPureNonExisting() => CheckNonExistingFrom(_cantorPure);

        [Benchmark]
        public int BareMinimumExisting() => CheckExistingFrom(_bareMinimum);
        [Benchmark]
        public int BareMinimumNonExisting() => CheckNonExistingFrom(_bareMinimum);

        [Benchmark]
        public int BareMinimumSubtractExisting() => CheckExistingFrom(_bareMinimumSubtract);
        [Benchmark]
        public int BareMinimumSubtractNonExisting() => CheckNonExistingFrom(_bareMinimumSubtract);

        [Benchmark]
        public int BareMinimum8And24Existing() => CheckExistingFrom(_bareMinimum8And24);
        [Benchmark]
        public int BareMinimum8And24NonExisting() => CheckNonExistingFrom(_bareMinimum8And24);

        [Benchmark]
        public int SimpleShiftExisting() => CheckExistingFrom(_simpleShift);
        [Benchmark]
        public int SimpleShiftNonExisting() => CheckNonExistingFrom(_simpleShift);

        [Benchmark]
        public int MultiplySumExisting() => CheckExistingFrom(_multiplySum);
        [Benchmark]
        public int MultiplySumNonExisting() => CheckNonExistingFrom(_multiplySum);

        [Benchmark]
        public int HashCodeCombineExisting() => CheckExistingFrom(_hashCodeCombine);
        [Benchmark]
        public int HashCodeCombineNonExisting() => CheckNonExistingFrom(_hashCodeCombine);

        private int CheckExistingFrom(HashSet<Point> hashSet)
        {
            int sum = 0;
            for (int i = 0; i < _points.Length; i++)
                if (hashSet.Contains(_points[i]))
                    sum++;

            return sum;
        }

        private int CheckNonExistingFrom(HashSet<Point> hashSet)
        {
            int sum = 0;
            for (int y = Size; y < Size * 2; y++)
                for (int x = Size; x < Size * 2; x++)
                    if (hashSet.Contains(new Point(x, y)))
                        sum++;

            return sum;
        }

        private HashSet<Point> CreateAndPopulate(IEqualityComparer<Point> algorithm)
        {
            var hashSet = new HashSet<Point>(algorithm);
            for (int i = 0; i < _points.Length; i++)
                hashSet.Add(_points[i]);

            return hashSet;
        }
    }
}

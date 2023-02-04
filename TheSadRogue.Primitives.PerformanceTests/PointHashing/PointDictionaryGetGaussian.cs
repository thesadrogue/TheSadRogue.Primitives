using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;
using SadRogue.Primitives.PointHashers;
using ShaiRandom;
using TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing
{
    /// <summary>
    /// A series of benchmarks that measure the amount of time it takes to retrieve values from a dictionary,
    /// where Points are being used as the key, when the dictionary is being passed different hashing algorithms to use.
    ///
    /// This version uses both positive and negative points, in order to maximize collision potential for most hashing algorithms.
    /// </summary>
    /// <remarks>
    /// This benchmark serves a similar purpose to <see cref="PointDictionaryAdd"/>; it simply provides another common
    /// real-world operation to benchmark, in order to gauge an algorithm's realistic effectiveness.
    /// </remarks>
    public class PointDictionaryGetGaussian
    {
        public IEnumerable<int> SizeData => SharedTestParams.Sizes;

        /// <summary>
        /// An area of Size x Size will be used for the purposes of determining the series of points to retrieve.
        /// </summary>
        [ParamsSource(nameof(SizeData))]
        public int Size;

        private Point[] _points = null!;
        private IEqualityComparer<Point> _sizeHasher = null!;
        private IEqualityComparer<Point> _rangeHasher = null!;

        private Dictionary<Point, int> _currentPrimitives = null!;
        private Dictionary<Point, int> _originalGoRogue = null!;
        private Dictionary<Point, int> _knownSizeHashing = null!;
        private Dictionary<Point, int> _knownRangeHashing = null!;
        private Dictionary<Point, int> _rosenbergStrong = null!;
        private Dictionary<Point, int> _rosenbergStrongMinusMultiply = null!;
        private Dictionary<Point, int> _rosenbergStrongPure = null!;
        private Dictionary<Point, int> _cantorPure = null!;
        private Dictionary<Point, int> _bareMinimum = null!;
        private Dictionary<Point, int> _multiplySum = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            // Create cached list of points
            _points = SharedUtilities.GaussianArray(Size);

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
            _multiplySum = CreateAndPopulate(MultiplySumAlgorithm.Instance);
        }

        [Benchmark]
        public int CurrentPrimitives() => GetAllFrom(_currentPrimitives);

        [Benchmark]
        public int OriginalGoRogue() => GetAllFrom(_originalGoRogue);

        [Benchmark]
        public int KnownSize() => GetAllFrom(_knownSizeHashing);

        [Benchmark]
        public int KnownRange() => GetAllFrom(_knownRangeHashing);

        [Benchmark]
        public int RosenbergStrongBased() => GetAllFrom(_rosenbergStrong);

        [Benchmark]
        public int RosenbergStrongBasedMinusMultiply() => GetAllFrom(_rosenbergStrongMinusMultiply);

        [Benchmark]
        public int RosenbergStrongPure() => GetAllFrom(_rosenbergStrongPure);

        [Benchmark]
        public int CantorPure() => GetAllFrom(_cantorPure);

        [Benchmark]
        public int BareMinimum() => GetAllFrom(_bareMinimum);

        [Benchmark]
        public int MultiplySum() => GetAllFrom(_multiplySum);

        private int GetAllFrom(Dictionary<Point, int> dict)
        {
            int sum = 0;
            for (int i = 0; i < _points.Length; i++)
                sum += dict[_points[i]];

            return sum;
        }

        private Dictionary<Point, int> CreateAndPopulate(IEqualityComparer<Point> algorithm)
        {
            var dict = new Dictionary<Point, int>(algorithm);
            for (int i = 0; i < _points.Length; i++)
                dict[_points[i]] = i;

            return dict;
        }
    }
}

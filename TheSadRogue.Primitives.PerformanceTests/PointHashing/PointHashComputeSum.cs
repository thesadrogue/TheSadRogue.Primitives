using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using JetBrains.Annotations;
using SadRogue.Primitives;
using ShaiRandom.Generators;

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
        public IEnumerable<HashingAlgorithm> AlgorithmData => SharedTestParams.Algorithms;
        public IEnumerable<DataSet> DataSetData => SharedTestParams.DataSets;

        /// <summary>
        /// The data set type to test.
        /// </summary>
        [UsedImplicitly]
        [ParamsSource(nameof(DataSetData))]
        public DataSet DataSet;

        /// <summary>
        /// An area of Size x Size will be used for the purposes of determining the series of points to hash.
        /// </summary>
        [UsedImplicitly]
        [ParamsSource(nameof(SizeData))]
        public int Size;

        /// <summary>
        /// The hashing algorithm to test; affects the equality comparer we use to calculate hashes.
        /// </summary>
        [UsedImplicitly]
        [ParamsSource(nameof(AlgorithmData))]
        public HashingAlgorithm Algorithm;



        private IEqualityComparer<Point> _comparer = null!;
        private Point[] _points = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            // Create list of points based on our data set.
            _points = SharedUtilities.GetDataSet(DataSet, Size);

            // Shuffle list to ensure cache linearity of the values isn't a factor.  In theory, the order of the values
            // should not affect the time it takes to sum, however since some of our data sets do generate points in
            // a very linear order, this should ensure no unwanted optimization can take place.
            new Xoshiro256StarStarRandom(1).Shuffle(_points);

            // Determine the correct equality comparer to use for the current hashing algorithm
            _comparer = SharedUtilities.GetHasher(Algorithm, Size) ?? EqualityComparer<Point>.Default;
        }

        [Benchmark]
        public int SumHashes()
        {
            int sum = 0;
            foreach (var point in _points)
                sum += _comparer.GetHashCode(point);

            return sum;
        }
    }
}

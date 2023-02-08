using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;
using ShaiRandom.Generators;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing
{
    /// <summary>
    /// A series of benchmarks that measure the amount of time it takes to retrieve values from a dictionary,
    /// where Points are being used as the key, when the dictionary is being passed different hashing algorithms to use.
    /// </summary>
    /// <remarks>
    /// Although dictionary add operations generally have more overhead than just the calls to GetHashCode they perform,
    /// the operation is affected by both the time it takes to compute a hash, and the number of collisions
    /// that hash generates.  This makes it a fairly well-rounded case which allows us to measure more real-world
    /// performance, which will take into account collisions as well as raw speed.
    /// </remarks>
    public class PointDictionaryGet
    {
        public IEnumerable<int> SizeData => SharedTestParams.Sizes;
        public IEnumerable<HashingAlgorithm> AlgorithmData => SharedTestParams.Algorithms;
        public IEnumerable<DataSet> DataSetData => SharedTestParams.DataSets;

        /// <summary>
        /// The data set type to test.
        /// </summary>
        [ParamsSource(nameof(DataSetData))]
        public DataSet DataSet;

        /// <summary>
        /// An area of Size x Size will be used for the purposes of determining the series of points to get.
        /// </summary>
        [ParamsSource(nameof(SizeData))]
        public int Size;

        /// <summary>
        /// The hashing algorithm to test; affects the equality comparer we use for the dictionary.
        /// </summary>
        [ParamsSource(nameof(AlgorithmData))]
        public HashingAlgorithm Algorithm;

        private Point[] _points = null!;
        private Dictionary<Point, int> _dictionary = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            // Create list of points based on our data set.
            _points = SharedUtilities.GetDataSet(DataSet, Size);

            // Shuffle list to ensure cache linearity of the hash slots based on the order in which we construct them
            // isn't a factor. This is particularly important because some data sets generate points in a very linear
            // order.
            new Xoshiro256StarStarRandom(1).Shuffle(_points);

            // Determine the correct equality comparer to use for the current hashing algorithm, and create the
            // dictionary using that comparer.  We very explicitly elect to NOT PASS A HASHER for CurrentPrimitives,
            // rather than passing EqualityComparer.Default.  This is because the current primitives hashing function
            // will always be measured by one of the other comparers anyway, so makes CurrentPrimitives a good measure
            // of if there are optimizations and the like which make the default implementation faster than a custom
            // equality comparer.
            var comparer = SharedUtilities.GetHasher(Algorithm, Size);
            _dictionary = (comparer == null) ? new Dictionary<Point, int>() : new Dictionary<Point, int>(comparer);
            for (int i = 0; i < _points.Length; i++)
                _dictionary[_points[i]] = i;
        }

        [Benchmark]
        public int GetAllPoints()
        {
            int sum = 0;
            for (int i = 0; i < _points.Length; i++)
                sum += _dictionary[_points[i]];

            return sum;
        }
    }
}

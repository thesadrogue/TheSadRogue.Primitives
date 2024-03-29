﻿using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using JetBrains.Annotations;
using SadRogue.Primitives;
using ShaiRandom.Generators;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing
{
    /// <summary>
    /// A series of benchmarks that measure the amount of time it takes to add Points to a hash set,
    /// when the hash set is being passed different hashing algorithms to use.
    ///
    /// Consider using the Contains tests as benchmarks instead; the tests implemented here contain a lot of overhead
    /// not pertaining to the hashing algorithms.
    /// </summary>
    /// <remarks>
    /// Although hash set add operations generally have more overhead than just the calls to GetHashCode they perform,
    /// the operation is affected by both the time it takes to compute a hash, and the number of collisions
    /// that hash generates.  In theory, this makes it a fairly well-rounded case which allows us to measure more real-world
    /// performance, which will take into account collisions as well as raw speed.
    ///
    /// In practice, however, we have to measure the time it takes to allocate a hash set in the test as well.
    /// The GC this generates is likely to skew the test results; so it is recommended to take these results, at best,
    /// with a grain of salt.  <see cref="PointHashSetContains"/> is probably a better measure.
    /// </remarks>
    public class PointHashSetAdd
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
        /// An area of Size x Size will be used for the purposes of determining the series of points to add.
        /// </summary>
        [UsedImplicitly]
        [ParamsSource(nameof(SizeData))]
        public int Size;

        /// <summary>
        /// The hashing algorithm to test; affects the equality comparer we use for the HashSet.
        /// </summary>
        [UsedImplicitly]
        [ParamsSource(nameof(AlgorithmData))]
        public HashingAlgorithm Algorithm;

        private IEqualityComparer<Point>? _comparer;
        private Point[] _points = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            // Create list of points based on our data set.
            _points = SharedUtilities.GetDataSet(DataSet, Size);

            // Shuffle list to ensure cache linearity of the hash slots based on the order in which we construct them
            // isn't a factor. This is particularly important because some data sets generate points in a very linear
            // order.
            new Xoshiro256StarStarRandom(1).Shuffle(_points);

            // Determine the correct equality comparer to use for the current hashing algorithm
            _comparer = SharedUtilities.GetHasher(Algorithm, Size);
        }

        [Benchmark]
        public HashSet<Point> AddAllPoints()
        {
            var hashSet = _comparer == null ? new HashSet<Point>() : new HashSet<Point>(_comparer);
            foreach (var point in _points)
                hashSet.Add(point);

            return hashSet;
        }
    }
}

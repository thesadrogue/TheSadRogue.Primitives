using System;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing
{
    /// <summary>
    /// Contains fields which act as data sources for test parameters that are shared across multiple PointHashing
    /// tests.
    /// </summary>
    public static class SharedTestParams
    {
        /// <summary>
        /// Contains all the values of the "Size" test parameter that will be tested when the tests are run.
        /// </summary>
        public static readonly int[] Sizes = { 10, 50, 100, 175, 256, 1024 };

        /// <summary>
        /// Contains all hashing algorithms that will be tested.
        /// </summary>
        public static readonly HashingAlgorithm[] Algorithms = Enum.GetValues<HashingAlgorithm>();

        /// <summary>
        /// Contains all data sets/types that the hashing algorithms will be tested with.
        /// </summary>
        public static readonly DataSet[] DataSets = Enum.GetValues<DataSet>();
    }
}

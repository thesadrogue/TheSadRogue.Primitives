using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Microsoft.VisualBasic;
using SadRogue.Primitives;
using SadRogue.Primitives.PointHashers;
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
    public class PointDictionaryAddGaussian
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
        /// <summary>
        /// Given a double between 0 and 1, gets a Gaussian-distributed (also called normal-distributed) double with range from -38.5 to 38.5 .
        /// </summary>
        /// <remarks>
        /// This uses an algorithm by Peter John Acklam, as implemented by Sherali Karimov.
        /// <a href = "https://web.archive.org/web/20150910002142/http://home.online.no/~pjacklam/notes/invnorm/impl/karimov/StatUtil.java" > Original source</a>.
        /// <a href = "https://web.archive.org/web/20151030215612/http://home.online.no/~pjacklam/notes/invnorm/" > Information on the algorithm</a>.
        /// <a href = "https://en.wikipedia.org/wiki/Probit_function" > Wikipedia's page on the probit function</a> may help, also.
        /// </remarks>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double Probit(double d)
        {
            if (d <= 0 || d >= 1)
            {
                return Math.CopySign(38.5, d - 0.5);
            }
            else if (d < 0.02425)
            {
                double q = Math.Sqrt(-2.0 * Math.Log(d));
                return (((((-7.784894002430293e-03 * q - 3.223964580411365e-01) * q - 2.400758277161838e+00) * q - 2.549732539343734e+00) * q + 4.374664141464968e+00) * q + 2.938163982698783e+00) / (
                        (((7.784695709041462e-03 * q + 3.224671290700398e-01) * q + 2.445134137142996e+00) * q + 3.754408661907416e+00) * q + 1.0);
            }
            else if (0.97575 < d)
            {
                double q = Math.Sqrt(-2.0 * Math.Log(1 - d));
                return -(((((-7.784894002430293e-03 * q - 3.223964580411365e-01) * q - 2.400758277161838e+00) * q - 2.549732539343734e+00) * q + 4.374664141464968e+00) * q + 2.938163982698783e+00) / (
                        (((7.784695709041462e-03 * q + 3.224671290700398e-01) * q + 2.445134137142996e+00) * q + 3.754408661907416e+00) * q + 1.0);
            }
            else
            {
                double q = d - 0.5;
                double r = q * q;
                return (((((-3.969683028665376e+01 * r + 2.209460984245205e+02) * r - 2.759285104469687e+02) * r + 1.383577518672690e+02) * r - 3.066479806614716e+01) * r + 2.506628277459239e+00) * q / (
                        ((((-5.447609879822406e+01 * r + 1.615858368580409e+02) * r - 1.556989798598866e+02) * r + 6.680131188771972e+01) * r - 1.328068155288572e+01) * r + 1.0);
            }
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            int totalSize = Size * Size;
            // Create cached list of points
            _points = new Point[totalSize];
            ulong xc = 1UL, yc = 2UL;
            HashSet<Point> pts = new HashSet<Point>(totalSize);
            unchecked
            {
                while(pts.Count < totalSize)
                {
                    // R2 sequence, sub-random with lots of space between nearby points
                    xc += 0xC13FA9A902A6328FUL;
                    yc += 0x91E10DA5C79E7B1DUL;
                    // Using well-spread inputs to Probit() gives points that shouldn't overlap as often.
                    // 1.1102230246251565E-16 is 2 to the -53 .
                    pts.Add(new Point((int)(Probit((xc >> 11) * 1.1102230246251565E-16) * 256.0),
                            (int)(Probit((yc >> 11) * 1.1102230246251565E-16) * 256.0)));
                }
                pts.CopyTo(_points);
            }

            // Create equality comparers now to ensure that the creation time isn't factored into benchmark
            // (since it is not for any other algorithms)
            _sizeHasher = new KnownSizeHasher(Size);
            _rangeHasher = new KnownRangeHasher(new Point(0, 0), new Point(Size, Size));
        }

        [Benchmark]
        public Dictionary<Point, int> CurrentPrimitives() => CreateAndPopulate(EqualityComparer<Point>.Default);

        [Benchmark]
        public Dictionary<Point, int> OriginalGoRogue() => CreateAndPopulate(OriginalGoRogueAlgorithm.Instance);

        [Benchmark]
        public Dictionary<Point, int> KnownSize() => CreateAndPopulate(_sizeHasher);

        [Benchmark]
        public Dictionary<Point, int> KnownRange() => CreateAndPopulate(_rangeHasher);

        [Benchmark]
        public Dictionary<Point, int> RosenbergStrongBased() => CreateAndPopulate(RosenbergStrongBasedAlgorithm.Instance);

        [Benchmark]
        public Dictionary<Point, int> RosenbergStrongBasedMinusMultiply() => CreateAndPopulate(RosenbergStrongBasedMinusMultiplyAlgorithm.Instance);

        [Benchmark]
        public Dictionary<Point, int> RosenbergStrongPure() => CreateAndPopulate(RosenbergStrongPureAlgorithm.Instance);

        [Benchmark]
        public Dictionary<Point, int> CantorPure() => CreateAndPopulate(CantorPureAlgorithm.Instance);

        [Benchmark]
        public Dictionary<Point, int> BareMinimum() => CreateAndPopulate(BareMinimumAlgorithm.Instance);

        [Benchmark]
        public Dictionary<Point, int> MultiplySum() => CreateAndPopulate(MultiplySumAlgorithm.Instance);

        private Dictionary<Point, int> CreateAndPopulate(IEqualityComparer<Point> algorithm)
        {
            var dict = new Dictionary<Point, int>(algorithm);
            for (int i = 0; i < _points.Length; i++)
                dict[_points[i]] = i;

            return dict;
        }
    }
}

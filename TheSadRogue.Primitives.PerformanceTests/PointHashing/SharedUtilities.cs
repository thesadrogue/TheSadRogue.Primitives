using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives;
using SadRogue.Primitives.PointHashers;
using ShaiRandom;
using TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing;

/// <summary>
/// Various data sets to test operations against
/// </summary>
public enum DataSet
{
    /// <summary>
    /// A data set with Size * Size coordinates, all with positive X and Y values.  The coordinates will be (0, 0)
    /// through (Size - 1, Size - 1), in shuffled order.
    /// </summary>
    PositiveOnly,
    /// <summary>
    /// A data set with Size * Size coordinates, with a fairly non-uniform set of coordinates spanning both positive
    /// and negative X and Y values.  Because this involves negative numbers, this data set may generate more collisions
    /// for many hashing algorithms.
    /// </summary>
    Gaussian,
}

/// <summary>
/// The hashing algorithm to use in the test.
/// </summary>
public enum HashingAlgorithm
{
    CurrentPrimitives,
    KnownSize,
    KnownRange,
    BareMinimum8And24,
    BareMinimum,
    BareMinimumSubtract,
    CantorPure,
    HashCodeCombine,
    MultiplySum,
    OriginalGoRogue,
    RosenbergStrongBased,
    RosenbergStrongBasedMinusMultiply,
    RosenbergStrongPure,
    SimpleShift,
}

/// <summary>
/// Some functions useful for generating test data from enumerations.
/// </summary>
public static class SharedUtilities
{
    public static IEqualityComparer<Point>? GetHasher(HashingAlgorithm algo, int size)
        // We disable this warning as it pertains to _unnamed_ switch values, because we still want the compiler to tell
        // us if we haven't added a _named_ value.
#pragma warning disable CS8524
        => algo switch
#pragma warning restore CS8524
        {
            HashingAlgorithm.CurrentPrimitives => null,
            HashingAlgorithm.BareMinimum => BareMinimumAlgorithm.Instance,
            HashingAlgorithm.BareMinimum8And24 => BareMinimum8And24Algorithm.Instance,
            HashingAlgorithm.BareMinimumSubtract => BareMinimumSubtractAlgorithm.Instance,
            HashingAlgorithm.CantorPure => CantorPureAlgorithm.Instance,
            HashingAlgorithm.HashCodeCombine => HashCodeCombineAlgorithm.Instance,
            HashingAlgorithm.MultiplySum => MultiplySumAlgorithm.Instance,
            HashingAlgorithm.OriginalGoRogue => OriginalGoRogueAlgorithm.Instance,
            HashingAlgorithm.RosenbergStrongBased => RosenbergStrongBasedAlgorithm.Instance,
            HashingAlgorithm.RosenbergStrongBasedMinusMultiply => RosenbergStrongBasedMinusMultiplyAlgorithm.Instance,
            HashingAlgorithm.RosenbergStrongPure => RosenbergStrongPureAlgorithm.Instance,
            HashingAlgorithm.SimpleShift => SimpleShiftAlgorithm.Instance,
            // (0, 0) -> (Size - 1, Size - 1) is the only reasonable use case for these 2 hashing algorithms, based on
            // how we define size.  We'll test it with all data sets, with the knowledge that we're explicitly violating
            // the contract stated in the documentation, when our coordinates don't match up to the range; so we'll
            // expect to see performance suffer.
            HashingAlgorithm.KnownSize => new KnownSizeHasher(size),
            HashingAlgorithm.KnownRange => new KnownRangeHasher(new Point(0, 0), new Point(size, size))
        };

    public static Point[] GetDataSet(DataSet set, int size)
        // We disable this warning as it pertains to _unnamed_ switch values, because we still want the compiler to tell
        // us if we haven't added a _named_ value.
#pragma warning disable CS8524
        => set switch
#pragma warning restore CS8524
        {
            DataSet.Gaussian => GaussianArray(size),
            DataSet.PositiveOnly => PositiveArray(size),
        };

    /// <summary>
    /// Generates an array of all points (0, 0) to (Size - 1, Size - 1)
    /// </summary>
    /// <param name="size"/>
    /// <returns/>
    private static Point[] PositiveArray(int size)
    {
        var points = new Point[size * size];
        for (int i = 0; i < points.Length; i++)
            points[i] = Point.FromIndex(i, size);

        return points;
    }

    /// <summary>
    /// Returns an array of both positive and negative points over the size given, which is likely to generate the
    /// most collisions for many hashing algorithms.
    /// </summary>
    /// <param name="size"/>
    /// <returns/>
    private static Point[] GaussianArray(int size)
    {
        int totalSize = size * size;
        ulong xc = 1UL, yc = 2UL;
        SadRogue.Primitives.Area pts = new();
        unchecked
        {
            while(pts.Count < totalSize)
            {
                // R2 sequence, sub-random with lots of space between nearby points
                xc += 0xC13FA9A902A6328FUL;
                yc += 0x91E10DA5C79E7B1DUL;
                // Using well-spread inputs to Probit() gives points that shouldn't overlap as often.
                // 1.1102230246251565E-16 is 2 to the -53 .
                pts.Add(new Point((int)(MathUtils.Probit((xc >> 11) * 1.1102230246251565E-16) * 256.0),
                    (int)(MathUtils.Probit((yc >> 11) * 1.1102230246251565E-16) * 256.0)));
            }
        }

        return pts.ToArray();
    }
}

using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives;
using ShaiRandom;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing;

/// <summary>
/// Some functions useful for generating arrays of test values used in hashing tests.
/// </summary>
public static class SharedUtilities
{
    /// <summary>
    /// Generates an array of all points (0, 0) to (Size - 1, Size - 1)
    /// </summary>
    /// <param name="size"/>
    /// <returns/>
    public static Point[] PositiveArray(int size)
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
    public static Point[] GaussianArray(int size)
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

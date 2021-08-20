using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms
{
    /// <summary>
    /// IEqualityComparer using the current primitives library hashing function defined in Point.
    /// </summary>
    /// <remarks>
    /// It is technically unnecessary to use a custom equality comparer for this; however doing so provides
    /// a universal baseline, so that any overhead involved due to the use of any custom equality comparer
    /// does not skew results.
    ///
    /// Note that this does bring in potential overhead of vtable lookups that probably would not actually happen
    /// in most use cases involving Dictionary, HashSet, etc; the compiler may also be unable to inline the
    /// GetHashCode function in this instance.  This test may be removed or modified in the future as a result
    /// of this.
    /// </remarks>
    public sealed class CurrentPrimitivesAlgorithm : IEqualityComparer<Point>
    {
        public static readonly IEqualityComparer<Point> Instance = new CurrentPrimitivesAlgorithm();

        public bool Equals(Point x, Point y) => x.Equals(y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(Point p) => p.GetHashCode();
    }
}

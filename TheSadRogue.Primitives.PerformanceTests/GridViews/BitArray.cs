using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using JetBrains.Annotations;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.PointHashers;

namespace TheSadRogue.Primitives.PerformanceTests.GridViews
{
    /// <summary>
    /// This class benchmarks BitArray against some functionally equivalent data structures.
    /// </summary>
    /// <remarks>
    /// In general, this is designed to gauge performance differences between BitArray and bool[].  It does so by
    /// testing cases that use the corresponding grid views, as well as a case where BitArray is used directly, in order
    /// to determine any overhead caused via access through the grid view.
    ///
    /// In order to keep these timings fair, BitArrayView must implement the indexers to not double-calculate index
    /// -> Point -> index (ie, it must implement all the indexers custom instead of using SettableGridViewBase), and the
    /// values are consistently accessed as if the starting value was a point (ie. using the ToIndex function).  The private
    /// variables for the data structures must also be of the concrete type (ie. BitArrayView or ArrayView) rather than
    /// the interface they implement (ie. IMapView).  Making them interfaces will trigger vtable indirections when calling
    /// interface methods; however, accessing them as their concrete type should _not_, since the types are sealed.
    /// This is a micro-optimization, but since the get benchmarks take on the order of half a nanosecond or less, it is
    /// relevant here.
    /// </remarks>
    public class BitArray
    {
        [UsedImplicitly]
        [Params(10, 100, 200)]
        public int Size;

        private readonly Point _on = new(1, 2);

        private ArrayView<bool> _boolArrayView = null!;
        private BitArrayView _bitArrayView = null!;
        private System.Collections.BitArray _bitArray = null!;
        private HashSet<Point> _hashSet = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _boolArrayView = new ArrayView<bool>(Size, Size) { [_on] = true };
            _bitArray = new System.Collections.BitArray(Size * Size) { [_on.ToIndex(Size)] = true };
            _bitArrayView = new BitArrayView(Size, Size) { [_on] = true };
            _hashSet = new HashSet<Point>(new KnownSizeHasher(Size)) { _on };
        }

        [Benchmark]
        public bool BoolArrayViewGet() => _boolArrayView[_on];

        [Benchmark]
        public bool BitArrayGet() => _bitArray[_on.ToIndex(Size)];

        [Benchmark]
        public bool BitArrayViewGet() => _bitArrayView[_on];

        [Benchmark]
        public bool HashSetGet() => _hashSet.Contains(_on);

        [Benchmark]
        public ArrayView<bool> BoolArrayViewFill()
        {
            _boolArrayView.Clear();
            return _boolArrayView;
        }

        [Benchmark]
        public System.Collections.BitArray BitArrayFill()
        {
            _bitArray.SetAll(false);
            return _bitArray;
        }

        [Benchmark]
        public BitArrayView BitArrayViewFill()
        {
            _bitArrayView.Fill(false);
            return _bitArrayView;
        }

        /// <summary>
        /// Note: This benchmark is not very fair compared to the other Fill benchmarks, since HashSet.Clear will scale
        /// linearly with the amount of items in the hash set (1 in this test case) where the others will not.
        /// Nonetheless, it is included for completeness.
        /// </summary>
        [Benchmark]
        public HashSet<Point> HashSetFill()
        {
            _hashSet.Clear();
            return _hashSet;
        }
    }
}

using System;
using BenchmarkDotNet.Attributes;
using JetBrains.Annotations;
using SadRogue.Primitives.GridViews;

namespace TheSadRogue.Primitives.PerformanceTests.GridViews
{
    public enum FillTestTypes
    {
        ArrayView,
        ArrayView2D,
        BitArrayView
    }

    public class FillAndClear
    {
        [UsedImplicitly]
        [Params(10, 100, 200)]
        public int Size;

        [UsedImplicitly]
        [ParamsAllValues]
        public FillTestTypes Type;

        private ISettableGridView<bool> _gridView = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _gridView = Type switch
            {
                FillTestTypes.ArrayView => new ArrayView<bool>(Size, Size),
                FillTestTypes.ArrayView2D => new ArrayView2D<bool>(Size, Size),
                FillTestTypes.BitArrayView => new BitArrayView(Size, Size),
                _ => throw new Exception("Unsupported grid view type.")
            };
        }

        [Benchmark]
        public ISettableGridView<bool> Clear()
        {
            _gridView.Clear();

            return _gridView;
        }

        [Benchmark]
        public ISettableGridView<bool> Fill()
        {
            _gridView.Fill(true);
            return _gridView;
        }

        [Benchmark]
        public ISettableGridView<bool> OldFill()
        {
            // This method is much faster for BitArrayView, so we'll special-case it to provide the best optimization
            // we can.  It's still better to call the BitArrayView directly, but since the Fill method can be
            // easily 10x faster for Bit arrays, even with both of these casts it's still faster than not
            if (_gridView is BitArrayView bitArray)
                bitArray.Fill(true);
            else
                _gridView.ApplyOverlay(_ => true);

            return _gridView;
        }

        [Benchmark]
        public ISettableGridView<bool> ManualFill()
        {
            for (int i = 0; i < _gridView.Count; i++)
                _gridView[i] = true;

            return _gridView;
        }
    }
}

using System;
using SadRogue.Primitives.GridViews;

namespace SadRogue.Primitives.UnitTests.Mocks
{
    /// <summary>
    /// A minimal implementation of SettableGridViewBase, which uses the default implementation for all functions
    /// which provide one.  Intended to allow unit testing of the default implementations, independent of implementation
    /// details of other grid views.
    /// </summary>
    /// <typeparam name="T"/>
    public class SettableGridViewBaseDefaultImplementationMock<T> : SettableGridViewBase<T>
    {
        private readonly ArrayView<T> _view;

        public override int Height => _view.Height;

        public override int Width => _view.Width;

        public override T this[Point pos]
        {
            get => _view[pos];
            set => _view[pos] = value;
        }

        public SettableGridViewBaseDefaultImplementationMock(int width, int height)
        {
            _view = new ArrayView<T>(width, height);
        }
    }

    /// <summary>
    /// A minimal implementation of SettableGridView1DIndexBase, which uses the default implementation for all functions
    /// which provide one.  Intended to allow unit testing of the default implementations, independent of implementation
    /// details of other grid views.
    /// </summary>
    /// <typeparam name="T"/>
    public class SettableGridView1DIndexBaseDefaultImplementationMock<T> : SettableGridView1DIndexBase<T>
    {
        private readonly ArrayView<T> _view;

        public override int Height => _view.Height;

        public override int Width => _view.Width;

        public override T this[int index]
        {
            get => _view[index];
            set => _view[index] = value;
        }

        public SettableGridView1DIndexBaseDefaultImplementationMock(int width, int height)
        {
            _view = new ArrayView<T>(width, height);
        }
    }

    /// <summary>
    /// A minimal implementation of GridViewBase, which uses the default implementation for all functions
    /// which provide one.  Intended to allow unit testing of the default implementations, independent of implementation
    /// details of other grid views.
    /// </summary>
    /// <typeparam name="T"/>
    public class GridViewBaseDefaultImplementationMock<T> : GridViewBase<T>
    {
        public readonly ArrayView<T> View;

        public override int Height => View.Height;

        public override int Width => View.Width;

        public override T this[Point pos]
        {
            get => View[pos];
            //set => View[pos] = value;
        }

        public GridViewBaseDefaultImplementationMock(int width, int height)
        {
            View = new ArrayView<T>(width, height);
        }
    }

    /// <summary>
    /// A minimal implementation of GridView1DIndexBase, which uses the default implementation for all functions
    /// which provide one.  Intended to allow unit testing of the default implementations, independent of implementation
    /// details of other grid views.
    /// </summary>
    /// <typeparam name="T"/>
    public class GridView1DIndexBaseDefaultImplementationMock<T> : GridView1DIndexBase<T>
    {
        public readonly ArrayView<T> View;

        public override int Height => View.Height;

        public override int Width => View.Width;

        public override T this[int index]
        {
            get => View[index];
            //set => View[pos] = value;
        }

        public GridView1DIndexBaseDefaultImplementationMock(int width, int height)
        {
            View = new ArrayView<T>(width, height);
        }
    }

    /// <summary>
    /// A very basic implementation of ISettableGridView which wraps an ArrayView.  This is pointless for any practical
    /// use case since ArrayView already implements this interface, but this implementation provides the minimal subset
    /// of required methods so that default methods/extensions methods can be tested using it.
    /// </summary>
    /// <typeparam name="T"/>
    public class SettableGridViewDefaultImplementationMock<T> : ISettableGridView<T>
    {
        public readonly ArrayView<T> View;
        public int Height => View.Height;

        public int Width => View.Width;

        public int Count => View.Count;

        public T this[int x, int y]
        {
            get => View[x, y];
            set => View[x, y] = value;
        }

        public T this[Point pos]
        {
            get => View[pos];
            set => View[pos] = value;
        }

        public T this[int index1D]
        {
            get => View[index1D];
            set => View[index1D] = value;
        }

        public SettableGridViewDefaultImplementationMock(int width, int height)
        {
            View = new ArrayView<T>(width, height);
        }
    }

    /// <summary>
    /// An incorrect translation grid view implementation which does not override any variation of TranslateGet.
    /// </summary>
    /// <typeparam name="T1"/>
    /// <typeparam name="T2"/>
    public class TranslationGridViewNoOverrides<T1, T2> : TranslationGridView<T1, T2>
    {
        public TranslationGridViewNoOverrides(IGridView<T1> baseGrid)
            : base(baseGrid)
        { }
    }

    /// <summary>
    /// TranslationGridView implementation which implements the most simple variation of TranslateGet, ie. the one which
    /// doesn't take a position.
    /// </summary>
    public class TranslationGridViewSimpleOverride : TranslationGridView<bool, int>
    {
        public TranslationGridViewSimpleOverride(IGridView<bool> baseGrid)
            : base(baseGrid)
        { }

        protected override int TranslateGet(bool value) => value ? 1 : 0;
    }

    /// <summary>
    /// A TranslationGridView implementation which implements the version of TranslateGet
    /// that takes a position and forces all perimeter squares to 0 in the output.
    /// </summary>
    public class TranslationGridViewPositionOverride : TranslationGridView<bool, int>
    {
        public TranslationGridViewPositionOverride(IGridView<bool> baseGrid)
            : base(baseGrid)
        { }

        protected override int TranslateGet(Point position, bool value)
            => this.Bounds().Expand(-1, -1).Contains(position) ? value ? 1 : 0 : 0;

    }

    /// <summary>
    /// An incorrect settable translation grid view implementation which does not override any variation of TranslateGet and TranslateSet.
    /// </summary>
    /// <typeparam name="T1"/>
    /// <typeparam name="T2"/>
    public class SettableTranslationGridViewNoOverrides<T1, T2> : SettableTranslationGridView<T1, T2>
    {
        public SettableTranslationGridViewNoOverrides(ISettableGridView<T1> baseGrid)
            : base(baseGrid)
        { }
    }

    /// <summary>
    /// SettableTranslationGridView implementation which implements the most simple variation of TranslateGet and
    /// TranslateSet, ie. the ones which don't take a position.
    /// </summary>
    public class SettableTranslationGridViewSimpleOverride : SettableTranslationGridView<bool, int>
    {
        public SettableTranslationGridViewSimpleOverride(ISettableGridView<bool> baseGrid)
            : base(baseGrid)
        { }

        public SettableTranslationGridViewSimpleOverride(ISettableGridView<bool> baseGrid, ISettableGridView<int> overlay)
            : base(baseGrid, overlay)
        { }

        protected override int TranslateGet(bool value) => value ? 1 : 0;

        protected override bool TranslateSet(int value) => value != 0;
    }

    /// <summary>
    /// A SettableTranslationGridView implementation which implements the version of TranslateGet and TranslateSet
    /// that takes a position and forces all perimeter squares to 0/false in the output.
    /// </summary>
    public class SettableTranslationGridViewPositionOverride : SettableTranslationGridView<bool, int>
    {
        public SettableTranslationGridViewPositionOverride(ISettableGridView<bool> baseGrid)
            : base(baseGrid)
        { }

        protected override int TranslateGet(Point position, bool value)
            => this.Bounds().Expand(-1, -1).Contains(position) ? value ? 1 : 0 : 0;

        protected override bool TranslateSet(Point position, int value)
            => this.Bounds().Expand(-1, -1).Contains(position) && value != 0;
    }

    /// <summary>
    /// A class that creates mock grid views for use in unit tests.
    /// </summary>
    internal static class MockGridViews
    {
        public static ArrayView2D<int> RectangleArrayView2D(int width, int height)
        {
            var grid = RectangleBooleanGrid(width, height);

            var arrayGrid = new ArrayView2D<int>(grid.Width, grid.Height);
            arrayGrid.ApplyOverlay(pos => grid[pos] ? 1 : 0);

            return arrayGrid;
        }

        public static ISettableGridView<double> RandomDoubleGrid(int width, int height)
        {
            var grid = new ArrayView<double>(width, height);
            Random rng = new Random();

            foreach (var pos in grid.Positions())
                grid[pos] = rng.NextDouble();

            return grid;
        }

        public static ISettableGridView<bool> RectangleBooleanGrid(int width, int height)
        {
            ISettableGridView<bool> grid = new ArrayView<bool>(width, height);
            foreach (var pos in grid.Bounds().Expand(-1, -1).Positions())
                grid[pos] = true;

            return grid;
        }
    }
}

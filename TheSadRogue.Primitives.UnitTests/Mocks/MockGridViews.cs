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
    /// TranslateGridView implementation which implements the most simple variation of TranslateGet, ie. the one which
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
    /// A TranslateGridView implementation which implements the version of TranslateGet
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

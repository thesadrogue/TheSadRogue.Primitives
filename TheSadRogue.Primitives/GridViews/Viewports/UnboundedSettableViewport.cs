using JetBrains.Annotations;
using SadRogue.Primitives.CoordinateSpaceTranslation;
using SadRogue.Primitives.UnboundedGridViews;

namespace SadRogue.Primitives.GridViews.Viewports
{
    /// <summary>
    /// Implements <see cref="ISettableGridView{T}"/> to expose a "viewport", or sub-area, of an unbounded grid view.
    /// Its indexers perform relative to absolute coordinate translations based on the viewport size/location, and
    /// return/set the proper value of type T from the underlying view.
    /// </summary>
    /// <remarks>
    /// This implementation, potentially paired with a <see cref="ViewportCoordinateSpaceTranslator{T}"/>, provides
    /// the code necessary to implement a settable viewport that can move around a potentially infinite grid and expose
    /// the section within the viewport as a grid view.  This allows you to expose an infinite grid view to an algorithm
    /// which expects a finite grid view, and have the algorithm work as if the grid were finite.
    /// </remarks>
    /// <typeparam name="T">The type being exposed by the viewport.</typeparam>
    [PublicAPI]
    public class UnboundedSettableViewport<T> : UnboundedViewport<T>, ISettableGridView<T>
    {
        /// <summary>
        /// The unbounded, settable grid view that this viewport is exposing values from.
        /// </summary>
        public new ISettableUnboundedGridView<T> GridView => (ISettableUnboundedGridView<T>)base.GridView;

        /// <inheritdoc/>
        public UnboundedSettableViewport(ISettableUnboundedGridView<T> gridView, Rectangle viewArea)
            : base(gridView, viewArea)
        { }

        /// <summary>
        /// Given a position in relative 1d-array-index style, returns/sets the "value" associated with that
        /// location in absolute coordinates.
        /// </summary>
        /// <param name="relativeIndex1D">
        /// Viewport-relative position of the location to retrieve/set the value for, as a 1D array index.
        /// </param>
        /// <returns>
        /// The "value" associated with the absolute location represented on the underlying map view.
        /// </returns>
        public new T this[int relativeIndex1D]
        {
            get => base[relativeIndex1D];
            set => GridView[ViewArea.Position + Point.FromIndex(relativeIndex1D, Width)] = value;
        }

        /// <summary>
        /// Given a position in relative coordinates, sets/returns the "value" associated with that
        /// location in absolute coordinates.
        /// </summary>
        /// <param name="relativePosition">
        /// Viewport-relative position of the location to retrieve/set the value for.
        /// </param>
        /// <returns>
        /// The "value" associated with the absolute location represented on the underlying map view.
        /// </returns>
        public new T this[Point relativePosition]
        {
            get => base[relativePosition];
            set => GridView[ViewArea.Position + relativePosition] = value;
        }

        /// <summary>
        /// Given an X and Y value in relative coordinates, sets/returns the "value" associated with
        /// that location in absolute coordinates.
        /// </summary>
        /// <param name="relativeX">Viewport-relative X-value of location.</param>
        /// <param name="relativeY">Viewport-relative Y-value of location.</param>
        /// <returns>
        /// The "value" associated with the absolute location represented on the underlying map view.
        /// </returns>
        public new T this[int relativeX, int relativeY]
        {
            get => base[relativeX, relativeY];
            set => GridView[ViewArea.X + relativeX, ViewArea.Y + relativeY] = value;
        }
    }
}

using System;

namespace SadRogue.Primitives.GridViews
{
    /// <summary>
    /// Implements <see cref="IGridView{T}"/> to expose a "viewport", or sub-area, of an unbounded grid view.
    /// Its indexers perform relative to absolute coordinate translations based on the viewport size/location, and
    /// return the proper value of type T from the underlying view.
    /// </summary>
    /// <remarks>
    /// This implementation, potentially paired with a <see cref="ViewportCoordinateSpaceTranslator"/>, provides
    /// the code necessary to implement a viewport that can move around a potentially infinite grid and expose the
    /// section within the viewport as a grid view.  This allows you to expose an infinite grid view to an algorithm
    /// which expects a finite grid view, and have the algorithm work as if the grid were finite.
    /// </remarks>
    /// <typeparam name="T">The type being exposed by the Viewport.</typeparam>
    public class UnboundedViewport<T> : GridViewBase<T>
    {
        // Analyzer misreads this because of ref return
#pragma warning disable IDE0044
        private Rectangle _viewArea;
#pragma warning restore IDE0044

        /// <summary>
        /// Constructor. Takes a unbounded grid view, and the initial subsection of that grid view to represent.
        /// </summary>
        /// <param name="gridView">The unbounded grid view being represented.</param>
        /// <param name="viewArea">The initial subsection of that grid to represent.</param>
        public UnboundedViewport(IUnboundedGridView<T> gridView, Rectangle viewArea)
        {
            GridView = gridView;
            _viewArea = viewArea;
        }

        /// <summary>
        /// The grid view that this UnboundedViewport is exposing values from.
        /// </summary>
        public IUnboundedGridView<T> GridView { get; private set; }

        /// <summary>
        /// The area of the base grid view that this viewport is exposing. Although this property does
        /// not explicitly expose a set accessor, it is returning a reference and as such may be
        /// assigned to.
        /// </summary>
        public ref Rectangle ViewArea => ref _viewArea;

        /// <summary>
        /// The height of the area being represented.
        /// </summary>
        public override int Height => _viewArea.Height;

        /// <summary>
        /// The width of the area being represented.
        /// </summary>
        public override int Width => _viewArea.Width;

        /// <summary>
        /// Given a position in relative coordinates, returns the "value" associated with that
        /// location in absolute coordinates.
        /// </summary>
        /// <param name="relativePosition">
        /// Viewport-relative position of the location to retrieve the value for.
        /// </param>
        /// <returns>
        /// The "value" associated with the absolute location represented on the underlying grid view.
        /// </returns>
        public override T this[Point relativePosition] => GridView[ViewArea.Position + relativePosition];

        /// <summary>
        /// Returns a string representation of the grid values inside the viewport.
        /// </summary>
        /// <returns>A string representation of the grid values inside the viewport.</returns>
        public override string ToString() => this.ExtendToString();

        /// <summary>
        /// Returns a string representation of the grid values inside the viewport, using
        /// <paramref name="elementStringifier" /> to determine what string represents each value.
        /// </summary>
        /// <param name="elementStringifier">
        /// Function determining the string representation of each element.
        /// </param>
        /// <returns>A string representation of the grid values inside the viewport.</returns>
        public string ToString(Func<T, string> elementStringifier)
            => this.ExtendToString(elementStringifier: elementStringifier);

        /// <summary>
        /// Returns a string representation of the grid values inside the viewport, using the function specified to turn
        /// elements into strings, and using the "field length" specified.
        /// </summary>
        /// <remarks>
        /// Each element of type T will have spaces added to cause it to take up exactly
        /// <paramref name="fieldSize" /> characters, provided <paramref name="fieldSize" />
        /// is less than the length of the element's string representation.
        /// </remarks>
        /// <param name="fieldSize">
        /// The size of the field to give each value.  A positive-number
        /// right-aligns the text within the field, while a negative number left-aligns the text.
        /// </param>
        /// <param name="elementStringifier">
        /// Function to use to convert each element to a string. null defaults to the ToString
        /// function of type T.
        /// </param>
        /// <returns>A string representation of the grid values inside the viewport.</returns>
        public string ToString(int fieldSize, Func<T, string>? elementStringifier = null)
            => this.ExtendToString(fieldSize, elementStringifier: elementStringifier);
    }
}

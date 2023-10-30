using JetBrains.Annotations;

namespace SadRogue.Primitives.GridViews.Viewports
{
    /// <summary>
    /// A grid view which exposes an arbitrary subsection of another grid view.  The grid view can be either unbounded,
    /// or a proper grid view.
    /// </summary>
    /// <remarks>
    /// In general, a viewport's indexers will take in a relative coordinate (relative to the viewport's position),
    /// and return the value from the underlying grid view at the absolute coordinate represented by that relative
    /// coordinate.  The width and height of the viewport are the same as the width and height of the viewport's
    /// view area.
    /// </remarks>
    /// <typeparam name="T">Type of items exposed for each location in the grid view.</typeparam>
    [PublicAPI]
    public interface IViewport<out T> : IGridView<T>
    {
        /// <summary>
        /// The area of the underlying grid view that this Viewport is exposing.
        /// </summary>
        public ref readonly Rectangle ViewArea { get; }
    }
}

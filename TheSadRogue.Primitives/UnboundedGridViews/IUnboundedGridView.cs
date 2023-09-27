using SadRogue.Primitives.GridViews;
using JetBrains.Annotations;

namespace SadRogue.Primitives.UnboundedGridViews
{
    /// <summary>
    /// An interface similar to <see cref="IGridView{T}"/>, except for it doesn't have a defined size.  Instead, it
    /// simply maps arbitrary 2D coordinates to a value.
    /// </summary>
    /// <remarks>
    ///  This is useful for representing infinite grids, grids that are chunk-loaded dynamically, or grids that have
    /// coordinate system which do not start at (0, 0).  Unlike <see cref="IGridView{T}"/>, coordinates given to the
    /// indexers in this interface are completely arbitrary, and do not need to be within any bounds.
    /// </remarks>
    /// <typeparam name="T">The type of item coordinates are mapped to.</typeparam>
    [PublicAPI]
    public interface IUnboundedGridView<out T>
    {
        /// <summary>
        /// Given an X and Y value, returns the "value" associated with that location.
        /// </summary>
        /// <remarks>
        /// Typically, this can be implemented via <see cref="this[Point]"/>.
        /// </remarks>
        /// <param name="x">X-value of location.</param>
        /// <param name="y">Y-value of location.</param>
        /// <returns>The "value" associated with that location.</returns>
        T this[int x, int y] { get; }

        /// <summary>
        /// Given a position, returns the "value" associated with that location.
        /// </summary>
        /// <param name="pos">Location to retrieve the value for.</param>
        /// <returns>The "value" associated with the provided location.</returns>
        T this[Point pos] { get; }
    }
}

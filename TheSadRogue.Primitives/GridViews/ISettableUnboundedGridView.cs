using JetBrains.Annotations;

namespace SadRogue.Primitives.GridViews
{
    /// <summary>
    /// An interface similar to <see cref="ISettableGridView{T}"/>, except for it doesn't have a defined size.  Instead, it
    /// simply maps arbitrary 2D coordinates to a value.
    /// </summary>
    /// <remarks>
    ///  This is useful for representing infinite grids, grids that are chunk-loaded dynamically, or grids that have
    /// coordinate system which do not start at (0, 0).  Unlike <see cref="ISettableGridView{T}"/>, coordinates given to
    /// the indexers in this interface are completely arbitrary, and do not need to be within any bounds.
    /// </remarks>
    /// <typeparam name="T">The type of item coordinates are mapped to.</typeparam>
    [PublicAPI]
    public interface ISettableUnboundedGridView<T> : IUnboundedGridView<T>
    {
        /// <summary>
        /// Given an X and Y value, returns/sets the "value" associated with that location.
        /// </summary>
        /// <remarks>
        /// Typically, this can be implemented via <see cref="this[Point]"/>.
        /// </remarks>
        /// <param name="x">X-value of location.</param>
        /// <param name="y">Y-value of location.</param>
        /// <returns>The "value" associated with that location.</returns>
        new T this[int x, int y] { get; set; }

        /// <summary>
        /// Given a position, returns/sets the "value" associated with that location.
        /// </summary>
        /// <param name="pos">Location to get/set the value for.</param>
        /// <returns>The "value" associated with the provided location.</returns>
        new T this[Point pos] { get; set; }
    }
}

using SadRogue.Primitives.GridViews;

namespace SadRogue.Primitives.CoordinateSpaceTranslation
{
    /// <summary>
    /// Interface representing the pairing of a coordinate space translator, and grid view whose coordinates represent
    /// coordinates in the translator's _local_ coordinate space.  Its indexers take global coordinates, and use the
    /// coordinate space translator to translate the coordinates to local and retrieve the corresponding value from the
    /// underlying grid view.
    /// </summary>
    /// <typeparam name="T">Type of values in the grid view being wrapped.</typeparam>
    /// <remarks>
    /// A typical use case of this interface is for exposing the output of algorithms which abstract away the concept of
    /// performing coordinate space translation, and wish to expose their results as a grid view.  The algorithm can then
    /// operate on its input grid view assuming it represents global coordinates, and use an implementation of this interface
    /// to expose its results in global coordinate space.
    /// </remarks>
    public interface ILocalCoordinateGridViewTranslator<out T>
    {
        /// <summary>
        /// A grid view whose (0, 0) -> (Width - 1, Height - 1) represent coordinates in <see cref="Translator"/>'s _local_
        /// coordinate space.
        /// </summary>
        IGridView<T> LocalGridView { get; }

        /// <summary>
        /// A coordinate space translator whose <see cref="ICoordinateSpaceTranslator.GlobalToLocalPosition"/> function
        /// returns coordinates in the grid view's coordinate range.
        /// </summary>
        ICoordinateSpaceTranslator Translator { get; }

        /// <summary>
        /// Gets the value for the given _global_ coordinate from the grid view.
        /// </summary>
        /// <param name="globalPos">Position, in the global coordinate space of <see cref="Translator"/>.</param>
        T this[Point globalPos] { get; }

        /// <summary>
        /// Gets the value for the given _global_ coordinate from the grid view.
        /// </summary>
        /// <param name="globalX">X-value of a position, in the global coordinate space of <see cref="Translator"/>.</param>
        /// <param name="globalY">Y-value of a position, in the global coordinate space of <see cref="Translator"/>.</param>
        T this[int globalX, int globalY] { get; }

        /// <summary>
        /// Simply returns the value from the corresponding indexer in <see cref="LocalGridView"/>.  This means there
        /// is NO coordinate translation taking place.
        /// </summary>
        /// <param name="index">Index of the value in <see cref="LocalGridView"/> to retrieve.</param>
        T this[int index] { get; }
    }
}

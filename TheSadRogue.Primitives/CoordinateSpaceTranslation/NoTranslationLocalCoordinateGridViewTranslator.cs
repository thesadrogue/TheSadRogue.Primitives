using System;
using SadRogue.Primitives.GridViews;

namespace SadRogue.Primitives.CoordinateSpaceTranslation
{
    /// <summary>
    /// A "dummy" implementation of <see cref="ILocalCoordinateGridViewTranslator{T}"/> which does not actually take
    /// a coordinate space translator, and performs no translation.  Instead, its indexers simply forward their parameters
    /// to the corresponding indexers in the given grid view.
    /// </summary>
    /// <typeparam name="T">Type of values in the grid view being wrapped.</typeparam>
    /// <remarks>
    /// This class is designed for use in abstractions which require <see cref="ILocalCoordinateGridViewTranslator{T}"/>,
    /// implementations, but don't actually need to perform any translation.
    /// </remarks>
    public class NoTranslationLocalCoordinateGridViewTranslator<T> : ILocalCoordinateGridViewTranslator<T>
    {
        /// <summary>
        /// The grid view whose values are returned from this class's indexers.
        /// </summary>
        public IGridView<T> LocalGridView { get; }

        /// <summary>
        /// Not implemented; this implementation has no translator.
        /// </summary>
        /// <exception cref="NotSupportedException">Always thrown if this property is accessed.</exception>
        public ICoordinateSpaceTranslator Translator => throw new NotSupportedException(
            $"{nameof(NoTranslationLocalCoordinateGridViewTranslator<T>)}" +
            " implementations do not have a translator attached.  If translation is required, use a different implementation of this interface.");

        /// <summary>
        /// Equivalent to calling LocalGridView[globalPos].
        /// </summary>
        /// <param name="globalPos">The position in the underlying grid view to retrieve the value for.</param>
        public T this[Point globalPos] => LocalGridView[globalPos];

        /// <summary>
        /// Equivalent to calling LocalGridView[globalX, globalY].
        /// </summary>
        /// <param name="globalX">X-value of the position in the underlying grid view to retrieve the value for.</param>
        /// <param name="globalY">Y-value of the position in the underlying grid view to retrieve the value for.</param>
        public T this[int globalX, int globalY] => LocalGridView[globalX, globalY];

        /// <summary>
        /// Equivalent to calling LocalGridView[index].
        /// </summary>
        /// <param name="index">The index in the underlying grid view to retrieve the value for.</param>
        public T this[int index] => LocalGridView[index];

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gridView">The grid view whose values are returned from this class's indexers.</param>
        public NoTranslationLocalCoordinateGridViewTranslator(IGridView<T> gridView)
        {
            LocalGridView = gridView;
        }
    }
}

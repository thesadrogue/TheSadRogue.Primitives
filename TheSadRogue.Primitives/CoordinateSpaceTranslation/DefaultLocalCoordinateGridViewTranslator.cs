using SadRogue.Primitives.GridViews;

namespace SadRogue.Primitives.CoordinateSpaceTranslation
{
    /// <summary>
    /// Class which implements <see cref="ILocalCoordinateGridViewTranslator{T}"/> in the typical way; by having a
    /// coordinate space translator, and grid view whose coordinates represent coordinates in the
    /// translator's _local_ coordinate space.  Its indexers take global coordinates, and use the coordinate
    /// space translator to translate the coordinates to local and retrieve the corresponding value from the underlying
    /// grid view.
    /// </summary>
    /// <typeparam name="T">Type of values in the grid view being wrapped.</typeparam>
    public class DefaultLocalCoordinateGridViewTranslator<T> : ILocalCoordinateGridViewTranslator<T>
    {
        /// <inheritdoc />
        public IGridView<T> LocalGridView {get; }

        /// <inheritdoc />
        public ICoordinateSpaceTranslator Translator { get; }

        /// <inheritdoc />
        public T this[Point globalPos] => LocalGridView[Translator.GlobalToLocalPosition(globalPos)];

        /// <inheritdoc />
        public T this[int globalX, int globalY] => LocalGridView[Translator.GlobalToLocalPosition(new Point(globalX, globalY))];

        /// <inheritdoc />
        public T this[int index] => LocalGridView[index];

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="localGridView">
        /// A grid view whose (0, 0) -> (Width - 1, Height - 1) represent coordinates in <see cref="Translator"/>'s _local_
        /// coordinate space.
        /// </param>
        /// <param name="translator">
        /// A coordinate space translator whose <see cref="ICoordinateSpaceTranslator.GlobalToLocalPosition"/> function
        /// returns coordinates in the grid view's coordinate range.
        /// </param>
        public DefaultLocalCoordinateGridViewTranslator(IGridView<T> localGridView, ICoordinateSpaceTranslator translator)
        {
            LocalGridView = localGridView;
            Translator = translator;
        }
    }
}

using JetBrains.Annotations;
using SadRogue.Primitives.GridViews.Viewports;

namespace SadRogue.Primitives.CoordinateSpaceTranslation
{
    /// <summary>
    /// A coordinate space translator which defines a coordinate space translator based on a viewport.  The local
    /// coordinate space is defined as the view area, and it uses the viewport's position to define the global
    /// coordinate space.  The coordinate space translator's definitions are updated based on the viewport's
    /// view area when <see cref="UpdateSpaceDefinition"/> is called.
    /// </summary>
    /// <typeparam name="T">The type of values associated with positions in the viewport.</typeparam>
    [PublicAPI]
    public class ViewportCoordinateSpaceTranslator<T> : ICoordinateSpaceTranslator
    {
        /// <summary>
        /// The viewport being used to define the local coordinate space.
        /// </summary>
        public IViewport<T> Viewport { get; }

        /// <summary>
        /// The current global coordinate representing the top-left corner of the local coordinate space.  To update
        /// this, call <see cref="UpdateSpaceDefinition"/>, which will query the viewport for its view area.
        /// </summary>
        public Point CurrentOffset { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="OffsetBasedCoordinateSpaceTranslator"/> with the given
        /// offset provider function.
        /// </summary>
        /// <param name="viewport">The viewport to use to define the local coordinate space.</param>
        public ViewportCoordinateSpaceTranslator(IViewport<T> viewport)
        {
            Viewport = viewport;
        }

        /// <inheritdoc />
        public Point GlobalToLocalPosition(Point position) => position - CurrentOffset;

        /// <inheritdoc />
        public Point LocalToGlobalPosition(Point position) => position + CurrentOffset;

        /// <inheritdoc />
        public void UpdateSpaceDefinition()
            => CurrentOffset = Viewport.ViewArea.Position;
    }
}

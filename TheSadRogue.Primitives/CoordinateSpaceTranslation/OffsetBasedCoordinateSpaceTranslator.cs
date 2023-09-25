using System;
using JetBrains.Annotations;

namespace SadRogue.Primitives.CoordinateSpaceTranslation
{
    /// <summary>
    /// A traditional coordinate space translation from some "global" coordinate space to a sub-area's coordinate space,
    /// where the offset specified (in global coordinates) is the top-left corner (aka (0, 0)) in the local coordinate
    /// space.
    /// </summary>
    [PublicAPI]
    public class OffsetBasedCoordinateSpaceTranslator : ICoordinateSpaceTranslator
    {
        /// <summary>
        /// A function which returns the global coordinate which should be (0, 0) in the local coordinate space.
        /// </summary>
        public Func<Point> OffsetProvider;

        /// <summary>
        /// The current global coordinate representing the top-left corner of the local coordinate space.  To update
        /// this, call <see cref="UpdateSpaceDefinition"/>, which will use the <see cref="OffsetProvider"/> to
        /// retrieve the current offset.
        /// </summary>
        public Point CurrentOffset { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="OffsetBasedCoordinateSpaceTranslator"/> with the given
        /// offset provider function.
        /// </summary>
        /// <param name="offsetProvider">The value to use for <see cref="OffsetProvider"/>.</param>
        public OffsetBasedCoordinateSpaceTranslator(Func<Point> offsetProvider)
        {
            OffsetProvider = offsetProvider;
        }

        /// <inheritdoc />
        public Point GlobalToLocalPosition(Point position) => position - CurrentOffset;

        /// <inheritdoc />
        public Point LocalToGlobalPosition(Point position) => position + CurrentOffset;

        /// <inheritdoc />
        public void UpdateSpaceDefinition()
            => CurrentOffset = OffsetProvider();
    }
}

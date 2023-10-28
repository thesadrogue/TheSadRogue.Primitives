using JetBrains.Annotations;
using SadRogue.Primitives.CoordinateSpaceTranslation;

namespace SadRogue.Primitives.GridViews.Viewports
{
    /// <summary>
    /// A collection of extension methods for <see cref="IViewport{T}"/>.
    /// </summary>
    [PublicAPI]
    public static class ViewportExtensions
    {
        /// <summary>
        /// Returns a <see cref="ICoordinateSpaceTranslator"/> that uses the given viewport to define the local
        /// coordinate space.  The returned translator will update its definition of the local coordinate space
        /// only when <see cref="ICoordinateSpaceTranslator.UpdateSpaceDefinition"/> is called; however when called,
        /// local coordinates will be defined as viewport-relative, and global coordinates will be defined as
        /// coordinates from the underlying grid view being exposed by the viewport.
        /// </summary>
        /// <param name="viewport">The viewport to use to define the coordinate space translator.</param>
        /// <typeparam name="T">The type of items being exposed by the viewport.</typeparam>
        /// <returns>
        /// A coordinate space translator which defines global coordinates as coordinates in the underlying
        /// grid view, and local coordinates as viewport-relative coordinates.
        /// </returns>
        public static ICoordinateSpaceTranslator GetCoordinateSpaceTranslator<T>(this IViewport<T> viewport)
            => new ViewportCoordinateSpaceTranslator<T>(viewport);
    }
}

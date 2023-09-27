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
        public static ICoordinateSpaceTranslator GetCoordinateSpaceTranslator<T>(this IViewport<T> viewport)
            => new ViewportCoordinateSpaceTranslator<T>(viewport);
    }
}

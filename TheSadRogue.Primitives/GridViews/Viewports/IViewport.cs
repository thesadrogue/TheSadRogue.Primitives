using JetBrains.Annotations;

namespace SadRogue.Primitives.GridViews.Viewports
{
    [PublicAPI]
    public interface IViewport<out T> : IGridView<T>
    {
        public ref readonly Rectangle ViewArea { get; }
    }
}

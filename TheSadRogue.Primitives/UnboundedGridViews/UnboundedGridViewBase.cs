using JetBrains.Annotations;

namespace SadRogue.Primitives.UnboundedGridViews
{
    /// <summary>
    /// A convenient base class to inherit from when implementing <see cref="IUnboundedGridView{T}"/> that minimizes
    /// the number of items you must implement by implementing indexers in terms of a single indexer taking a Point.
    /// </summary>
    /// <typeparam name="T">The type of value being returned by the indexer functions.</typeparam>
    [PublicAPI]
    public abstract class UnboundedGridViewBase<T> : IUnboundedGridView<T>
    {
        /// <inheritdoc />
        public T this[int x, int y] => this[new Point(x, y)];

        /// <inheritdoc />
        public abstract T this[Point pos] { get; }
    }
}

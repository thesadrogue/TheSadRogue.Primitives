using JetBrains.Annotations;

namespace SadRogue.Primitives.UnboundedGridViews
{
    /// <summary>
    /// A convenient base class to inherit from when implementing <see cref="ISettableUnboundedGridView{T}"/> that minimizes
    /// the number of items you must implement by implementing indexers in terms of a single indexer taking a Point.
    /// </summary>
    /// <typeparam name="T">The type of value being returned by the indexer functions.</typeparam>
    [PublicAPI]
    public abstract class SettableUnboundedGridViewBase<T> : ISettableUnboundedGridView<T>
    {
        /// <inheritdoc cref="ISettableUnboundedGridView{T}"/>
        public T this[int x, int y]
        {
            get => this[new Point(x, y)];
            set => this[new Point(x, y)] = value;
        }

        /// <inheritdoc cref="ISettableUnboundedGridView{T}"/>
        public abstract T this[Point pos] { get; set; }
    }
}

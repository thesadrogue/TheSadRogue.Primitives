using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace SadRogue.Primitives.GridViews
{
    /// <summary>
    /// A convenient base class to inherit from when implementing <see cref="ISettableGridView{T}"/> that minimizes
    /// the number of items you must implement by implementing indexers in terms of a single indexer taking a Point.
    /// </summary>
    [PublicAPI]
    public abstract class SettableGridViewBase<T> : ISettableGridView<T>
    {
        /// <inheritdoc />
        public abstract int Height { get; }

        /// <inheritdoc />
        public abstract int Width { get; }

        /// <inheritdoc cref="ISettableGridView{T}"/>
        public abstract T this[Point pos] { get; set; }

        /// <inheritdoc />
        public int Count => Width * Height;

        /// <inheritdoc cref="ISettableGridView{T}"/>
        public T this[int x, int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[new Point(x, y)];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => this[new Point(x, y)] = value;
        }

        /// <inheritdoc cref="ISettableGridView{T}"/>
        public T this[int index1D]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[Point.FromIndex(index1D, Width)];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => this[Point.FromIndex(index1D, Width)] = value;
        }

        /// <inheritdoc/>
        public virtual void Fill(T value)
        {
            for (int i = 0; i < Count; i++)
                this[i] = value;
        }

        /// <inheritdoc/>
        public virtual void Clear() => Fill(default!);
    }
}

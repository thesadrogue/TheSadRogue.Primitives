using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace SadRogue.Primitives.GridViews
{
    /// <summary>
    /// Exactly like <see cref="GridViewBase{T}"/>, except for the one indexer left to the user to implement
    /// is the one which takes a 1D array, and the position-based indexers are implemented off that.
    /// </summary>
    /// <remarks>
    /// This can be more convenient than <see cref="GridViewBase{T}"/> for use cases where 1D indices are easiest
    /// to work with, and is technically more efficient for cases such as wrapping a 1D array, where the backing data
    /// structure takes an index (although this should typically be considered a micro-optimization).
    /// </remarks>
    /// <typeparam name="T">The type of value being stored.</typeparam>
    [PublicAPI]
    public abstract class GridView1DIndexBase<T> : IGridView<T>
    {
        /// <inheritdoc />
        public abstract int Height { get; }
        /// <inheritdoc />
        public abstract int Width { get; }

        /// <inheritdoc />
        public T this[Point pos]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[pos.ToIndex(Width)];
        }

        /// <inheritdoc />
        public int Count => Width * Height;

        /// <inheritdoc />
        public T this[int x, int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[new Point(x, y)];
        }

        /// <inheritdoc />
        public abstract T this[int index1D] { get; }
    }
}

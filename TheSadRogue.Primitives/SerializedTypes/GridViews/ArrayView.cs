using System;
using SadRogue.Primitives.GridViews;

namespace SadRogue.Primitives.SerializedTypes.GridViews
{
    /// <summary>
    /// Serializable (pure-data) object representing an <see cref="ArrayView{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of value being exposed via the grid view.</typeparam>
    [Serializable]
    public struct ArrayViewSerialized<T>
    {
        /// <summary>
        /// Width of the array view.
        /// </summary>
        public int Width;

        /// <summary>
        /// The data from the array view.
        /// </summary>
        public T[] Data;

        /// <summary>
        /// Converts <see cref="ArrayView{T}"/> to <see cref="ArrayViewSerialized{T}"/>.
        /// </summary>
        /// <param name="arrayView"/>
        /// <returns/>
        public static implicit operator ArrayViewSerialized<T>(ArrayView<T> arrayView)
            => new ArrayViewSerialized<T>()
            {
                Width = arrayView.Width,
                Data = (ArrayView<T>)arrayView.Clone()
            };

        /// <summary>
        /// Converts <see cref="ArrayViewSerialized{T}"/> to <see cref="ArrayView{T}"/>.
        /// </summary>
        /// <param name="arrayView"/>
        /// <returns/>
        public static implicit operator ArrayView<T>(ArrayViewSerialized<T> arrayView)
            => new ArrayView<T>(arrayView.Data, arrayView.Width);
    }
}

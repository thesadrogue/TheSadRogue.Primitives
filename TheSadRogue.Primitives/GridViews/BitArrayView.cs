using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace SadRogue.Primitives.GridViews
{
    /// <summary>
    /// A grid view that wraps a C# BitArray into a settable grid view of boolean values.
    /// </summary>
    /// <remarks>
    /// This grid view con be useful to represent a region or area of a 2d grid where points are either "on" or "off".
    /// HashSet&lt;Point&gt; can work for this purpose, but hashing can be slow.  bool[] or ArrayView&lt;bool&gt;
    /// are other options, but this class uses approximately 8x less memory than those options, and is only very slightly
    /// slower (less than 0.5ns) in terms of index access.  The Fill operation is actually much faster than the
    /// corresponding operation for a boolean array, which can make it a very useful alternative as a "visited" array
    /// when iterating over a grid.
    /// </remarks>
    public sealed class BitArrayView : SettableGridView1DIndexBase<bool>, ICloneable, IMatchable<BitArrayView>
    {
        private readonly BitArray _array;

        /// <inheritdoc />
        public override int Height { get; }

        /// <inheritdoc />
        public override int Width { get; }

        /// <inheritdoc />
        public override bool this[int index1D]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _array[index1D];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _array[index1D] = value;
        }

        /// <summary>
        /// Constructor. Takes width and height of array to create.
        /// </summary>
        /// <param name="width">Width of array.</param>
        /// <param name="height">Height of array.</param>
        public BitArrayView(int width, int height)
            : this(new BitArray(width * height), width)
        { }

        /// <summary>
        /// Constructor.  Takes an existing 1D array to use as the underlying array, and
        /// the width of the 2D grid represented by that array.
        /// </summary>
        /// <param name="existingArray">Existing 1D array to use as the underlying array.</param>
        /// <param name="width">The width of the 2D grid represented by <paramref name="existingArray" />.</param>
        public BitArrayView(BitArray existingArray, int width)
        {
            if (existingArray.Length % width != 0)
                throw new ArgumentException($"Existing {nameof(BitArray)} must have length equal to {nameof(width)}*height.",
                    nameof(existingArray));

            _array = existingArray;
            Width = width;
            Height = existingArray.Length / width;
        }

        /// <summary>
        /// Performs deep copy of bit-array view.
        /// </summary>
        /// <returns>The cloned BitArrayView.</returns>
        public object Clone()
        {
            var newObj = new BitArrayView(Width, Height);

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    newObj[x, y] = _array[Point.ToIndex(x, y, Width)];

            return newObj;
        }

        /// <summary>
        /// Compares the current BitArrayView to the one given.
        /// </summary>
        /// <param name="other" />
        /// <returns>True if the given BitArrayView references the same underlying bit-array, false otherwise.</returns>
        public bool Matches(BitArrayView? other) => !(other is null) && _array == other._array;

        /// <summary>
        /// Allows implicit conversion to BitArray.  Does not copy the underlying values.
        /// </summary>
        /// <param name="arrayView">BitArrayView to convert.</param>
        public static implicit operator BitArray(BitArrayView arrayView) => arrayView._array;

        /// <summary>
        /// Converts to BitArray, without copying the values.  Typically using this method is unnecessary
        /// and you can use the implicit conversion defined for this type instead.
        /// </summary>
        /// <returns>The underlying BitArray data as a 1D array.</returns>
        public BitArray ToBitArray() => this;

        /// <summary>
        /// Sets each location in the grid view to the value specified.
        /// </summary>
        /// <remarks>
        /// This method is much faster than the typical Fill extension method for grid views; and is even faster than an
        /// equivalently sized boolean array's Clear operation.
        /// </remarks>
        /// <param name="value">Value to fill the grid view with.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Fill(bool value) => _array.SetAll(value);

        /// <summary>
        /// Returns a string representation of the grid values.
        /// </summary>
        /// <returns>A string representation of the grid values.</returns>
        public override string ToString() => this.ExtendToString();

        /// <summary>
        /// Returns a string representation of the grid values, using <paramref name="elementStringifier" />
        /// to determine what string represents each value.
        /// </summary>
        /// <param name="elementStringifier">
        /// Function determining the string representation of each value.
        /// </param>
        /// <returns>A string representation of the grid values.</returns>
        public string ToString(Func<bool, string> elementStringifier)
            => this.ExtendToString(elementStringifier: elementStringifier);

        /// <summary>
        /// Returns a string representation of the grid values using the given parameters.
        /// </summary>
        /// <remarks>
        /// Each element will have spaces added to cause it to take up exactly
        /// <paramref name="fieldSize" /> characters, provided <paramref name="fieldSize" />
        /// is less than the length of the value's string representation.
        /// </remarks>
        /// <param name="fieldSize">
        /// The size of the field to give each value.  A positive-number
        /// right-aligns the text within the field, while a negative number left-aligns the text.
        /// </param>
        /// <param name="elementStringifier">
        /// Function to use to convert each value to a string. null defaults to the ToString
        /// function of type T.
        /// </param>
        /// <returns>A string representation of the grid values.</returns>
        public string ToString(int fieldSize, Func<bool, string>? elementStringifier = null)
            => this.ExtendToString(fieldSize, elementStringifier: elementStringifier);
    }
}

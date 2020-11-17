﻿using System;

namespace SadRogue.Primitives.GridViews
{
    /// <summary>
    /// Implementation of <see cref="ISettableGridView{T}" /> that uses a 2D array to store data.
    /// </summary>
    /// <remarks>
    /// An <see cref="ArrayView2D{T}" /> can be implicitly converted to its underlying 2D array,
    /// which allows exposing that array to code that works with 2D arrays.  Modifications in the array
    /// appear in the map view as well.
    ///
    /// If you need a 1D array instead of 2D, then you should use <see cref="ArrayView{T}" /> instead.
    /// </remarks>
    /// <typeparam name="T">The type of value being stored.</typeparam>
    public sealed class ArrayView2D<T> : SettableGridViewBase<T>, ICloneable, IEquatable<ArrayView2D<T>>
    {
        private readonly T[,] _array;

        /// <summary>
        /// Constructor. Takes width and height of array to create.
        /// </summary>
        /// <param name="width">Width of array.</param>
        /// <param name="height">Height of array.</param>
        public ArrayView2D(int width, int height)
            : this(new T[width, height])
        { }

        /// <summary>
        /// Constructor.  Takes an existing 2D array to use as the underlying data structure.
        /// </summary>
        /// <param name="existingArray">An existing 2D array to use as the data structure.</param>
        public ArrayView2D(T[,] existingArray) => _array = existingArray;

        /// <summary>
        /// Performs deep copy of array view.
        /// </summary>
        /// <returns>The cloned ArrayView2D.</returns>
        public object Clone()
        {
            var newObj = new ArrayView2D<T>(Width, Height);

            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                    newObj[x, y] = _array[x, y];

            return newObj;
        }

        /// <summary>
        /// Compares the current ArrayView2D to the one given.
        /// </summary>
        /// <param name="other" />
        /// <returns>True if the given ArrayView2D&lt;T&gt; with a reference to the same underlying array, false otherwise.</returns>
        public bool Equals(ArrayView2D<T>? other) => !(other is null) && _array == other._array;

        /// <inheritdoc />
        public override int Height => _array.GetLength(1);

        /// <inheritdoc />
        public override int Width => _array.GetLength(0);

        /// <inheritdoc />
        public override T this[Point pos]
        {
            get => _array[pos.X, pos.Y];
            set => _array[pos.X, pos.Y] = value;
        }


#pragma warning disable CA2225 // The proper equivalent function is provided, however because the type is [,] instead of MultidimensionalArray the analyzer cannot determine this properly.
        /// <summary>
        /// Allows implicit conversion to 2D array.  Does not copy the underlying values.
        /// </summary>
        /// <param name="arrayView">The ArrayView2D to convert.</param>
        public static implicit operator T[,](ArrayView2D<T> arrayView) => arrayView._array;
#pragma warning restore CA2225

        /// <summary>
        /// Converts to 2D array, without copying the values.  Typically using this method is unnecessary
        /// and you can use the implicit conversion defined for this type instead.
        /// </summary>
        /// <returns>The underlying ArrayView data as a 1D array.</returns>
        public T[,] ToMultidimensionalArray() => _array;

        /// <summary>
        /// Sets each element in the ArrayView to the default for type T.
        /// </summary>
        public void Clear() => Array.Clear(_array, 0, _array.Length);

        /// <summary>
        /// Compares the current ArrayView2D to the object given.
        /// </summary>
        /// <param name="obj" />
        /// <returns>
        /// True if the given object is an ArrayView2D&lt;T&gt; with a reference to the same underlying array, false
        /// otherwise.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (obj is ArrayView2D<T> e)
                return Equals(e);

            return false;
        }

        /// <summary>
        /// Returns a hash-value for this object.
        /// </summary>
        /// <returns />
        public override int GetHashCode() => _array.GetHashCode();

        /// <summary>
        /// Compares the two ArrayView2D instances.
        /// </summary>
        /// <param name="lhs" />
        /// <param name="rhs" />
        /// <returns>
        /// True if the two given ArrayView2D&lt;T&gt; instances have a reference to the same underlying array, false
        /// otherwise.
        /// </returns>
        public static bool operator ==(ArrayView2D<T>? lhs, ArrayView2D<T>? rhs) => lhs?.Equals(rhs) ?? rhs is null;

        /// <summary>
        /// Compares the two ArrayView2D instances.
        /// </summary>
        /// <param name="lhs" />
        /// <param name="rhs" />
        /// <returns>
        /// True if the two given ArrayView2D&lt;T&gt; instances do NOT have a reference to the same underlying array,
        /// false otherwise.
        /// </returns>
        public static bool operator !=(ArrayView2D<T>? lhs, ArrayView2D<T>? rhs) => !(lhs == rhs);

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
        /// <returns>A string representation of the 2D array.</returns>
        public string ToString(Func<T, string> elementStringifier)
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
        public string ToString(int fieldSize, Func<T, string>? elementStringifier = null)
            => this.ExtendToString(fieldSize, elementStringifier: elementStringifier);
    }
}
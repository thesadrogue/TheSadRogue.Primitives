﻿using System;
using JetBrains.Annotations;

namespace SadRogue.Primitives.GridViews
{
    /// <summary>
    /// Class implementing <see cref="ISettableGridView{T}"/>, by providing the "get" and "set" functionality via
    /// functions that are passed in at construction.  For a version that implements <see cref="IGridView{T}" />, see
    /// <see cref="LambdaGridView{T}" />.
    /// </summary>
    /// <typeparam name="T">The type of value being returned by the indexer functions.</typeparam>
    [PublicAPI]
    public sealed class LambdaSettableGridView<T> : SettableGridViewBase<T>
    {
        private readonly Func<int> _heightGetter;
        private readonly Func<Point, T> _valueGetter;
        private readonly Action<Point, T> _valueSetter;
        private readonly Func<int> _widthGetter;

        /// <summary>
        /// Constructor. Takes the width and height of the grid, and the functions to use to
        /// retrieve/set the value for a location.
        /// </summary>
        /// <remarks>
        /// This constructor is useful if the width and height of the underlying representation do
        /// not change, so they can safely be passed in as constants.
        /// </remarks>
        /// <param name="width">The (constant) width of the map.</param>
        /// <param name="height">The (constant) height of the map.</param>
        /// <param name="valueGetter">
        /// A function/lambda that returns the value of type T associated with the location it is given.
        /// </param>
        /// <param name="valueSetter">
        /// A function/lambda that updates the underlying representation of the grid being represented accordingly,
        /// given a type T and position to which it was set.
        /// </param>
        public LambdaSettableGridView(int width, int height, Func<Point, T> valueGetter, Action<Point, T> valueSetter)
            : this(() => width, () => height, valueGetter, valueSetter)
        { }

        /// <summary>
        /// Constructor. Takes functions that retrieve the width and height of the grid, and the
        /// functions used to retrieve/set the value for a location.
        /// </summary>
        /// <remarks>
        /// This constructor is useful if the width and height of the underlying representation may
        /// change -- one can provide functions that retrieve the width and height of the map being
        /// represented, and these functions will be called any time the <see cref="Width" /> and <see cref="Height" />
        /// properties are retrieved.
        /// </remarks>
        /// <param name="widthGetter">
        /// A function/lambda that retrieves the width of the grid being represented.
        /// </param>
        /// <param name="heightGetter">
        /// A function/lambda that retrieves the height of the grid being represented.
        /// </param>
        /// <param name="valueGetter">
        /// A function/lambda that returns the value of type T associated with the location it is given.
        /// </param>
        /// <param name="valueSetter">
        /// A function/lambda that updates the grid being represented accordingly, given a type T and
        /// position to which it was set.
        /// </param>
        public LambdaSettableGridView(Func<int> widthGetter, Func<int> heightGetter, Func<Point, T> valueGetter,
                                     Action<Point, T> valueSetter)
        {
            _widthGetter = widthGetter;
            _heightGetter = heightGetter;
            _valueGetter = valueGetter;
            _valueSetter = valueSetter;
        }

        /// <inheritdoc />
        public override int Height => _heightGetter();

        /// <inheritdoc />
        public override int Width => _widthGetter();

        /// <summary>
        /// Given a position, returns/sets the "value" associated with that location, by calling the
        /// valueGetter/valueSetter functions provided at construction.
        /// </summary>
        /// <param name="pos">Location to retrieve/set the value for.</param>
        /// <returns>
        /// The "value" associated with the provided location, according to the valueGetter function
        /// provided at construction.
        /// </returns>
        public override T this[Point pos]
        {
            get => _valueGetter(pos);
            set => _valueSetter(pos, value);
        }

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
        /// Function determining the string representation of each element.
        /// </param>
        /// <returns>A string representation of the grid values.</returns>
        public string ToString(Func<T, string> elementStringifier)
            => this.ExtendToString(elementStringifier: elementStringifier);

        /// <summary>
        /// Returns a string representation of the grid values, using the function specified to turn elements into
        /// strings, and using the "field length" specified.
        /// </summary>
        /// <remarks>
        /// Each element of type T will have spaces added to cause it to take up exactly
        /// <paramref name="fieldSize" /> characters, provided <paramref name="fieldSize" />
        /// is less than the length of the element's string representation.
        /// </remarks>
        /// <param name="fieldSize">
        /// The size of the field to give each value.  A positive-number
        /// right-aligns the text within the field, while a negative number left-aligns the text.
        /// </param>
        /// <param name="elementStringifier">
        /// Function to use to convert each element to a string. null defaults to the ToString
        /// function of type T.
        /// </param>
        /// <returns>A string representation of the grid values.</returns>
        public string ToString(int fieldSize, Func<T, string>? elementStringifier = null)
            => this.ExtendToString(fieldSize, elementStringifier: elementStringifier);
    }
}

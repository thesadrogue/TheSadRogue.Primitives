using System;
using JetBrains.Annotations;

namespace SadRogue.Primitives.UnboundedGridViews
{
    /// <summary>
    /// Class implementing <see cref="ISettableUnboundedGridView{T}"/>, by providing the "get" and "set" functionality
    /// via functions that are passed in at construction.  For a version that implements
    /// <see cref="IUnboundedGridView{T}" />, see <see cref="LambdaUnboundedGridView{T}" />.
    /// </summary>
    /// <typeparam name="T">The type of value being returned by the indexer functions.</typeparam>
    [PublicAPI]
    public class LambdaSettableUnboundedGridView<T> : SettableUnboundedGridViewBase<T>
    {
        private readonly Func<Point, T> _valueGetter;
        private readonly Action<Point, T> _valueSetter;

        /// <inheritdoc />
        public override T this[Point pos]
        {
            get => _valueGetter(pos);
            set => _valueSetter(pos, value);
        }

        /// <summary>
        /// Constructor. Takes the functions to use to retrieve/set the value for a location as parameters.
        /// </summary>
        /// <param name="valueGetter">
        /// A function/lambda that returns the value of type T associated with the location it is given.
        /// </param>
        /// <param name="valueSetter">
        /// A function/lambda that updates the underlying representation of the grid being represented accordingly,
        /// given a type T and position to which it was set.
        /// </param>
        public LambdaSettableUnboundedGridView(Func<Point, T> valueGetter, Action<Point, T> valueSetter)
        {
            _valueGetter = valueGetter;
            _valueSetter = valueSetter;
        }
    }
}

using System;
using JetBrains.Annotations;

namespace SadRogue.Primitives.UnboundedGridViews
{
    /// <summary>
    /// Class implementing <see cref="IUnboundedGridView{T}"/>, by providing the "get" functionality via a function that
    /// is passed in at construction.  For a version that implements <see cref="ISettableUnboundedGridView{T}" />, see
    /// <see cref="LambdaSettableUnboundedGridView{T}" />.
    /// </summary>
    /// <typeparam name="T">The type of value being returned by the indexer functions.</typeparam>
    [PublicAPI]
    public class LambdaUnboundedGridView<T> : UnboundedGridViewBase<T>
    {
        private readonly Func<Point, T> _valueGetter;

        /// <inheritdoc />
        public override T this[Point pos] => _valueGetter(pos);

        /// <summary>
        /// Constructor. Takes as a parameter the function to use to retrieve the value for a location.
        /// </summary>
        /// <param name="valueGetter">
        /// A lambda/function that returns the value of type T associated with the location it is given.
        /// This function is called each time the unbounded grid view's indexers are called upon to retrieve a value
        /// from a location.
        /// </param>
        public LambdaUnboundedGridView(Func<Point, T> valueGetter)
        {
            _valueGetter = valueGetter;
        }
    }
}

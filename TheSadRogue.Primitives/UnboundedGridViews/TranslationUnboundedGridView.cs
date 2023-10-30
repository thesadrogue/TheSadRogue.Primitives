using System;
using JetBrains.Annotations;

namespace SadRogue.Primitives.UnboundedGridViews
{
    /// <summary>
    /// Class implementing <see cref="IUnboundedGridView{T}"/> by providing a function that translates values from one
    /// grid view with complex data types, to an unbounded grid view with simple data types.  For a version that provides
    /// "set" functionality, see <see cref="SettableTranslationUnboundedGridView{T1,T2}" />.
    /// </summary>
    /// <remarks>
    /// This class is useful if the underlying representation of the data you are creating a grid view for is complex,
    /// and you simply need to map a complex data type to a simpler one.  For example, you might implement the
    /// <see cref="TranslateGet(SadRogue.Primitives.Point,T1)"/> function to extract a property from a more complex
    /// structure.  If your mapping is very simple, or you do not wish to create a subclass, see
    /// <see cref="LambdaTranslationUnboundedGridView{T1,T2}"/>.
    /// </remarks>
    /// <typeparam name="T1">The type of your underlying data.</typeparam>
    /// <typeparam name="T2">The type of the data being exposed by the grid view.</typeparam>
    [PublicAPI]
    public abstract class TranslationUnboundedGridView<T1, T2> : UnboundedGridViewBase<T2>
    {
        /// <summary>
        /// Constructor. Takes an existing grid view to create a view from.
        /// </summary>
        /// <param name="baseGrid">A grid view exposing your underlying data.</param>
        protected TranslationUnboundedGridView(IUnboundedGridView<T1> baseGrid) => BaseGrid = baseGrid;

        /// <summary>
        /// The underlying grid data, exposed as a grid view.
        /// </summary>
        public IUnboundedGridView<T1> BaseGrid { get; private set; }

        /// <summary>
        /// Given a position, translates and returns the "value" associated with that position.
        /// </summary>
        /// <param name="pos">Location to get the value for.</param>
        /// <returns>The translated "value" associated with the provided location.</returns>
        public override T2 this[Point pos] => TranslateGet(pos, BaseGrid[pos]);

        /// <summary>
        /// Translates your actual data into the view type using just the data value itself. If you need
        /// the location as well to perform the translation, implement <see cref="TranslateGet(Point, T1)" />
        /// instead.
        /// </summary>
        /// <param name="value">The data value from the base grid view.</param>
        /// <returns>A value of the mapped data type.</returns>
        protected virtual T2 TranslateGet(T1 value) =>
            throw new NotSupportedException(
                $"{nameof(TranslateGet)}(T1) was not implemented, and {nameof(TranslateGet)}(Point, T1) was not re-implemented.  One of these two functions must be implemented.");

        /// <summary>
        /// Translates your actual data into the view type using the position and the data value itself. If
        /// you need only the data value to perform the translation, implement <see cref="TranslateGet(T1)" />
        /// instead.
        /// </summary>
        /// <param name="position">The position of the given data value.</param>
        /// <param name="value">The data value from your underlying grid view.</param>
        /// <returns>A value of the mapped data type.</returns>
        protected virtual T2 TranslateGet(Point position, T1 value) => TranslateGet(value);
    }
}

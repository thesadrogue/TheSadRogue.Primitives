using System;
using JetBrains.Annotations;

namespace SadRogue.Primitives.UnboundedGridViews
{
    /// <summary>
    /// Class implementing <see cref="ISettableUnboundedGridView{T}"/> by providing a functions that translate values from one
    /// grid view with complex data types, to a grid view with simple data types, and vice versa.  For a version that
    /// provides only "get" functionality, see <see cref="TranslationUnboundedGridView{T1,T2}" />.
    /// </summary>
    /// <remarks>
    /// See <see cref="TranslationUnboundedGridView{T1,T2}" />.  The use case is the same, except that this class
    /// implements <see cref="ISettableUnboundedGridView{T}" /> instead, and thus also allows you to specify
    /// set-translations via TranslateSet.
    /// </remarks>
    /// <typeparam name="T1">The type of your underlying data.</typeparam>
    /// <typeparam name="T2">The type of the data being exposed by the grid view.</typeparam>
    [PublicAPI]
    public abstract class SettableTranslationUnboundedGridView<T1, T2> : SettableUnboundedGridViewBase<T2>
    {
        /// <summary>
        /// Constructor. Takes an existing grid view to create a view from.
        /// </summary>
        /// <param name="baseGrid">A grid view exposing your underlying map data.</param>
        protected SettableTranslationUnboundedGridView(ISettableUnboundedGridView<T1> baseGrid) => BaseGrid = baseGrid;

        /// <summary>
        /// The grid view exposing your underlying data.
        /// </summary>
        public ISettableUnboundedGridView<T1> BaseGrid { get; private set; }

        /// <summary>
        /// Given a position, translates and returns/sets the "value" associated with that position.
        /// </summary>
        /// <param name="pos">Location to get/set the value for.</param>
        /// <returns>The translated "value" associated with the provided location.</returns>
        public override T2 this[Point pos]
        {
            get => TranslateGet(pos, BaseGrid[pos]);
            set => BaseGrid[pos] = TranslateSet(pos, value);
        }

        /// <summary>
        /// Translates your underlying data into the view type. Takes only a value from the underlying data.
        /// If a position is also needed to perform the translation, use <see cref="TranslateGet(Point, T1)" />
        /// instead.
        /// </summary>
        /// <param name="value">The data value from your underlying data.</param>
        /// <returns>A value of the mapped data type.</returns>
        protected virtual T2 TranslateGet(T1 value) =>
            throw new NotSupportedException(
                $"{nameof(TranslateGet)}(T1) was not implemented, and {nameof(TranslateGet)}(Point, T1) was not re-implemented.  One of these two functions must be implemented.");

        /// <summary>
        /// Translates your underlying data into the view type. Takes a value from the underlying data and
        /// the corresponding position for that value. If a position is not needed to perform the
        /// translation, use <see cref="TranslateGet(T1)" /> instead.
        /// </summary>
        /// <param name="position">The position of the given data value from your underlying data structure.</param>
        /// <param name="value">The data value from your underlying structure.</param>
        /// <returns>A value of the mapped data type.</returns>
        protected virtual T2 TranslateGet(Point position, T1 value) => TranslateGet(value);

        /// <summary>
        /// Translates the view type into the appropriate form for your underlying data. Takes only a value
        /// from the grid view itself. If a position is also needed to perform the translation, use
        /// <see cref="TranslateSet(Point, T2)" /> instead.
        /// </summary>
        /// <param name="value">A value of the mapped data type.</param>
        /// <returns>The data value for your underlying representation.</returns>
        protected virtual T1 TranslateSet(T2 value) =>
            throw new NotSupportedException(
                $"{nameof(TranslateSet)}(T2) was not implemented, and {nameof(TranslateSet)}(Point, T2) was not re-implemented.  One of these two functions must be implemented.");

        /// <summary>
        /// Translates the view type into the appropriate form for your underlying data. Takes a value from
        /// the underlying data, and it corresponding position. If a position is not needed to perform
        /// the translation, use <see cref="TranslateSet(T2)" /> instead.
        /// </summary>
        /// <param name="position">The position of the given mapped data type.</param>
        /// <param name="value">A value of the mapped data type.</param>
        /// <returns>The data value for your underlying representation.</returns>
        protected virtual T1 TranslateSet(Point position, T2 value) => TranslateSet(value);
    }
}

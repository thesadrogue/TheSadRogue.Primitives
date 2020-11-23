using System;
using System.Collections.Generic;
using System.Text;

namespace SadRogue.Primitives.GridViews
{
    /// <summary>
    /// Extensions for <see cref="IGridView{T}" /> implementations that provide basic utility functions
    /// for them.
    /// </summary>
    public static class GridViewExtensions
    {
        /// <summary>
        /// Sets all the values of the current grid view to be equal to the corresponding values from
        /// the grid view you pass in.
        /// </summary>
        /// <typeparam name="T" />
        /// <param name="self" />
        /// <param name="overlay">
        /// The data apply to the view. Must have identical dimensions to the current view.
        /// </param>
        public static void ApplyOverlay<T>(this ISettableGridView<T> self, IGridView<T> overlay)
        {
            if (self.Height != overlay.Height || self.Width != overlay.Width)
                throw new ArgumentException("Overlay size must match current grid view size.");

            for (var y = 0; y < self.Height; ++y)
                for (var x = 0; x < self.Width; ++x)
                    self[x, y] = overlay[x, y];
        }

        /// <summary>
        /// Sets the values for each location of the current grid view to be equal to the value returned from the given
        /// function when given that position.
        /// </summary>
        /// <typeparam name="T" />
        /// <param name="self" />
        /// <param name="valueFunc">
        /// Function returning data for each location in the grid view.
        /// </param>
        public static void ApplyOverlay<T>(this ISettableGridView<T> self, Func<Point, T> valueFunc)
            => self.ApplyOverlay(new LambdaGridView<T>(self.Width, self.Height, valueFunc));

        /// <summary>
        /// Gets a rectangle representing the bounds of the current grid view.
        /// </summary>
        /// <typeparam name="T" />
        /// <param name="gridView" />
        /// <returns>A rectangle representing the grid view's bounds.</returns>
        public static Rectangle Bounds<T>(this IGridView<T> gridView)
            => new Rectangle(0, 0, gridView.Width, gridView.Height);

        /// <summary>
        /// Returns whether or not the given position is contained within the current grid view or not.
        /// </summary>
        /// <typeparam name="T" />
        /// <param name="gridView" />
        /// <param name="x">X-value of the position to check.</param>
        /// <param name="y">Y-value of the position to check.</param>
        /// <returns>True if the given position is contained within this grid view, false otherwise.</returns>
        public static bool Contains<T>(this IGridView<T> gridView, int x, int y)
            => x >= 0 && y >= 0 && x < gridView.Width && y < gridView.Height;

        /// <summary>
        /// Returns whether or not the given position is contained within the current grid view or not.
        /// </summary>
        /// <typeparam name="T" />
        /// <param name="gridView" />
        /// <param name="position">The position to check.</param>
        /// <returns>True if the given position is contained within this grid view, false otherwise.</returns>
        public static bool Contains<T>(this IGridView<T> gridView, Point position)
            => position.X >= 0 && position.Y >= 0 && position.X < gridView.Width && position.Y < gridView.Height;

        /// <summary>
        /// Allows stringifying the contents of a grid view. Takes characters to surround the grid view printout, and
        /// each row, the method used to get the string representation of each element (defaulting to the ToString
        /// function of type T), and separation characters for each element and row.
        /// </summary>
        /// <typeparam name="T" />
        /// <param name="gridView" />
        /// <param name="begin">Character(s) that should precede the IGridView printout.</param>
        /// <param name="beginRow">Character(s) that should precede each row.</param>
        /// <param name="elementStringifier">
        /// Function to use to get the string representation of each value. null uses the ToString
        /// function of type T.
        /// </param>
        /// <param name="rowSeparator">Character(s) to separate each row from the next.</param>
        /// <param name="elementSeparator">Character(s) to separate each element from the next.</param>
        /// <param name="endRow">Character(s) that should follow each row.</param>
        /// <param name="end">Character(s) that should follow the IGridView printout.</param>
        /// <returns>A string representation of the values in the grid view.</returns>
        public static string ExtendToString<T>(this IGridView<T> gridView, string begin = "", string beginRow = "",
                                               Func<T, string>? elementStringifier = null,
                                               string rowSeparator = "\n", string elementSeparator = " ",
                                               string endRow = "", string end = "")
        {
            elementStringifier ??= obj => obj?.ToString() ?? "null";

            var result = new StringBuilder(begin);
            for (var y = 0; y < gridView.Height; y++)
            {
                result.Append(beginRow);
                for (var x = 0; x < gridView.Width; x++)
                {
                    result.Append(elementStringifier(gridView[x, y]));
                    if (x != gridView.Width - 1) result.Append(elementSeparator);
                }

                result.Append(endRow);
                if (y != gridView.Height - 1) result.Append(rowSeparator);
            }

            result.Append(end);

            return result.ToString();
        }

        /// <summary>
        /// Allows stringifying the contents of a grid view. Takes characters to surround the grid view representation,
        /// and each row, the method used to get the string representation of each element (defaulting to the ToString
        /// function of type T), and separation characters for each element and row. Takes the size of the field to
        /// give each element, characters to surround the GridView printout, and each row, the method used to get the
        /// string representation of each element (defaulting to the ToString function of type T), and separation
        /// characters for each element and row.
        /// </summary>
        /// <typeparam name="T" />
        /// <param name="gridView" />
        /// <param name="fieldSize">
        /// The amount of space each element should take up in characters. A positive number aligns
        /// the text to the right of the space, while a negative number aligns the text to the left.
        /// </param>
        /// <param name="begin">Character(s) that should precede the IGridView printout.</param>
        /// <param name="beginRow">Character(s) that should precede each row.</param>
        /// <param name="elementStringifier">
        /// Function to use to get the string representation of each value. Null uses the ToString
        /// function of type T.
        /// </param>
        /// <param name="rowSeparator">Character(s) to separate each row from the next.</param>
        /// <param name="elementSeparator">Character(s) to separate each element from the next.</param>
        /// <param name="endRow">Character(s) that should follow each row.</param>
        /// <param name="end">Character(s) that should follow the IGridView printout.</param>
        /// <returns>A string representation of the grid view.</returns>
        public static string ExtendToString<T>(this IGridView<T> gridView, int fieldSize, string begin = "",
                                               string beginRow = "", Func<T, string>? elementStringifier = null,
                                               string rowSeparator = "\n", string elementSeparator = " ",
                                               string endRow = "", string end = "")
        {
            elementStringifier ??= obj => obj?.ToString() ?? "null";

            var result = new StringBuilder(begin);
            for (var y = 0; y < gridView.Height; y++)
            {
                result.Append(beginRow);
                for (var x = 0; x < gridView.Width; x++)
                {
                    result.Append(string.Format($"{{0, {fieldSize}}} ", elementStringifier(gridView[x, y])));
                    if (x != gridView.Width - 1) result.Append(elementSeparator);
                }

                result.Append(endRow);
                if (y != gridView.Height - 1) result.Append(rowSeparator);
            }

            result.Append(end);

            return result.ToString();
        }

        /// <summary>
        /// Sets each location in the grid view to the value specified.
        /// </summary>
        /// <typeparam name="T" />
        /// <param name="self" />
        /// <param name="value">Value to fill the grid view with.</param>
        public static void Fill<T>(this ISettableGridView<T> self, T value)
            => self.ApplyOverlay(_ => value);

        /// <summary>
        /// Iterates through each position in the grid view.
        /// </summary>
        /// <typeparam name="T" />
        /// <param name="gridView" />
        /// <returns>All positions in the IGridView.</returns>
        public static IEnumerable<Point> Positions<T>(this IGridView<T> gridView)
        {
            for (var y = 0; y < gridView.Height; y++)
                for (var x = 0; x < gridView.Width; x++)
                    yield return new Point(x, y);
        }
    }
}

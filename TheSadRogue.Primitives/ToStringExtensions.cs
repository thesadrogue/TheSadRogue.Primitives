using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using SadRogue.Primitives.GridViews;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Static class containing extension extension methods for printing various built-in C# classes
    /// (as well as some grid views) as strings neatly.
    ///
    /// Useful for debugging, and within the primitives library for ToString implementations.
    /// </summary>
    [PublicAPI]
    public static class ToStringExtensions
    {
        /// <summary>
        /// Extension method for <see cref="IEnumerable{T}" /> that allows retrieving a string
        /// representing the contents.
        /// </summary>
        /// <remarks>
        /// Built-in C# data structures like <see cref="List{T}" /> implement <see cref="IEnumerable{T}" />,
        /// and as such this method can be used to stringify the contents of C# built-in data structures.
        /// When no customization parameters are specified, it defaults to a representation looking something
        /// like [elem1, elem2, elem3].
        /// </remarks>
        /// <typeparam name="T" />
        /// <param name="enumerable" />
        /// <param name="begin">Character(s) that should precede the string representation of the IEnumerable's elements.</param>
        /// <param name="elementStringifier">
        /// Function to use to get the string representation of each element. Specifying null uses the ToString
        /// function of type T.
        /// </param>
        /// <param name="separator">Characters to separate the IEnumerable's elements by.</param>
        /// <param name="end">Character(s) that should follow the string representation of the IEnumerable's elements.</param>
        /// <returns>A string representation of the IEnumerable.</returns>
        public static string ExtendToString<T>(this IEnumerable<T> enumerable, string begin = "[",
                                               Func<T, string>? elementStringifier = null, string separator = ", ",
                                               string end = "]")
        {
            elementStringifier ??= obj => obj?.ToString() ?? "null";

            var result = new StringBuilder(begin);
            bool first = true;
            foreach (var item in enumerable)
            {
                if (first)
                    first = false;
                else
                    result.Append(separator);

                result.Append(elementStringifier(item));
            }

            result.Append(end);

            return result.ToString();
        }

        /// <summary>
        /// Extension method for <see cref="ISet{T}" /> that allows retrieving a string representing the
        /// contents.
        /// </summary>
        /// <remarks>
        /// Built-in C# data structures like <see cref="HashSet{T}" /> implement <see cref="ISet{T}" />,
        /// and as such this method can be used to stringify the contents of C# built-in set structures.
        /// When no customization parameters are specified, it defaults to a representation looking something
        /// like set(elem1, elem2, elem3).
        /// </remarks>
        /// <typeparam name="T" />
        /// <param name="set" />
        /// <param name="begin">Character(s) that should precede the string representation of the set's elements.</param>
        /// <param name="elementStringifier">
        /// Function to use to get the string representation of each element. Specifying null uses the ToString
        /// function of type T.
        /// </param>
        /// <param name="separator">Characters to separate the set's items by.</param>
        /// <param name="end">Character(s) that should follow the string representation of the set's elements.</param>
        /// <returns>A string representation of the ISet.</returns>
        public static string ExtendToString<T>(this ISet<T> set, string begin = "set(",
                                               Func<T, string>? elementStringifier = null, string separator = ", ",
                                               string end = ")")
            => ExtendToString((IEnumerable<T>)set, begin, elementStringifier, separator, end);

        /// <summary>
        /// Extension method for dictionaries that allows retrieving a string representing the dictionary's contents.
        /// </summary>
        /// <remarks>
        /// Built-in C# data structures like <see cref="Dictionary{T, V}" /> implement <see cref="IDictionary{T, V}" />,
        /// and as such this method can be used to stringify the contents of C# built-in dictionary structures.
        /// When no customization parameters are specified, it defaults to a representation looking something
        /// like {key1 : value, key2 : value}.
        /// </remarks>
        /// <typeparam name="TKey" />
        /// <typeparam name="TValue" />
        /// <param name="dictionary" />
        /// <param name="begin">Character(s) that should precede the string representation of the dictionary's elements.</param>
        /// <param name="keyStringifier">
        /// Function to use to get the string representation of each key. Specifying null uses the ToString
        /// function of type K.
        /// </param>
        /// <param name="valueStringifier">
        /// Function to use to get the string representation of each value. Specifying null uses the ToString
        /// function of type V.
        /// </param>
        /// <param name="kvSeparator">Characters used to separate each value from its key.</param>
        /// <param name="pairSeparator">Characters used to separate each key-value pair from the next.</param>
        /// <param name="end">Character(s) that should follow the string representation of the dictionary's elements.</param>
        /// <returns>A string representation of the IDictionary.</returns>
        public static string ExtendToString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, string begin = "{",
                                                          Func<TKey, string>? keyStringifier = null,
                                                          Func<TValue, string>? valueStringifier = null,
                                                          string kvSeparator = " : ", string pairSeparator = ", ",
                                                          string end = "}")
            where TKey : notnull
        {
            keyStringifier ??= obj => obj.ToString() ?? "null";

            valueStringifier ??= obj => obj?.ToString() ?? "null";

            var result = new StringBuilder(begin);
            bool first = true;
            foreach (var (key, value) in dictionary)
            {
                if (first)
                    first = false;
                else
                    result.Append(pairSeparator);

                result.Append(keyStringifier(key) + kvSeparator + valueStringifier(value));
            }

            result.Append(end);

            return result.ToString();
        }

        /// <summary>
        /// Extension method for 2D arrays that allows retrieving a string representing the contents.
        /// </summary>
        /// <typeparam name="T" />
        /// <param name="array" />
        /// <param name="begin">Character(s) that should precede the string representation of the 2D array.</param>
        /// <param name="beginRow">Character(s) that should precede the string representation of each row.</param>
        /// <param name="elementStringifier">
        /// Function to use to get the string representation of each value. Specifying null uses the ToString
        /// function of type T.
        /// </param>
        /// <param name="rowSeparator">Character(s) used to separate each row from the next.</param>
        /// <param name="elementSeparator">Character(s) used to separate each element from the next.</param>
        /// <param name="endRow">Character(s) that should follow the string representation of each row.</param>
        /// <param name="end">Character(s) that should follow the string representation of the 2D array.</param>
        /// <returns>A string representation of the 2D array.</returns>
        public static string ExtendToString<T>(this T[,] array, string begin = "[\n", string beginRow = "\t[",
                                               Func<T, string>? elementStringifier = null,
                                               string rowSeparator = ",\n", string elementSeparator = ", ",
                                               string endRow = "]", string end = "\n]")
        {
            elementStringifier ??= obj => obj?.ToString() ?? "null";

            var result = new StringBuilder(begin);
            for (int x = 0; x < array.GetLength(0); x++)
            {
                result.Append(beginRow);
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    result.Append(elementStringifier(array[x, y]));
                    if (y != array.GetLength(1) - 1) result.Append(elementSeparator);
                }

                result.Append(endRow);
                if (x != array.GetLength(0) - 1) result.Append(rowSeparator);
            }

            result.Append(end);
            return result.ToString();
        }

        /// <summary>
        /// Extension method for 2D arrays that allows retrieving a string representing the contents,
        /// formatted as if the 2D array represents a coordinate plane/grid.
        /// </summary>
        /// <remarks>
        /// This differs from
        /// <see cref="ExtendToString{T}(T[,], string, string, Func{T, string}, string, string, string, string)" />
        /// in that this method prints the array
        /// such that array[x+1, y] is printed to the RIGHT of array[x, y], rather than BELOW it.
        /// Effectively it assumes the indexes being used are grid/coordinate plane coordinates.
        /// </remarks>
        /// <typeparam name="T" />
        /// <param name="array" />
        /// <param name="begin">Character(s) that should precede the string representation of the 2D array.</param>
        /// <param name="beginRow">Character(s) that should precede the string representation of each row.</param>
        /// <param name="elementStringifier">
        /// Function to use to get the string representation of each value. Specifying null uses the ToString
        /// function of type T.
        /// </param>
        /// <param name="rowSeparator">Character(s) used to separate each row from the next.</param>
        /// <param name="elementSeparator">Character(s) used to separate each element from the next.</param>
        /// <param name="endRow">Character(s) that should follow the string representation of each row.</param>
        /// <param name="end">Character(s) that should follow the string representation of the 2D array.</param>
        /// <returns>
        /// A string representation of the 2D array, formatted as if the array represents a 2D coordinate plane/grid map.
        /// </returns>
        public static string ExtendToStringGrid<T>(this T[,] array, string begin = "", string beginRow = "",
                                                   Func<T, string>? elementStringifier = null,
                                                   string rowSeparator = "\n", string elementSeparator = " ",
                                                   string endRow = "", string end = "")
            => new ArrayView2D<T>(array).ExtendToString(begin, beginRow, elementStringifier, rowSeparator,
                elementSeparator, endRow, end);

        /// <summary>
        /// Extension method for 2D arrays that allows retrieving a string representing the contents,
        /// formatted as if the 2D array represents a coordinate plane/grid.
        /// </summary>
        /// <remarks>
        /// This differs from
        /// <see cref="ExtendToString{T}(T[,], string, string, Func{T, string}, string, string, string, string)" />
        /// in that this method prints the array such that array[x+1, y] is printed to the RIGHT of array[x, y], rather than BELOW
        /// it.
        /// Effectively it assumes the indexes being used are grid/coordinate plane coordinates.
        /// </remarks>
        /// <typeparam name="T" />
        /// <param name="array" />
        /// <param name="fieldSize">
        /// The amount of space each element should take up in characters. A positive number aligns
        /// the text to the right of the space, while a negative number aligns the text to the left.
        /// </param>
        /// <param name="begin">Character(s) that should precede the string representation of the 2D array.</param>
        /// <param name="beginRow">Character(s) that should precede the string representation of each row.</param>
        /// <param name="elementStringifier">
        /// Function to use to get the string representation of each value. Specifying null uses the ToString
        /// function of type T.
        /// </param>
        /// <param name="rowSeparator">Character(s) used to separate each row from the next.</param>
        /// <param name="elementSeparator">Character(s) used to separate each element from the next.</param>
        /// <param name="endRow">Character(s) that should follow the string representation of each row.</param>
        /// <param name="end">Character(s) that should follow the string representation of the 2D array.</param>
        /// <returns>
        /// A string representation of the 2D array, formatted as if the array represents a 2D coordinate plane/grid map.
        /// </returns>
        public static string ExtendToStringGrid<T>(this T[,] array, int fieldSize, string begin = "",
                                                   string beginRow = "", Func<T, string>? elementStringifier = null,
                                                   string rowSeparator = "\n", string elementSeparator = " ",
                                                   string endRow = "", string end = "")
            => new ArrayView2D<T>(array).ExtendToString(fieldSize, begin, beginRow, elementStringifier, rowSeparator,
                elementSeparator, endRow, end);
    }
}

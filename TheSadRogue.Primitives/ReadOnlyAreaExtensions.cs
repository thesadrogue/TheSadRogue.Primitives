namespace SadRogue.Primitives
{
    /// <summary>
    /// Extension methods for IReadOnlyArea.
    /// </summary>
    public static class ReadOnlyAreaExtensions
    {
        /// <summary>
        /// Returns an enumerator which can be used to iterate over the positions in this area with a foreach loop
        /// in the most efficient manner possible.
        /// </summary>
        /// <remarks>
        /// The enumerator returned will use the area's indexer to iterate over the positions (like you might a list),
        /// if the area's <see cref="IReadOnlyArea.UseIndexEnumeration"/> is true.  Otherwise, it uses the typical IEnumerator
        /// implementation for that area.
        ///
        /// This may be significantly faster than the typical IEnumerable/IEnumerator usage for implementations which have
        /// <see cref="IReadOnlyArea.UseIndexEnumeration"/> set to true; however it won't have much benefit otherwise.
        /// </remarks>
        /// <param name="self"/>
        /// <returns>A custom enumerator that iterates over the positions in the area in the most efficient manner possible.</returns>
        public static ReadOnlyAreaPositionsEnumerable FastEnumerator(this IReadOnlyArea self)
            => new ReadOnlyAreaPositionsEnumerable(self);
    }
}

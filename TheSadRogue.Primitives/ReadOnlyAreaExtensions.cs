namespace SadRogue.Primitives
{
    /// <summary>
    /// Extension methods for IReadOnlyArea.
    /// </summary>
    public static class ReadOnlyAreaExtensions
    {
        /// <summary>
        /// Returns an enumerator which can be used to iterate over the positions in this area with a foreach loop
        /// much more efficiently than the typical IEnumerable implementation, for many IReadOnlyArea implementations.
        /// </summary>
        /// <remarks>
        /// This enumerator simply uses the indexer to iterate over the area, like you might a list.  For implementations
        /// such as <see cref="Area"/>, using this is typically much faster than using the regular IEnumerable (but still
        /// slower than a manual for loop in some cases).  For implementations of <see cref="IReadOnlyArea"/> with a more complex
        /// indexer function, there may not be as much benefit here.
        /// </remarks>
        /// <param name="self"/>
        /// <returns>A custom enumerator that iterates over the positions in the area using the indexer.</returns>
        public static ReadOnlyAreaPositionsEnumerable FastEnumerator(this IReadOnlyArea self)
            => new ReadOnlyAreaPositionsEnumerable(self);
    }
}

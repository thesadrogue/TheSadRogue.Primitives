namespace SadRogue.Primitives
{
    public static class ReadOnlyAreaExtensions
    {
        public static ReadOnlyAreaPostionsEnumerable FastEnumerator(this IReadOnlyArea self)
            => new ReadOnlyAreaPostionsEnumerable(self);
    }
}

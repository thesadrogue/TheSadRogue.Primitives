using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SadRogue.Primitives
{
    public static class ReadOnlyAreaExtensions
    {
        public static IEnumerable<Point> ToEnumerable(this IReadOnlyArea self)
            => new ReadOnlyAreaPostionsEnumerable(self).ToEnumerable();

        //public static ReadOnlyAreaPostionsEnumerable GetEnumerator(this IReadOnlyArea self) => new ReadOnlyAreaPostionsEnumerable(self);
    }
}

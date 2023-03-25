using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives.Pooling;
using Xunit;

namespace SadRogue.Primitives.UnitTests.Pooling
{
    public class NoPoolingListPoolTests
    {
        [Fact]
        public void DoesNotPool()
        {
            var listPool = new NoPoolingListPool<int>();

            var lists = new[]
            {
                new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>()
            };
            foreach (var list in lists)
                listPool.Return(list);

            // Nothing was saved since this "pool" implementation does not pool.
            var returnedLists = Enumerable.Range(0, lists.Length).Select(i => listPool.Rent()).ToArray();
            Assert.Equal(lists.Length, returnedLists.Length);

            foreach (var list in returnedLists)
                Assert.DoesNotContain(lists, l => ReferenceEquals(l, list));
        }

        [Fact]
        public void ClearIsNoOp()
        {
            var listPool = new NoPoolingListPool<int>();

            var lists = new[]
            {
                new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>()
            };
            foreach (var list in lists)
                listPool.Return(list);

            listPool.Clear();

            // Nothing was saved, just like above tests.
            var returnedLists = Enumerable.Range(0, lists.Length).Select(i => listPool.Rent()).ToArray();
            Assert.Equal(lists.Length, returnedLists.Length);

            foreach (var list in returnedLists)
                Assert.DoesNotContain(lists, l => ReferenceEquals(l, list));
        }
    }
}

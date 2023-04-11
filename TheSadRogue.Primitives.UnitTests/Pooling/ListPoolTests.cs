using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives.Pooling;
using Xunit;

namespace SadRogue.Primitives.UnitTests.Pooling
{
    public class ListPoolTests
    {
        [Fact]
        public void BasicConstruction()
        {
            var listPool = new ListPool<int>(50, 25);
            Assert.Equal(50, listPool.MaxLists);
            Assert.Equal(25, listPool.MaxCapacity);
        }

        [Fact]
        public void BasicRentReturn()
        {
            var listPool = new ListPool<int>(50, 25);

            List<int> list = listPool.Rent();
            Assert.NotNull(list);

            listPool.Return(list);

            var list2 = listPool.Rent();
            Assert.Same(list, list2);
        }

        [Fact]
        public void CapacityChangedCorrectly()
        {
            var listPool = new ListPool<int>(50, 32);

            // Not resized since it is at or under max capacity limit
            var list = new List<int>(32);
            listPool.Return(list);
            var returned = listPool.Rent();
            Assert.Equal(list.Capacity, returned.Capacity);

            // Not resized since it is at or under max capacity limit
            list = new List<int>(31);
            listPool.Return(list);
            returned = listPool.Rent();
            Assert.Equal(list.Capacity, returned.Capacity);

            // Resized when returned, since it exceeds the max capacity limit
            list = new List<int>(33);
            listPool.Return(list);
            returned = listPool.Rent();
            Assert.Equal(32, returned.Capacity);
        }

        [Fact]
        public void MaxListsEnforced()
        {
            var listPool = new ListPool<int>(3, 32);

            var lists = new[]
            {
                new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>()
            };
            foreach (var list in lists)
                listPool.Return(list);

            var returnedLists = Enumerable.Range(0, lists.Length).Select(i => listPool.Rent()).ToArray();
            Assert.Equal(lists.Length, returnedLists.Length);

            // ListPool saved the first 3 lists we passed in (in some order)
            var firstThree = lists.Take(3).ToArray();
            for (int i = 0; i < 3; i++)
                Assert.Contains(firstThree, l => ReferenceEquals(l, returnedLists[i]));

            // After that, it didn't save any of them, because they exceeded the max lists limit.
            for (int i = 3; i < returnedLists.Length; i++)
                Assert.DoesNotContain(lists, l => ReferenceEquals(l, returnedLists[i]));
        }

        [Fact]
        public void Clear()
        {
            var listPool = new ListPool<int>(3, 32);

            var lists = new[]
            {
                new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>()
            };
            foreach (var list in lists)
                listPool.Return(list);

            listPool.Clear();

            // Nothing was saved since we cleared the lists.
            var returnedLists = Enumerable.Range(0, lists.Length).Select(i => listPool.Rent()).ToArray();
            Assert.Equal(lists.Length, returnedLists.Length);

            foreach (var list in returnedLists)
                Assert.DoesNotContain(lists, l => ReferenceEquals(l, list));
        }
    }
}

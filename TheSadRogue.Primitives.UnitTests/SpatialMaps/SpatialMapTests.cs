using System;
using System.Linq;
using SadRogue.Primitives.SpatialMaps;
using SadRogue.Primitives.UnitTests.Mocks;
using Xunit;

namespace SadRogue.Primitives.UnitTests.SpatialMaps
{
    public class SpatialMapTests
    {
        //private readonly ITestOutputHelper _output;
        private readonly MockSpatialMapItem _initialItem;
        private static readonly Point s_initialItemPos = (1, 2);
        private static readonly Point s_newItemPos = (2, 3);
        private readonly SpatialMap<MockSpatialMapItem> _spatialMap;

        public SpatialMapTests()
        {
            //_output = output;

            // Set up the spatial map and some initial test item
            _spatialMap = new SpatialMap<MockSpatialMapItem>();
            _initialItem = new MockSpatialMapItem(0);
            _spatialMap.Add(_initialItem, s_initialItemPos);
        }

        #region Add Item
        [Fact]
        public void AddItemValid()
        {
            // Just the starting items to begin with, and none at new location
            Assert.Empty(_spatialMap.GetItemsAt(s_newItemPos));
            Assert.Equal(1, _spatialMap.Count);

            // Add item at new location
            var item = new MockSpatialMapItem(0);
            _spatialMap.Add(item, s_newItemPos);
            Assert.Single(_spatialMap.GetItemsAt(s_newItemPos));
            Assert.Equal(2, _spatialMap.Count);
        }

        [Fact]
        public void AddItemXYValid()
        {
            // Just the starting items to begin with, and none at new location
            Assert.Empty(_spatialMap.GetItemsAt(s_newItemPos));
            Assert.Equal(1, _spatialMap.Count);

            // Add item at new location
            var item = new MockSpatialMapItem(0);
            _spatialMap.Add(item, s_newItemPos.X, s_newItemPos.Y);
            Assert.Single(_spatialMap.GetItemsAt(s_newItemPos));
            Assert.Equal(2, _spatialMap.Count);
        }

        [Fact]
        public void AddItemInvalid()
        {
            var (item, position) = (new MockSpatialMapItem(0), s_initialItemPos);
            Assert.Single(_spatialMap.GetItemsAt(position));
            int prevCount = _spatialMap.Count;

            // Should throw exception and not add item
            Assert.Throws<ArgumentException>(() => _spatialMap.Add(item, position));
            Assert.Single( _spatialMap.GetItemsAt(position));
            Assert.Equal(prevCount, _spatialMap.Count);

            Assert.Throws<ArgumentException>(() => _spatialMap.Add(item, position.X, position.Y));
            Assert.Single( _spatialMap.GetItemsAt(position));
            Assert.Equal(prevCount, _spatialMap.Count);
        }
        #endregion

        #region Remove Item
        [Fact]
        public void RemoveItemValid()
        {
            _spatialMap.Remove(_initialItem);
            Assert.Equal(0, _spatialMap.Count);
            Assert.Empty(_spatialMap);
        }

        [Fact]
        public void RemoveItemInvalid()
        {
            int prevCount = _spatialMap.Count;

            var nonexistentItem = new MockSpatialMapItem(0);

            Assert.Throws<ArgumentException>(() => _spatialMap.Remove(nonexistentItem));
            Assert.Single(_spatialMap.GetItemsAt(s_initialItemPos));
            Assert.Equal(prevCount, _spatialMap.Count);
        }

        [Fact]
        public void RemoveByPositionValid()
        {
            var itemsRemoved = _spatialMap.Remove(s_initialItemPos);
            Assert.Single(itemsRemoved);
            Assert.Equal(0, _spatialMap.Count);
            Assert.Empty(_spatialMap.GetItemsAt(s_initialItemPos));
        }

        [Fact]
        public void RemoveByXYValid()
        {
            var itemsRemoved = _spatialMap.Remove(s_initialItemPos.X, s_initialItemPos.Y);
            Assert.Single(itemsRemoved);
            Assert.Equal(0, _spatialMap.Count);
            Assert.Empty(_spatialMap.GetItemsAt(s_initialItemPos));
        }

        [Fact]
        public void RemoveByPositionInvalid()
        {
            var itemsRemoved = _spatialMap.Remove(s_newItemPos);
            Assert.Empty(itemsRemoved);
            Assert.Equal(1, _spatialMap.Count);
            Assert.Single(_spatialMap.GetItemsAt(s_initialItemPos));
        }
        #endregion

        #region Move Items
        [Fact]
        public void MoveItemValid()
        {
            Assert.Empty(_spatialMap.GetItemsAt(s_newItemPos));


            _spatialMap.Move(_initialItem, s_newItemPos);
            Assert.Equal(1, _spatialMap.Count);
            Assert.Empty(_spatialMap.GetItemsAt(s_initialItemPos));
            Assert.Single(_spatialMap.GetItemsAt(s_newItemPos));
        }

        [Fact]
        public void MoveItemXYValid()
        {
            Assert.Empty(_spatialMap.GetItemsAt(s_newItemPos));


            _spatialMap.Move(_initialItem, s_newItemPos.X, s_newItemPos.Y);
            Assert.Equal(1, _spatialMap.Count);
            Assert.Empty(_spatialMap.GetItemsAt(s_initialItemPos));
            Assert.Single(_spatialMap.GetItemsAt(s_newItemPos));
        }

        [Fact]
        public void MoveItemInvalid()
        {
            // Create item and add to a different position
            var lastItem = new MockSpatialMapItem(0);
            _spatialMap.Add(lastItem, s_newItemPos);
            Assert.Equal(2, _spatialMap.Count);

            // Throws because there is already an item at the initial position
            Assert.Throws<ArgumentException>(() => _spatialMap.Move(lastItem, s_initialItemPos));
        }

        [Fact]
        public void MoveItemDoesNotExist()
        {
            int prevCount = _spatialMap.Count;

            var nonexistentItem = new MockSpatialMapItem(0);

            Assert.Throws<ArgumentException>(() => _spatialMap.Move(nonexistentItem, s_newItemPos));
            Assert.Single(_spatialMap.GetItemsAt(s_initialItemPos));
            Assert.Equal(prevCount, _spatialMap.Count);
        }

        [Fact]
        public void MoveValidItemsAllValid()
        {
            var movedItems = _spatialMap.MoveValid(s_initialItemPos, s_newItemPos);
            Assert.Single(movedItems);
            Assert.Empty(_spatialMap.GetItemsAt(s_initialItemPos));
            Assert.Single(_spatialMap.GetItemsAt(s_newItemPos));
        }

        [Fact]
        public void MoveValidItemsXYAllValid()
        {
            var movedItems = _spatialMap.MoveValid(s_initialItemPos.X, s_initialItemPos.Y, s_newItemPos.X, s_newItemPos.Y);
            Assert.Single(movedItems);
            Assert.Empty(_spatialMap.GetItemsAt(s_initialItemPos));
            Assert.Single(_spatialMap.GetItemsAt(s_newItemPos));
        }

        [Fact]
        public void MoveValidItemsNoneValid()
        {
            // Create item and add it to a different position
            var lastItem = new MockSpatialMapItem(0);
            _spatialMap.Add(lastItem, s_newItemPos);
            Assert.Equal(2, _spatialMap.Count);

            // The object here cannot move because it's blocked by an existing item
            Assert.Empty(_spatialMap.MoveValid(s_newItemPos, s_initialItemPos));
            Assert.Single(_spatialMap.GetItemsAt(s_initialItemPos));
            Assert.Single(_spatialMap.GetItemsAt(s_newItemPos));
        }

        [Fact]
        public void MoveValidItemsXYNoneValid()
        {
            // Create item and add it to a different position
            var lastItem = new MockSpatialMapItem(0);
            _spatialMap.Add(lastItem, s_newItemPos);
            Assert.Equal(2, _spatialMap.Count);

            // The object here cannot move because it's blocked by an existing item
            Assert.Empty(_spatialMap.MoveValid(s_newItemPos.X, s_newItemPos.Y, s_initialItemPos.X, s_initialItemPos.Y));
            Assert.Single(_spatialMap.GetItemsAt(s_initialItemPos));
            Assert.Single(_spatialMap.GetItemsAt(s_newItemPos));
        }

        [Fact]
        public void MoveAllItemsValid()
        {
            // No items at new location to start
            Assert.Empty(_spatialMap.GetItemsAt(s_newItemPos));

            // No items blocked so should succeed
            _spatialMap.MoveAll(s_initialItemPos, s_newItemPos);
            Assert.Single(_spatialMap.GetItemsAt(s_newItemPos));
            Assert.Empty(_spatialMap.GetItemsAt(s_initialItemPos));
            Assert.Equal(1, _spatialMap.Count);
        }

        [Fact]
        public void MoveAllItemsXYValid()
        {
            // No items at new location to start
            Assert.Empty(_spatialMap.GetItemsAt(s_newItemPos));

            // No items blocked so should succeed
            _spatialMap.MoveAll(s_initialItemPos.X, s_initialItemPos.Y, s_newItemPos.X, s_newItemPos.Y);
            Assert.Single(_spatialMap.GetItemsAt(s_newItemPos));
            Assert.Empty(_spatialMap.GetItemsAt(s_initialItemPos));
            Assert.Equal(1, _spatialMap.Count);
        }

        [Fact]
        public void MoveAllItemsInvalid()
        {
            // Create item and add it to a different position
            var lastItem = new MockSpatialMapItem(0);
            _spatialMap.Add(lastItem, s_newItemPos);
            Assert.Equal(2, _spatialMap.Count);

            // The new position is blocked (by lastItem), so this should fail.
            Assert.Throws<ArgumentException>(() => _spatialMap.MoveAll(s_initialItemPos, s_newItemPos));
        }

        [Fact]
        public void MoveAllItemsXYInvalid()
        {
            // Create item and add it to a different position
            var lastItem = new MockSpatialMapItem(0);
            _spatialMap.Add(lastItem, s_newItemPos);
            Assert.Equal(2, _spatialMap.Count);

            // The new position is blocked (by lastItem), so this should fail.
            Assert.Throws<ArgumentException>(() => _spatialMap.MoveAll(s_initialItemPos.X, s_initialItemPos.Y, s_newItemPos.X, s_newItemPos.Y));
        }
        #endregion

        #region Contains
        [Fact]
        public void ContainsItem()
        {
            Assert.True(_spatialMap.Contains(_initialItem));

            // Unadded item is not contained
            var unaddedItem = new MockSpatialMapItem(0);
            Assert.False(_spatialMap.Contains(unaddedItem));

            // Moved items are still in the spatial map
            _spatialMap.Move(_initialItem, s_newItemPos);
            Assert.True(_spatialMap.Contains(_initialItem));
        }

        [Fact]
        public void ContainsPosition()
        {
            Assert.True(_spatialMap.Contains(s_initialItemPos));
            Assert.False(_spatialMap.Contains(s_newItemPos));

            // Moved items update results from contained
            _spatialMap.Move(_initialItem, s_newItemPos);
            Assert.False(_spatialMap.Contains(s_initialItemPos));
            Assert.True(_spatialMap.Contains(s_newItemPos));
        }

        [Fact]
        public void ContainsXY()
        {
            Assert.True(_spatialMap.Contains(s_initialItemPos.X, s_initialItemPos.Y));
            Assert.False(_spatialMap.Contains(s_newItemPos.X, s_newItemPos.Y));

            // Moved items update results from contained
            _spatialMap.Move(_initialItem, s_newItemPos);
            Assert.False(_spatialMap.Contains(s_initialItemPos.X, s_initialItemPos.Y));
            Assert.True(_spatialMap.Contains(s_newItemPos.X, s_newItemPos.Y));
        }
        #endregion

        #region GetItemsAt
        // Test that GetItemsAt returns the correct items at s_initialItemPos
        [Fact]
        public void GetItemsAt()
        {
            var itemsAt = _spatialMap.GetItemsAt(s_initialItemPos).ToArray();
            Assert.Single(itemsAt);
            Assert.Equal(_initialItem, itemsAt[0]);
        }

        // Test that GetItemsAt returns no items at empty locations
        [Fact]
        public void GetItemsAtEmpty()
        {
            Assert.Empty(_spatialMap.GetItemsAt(s_newItemPos));
        }
        #endregion

        [Fact]
        public void SpatialMapCreate()
        {
            var mySpatialMap = new SpatialMap<MyIDImpl>();

            Assert.Equal(0, mySpatialMap.Count);
            Assert.Empty(mySpatialMap.Items);
            Assert.Empty(mySpatialMap);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives.SpatialMaps;
using SadRogue.Primitives.UnitTests.Mocks;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests.SpatialMaps
{
    public class LayeredSpatialMapTests
    {
        //private readonly ITestOutputHelper _output;

        private readonly LayeredSpatialMap<MockSpatialMapItem> _spatialMap;
        private static readonly Point s_initialItemsPos = (1, 2);
        private static readonly Point s_newItemsPos = (2, 3);
        private const int StartingLayer = 1;
        private const int NumLayers = 6;
        private static readonly IEnumerable<int> s_validLayers = Enumerable.Range(StartingLayer, NumLayers);
        private static List<MockSpatialMapItem> s_initialItems = new List<MockSpatialMapItem>();


        public static (MockSpatialMapItem item, Point pos)[] InvalidAddCases =
        {
            // Above the maximum layer
            (new MockSpatialMapItem(StartingLayer + NumLayers), s_newItemsPos),
            // Below the minimum layer
            (new MockSpatialMapItem(StartingLayer - 1), s_newItemsPos),
            // Layer doesn't support multiple items and an item is already here
            (new MockSpatialMapItem(StartingLayer + 1), s_initialItemsPos)
        };

        public LayeredSpatialMapTests()
        {
            //_output = output;

            s_initialItems = new List<MockSpatialMapItem>();

            // Set up items on all but one layer of the spatial map.  No layers
            // support multiple items.
            _spatialMap = new LayeredSpatialMap<MockSpatialMapItem>(NumLayers, startingLayer: StartingLayer);

            foreach (int layer in s_validLayers)
            {
                var item = new MockSpatialMapItem(layer);
                _spatialMap.Add(item, s_initialItemsPos);
                s_initialItems.Add(item);
            }
        }

        [Fact]
        public void Construction()
        {
            // No multiple item layers, 32 layers, starting at 0
            var sm = new LayeredSpatialMap<MockSpatialMapItem>(32);
            Assert.Equal(32, sm.NumberOfLayers);
            Assert.Equal(0, sm.StartingLayer);
            foreach (var layer in sm.Layers)
                Assert.True(layer is AdvancedSpatialMap<MockSpatialMapItem>);

            // Test multiple item layers
            uint multipleItemLayerMask = LayerMasker.Default.Mask(1, 2, 5);
            sm = new LayeredSpatialMap<MockSpatialMapItem>(10, startingLayer: 0,
                layersSupportingMultipleItems: multipleItemLayerMask);
            Assert.Equal(10, sm.NumberOfLayers);
            Assert.Equal(0, sm.StartingLayer);

            int layerNum = 0;
            foreach (var layer in sm.Layers)
            {
                Assert.Equal(LayerMasker.Default.HasLayer(multipleItemLayerMask, layerNum),
                    layer is AdvancedMultiSpatialMap<MockSpatialMapItem>);
                layerNum++;
            }

            // Test arbitrary starting layer (initial values)
            const int startingLayer = 1;
            const int numberOfLayers = 5;
            sm = new LayeredSpatialMap<MockSpatialMapItem>(numberOfLayers, startingLayer: startingLayer);

            Assert.Equal(numberOfLayers, sm.NumberOfLayers);
            Assert.Equal(startingLayer, sm.StartingLayer);
        }

        [Fact]
        public void AddItemValid()
        {
            // Just the starting items to begin with, and none at new location
            Assert.Empty(_spatialMap.GetItemsAt(s_newItemsPos));
            Assert.Equal(s_initialItems.Count, _spatialMap.Count);

            var itemsAdded = new List<MockSpatialMapItem>();
            foreach (int layer in s_validLayers)
            {
                var item = new MockSpatialMapItem(layer);
                _spatialMap.Add(item, s_newItemsPos);
                itemsAdded.Add(item);
                Assert.Equal(itemsAdded.Count, _spatialMap.GetItemsAt(s_newItemsPos).Count());
                Assert.Equal(itemsAdded.Count + s_initialItems.Count, _spatialMap.Count);
            }
        }

        [Theory]
        [MemberDataTuple(nameof(InvalidAddCases))]
        public void AddItemInvalid(MockSpatialMapItem item, Point position)
        {
            int prevAtPos = _spatialMap.GetItemsAt(position).Count();
            int prevCount = _spatialMap.Count;

            // Should throw exception and not add item
            Assert.Throws<ArgumentException>(() => _spatialMap.Add(item, position));
            Assert.Equal(prevAtPos, _spatialMap.GetItemsAt(position).Count());
            Assert.Equal(prevCount, _spatialMap.Count);
        }

        [Fact]
        public void RemoveItemValid()
        {
            int remainingItems = s_initialItems.Count;
            foreach (var item in s_initialItems)
            {
                _spatialMap.Remove(item);
                remainingItems--;

                Assert.Equal(remainingItems, _spatialMap.GetItemsAt((1, 2)).Count());
                Assert.Equal(remainingItems, _spatialMap.Count);
            }
        }

        [Fact]
        public void RemoveItemInvalid()
        {
            int prevAtPos = _spatialMap.GetItemsAt(s_initialItemsPos).Count();
            int prevCount = _spatialMap.Count;

            var nonexistentItem = new MockSpatialMapItem(StartingLayer + 1);

            Assert.Throws<ArgumentException>(() => _spatialMap.Remove(nonexistentItem));
            Assert.Equal(prevAtPos, _spatialMap.GetItemsAt(s_initialItemsPos).Count());
            Assert.Equal(prevCount, _spatialMap.Count);
        }

        [Fact]
        public void RemoveByPositionValid()
        {
            var itemsRemoved = _spatialMap.Remove(s_initialItemsPos).ToList();
            Assert.Equal(s_initialItems.Count, itemsRemoved.Count);
            Assert.Equal(0, _spatialMap.Count);
            Assert.Empty(_spatialMap.GetItemsAt(s_initialItemsPos));
        }

        [Fact]
        public void RemoveByPositionInvalid()
        {
            var itemsRemoved = _spatialMap.Remove(s_newItemsPos).ToList();
            Assert.Empty(itemsRemoved);
            Assert.Equal(s_initialItems.Count, _spatialMap.Count);
            Assert.Equal(s_initialItems.Count, _spatialMap.GetItemsAt(s_initialItemsPos).Count());
        }

        [Fact]
        public void MoveItemValid()
        {
            Assert.Empty(_spatialMap.GetItemsAt(s_newItemsPos));

            foreach (var (item, index) in s_initialItems.Enumerate())
            {
                _spatialMap.Move(item, s_newItemsPos);
                Assert.Equal(s_initialItems.Count, _spatialMap.Count);
                Assert.Equal(s_initialItems.Count - index - 1, _spatialMap.GetItemsAt(s_initialItemsPos).Count());
                Assert.Equal(index + 1, _spatialMap.GetItemsAt(s_newItemsPos).Count());
            }

            // TODO: Add item to layer supporting multiple items
        }

        [Fact]
        public void MoveItemInvalid()
        {
            // Create item and add to a different position
            var lastItem = new MockSpatialMapItem(3);
            _spatialMap.Add(lastItem, s_newItemsPos);
            Assert.Equal(s_initialItems.Count + 1, _spatialMap.Count);

            // Throws because there is already an item at the initial position on layer 3, and layer 3 doesn't support
            // multiple items
            Assert.Throws<ArgumentException>(() => _spatialMap.Move(lastItem, s_initialItemsPos));
        }

        [Fact]
        public void MoveValidItemsAllValid()
        {
            var movedItems = _spatialMap.MoveValid(s_initialItemsPos, s_newItemsPos);
            Assert.Equal(s_initialItems.Count, movedItems.Count);
            Assert.Empty(_spatialMap.GetItemsAt(s_initialItemsPos));
            Assert.Equal(s_initialItems.Count, _spatialMap.GetItemsAt(s_newItemsPos).Count());
        }

        [Fact]
        public void MoveValidItemsSomeValid()
        {
            // Create item and add it to a different position
            var lastItem = new MockSpatialMapItem(3);
            _spatialMap.Add(lastItem, s_newItemsPos);
            Assert.Equal(s_initialItems.Count + 1, _spatialMap.Count);

            // Most of the objects can move; 1 layer's worth is blocked by lastItem
            Assert.Equal(s_initialItems.Count - (1 * (s_initialItems.Count / NumLayers)), _spatialMap.MoveValid(s_initialItemsPos, s_newItemsPos).Count);
            Assert.Single(_spatialMap.GetItemsAt(s_initialItemsPos));
            Assert.Equal(s_initialItems.Count, _spatialMap.GetItemsAt(s_newItemsPos).Count());
        }

        [Fact]
        public void MoveValidItemsNoneValid()
        {
            // Create item and add it to a different position
            var lastItem = new MockSpatialMapItem(3);
            _spatialMap.Add(lastItem, s_newItemsPos);
            Assert.Equal(s_initialItems.Count + 1, _spatialMap.Count);

            // The object here cannot move because it's blocked by an existing item
            Assert.Empty(_spatialMap.MoveValid(s_newItemsPos, s_initialItemsPos));
            Assert.Equal(s_initialItems.Count, _spatialMap.GetItemsAt(s_initialItemsPos).Count());
            Assert.Single(_spatialMap.GetItemsAt(s_newItemsPos));
        }

        [Fact]
        public void MoveAllItemsValid()
        {
            // No items at new location to start
            Assert.Empty(_spatialMap.GetItemsAt(s_newItemsPos));

            // No items blocked so should succeed
            _spatialMap.MoveAll(s_initialItemsPos, s_newItemsPos);
            Assert.Equal(s_initialItems.Count, _spatialMap.GetItemsAt(s_newItemsPos).Count());
            Assert.Empty(_spatialMap.GetItemsAt(s_initialItemsPos));
            Assert.Equal(s_initialItems.Count, _spatialMap.Count);
        }

        [Fact]
        public void MoveAllItemsInvalid()
        {
            // Create item and add it to a different position
            var lastItem = new MockSpatialMapItem(3);
            _spatialMap.Add(lastItem, s_newItemsPos);
            Assert.Equal(s_initialItems.Count + 1, _spatialMap.Count);

            // There is one item that is blocked (by lastItem), so this should fail.  Some items will be moved.
            Assert.Throws<ArgumentException>(() => _spatialMap.MoveAll(s_initialItemsPos, s_newItemsPos));
        }

        [Fact]
        public void MoveAllItemsLayerMaskValid()
        {
            // Create item and add it to a different position
            var lastItem = new MockSpatialMapItem(3);
            _spatialMap.Add(lastItem, s_newItemsPos);
            Assert.Equal(s_initialItems.Count + 1, _spatialMap.Count);

            // The only item blocked is not included in the layer mask, so should succeed
            _spatialMap.MoveAll(s_initialItemsPos, s_newItemsPos, ~_spatialMap.LayerMasker.Mask(3));
            Assert.Equal(s_initialItems.Count, _spatialMap.GetItemsAt(s_newItemsPos).Count());
            Assert.Single(_spatialMap.GetItemsAt(s_initialItemsPos));
            Assert.Equal(s_initialItems.Count + 1, _spatialMap.Count);
        }

        [Fact]
        public void MoveAllItemsLayerMaskInvalid()
        {
            // Create item and add it to a different position
            var lastItem = new MockSpatialMapItem(3);
            _spatialMap.Add(lastItem, s_newItemsPos);
            Assert.Equal(s_initialItems.Count + 1, _spatialMap.Count);

            // There is one item that is blocked (by lastItem), so this should fail.  Some items will be moved.
            Assert.Throws<ArgumentException>(() => _spatialMap.MoveAll(s_initialItemsPos, s_newItemsPos, _spatialMap.LayerMasker.Mask(1, 3, 5)));
        }

        [Fact]
        public void TryMoveAllItemsLayerMaskValid()
        {
            // Create item and add it to a different position
            var lastItem = new MockSpatialMapItem(3);
            _spatialMap.Add(lastItem, s_newItemsPos);
            Assert.Equal(s_initialItems.Count + 1, _spatialMap.Count);

            // The only item blocked is not included in the layer mask, so should succeed
            bool val = _spatialMap.TryMoveAll(s_initialItemsPos, s_newItemsPos, ~_spatialMap.LayerMasker.Mask(3));
            Assert.True(val);
            Assert.Equal(s_initialItems.Count, _spatialMap.GetItemsAt(s_newItemsPos).Count());
            Assert.Single(_spatialMap.GetItemsAt(s_initialItemsPos));
            Assert.Equal(s_initialItems.Count + 1, _spatialMap.Count);
        }

        [Fact]
        public void TryMoveAllItemsLayerMaskSomeValid()
        {
            // Mask used for operations
            uint mask = _spatialMap.LayerMasker.Mask(1, 2, 3);
            const int layersInMask = 3;

            // Create item and add it to a different position
            var lastItem = new MockSpatialMapItem(3);
            _spatialMap.Add(lastItem, s_newItemsPos);
            Assert.Equal(s_initialItems.Count + 1, _spatialMap.Count);

            // Most of the objects can move; 1 is blocked by lastItem, so none move
            bool val = _spatialMap.TryMoveAll(s_initialItemsPos, s_newItemsPos, mask);
            Assert.False(val);
            Assert.Equal(2, layersInMask - 1);
            Assert.Equal(s_initialItems.Count, _spatialMap.GetItemsAt(s_initialItemsPos).Count());
            Assert.Single(_spatialMap.GetItemsAt(s_newItemsPos));
        }

        [Fact]
        public void MoveValidItemsLayerMaskAllValid()
        {
            var movedItems = _spatialMap.MoveValid(s_initialItemsPos, s_newItemsPos, ~_spatialMap.LayerMasker.Mask(2));
            Assert.Equal(s_initialItems.Count - 1, movedItems.Count);
            Assert.Single(_spatialMap.GetItemsAt(s_initialItemsPos));
            Assert.Equal(s_initialItems.Count - 1, _spatialMap.GetItemsAt(s_newItemsPos).Count());
        }

        [Fact]
        public void MoveValidItemsLayerMaskSomeValid()
        {
            // Mask used for operations
            uint mask = _spatialMap.LayerMasker.Mask(1, 2, 3);
            const int layersInMask = 3;

            // Create item and add it to a different position
            var lastItem = new MockSpatialMapItem(3);
            _spatialMap.Add(lastItem, s_newItemsPos);
            Assert.Equal(s_initialItems.Count + 1, _spatialMap.Count);

            // Most of the objects can move; 1 is blocked by lastItem
            int count = _spatialMap.MoveValid(s_initialItemsPos, s_newItemsPos, mask).Count;
            Assert.Equal(layersInMask - 1, count);
            Assert.Equal(s_initialItems.Count - count, _spatialMap.GetItemsAt(s_initialItemsPos).Count());
            Assert.Equal(count + 1, _spatialMap.GetItemsAt(s_newItemsPos).Count());
        }

        [Fact]
        public void MoveValidItemsLayerMaskNoneValid()
        {
            // Create item and add it to a different position
            var lastItem = new MockSpatialMapItem(3);
            _spatialMap.Add(lastItem, s_newItemsPos);
            Assert.Equal(s_initialItems.Count + 1, _spatialMap.Count);

            // The object here cannot move because it's blocked by an existing item
            Assert.Empty(_spatialMap.MoveValid(s_newItemsPos, s_initialItemsPos, _spatialMap.LayerMasker.Mask(3)));
            Assert.Equal(s_initialItems.Count, _spatialMap.GetItemsAt(s_initialItemsPos).Count());
            Assert.Single(_spatialMap.GetItemsAt(s_newItemsPos));
        }

        [Fact]
        public void ContainsItem()
        {
            foreach (var item in s_initialItems)
                Assert.True(_spatialMap.Contains(item));

            // Unadded item is not contained
            var unaddedItem = new MockSpatialMapItem(StartingLayer + 1);
            Assert.False(_spatialMap.Contains(unaddedItem));

            // Moved items are still in the spatial map
            _spatialMap.Move(s_initialItems[0], s_newItemsPos);
            foreach (var item in s_initialItems)
                Assert.True(_spatialMap.Contains(item));
        }

        [Fact]
        public void ContainsPosition()
        {
            Assert.True(_spatialMap.Contains(s_initialItemsPos));
            Assert.False(_spatialMap.Contains(s_newItemsPos));

            // Moved items update results from contained
            _spatialMap.Move(s_initialItems[0], s_newItemsPos);
            Assert.True(_spatialMap.Contains(s_initialItemsPos));
            Assert.True(_spatialMap.Contains(s_newItemsPos));
        }

        // TODO: Remaining functions such as getting all items, etc.

        // TODO: Layer-based function tests for functions other than MoveValid/MoveAll

        [Fact]
        public void GetLayersInMaskExisting()
        {
            int[] layers = { 1, 2, 4, 5 };

            int idx = layers.Length - 1;
            foreach (var layer in _spatialMap.GetLayersInMask(_spatialMap.LayerMasker.Mask(layers)))
            {
                var item = layer.GetItemsAt(s_initialItemsPos).Single();
                Assert.Equal(layers[idx], item.Layer);

                idx--;
            }
        }

        [Fact]
        public void GetLayersInMaskNonExisting()
        {
            int[] layers = { 1, 2, 4, 5 };

            // Any layers not in the layered spatial map are filtered out.
            int idx = layers.Length - 1;
            foreach (var layer in _spatialMap.GetLayersInMask(_spatialMap.LayerMasker.Mask(0, 1, 2, 4, 5, 7)))
            {
                var item = layer.GetItemsAt(s_initialItemsPos).Single();
                Assert.Equal(layers[idx], item.Layer);

                idx--;
            }
        }
    }
}

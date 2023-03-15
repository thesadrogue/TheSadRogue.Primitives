using System.Linq;
using SadRogue.Primitives.SpatialMaps;
using SadRogue.Primitives.UnitTests.Mocks;
using Xunit;

namespace SadRogue.Primitives.UnitTests.SpatialMaps
{
    public class AutoSyncLayeredSpatialMapTests
    {
        [Fact]
        public void SettingPositionAutoSyncsSpatialMap()
        {
            var map = new AutoSyncLayeredSpatialMap<MockPositionableSpatialMapItem>(1, startingLayer: 1, layersSupportingMultipleItems: uint.MaxValue);
            var item = new MockPositionableSpatialMapItem(1, (1, 2));
            map.Add(item);

            Assert.Equal(1, map.Count);
            Assert.Single(map.GetItemsAt(item.Position));

            item.Position = (3, 4);

            Assert.Equal(1, map.Count);
            Assert.Empty(map.GetItemsAt((1, 2)));
            Assert.Single(map.GetItemsAt(item.Position));

            var item2 = new MockPositionableSpatialMapItem(1, (1, 2));
            map.Add(item2);
            item.Position = item2.Position;
            Assert.Equal(2, map.Count);
            Assert.Equal(2, map.GetItemsAt((1, 2)).Count());

            map.Remove(item);
            item.Position = (5, 6); // Validate event handler is unregistered
            Assert.Equal(1, map.Count);
        }

        [Fact]
        public void UsingMoveFunctionsAutoSyncsPosition()
        {
            var map = new AutoSyncLayeredSpatialMap<MockPositionableSpatialMapItem>(1,  startingLayer: 1, layersSupportingMultipleItems: uint.MaxValue);
            var item = new MockPositionableSpatialMapItem(1, (1, 2));
            map.Add(item);

            Assert.Equal(1, map.Count);
            Assert.Single(map.GetItemsAt(item.Position));

            map.Move(item, (3, 4));

            Assert.Equal(1, map.Count);
            Assert.Empty(map.GetItemsAt((1, 2)));
            Assert.Single(map.GetItemsAt(item.Position));
            Assert.Equal(new Point(3, 4), item.Position);

            var item2 = new MockPositionableSpatialMapItem(1, (1, 2));
            map.Add(item2);
            map.Move(item, item2.Position);
            Assert.Equal(2, map.Count);
            Assert.Equal(2, map.GetItemsAt((1, 2)).Count());
            Assert.Equal(item2.Position, item.Position);

            map.Remove(item);
            item.Position = (5, 6); // Attempt to validate handlers are unregistered (move would crash with an object not in the map)
            Assert.Equal(1, map.Count);
        }

        [Fact]
        public void MapIsSyncedWhenPositionChangedTriggers()
        {
            var map = new AutoSyncLayeredSpatialMap<MockPositionableSpatialMapItem>(1,  startingLayer: 1, layersSupportingMultipleItems: uint.MaxValue);
            var item = new MockPositionableSpatialMapItem(1, (1, 2));
            item.PositionChanged += (s, e)
                => Assert.Equal(e.NewValue, map.GetPositionOf((MockPositionableSpatialMapItem)s!));
            map.Add(item);

            item.Position = (3, 4);
            map.Move(item, (2, 3));
        }
    }
}

using System;
using System.Linq;
using SadRogue.Primitives.SpatialMaps;
using SadRogue.Primitives.UnitTests.Mocks;
using Xunit;

namespace SadRogue.Primitives.UnitTests.SpatialMaps
{
    public class AutoSyncSpatialMapTests
    {
        [Fact]
        public void MovementAutoSyncs()
        {
            var map = new AutoSyncMultiSpatialMap<MockPositionableSpatialMapItem>();
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

    }
}

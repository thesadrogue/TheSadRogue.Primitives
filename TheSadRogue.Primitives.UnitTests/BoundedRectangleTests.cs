using Xunit;

namespace SadRogue.Primitives.UnitTests
{
    public class BoundedRectangleTests
    {
        /// <summary>
        /// Test that the bounding box being modified appropriately modifies the area as well.
        /// </summary>
        [Fact]
        public void BoundingBoxModfication()
        {
            var rect = new BoundedRectangle((0, 1, 10, 10), (0, 0, 15, 15));
            Assert.True(rect.BoundingBox.Contains(rect.Area));

            // Bounding box modified to something that the current area will violate
            rect.SetBoundingBox((-10, -10, 10, 10));
            Assert.True(rect.BoundingBox.Contains(rect.Area));
        }
    }
}

using System.Collections.Generic;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    public class RectangleExtensionTests
    {
        #region Test Data
        private static readonly Rectangle[] s_rectangles = { (1, 2, 3, 5), (3, 5, 0, 1), (6, 3, 1, 0), (7, 8, 0, 0) };
        public static IEnumerable<(Rectangle r1, Rectangle r2)> RectanglePairs => s_rectangles.Combinate(s_rectangles);
        #endregion

        #region Comparison
        [Theory]
        [MemberDataTuple(nameof(RectanglePairs))]
        public void Matches(Rectangle r1, Rectangle r2)
        {
            Assert.Equal(r1.Matches(r2), RectangleExtensions.Matches(r1, r2));
        }
        #endregion


    }
}

using System.Collections.Generic;
using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.SerializedTypes.GridViews;
using Xunit;

namespace SadRogue.Primitives.UnitTests.Serialization.SerializedTypesIndividual
{
    /// <summary>
    /// <see cref="DiffAwareGridViewSerialized{T}"/> can't be tested in the normal serialization framework right now
    /// because the base object contains structures which are not guaranteed to be serializable.  Therefore, we will
    /// tests the implicit conversions here.
    /// </summary>
    public class DiffAwareGridViewSerializedTests
    {
        [Fact]
        public void ImplicitConversionTest()
        {
            var view = new ArrayView<bool>(20, 30);
            view.Fill(true);

            var diffAware = new DiffAwareGridView<bool>(view);

            diffAware[(1, 2)] = false;
            diffAware.FinalizeCurrentDiff();

            diffAware.RevertToPreviousDiff();

            DiffAwareGridViewSerialized<bool> serialized = diffAware;
            DiffAwareGridView<bool> deserialized = serialized;

            Assert.Equal(diffAware.CurrentDiffIndex, deserialized.CurrentDiffIndex);
            Assert.Equal((IEnumerable<Diff<bool>>)diffAware.Diffs, deserialized.Diffs);

            Assert.Equal(diffAware.Width, deserialized.Width);
            Assert.Equal(diffAware.Height, deserialized.Height);
            for (int i = 0; i < diffAware.Width * diffAware.Height; i++)
                Assert.Equal(diffAware[i], deserialized[i]);
        }
    }
}

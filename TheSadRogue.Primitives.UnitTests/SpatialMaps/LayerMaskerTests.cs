using System;
using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives.SpatialMaps;
using Xunit;
using Xunit.Abstractions;

namespace SadRogue.Primitives.UnitTests.SpatialMaps
{
    public class LayerMaskTests
    {
        private readonly ITestOutputHelper _output;

        public LayerMaskTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ConstructOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LayerMasker(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new LayerMasker(33));
        }

        [Fact]
        public void AddLayer()
        {
            var masker = new LayerMasker();

            uint mask = masker.Mask(0); // First layer;

            mask = masker.AddLayers(mask, 3);
            Assert.Equal(masker.Mask(0, 3), mask);

            mask = masker.AddLayers(mask, 31, 32);
            Assert.Equal(masker.Mask(0, 3, 31, 32), mask);

            mask = masker.AddLayers(mask, (IEnumerable<int>)new[] { 4, 5, 6 });
            Assert.Equal(masker.Mask(0, 3, 4, 5, 6, 31), mask);

            masker = new LayerMasker(4);
            mask = masker.Mask(1);

            mask = masker.AddLayers(mask, 0);
            Assert.Equal(masker.Mask(1, 0), mask);

            mask = masker.AddLayers(mask, masker.Mask(3, 2));
            Assert.Equal(masker.Mask(0, 1, 2, 3), mask);
        }

        [Fact]
        public void OutOfBoundsLayersExcluded()
        {
            var masker = new LayerMasker(2);

            // Anything out of range is ignored
            uint mask = masker.AddLayers(8, 2);
            Assert.Equal(0U, mask);

            mask = masker.AddLayers(8, 1);
            Assert.Equal(2U, mask);

            mask = masker.AddLayers(1, 2);
            Assert.Equal(1U, mask);

            // Same for IEnumerable version
            mask = masker.AddLayers(8, 2.Yield());
            Assert.Equal(0U, mask);

            mask = masker.AddLayers(8, 1.Yield());
            Assert.Equal(2U, mask);

            mask = masker.AddLayers(1, 2.Yield());
            Assert.Equal(1U, mask);

            // Same for mask version
            mask = masker.AddLayers(8, 4U);
            Assert.Equal(0U, mask);

            mask = masker.AddLayers(8, 2U);
            Assert.Equal(2U, mask);

            mask = masker.AddLayers(1, 4U);
            Assert.Equal(1U, mask);

        }

        [Fact]
        public void GetLayers()
        {
            var masker = new LayerMasker();
            int[] layers = { 0, 2, 5 };

            uint mask = masker.Mask(layers);
            int[] layerReturn = masker.Layers(mask).ToArray();

            layers = layers.OrderByDescending(i => i).ToArray();

            _output.WriteLine("Actual layers");
            _output.WriteLine(layers.ExtendToString());
            _output.WriteLine("Returned layers:");
            _output.WriteLine(layerReturn.ExtendToString());

            Assert.Equal(layers.Length, layerReturn.Length);
            for (int i = 0; i < layers.Length; i++)
                Assert.Equal(layers[i], layerReturn[i]);

            masker = new LayerMasker(3);
            layerReturn = masker.Layers(mask).ToArray();
            layers = layers.OrderByDescending(i => i).Where(i => i < 3).ToArray();
            Assert.Equal(layers.Length, layerReturn.Length);
            for (int i = 0; i < layers.Length; i++)
                Assert.Equal(layers[i], layerReturn[i]);
        }

        [Fact]
        public void Mask()
        {
            var masker = new LayerMasker();

            uint mask = masker.Mask(0, 2, 5);
            Assert.Equal((uint)37, mask);

            mask = masker.Mask(31);
            Assert.Equal(2147483648, mask);

            mask = masker.Mask(Enumerable.Range(0, 32));
            Assert.Equal(uint.MaxValue, masker.AllLayers);
            Assert.Equal(masker.AllLayers, mask);

            mask = masker.NoLayers;
            Assert.Equal((uint)0, mask);

            masker = new LayerMasker(3);
            mask = masker.Mask(0, 2, 5);
            Assert.Equal((uint)5, mask); // 5 should be excluded since only 3 layers

            mask = masker.Mask(Enumerable.Range(0, 32));
            Assert.Equal((uint)7, masker.AllLayers);
            Assert.Equal(masker.AllLayers, mask); // All layers that don't exist are ignored
        }

        [Fact]
        public void MaskAllAbove()
        {
            var masker = new LayerMasker();
            uint mask = masker.MaskAllAbove(3);
            Assert.Equal(uint.MaxValue - 7, mask);

            mask = masker.MaskAllAbove(8);
            Assert.Equal(uint.MaxValue - 255, mask);

            mask = masker.MaskAllAbove(0);
            Assert.Equal(uint.MaxValue, mask);

            mask = masker.MaskAllAbove(31);
            Assert.Equal(2147483648, mask);

            masker = new LayerMasker(3);
            mask = masker.MaskAllAbove(2);
            Assert.Equal((uint)4, mask);

            mask = masker.MaskAllAbove(1);
            Assert.Equal((uint)6, mask);

            mask = masker.MaskAllAbove(3);
            Assert.Equal((uint)0, mask); // Layers should be ignored that don't exist
        }

        [Fact]
        public void MaskAllBelow()
        {
            var masker = new LayerMasker();
            uint mask = masker.MaskAllBelow(3);
            Assert.Equal((uint)15, mask);

            mask = masker.MaskAllBelow(7);
            Assert.Equal((uint)255, mask);

            mask = masker.MaskAllBelow(0);
            Assert.Equal((uint)1, mask);

            mask = masker.MaskAllBelow(31);
            Assert.Equal(uint.MaxValue, mask);

            masker = new LayerMasker(3);
            mask = masker.MaskAllBelow(2);
            Assert.Equal(masker.AllLayers, mask);
            Assert.Equal((uint)7, masker.AllLayers);

            mask = masker.MaskAllBelow(1);
            Assert.Equal((uint)3, mask);

            mask = masker.MaskAllBelow(31);
            Assert.Equal((uint)7, masker.AllLayers);
            Assert.Equal(masker.AllLayers, mask); // Layers should be ignored that don't exist
        }
    }
}

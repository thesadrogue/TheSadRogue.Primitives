using System;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    /// <summary>
    /// Tests for the <see cref="IDGenerator"/> class.
    /// </summary>
    public class IDGeneratorTests
    {
        #region TestData

        public static uint[] StartingValues = { 0U, 1U };

        #endregion
        [Theory]
        [MemberDataEnumerable(nameof(StartingValues))]
        public void BasicConstruction(uint startingValue)
        {
            var gen = new IDGenerator(startingValue);
            Assert.Equal(startingValue, gen.CurrentInteger);
            Assert.False(gen.LastAssigned);
        }

        [Fact]
        public void UseOnLastAssignedThrowsException()
        {
            var gen = new IDGenerator(uint.MaxValue, true);
            Assert.True(gen.LastAssigned);
            Assert.Equal(uint.MaxValue, gen.CurrentInteger);

            Assert.Throws<InvalidOperationException>(() => gen.UseID());
        }

        [Fact]
        public void LastAssignedWithNonMaxIntegerNotAllowed()
        {
            Assert.Throws<ArgumentException>(() => new IDGenerator(0, true));
        }

        [Theory]
        [MemberDataEnumerable(nameof(StartingValues))]
        public void UseIDIncrements(uint startingValue)
        {
            var idGen = new IDGenerator(startingValue);

            for (uint i = startingValue; i < startingValue + 5; i++)
            {
                Assert.Equal(i, idGen.UseID());
                Assert.Equal(i + 1, idGen.CurrentInteger);
            }
        }

        [Fact]
        public void LastIDAssignsCorrectly()
        {
            var idGen = new IDGenerator(uint.MaxValue);
            Assert.False(idGen.LastAssigned);

            Assert.Equal(uint.MaxValue, idGen.UseID());
            Assert.Equal(uint.MaxValue, idGen.CurrentInteger);
            Assert.True(idGen.LastAssigned);

            // Further attempts at assignment fail
            Assert.Throws<InvalidOperationException>(() => idGen.UseID());
        }

        [Fact]
        public void TestMatches()
        {
            var idGen1 = new IDGenerator(5);
            var idGen2 = new IDGenerator(5);
            Assert.True(idGen1.Matches(idGen2));
            Assert.True(idGen2.Matches(idGen1));

            idGen2 = new IDGenerator(6);
            Assert.False(idGen1.Matches(idGen2));
            Assert.False(idGen2.Matches(idGen1));

            idGen1 = new IDGenerator(uint.MaxValue);
            idGen2 = new IDGenerator(uint.MaxValue, true);
            Assert.False(idGen1.Matches(idGen2));
            Assert.False(idGen2.Matches(idGen1));

            Assert.False(idGen1.Matches(null));
        }
    }
}

﻿using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.UnitTests.Mocks;
using Xunit;

namespace SadRogue.Primitives.UnitTests.GridViews
{
    public class SettableGridView1DIndexBaseTests
    {
        #region Clear

        [Fact]
        public void TestClear()
        {
            var view = new SettableGridView1DIndexBaseDefaultImplementationMock<int>(70, 51);
            foreach (var pos in view.Positions())
                view[pos] = 42;

            view.Clear();

            foreach (var pos in view.Positions())
                Assert.Equal(default, view[pos]);
        }
        #endregion
    }
}

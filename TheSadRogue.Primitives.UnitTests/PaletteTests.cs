using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    public class PaletteTests
    {
        #region Test

        // Test cases for testing NearestIndex.  Fairly arbitrary
        public static Color[] DistanceTests =
        {
            Color.Azure, new Color(123, 234, 55), new Color(255, 255, 255), new Color(0, 0, 0)
        };

        #endregion
        #region Constructor

        [Fact]
        public void ConstructorEnumerable()
        {
            var colors = new[] { Color.Purple, Color.Blue, Color.Red };

            var palette = new Palette(colors);
            // ReSharper disable once RedundantCast
            Assert.Equal((IEnumerable<Color>)colors, palette);
            Assert.Equal(colors.Length, palette.Length);
            Assert.Equal(colors.Length, palette.Count);
        }

        [Fact]
        public void ConstructorNumColors()
        {
            var palette = new Palette(4);

            Assert.Equal(4, palette.Length);
            Assert.Equal(4, palette.Count);

            for (int i = 0; i < palette.Length; i++)
                Assert.Equal(new Color(), palette[i]);
        }
        #endregion

        #region Get/Set

        [Fact]
        public void Get()
        {
            var colors = new[] { Color.Purple, Color.Blue, Color.Red };

            var palette = new Palette(colors);

            for (int i = 0; i < colors.Length; i++)
                Assert.Equal(colors[i], palette[i]);
        }

        [Fact]
        public void Set()
        {
            var colors = new[] { Color.Purple, Color.Blue, Color.Red, Color.Green };
            var palette = new Palette(colors.Length);

            for (int i = 0; i < colors.Length; i++)
                palette[i] = colors[i];

            for (int i = 0; i < colors.Length; i++)
                Assert.Equal(colors[i], palette[i]);

            // ReSharper disable once RedundantCast
            Assert.Equal((IEnumerable<Color>)colors, palette);
        }
        #endregion

        #region Enumerable
        [Fact]
        public void EnumerationOfColors()
        {
            var colors = new[] { Color.Purple, Color.Blue, Color.Red, Color.Green };

            var palette = new Palette(colors);

            // ReSharper disable once RedundantCast
            Assert.Equal(colors, (IEnumerable<Color>)palette);

            var objectEnumerableColors = new List<Color>();
            IEnumerator e = palette.GetEnumerator();
            while (e.MoveNext())
            {
                var colorObj = e.Current;
                Assert.NotNull(colorObj);
                var stop = (Color)colorObj;
                objectEnumerableColors.Add(stop);
            }
            Assert.Equal(colors, objectEnumerableColors);
        }

        #endregion

        #region Shift Methods

        [Fact]
        public void ShiftEntireLeft()
        {
            var colors = new[] { Color.Purple, Color.Blue, Color.Red, Color.Green };
            var palette = new Palette(colors);

            for (int numShifts = 1; numShifts <= 2; numShifts++)
            {
                palette.ShiftLeft();

                for (int i = 0; i < palette.Length; i++)
                    Assert.Equal(colors[(i + numShifts) % colors.Length], palette[i]);
            }
        }

        [Fact]
        public void ShiftEntireRight()
        {
            var colors = new[] { Color.Purple, Color.Blue, Color.Red, Color.Green };
            var palette = new Palette(colors);

            for (int numShifts = 1; numShifts <= 2; numShifts++)
            {
                palette.ShiftRight();

                for (int i = 0; i < palette.Length; i++)
                {
                    int shifted = i - numShifts;
                    if (shifted < 0)
                        shifted = palette.Length - Math.Abs(shifted);

                    Assert.Equal(colors[shifted], palette[i]);
                }
            }
        }

        [Fact]
        public void ShiftLeftLowerBoundsCheck()
        {
            var colors = new[] { Color.Purple, Color.Blue, Color.Red, Color.Green };
            var palette = new Palette(colors);

            palette.ShiftLeft(1, colors.Length - 1);

            Assert.Equal(colors[0], palette[0]);

            for (int i = 1; i < palette.Length; i++)
            {
                int shiftedFrom = i + 1;
                if (shiftedFrom >= colors.Length)
                    shiftedFrom = 1 + (colors.Length - shiftedFrom);

                Assert.Equal(colors[shiftedFrom], palette[i]);
            }
        }

        [Fact]
        public void ShiftLeftUpperBoundsCheck()
        {
            var colors = new[] { Color.Purple, Color.Blue, Color.Red, Color.Green };
            var palette = new Palette(colors);

            palette.ShiftLeft(0, colors.Length - 1);

            Assert.Equal(colors[^1], palette[^1]);

            for (int i = 0; i < palette.Length - 1; i++)
            {
                int shiftedFrom = (i + 1) % (colors.Length - 1);
                Assert.Equal(colors[shiftedFrom], palette[i]);
            }
        }

        [Fact]
        public void ShiftLeftRangeOverflowsArray()
        {
            var colors = new[] { Color.Purple, Color.Blue, Color.Red, Color.Green };
            var palette = new Palette(colors);

            Assert.Throws<ArgumentException>(() => palette.ShiftLeft(1, colors.Length));

            Assert.Equal(colors, palette);
        }

        [Fact]
        public void ShiftRightLowerBoundsCheck()
        {
            var colors = new[] { Color.Purple, Color.Blue, Color.Red, Color.Green };
            var palette = new Palette(colors);

            palette.ShiftRight(1, colors.Length - 1);

            Assert.Equal(colors[0], palette[0]);

            for (int i = 1; i < palette.Length; i++)
            {
                int shiftedTo = i + 1;
                if (shiftedTo >= colors.Length)
                    shiftedTo = 1 + (colors.Length - shiftedTo);

                Assert.Equal(colors[i], palette[shiftedTo]);
            }
        }

        [Fact]
        public void ShiftRightUpperBoundsCheck()
        {
            var colors = new[] { Color.Purple, Color.Blue, Color.Red, Color.Green };
            var palette = new Palette(colors);

            palette.ShiftRight(0, colors.Length - 1);

            Assert.Equal(colors[^1], palette[^1]);

            for (int i = 0; i < palette.Length - 1; i++)
            {
                int shiftedTo = (i + 1) % (colors.Length - 1);
                Assert.Equal(colors[i], palette[shiftedTo]);
            }
        }

        [Fact]
        public void ShiftRightRangeOverflowsArray()
        {
            var colors = new[] { Color.Purple, Color.Blue, Color.Red, Color.Green };
            var palette = new Palette(colors);

            Assert.Throws<IndexOutOfRangeException>(() => palette.ShiftRight(1, colors.Length));

            Assert.Equal(colors, palette);
        }

        #endregion

        #region Get Nearest Color
        [Fact]
        public void GetNearestForColorInPalette()
        {
            var colors = new[] { new Color(1, 2, 3), new Color (1, 2, 4), new Color(1, 4, 3), new Color(4, 2, 3) };
            var palette = new Palette(colors);

            for (int i = 0; i < colors.Length; i++)
            {
                Assert.Equal(i, palette.GetNearestIndex(colors[i]));
                Assert.Equal(colors[i], palette.GetNearest(colors[i]));
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(DistanceTests))]
        public void GetNearestByDistance(Color color)
        {
            var colors = new[]
            {
                Color.PeachPuff, Color.Azure, Color.DarkRed, Color.GreenYellow, Color.ForestGreen, Color.DarkSalmon
            };
            var palette = new Palette(colors);

            // Find minimum for test color
            var nearestColors = new HashSet<Color>();
            int nearestDistance = int.MaxValue;

            foreach (var possibleColor in colors)
            {
                // Current implementation intentionally does not account for alpha (though this could change)
                int dist = Math.Abs(color.R - possibleColor.R) + Math.Abs(color.G - possibleColor.G) +
                           Math.Abs(color.G - possibleColor.G);
                if (dist < nearestDistance)
                {
                    nearestColors.Clear();
                    nearestDistance = dist;
                    nearestColors.Add(possibleColor);
                }
                else if (dist == nearestDistance)
                    nearestColors.Add(possibleColor);
            }

            // Assert our function finds one of the minimums
            int nearestIdx = palette.GetNearestIndex(color);
            var nearest = palette.GetNearest(color);

            Assert.Contains(palette[nearestIdx], nearestColors);
            Assert.Contains(nearest, nearestColors);
        }
        #endregion

        #region Matches
        [Fact]
        public void Matches()
        {
            var colors = new[] { Color.Purple, Color.Blue, Color.Red, Color.Green };
            var colors2 = new[] { Color.Purple, Color.Blue, Color.Red, Color.Green };

            var palette = new Palette(colors);
            var palette2 = new Palette(colors2);

            Assert.True(palette.Matches(palette));
            Assert.True(palette2.Matches(palette2));

            Assert.True(palette.Matches(palette2));
            Assert.True(palette2.Matches(palette));

            colors2[1] = new Color(1, 2, 3);
            palette2 = new Palette(colors2);

            Assert.False(palette.Matches(palette2));
            Assert.False(palette2.Matches(palette));
        }

        [Fact]
        public void MatchesDifferentLengths()
        {
            var colors = new[] { Color.Purple, Color.Blue, Color.Red, Color.Green };
            var colors2 = new[] { Color.Purple, Color.Blue, Color.Red, Color.Green, Color.LemonChiffon };

            var palette = new Palette(colors);
            var palette2 = new Palette(colors2);

            Assert.False(palette.Matches(palette2));
        }

        [Fact]
        public void MatchesNull()
        {
            var colors = new[] { Color.Purple, Color.Blue, Color.Red, Color.Green };

            var palette = new Palette(colors);
            Assert.False(palette.Matches(null));
        }
        #endregion

    }
}

using System;
using System.Linq;
using Xunit;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace TheSadRogue.Primitives.UnitTests
{
    public class AdjacencyRuleTests
    {
        public static Direction[] EightWayDirectionsClockwise
            => new[]{ Direction.UP, Direction.UP_RIGHT, Direction.RIGHT, Direction.DOWN_RIGHT, Direction.DOWN,
                Direction.DOWN_LEFT, Direction.LEFT, Direction.UP_LEFT };

        public static Direction[] DiagonalDirectionsClockwise
            => new[]{ Direction.UP_RIGHT,Direction.DOWN_RIGHT, Direction.DOWN_LEFT, Direction.UP_LEFT };

        public static Direction[] CardinalsDirectionsClockwise
            => new[]{ Direction.UP, Direction.RIGHT, Direction.DOWN, Direction.LEFT };

        public static IEnumerable<(AdjacencyRule, Direction)> ClockwisePairs
            => AdjacencyRule.EIGHT_WAY.ToEnumerable().Combinate(EightWayDirectionsClockwise)
            .Concat(AdjacencyRule.CARDINALS.ToEnumerable().Combinate(CardinalsDirectionsClockwise))
            .Concat(AdjacencyRule.DIAGONALS.ToEnumerable().Combinate(DiagonalDirectionsClockwise));

        [Theory]
        [MemberData2(nameof(ClockwisePairs))]
        public void ValidDirectionsClockwiseOrder(AdjacencyRule rule, Direction startingDir)
        {
            Direction[] dirs = null;
            switch(rule.Type)
            {
                case AdjacencyRule.Types.CARDINALS:
                    dirs = CardinalsDirectionsClockwise.ToArray();
                    break;
                case AdjacencyRule.Types.DIAGONALS:
                    dirs = DiagonalDirectionsClockwise.ToArray();
                    break;
                case AdjacencyRule.Types.EIGHT_WAY:
                    dirs = EightWayDirectionsClockwise.ToArray();
                    break;
            }

            Direction[] clockwiseDirs = rule.DirectionsOfNeighborsClockwise(startingDir).ToArray();
            Assert.Equal(startingDir, clockwiseDirs[0]);
            Assert.Equal(clockwiseDirs.Length, dirs.Length);

            int index = Array.FindIndex(dirs, x => x == startingDir);
            Assert.False(index == -1, "Direction found in result that shouldn't be there.");

            for (int i = 0; i < clockwiseDirs.Length; i++)
                Assert.Equal(dirs[(index + i) % dirs.Length], clockwiseDirs[i]);
        }
    }
}

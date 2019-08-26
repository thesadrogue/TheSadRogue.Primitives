using System;
using System.Collections.Generic;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Structure representing a method for determining which coordinates are adjacent to a given
    /// coordinate, and which directions those neighbors are in. Cannot be instantiated -- premade
    /// static instances are provided.
    /// </summary>
    public struct AdjacencyRule : IEquatable<AdjacencyRule>
    {
        /// <summary>
        /// Represents method of determining adjacency where neighbors are considered adjacent if
        /// they are in a cardinal direction, eg. 4-way (manhattan-based) connectivity.
        /// </summary>
        public static readonly AdjacencyRule CARDINALS = new AdjacencyRule(Types.CARDINALS);

        /// <summary>
        /// Represents method of determining adjacency where neighbors are considered adjacent only
        /// if they are in a diagonal direction.
        /// </summary>
        public static readonly AdjacencyRule DIAGONALS = new AdjacencyRule(Types.DIAGONALS);

        /// <summary>
        /// Represents method of determining adjacency where all 8 possible neighbors are considered
        /// adjacent (eg. 8-way connectivity).
        /// </summary>
        public static readonly AdjacencyRule EIGHT_WAY = new AdjacencyRule(Types.EIGHT_WAY);

        private static readonly string[] writeVals = Enum.GetNames(typeof(Types));

        // Constructor, takes type.
        private AdjacencyRule(Types type)
        {
            Type = type;
        }

        /// <summary>
        /// Enum representing <see cref="AdjacencyRule"/> types. Each AdjacencyRule instance has a <see cref="Type"/> field
        /// which contains the corresponding value from this enum.  Useful for easy mapping of AdjacencyRule
        /// types to a primitive type (for cases like a switch statement).
        /// </summary>
        public enum Types
        {
            /// <summary>
            /// Type for <see cref="AdjacencyRule.CARDINALS"/>.
            /// </summary>
            CARDINALS,

            /// <summary>
            /// Type for <see cref="AdjacencyRule.DIAGONALS"/>.
            /// </summary>
            DIAGONALS,

            /// <summary>
            /// Type for <see cref="AdjacencyRule.EIGHT_WAY"/>.
            /// </summary>
            EIGHT_WAY
        }

        /// <summary>
        /// Enum value representing the method of determining adjacency -- useful for using
        /// <see cref="AdjacencyRule"/> types in switch statements.
        /// </summary>
        public readonly Types Type;

        /// <summary>
        /// Gets the <see cref="AdjacencyRule"/> class instance representing the adjacency type specified.
        /// </summary>
        /// <param name="adjacencyRuleType">The enum value for the adjacency method.</param>
        /// <returns>The <see cref="AdjacencyRule"/> class representing the given adjacency method type.</returns>
        public static AdjacencyRule ToAdjacencyRule(Types adjacencyRuleType)
        {
            switch (adjacencyRuleType)
            {
                case Types.CARDINALS:
                    return CARDINALS;

                case Types.DIAGONALS:
                    return DIAGONALS;

                case Types.EIGHT_WAY:
                    return EIGHT_WAY;

                default:
                    throw new Exception($"Could not convert {nameof(Type)} type to {nameof(AdjacencyRule)} -- this is a bug!."); // Will not occur
            }
        }

        /// <summary>
        /// Gets directions leading to neighboring locations, according to the current adjacency
        /// method. Cardinals are returned before any diagonals.
        /// </summary>
        /// <returns>Directions that lead to neighboring locations.</returns>
        public IEnumerable<Direction> DirectionsOfNeighbors()
        {
            switch (Type)
            {
                case Types.CARDINALS:
                    yield return Direction.UP;
                    yield return Direction.DOWN;
                    yield return Direction.LEFT;
                    yield return Direction.RIGHT;
                    break;

                case Types.DIAGONALS:
                    yield return Direction.UP_LEFT;
                    yield return Direction.UP_RIGHT;
                    yield return Direction.DOWN_LEFT;
                    yield return Direction.DOWN_RIGHT;
                    break;

                case Types.EIGHT_WAY:
                    yield return Direction.UP;
                    yield return Direction.DOWN;
                    yield return Direction.LEFT;
                    yield return Direction.RIGHT;
                    yield return Direction.UP_LEFT;
                    yield return Direction.UP_RIGHT;
                    yield return Direction.DOWN_LEFT;
                    yield return Direction.DOWN_RIGHT;
                    break;
            }
        }

        /// <summary>
        /// Gets directions leading to neighboring locations, according to the current adjacency
        /// method. Appropriate directions are returned in clockwise order from the given starting
        /// direction.
        /// </summary>
        /// <param name="startingDirection">The direction to start with.  <see cref="Direction.NONE"/>
        /// causes the default starting direction to be used, which is UP for CARDINALS/EIGHT_WAY, and UP_RIGHT
        /// for diagonals.</param>
        /// <returns>Directions that lead to neighboring locations.</returns>
        public IEnumerable<Direction> DirectionsOfNeighborsClockwise(Direction startingDirection = default)
        {
            switch (Type)
            {
                case Types.CARDINALS:
                    if (startingDirection == Direction.NONE)
                        startingDirection = Direction.UP;

                    if ((int)startingDirection.Type % 2 == 0)
                        startingDirection++; // Make it a cardinal

                    yield return startingDirection;
                    yield return startingDirection + 2;
                    yield return startingDirection + 4;
                    yield return startingDirection + 6;
                    break;

                case Types.DIAGONALS:
                    if (startingDirection == Direction.NONE)
                        startingDirection = Direction.UP_RIGHT;

                    if ((int)startingDirection.Type % 2 == 1)
                        startingDirection++; // Make it a diagonal

                    yield return startingDirection;
                    yield return startingDirection + 2;
                    yield return startingDirection + 4;
                    yield return startingDirection + 6;
                    break;

                case Types.EIGHT_WAY:
                    if (startingDirection == Direction.NONE)
                        startingDirection = Direction.UP;

                    for (int i = 1; i <= 8; i++)
                    {
                        yield return startingDirection;
                        startingDirection++;
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets directions leading to neighboring locations, according to the current adjacency
        /// method. Appropriate directions are returned in counter-clockwise order from the given
        /// starting direction.
        /// </summary>
        /// <param name="startingDirection">The direction to start with.  null or <see cref="Direction.NONE"/>
        /// causes the default starting direction to be used, which is UP for CARDINALS/EIGHT_WAY, and UP_LEFT
        /// for diagonals.</param>
        /// <returns>Directions that lead to neighboring locations.</returns>
        public IEnumerable<Direction> DirectionsOfNeighborsCounterClockwise(Direction startingDirection = default)
        {
            switch (Type)
            {
                case Types.CARDINALS:
                    if (startingDirection == Direction.NONE)
                        startingDirection = Direction.UP;

                    if ((int)startingDirection.Type % 2 == 1)
                        startingDirection--; // Make it a cardinal

                    yield return startingDirection;
                    yield return startingDirection - 2;
                    yield return startingDirection - 4;
                    yield return startingDirection - 6;
                    break;

                case Types.DIAGONALS:
                    if (startingDirection == null || startingDirection == Direction.NONE)
                        startingDirection = Direction.UP_LEFT;

                    if ((int)startingDirection.Type % 2 == 0)
                        startingDirection--; // Make it a diagonal

                    yield return startingDirection;
                    yield return startingDirection - 2;
                    yield return startingDirection - 4;
                    yield return startingDirection - 6;
                    break;

                case Types.EIGHT_WAY:
                    if (startingDirection == null || startingDirection == Direction.NONE)
                        startingDirection = Direction.UP;

                    for (int i = 1; i <= 8; i++)
                    {
                        yield return startingDirection;
                        startingDirection--;
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets all neighbors of the specified location, based on the current adjacency method.
        /// Cardinals are returned before any diagonals.
        /// </summary>
        /// <param name="startingLocation">Location to return neighbors for.</param>
        /// <returns>All neighbors of the given location.</returns>
        public IEnumerable<Point> Neighbors(Point startingLocation)
        {
            foreach (var dir in DirectionsOfNeighbors())
                yield return startingLocation + dir;
        }

        /// <summary>
        /// Gets all neighbors of the specified location, based on the current adjacency method.
        /// Neighbors are returned in clockwise order, starting with the neighbor in the given
        /// starting direction.
        /// </summary>
        /// <param name="startingLocation">Location to return neighbors for.</param>
        /// <param name="startingDirection">
        /// The neighbor in this direction will be returned first, proceeding clockwise.
        /// If <see cref="Direction.NONE"/> is specified, the default starting direction
        /// is used, which is <see cref="Direction.UP"/> for CARDINALS/EIGHT_WAY, and <see cref="Direction.UP_RIGHT"/>
        /// for DIAGONALS.
        /// </param>
        /// <returns>All neighbors of the given location.</returns>
        public IEnumerable<Point> NeighborsClockwise(Point startingLocation, Direction startingDirection = default)
        {
            foreach (var dir in DirectionsOfNeighborsClockwise(startingDirection))
                yield return startingLocation + dir;
        }

        /// <summary>
        /// Gets all neighbors of the specified location, based on the current adjacency method.
        /// Neighbors are returned in counter-clockwise order, starting with the neighbor in the given
        /// starting direction.
        /// </summary>
        /// <param name="startingLocation">Location to return neighbors for.</param>
        /// <param name="startingDirection">
        /// The neighbor in this direction will be returned first, proceeding counter-clockwise.
        /// If <see cref="Direction.NONE"/> is specified, the default starting direction
        /// is used, which is <see cref="Direction.UP"/> for CARDINALS/EIGHT_WAY, and
        /// <see cref="Direction.UP_LEFT"/> for DIAGONALS.
        /// </param>
        /// <returns>All neighbors of the given location.</returns>
        public IEnumerable<Point> NeighborsCounterClockwise(Point startingLocation, Direction startingDirection = default)
        {
            foreach (var dir in DirectionsOfNeighborsCounterClockwise(startingDirection))
                yield return startingLocation + dir;
        }

        /// <summary>
        /// True if the given AdjacencyRule has the same Type the current one.
        /// </summary>
        /// <param name="other">AdjacencyRule to compare.</param>
        /// <returns>True if the two directions are the same, false if not.</returns>
        public bool Equals(AdjacencyRule other) => Type == other.Type;

        /// <summary>
        /// Same as operator == in this case; returns false if <paramref name="obj"/> is not an AdjacencyRule.
        /// </summary>
        /// <param name="obj">The object to compare the current AdjacencyRule to.</param>
        /// <returns>
        /// True if <paramref name="obj"/> is an AdjacencyRule, and the two adjacency rules are equal, false otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is AdjacencyRule c && Equals(c);

        /// <summary>
        /// Returns a hash-map value for the current object.
        /// </summary>
        /// <returns/>
        public override int GetHashCode() => Type.GetHashCode();

        /// <summary>
        /// True if the two adjacency rules have the same Type.
        /// </summary>
        /// <param name="lhs"/>
        /// <param name="rhs"/>
        /// <returns>True if the two adjacency rules are equal, false if not.</returns>
        public static bool operator ==(AdjacencyRule lhs, AdjacencyRule rhs) => lhs.Type == rhs.Type;

        /// <summary>
        /// True if the types are not equal.
        /// </summary>
        /// <param name="lhs"/>
        /// <param name="rhs"/>
        /// <returns>
        /// True if the types are not equal, false if they are both equal.
        /// </returns>
        public static bool operator !=(AdjacencyRule lhs, AdjacencyRule rhs) => !(lhs == rhs);

        /// <summary>
        /// Returns a string representation of the <see cref="AdjacencyRule"/>.
        /// </summary>
        /// <returns>A string representation of the <see cref="AdjacencyRule"/>.</returns>
        public override string ToString() => writeVals[(int)Type];
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Structure representing a method for determining which coordinates are adjacent to a given
    /// coordinate, and which directions those neighbors are in. Cannot be instantiated -- pre-made
    /// static instances are provided.
    /// </summary>
    [DataContract]
    public readonly struct AdjacencyRule : IEquatable<AdjacencyRule>, IMatchable<AdjacencyRule>
    {
        /// <summary>
        /// Represents method of determining adjacency where neighbors are considered adjacent if
        /// they are in a cardinal direction, eg. 4-way (manhattan-based) connectivity.
        /// </summary>
        public static readonly AdjacencyRule Cardinals = new AdjacencyRule(Types.Cardinals);

        /// <summary>
        /// Represents method of determining adjacency where neighbors are considered adjacent only
        /// if they are in a diagonal direction.
        /// </summary>
        public static readonly AdjacencyRule Diagonals = new AdjacencyRule(Types.Diagonals);

        /// <summary>
        /// Represents method of determining adjacency where all 8 possible neighbors are considered
        /// adjacent (eg. 8-way connectivity).
        /// </summary>
        public static readonly AdjacencyRule EightWay = new AdjacencyRule(Types.EightWay);

        private static readonly string[] s_writeValues = Enum.GetNames(typeof(Types));

        // Constructor, takes type.
        private AdjacencyRule(Types type) => Type = type;

        /// <summary>
        /// Enum representing <see cref="AdjacencyRule"/> types. Each AdjacencyRule instance has a <see cref="Type"/> field
        /// which contains the corresponding value from this enum.  Useful for easy mapping of AdjacencyRule
        /// types to a primitive type (for cases like a switch statement).
        /// </summary>
        public enum Types
        {
            /// <summary>
            /// Type for <see cref="AdjacencyRule.Cardinals"/>.
            /// </summary>
            Cardinals,

            /// <summary>
            /// Type for <see cref="AdjacencyRule.Diagonals"/>.
            /// </summary>
            Diagonals,

            /// <summary>
            /// Type for <see cref="AdjacencyRule.EightWay"/>.
            /// </summary>
            EightWay
        }

        /// <summary>
        /// Enum value representing the method of determining adjacency -- useful for using
        /// <see cref="AdjacencyRule"/> types in switch statements.
        /// </summary>
        [DataMember] public readonly Types Type;

        /// <summary>
        /// Gets directions leading to neighboring locations, according to the current adjacency
        /// method. Cardinals are returned before any diagonals.
        /// </summary>
        /// <returns>Directions that lead to neighboring locations.</returns>
        [Pure]
        public IEnumerable<Direction> DirectionsOfNeighbors()
        {
            switch (Type)
            {
                case Types.Cardinals:
                    yield return Direction.Up;
                    yield return Direction.Down;
                    yield return Direction.Left;
                    yield return Direction.Right;
                    break;

                case Types.Diagonals:
                    yield return Direction.UpLeft;
                    yield return Direction.UpRight;
                    yield return Direction.DownLeft;
                    yield return Direction.DownRight;
                    break;

                case Types.EightWay:
                    yield return Direction.Up;
                    yield return Direction.Down;
                    yield return Direction.Left;
                    yield return Direction.Right;
                    yield return Direction.UpLeft;
                    yield return Direction.UpRight;
                    yield return Direction.DownLeft;
                    yield return Direction.DownRight;
                    break;
                default:
                    throw new NotSupportedException(
                        $"{nameof(DirectionsOfNeighbors)} does not support AdjacencyRule type {Type} -- this is a bug!");
            }
        }

        /// <summary>
        /// Gets directions leading to neighboring locations, according to the current adjacency
        /// method. Appropriate directions are returned in clockwise order from the given starting
        /// direction.
        /// </summary>
        /// <param name="startingDirection">The direction to start with.  <see cref="Direction.None"/>
        /// causes the default starting direction to be used, which is UP for CARDINALS/EIGHT_WAY, and UP_RIGHT
        /// for diagonals.</param>
        /// <returns>Directions that lead to neighboring locations.</returns>
        [Pure]
        public IEnumerable<Direction> DirectionsOfNeighborsClockwise(Direction startingDirection = default)
        {
            switch (Type)
            {
                case Types.Cardinals:
                    if (startingDirection == Direction.None)
                        startingDirection = Direction.Up;

                    if ((int)startingDirection.Type % 2 == 0)
                        startingDirection++; // Make it a cardinal

                    yield return startingDirection;
                    yield return startingDirection + 2;
                    yield return startingDirection + 4;
                    yield return startingDirection + 6;
                    break;

                case Types.Diagonals:
                    if (startingDirection == Direction.None)
                        startingDirection = Direction.UpRight;

                    if ((int)startingDirection.Type % 2 == 1)
                        startingDirection++; // Make it a diagonal

                    yield return startingDirection;
                    yield return startingDirection + 2;
                    yield return startingDirection + 4;
                    yield return startingDirection + 6;
                    break;

                case Types.EightWay:
                    if (startingDirection == Direction.None)
                        startingDirection = Direction.Up;

                    for (int i = 1; i <= 8; i++)
                    {
                        yield return startingDirection;
                        startingDirection++;
                    }

                    break;
                default:
                    throw new NotSupportedException(
                        $"{nameof(DirectionsOfNeighborsClockwise)} does not support AdjacencyRule type {Type} -- this is a bug!");
            }
        }

        /// <summary>
        /// Gets directions leading to neighboring locations, according to the current adjacency
        /// method. Appropriate directions are returned in counter-clockwise order from the given
        /// starting direction.
        /// </summary>
        /// <param name="startingDirection">The direction to start with.  null or <see cref="Direction.None"/>
        /// causes the default starting direction to be used, which is UP for CARDINALS/EIGHT_WAY, and UP_LEFT
        /// for diagonals.</param>
        /// <returns>Directions that lead to neighboring locations.</returns>
        [Pure]
        public IEnumerable<Direction> DirectionsOfNeighborsCounterClockwise(Direction startingDirection = default)
        {
            switch (Type)
            {
                case Types.Cardinals:
                    if (startingDirection == Direction.None)
                        startingDirection = Direction.Up;

                    if ((int)startingDirection.Type % 2 == 0)
                        startingDirection--; // Make it a cardinal

                    yield return startingDirection;
                    yield return startingDirection - 2;
                    yield return startingDirection - 4;
                    yield return startingDirection - 6;
                    break;

                case Types.Diagonals:
                    if (startingDirection == Direction.None)
                        startingDirection = Direction.UpLeft;

                    if ((int)startingDirection.Type % 2 == 1)
                        startingDirection--; // Make it a diagonal

                    yield return startingDirection;
                    yield return startingDirection - 2;
                    yield return startingDirection - 4;
                    yield return startingDirection - 6;
                    break;

                case Types.EightWay:
                    if (startingDirection == Direction.None)
                        startingDirection = Direction.Up;

                    for (int i = 1; i <= 8; i++)
                    {
                        yield return startingDirection;
                        startingDirection--;
                    }

                    break;
                default:
                    throw new NotSupportedException(
                        $"{nameof(DirectionsOfNeighborsCounterClockwise)} does not support AdjacencyRule type {Type} -- this is a bug!");
            }
        }

        /// <summary>
        /// True if the given AdjacencyRule has the same Type the current one.
        /// </summary>
        /// <param name="other">AdjacencyRule to compare.</param>
        /// <returns>True if the two directions are the same, false if not.</returns>
        [Pure]
        public bool Matches(AdjacencyRule other) => Equals(other);

        /// <summary>
        /// Gets all neighbors of the specified location, based on the current adjacency method.
        /// Cardinals are returned before any diagonals.
        /// </summary>
        /// <param name="startingLocation">Location to return neighbors for.</param>
        /// <returns>All neighbors of the given location.</returns>
        [Pure]
        public IEnumerable<Point> Neighbors(Point startingLocation)
        {
            foreach (Direction dir in DirectionsOfNeighbors())
                yield return startingLocation + dir;
        }

        /// <summary>
        /// Gets all neighbors of the specified location, based on the current adjacency method.
        /// Cardinals are returned before any diagonals.
        /// </summary>
        /// <param name="startingLocationX">X-value of the location to return neighbors for.</param>
        /// <param name="startingLocationY">Y-value of the location to return neighbors for.</param>
        /// <returns>All neighbors of the given location.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public IEnumerable<Point> Neighbors(int startingLocationX, int startingLocationY)
            => Neighbors(new Point(startingLocationX, startingLocationY));

        /// <summary>
        /// Gets all neighbors of the specified location, based on the current adjacency method.
        /// Neighbors are returned in clockwise order, starting with the neighbor in the given
        /// starting direction.
        /// </summary>
        /// <param name="startingLocation">Location to return neighbors for.</param>
        /// <param name="startingDirection">
        /// The neighbor in this direction will be returned first, proceeding clockwise.
        /// If <see cref="Direction.None"/> is specified, the default starting direction
        /// is used, which is <see cref="Direction.Up"/> for CARDINALS/EIGHT_WAY, and <see cref="Direction.UpRight"/>
        /// for DIAGONALS.
        /// </param>
        /// <returns>All neighbors of the given location.</returns>
        [Pure]
        public IEnumerable<Point> NeighborsClockwise(Point startingLocation, Direction startingDirection = default)
        {
            foreach (Direction dir in DirectionsOfNeighborsClockwise(startingDirection))
                yield return startingLocation + dir;
        }

        /// <summary>
        /// Gets all neighbors of the specified location, based on the current adjacency method.
        /// Neighbors are returned in clockwise order, starting with the neighbor in the given
        /// starting direction.
        /// </summary>
        /// <param name="startingLocationX">X-value of the location to return neighbors for.</param>
        /// <param name="startingLocationY">Y-value of the location to return neighbors for.</param>
        /// <param name="startingDirection">
        /// The neighbor in this direction will be returned first, proceeding clockwise.
        /// If <see cref="Direction.None"/> is specified, the default starting direction
        /// is used, which is <see cref="Direction.Up"/> for CARDINALS/EIGHT_WAY, and <see cref="Direction.UpRight"/>
        /// for DIAGONALS.
        /// </param>
        /// <returns>All neighbors of the given location.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Point> NeighborsClockwise(int startingLocationX, int startingLocationY,
                                                     Direction startingDirection = default)
            => NeighborsClockwise(new Point(startingLocationX, startingLocationY), startingDirection);

        /// <summary>
        /// Gets all neighbors of the specified location, based on the current adjacency method.
        /// Neighbors are returned in counter-clockwise order, starting with the neighbor in the given
        /// starting direction.
        /// </summary>
        /// <param name="startingLocation">Location to return neighbors for.</param>
        /// <param name="startingDirection">
        /// The neighbor in this direction will be returned first, proceeding counter-clockwise.
        /// If <see cref="Direction.None"/> is specified, the default starting direction
        /// is used, which is <see cref="Direction.Up"/> for CARDINALS/EIGHT_WAY, and
        /// <see cref="Direction.UpLeft"/> for DIAGONALS.
        /// </param>
        /// <returns>All neighbors of the given location.</returns>
        [Pure]
        public IEnumerable<Point> NeighborsCounterClockwise(Point startingLocation,
                                                            Direction startingDirection = default)
        {
            foreach (Direction dir in DirectionsOfNeighborsCounterClockwise(startingDirection))
                yield return startingLocation + dir;
        }

        /// <summary>
        /// Gets all neighbors of the specified location, based on the current adjacency method.
        /// Neighbors are returned in counter-clockwise order, starting with the neighbor in the given
        /// starting direction.
        /// </summary>
        /// <param name="startingLocationX">X-value of the location to return neighbors for.</param>
        /// <param name="startingLocationY">Y-value of the location to return neighbors for.</param>
        /// <param name="startingDirection">
        /// The neighbor in this direction will be returned first, proceeding counter-clockwise.
        /// If <see cref="Direction.None"/> is specified, the default starting direction
        /// is used, which is <see cref="Direction.Up"/> for CARDINALS/EIGHT_WAY, and
        /// <see cref="Direction.UpLeft"/> for DIAGONALS.
        /// </param>
        /// <returns>All neighbors of the given location.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Point> NeighborsCounterClockwise(int startingLocationX, int startingLocationY,
                                                            Direction startingDirection = default)
            => NeighborsCounterClockwise(new Point(startingLocationX, startingLocationY), startingDirection);

        /// <summary>
        /// True if the given AdjacencyRule has the same Type the current one.
        /// </summary>
        /// <param name="other">AdjacencyRule to compare.</param>
        /// <returns>True if the two directions are the same, false if not.</returns>
        [Pure]
        public bool Equals(AdjacencyRule other) => Type == other.Type;

        /// <summary>
        /// Same as operator == in this case; returns false if <paramref name="obj"/> is not an AdjacencyRule.
        /// </summary>
        /// <param name="obj">The object to compare the current AdjacencyRule to.</param>
        /// <returns>
        /// True if <paramref name="obj"/> is an AdjacencyRule, and the two adjacency rules are equal, false otherwise.
        /// </returns>
        [Pure]
        public override bool Equals(object? obj) => obj is AdjacencyRule c && Equals(c);

        /// <summary>
        /// Returns a hash-map value for the current object.
        /// </summary>
        /// <returns/>
        [Pure]
        public override int GetHashCode() => Type.GetHashCode();

        /// <summary>
        /// True if the two adjacency rules have the same Type.
        /// </summary>
        /// <param name="lhs"/>
        /// <param name="rhs"/>
        /// <returns>True if the two adjacency rules are equal, false if not.</returns>
        [Pure]
        public static bool operator ==(AdjacencyRule lhs, AdjacencyRule rhs) => lhs.Type == rhs.Type;

        /// <summary>
        /// True if the types are not equal.
        /// </summary>
        /// <param name="lhs"/>
        /// <param name="rhs"/>
        /// <returns>
        /// True if the types are not equal, false if they are both equal.
        /// </returns>
        [Pure]
        public static bool operator !=(AdjacencyRule lhs, AdjacencyRule rhs) => !(lhs == rhs);

        /// <summary>
        /// Implicitly converts an AdjacencyRule to its corresponding <see cref="Type"/>.
        /// </summary>
        /// <param name="rule"/>
        [Pure]
        public static implicit operator Types(AdjacencyRule rule) => rule.Type;

        /// <summary>
        /// Implicitly converts an <see cref="Types"/> enum value to its corresponding AdjacencyRule.
        /// </summary>
        /// <param name="type"/>
        [Pure]
        public static implicit operator AdjacencyRule(Types type) => type switch
        {
            Types.Cardinals => Cardinals,
            Types.Diagonals => Diagonals,
            Types.EightWay => EightWay,
            _ => throw new NotSupportedException(
                $"Could not convert type {type} to {nameof(AdjacencyRule)} -- this is a bug!.")
        };

        /// <summary>
        /// Returns a string representation of the <see cref="AdjacencyRule"/>.
        /// </summary>
        /// <returns>A string representation of the <see cref="AdjacencyRule"/>.</returns>
        [Pure]
        public override string ToString() => s_writeValues[(int)Type];
    }
}

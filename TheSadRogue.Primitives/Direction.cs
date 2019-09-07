using System;
using System.Linq;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Structure representing the concept of a "direction" on a grid, and "defines" the coordinate plane via the
    /// <see cref="Direction.YIncreasesUpward"/> flag. Interacts with Point to allow easy translation
    /// of positions in a direction, and contains numerous helper functions for retrieving directions in
    /// various orders, getting direction closest to a line, etc.
    /// </summary>
    /// <remarks>
    /// The static <see cref="Direction.YIncreasesUpward"/> flag defines the way that many algorithms
    /// interpret the coordinate plane.  By default, this flag is false, meaning that the y-value of positions
    /// is assumed to DECREASE as you proceed in the direction defined by <see cref="Direction.UP"/>, and
    /// increase as you go downward.  If the coordinate plane is displayed on the screen, the origin would be
    /// the top left corner.  This default setting matches the typical console/computer graphic definition of the
    /// coordinate plane.  Setting the flag to true inverts this, so that the y-value of positions INCREASES
    /// as you proceed in the direction defined by <see cref="Direction.UP"/>.  This places the origin in the bottom
    /// left corner, and matches a typical mathmatical definition of a euclidean coordinate plane, as well as the scene
    /// coordinate plane defined by Unity and other game engines.
    /// </remarks>
    public struct Direction : IEquatable<Direction>
    {
        private static readonly string[] writeVals = Enum.GetNames(typeof(Types));

        // All directions that aren't NONE.
        private static readonly Types[] validTypes = Enum.GetValues(typeof(Types)).Cast<Types>().Skip(1).ToArray();
        private static readonly (int dx, int dy)[] deltaVals;

        private static bool _yIncreasesUpward;

        private static bool initYInc;

        static Direction()
        {
            deltaVals = new (int, int)[9];

            // These delta values don't change so we initialize these now
            deltaVals[(int)Types.LEFT] = (-1, 0);
            deltaVals[(int)Types.RIGHT] = (1, 0);
            deltaVals[(int)Types.NONE] = (0, 0);

            // Initialize direction instances to point to each type
            UP = new Direction(Types.UP);
            UP_RIGHT = new Direction(Types.UP_RIGHT);
            RIGHT = new Direction(Types.RIGHT);
            DOWN_RIGHT = new Direction(Types.DOWN_RIGHT);
            DOWN = new Direction(Types.DOWN);
            DOWN_LEFT = new Direction(Types.DOWN_LEFT);
            LEFT = new Direction(Types.LEFT);
            UP_LEFT = new Direction(Types.UP_LEFT);
            NONE = new Direction(Types.NONE);

            // YIncreasesUpward property setter sets all the remaining dx/dy values in the array
            initYInc = false;
            YIncreasesUpward = false; // Initializes rest of distance values
        }

        private Direction(Types type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Enum representing Direction types. Each Direction instance has a <see cref="Type"/> field
        /// which contains the corresponding value from this enum.  Useful for easy mapping of Direction
        /// types to a primitive type (for cases like a switch statement).
        /// </summary>
        public enum Types
        {
            /// <summary>
            /// Type for <see cref="Direction.NONE"/>.
            /// </summary>
            NONE,
            /// <summary>
            /// Type for <see cref="Direction.UP"/>.
            /// </summary>
            UP,

            /// <summary>
            /// Type for <see cref="Direction.UP_RIGHT"/>.
            /// </summary>
            UP_RIGHT,

            /// <summary>
            /// Type for <see cref="Direction.RIGHT"/>.
            /// </summary>
            RIGHT,

            /// <summary>
            /// Type for <see cref="Direction.DOWN_RIGHT"/>.
            /// </summary>
            DOWN_RIGHT,

            /// <summary>
            /// Type for <see cref="Direction.DOWN"/>.
            /// </summary>
            DOWN,

            /// <summary>
            /// Type for <see cref="Direction.DOWN_LEFT"/>.
            /// </summary>
            DOWN_LEFT,

            /// <summary>
            /// Type for <see cref="Direction.LEFT"/>.
            /// </summary>
            LEFT,

            /// <summary>
            /// Type for <see cref="Direction.UP_LEFT"/>.
            /// </summary>
            UP_LEFT
        };

        /// <summary>
        /// Down direction.
        /// </summary>
        public static readonly Direction DOWN;

        /// <summary>
        /// Down-left direction.
        /// </summary>
        public static readonly Direction DOWN_LEFT;

        /// <summary>
        /// Down-right direction.
        /// </summary>
        public static readonly Direction DOWN_RIGHT;

        /// <summary>
        /// Left direction.
        /// </summary>
        public static readonly Direction LEFT;

        /// <summary>
        /// No direction.
        /// </summary>
        public static readonly Direction NONE;

        /// <summary>
        /// Right direction.
        /// </summary>
        public static readonly Direction RIGHT;

        /// <summary>
        /// Up direction.
        /// </summary>
        public static readonly Direction UP;

        /// <summary>
        /// Up-left direction.
        /// </summary>
        public static readonly Direction UP_LEFT;

        /// <summary>
        /// Up-right direction.
        /// </summary>
        public static readonly Direction UP_RIGHT;

        /// <summary>
        /// Whether or not a positive y-value indicates an upward change. Changing this in a multi-threaded environment where a thread might be
        /// in the middle of performing operations using directions can lead to unintended behavior -- it is intended that this configuration be done
        /// as part of an initialization routine.
        /// </summary>
        /// <remarks>
        /// If true, directions with an upwards component represent a positive change in y-value, and ones with downward components
        /// represent a negative change in y-value.  Setting this to false (which is the default) inverts this.
        /// </remarks>
        public static bool YIncreasesUpward
        {
            get { return _yIncreasesUpward; }
            set
            {
                if (_yIncreasesUpward != value || !initYInc)
                {
                    initYInc = true;
                    _yIncreasesUpward = value;
                    yMult = (_yIncreasesUpward) ? -1 : 1;

                    deltaVals[(int)Types.UP] = (0, -1 * yMult);
                    deltaVals[(int)Types.DOWN] = (0, 1 * yMult);
                    deltaVals[(int)Types.UP_LEFT] = (-1, -1 * yMult);
                    deltaVals[(int)Types.UP_RIGHT] = (1, -1 * yMult);
                    deltaVals[(int)Types.DOWN_LEFT] = (-1, 1 * yMult);
                    deltaVals[(int)Types.DOWN_RIGHT] = (1, 1 * yMult);
                }
            }
        }

        /// <summary>
        /// Change in x-value represented by this direction.
        /// </summary>
        public int DeltaX => deltaVals[(int)Type].dx;

        /// <summary>
        /// Change in y-value represented by this direction.
        /// </summary>
        public int DeltaY => deltaVals[(int)Type].dy;

        /// <summary>
        /// Enum type corresponding to direction being represented.
        /// </summary>
        public readonly Types Type;

        /// <summary>
        /// True if the given direction has the same Type the current one.
        /// </summary>
        /// <param name="other">Direction to compare.</param>
        /// <returns>True if the two directions are the same, false if not.</returns>
        public bool Equals(Direction other) => Type == other.Type;

        /// <summary>
        /// Same as operator == in this case; returns false if <paramref name="obj"/> is not a Direction.
        /// </summary>
        /// <param name="obj">The object to compare the current Direction to.</param>
        /// <returns>
        /// True if <paramref name="obj"/> is a Direction, and the two directions are equal, false otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is Direction c && Equals(c);

        /// <summary>
        /// Returns a hash-map value for the current object.
        /// </summary>
        /// <returns/>
        public override int GetHashCode() => Type.GetHashCode();

        /// <summary>
        /// True if the two directions have the same Type.
        /// </summary>
        /// <param name="lhs"/>
        /// <param name="rhs"/>
        /// <returns>True if the two directions are equal, false if not.</returns>
        public static bool operator ==(Direction lhs, Direction rhs) => lhs.Type ==rhs.Type;

        /// <summary>
        /// True if the types are not equal.
        /// </summary>
        /// <param name="lhs"/>
        /// <param name="rhs"/>
        /// <returns>
        /// True if the types are not equal, false if they are both equal.
        /// </returns>
        public static bool operator !=(Direction lhs, Direction rhs) => !(lhs == rhs);

        internal static int yMult { get; private set; }

        /// <summary>
        /// Returns the cardinal direction that most closely matches the degree heading of the given
        /// line. Rounds clockwise if the heading is exactly on a diagonal direction. Similar to
        /// <see cref="GetDirection(Point, Point)"/>, except this function returns only cardinal directions.
        /// </summary>
        /// <param name="start">Starting coordinate of the line.</param>
        /// <param name="end">Ending coordinate of the line.</param>
        /// <returns>
        /// The cardinal direction that most closely matches the heading indicated by the given line.
        /// </returns>
        public static Direction GetCardinalDirection(Point start, Point end) => GetCardinalDirection(new Point(end.X - start.X, end.Y - start.Y));

        /// <summary>
        /// Returns the cardinal direction that most closely matches the degree heading of a line
        /// with the given delta-change values. Rounds clockwise if exactly on a diagonal. Similar to
        /// <see cref="GetDirection(Point)"/>, except this function returns only cardinal directions.
        /// </summary>
        /// <param name="deltaChange">
        /// Vector representing the change in x and change in y across the line (deltaChange.X is the
        /// change in x, deltaChange.Y is the change in y).
        /// </param>
        /// <returns>
        /// The cardinal direction that most closely matches the degree heading of the given line.
        /// </returns>
        public static Direction GetCardinalDirection(Point deltaChange)
        {
            int dx = deltaChange.X;
            int dy = deltaChange.Y;

            if (dx == 0 && dy == 0)
                return NONE;

            dy *= yMult;

            double angle = Math.Atan2(dy, dx);
            double degree = MathHelpers.ToDegree(angle);
            degree += 450; // Rotate angle such that it's all positives, and such that 0 is up.
            degree %= 360; // Normalize angle to 0-360

            if (degree < 45.0)
                return UP;
            if (degree < 135.0)
                return RIGHT;
            if (degree < 225.0)
                return DOWN;
            if (degree < 315.0)
                return LEFT;

            return UP;
        }

        /// <summary>
        /// Returns the direction that most closely matches the degree heading of the given line.
        /// Rounds clockwise if the heading is exactly between two directions.
        /// </summary>
        /// <param name="start">Starting coordinate of the line.</param>
        /// <param name="end">Ending coordinate of the line.</param>
        /// <returns>
        /// The direction that most closely matches the heading indicated by the given line.
        /// </returns>
        public static Direction GetDirection(Point start, Point end) => GetDirection(new Point(end.X - start.X, end.Y - start.Y));


        /// <summary>
        /// Returns the direction that most closely matches the degree heading of a line with the
        /// given delta-change values. Rounds clockwise if the heading is exactly between two directions.
        /// </summary>
        /// <param name="deltaChange">
        /// Vector representing the change in x and change in y across the line (deltaChange.X is the
        /// change in x, deltaChange.Y is the change in y).
        /// </param>
        /// <returns>
        /// The direction that most closely matches the heading indicated by the given input.
        /// </returns>
        public static Direction GetDirection(Point deltaChange)
        {
            int dx = deltaChange.X;
            int dy = deltaChange.Y;

            if (dx == 0 && dy == 0)
                return NONE;

            dy *= yMult;

            double angle = Math.Atan2(dy, dx);
            double degree = MathHelpers.ToDegree(angle);
            degree += 450; // Rotate angle such that it's all positives, and such that 0 is up.
            degree %= 360; // Normalize angle to 0-360

            if (degree < 22.5)
                return UP;
            if (degree < 67.5)
                return UP_RIGHT;
            if (degree < 112.5)
                return RIGHT;
            if (degree < 157.5)
                return DOWN_RIGHT;
            if (degree < 202.5)
                return DOWN;
            if (degree < 247.5)
                return DOWN_LEFT;
            if (degree < 292.5)
                return LEFT;
            if (degree < 337.5)
                return UP_LEFT;

            return UP;
        }

        /// <summary>
        /// Moves the direction counter-clockwise <paramref name="i"/> times.
        /// </summary>
        /// <param name="d"/>
        /// <param name="i"/>
        /// <returns>
        /// The given direction moved counter-clockwise <paramref name="i"/> times.
        /// </returns>
        public static Direction operator -(Direction d, int i) => (d == NONE) ? NONE : ToDirection(validTypes[WrapAround((int)d.Type - i - 1, 8)]);

        /// <summary>
        /// Moves the direction counter-clockwise by one.
        /// </summary>
        /// <param name="d"/>
        /// <returns>The direction one unit counterclockwise of <paramref name="d"/>.</returns>
        public static Direction operator --(Direction d) => (d == NONE) ? NONE : ToDirection(validTypes[WrapAround((int)d.Type - 2, 8)]);

        /// <summary>
        /// Moves the direction clockwise <paramref name="i"/> times.
        /// </summary>
        /// <param name="d"/>
        /// <param name="i"/>
        /// <returns>
        /// The given direction moved clockwise <paramref name="i"/> times.
        /// </returns>
        public static Direction operator +(Direction d, int i) => (d == NONE) ? NONE : ToDirection(validTypes[WrapAround((int)d.Type + i - 1, 8)]);

        /// <summary>
        /// Moves the direction clockwise by one.
        /// </summary>
        /// <param name="d"/>
        /// <returns>The direction one unit clockwise of <paramref name="d"/>.</returns>
        public static Direction operator ++(Direction d) => (d == NONE) ? NONE : ToDirection(validTypes[WrapAround((int)d.Type, 8)]);

        /// <summary>
        /// Gets the Direction class instance representing the direction type specified.
        /// </summary>
        /// <param name="directionType">The enum value for the direction.</param>
        /// <returns>The direction class representing the given direction.</returns>
        public static Direction ToDirection(Types directionType)
        {
            switch (directionType)
            {
                case Types.UP:
                    return UP;

                case Types.UP_RIGHT:
                    return UP_RIGHT;

                case Types.RIGHT:
                    return RIGHT;

                case Types.DOWN_RIGHT:
                    return DOWN_RIGHT;

                case Types.DOWN:
                    return DOWN;

                case Types.DOWN_LEFT:
                    return DOWN_LEFT;

                case Types.LEFT:
                    return LEFT;

                case Types.UP_LEFT:
                    return UP_LEFT;

                case Types.NONE:
                    return NONE;

                default:
                    throw new Exception($"Could not convert {nameof(Type)} instance to {nameof(Direction)} -- this is a bug!."); // Will not occur
            }
        }

        /// <summary>
        /// Returns true if the current direction is a cardinal direction.
        /// </summary>
        /// <returns>True if the current direction is a cardinal direction, false otherwise.</returns>
        public bool IsCardinal() => this != NONE && (DeltaX == 0 || DeltaY == 0);

        /// <summary>
        /// Writes the string (eg. "UP", "UP_RIGHT", etc.) for the direction.
        /// </summary>
        /// <returns>String representation of the direction.</returns>
        public override string ToString() => writeVals[(int)Type];

        #region Tuple Compatibility
        /// <summary>
        /// Translates the given position by one unit in the given direction.
        /// </summary>
        /// <param name="tuple"/>
        /// <param name="d"/>
        /// <returns>
        /// Tuple (tuple.y + d.DeltaX, tuple.y + d.DeltaY).
        /// </returns>
        public static (int x, int y) operator +((int x, int y) tuple, Direction d) => (tuple.x + d.DeltaX, tuple.y + d.DeltaY);
        #endregion

        private static int WrapAround(int num, int wrapTo) => (num % wrapTo + wrapTo) % wrapTo;
    }
}

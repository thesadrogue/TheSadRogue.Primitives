using System;
using System.Collections.Generic;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Class representing different shapes that define the concept of a radius on a grid. You cannot
    /// create instances of this class using a constructor -- instead, this class contains static instances
    /// representing the various radius shapes.
    /// </summary>
    /// <remarks>
    /// Contains utility functions to work with radius shapes.  Instances of Radius are also implicitly
    /// convertible to both <see cref="Distance"/> and <see cref="AdjacencyRule"/> (since both a method
    /// of determining adjacent locations and a method of calculating distance are implied by a radius
    /// shape).
    /// </remarks>
    public class Radius
    {
        /// <summary>
        /// Radius is a circle around the center point. CIRCLE would represent movement radius in
        /// an 8-way movement scheme with a ~1.41 cost multiplier for diagonal movement.
        /// </summary>
        public static readonly Radius CIRCLE = new Radius(Types.CIRCLE);

        /// <summary>
        /// Radius is a diamond around the center point. DIAMOND would represent movement radius
        /// in a 4-way movement scheme.
        /// </summary>
        public static readonly Radius DIAMOND = new Radius(Types.DIAMOND);

        /// <summary>
        /// Radius is a square around the center point. SQUARE would represent movement radius in
        /// an 8-way movement scheme, where all 8 squares around an item are considered equal distance
        /// away.
        /// </summary>
        public static readonly Radius SQUARE = new Radius(Types.SQUARE);

        /// <summary>
        /// Enum value representing the radius shape -- useful for using Radius types in switch
        /// statements.
        /// </summary>
        public readonly Types Type;

        private static readonly string[] writeVals = Enum.GetNames(typeof(Types));

        private Radius(Types type)
        {
            Type = type;
        }

        /// <summary>
        /// Enum representing Radius types. Each Radius instance has a <see cref="Type"/> field
        /// which contains the corresponding value from this enum.  Useful for easy mapping of Radius
        /// types to a primitive type (for cases like a switch statement).
        /// </summary>
        public enum Types
        {
            /// <summary>
            /// Type for Radius.SQUARE.
            /// </summary>
            SQUARE,

            /// <summary>
            /// Type for Radius.DIAMOND.
            /// </summary>
            DIAMOND,

            /// <summary>
            /// Type for Radius.CIRCLE.
            /// </summary>
            CIRCLE,
        };

        /// <summary>
        /// Returns an IEnumerable of all positions in a radius of the current shape defined by the given parameters.
        /// </summary>
        /// <remarks>
        /// If you are getting postions for a radius of the same size frequently, it may be more performant to instead
        /// construct a <see cref="RadiusLocationContext"/> to represent it, and pass that to
        /// <see cref="PositionsInRadius(RadiusLocationContext)"/>.
        /// 
        /// The positions returned are all guaranteed to be within the <paramref name="bounds"/> specified.  As well,
        /// they are guaranteed to be in order from least distance from center to most distance if either
        /// <see cref="Radius.DIAMOND"/> or <see cref="Radius.SQUARE"/> is being used.
        /// </remarks>
        /// <param name="center">Center-point of the radius.</param>
        /// <param name="radius">Length of the radius.</param>
        /// <param name="bounds">Bounds to restrict the returned values by.</param>
        /// <returns>All points in the radius shape defined by the given parameters, in order from least distance to greatest
        /// if <see cref="Radius.DIAMOND"/> or <see cref="Radius.SQUARE"/> is being used.</returns>
        public IEnumerable<Point> PositionsInRadius(Point center, int radius, Rectangle bounds)
            => PositionsInRadius(new RadiusLocationContext(center, radius, bounds));

        /// <summary>
        /// Returns an IEnumerable of all positions in a radius of the current shape defined by the given parameters.
        /// </summary>
        /// <remarks>
        /// If you are getting postions for a radius of the same size frequently, it may be more performant to instead
        /// construct a <see cref="RadiusLocationContext"/> to represent it, and pass that to
        /// <see cref="PositionsInRadius(RadiusLocationContext)"/>.
        /// 
        /// The positions returned are guaranteed to be in order from least distance from center to most distance if either
        /// <see cref="Radius.DIAMOND"/> or <see cref="Radius.SQUARE"/> is being used.
        /// </remarks>
        /// <param name="center">Center-point of the radius.</param>
        /// <param name="radius">Length of the radius.</param>
        /// <returns>All points in the radius shape defined by the given parameters, in order from least distance to greatest
        /// if <see cref="Radius.DIAMOND"/> or <see cref="Radius.SQUARE"/> is being used.</returns>
        public IEnumerable<Point> PositionsInRadius(Point center, int radius)
            => PositionsInRadius(new RadiusLocationContext(center, radius));

        /// <summary>
        /// Returns an IEnumerable of all positions in a radius of the current shape defined by the given context.  Creating
        /// a context to store, and using this function instead of another overload may be more performant when you plan to get
        /// the positions for a radius of the same size multiple times, even if the shape/position are changing.
        /// </summary>
        /// <remarks>
        /// The positions returned are all guaranteed to be within the <paramref name="bounds"/> specified in the context, unless
        /// the bounds are unspecified, in which case no bound restriction results.
        /// 
        /// As well, they are guaranteed to be in order from least distance from center to most distance if either
        /// <see cref="Radius.DIAMOND"/> or <see cref="Radius.SQUARE"/> is being used.
        /// </remarks>
        /// <param name="context">Context defining radius parameters.</param>
        /// <returns>All points in the radius shape defined by the given context, in order from least distance to greatest
        /// if <see cref="Radius.DIAMOND"/> or <see cref="Radius.SQUARE"/> is being used.</returns>
        public IEnumerable<Point> PositionsInRadius(RadiusLocationContext context)
        {
            if (context._newlyInitialized)
                context._newlyInitialized = false;
            else
                Array.Clear(context._inQueue, 0, context._inQueue.Length);

            int startArrayIndex = context._inQueue.GetLength(0) / 2;
            Point topLeft = context.Center - context.Radius;
            AdjacencyRule rule = this;
            Distance distCalc = this;

            var q = new Queue<Point>();
            q.Enqueue(context.Center);
            context._inQueue[startArrayIndex, startArrayIndex] = true;

            Point cur;
            Point localNeighbor;

            while (q.Count != 0)
            {
                cur = q.Dequeue();
                yield return cur;

                // Add neighbors
                foreach (var neighbor in rule.Neighbors(cur))
                {
                    localNeighbor = neighbor - topLeft;

                    if (context._inQueue[localNeighbor.X, localNeighbor.Y] ||
                        context.Bounds != Rectangle.EMPTY && !context.Bounds.Contains(neighbor) ||
                        distCalc.Calculate(context.Center, neighbor) > context.Radius)
                        continue;

                    q.Enqueue(neighbor);
                    context._inQueue[localNeighbor.X, localNeighbor.Y] = true;
                }
            }
        }

        /// <summary>
        /// Allows implicit casting to the <see cref="AdjacencyRule"/> type.
        /// </summary>
        /// <remarks>
        /// The rule corresponding to the proper definition of distance that creates the
        /// radius shape casted will be returned.
        /// </remarks>
        /// <param name="radius">Radius type being casted.</param>
        public static implicit operator AdjacencyRule(Radius radius)
        {
            switch (radius.Type)
            {
                case Types.CIRCLE:
                case Types.SQUARE:
                    return AdjacencyRule.EIGHT_WAY;

                case Types.DIAMOND:
                    return AdjacencyRule.CARDINALS;

                default:
                    return null; // Will not occur
            }
        }

        /// <summary>
        /// Allows implicit casting to the <see cref="Distance"/> type.
        /// </summary>
        /// <remarks>
        /// The <see cref="Distance"/> instance corresponding to the proper definition of
        /// distance that creates the radius shape casted will be returned.
        /// </remarks>
        /// <param name="radius">Radius type being casted.</param>
        public static implicit operator Distance(Radius radius)
        {
            switch (radius.Type)
            {
                case Types.CIRCLE:
                    return Distance.EUCLIDEAN;

                case Types.DIAMOND:
                    return Distance.MANHATTAN;

                case Types.SQUARE:
                    return Distance.CHEBYSHEV;

                default:
                    return null; // Will not occur
            }
        }

        /// <summary>
        /// Gets the Radius class instance representing the radius type specified.
        /// </summary>
        /// <param name="radiusType">The enum value for the radius shape.</param>
        /// <returns>The radius class representing the given radius shape.</returns>
        public static Radius ToRadius(Types radiusType)
        {
            switch (radiusType)
            {
                case Types.CIRCLE:
                    return CIRCLE;

                case Types.DIAMOND:
                    return DIAMOND;

                case Types.SQUARE:
                    return SQUARE;

                default:
                    return null; // Will never occur
            }
        }

        /// <summary>
        /// Returns a string representation of the Radius.
        /// </summary>
        /// <returns>A string representation of the Radius.</returns>
        public override string ToString() => writeVals[(int)Type];
    }

    public class RadiusLocationContext
    {
        internal bool[,] _inQueue;
        internal bool _newlyInitialized;

        private int _radius;
        public int Radius
        {
            get => _radius;
            set
            {
                if (_radius != value)
                {
                    _radius = value;
                    int size = _radius * 2 + 1;
                    _inQueue = new bool[size, size];
                }
            }
        }

        public Point Center { get; set; }

        public Rectangle Bounds;

        public RadiusLocationContext(Point center, int radius, Rectangle bounds)
        {
            Center = center;
            _radius = radius;
            Bounds = bounds;

            int size = _radius * 2 + 1;
            _inQueue = new bool[size, size];
            _newlyInitialized = true;
        }

        public RadiusLocationContext(Point center, int radius)
            : this(center, radius, Rectangle.EMPTY) { }
    }
}

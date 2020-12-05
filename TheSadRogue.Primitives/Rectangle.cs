using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Represents a 2D rectangle. Provides numerous static functions that enable creation and common operations
    /// involving rectangles.
    /// </summary>
    [DataContract]
    public readonly struct Rectangle : IEquatable<Rectangle>, IEquatable<(int x, int y, int width, int height)>,
                                       IMatchable<Rectangle>, IMatchable<(int x, int y, int width, int height)>,
                                       IEquatable<(Point minExtent, Point maxExtent)>, IMatchable<(Point minExtent, Point maxExtent)>
    {
        /// <summary>
        /// The empty rectangle. Has origin of (0, 0) with 0 width and height.
        /// </summary>
        public static readonly Rectangle Empty = new Rectangle(0, 0, 0, 0);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">Minimum x coordinate that is inside the rectangle.</param>
        /// <param name="y">Minimum y coordinate that is inside the rectangle.</param>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="minExtent">Minimum x and y values that are considered inside the rectangle.</param>
        /// <param name="maxExtent">Maximum x and y values that are considered inside the rectangle.</param>
        public Rectangle(Point minExtent, Point maxExtent)
        {
            X = minExtent.X;
            Y = minExtent.Y;
            Width = maxExtent.X - X + 1;
            Height = maxExtent.Y - Y + 1;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="center">The center point of the rectangle.</param>
        /// <param name="horizontalRadius">
        /// Number of units to the left and right of the center point that are included within the rectangle.
        /// </param>
        /// <param name="verticalRadius">
        /// Number of units to the top and bottom of the center point that are included within the rectangle.
        /// </param>
        public Rectangle(Point center, int horizontalRadius, int verticalRadius)
        {
            X = center.X - horizontalRadius;
            Y = center.Y - verticalRadius;
            Width = 2 * horizontalRadius + 1;
            Height = 2 * verticalRadius + 1;
        }

        /// <summary>
        /// Calculates the area of the rectangle.
        /// </summary>
        public int Area => Width * Height;

        /// <summary>
        /// The center coordinate of the rectangle, rounded up if the exact center is between two
        /// positions. The center of a rectangle with width/height 1 is its <see cref="Position"/>.
        /// </summary>
        public Point Center => new Point(X + Width / 2, Y + Height / 2);

        /// <summary>
        /// The height of the rectangle.
        /// </summary>
        [DataMember] public readonly int Height;

        /// <summary>
        /// Whether or not this rectangle is empty (has width and height of 0).
        /// </summary>
        public bool IsEmpty => Width == 0 && Height == 0;

        /// <summary>
        /// The maximum X and Y coordinates that are included in the rectangle.
        /// </summary>
        public Point MaxExtent => new Point(MaxExtentX, MaxExtentY);

        /// <summary>
        /// The maximum X-coordinate that is included in the rectangle.
        /// </summary>
        public int MaxExtentX => X + Width - 1;

        /// <summary>
        /// The maximum Y-coordinate that is included in the rectangle.
        /// </summary>
        public int MaxExtentY => Y + Height - 1;

        /// <summary>
        /// Minimum extent of the rectangle (minimum x and y values that are included within it).
        /// Identical to <see cref="Position"/> because we define the rectangle's position by its
        /// minimum extent.
        /// </summary>
        public Point MinExtent => new Point(X, Y);

        /// <summary>
        /// X-value of the minimum extent of the rectangle (minimum x value that is included within
        /// it). Identical to the <see cref="X"/> value because we define the rectangle's position
        /// by its minimum extent.
        /// </summary>
        public int MinExtentX => X;

        /// <summary>
        /// Y-value of the minimum extent of the rectangle (minimum y value that is included within
        /// it). Identical to the <see cref="Y"/> value because we define the rectangle's position
        /// by its minimum extent.
        /// </summary>
        public int MinExtentY => Y;

        /// <summary>
        /// Coord representing the position (min x- and y-values) of the rectangle.
        /// </summary>
        public Point Position => new Point(X, Y);

        /// <summary>
        /// Returns a coordinate (Width, Height), which represents the size of the rectangle.
        /// </summary>
        public Point Size => new Point(Width, Height);

        /// <summary>
        /// The width of the rectangle.
        /// </summary>
        [DataMember] public readonly int Width;

        /// <summary>
        /// X-coordinate of position of the rectangle.
        /// </summary>
        [DataMember] public readonly int X;

        /// <summary>
        /// Y-coordinate of position of the rectangle.
        /// </summary>
        [DataMember] public readonly int Y;

        /// <summary>
        /// Creates a rectangle with the given minimum and maximum extents. Effectively a
        /// constructor, but with extra overloads not possible to provide in constructors alone.
        /// </summary>
        /// <param name="minExtent">Minimum (x, y) coordinates that are inside the rectangle.</param>
        /// <param name="maxExtent">Maximum (x, y) coordinates that are inside the rectangle.</param>
        /// <returns>A new Rectangle with the given minimum and maximum extents.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle WithExtents(Point minExtent, Point maxExtent) => new Rectangle(minExtent, maxExtent);

        /// <summary>
        /// Creates a rectangle centered on the given position, with the given horizontal and
        /// vertical radius values. Effectively a constructor, but with extra overloads not possible
        /// to provide in constructors alone.
        /// </summary>
        /// <param name="center">Center of the rectangle.</param>
        /// <param name="horizontalRadius">
        /// Number of units to the left and right of the center point that are included within the rectangle.
        /// </param>
        /// <param name="verticalRadius">
        /// Number of units to the top and bottom of the center point that are included within the rectangle.
        /// </param>
        /// <returns>A new rectangle with the given center point and radius values.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle WithRadius(Point center, int horizontalRadius, int verticalRadius)
            => new Rectangle(center, horizontalRadius, verticalRadius);

        /// <summary>
        /// Creates a rectangle with the given position and size. Effectively a constructor, but with
        /// extra overloads not possible to provide in constructors alone.
        /// </summary>
        /// <param name="position">Minimum x/y coordinate that is inside the rectangle.</param>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        /// <returns>A new rectangle at the given position with the given width and height.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle WithPositionAndSize(Point position, int width, int height)
            => new Rectangle(position.X, position.Y, width, height);

        /// <summary>
        /// Creates a rectangle with the given position and size. Effectively a constructor, but with
        /// extra overloads not possible to provide in constructors alone.
        /// </summary>
        /// <param name="position">Minimum (x, y) values that are inside the resulting rectangle.</param>
        /// <param name="size">The size of the rectangle, in form (width, height).</param>
        /// <returns>A new rectangle at the given position with the given size.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle WithPositionAndSize(Point position, Point size)
            => new Rectangle(position.X, position.Y, size.X, size.Y);

        /// <summary>
        /// Gets an <see cref="Primitives.Area"/> representing every location in <paramref name="rect1"/> that
        /// is NOT in <paramref name="rect2"/>.
        /// </summary>
        /// <param name="rect1"/>
        /// <param name="rect2"/>
        /// <returns>A <see cref="Primitives.Area"/> representing every location in <paramref name="rect1"/> that
        /// is NOT in <paramref name="rect2"/>.</returns>
        [Pure]
        public static Area GetDifference(Rectangle rect1, Rectangle rect2)
        {
            var retVal = new Area();

            foreach (Point pos in rect1.Positions())
            {
                if (rect2.Contains(pos))
                    continue;

                retVal.Add(pos);
            }

            return retVal;
        }

        /// <summary>
        /// Gets a <see cref="Primitives.Area"/> representing the exact union of the specified rectangles, eg.
        /// an area containing all locations from either rectangle.
        /// </summary>
        /// <param name="r1"/>
        /// <param name="r2"/>
        /// <returns>A <see cref="Primitives.Area"/> containing every position in either rectangle.</returns>
        [Pure]
        public static Area GetExactUnion(Rectangle r1, Rectangle r2)
        {
            var retVal = new Area();

            for (int x = r1.X; x <= r1.MaxExtentX; x++)
                for (int y = r1.Y; y <= r1.MaxExtentY; y++)
                    retVal.Add(new Point(x, y));

            for (int x = r2.X; x <= r2.MaxExtentX; x++)
                for (int y = r2.Y; y <= r2.MaxExtentY; y++)
                    retVal.Add(new Point(x, y));

            return retVal;
        }

        /// <summary>
        /// Returns the rectangle that represents the intersection of the two rectangles specified,
        /// or the empty rectangle if the specified rectangles do not intersect.
        /// </summary>
        /// <param name="r1"/>
        /// <param name="r2"/>
        /// <returns>
        /// Rectangle representing the intersection of <paramref name="r1"/> and <paramref name="r2"/>, or
        /// the empty rectangle if the two rectangles do not intersect.
        /// </returns>
        [Pure]
        public static Rectangle GetIntersection(Rectangle r1, Rectangle r2)
        {
            if (r1.Intersects(r2))
            {
                int minX = Math.Max(r1.X, r2.X);
                int minY = Math.Max(r1.Y, r2.Y);

                int exclusiveMaxX = Math.Min(r1.X + r1.Width, r2.X + r2.Width);
                int exclusiveMaxY = Math.Min(r1.Y + r1.Height, r2.Y + r2.Height);

                return new Rectangle(minX, minY, exclusiveMaxX - minX, exclusiveMaxY - minY);
            }

            return Empty;
        }

        /// <summary>
        /// Gets the smallest possible rectangle that includes the entire area of both <paramref name="r1"/> and
        /// <paramref name="r2"/>.
        /// </summary>
        /// <param name="r1"/>
        /// <param name="r2"/>
        /// <returns>
        /// The smallest possible rectangle that includes the entire area of both <paramref name="r1"/> and
        /// <paramref name="r2"/>.
        /// </returns>
        [Pure]
        public static Rectangle GetUnion(Rectangle r1, Rectangle r2)
        {
            int x = Math.Min(r1.X, r2.X);
            int y = Math.Min(r1.Y, r2.Y);

            return new Rectangle(x, y, Math.Max(r1.X + r1.Width, r2.X + r2.Width) - x,
                Math.Max(r1.Y + r1.Height, r2.Y + r2.Height) - y);
        }

        /// <summary>
        /// Returns whether or not the rectangles differ in either their positions or extents.
        /// </summary>
        /// <param name="r1"/>
        /// <param name="r2"/>
        /// <returns>true if the rectangles do NOT encompass the same area, false otherwise.</returns>
        [Pure]
        public static bool operator !=(Rectangle r1, Rectangle r2) => !(r1 == r2);

        /// <summary>
        /// Returns whether or not the rectangles have the same position and extents.
        /// </summary>
        /// <param name="r1"/>
        /// <param name="r2"/>
        /// <returns>
        /// true if the area of the two rectangles encompass the exact same area, false otherwise.
        /// </returns>
        [Pure]
        public static bool operator ==(Rectangle r1, Rectangle r2)
            => r1.X == r2.X && r1.Y == r2.Y && r1.Width == r2.Width && r1.Height == r2.Height;

        /// <summary>
        /// Creates and returns a new rectangle that is the same size as the current one, but with
        /// the center moved to the given position.
        /// </summary>
        /// <param name="center">The center-point for the new rectangle.</param>
        /// <returns>
        /// A new rectangle that is the same size as the current one, but with the center moved to
        /// the given location.
        /// </returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle WithCenter(Point center)
            => new Rectangle(center.X - Width / 2, center.Y - Height / 2, Width, Height);

        /// <summary>
        /// Creates and returns a new rectangle whose position is the same as the current one, but
        /// has its height changed by the given delta-change value.
        /// </summary>
        /// <param name="deltaHeight">Delta-change for the height of the new rectangle.</param>
        /// <returns>A new rectangle whose height is modified by the given delta-change value.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle ChangeHeight(int deltaHeight)
            => new Rectangle(X, Y, Width, Height + deltaHeight);

        /// <summary>
        /// Creates and returns a new rectangle whose position is the same as the current one, but
        /// has its width and height changed by the given delta-change values.
        /// </summary>
        /// <param name="deltaChange">
        /// Vector (deltaWidth, deltaHeight) specifying the delta-change values for the width/height
        /// of the new Rectangle.
        /// </param>
        /// <returns>
        /// A new rectangle whose width/height are modified by the given delta-change values.
        /// </returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle ChangeSize(Point deltaChange)
            => new Rectangle(X, Y, Width + deltaChange.X, Height + deltaChange.Y);

        /// <summary>
        /// Creates and returns a new rectangle whose position is the same as the current one, but
        /// has its width changed by the given delta-change value.
        /// </summary>
        /// <param name="deltaWidth">Delta-change for the width of the new rectangle.</param>
        /// <returns>A new rectangle whose width is modified by the given delta-change value.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle ChangeWidth(int deltaWidth)
            => new Rectangle(X, Y, Width + deltaWidth, Height);

        /// <summary>
        /// Creates and returns a new rectangle that has its position moved in the given direction.
        /// </summary>
        /// <param name="direction">The direction to move the new rectangle in.</param>
        /// <returns>A new rectangle that has its position moved in the given direction.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle ChangePosition(Direction direction) => Translate(direction);

        /// <summary>
        /// Creates and returns a new rectangle whose position has been moved by the given
        /// delta-change values.
        /// </summary>
        /// <param name="deltaChange">Delta-x and delta-y values by which to move the new rectangle.</param>
        /// <returns>
        /// A new rectangle, whose position has been moved by the given delta-change values.
        /// </returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle ChangePosition(Point deltaChange) => Translate(deltaChange);

        /// <summary>
        /// Creates and returns a new rectangle whose x-position has been moved by the given delta value.
        /// </summary>
        /// <param name="dx">Value by which to move the new rectangle's x-position.</param>
        /// <returns>A new rectangle, whose x-position has been moved by the given delta-x value.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle ChangeX(int dx) => TranslateX(dx);

        /// <summary>
        /// Creates and returns a new rectangle whose y-position has been moved by the given delta value.
        /// </summary>
        /// <param name="dy">Value by which to move the new rectangle's y-position.</param>
        /// <returns>A new rectangle, whose y-position has been moved by the given delta-y value.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle ChangeY(int dy) => TranslateY(dy);

        /// <summary>
        /// Returns whether or not the specified point is considered within the rectangle.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>Whether or not the specified point is considered within the rectangle.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Point position)
            => position.X >= X && position.X < X + Width && position.Y >= Y && position.Y < Y + Height;

        /// <summary>
        /// Returns whether or not the specified rectangle is considered completely contained within
        /// the current one.
        /// </summary>
        /// <param name="other">The rectangle to check.</param>
        /// <returns>
        /// True if the given rectangle is completely contained within the current one, false otherwise.
        /// </returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Rectangle other) => X <= other.X && other.X + other.Width <= X + Width && Y <= other.Y &&
                                                 other.Y + other.Height <= Y + Height;

        /// <summary>
        /// Compares based upon whether or not the areas contained within the rectangle are identical
        /// in both position and extents.
        /// </summary>
        /// <param name="other"/>
        /// <returns>
        /// true if the area of the two rectangles encompass the exact same area, false otherwise.
        /// </returns>
        [Pure]
        public bool Equals(Rectangle other)
            => X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;

        /// <summary>
        /// Compares to an arbitrary object.
        /// </summary>
        /// <param name="obj"/>
        /// <returns>
        /// true if the object specified is a rectangle instance and encompasses the same area, false otherwise.
        /// </returns>
        [Pure]
        public override bool Equals(object? obj) => obj is Rectangle && this == (Rectangle)obj;

        /// <summary>
        /// Returns a new rectangle, expanded on each side by the given amounts.  Negative change values
        /// can be used to shrink the rectangle on each side.
        /// </summary>
        /// <param name="horizontalChange">
        /// Number of additional columns to include on the left and right of the rectangle.
        /// </param>
        /// <param name="verticalChange">
        /// Number of additional rows to include on the top and bottom of the rectangle.
        /// </param>
        /// <returns>A new rectangle, expanded appropriately.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle Expand(int horizontalChange, int verticalChange)
            => new Rectangle(X - horizontalChange, Y - verticalChange, Width + 2 * horizontalChange,
                Height + 2 * verticalChange);

        /// <summary>
        /// Simple hashing.
        /// </summary>
        /// <returns>Hash code for rectangle.</returns>
        [Pure]
        public override int GetHashCode()
            => X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();

        /// <summary>
        /// Returns whether or not the given rectangle intersects the current one.
        /// </summary>
        /// <param name="other">The rectangle to check.</param>
        /// <returns>True if the given rectangle intersects with the current one, false otherwise.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(Rectangle other) => other.X < X + Width && X < other.X + other.Width &&
                                                   other.Y < Y + Height && Y < other.Y + other.Height;

        /// <summary>
        /// Compares based upon whether or not the areas contained within the rectangle are identical
        /// in both position and extents.
        /// </summary>
        /// <param name="other"/>
        /// <returns>
        /// true if the area of the two rectangles encompass the exact same area, false otherwise.
        /// </returns>
        [Pure]
        public bool Matches(Rectangle other) => Equals(other);

        /// <summary>
        /// Creates and returns a new rectangle that has its <see cref="Position"/> moved to the given position.
        /// </summary>
        /// <param name="position">The position for the new rectangle.</param>
        /// <returns>A new rectangle that has its position changed to the given value.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle WithPosition(Point position)
            => new Rectangle(position.X, position.Y, Width, Height);

        /// <summary>
        /// Creates and returns a new rectangle that has its position moved in the given direction.
        /// </summary>
        /// <param name="direction">The direction to move the new rectangle in.</param>
        /// <returns>A new rectangle that has its position moved in the given direction.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle Translate(Direction direction)
        {
            Point newPos = Position + direction;
            return new Rectangle(newPos.X, newPos.Y, Width, Height);
        }

        /// <summary>
        /// Creates and returns a new rectangle that has its X value moved to the given x-coordinate.
        /// </summary>
        /// <param name="x">The X value for the new rectangle.</param>
        /// <returns>A new rectangle with X changed to the given value.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle WithX(int x) => new Rectangle(x, Y, Width, Height);

        /// <summary>
        /// Creates and returns a new rectangle that has its Y value moved to the given y-coordinate.
        /// </summary>
        /// <param name="y">The Y value for the new rectangle.</param>
        /// <returns>A new rectangle with Y changed to the given value.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle WithY(int y) => new Rectangle(X, y, Width, Height);

        /// <summary>
        /// Returns all positions in the rectangle.
        /// </summary>
        /// <returns>All positions in the rectangle.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Point> Positions()
        {
            for (int y = Y; y <= MaxExtentY; y++)
                for (int x = X; x <= MaxExtentX; x++)
                    yield return new Point(x, y);
        }

        /// <summary>
        /// Creates and returns a new rectangle that has the same position and width as the current
        /// one, but with the height changed to the given value.
        /// </summary>
        /// <param name="height">The height for the new rectangle.</param>
        /// <returns>A new rectangle with its height changed to the given value.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle WithHeight(int height)
            => new Rectangle(X, Y, Width, height);

        /// <summary>
        /// Creates and returns a new rectangle that has been shrunk/expanded as necessary, such that
        /// the maximum extent is the specified value.
        /// </summary>
        /// <param name="maxExtent">The maximum extent of the new rectangle.</param>
        /// <returns>A new rectangle that has its maximum extent adjusted to the specified value.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle WithMaxExtent(Point maxExtent)
            => new Rectangle(MinExtent, maxExtent);

        /// <summary>
        /// Creates and returns a new rectangle that has been shrunk/expanded as necessary, such that
        /// the x-value of maximum extent is changed to the specified value.
        /// </summary>
        /// <param name="x">The x-coordinate for the maximum extent of the new rectangle.</param>
        /// <returns>A new rectangle, with its <see cref="MaxExtentX"/> adjusted to the specified value.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle WithMaxExtentX(int x)
            => new Rectangle(MinExtent, new Point(x, MaxExtentY));

        /// <summary>
        /// Creates and returns a new rectangle that has been shrunk/expanded as necessary, such that
        /// the y-value of maximum extent is changed to the specified value.
        /// </summary>
        /// <param name="y">The y-coordinate for the maximum extent of the new rectangle.</param>
        /// <returns>A new rectangle, with its <see cref="MaxExtentY"/> adjusted to the specified value.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle WithMaxExtentY(int y)
            => new Rectangle(MinExtent, new Point(MaxExtentX, y));

        /// <summary>
        /// Creates and returns a new rectangle that has been shrunk/expanded as necessary, such that
        /// the minimum extent is the specified value.
        /// </summary>
        /// <param name="minExtent">The minimum extent of the new rectangle.</param>
        /// <returns>A new rectangle that has its minimum extent adjusted to the specified value.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle WithMinExtent(Point minExtent)
            => new Rectangle(minExtent, MaxExtent);

        /// <summary>
        /// Creates and returns a new rectangle that has been shrunk/expanded as necessary, such that
        /// the x-value of minimum extent is changed to the specified value.
        /// </summary>
        /// <param name="x">The x-coordinate for the minimum extent of the new rectangle.</param>
        /// <returns>A new rectangle, with its <see cref="MinExtentX"/> adjusted to the specified value.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle WithMinExtentX(int x)
            => new Rectangle(new Point(x, MinExtentY), MaxExtent);

        /// <summary>
        /// Creates and returns a new rectangle that has been shrunk/expanded as necessary, such that
        /// the y-value of minimum extent is changed to the specified value.
        /// </summary>
        /// <param name="y">The y-coordinate for the minimum extent of the new rectangle.</param>
        /// <returns>A new rectangle, with its <see cref="MinExtentY"/> adjusted to the specified value.</returns>
        /// &gt;
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle WithMinExtentY(int y)
            => new Rectangle(new Point(MinExtentX, y), MaxExtent);

        /// <summary>
        /// Creates and returns a new rectangle whose position is the same as the current one, but
        /// has the specified width and height.
        /// </summary>
        /// <param name="width">The width for the new rectangle.</param>
        /// <param name="height">The height for the new rectangle.</param>
        /// <returns>A new rectangle with the given width and height.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle WithSize(int width, int height)
            => new Rectangle(X, Y, width, height);

        /// <summary>
        /// Creates and returns a new rectangle whose position is the same as the current one, but
        /// has the specified width and height.
        /// </summary>
        /// <param name="size">Vector (width, height) specifying the width/height of the new rectangle.</param>
        /// <returns>A new rectangle with the given width and height.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle WithSize(Point size)
            => new Rectangle(X, Y, size.X, size.Y);

        /// <summary>
        /// Creates and returns a new rectangle that is exactly the same as the current one, but with
        /// the width changed to the given value.
        /// </summary>
        /// <param name="width">The width for the new rectangle.</param>
        /// <returns>A new rectangle with its <see cref="Width"/> changed to the given value.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle WithWidth(int width) => new Rectangle(X, Y, width, Height);

        /// <summary>
        /// Returns a string representing the rectangle, formatted as
        /// (<see cref="X"/>, <see cref="Y"/>) -&gt; (<see cref="MaxExtentX"/>, <see cref="MaxExtentY"/>)
        /// </summary>
        /// <returns>String formatted as above.</returns>
        [Pure]
        public override string ToString() => Position + " -> " + MaxExtent;

        /// <summary>
        /// Creates and returns a new rectangle whose position has been moved by the given
        /// delta-change values.
        /// </summary>
        /// <param name="deltaChange">Delta-x and delta-y values by which to move the new rectangle.</param>
        /// <returns>
        /// A new rectangle, whose position has been moved by the given delta-change values.
        /// </returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle Translate(Point deltaChange)
            => new Rectangle(X + deltaChange.X, Y + deltaChange.Y, Width, Height);

        /// <summary>
        /// Creates and returns a new rectangle whose x-position has been moved by the given delta value.
        /// </summary>
        /// <param name="dx">Value by which to move the new rectangle's x-position.</param>
        /// <returns>A new rectangle, whose x-position has been moved by the given delta-x value.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle TranslateX(int dx)
            => new Rectangle(X + dx, Y, Width, Height);

        /// <summary>
        /// Creates and returns a new rectangle whose y-position has been moved by the given delta value.
        /// </summary>
        /// <param name="dy">Value by which to move the new rectangle's y-position.</param>
        /// <returns>A new rectangle, whose y-position has been moved by the given delta-y value.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle TranslateY(int dy)
            => new Rectangle(X, Y + dy, Width, Height);

        #region Tuple Compatibility

        #region (x, y, width, height)

        /// <summary>
        /// Implicitly converts a GoRogue Rectangle to an equivalent tuple of 4 integers (x, y, width, height).
        /// </summary>
        /// <param name="rect" />
        /// <returns />
        [Pure]
        public static implicit operator (int x, int y, int width, int height)(Rectangle rect)
            => (rect.X, rect.Y, rect.Width, rect.Height);

        /// <summary>
        /// Implicitly converts a tuple of 4 integers (x, y, width, height) to an equivalent GoRogue Rectangle.
        /// </summary>
        /// <param name="tuple" />
        /// <returns />
        [Pure]
        public static implicit operator Rectangle((int x, int y, int width, int height) tuple)
            => new Rectangle(tuple.x, tuple.y, tuple.width, tuple.height);

        /// <summary>
        /// Adds support for C# Deconstruction syntax.
        /// </summary>
        /// <param name="x" />
        /// <param name="y" />
        /// <param name="width" />
        /// <param name="height" />
        [Pure]
        public void Deconstruct(out int x, out int y, out int width, out int height)
        {
            x = X;
            y = Y;
            width = Width;
            height = Height;
        }

        /// <summary>
        /// True if the two rectangles represent the same area.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns>True if the two rectangles are equal, false if not.</returns>
        [Pure]
        public static bool operator ==(Rectangle r1, (int x, int y, int width, int height) r2)
            => r1.X == r2.x && r1.Y == r2.y && r1.Width == r2.width && r1.Height == r2.height;

        /// <summary>
        /// True if any of the rectangles' x/y/width/height values are not equal.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns>
        /// True if any of the x/y/width/height values are not equal, false if they are all equal.
        /// </returns>
        [Pure]
        public static bool operator !=(Rectangle r1, (int x, int y, int width, int height) r2) => !(r1 == r2);

        /// <summary>
        /// True if the two rectangles represent the same area.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns>True if the two rectangles are equal, false if not.</returns>
        [Pure]
        public static bool operator ==((int x, int y, int width, int height) r1, Rectangle r2)
            => r1.x == r2.X && r1.y == r2.Y && r1.width == r2.Width && r1.height == r2.Height;

        /// <summary>
        /// True if any of the rectangles' x/y/width/height values are not equal.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns>
        /// True if any of the x/y/width/height values are not equal, false if they are all equal.
        /// </returns>
        [Pure]
        public static bool operator !=((int x, int y, int width, int height) r1, Rectangle r2) => !(r1 == r2);

        /// <summary>
        /// True if the given position has equal x and y values to the current one.
        /// </summary>
        /// <param name="other">Point to compare.</param>
        /// <returns>True if the two positions are equal, false if not.</returns>
        [Pure]
        public bool Equals((int x, int y, int width, int height) other)
            => X == other.x && Y == other.y && Width == other.width && Height == other.height;

        /// <summary>
        /// True if the given position has equal x and y values to the current one.
        /// </summary>
        /// <param name="other">Point to compare.</param>
        /// <returns>True if the two positions are equal, false if not.</returns>
        [Pure]
        public bool Matches((int x, int y, int width, int height) other) => Equals(other);

        #endregion

        #region (minExtent, maxExtent)

        /// <summary>
        /// Implicitly converts a GoRogue Rectangle to an equivalent tuple of 2 Points (minExtent, maxExtent).
        /// </summary>
        /// <param name="rect" />
        /// <returns />
        [Pure]
        public static implicit operator (Point minExtent, Point maxExtent)(Rectangle rect)
            => (rect.MinExtent, rect.MaxExtent);

        /// <summary>
        /// Implicitly converts a tuple of 2 Points (minExtent, maxExtent) to an equivalent GoRogue Rectangle.
        /// </summary>
        /// <param name="tuple" />
        /// <returns />
        [Pure]
        public static implicit operator Rectangle((Point minExtent, Point maxExtent) tuple)
            => new Rectangle(tuple.minExtent, tuple.maxExtent);

        /// <summary>
        /// Adds support for C# Deconstruction syntax.
        /// </summary>
        /// <param name="minExtent" />
        /// <param name="maxExtent" />
        [Pure]
        public void Deconstruct(out Point minExtent, out Point maxExtent)
        {
            minExtent = MinExtent;
            maxExtent = MaxExtent;
        }

        /// <summary>
        /// True if the two rectangles represent the same area.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns>True if the two rectangles are equal, false if not.</returns>
        [Pure]
        public static bool operator ==(Rectangle r1, (Point minExtent, Point maxExtent) r2)
            => r1.MinExtent == r2.minExtent && r1.MaxExtent == r2.maxExtent;

        /// <summary>
        /// True if any of the rectangles' x/y/width/height values are not equal.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns>
        /// True if any of the x/y/width/height values are not equal, false if they are all equal.
        /// </returns>
        [Pure]
        public static bool operator !=(Rectangle r1, (Point minExtent, Point maxExtent) r2) => !(r1 == r2);

        /// <summary>
        /// True if the two rectangles represent the same area.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns>True if the two rectangles are equal, false if not.</returns>
        [Pure]
        public static bool operator ==((Point minExtent, Point maxExtent) r1, Rectangle r2)
            => r1.minExtent == r2.MinExtent && r1.maxExtent == r2.MaxExtent;

        /// <summary>
        /// True if any of the rectangles' x/y/width/height values are not equal.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns>
        /// True if any of the x/y/width/height values are not equal, false if they are all equal.
        /// </returns>
        [Pure]
        public static bool operator !=((Point minExtent, Point maxExtent) r1, Rectangle r2) => !(r1 == r2);

        /// <summary>
        /// True if the given position has equal x and y values to the current one.
        /// </summary>
        /// <param name="other">Point to compare.</param>
        /// <returns>True if the two positions are equal, false if not.</returns>
        [Pure]
        public bool Equals((Point minExtent, Point maxExtent) other)
            => MinExtent == other.minExtent && MaxExtent == other.maxExtent;

        /// <summary>
        /// True if the given position has equal x and y values to the current one.
        /// </summary>
        /// <param name="other">Point to compare.</param>
        /// <returns>True if the two positions are equal, false if not.</returns>
        [Pure]
        public bool Matches((Point minExtent, Point maxExtent) other) => Equals(other);

        #endregion

        #endregion

        /// <summary>
        /// Gets all positions that reside on the inner perimeter of the rectangle.
        /// </summary>
        /// <returns>IEnumerable of all positions that reside on the inner perimeter of the rectangle.</returns>
        [Pure]
        public IEnumerable<Point> PerimeterPositions()
        {
            for (int x = MinExtentX; x <= MaxExtentX; x++)
                yield return new Point(x, MinExtentY); // Minimum y-side perimeter

            for (int y = MinExtentY + 1;
                y <= MaxExtentY;
                y++) // Start offset 1, since last loop returned the corner piece
                yield return new Point(MaxExtentX, y);

            for (int x = MaxExtentX - 1;
                x >= MinExtentX;
                x--) // Again skip 1 because last loop returned the corner piece
                yield return new Point(x, MaxExtentY);

            for (int y = MaxExtentY - 1;
                y >= MinExtentY + 1;
                y--) // Skip 1 on both ends, because last loop returned one corner, first loop returned the other
                yield return new Point(MinExtentX, y);
        }

        /// <summary>
        /// Returns whether or not the given position lines on the given edge of the rectangle.
        /// </summary>
        /// <param name="point"/>
        /// <param name="side"/>
        /// <returns>True if the given position lies along the given edge of the rectangle, false otherwise.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsOnSide(Point point, Direction side) => side.Type switch
        {
            Direction.Types.Up => point.X >= MinExtentX && point.X <= MaxExtentX && point.Y ==
            (Direction.YIncreasesUpward ? MaxExtentY : MinExtentY),
            Direction.Types.Down => point.X >= MinExtentX && point.X <= MaxExtentX && point.Y ==
            (Direction.YIncreasesUpward ? MinExtentY : MaxExtentY),
            Direction.Types.Left => point.Y >= MinExtentY && point.Y <= MaxExtentY && point.X == MinExtentX,
            Direction.Types.Right => point.Y >= MinExtentY && point.Y <= MaxExtentY && point.X == MaxExtentX,
            _ => throw new ArgumentException($"{side} is not a valid side.  Sides must be cardinal directions.",
                nameof(side))
        };

        /// <summary>
        /// Gets all positions that reside on the min-y line of the rectangle.
        /// </summary>
        /// <returns>IEnumerable of all positions that lie on the min-y line of the rectangle.</returns>
        [Pure]
        public IEnumerable<Point> MinYPositions()
        {
            for (int x = MinExtentX; x <= MaxExtentX; x++)
                yield return new Point(x, MinExtentY);
        }

        /// <summary>
        /// Gets all positions that reside on the max-y line of the rectangle.
        /// </summary>
        /// <returns>IEnumerable of all positions that lie on the max-y line of the rectangle.</returns>
        [Pure]
        public IEnumerable<Point> MaxYPositions()
        {
            for (int x = MinExtentX; x <= MaxExtentX; x++)
                yield return new Point(x, MaxExtentY);
        }

        /// <summary>
        /// Gets all positions that reside on the min-x line of the rectangle.
        /// </summary>
        /// <returns>IEnumerable of all positions that lie on the min-x line of the rectangle.</returns>
        [Pure]
        public IEnumerable<Point> MinXPositions()
        {
            for (int y = MinExtentY; y <= MaxExtentY; y++)
                yield return new Point(MinExtentX, y);
        }

        /// <summary>
        /// Gets all positions that reside on the max-x line of the rectangle.
        /// </summary>
        /// <returns>IEnumerable of all positions that lie on the max-x line of the rectangle.</returns>
        [Pure]
        public IEnumerable<Point> MaxXPositions()
        {
            for (int y = MinExtentY; y <= MaxExtentY; y++)
                yield return new Point(MaxExtentX, y);
        }

        /// <summary>
        /// Gets an IEnumerable of all positions that line on the inner perimeter of the rectangle,
        /// on the given side of the rectangle.
        /// </summary>
        /// <param name="side">Side to get positions for.</param>
        /// <returns>IEnumerable of all positions that line on the inner perimeter of the rectangle on the given side.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Point> PositionsOnSide(Direction side) => side.Type switch
        {
            Direction.Types.Up => Direction.YIncreasesUpward ? MaxYPositions() : MinYPositions(),
            Direction.Types.Right => MaxXPositions(),
            Direction.Types.Down => Direction.YIncreasesUpward ? MinYPositions() : MaxYPositions(),
            Direction.Types.Left => MinXPositions(),
            _ => throw new Exception("Cannot retrieve positions on a non-cardinal side of a rectangle.")
        };

        #region Division & Bisection
        /// <summary>
        /// Recursively divides this rectangle in half until the small rectangles are between minimumDimension and 2 * minimumDimenson.
        /// </summary>
        /// <param name="minimumDimension">The smallest allowable dimension for a rectangle to be.</param>
        /// <returns>A list of all rectangles that add up to the original rectangle</returns>
        public IEnumerable<Rectangle> BisectRecursive(int minimumDimension)
        {
            foreach (Rectangle child in Bisect())
            {
                if (child.Width < minimumDimension * 2 && child.Height < minimumDimension * 2)
                    yield return child;

                else
                    foreach (var grandChild in child.BisectRecursive(minimumDimension))
                        yield return grandChild;

            }
        }

        /// <summary>
        /// Bisects the rectangle into two halves along its longest axis.
        /// </summary>
        /// <returns>
        /// An IEnumerable of either Top and Bottom halves, or Left and Right halves,
        /// at indexes 0 and 1 respectively.
        /// </returns>
        /// <remarks>
        /// Cuts the rectangle by reducing it's longest dimension by half. For example, a
        /// rectangle that extends from (3, 3) to (18, 7) that calls DivideInHalf will
        /// return an IEnumerable with two rectangles. The rectangle at index 0
        /// starts at (3, 3) and extends to (9, 7), and the rectangle at index 1 extends from
        /// (10, 3) to (18, 7)
        /// </remarks>
        public IEnumerable<Rectangle> Bisect()
        {
            if (Width > Height)
                return BisectVertically();

            else if (Width < Height)
                return BisectHorizontally();

            else
                return BisectHorizontally();
        }

        /// <summary>
        /// Bisects the rectangle into top and bottom halves
        /// </summary>
        /// <returns>An IEnumerable with Top and Bottom Rectangles in indexes 0 and 1, respectively</returns>
        public IEnumerable<Rectangle> BisectHorizontally()
        {
            int startX = MinExtentX;
            int stopY = MaxExtentY;
            int startY = MinExtentY;
            int stopX = MaxExtentX;
            int bisection = (startY + stopY) / 2;

            yield return new Rectangle(new Point(startX, startY), new Point(stopX, bisection));
            yield return new Rectangle(new Point(startX, bisection + 1), new Point(stopX, stopY));
        }

        /// <summary>
        /// Divides the rectangle into a left and right half.
        /// </summary>
        /// <returns>
        /// An IEnumerable with the Left and Right rectangles in index 0 and 1 respectively.
        /// </returns>
        public IEnumerable<Rectangle> BisectVertically()
        {
            int startY = MinExtentY;
            int stopY = MaxExtentY;
            int startX = MinExtentX;
            int stopX = MaxExtentX;
            int bisection = (startX + stopX) / 2;

            yield return new Rectangle(new Point(startX, startY), new Point(bisection, stopY));
            yield return new Rectangle(new Point(bisection + 1, startY), new Point(stopX, stopY));
        }

        /// <summary>
        /// Divides this rectangle by another, and returns an IEnumerable of divisor-sized rectangles that fit
        /// within the dividend.
        /// </summary>
        /// <param name="divisor">The rectangle by which to divide</param>
        /// <returns>
        /// IEnumerable of Rectangles the size of divisor, that fit within this rectangle.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the width or height of the divisor is 0 or less</exception>
        public IEnumerable<Rectangle> Divide(Rectangle divisor)
        {
            if(divisor.Width <= 0 || divisor.Height <= 0)
                throw new ArgumentOutOfRangeException(nameof(divisor), "Divisor cannot have a width or height of 0.");

            int columns = this.Width / divisor.Width;
            int rows = this.Height / divisor.Height;
            Point origin = (this.MinExtentX, this.MinExtentY);
            Point size = new Point(divisor.Width, divisor.Height) - 1;

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Point start = origin + (divisor.Width * x, divisor.Height * y);
                    yield return new Rectangle(start, start + size);
                }
            }
        }
        #endregion
    }
}

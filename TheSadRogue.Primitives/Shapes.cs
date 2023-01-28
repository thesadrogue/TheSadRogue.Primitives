using System;
using System.Collections.Generic;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Provides implementations of various shape-drawing algorithms which are useful for
    /// for generating shapes on a 2D integer grid.  These include lines, circles, etc.
    /// </summary>
    public static class Shapes
    {
        /// <summary>
        /// Various supported line-drawing algorithms.
        /// </summary>
        public enum LineAlgorithm
        {
            /// <summary>
            /// Bresenham line algorithm.  Points are guaranteed to be in order from start to finish.
            /// </summary>
            Bresenham,

            /// <summary>
            /// Digital Differential Analyzer line algorithm.  It will produce slightly different lines compared to
            /// Bresenham, and it takes approximately the same time as Bresenham (very slightly slower) for most inputs.
            /// Points are guaranteed to be in order from start to finish.
            /// </summary>
            DDA,

            /// <summary>
            /// Line algorithm that takes only orthogonal steps (each grid location on the line
            /// returned is within one cardinal direction of the previous one). Potentially useful
            /// for LOS in games that use MANHATTAN distance. Based on the algorithm
            /// <a href="http://www.redblobgames.com/grids/line-drawing.html#stepping">here</a>.
            /// Points are guaranteed to be in order from start to finish.
            /// </summary>
            Orthogonal
        }

        private const int ModifierX = 0x7fff;
        private const int ModifierY = 0x7fff;

        /// <summary>
        /// Returns an IEnumerable of every point, in order, closest to a line between the two points
        /// specified, using the line drawing algorithm given. The start and end points will be included.
        /// </summary>
        /// <param name="start">The start point of the line.</param>
        /// <param name="end">The end point of the line.</param>
        /// <param name="type">The line-drawing algorithm to use to generate the line.</param>
        /// <returns>
        /// An IEnumerable of every point, in order, closest to a line between the two points
        /// specified (according to the algorithm given).
        /// </returns>
        public static IEnumerable<Point> GetLine(Point start, Point end, LineAlgorithm type = LineAlgorithm.Bresenham)
            => GetLine(start.X, start.Y, end.X, end.Y, type);

        /// <summary>
        /// Returns an IEnumerable of every point, in order, closest to a line between the two points
        /// specified, using the line drawing algorithm given. The start and end points will be included.
        /// </summary>
        /// <param name="startX">X-coordinate of the starting point of the line.</param>
        /// <param name="startY">Y-coordinate of the starting point of the line.</param>
        /// <param name="endX">X-coordinate of the ending point of the line.</param>
        /// ///
        /// <param name="endY">Y-coordinate of the ending point of the line.</param>
        /// <param name="type">The line-drawing algorithm to use to generate the line.</param>
        /// <returns>
        /// An IEnumerable of every point, in order, closest to a line between the two points
        /// specified (according to the algorithm given).
        /// </returns>
        public static IEnumerable<Point> GetLine(int startX, int startY, int endX, int endY,
                                             LineAlgorithm type = LineAlgorithm.Bresenham)
        {
            switch (type)
            {
                case LineAlgorithm.Bresenham:
                    return Bresenham(startX, startY, endX, endY);

                case LineAlgorithm.DDA:
                    return DDA(startX, startY, endX, endY);

                case LineAlgorithm.Orthogonal:
                    return Ortho(startX, startY, endX, endY);

                default:
                    throw new ArgumentException("Unsupported line-drawing algorithm.", nameof(type));
            }
        }

        /// <summary>
        /// Gets the points on the outside of a circle.
        /// </summary>
        /// <param name="center">Center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        public static IEnumerable<Point> GetCircle(Point center, int radius)
            => Circle(center.X, center.Y, radius);


        private static IEnumerable<Point> Circle(int centerX, int centerY, int radius)
        {
            int xi = -radius, yi = 0, err = 2 - 2 * radius; /* II. Quadrant */
            do
            {
                yield return new Point(centerX - xi, centerY + yi); /*   I. Quadrant */
                yield return new Point(centerX - yi, centerY - xi); /*  II. Quadrant */
                yield return new Point(centerX + xi, centerY - yi); /* III. Quadrant */
                yield return new Point(centerX + yi, centerY + xi); /*  IV. Quadrant */
                // plot(centerX - xi, centerY + yi); /*   I. Quadrant */
                // plot(centerX - yi, centerY - xi); /*  II. Quadrant */
                // plot(centerX + xi, centerY - yi); /* III. Quadrant */
                // plot(centerX + yi, centerY + xi); /*  IV. Quadrant */
                radius = err;
                if (radius <= yi)
                    err += ++yi * 2 + 1;           /* e_xy+e_y < 0 */

                if (radius > xi || err > yi)
                    err += ++xi * 2 + 1; /* e_xy+e_x > 0 or no 2nd y-step */

            } while (xi < 0);
        }

        // TODO: Ellipse

        private static IEnumerable<Point> Bresenham(int startX, int startY, int endX, int endY)
        {
            int w = endX - startX;
            int h = endY - startY;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1;
            else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1;
            else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1;
            else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1;
                else if (h > 0) dy2 = 1;
                dx2 = 0;
            }

            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                yield return new Point(startX, startY);
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    startX += dx1;
                    startY += dy1;
                }
                else
                {
                    startX += dx2;
                    startY += dy2;
                }
            }
        }

        private static IEnumerable<Point> DDA(int startX, int startY, int endX, int endY)
        {
            int dx = endX - startX;
            int dy = endY - startY;

            int nx = Math.Abs(dx);
            int ny = Math.Abs(dy);

            // Calculate octant value
            int octant = (dy < 0 ? 4 : 0) | (dx < 0 ? 2 : 0) | (ny > nx ? 1 : 0);
            int fraction = 0;
            int mn = Math.Max(nx, ny);

            int move;

            if (mn == 0)
            {
                yield return new Point(startX, startY);
                yield break;
            }

            if (ny == 0)
            {
                if (dx > 0)
                    for (int x = startX; x <= endX; x++)
                        yield return new Point(x, startY);
                else
                    for (int x = startX; x >= endX; x--)
                        yield return new Point(x, startY);

                yield break;
            }

            if (nx == 0)
            {
                if (dy > 0)
                    for (int y = startY; y <= endY; y++)
                        yield return new Point(startX, y);
                else
                    for (int y = startY; y >= endY; y--)
                        yield return new Point(startX, y);

                yield break;
            }

            switch (octant)
            {
                case 0: // +x, +y
                    move = (ny << 16) / nx;
                    for (int primary = startX; primary <= endX; primary++, fraction += move)
                        yield return new Point(primary, startY + ((fraction + ModifierY) >> 16));
                    break;

                case 1:
                    move = (nx << 16) / ny;
                    for (int primary = startY; primary <= endY; primary++, fraction += move)
                        yield return new Point(startX + ((fraction + ModifierX) >> 16), primary);
                    break;

                case 2: // -x, +y
                    move = (ny << 16) / nx;
                    for (int primary = startX; primary >= endX; primary--, fraction += move)
                        yield return new Point(primary, startY + ((fraction + ModifierY) >> 16));
                    break;

                case 3:
                    move = (nx << 16) / ny;
                    for (int primary = startY; primary <= endY; primary++, fraction += move)
                        yield return new Point(startX - ((fraction + ModifierX) >> 16), primary);
                    break;

                case 6: // -x, -y
                    move = (ny << 16) / nx;
                    for (int primary = startX; primary >= endX; primary--, fraction += move)
                        yield return new Point(primary, startY - ((fraction + ModifierY) >> 16));
                    break;

                case 7:
                    move = (nx << 16) / ny;
                    for (int primary = startY; primary >= endY; primary--, fraction += move)
                        yield return new Point(startX - ((fraction + ModifierX) >> 16), primary);
                    break;

                case 4: // +x, -y
                    move = (ny << 16) / nx;
                    for (int primary = startX; primary <= endX; primary++, fraction += move)
                        yield return new Point(primary, startY - ((fraction + ModifierY) >> 16));
                    break;

                case 5:
                    move = (nx << 16) / ny;
                    for (int primary = startY; primary >= endY; primary--, fraction += move)
                        yield return new Point(startX + ((fraction + ModifierX) >> 16), primary);
                    break;
            }
        }

        private static IEnumerable<Point> Ortho(int startX, int startY, int endX, int endY)
        {
            int dx = endX - startX;
            int dy = endY - startY;

            int nx = Math.Abs(dx);
            int ny = Math.Abs(dy);

            int signX = dx > 0 ? 1 : -1;
            int signY = dy > 0 ? 1 : -1;

            int workX = startX;
            int workY = startY;

            yield return new Point(startX, startY);

            for (int ix = 0, iy = 0; ix < nx || iy < ny;)
            {
                // Optimized version of `if ((0.5 + ix) / nx < (0.5 + iy) / ny)`
                if ((1 + ix + ix) * ny < (1 + iy + iy) * nx)
                {
                    workX += signX;
                    ix++;
                }
                else
                {
                    workY += signY;
                    iy++;
                }

                yield return new Point(workX, workY);
            }
        }
    }
}

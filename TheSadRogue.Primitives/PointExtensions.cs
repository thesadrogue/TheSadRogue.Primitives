namespace SadRogue.Primitives
{
    /// <summary>
    /// Contains set of operators that match ones defined by other packages for interoperability,
    /// so syntax may be uniform.  Functionality is similar to the corresponding actual operators for Point.
    /// </summary>
    public static class PointExtensions
    {
        /// <summary>
        /// Adds the given Point's x/y values to the current Point's x/y values.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns>Position (self.X + other.X, self.Y + other.Y).</returns>
        public static Point Add(this Point self, Point other) => self + other;

        /// <summary>
        /// Adds the given scalar to the current Point's x/y values.
        /// </summary>
        /// <param name="self"/>
        /// <param name="i"/>
        /// <returns>Position (self.X + i, self.Y + i).</returns>
        public static Point Add(this Point self, int i) => self + i;

        /// <summary>
        /// Translates the current position by one unit in the given direction.
        /// </summary>
        /// <param name="self"/>
        /// <param name="dir"/>
        /// <returns>Position (c.X + d.DeltaX, c.Y + d.DeltaY)</returns>
        public static Point Add(this Point self, Direction dir) => self + dir;

        /// <summary>
        /// Subtracts the given Point's x/y values from the current Point's x/y values.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns>Position (self.X - other.X, self.Y - other.Y).</returns>
        public static Point Subtract(this Point self, Point other) => self - other;

        /// <summary>
        /// Subtracts the given scalar from the current Point's x/y values.
        /// </summary>
        /// <param name="self"/>
        /// <param name="i"/>
        /// <returns>Position (self.X - i, self.Y - i).</returns>
        public static Point Subtract(this Point self, int i) => self - i;

        /// <summary>
        /// Multiplies the current Point's x/y values by the given Point's x/y values.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns>Position (self.X * other.X, self.Y * other.Y).</returns>
        public static Point Multiply(this Point self, Point other) => self * other;

        /// <summary>
        /// Multiplies the current Point's x/y values by the given scalar.
        /// </summary>
        /// <param name="self"/>
        /// <param name="i"/>
        /// <returns>Position (self.X * i, self.Y * i).</returns>
        public static Point Multiply(this Point self, int i) => self * i;

        /// <summary>
        /// Multiplies the current Point's x/y values by the given scalar, rounding to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="d"/>
        /// <returns>Position (self.X * d, self.Y * d), with each value rounded to the nearest integer.</returns>
        public static Point Multiply(this Point self, double d) => self * d;

        /// <summary>
        /// Divides the current Point's x/y values by the given Point's x/y values, rounding to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns>Position (self.X / other.X, self.Y / other.Y), with each value rounded to the nearest integer.</returns>
        public static Point Divide(this Point self, Point other) => self / other;

        /// <summary>
        /// Divides the current Point's x/y values by the given scalar, rounding to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="i"/>
        /// <returns>Position(self.X / i, self.Y / i), with each value rounded to the nearest integer.</returns>
        public static Point Divide(this Point self, int i) => self / i;

        /// <summary>
        /// Divides the current Point's x/y values by the given scalar, rounding to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="d"/>
        /// <returns>Position(self.X / d, self.Y / d), with each value rounded to the nearest integer.</returns>
        public static Point Divide(this Point self, double d) => self / d;
    }
}

namespace SadRogue.Primitives
{
    /// <summary>
    /// Contains set of operators that match ones defined by other packages for interoperability,
    /// so syntax may be uniform.  Functionality is similar to the corresponding actual operators for Color.
    /// </summary>
    public static class RectangleExtensions
    {
        /// <summary>
        /// Compares a two rectangles for equality.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        public static bool Matches(this Rectangle self, Rectangle other) => self.Equals(other);
    }
}

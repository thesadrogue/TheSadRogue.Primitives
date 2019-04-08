namespace SadRogue.Primitives
{
	/// <summary>
	/// Contains set of operators that match ones defined by other packages for interoperability,
	/// so syntax may be uniform.  Functionality is similar to the corresponding actual operators for Point.
	/// </summary>
	public static class PointExtensions
	{
		// TODO: Multiply and divide
		public static Point Add(this Point self, Point other) => self + other;
		public static Point Add(this Point self, int i) => self + i;
		public static Point Add(this Point self, Direction dir) => self + dir;

		public static Point Subtract(this Point self, Point other) => self - other;
		public static Point Subtract(this Point self, int i) => self - i;


	}
}

using System;

namespace SadRogue.Primitives
{
    /// <summary>
	/// This class defines a 2D rectanglar area, whose area is automatically "locked" to
	/// being inside a rectangular bounding box as it is changed. A typical use might be
	/// keeping track of a camera's view area.
	/// </summary>
    [Serializable]
	public class BoundedRectangle : IEquatable<BoundedRectangle>
    {
        private Rectangle _area;
        private Rectangle _boundingBox;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="area">Initial area for the rectangle.</param>
        /// <param name="boundingBox">Initial bounding box by which to bound the rectangle.</param>
        public BoundedRectangle(Rectangle area, Rectangle boundingBox)
        {
            _boundingBox = boundingBox;
            _area = area;

            boundLock();
        }

        /// <summary>
        /// The rectangle that is guaranteed to be contained completely within <see cref="BoundingBox"/>,
        /// and will be restricted as such when set to.
        /// </summary>
        public Rectangle Area
        {
            get => _area;

            set
            {
                _area = value;
                if (!_boundingBox.Contains(_area))
                    boundLock();
            }
        }

        /// <summary>
        /// The rectangle which <see cref="Area"/> is automatically bounded to be within.  Although this
        /// property does not explicitly provide a set accessor, it is returning a reference so therefore
        /// the property may be assigned to.
        /// </summary>
        public ref Rectangle BoundingBox
        {
            get => ref _boundingBox;
        }

        /// <summary>
        /// True if the given BoundedRectangle has the same Bounds and Area as the current one.
        /// </summary>
        /// <param name="other">BoundedRectangle to compare.</param>
        /// <returns>True if the two BoundedRectangles are the same, false if not.</returns>
        public bool Equals(BoundedRectangle other) => !ReferenceEquals(other, null) && _area == other._area && _boundingBox == other._boundingBox;

        /// <summary>
        /// Same as operator == in this case; returns false if <paramref name="obj"/> is not a BoundedRectangle.
        /// </summary>
        /// <param name="obj">The object to compare the current BoundedRectangle to.</param>
        /// <returns>
        /// True if <paramref name="obj"/> is a BoundedRectangle, and the two BoundedRectangles are the same, false otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is BoundedRectangle c && Equals(c);

        /// <summary>
        /// Returns a hash-map value for the current object.
        /// </summary>
        /// <returns/>
        public override int GetHashCode() => _area.GetHashCode() ^ _boundingBox.GetHashCode();

        /// <summary>
        /// True if the two BoundedRectangle objects are equivalent.
        /// </summary>
        /// <param name="lhs"/>
        /// <param name="rhs"/>
        /// <returns>True if the two BoundedRectangle objects are equivalent, false if not.</returns>
        public static bool operator ==(BoundedRectangle lhs, BoundedRectangle rhs) => lhs?.Equals(rhs) ?? rhs is null;

        /// <summary>
        /// True if the types are not equal.
        /// </summary>
        /// <param name="lhs"/>
        /// <param name="rhs"/>
        /// <returns>
        /// True if the types are not equal, false if they are both equal.
        /// </returns>
        public static bool operator !=(BoundedRectangle lhs, BoundedRectangle rhs) => !(lhs == rhs);

        private void boundLock()
        {
            int x = _area.X, y = _area.Y, width = _area.Width, height = _area.Height;

            if (width > _boundingBox.Width)
                width = _boundingBox.Width;
            if (height > _boundingBox.Height)
                height = _boundingBox.Height;

            if (x < _boundingBox.X)
                x = _boundingBox.X;
            if (y < _boundingBox.Y)
                y = _boundingBox.Y;

            if (x > _boundingBox.MaxExtentX - width + 1)
                x = _boundingBox.MaxExtentX - width + 1;
            if (y > _boundingBox.MaxExtentY - height + 1)
                y = _boundingBox.MaxExtentY - height + 1;

            _area = new Rectangle(x, y, width, height);
        }
    }
}

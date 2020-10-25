using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace SadRogue.Primitives
{
    /// <summary>
    /// This class defines a 2D rectangular area, whose area is automatically "locked" to
    /// being inside a rectangular bounding box as it is changed. A typical use might be
    /// keeping track of a camera's view area.
    /// </summary>
    [DataContract]
    public class BoundedRectangle : IMatchable<BoundedRectangle>
    {
        [DataMember] private Rectangle _area;
        // A bug in code cleanup will add the readonly modifier to this field even though it would break the BoundingBox property at compile-time,
        // unless we disable the warning for this line
#pragma warning disable IDE0044
        [DataMember] private Rectangle _boundingBox;
#pragma warning restore IDE0044

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="area">Initial area for the rectangle.</param>
        /// <param name="boundingBox">Initial bounding box by which to bound the rectangle.</param>
        public BoundedRectangle(Rectangle area, Rectangle boundingBox)
        {
            _boundingBox = boundingBox;
            _area = area;

            BoundLock();
        }

        /// <summary>
        /// The rectangle that is guaranteed to be contained completely within <see cref="BoundingBox" />.
        /// Use <see cref="SetArea(Rectangle)" /> to set the area.
        /// </summary>
        public ref readonly Rectangle Area => ref _area;

        /// <summary>
        /// The rectangle which <see cref="Area" /> is automatically bounded to be within.  Use the
        /// <see cref="SetBoundingBox(Rectangle)" /> property to set the bounding box.
        /// </summary>
        public ref readonly Rectangle BoundingBox => ref _boundingBox;

        /// <summary>
        /// True if the given BoundedRectangle has the same Bounds and Area as the current one.
        /// </summary>
        /// <param name="other">BoundedRectangle to compare.</param>
        /// <returns>True if the two BoundedRectangles are the same, false if not.</returns>
        public bool Matches([AllowNull]BoundedRectangle other)
            => !ReferenceEquals(other, null) && _area == other._area && _boundingBox == other._boundingBox;

        /// <summary>
        /// Forces the area given to conform to the <see cref="BoundingBox" /> specified and sets it to <see cref="Area" />.
        /// </summary>
        /// <param name="newArea">The new area to bound and set.</param>
        public void SetArea(Rectangle newArea)
        {
            _area = newArea;
            if (!_boundingBox.Contains(_area))
                BoundLock();
        }

        /// <summary>
        /// Sets the bounding box to the specified value, and forces the current area to fit within the bounding box
        /// as needed.
        /// </summary>
        /// <param name="newBoundingBox">The new bounding box to apply.</param>
        public void SetBoundingBox(Rectangle newBoundingBox)
        {
            _boundingBox = newBoundingBox;
            if (!_boundingBox.Contains(_area))
                BoundLock();
        }

        private void BoundLock()
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

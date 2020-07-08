using System;

namespace SadRogue.Primitives.SerializedTypes
{
    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="BoundedRectangle"/>.
    /// </summary>
    [Serializable]
    public struct BoundedRectangleSerialized
    {
        /// <summary>
        /// Area the rectangle encompasses.
        /// </summary>
        public RectangleSerialized Area;

        /// <summary>
        /// Bounds the area is restricted to.
        /// </summary>
        public RectangleSerialized Bounds;

        /// <summary>
        /// Converts <see cref="BoundedRectangle"/> to <see cref="BoundedRectangleSerialized"/>.
        /// </summary>
        /// <param name="rect"/>
        /// <returns/>
        public static implicit operator BoundedRectangleSerialized(BoundedRectangle rect) =>
            new BoundedRectangleSerialized() { Area = rect.Area, Bounds = rect.BoundingBox };

        /// <summary>
        /// Converts <see cref="BoundedRectangleSerialized"/> to <see cref="BoundedRectangle"/>.
        /// </summary>
        /// <param name="rect"/>
        /// <returns/>
        public static implicit operator BoundedRectangle(BoundedRectangleSerialized rect) =>
            new BoundedRectangle(rect.Area, rect.Bounds);
    }
}

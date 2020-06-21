using System;

namespace SadRogue.Primitives.SerializedTypes
{
    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="BoundedRectangle"/>.
    /// </summary>
    [Serializable]
    public struct BoundedRectangleSerialized
    {
        public RectangleSerialized Area;
        public RectangleSerialized Bounds;

        public static implicit operator BoundedRectangleSerialized(BoundedRectangle rect) =>
            new BoundedRectangleSerialized()
        {
            Area = rect.Area,
            Bounds = rect.BoundingBox
        };

        public static implicit operator BoundedRectangle(BoundedRectangleSerialized rect) =>
            new BoundedRectangle(rect.Area, rect.Bounds);
    }
}

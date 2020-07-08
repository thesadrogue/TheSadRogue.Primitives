using System;

namespace SadRogue.Primitives.SerializedTypes
{
    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="Point"/>.
    /// </summary>
    [Serializable]
    public struct PointSerialized
    {
        /// <summary>
        /// X-coordinate of the point.
        /// </summary>
        public int X;

        /// <summary>
        /// Y-coordinate of the point.
        /// </summary>
        public int Y;

        /// <summary>
        /// Converts from <see cref="Point"/> to <see cref="PointSerialized"/>.
        /// </summary>
        /// <param name="point"/>
        /// <returns/>
        public static implicit operator PointSerialized(Point point)
            => new PointSerialized() { X = point.X, Y = point.Y };

        /// <summary>
        /// Converts from <see cref="PointSerialized"/> to <see cref="Point"/>.
        /// </summary>
        /// <param name="serialized"/>
        /// <returns/>
        public static implicit operator Point(PointSerialized serialized)
            => new Point(serialized.X, serialized.Y);
    }
}

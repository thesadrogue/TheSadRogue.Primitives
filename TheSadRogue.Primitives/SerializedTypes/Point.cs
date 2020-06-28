using System;

namespace SadRogue.Primitives.SerializedTypes
{
    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="Point"/>.
    /// </summary>
    [Serializable]
    public struct PointSerialized
    {
        public int X;
        public int Y;

        public static implicit operator PointSerialized(Point point) => new PointSerialized() { X = point.X, Y = point.Y };

        public static implicit operator Point(PointSerialized point) => new Point(point.X, point.Y);
    }
}

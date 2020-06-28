using System;

namespace SadRogue.Primitives.SerializedTypes
{
    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="Rectangle"/>.
    /// </summary>
    [Serializable]
    public struct RectangleSerialized
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public static implicit operator RectangleSerialized(Rectangle rect) => new RectangleSerialized()
        {
            X = rect.X,
            Y = rect.Y,
            Width = rect.Width,
            Height = rect.Height
        };

        public static implicit operator Rectangle(RectangleSerialized rect) => new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
    }
}

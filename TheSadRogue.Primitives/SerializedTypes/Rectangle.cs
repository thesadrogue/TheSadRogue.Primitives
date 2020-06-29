using System;

namespace SadRogue.Primitives.SerializedTypes
{
    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="Rectangle"/>.
    /// </summary>
    [Serializable]
    public struct RectangleSerialized
    {
        /// <summary>
        /// X-coordinate of the minimum extent of the rectangle.
        /// </summary>
        public int X;

        /// <summary>
        /// Y-coordinate of the minimum extent of the rectangle.
        /// </summary>
        public int Y;

        /// <summary>
        /// Width of the rectangle.
        /// </summary>
        public int Width;

        /// <summary>
        /// Height of the rectangle.
        /// </summary>
        public int Height;

        /// <summary>
        /// Converts from <see cref="Rectangle"/> to <see cref="RectangleSerialized"/>.
        /// </summary>
        /// <param name="rect"/>
        /// <returns/>
        public static implicit operator RectangleSerialized(Rectangle rect) => new RectangleSerialized()
        {
            X = rect.X, Y = rect.Y, Width = rect.Width, Height = rect.Height
        };

        /// <summary>
        /// Converts from <see cref="RectangleSerialized"/> to <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="rect"/>
        /// <returns/>
        public static implicit operator Rectangle(RectangleSerialized rect)
            => new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
    }
}

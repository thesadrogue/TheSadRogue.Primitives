using System;

namespace SadRogue.Primitives.SerializedTypes
{
    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="BisectionResult"/>.
    /// </summary>
    [Serializable]
    public struct BisectionResultSerialized
    {
        /// <summary>
        /// The first rectangle.
        /// </summary>
        public RectangleSerialized Rect1;

        /// <summary>
        /// The second rectangle.
        /// </summary>
        public RectangleSerialized Rect2;

        /// <summary>
        /// Converts from <see cref="BisectionResult"/> to <see cref="BisectionResultSerialized"/>.
        /// </summary>
        /// <param name="result"/>
        /// <returns/>
        public static implicit operator BisectionResultSerialized(BisectionResult result) => new BisectionResultSerialized()
        {
            Rect1 = result.Rect1,
            Rect2 = result.Rect2,
            
        };

        /// <summary>
        /// Converts from <see cref="BisectionResultSerialized"/> to <see cref="BisectionResult"/>.
        /// </summary>
        /// <param name="result"/>
        /// <returns/>
        public static implicit operator BisectionResult(BisectionResultSerialized result)
            => new BisectionResult(result.Rect1, result.Rect2);
    }

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

using System;
using JetBrains.Annotations;

namespace SadRogue.Primitives.SerializedTypes
{
    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="Color"/>.
    /// </summary>
    [PublicAPI]
    [Serializable]
    public struct ColorSerialized
    {
        /// <summary>
        /// "R" value for the color.
        /// </summary>
        public byte R;

        /// <summary>
        /// "G" value for the color.
        /// </summary>
        public byte G;

        /// <summary>
        /// "B" value for the color.
        /// </summary>
        public byte B;

        /// <summary>
        /// Alpha-value for the color.
        /// </summary>
        public byte A;

        /// <summary>
        /// Converts <see cref="Color"/> to <see cref="ColorSerialized"/>.
        /// </summary>
        /// <param name="color"/>
        /// <returns/>
        public static implicit operator ColorSerialized(Color color)
            => new ColorSerialized() { R = color.R, G = color.G, B = color.B, A = color.A };

        /// <summary>
        /// Converts <see cref="ColorSerialized"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="color"/>
        /// <returns/>
        public static implicit operator Color(ColorSerialized color) => new Color(color.R, color.G, color.B, color.A);
    }
}

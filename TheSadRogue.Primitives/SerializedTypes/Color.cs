using System;

namespace SadRogue.Primitives.SerializedTypes
{
    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="Color"/>.
    /// </summary>
    [Serializable]
    public struct ColorSerialized
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public static implicit operator ColorSerialized(Color color) => new ColorSerialized() { R = color.R, G = color.G, B = color.B, A = color.A };

        public static implicit operator Color(ColorSerialized color) => new Color(color.R, color.G, color.B, color.A);
    }
}

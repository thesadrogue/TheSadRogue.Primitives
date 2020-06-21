using System;
using System.Collections.Generic;
using System.Linq;

namespace SadRogue.Primitives.SerializedTypes
{
    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="Palette"/>.
    /// </summary>
    [Serializable]
    public struct PaletteSerialized
    {
        public List<ColorSerialized> Colors;

        public static implicit operator Palette(PaletteSerialized serialized) => new Palette(serialized.Colors.Cast<Color>());

        public static implicit operator PaletteSerialized(Palette palette) =>
            new PaletteSerialized() { Colors = palette.Cast<ColorSerialized>().ToList() };
    }


}

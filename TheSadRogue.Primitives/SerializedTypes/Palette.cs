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
        /// <summary>
        /// Colors in the palette.
        /// </summary>
        public List<ColorSerialized> Colors;

        /// <summary>
        /// Converts from <see cref="PaletteSerialized"/> to <see cref="Palette"/>.
        /// </summary>
        /// <param name="serialized"/>
        /// <returns/>
        public static implicit operator Palette(PaletteSerialized serialized) => new Palette(serialized.Colors.Select(colorSerialized => (Color)colorSerialized));

        /// <summary>
        /// Converts from <see cref="Palette"/> to <see cref="PaletteSerialized"/>.
        /// </summary>
        /// <param name="palette"/>
        /// <returns/>
        public static implicit operator PaletteSerialized(Palette palette) =>
            new PaletteSerialized() { Colors = palette.Select(color => (ColorSerialized)color).ToList() };
    }


}

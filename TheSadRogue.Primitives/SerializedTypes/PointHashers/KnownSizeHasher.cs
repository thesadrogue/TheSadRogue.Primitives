using System;
using SadRogue.Primitives.PointHashers;

namespace SadRogue.Primitives.SerializedTypes.PointHashers
{
    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="KnownSizeHasher"/>.
    /// </summary>
    [Serializable]
    public struct KnownSizeHasherSerialized
    {
        /// <summary>
        /// The maximum-x value given to the KnownSizeHasher.
        /// </summary>
        public int MaxXValue;

        /// <summary>
        /// Converts <see cref="KnownSizeHasherSerialized"/> to <see cref="KnownSizeHasher"/>.
        /// </summary>
        /// <param name="serialized"/>
        /// <returns/>
        public static implicit operator KnownSizeHasher(KnownSizeHasherSerialized serialized)
            => new KnownSizeHasher(serialized.MaxXValue);

        /// <summary>
        /// Converts <see cref="KnownSizeHasher"/> to <see cref="KnownSizeHasherSerialized"/>.
        /// </summary>
        /// <param name="sizeHasher"/>
        /// <returns/>
        public static implicit operator KnownSizeHasherSerialized(KnownSizeHasher sizeHasher)
            => new KnownSizeHasherSerialized { MaxXValue = sizeHasher.MaxXValue };
    }
}

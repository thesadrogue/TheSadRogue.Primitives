using System;
using SadRogue.Primitives.PointHashers;

namespace SadRogue.Primitives.SerializedTypes.PointHashers
{
    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="KnownRangeHasher"/>.
    /// </summary>
    [Serializable]
    public struct KnownRangeHasherSerialized
    {
        /// <summary>
        /// Minimum x/y values that will generally occur in points hashed by this instance.
        /// </summary>
        public PointSerialized MinExtent;

        /// <summary>
        /// Width of the bounds of points hashed by this instance.
        /// </summary>
        public int BoundsWidth;

        /// <summary>
        /// Converts <see cref="KnownRangeHasherSerialized"/> to <see cref="KnownRangeHasher"/>.
        /// </summary>
        /// <param name="serialized"/>
        /// <returns/>
        public static implicit operator KnownRangeHasher(KnownRangeHasherSerialized serialized)
            => new KnownRangeHasher(serialized.MinExtent, serialized.BoundsWidth);

        /// <summary>
        /// Converts <see cref="KnownRangeHasher"/> to <see cref="KnownRangeHasherSerialized"/>.
        /// </summary>
        /// <param name="rangeHasher"/>
        /// <returns/>
        public static implicit operator KnownRangeHasherSerialized(KnownRangeHasher rangeHasher)
            => new KnownRangeHasherSerialized
            {
                MinExtent = rangeHasher.MinExtent,
                BoundsWidth = rangeHasher.BoundsWidth
            };
    }
}

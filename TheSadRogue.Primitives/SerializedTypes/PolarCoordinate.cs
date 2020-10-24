using System;

namespace SadRogue.Primitives.SerializedTypes
{
    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="PolarCoordinate"/>.
    /// </summary>
    [Serializable]
    public struct PolarCoordinateSerialized
    {
        /// <summary>
        /// The distance away from the Origin (0,0) of this Polar Coord
        /// </summary>
        public double Radius;

        /// <summary>
        /// The angle of rotation, clockwise, in radians
        /// </summary>
        public double Theta;

        /// <summary>
        /// Converts from <see cref="PolarCoordinate"/> to <see cref="PolarCoordinateSerialized"/>.
        /// </summary>
        /// <param name="point"/>
        /// <returns/>
        public static implicit operator PolarCoordinateSerialized(PolarCoordinate point)
            => new PolarCoordinateSerialized { Radius = point.Radius, Theta = point.Theta };

        /// <summary>
        /// Converts from <see cref="PolarCoordinateSerialized"/> to <see cref="PolarCoordinate"/>.
        /// </summary>
        /// <param name="serialized"/>
        /// <returns/>
        public static implicit operator PolarCoordinate(PolarCoordinateSerialized serialized)
            => new PolarCoordinate(serialized.Radius, serialized.Theta);
    }
}

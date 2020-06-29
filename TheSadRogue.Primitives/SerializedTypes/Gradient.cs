using System;
using System.Collections.Generic;
using System.Linq;

namespace SadRogue.Primitives.SerializedTypes
{
    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="GradientStop"/>.
    /// </summary>
    [Serializable]
    public struct GradientStopSerialized
    {
        /// <summary>
        /// Color of the stop.
        /// </summary>
        public ColorSerialized Color;
        /// <summary>
        /// Location where the stop is used.
        /// </summary>
        public float Stop;

        /// <summary>
        /// Converts from <see cref="GradientStopSerialized"/> to <see cref="GradientStop"/>.
        /// </summary>
        /// <param name="serialized"/>
        /// <returns/>
        public static implicit operator GradientStop(GradientStopSerialized serialized) => new GradientStop(serialized.Color, serialized.Stop);

        /// <summary>
        /// Converts from <see cref="GradientStop"/> to <see cref="GradientStopSerialized"/>.
        /// </summary>
        /// <param name="stop"/>
        /// <returns/>
        public static implicit operator GradientStopSerialized(GradientStop stop) =>
            new GradientStopSerialized() {Color = stop.Color, Stop = stop.Stop};
    }

    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="Gradient"/>.
    /// </summary>
    [Serializable]
    public struct GradientSerialized
    {
        /// <summary>
        /// Colors/stop locations that describe the gradient.
        /// </summary>
        public List<GradientStopSerialized> Stops;

        /// <summary>
        /// Converts <see cref="GradientSerialized"/> to <see cref="Gradient"/>.
        /// </summary>
        /// <param name="serialized"/>
        /// <returns/>
        public static implicit operator Gradient(GradientSerialized serialized)
        {

            var colors = serialized.Stops.Select(stop => (Color)stop.Color).ToArray();
            var stops = serialized.Stops.Select(stop => stop.Stop);

            return new Gradient(colors, stops);
        }

        /// <summary>
        /// Converts <see cref="Gradient"/> to <see cref="GradientSerialized"/>.
        /// </summary>
        /// <param name="gradient"/>
        /// <returns/>
        public static implicit operator GradientSerialized(Gradient gradient)
            => new GradientSerialized()
            {
                Stops = gradient.Stops.Select(stop => (GradientStopSerialized)stop).ToList()
            };
    }
}

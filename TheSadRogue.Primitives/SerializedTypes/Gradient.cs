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
        public ColorSerialized Color;
        public float Stop;

        public static implicit operator GradientStop(GradientStopSerialized serialized) => new GradientStop(serialized.Color, serialized.Stop);

        public static implicit operator GradientStopSerialized(GradientStop stop) =>
            new GradientStopSerialized() {Color = stop.Color, Stop = stop.Stop};
    }

    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="Gradient"/>.
    /// </summary>
    [Serializable]
    public struct GradientSerialized
    {
        public List<GradientStopSerialized> Stops;

        public static implicit operator Gradient(GradientSerialized serialized)
        {

            var colors = serialized.Stops.Select(stop => (Color)stop.Color).ToArray();
            var stops = serialized.Stops.Select(stop => stop.Stop);

            return new Gradient(colors, stops);
        }

        public static implicit operator GradientSerialized(Gradient gradient)
            => new GradientSerialized()
            {
                Stops = gradient.Stops.Select(stop => (GradientStopSerialized)stop).ToList()
            };
    }
}

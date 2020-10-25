using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;

namespace SadRogue.Primitives
{
    /// <summary>
    /// A gradient stop. Defines a color and where it is located within the gradient.
    /// </summary>
    [DataContract]
    public readonly struct GradientStop : IEquatable<GradientStop>, IMatchable<GradientStop>
    {
        /// <summary>
        /// The color.
        /// </summary>
        [DataMember] public readonly Color Color;

        /// <summary>
        /// The color stop in the gradient this applies to.
        /// </summary>
        [DataMember] public readonly float Stop;

        /// <summary>
        /// Creates a new gradient stop.
        /// </summary>
        /// <param name="color">The color to use.</param>
        /// <param name="stop">The position of the stop.</param>
        public GradientStop(Color color, float stop)
        {
            Color = color;
            Stop = stop;
        }

        /// <summary>
        /// Compares two gradient stops based on their color and stop value.
        /// </summary>
        /// <param name="lhs"/>
        /// <param name="rhs"/>
        /// <returns>True if the gradients have the same color and stop; false otherwise.</returns>
        [Pure]
        public static bool operator ==(GradientStop lhs, GradientStop rhs)
            => lhs.Color == rhs.Color && Math.Abs(lhs.Stop - rhs.Stop) < 0.0000000001;

        /// <summary>
        /// Compares two gradient stops based on their color and stop value.
        /// </summary>
        /// <param name="lhs"/>
        /// <param name="rhs"/>
        /// <returns>True if the gradients have different color or stop values, false otherwise.</returns>
        [Pure]
        public static bool operator !=(GradientStop lhs, GradientStop rhs) => !(lhs == rhs);

        /// <summary>
        /// Gets a hash code based upon the stop's color and stop values.
        /// </summary>
        /// <returns>A hash code based upon the stop's color and stop values.</returns>
        [Pure]
        public override int GetHashCode() => Color.GetHashCode() ^ Stop.GetHashCode();

        /// <summary>
        /// Returns true if <paramref name="obj"/> is a gradient stop with the same color and stop value.
        /// </summary>
        /// <param name="obj"/>
        /// <returns>True if <paramref name="obj"/> is a gradient stop with the same color and stop value.</returns>
        [Pure]
        public override bool Equals(object? obj) => obj is GradientStop g && this == g;

        /// <summary>
        /// Compares this gradient stop to the one given.
        /// </summary>
        /// <param name="g"/>
        /// <returns>True if this gradient stop and the specified one have the same color and stop values; false otherwise.</returns>
        [Pure]
        public bool Equals(GradientStop g) => Color == g.Color && Math.Abs(Stop - g.Stop) < 0.0000000001;

        /// <summary>
        /// Compares this gradient stop to the one given.
        /// </summary>
        /// <param name="other"/>
        /// <returns>True if this gradient stop and the specified one have the same color and stop values; false otherwise.</returns>
        [Pure]
        public bool Matches(GradientStop other) => Equals(other);
    }

    /// <summary>
    /// Represents a gradient with multiple color stops.
    /// </summary>
    [DataContract]
    public class Gradient : IEnumerable<GradientStop>, IMatchable<Gradient>
    {
        /// <summary>
        /// The color stops that define the gradient.
        /// </summary>
        [DataMember] public readonly GradientStop[] Stops;

        /// <summary>
        /// Creates a new color gradient with the defined colors and stops.
        /// </summary>
        /// <param name="colors">The colors with the gradient.</param>
        /// <param name="stops">The gradient stops where the colors are used.</param>
        public Gradient(IEnumerable<Color> colors, IEnumerable<float> stops)
        {
            Color[] colorList = colors.ToArray();
            float[] stopList = stops.ToArray();


            if (colorList.Length != stopList.Length)
                throw new Exception("Both colors and stops much match in array length.");

            Stops = new GradientStop[colorList.Length];

            for (int i = 0; i < colorList.Length; i++)
                Stops[i] = new GradientStop(colorList[i], stopList[i]);
        }

        /// <summary>
        /// Creates a new color gradient with only two colors, the first at the start, and the second at the end.
        /// </summary>
        /// <param name="startingColor">The starting color of the gradient.</param>
        /// <param name="endingColor">The ending color of the gradient.</param>
        public Gradient(Color startingColor, Color endingColor)
            : this(new[] { startingColor, endingColor }, new[] { 0f, 1f })
        { }

        /// <summary>
        /// Creates a new color gradient, evenly spacing them out. At least one color must be provided.
        /// </summary>
        /// <param name="colors">The colors to create a gradient from.</param>
        public Gradient(params Color[] colors)
        {
            switch (colors.Length)
            {
                case 0:
                    throw new ArgumentException("At least one color must be provided on this constructor.");
                case 1:
                {
                    Stops = new GradientStop[2];
                    Stops[0] = new GradientStop(colors[0], 0f);
                    Stops[1] = new GradientStop(colors[0], 1f);
                    break;
                }
                default:
                {
                    Stops = new GradientStop[colors.Length];
                    float stopStrength = 1f / (colors.Length - 1);

                    for (int i = 0; i < colors.Length; i++)
                        Stops[i] = new GradientStop(colors[i], i * stopStrength);

                    break;
                }
            }
        }

        /// <summary>
        /// Creates a new color gradient with the given colors/stops.
        /// </summary>
        /// <param name="gradientStops">Stops to include in the gradient.</param>
        public Gradient(IEnumerable<GradientStop> gradientStops) => Stops = gradientStops.ToArray();

        /// <summary>
        /// Gets an enumerator with all of the gradient stops.
        /// </summary>
        /// <returns>An enumerator</returns>
        public IEnumerator<GradientStop> GetEnumerator() => ((IEnumerable<GradientStop>)Stops).GetEnumerator();

        /// <summary>
        /// Gets an enumerator with all of the gradient stops.
        /// </summary>
        /// <returns>An enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => Stops.GetEnumerator();

        /// <summary>
        /// Gets an array of colors based from the gradient.
        /// </summary>
        /// <param name="count">The amount of colors to produce.</param>
        /// <returns>An array of colors.</returns>
        public Color[] ToColorArray(int count)
        {
            Color[] returnArray = new Color[count];

            switch (Stops.Length)
            {
                case 0:
                    throw new IndexOutOfRangeException(
                        "The ColorGradient object does not have any gradient stops defined.");
                case 1:
                {
                    for (int i = 0; i < count; i++)
                        returnArray[i] = Stops[0].Color;

                    return returnArray;
                }
            }

            float lerp = 1f / (count - 1);
            float lerpTotal = 0f;

            returnArray[0] = Stops[0].Color;
            returnArray[count - 1] = Stops[^1].Color;

            for (int i = 1; i < count - 1; i++)
            {
                lerpTotal += lerp;
                int counter = 0;
                while (counter < Stops.Length && Stops[counter].Stop < lerpTotal)
                    counter++;

                counter--;
                counter = MathHelpers.Clamp(counter, 0, Stops.Length - 2);

                float newLerp = (Stops[counter].Stop - lerpTotal) / (Stops[counter].Stop - Stops[counter + 1].Stop);

                returnArray[i] = Color.Lerp(Stops[counter].Color, Stops[counter + 1].Color, newLerp);
            }

            return returnArray;
        }

        /// <summary>
        /// Returns a color from this gradient at the specified lerp value.
        /// </summary>
        /// <param name="amount">The lerp amount.</param>
        /// <returns>A color.</returns>
        public Color Lerp(float amount)
        {
            switch (Stops.Length)
            {
                case 0:
                    throw new IndexOutOfRangeException(
                        "The ColorGradient object does not have any gradient stops defined.");
                case 1:
                    return Stops[0].Color;
            }

            int counter = 0;
            while (counter < Stops.Length && Stops[counter].Stop < amount)
                counter++;

            counter--;
            counter = MathHelpers.Clamp(counter, 0, Stops.Length - 2);

            float newLerp = (Stops[counter].Stop - amount) / (Stops[counter].Stop - Stops[counter + 1].Stop);

            return Color.Lerp(Stops[counter].Color, Stops[counter + 1].Color, newLerp);
        }

        /// <summary>
        /// Converts a color to a gradient.
        /// </summary>
        /// <param name="color" />
        public static implicit operator Gradient(Color color) => new Gradient(color, color);

        /// <summary>
        /// Returns true if the given gradients contain precisely the same stops.
        /// </summary>
        /// <param name="other"/>
        /// <returns>True if the given gradients contain precisely the same stops; false otherwise.</returns>
        public bool Matches(Gradient? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (Stops.Length != other.Stops.Length)
                return false;

            for (int i = 0; i < Stops.Length; i++)
                if (Stops[i] != other.Stops[i])
                    return false;

            return true;
        }
    }
}

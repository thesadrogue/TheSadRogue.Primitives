using System;
using System.Collections;
using System.Collections.Generic;

namespace SadRogue.Primitives
{
    /// <summary>
    /// A gradient stop. Defines a color and where it is located within the gradient.
    /// </summary>
    [Serializable]
    public struct GradientStop : IEquatable<GradientStop>
    {
        /// <summary>
        /// The color.
        /// </summary>
        public readonly Color Color;

        /// <summary>
        /// The color stop in the gradiant this applies to.
        /// </summary>
        public readonly float Stop;

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

        public static bool operator ==(GradientStop lhs, GradientStop rhs) => lhs.Color == rhs.Color && lhs.Stop == rhs.Stop;
        public static bool operator !=(GradientStop lhs, GradientStop rhs) => !(lhs == rhs);

        public override int GetHashCode() => Color.GetHashCode() ^ Stop.GetHashCode();

        public override bool Equals(object obj) => obj is GradientStop g && this == g;

        public bool Equals(GradientStop g) => Color == g.Color && Stop == g.Stop;
    }

    /// <summary>
    /// Represents a gradient with multiple color stops.
    /// </summary>
    [Serializable]
    public class Gradient : IEnumerable<GradientStop>
    {
        /// <summary>
        /// The color stops that define the gradient.
        /// </summary>
        public GradientStop[] Stops { get; set; }

        /// <summary>
        /// Creates a new color gradient with the defined colors and stops.
        /// </summary>
        /// <param name="colors">The colors with the gradient.</param>
        /// <param name="stops">The gradient stops where the colors are used.</param>
        public Gradient(IEnumerable<Color> colors, IEnumerable<float> stops)
        {
            Color[] colorList = (Color[])colors;
            float[] stopList = (float[])stops;


            if (colorList.Length != stopList.Length)
            {
                throw new global::System.Exception("Both colors and stops much match in array length.");
            }

            Stops = new GradientStop[colorList.Length];

            for (int i = 0; i < colorList.Length; i++)
            {
                Stops[i] = new GradientStop(colorList[i], stopList[i]);
            }
        }

        /// <summary>
        /// Creates a new color gradient with only two colors, the first at the start, and the second at the end.
        /// </summary>
        /// <param name="startingColor">The starting color of the gradient.</param>
        /// <param name="endingColor">The ending color of the gradient.</param>
        public Gradient(Color startingColor, Color endingColor) : this(new Color[] { startingColor, endingColor }, new float[] { 0f, 1f })
        {

        }

        /// <summary>
        /// Creates a new color gradient, evenly spacing them out. At least one color must be provided.
        /// </summary>
        /// <param name="colors">The colors to create a gradient from.</param>
        public Gradient(params Color[] colors)
        {
            if (colors.Length == 0)
            {
                throw new global::System.ArgumentException("At least one color must be provided on this constructor.");
            }

            if (colors.Length == 1)
            {
                Color color = colors[0];
                colors = new Color[2];
                colors[0] = color;
                colors[1] = color;
            }

            if (colors.Length >= 2)
            {
                Stops = new GradientStop[colors.Length];
                float stopStrength = 1f / (colors.Length - 1);

                for (int i = 0; i < colors.Length; i++)
                {
                    Stops[i] = new GradientStop(colors[i], i * stopStrength);
                }
            }

        }

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

            if (Stops.Length == 0)
            {
                throw new global::System.IndexOutOfRangeException("The ColorGradient object does not have any gradient stops defined.");
            }
            else if (Stops.Length == 1)
            {
                for (int i = 0; i < count; i++)
                {
                    returnArray[i] = Stops[0].Color;
                }

                return returnArray;
            }

            float lerp = 1f / (count - 1);
            float lerpTotal = 0f;

            returnArray[0] = Stops[0].Color;
            returnArray[count - 1] = Stops[Stops.Length - 1].Color;

            for (int i = 1; i < count - 1; i++)
            {
                lerpTotal += lerp;
                int counter;
                for (counter = 0; counter < Stops.Length && Stops[counter].Stop < lerpTotal; counter++)
                {
                    ;
                }

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
            if (Stops.Length == 0)
            {
                throw new System.IndexOutOfRangeException("The ColorGradient object does not have any gradient stops defined.");
            }
            else if (Stops.Length == 1)
            {
                return Stops[0].Color;
            }

            int counter;
            for (counter = 0; counter < Stops.Length && Stops[counter].Stop < amount; counter++)
            {
                ;
            }

            counter--;
            counter = MathHelpers.Clamp(counter, 0, Stops.Length - 2);

            float newLerp = (Stops[counter].Stop - amount) / (Stops[counter].Stop - Stops[counter + 1].Stop);

            return Color.Lerp(Stops[counter].Color, Stops[counter + 1].Color, newLerp);
        }

        public static implicit operator Gradient(Color color) => new Gradient(color, color);

        public static implicit operator Color(Gradient gradient) => gradient.Stops[0].Color;
    }
}

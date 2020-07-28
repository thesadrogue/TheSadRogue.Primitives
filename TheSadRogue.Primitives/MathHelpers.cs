using System;
using System.Runtime.CompilerServices;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Static class consisting of mathematical "helper" functions and constants that can be useful
    /// for performing operations on a 2D grid.
    /// </summary>
    public static class MathHelpers
    {
        /// <summary>
        /// Result of 1/360; represents in decimal form a percent of a circle that a degree constitutes.
        /// </summary>
        public const double DegreePctOfCircle = 0.002777777777777778;

        /// <summary>
        /// Pi / 180. Used to speed up calculations related to circles.
        /// </summary>
        public const double DegreesToRadiansConstant = Math.PI / 180;

        /// <summary>
        /// 180 / Pi. Used to speed up calculations related to circles.
        /// </summary>
        public const double RadiansToDegreesConstant = 180 / Math.PI;

        /// <summary>
        /// Converts given angle from radians to degrees.
        /// </summary>
        /// <param name="radAngle">Angle in radians.</param>
        /// <returns>The given angle in degrees.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDegree(double radAngle) => radAngle * RadiansToDegreesConstant;

        /// <summary>
        /// Converts given angle from degrees to radians.
        /// </summary>
        /// <param name="degAngle">Angle in degrees.</param>
        /// <returns>The given angle in radians.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToRadian(double degAngle) => degAngle * DegreesToRadiansConstant;

        /// <summary>
        /// Restricts a value to a specified range.
        /// </summary>
        /// <param name="value">The value to restrict.</param>
        /// <param name="min">The minimum to clamp the value to.</param>
        /// <param name="max">The maximum to clamp the value to.</param>
        /// <returns>The clamped value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                value = min;

            if (value > max)
                value = max;

            return value;
        }

        /// <summary>
        /// Restricts a value to a specified range.
        /// </summary>
        /// <param name="value">The value to restrict.</param>
        /// <param name="min">The minimum to clamp the value to.</param>
        /// <param name="max">The maximum to clamp the value to.</param>
        /// <returns>The clamped value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                value = min;

            if (value > max)
                value = max;

            return value;
        }

        /// <summary>
        /// Restricts a value to a specified range.
        /// </summary>
        /// <param name="value">The value to restrict.</param>
        /// <param name="min">The minimum to clamp the value to.</param>
        /// <param name="max">The maximum to clamp the value to.</param>
        /// <returns>The clamped value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
                value = min;

            if (value > max)
                value = max;

            return value;
        }

        /// <summary>
        /// Performs linear interpolation between two values.
        /// </summary>
        /// <param name="value1">Starting value.</param>
        /// <param name="value2">Ending value.</param>
        /// <param name="amount">The weight to apply to <paramref name="value2"/>.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float value1, float value2, float amount) =>
            value1 + (value2 - value1) * amount;

        /// <summary>
        /// Performs linear interpolation between two values.
        /// </summary>
        /// <param name="value1">Starting value.</param>
        /// <param name="value2">Ending value.</param>
        /// <param name="amount">The weight to apply to <paramref name="value2"/>.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Lerp(double value1, double value2, double amount) =>
            value1 + (value2 - value1) * amount;

        /// <summary>
        /// Wraps a value around the min and max.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="min">The minimum value before it transforms into the maximum.</param>
        /// <param name="max">The maximum value before it transforms into the minimum.</param>
        /// <returns>A new value if it falls outside the min/max range otherwise, the same value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Wrap(float value, float min, float max)
        {
            if (value < min)
                value = max - (min - value) % (max - min);
            else
                value = min + (value - min) % (max - min);

            return value;
        }
    }
}

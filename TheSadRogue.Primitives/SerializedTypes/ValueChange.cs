using System;
using SadRogue.Primitives.GridViews;

namespace SadRogue.Primitives.SerializedTypes
{
    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="ValueChange{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of value being changed.</typeparam>
    [Serializable]
    public struct ValueChangeSerialized<T> where T : struct
    {
        /// <summary>
        /// Position whose value was changed.
        /// </summary>
        public PointSerialized Position;

        /// <summary>
        /// Original value that was changed.
        /// </summary>
        public T OldValue;

        /// <summary>
        /// New value that was set.
        /// </summary>
        public T NewValue;

        /// <summary>
        /// Converts <see cref="ValueChange{T}"/> to <see cref="ValueChangeSerialized{T}"/>.
        /// </summary>
        /// <param name="valueChange"/>
        /// <returns/>
        public static implicit operator ValueChangeSerialized<T>(ValueChange<T> valueChange)
            => new ValueChangeSerialized<T>
            {
                Position = valueChange.Position,
                OldValue = valueChange.OldValue,
                NewValue = valueChange.NewValue
            };

        /// <summary>
        /// Converts <see cref="ValueChangeSerialized{T}"/> to <see cref="ValueChange{T}"/>.
        /// </summary>
        /// <param name="valueChange"/>
        /// <returns/>
        public static implicit operator ValueChange<T>(ValueChangeSerialized<T> valueChange)
            => new ValueChange<T>(valueChange.Position, valueChange.OldValue, valueChange.NewValue);
    }
}

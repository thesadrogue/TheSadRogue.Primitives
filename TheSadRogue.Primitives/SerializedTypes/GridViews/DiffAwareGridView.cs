using System;
using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives.GridViews;

namespace SadRogue.Primitives.SerializedTypes.GridViews
{
    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="Diff{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of value being changed.</typeparam>
    [Serializable]
    public struct DiffSerialized<T> where T : struct
    {
        /// <summary>
        /// Changes recorded in this diff.
        /// </summary>
        public List<ValueChangeSerialized<T>> Changes;

        /// <summary>
        /// Converts <see cref="Diff{T}"/> to <see cref="DiffSerialized{T}"/>.
        /// </summary>
        /// <param name="diff"/>
        /// <returns/>
        public static implicit operator DiffSerialized<T>(Diff<T> diff)
            => new DiffSerialized<T>
            {
                Changes = new List<ValueChangeSerialized<T>>(
                    diff.Changes.Select(change => (ValueChangeSerialized<T>)change)),
            };

        /// <summary>
        /// Converts <see cref="DiffSerialized{T}"/> to <see cref="Diff{T}"/>.
        /// </summary>
        /// <param name="diff"/>
        /// <returns/>
        public static implicit operator Diff<T>(DiffSerialized<T> diff)
            => new Diff<T>(diff.Changes.Select(change => (ValueChange<T>)change));
    }

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

    /// <summary>
    /// Serializable (pure-data) object representing a <see cref="DiffAwareGridView{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of value being changed.</typeparam>
    [Serializable]
    public struct DiffAwareGridViewSerialized<T> where T : struct
    {
        /// <summary>
        /// The grid view whose changes are being recorded in diffs.
        /// </summary>
        public ISettableGridView<T> BaseGrid;

        /// <summary>
        /// The index of the diff whose ending state is currently reflected in <see cref="BaseGrid"/>.
        /// </summary>
        public int CurrentDiffIndex;

        /// <summary>
        /// All diffs recorded for the current grid view, and their changes.
        /// </summary>
        public List<DiffSerialized<T>> Diffs;

        /// <summary>
        /// Whether or not to automatically compress diffs when the currently applied diff is changed.
        /// </summary>
        public bool AutoCompress;

        /// <summary>
        /// Converts <see cref="DiffAwareGridView{T}"/> to <see cref="DiffAwareGridViewSerialized{T}"/>.
        /// </summary>
        /// <param name="view"/>
        /// <returns/>
        public static implicit operator DiffAwareGridViewSerialized<T>(DiffAwareGridView<T> view)
            => new DiffAwareGridViewSerialized<T>
            {
                // Cast is safe because the view was created as this in its implementation
                BaseGrid = (ISettableGridView<T>)view.BaseGrid,
                CurrentDiffIndex = view.CurrentDiffIndex,
                Diffs = new List<DiffSerialized<T>>(view.Diffs.Select(diff => (DiffSerialized<T>)diff)),
                AutoCompress = view.AutoCompress
            };

        /// <summary>
        /// Converts <see cref="DiffAwareGridViewSerialized{T}"/> to <see cref="DiffAwareGridView{T}"/>.
        /// </summary>
        /// <param name="view"/>
        /// <returns/>
        public static implicit operator DiffAwareGridView<T>(DiffAwareGridViewSerialized<T> view)
        {
            var diffView = new DiffAwareGridView<T>(view.BaseGrid, view.AutoCompress);
            diffView.SetHistory(view.Diffs.Select(diff => (Diff<T>)diff));

            return diffView;
        }
    }
}

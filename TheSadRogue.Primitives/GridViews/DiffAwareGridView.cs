using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SadRogue.Primitives.GridViews
{
    /// <summary>
    /// Records a value change in a diff as recorded by a <see cref="DiffAwareGridView{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of value being changed.</typeparam>
    [DataContract]
    public readonly struct ValueChange<T> : IEquatable<ValueChange<T>>, IMatchable<ValueChange<T>>
        where T : struct
    {
        /// <summary>
        /// Position whose value was changed.
        /// </summary>
        [DataMember] public readonly Point Position;

        /// <summary>
        /// Original value that was changed.
        /// </summary>
        [DataMember] public readonly T OldValue;

        /// <summary>
        /// New value that was set.
        /// </summary>
        [DataMember] public readonly T NewValue;

        /// <summary>
        /// Creates a new value change record.
        /// </summary>
        /// <param name="position">Position whose value was changed.</param>
        /// <param name="oldValue">Original value that was changed.</param>
        /// <param name="newValue">New value that was set.</param>
        public ValueChange(Point position, T oldValue, T newValue)
        {
            Position = position;
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <inheritdoc />
        public bool Equals(ValueChange<T> other)
            => Position.Equals(other.Position) && OldValue.Equals(other.OldValue) && NewValue.Equals(other.NewValue);

        /// <summary>
        /// Compares the two changes according to their positions and values.
        /// </summary>
        /// <param name="other"/>
        /// <returns>True if the two value changes represent the same change, false otherwise.</returns>
        public bool Matches(ValueChange<T> other) => Equals(other);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is ValueChange<T> other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Position, OldValue, NewValue);

        /// <summary>
        /// Tests the two changes by their fields for equality.
        /// </summary>
        /// <param name="left"/>
        /// <param name="right"/>
        /// <returns>True if all the changes are equivalent; false otherwise.</returns>
        public static bool operator ==(ValueChange<T> left, ValueChange<T> right) => left.Equals(right);

        /// <summary>
        /// Tests the two changes by their fields for inequality.
        /// </summary>
        /// <param name="left"/>
        /// <param name="right"/>
        /// <returns>True if all the changes are equivalent; false otherwise.</returns>
        public static bool operator !=(ValueChange<T> left, ValueChange<T> right) => !left.Equals(right);

        /// <inheritdoc />
        public override string ToString() => $"{Position}: {OldValue} -> {NewValue}";
    }

    /// <summary>
    /// Represents a unique patch/diff of the state of a <see cref="DiffAwareGridView{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of value stored in the grid view.</typeparam>
    public class Diff<T> : IEnumerable<ValueChange<T>>
        where T : struct
    {
        private List<ValueChange<T>> _changes;
        /// <summary>
        /// Read-only list of changes made in this time step.
        /// </summary>
        public IReadOnlyList<ValueChange<T>> Changes => _changes.AsReadOnly();

        /// <summary>
        /// Whether or not the list of changes in this diff has been finalized, eg allows more changes to be added.
        /// </summary>
        public bool IsFinalized { get; private set; }

        /// <summary>
        /// Creates a new empty diff.
        /// </summary>
        public Diff()
        {
            _changes = new List<ValueChange<T>>();
            IsCompressed = true;
            IsFinalized = false;
        }

        /// <summary>
        /// Creates a diff composed of the specified changes.
        /// </summary>
        /// <param name="changes">Changes to create a diff from.</param>
        public Diff(IEnumerable<ValueChange<T>> changes)
        {
            _changes = new List<ValueChange<T>>(changes);
            IsCompressed = CheckCompressed();
            IsFinalized = false;
        }

        /// <summary>
        /// Whether or not the diff is currently known to be at the minimal possible size.
        /// </summary>
        public bool IsCompressed { get; private set; }

        /// <summary>
        /// Adds a change to the diff.
        /// </summary>
        /// <param name="change">Change to add.</param>
        public void Add(ValueChange<T> change)
        {
            if (IsFinalized)
                throw new Exception("Cannot add change to a finalized diff.");

            if (change.OldValue.Equals(change.NewValue))
                throw new Exception("Cannot add change to diff that does not change a value.");

            _changes.Add(change);

            // Change means its worth potentially minimizing, as this change might duplicate previous ones
            IsCompressed = false;
        }

        /// <summary>
        /// Finalizes the current diff, such that no changes are allowed to be added to it.  It can still be compressed.
        /// </summary>
        public void FinalizeChanges()
        {
            IsFinalized = true;
        }

        /// <summary>
        /// Reduces the diff to the minimum possible changes to achieve the resulting values by removing duplicate
        /// positions from the change list.
        /// </summary>
        public void Compress()
        {
            // No work to do
            if (IsCompressed)
                return;

            // Position of previous change we saw
            Point currentPositionGroup = Point.None;
            // NewValue of previous change we saw
            T prevNewValue = default;
            // Oldest OldValue we have encountered in the current position group.
            T firstOldValue = default;
            // New list of (minimal) changes
            var newChanges = new List<ValueChange<T>>();

            // Iterates in order sorted by position group, then by position in old list
            bool isFirstPositionGroup = true;
            foreach (var (_, change) in Changes
                                                .Select((change, index) => (index, change))
                                                .OrderBy(tuple => tuple.change.Position.X)
                                                .ThenBy(tuple => tuple.change.Position.Y)
                                                .ThenBy(tuple => tuple.index))
            {
                // Either we've changed position groups or this is the first group
                if (currentPositionGroup != change.Position)
                {
                    // In all cases but the initial group, we should add a change representing the starting value from
                    // the last time step and the newest value in this one (if they are different)
                    if (!isFirstPositionGroup)
                    {
                        // If the two are equal, the net result of the step is no change; otherwise add a change
                        if (!firstOldValue.Equals(prevNewValue))
                            newChanges.Add(new ValueChange<T>(currentPositionGroup, firstOldValue, prevNewValue));
                    }

                    // In any case, update the cached old value and position for start of a group
                    firstOldValue = change.OldValue;
                    currentPositionGroup = change.Position;
                    isFirstPositionGroup = false;
                }

                // Update cached new value for next iteration
                prevNewValue = change.NewValue;
            }

            // We still need to add a change for the last item in the set, since there was never a position group
            // switch for it
            if (!firstOldValue.Equals(prevNewValue))
                newChanges.Add(new ValueChange<T>(currentPositionGroup, firstOldValue, prevNewValue));

            // Switch out the change lists and mark the diff as fully compressed
            _changes = newChanges;
            IsCompressed = true;
        }

        private bool CheckCompressed()
        {
            var positions = _changes.Select(change => change.Position).ToHashSet();
            return positions.Count == _changes.Count;
        }

        /// <inheritdoc />
        public IEnumerator<ValueChange<T>> GetEnumerator() => _changes.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// A grid view wrapper useful for recording diffs (change-sets) of changes to a grid view, and applying/removing
    /// those change-sets of values from the grid view.  Only works with grid views of value types.
    /// </summary>
    /// <remarks>
    /// Generally, this class is useful with values types/primitive types wherein values are completely replaced when
    /// they are modified.  It allows applying a series of change sets in forward or reverse order; and as such
    /// can be extremely useful for debugging or situations where you want to record intermediate states of an
    /// algorithm.
    /// </remarks>
    /// <typeparam name="T">Type of value in the grid view.  Must be a value type.</typeparam>
    public class DiffAwareGridView<T> : SettableGridViewBase<T>
        where T : struct
    {
        private readonly ISettableGridView<T> _baseGrid;

        /// <summary>
        /// The grid view whose changes are being recorded in diffs.
        /// </summary>
        public IGridView<T> BaseGrid => _baseGrid;

        /// <inheritdoc />
        public override int Height => BaseGrid.Height;

        /// <inheritdoc />
        public override int Width => BaseGrid.Width;

        /// <inheritdoc />
        public override T this[Point pos]
        {
            get => _baseGrid[pos];
            set
            {
                T oldValue = _baseGrid[pos];

                // No change necessary
                if (oldValue.Equals(value))
                    return;

                // We can't make changes when there's previously recorded states to apply
                if (CurrentDiffIndex < _diffs.Count - 1)
                    throw new InvalidOperationException(
                        $"Cannot set values to a {nameof(DiffAwareGridView<T>)} when there are existing diffs " +
                        "that are not applied.");

                // If there are no diffs or the current diff is finalized, add a new one to record the current change.
                // No need to compress as it should either be finalized during application or finalization
                if (CurrentDiffIndex == -1 || _diffs[CurrentDiffIndex].IsFinalized)
                {
                    _diffs.Add(new Diff<T>());
                    CurrentDiffIndex++;
                }

                // Apply change to base grid view and add to current diff
                _baseGrid[pos] = value;
                _diffs[^1].Add(new ValueChange<T>(pos, oldValue, value));
            }
        }

        /// <summary>
        /// The index of the diff whose ending state is currently reflected in <see cref="BaseGrid"/>. Returns -1
        /// if none of the diffs in the list have been applied (eg. the grid view is in the state it was in at the
        /// <see cref="DiffAwareGridView{T}"/>'s creation.
        /// </summary>
        public int CurrentDiffIndex { get; private set; }

        private List<Diff<T>> _diffs;
        /// <summary>
        /// All diffs recorded for the current grid view, and their changes.
        /// </summary>
        public IReadOnlyList<Diff<T>> Diffs => _diffs.AsReadOnly();

        /// <summary>
        /// Whether or not to automatically compress diffs when the currently applied diff is changed.
        /// </summary>
        public bool AutoCompress;

        /// <summary>
        /// Constructs a diff-aware grid view that wraps around an existing grid view.
        /// </summary>
        /// <param name="baseGrid">The grid view whose changes are to be recorded in diffs.</param>
        /// <param name="autoCompress">
        /// Whether or not to automatically compress diffs when the currently applied diff is changed.
        /// </param>
        public DiffAwareGridView(ISettableGridView<T> baseGrid, bool autoCompress = true)
        {
            _baseGrid = baseGrid;
            CurrentDiffIndex = -1;
            AutoCompress = autoCompress;
            _diffs = new List<Diff<T>>();
        }

        /// <summary>
        /// Constructs a diff-aware grid view, whose base grid will be a new <see cref="ArrayView{T}"/>.
        /// </summary>
        /// <param name="width">Width of the base grid view that will be created.</param>
        /// <param name="height">Height of the base grid view that will be created.</param>
        /// <param name="autoCompress">
        /// Whether or not to automatically compress diffs when the currently applied diff is changed.
        /// </param>
        public DiffAwareGridView(int width, int height, bool autoCompress = true)
            : this(new ArrayView<T>(width, height), autoCompress)
        { }

        /// <summary>
        /// Sets the baseline values (eg. values before any diffs are recorded) to the values from the given grid view.
        /// Only valid to do before any diffs are recorded.
        /// </summary>
        /// <param name="baseline">Baseline values to use.  Must have same width/height as <see cref="BaseGrid"/>.</param>
        public void SetBaseline(IGridView<T> baseline)
        {
            if (baseline.Width != BaseGrid.Width || baseline.Height != BaseGrid.Height)
                throw new ArgumentException(
                    $"Baseline grid view's width/height must be same as {nameof(BaseGrid)}.",
                    nameof(baseline));

            if (_diffs.Count != 0)
                throw new InvalidOperationException("Baseline values must be set before any diffs are recorded.");

            _baseGrid.ApplyOverlay(baseline);
        }

        /// <summary>
        /// Applies the next recorded diff, or throws exception if there is no future diffs recorded.
        /// </summary>
        public void ApplyNextDiff()
        {
            // Can't apply a diff if there is no next diff
            if (CurrentDiffIndex >= _diffs.Count - 1)
                throw new InvalidOperationException($"Cannot {nameof(ApplyNextDiff)} when the view is already " +
                                                    "synchronized with the most recent recorded diff.");

            // Compress the diff we're about to switch off if it needs it and auto-compression is on
            if (AutoCompress && CurrentDiffIndex != -1)
                _diffs[CurrentDiffIndex].Compress();

            // Modify state to reflect diff we're applying
            CurrentDiffIndex++;

            // Compress the diff we're about to apply if it needs it and auto-compression is on
            if (AutoCompress)
                _diffs[CurrentDiffIndex].Compress();

            // Apply diff's changes
            foreach (var change in _diffs[CurrentDiffIndex].Changes)
                _baseGrid[change.Position] = change.NewValue;
        }

        /// <summary>
        /// Reverts the current diff's changes, so that the grid view will be in the state it was in at the end
        /// of the previous diff.  Throws exception if no diffs are applied.
        /// </summary>
        public void RevertToPreviousDiff()
        {
            if (CurrentDiffIndex == -1 || _diffs.Count == 0)
                throw new InvalidOperationException(
                    $"Cannot {nameof(RevertToPreviousDiff)} when there are no applied diffs.");


            // Compress the diff we're about to switch off if it needs it and auto-compression is on
            if (AutoCompress)
                _diffs[CurrentDiffIndex].Compress();

            // Revert current diff's changes
            foreach (var change in _diffs[CurrentDiffIndex].Changes)
                _baseGrid[change.Position] = change.OldValue;

            // If the diff we are switching off of is empty and we don't allow blank diffs, get rid of it
            if (_diffs[CurrentDiffIndex].Changes.Count == 0)
                _diffs.RemoveAt(CurrentDiffIndex);

            // Modify state to reflect diff we're applying.  Exit if we're at beginning state, eg. there are no longer
            // any diffs applied
            CurrentDiffIndex--;
            if (CurrentDiffIndex == -1)
                return;

            // If there is a previous diff, compress it if it needs it and auto-compression is on
            if (AutoCompress)
                _diffs[CurrentDiffIndex].Compress();
        }

        /// <summary>
        /// Finalizes the current diff so that no more changes can be added to it; future changes will create a new
        /// diff.  Throws exceptions if there are diffs that are not currently applied.
        /// </summary>
        public void FinalizeCurrentDiff()
        {
            if (CurrentDiffIndex < _diffs.Count - 1)
                throw new InvalidOperationException(
                    $"Cannot {nameof(FinalizeCurrentDiff)} if there are existing diffs that are not applied.");

            // Nothing to do here; we're already in the appropriate state
            if (Diffs.Count == 0)
                return;

            // Compress diff if needed
            if (AutoCompress)
                _diffs[^1].Compress();

            // If the diff we're finalizing has no changes, and we don't allow blank diffs, just remove it
            if (_diffs[^1].Changes.Count == 0)
            {
                _diffs.RemoveAt(_diffs.Count - 1);
                CurrentDiffIndex--;
            }
            // We don't add to the index since there's no changes in the new diff so the current one still represents
            // the current state of the grid view.  Instead, we just finalize current diff so that a new one will
            // be created if changes happen in the future.
            else
                _diffs[^1].FinalizeChanges();
        }

        /// <summary>
        /// Convenience method that calls <see cref="ApplyNextDiff"/> if there are existing diffs to apply, and
        /// <see cref="FinalizeCurrentDiff"/> if there are no existing diffs to apply.  Returns whether or not an
        /// existing diff was applied.
        /// </summary>
        /// <returns>True if an existing diff is applied, false if a new one was created.</returns>
        public bool ApplyNextDiffOrFinalize()
        {
            if (CurrentDiffIndex >= _diffs.Count - 1)
            {
                FinalizeCurrentDiff();
                return false;
            }

            ApplyNextDiff();
            return true;
        }

        /// <summary>
        /// Erase recorded diffs without modifying state of grid view.
        /// </summary>
        public void ClearHistory()
        {
            // Set the input as the history and sync index
            _diffs.Clear();
            CurrentDiffIndex = -1;
        }

        /// <summary>
        /// Overwrite any history present and replace it with the history given.  This does not modify the underlying
        /// grid view; the history must be valid with respect to its current state.
        /// </summary>
        /// <remarks>
        /// Generally, you will not call this function, however it can be useful for serialization and copying
        /// histories between objects.  Assumes the grid view is in a state that reflects all of the diffs in the
        /// history being applied.
        /// </remarks>
        /// <param name="history">The history to apply.</param>
        public void SetHistory(IEnumerable<Diff<T>> history)
        {
            var historyList = history.ToList();
            SetHistoryList(historyList, historyList.Count - 1);
        }

        /// <summary>
        /// Overwrite any history present and replace it with the history given.  This does not modify the underlying
        /// grid view; the history must be valid with respect to its current state.
        /// </summary>
        /// <remarks>
        /// Generally, you will not call this function, however it can be useful for serialization and copying
        /// histories between objects.
        /// </remarks>
        /// <param name="history">The history to apply.</param>
        /// <param name="currentIndex">The index of the given history that is applied to the <see cref="BaseGrid"/>.
        /// Set to the length of the list - 1 if all diffs have been applied, or -1 if none of them have.</param>
        public void SetHistory(IEnumerable<Diff<T>> history, int currentIndex)
        {
            var historyList = history.ToList();
            SetHistoryList(historyList, currentIndex);
        }

        private void SetHistoryList(List<Diff<T>> historyList, int currentIndex)
        {
            // Validate history state
            var gridValues = new Dictionary<Point, T>();
            if (historyList.Count == 0)
                throw new ArgumentException($"Cannot use {nameof(SetHistory)} to clear history.", nameof(historyList));
            if (currentIndex <= -1 || currentIndex > historyList.Count - 1)
                throw new ArgumentException("Index is not within valid range for history specified", nameof(currentIndex));

            // Validate history itself ahead of the current position
            for (int i = currentIndex + 1; i < historyList.Count; i++)
            {
                if (historyList[i].Changes.Count == 0)
                    throw new Exception($"Cannot have blank diffs in history of a {nameof(DiffAwareGridView<T>)}.");

                foreach (var change in historyList[i])
                {
                    if (!gridValues.ContainsKey(change.Position))
                    {
                        if (!BaseGrid[change.Position].Equals(change.OldValue))
                            throw new ArgumentException("History is not valid given state of grid view.", nameof(historyList));
                    }
                    else
                    {
                        if (!gridValues[change.Position].Equals(change.OldValue))
                            throw new ArgumentException("History is not valid given state of grid view.", nameof(historyList));
                    }

                    gridValues[change.Position] = change.NewValue;
                }
            }

            // Validate history itself behind the current position
            gridValues.Clear();
            for(int i = currentIndex; i >= 0; i--)
            {
                if (historyList[i].Changes.Count == 0)
                    throw new Exception($"Cannot have blank diffs in history of a {nameof(DiffAwareGridView<T>)}.");

                foreach (var change in historyList[i].Reverse())
                {
                    if (!gridValues.ContainsKey(change.Position))
                    {
                        if (!BaseGrid[change.Position].Equals(change.NewValue))
                            throw new ArgumentException("History is not valid given state of grid view.", nameof(historyList));
                    }
                    else
                    {
                        if (!gridValues[change.Position].Equals(change.NewValue))
                            throw new ArgumentException("History is not valid given state of grid view.", nameof(historyList));
                    }

                    gridValues[change.Position] = change.OldValue;
                }
            }

            // Set the input as the history and sync index
            _diffs = historyList;
            CurrentDiffIndex = currentIndex;
        }
    }
}

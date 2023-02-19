using System;
using System.Collections;
using System.Collections.Generic;

namespace SadRogue.Primitives.SpatialMaps
{
    /// <summary>
    /// A version of <see cref="AdvancedSpatialMap{T}"/> which takes items that implement <see cref="IPositionable"/>,
    /// and uses that interface's properties/events to automatically ensure items are recorded at the proper positions
    /// in the spatial map when they move and that the position fields are updated if the spatial map's move functions
    /// are used.
    /// </summary>
    /// <remarks>
    /// This class automatically keeps the spatial map position of each object synced up with their
    /// <see cref="IPositionable.Position"/> property; you may either use the Move functions of the spatial map,
    /// in which case the Position fields of the objects are updated as appropriate, or you may change the Position
    /// field, in which case the spatial map position is updated to match.
    ///
    /// If you want to manually control the positions of items in the spatial map, you should use
    /// <see cref="AdvancedSpatialMap{T}"/> instead.
    /// </remarks>
    /// <typeparam name="T">The type of object that will be contained by this spatial map.</typeparam>
    public class AutoSyncAdvancedSpatialMap<T> : ISpatialMap<T>
        where T : class, IPositionable
    {
        private readonly AdvancedSpatialMap<T> _spatialMap;

        /// <inheritdoc/>
        public int Count => _spatialMap.Count;

        /// <inheritdoc/>
        public IEnumerable<T> Items => _spatialMap.Items;

        /// <inheritdoc/>
        public IEnumerable<Point> Positions => _spatialMap.Positions;

        /// <inheritdoc/>
        public event EventHandler<ItemEventArgs<T>>? ItemAdded
        {
            add => _spatialMap.ItemAdded += value;
            remove => _spatialMap.ItemAdded -= value;
        }

        /// <inheritdoc/>
        public event EventHandler<ItemMovedEventArgs<T>>? ItemMoved
        {
            add => _spatialMap.ItemMoved += value;
            remove => _spatialMap.ItemMoved -= value;
        }

        /// <inheritdoc/>
        public event EventHandler<ItemEventArgs<T>>? ItemRemoved
        {
            add => _spatialMap.ItemRemoved += value;
            remove => _spatialMap.ItemRemoved -= value;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="itemComparer">
        /// Equality comparer to use for comparison and hashing of type T. Be especially mindful of the
        /// efficiency of its GetHashCode function, as it will determine the efficiency of many spatial map functions.
        /// functions.
        /// </param>
        /// <param name="pointComparer">
        /// Equality comparer to use for comparison and hashing of points, as object are added to/removed from/moved
        /// around the spatial map.  Be especially mindful of the efficiency of its GetHashCode function, as it will
        /// determine the efficiency of many spatial map functions.  Defaults to the default equality comparer for
        /// Point, which uses a fairly efficient generalized hashing algorithm.
        /// </param>
        /// <param name="initialCapacity">
        /// The initial maximum number of elements the spatial map can hold before it has to
        /// internally resize data structures. Defaults to 32.
        /// </param>
        public AutoSyncAdvancedSpatialMap(IEqualityComparer<T> itemComparer, IEqualityComparer<Point>? pointComparer = null,
                                  int initialCapacity = 32)
        {
            _spatialMap = new AdvancedSpatialMap<T>(itemComparer, pointComparer, initialCapacity);

            _spatialMap.ItemAdded += OnItemAdded;
            _spatialMap.ItemRemoved += OnItemRemoved;
        }

        #region Enumeration
        /// <summary>
        /// Used by foreach loop, so that the class will give ISpatialTuple objects when used in a
        /// foreach loop. Generally should never be called explicitly.
        /// </summary>
        /// <returns>An enumerator for the spatial map</returns>
        public IEnumerator<ItemPositionPair<T>> GetEnumerator() => _spatialMap.GetEnumerator();

        /// <summary>
        /// Generic iterator used internally by foreach loops.
        /// </summary>
        /// <returns>Enumerator to ISpatialTuple instances.</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_spatialMap).GetEnumerator();
        #endregion

        /// <inheritdoc />
        public IReadOnlySpatialMap<T> AsReadOnly() => _spatialMap.AsReadOnly();

        #region Add
        /// <summary>
        /// Returns true if the given item can be added at the given position, eg. the item is not already in the
        /// spatial map and the position is not already filled; false otherwise.
        /// </summary>
        /// <param name="newItem">Item to add.</param>
        /// <param name="position">Position to add item to.</param>
        /// <returns>True if the item can be successfully added at the position given; false otherwise.</returns>
        public bool CanAdd(T newItem, Point position) => _spatialMap.CanAdd(newItem, position);

        /// <summary>
        /// Returns true if the given item can be added at the given position, eg. if the item is not already in the spatial map;
        /// false otherwise.
        /// </summary>
        /// <param name="newItem">Item to add.</param>
        /// <param name="x">X-value of the position to add item to.</param>
        /// <param name="y">Y-value of the position to add item to.</param>
        /// <returns>True if the item can be successfully added at the position given; false otherwise.</returns>
        public bool CanAdd(T newItem, int x, int y) => _spatialMap.CanAdd(newItem, x, y);

        /// <summary>
        /// Adds the given item at its position, provided the item is not already in the
        /// spatial map and the position is not already filled. If either of those are the case,
        /// throws ArgumentException.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void Add(T item) => _spatialMap.Add(item, item.Position);

        /// <summary>
        /// Changes the position field of the given item to the given value, and then adds it to the spatial map,
        /// provided the item is not already in the spatial map and the position is not already filled. If either of
        /// those are the case, throws ArgumentException.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="position">Position to add item to.</param>
        public void Add(T item, Point position)
        {
            item.Position = position;
            _spatialMap.Add(item, position);
        }

        /// <summary>
        /// Changes the position field of the given item to the given value, and then adds it to the spatial map,
        /// provided the item is not already in the spatial map and the position is not already filled. If either of
        /// those are the case, throws ArgumentException.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="x">X-value of the position to add item to.</param>
        /// <param name="y">Y-value of the position to add item to.</param>
        public void Add(T item, int x, int y) => Add(item, new Point(x, y));

        /// <summary>
        /// Tries to add the given item at its position, provided the item is not already in the
        /// spatial map and the position is not already filled. If either of those are the case,
        /// returns false.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <returns>True if the item was successfully added; false otherwise.</returns>
        public bool TryAdd(T item) => _spatialMap.TryAdd(item, item.Position);

        /// <summary>
        /// Changes the position field of the given item to the given value, and tries to add the given item at the given
        /// position, provided the item is not already in the spatial map and the position is not already filled.
        /// If either of those are the case, nothing is changed and the function returns false.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="position">Position to add item to.</param>
        /// <returns>True if the item was successfully added; false otherwise.</returns>
        public bool TryAdd(T item, Point position)
        {
            if (!_spatialMap.TryAdd(item, position)) return false;
            item.Position = position;
            return true;
        }

        /// <summary>
        /// Changes the position field of the given item to the given value, and tries to add the given item at the given
        /// position, provided the item is not already in the spatial map and the position is not already filled.
        /// If either of those are the case, nothing is changed and the function returns false.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="x">X-value of the position to add item to.</param>
        /// <param name="y">Y-value of the position to add item to.</param>
        /// <returns>True if the item was successfully added; false otherwise.</returns>
        public bool TryAdd(T item, int x, int y) => TryAdd(item, new Point(x, y));
        #endregion

        #region Move
        /// <summary>
        /// Returns true if the given item can be moved from its current location to the specified one, eg. if the item
        /// does exists in the spatial map and if the new position is not already filled by some other item; false otherwise.
        /// </summary>
        /// <param name="item">Item to move.</param>
        /// <param name="target">Location to move item to.</param>
        /// <returns>true if the given item can be moved to the given position; false otherwise.</returns>
        public bool CanMove(T item, Point target) => _spatialMap.CanMove(item, target);

        /// <summary>
        /// Returns true if the given item can be moved from its current location to the specified one, eg. if the item
        /// exists in the spatial map and if the new position is not already filled by some other item; false otherwise.
        /// </summary>
        /// <param name="item">Item to move.</param>
        /// <param name="targetX">X-value of the location to move item to.</param>
        /// <param name="targetY">Y-value of the location to move item to.</param>
        /// <returns>true if the given item can be moved to the given position; false otherwise.</returns>
        public bool CanMove(T item, int targetX, int targetY) => _spatialMap.CanMove(item, targetX, targetY);

        /// <summary>
        /// Returns true if the item at the current position specified can be moved to the target position, eg. if an item exists
        /// at the current
        /// position and the new position is not already filled by some other item; false otherwise.
        /// </summary>
        /// <param name="current">Location to move items from.</param>
        /// <param name="target">Location to move items to.</param>
        /// <returns>
        /// true if all items at the position current can be moved to the position target; false if one or more items
        /// cannot be moved.
        /// </returns>
        public bool CanMoveAll(Point current, Point target) => _spatialMap.CanMoveAll(current, target);

        /// <summary>
        /// Returns true if the item at the current position specified can be moved to the target position, eg. if an item exists
        /// at the current
        /// position and the new position is not already filled by some other item; false otherwise.
        /// </summary>
        /// <param name="currentX">X-value of the location to move items from.</param>
        /// <param name="currentY">Y-value of the location to move items from.</param>
        /// <param name="targetX">X-value of the location to move items to.</param>
        /// <param name="targetY">Y-value of the location to move items to.</param>
        /// <returns>
        /// true if all items at the position current can be moved to the position target; false if one or more items
        /// cannot be moved.
        /// </returns>
        public bool CanMoveAll(int currentX, int currentY, int targetX, int targetY) => _spatialMap.CanMoveAll(currentX, currentY, targetX, targetY);

        /// <summary>
        /// Moves the item specified to the position specified, updating its <see cref="IPositionable.Position"/> field
        /// accordingly. Throws ArgumentException if the item does not exist in the spatial map or if the position is
        /// already filled by some other item.
        /// </summary>
        /// <param name="item">Item to move.</param>
        /// <param name="target">Location to move item to.</param>
        public void Move(T item, Point target)
        {
            item.Position = target;
        }

        /// <summary>
        /// Moves the item specified to the position specified, updating its <see cref="IPositionable.Position"/> field
        /// accordingly. Throws ArgumentException if the item does not exist in the spatial map or if the position is
        /// already filled by some other item.
        /// </summary>
        /// <param name="item">Item to move.</param>
        /// <param name="targetX">X-value of the location to move it to.</param>
        /// <param name="targetY">Y-value of the location to move it to.</param>
        public void Move(T item, int targetX, int targetY)
        {
            item.Position = new Point(targetX, targetY);
        }

        /// <inheritdoc />
        public bool TryMove(T item, Point target)
        {
            if (!_spatialMap.TryMove(item, target))
                return false;

            item.Position = target;
            return true;
        }

        /// <inheritdoc />
        public bool TryMove(T item, int targetX, int targetY) => TryMove(item, new Point(targetX, targetY));

        /// <summary>
        /// Moves the item at the specified source location to the target location.  Throws ArgumentException if one or
        /// more items cannot be moved, eg. if no item exists at the current position or the new position is already
        /// filled by some other item.
        /// </summary>
        /// <param name="current">Location to move items from.</param>
        /// <param name="target">Location to move items to.</param>
        public void MoveAll(Point current, Point target)
        {
            _spatialMap.MoveAll(current, target);

            // SpatialMap enforces that only one entity can be at any position; so if the move succeeded, the one
            // at the target position has to be the one we moved.
            _spatialMap.GetItem(target).Position = target;
        }

        /// <inheritdoc/>
        public bool TryMoveAll(Point current, Point target)
        {
            if (!_spatialMap.TryMoveAll(current, target))
                return false;
            // SpatialMap enforces that only one entity can be at any position; so if the move succeeded, the one
            // at the target position has to be the one we moved.
            _spatialMap.GetItem(target).Position = target;
            return true;
        }

        /// <summary>
        /// Moves the item at the specified source location to the target location.  Throws ArgumentException if one or
        /// more items cannot be moved, eg. if no item exists at the current position or the new position is already
        /// filled by some other item.
        /// </summary>
        /// <param name="currentX">X-value of the location to move items from.</param>
        /// <param name="currentY">Y-value of the location to move items from.</param>
        /// <param name="targetX">X-value of the location to move items to.</param>
        /// <param name="targetY">Y-value of the location to move items to.</param>
        public void MoveAll(int currentX, int currentY, int targetX, int targetY)
            => MoveAll(new Point(currentX, currentY), new Point(targetX, targetY));

        /// <inheritdoc/>
        public bool TryMoveAll(int currentX, int currentY, int targetX, int targetY) => _spatialMap.TryMoveAll(currentX, currentY, targetX, targetY);

        /// <summary>
        /// Moves whatever is at position current, if anything, to the target position, if it is a valid move.
        /// If something was moved, it returns what was moved. If nothing was moved, eg. either there was nothing at
        /// <paramref name="current" /> or already something at <paramref name="target" />, returns nothing.
        /// </summary>
        /// <remarks>
        /// Since this implementation of ISpatialMap guarantees that only one item may be at any
        /// given location at a time, the returned values will either be none, or a single value.
        /// </remarks>
        /// <param name="current">The position of the item to move.</param>
        /// <param name="target">The position to move the item to.</param>
        /// <returns>
        /// The item moved as a 1-element list if something was moved, or nothing if no item
        /// was moved.
        /// </returns>
        public List<T> MoveValid(Point current, Point target)
        {
            var moved = _spatialMap.MoveValid(current, target);
            if (moved.Count > 0)
                moved[0].Position = target;

            return moved;
        }

        /// <inheritdoc/>
        public void MoveValid(Point current, Point target, List<T> itemsMovedOutput)
        {
            int index = itemsMovedOutput.Count;
            _spatialMap.MoveValid(current, target, itemsMovedOutput);
            if (_spatialMap.Count > index)
                itemsMovedOutput[index].Position = target;
        }

        /// <summary>
        /// Moves whatever is at the "current" position specified, if anything, to the "target" position, if
        /// it is a valid move. If something was moved, it returns what was moved. If nothing was moved, eg.
        /// either there was nothing at the "current" position given, or already something at the "target" position
        /// given, it returns nothing.
        /// </summary>
        /// <remarks>
        /// Since this implementation of ISpatialMap guarantees that only one item may be at any
        /// given location at a time, the returned values will either be none, or a single value.
        /// </remarks>
        /// <param name="currentX">X-value of the location to move item from.</param>
        /// <param name="currentY">Y-value of the location to move item from.</param>
        /// <param name="targetX">X-value of the location to move item to.</param>
        /// <param name="targetY">Y-value of the location to move item to.</param>
        /// <returns>
        /// The item moved as a 1-element IEnumerable if something was moved, or nothing if no item
        /// was moved.
        /// </returns>
        public List<T> MoveValid(int currentX, int currentY, int targetX, int targetY)
            => MoveValid(new Point(currentX, currentY), new Point(targetX, targetY));

        /// <inheritdoc/>
        public void MoveValid(int currentX, int currentY, int targetX, int targetY, List<T> itemsMovedOutput)
            => MoveValid(new Point(currentX, currentY), new Point(targetX, targetY), itemsMovedOutput);
        #endregion

        #region Contains
        /// <inheritdoc />
        public bool Contains(T item) => _spatialMap.Contains(item);

        /// <inheritdoc />
        public bool Contains(Point position) => _spatialMap.Contains(position);

        /// <inheritdoc />
        public bool Contains(int x, int y) => _spatialMap.Contains(x, y);
        #endregion

        #region Get Items/Positions
        /// <summary>
        /// Gets the item at the given position as a 1-element enumerable if there is any item there,
        /// or nothing if there is nothing at that position.
        /// </summary>
        /// <remarks>
        /// Since this implementation guarantees that only one item can be at any given
        /// location at once, the return value is guaranteed to be at most one element. You may find it
        /// more convenient to use the <see cref="GetItem(Point)" /> function when you know you are
        /// dealing with a SpatialMap/AdvancedSpatialMap instance.
        /// </remarks>
        /// <param name="position">The position to return the item for.</param>
        /// <returns>
        /// The item at the given position as a 1-element enumerable, if there is an item there, or
        /// nothing if there is no item there.
        /// </returns>
        public IEnumerable<T> GetItemsAt(Point position) => _spatialMap.GetItemsAt(position);

        /// <summary>
        /// Gets the item at the given position as a 1-element enumerable if there is any item there,
        /// or nothing if there is nothing at that position.
        /// </summary>
        /// <remarks>
        /// Since this implementation guarantees that only one item can be at any given
        /// location at once, the return value is guaranteed to be at most one element. You may find it
        /// more convenient to use the <see cref="GetItem(int, int)" /> function when you know you are
        /// dealing with a SpatialMap/AdvancedSpatialMap instance.
        /// </remarks>
        /// <param name="x">The x-value of the position to return the item(s) for.</param>
        /// <param name="y">The y-value of the position to return the item(s) for.</param>
        /// <returns>
        /// The item at the given position as a 1-element enumerable, if there is an item there, or
        /// nothing if there is no item there.
        /// </returns>
        public IEnumerable<T> GetItemsAt(int x, int y) => _spatialMap.GetItemsAt(x, y);

        /// <inheritdoc />
        public Point? GetPositionOfOrNull(T item) => _spatialMap.GetPositionOfOrNull(item);

        /// <inheritdoc />
        public bool TryGetPositionOf(T item, out Point position) => _spatialMap.TryGetPositionOf(item, out position);

        /// <inheritdoc />
        public Point GetPositionOf(T item) => _spatialMap.GetPositionOf(item);

        /// <summary>
        /// Gets the item at the given position.  Throws ArgumentException no item exists at the given location.
        /// </summary>
        /// <exception cref="ArgumentException">No item is present in the spatial map at the given position.</exception>
        /// <remarks>
        /// Intended to be a more convenient function as compared to <see cref="GetItemsAt(Point)" />, since
        /// this spatial map implementation only allows a single item to at any given location at a time.
        /// </remarks>
        /// <param name="position">The position to return the item for.</param>
        /// <returns>
        /// The item at the given position.
        /// </returns>
        public T GetItem(Point position) => _spatialMap.GetItem(position);

        /// <summary>
        /// Gets the item at the given position.  Throws ArgumentException no item exists at the given location.
        /// </summary>
        /// <exception cref="ArgumentException">No item is present in the spatial map at the given position.</exception>
        /// <remarks>
        /// Intended to be a more convenient function as compared to <see cref="GetItemsAt(Point)" />, since
        /// this spatial map implementation only allows a single item to at any given location at a time.
        /// </remarks>
        /// <param name="x">The x-value of the position to return the item for.</param>
        /// <param name="y">The y-value of the position to return the item for.</param>
        /// <returns>
        /// The item at the given position.
        /// </returns>
        public T GetItem(int x, int y) => _spatialMap.GetItem(x, y);

        /// <summary>
        /// Gets the item at the given position, or default(T) if no item exists.
        /// </summary>
        /// <remarks>
        /// Intended to be a more convenient function as compared to <see cref="GetItemsAt(Point)" />, since
        /// this spatial map implementation only allows a single item to at any given location at a time.
        /// </remarks>
        /// <param name="position">The position to return the item for.</param>
        /// <returns>
        /// The item at the given position, or default(T) if no item exists at that location.
        /// </returns>
        public T? GetItemOrDefault(Point position) => _spatialMap.GetItemOrDefault(position);

        /// <summary>
        /// Gets the item at the given position, or default(T) if no item exists.
        /// </summary>
        /// <remarks>
        /// Intended to be a more convenient function as compared to <see cref="GetItemsAt(int, int)" />, since
        /// this spatial map implementation only allows a single item to at any given location at a time.
        /// </remarks>
        /// <param name="x">The x-value of the position to return the item for.</param>
        /// <param name="y">The y-value of the position to return the item for.</param>
        /// <returns>
        /// The item at the given position, or default(T) if no item exists at that location.
        /// </returns>
        public T? GetItemOrDefault(int x, int y) => _spatialMap.GetItemOrDefault(x, y);
        #endregion

        /// <summary>
        /// Returns a string representation of the spatial map, allowing display of the spatial map's
        /// items in a specified way.
        /// </summary>
        /// <param name="itemStringifier">Function that turns an item into a string.</param>
        /// <returns>A string representation of the spatial map.</returns>
        public string ToString(Func<T, string> itemStringifier) => _spatialMap.ToString(itemStringifier);

        /// <summary>
        /// Returns a string representation of the spatial map.
        /// </summary>
        /// <returns>A string representation of the spatial map.</returns>
        public override string ToString() => _spatialMap.ToString();

        #region Clear/Remove
        /// <inheritdoc />
        public void Clear() => _spatialMap.Clear();

        /// <summary>
        /// Removes the item specified. Throws ArgumentException if the item specified was
        /// not in the spatial map.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void Remove(T item) => _spatialMap.Remove(item);

        /// <summary>
        /// Removes the item specified. If the item specified was not in the spatial map, does nothing and returns false.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was removed; false otherwise.</returns>
        public bool TryRemove(T item) => _spatialMap.TryRemove(item);

        /// <summary>
        /// Removes whatever is at the given position, if anything, and returns the item removed as a
        /// 1-element IEnumerable. Returns nothing if no item was at the position specified.
        /// </summary>
        /// <remarks>
        /// Since this implementation of ISpatialMap guarantees that only one item can be at any given
        /// location at a time, the returned value is guaranteed to be either nothing or a single element.
        /// </remarks>
        /// <param name="position">The position of the item to remove.</param>
        /// <returns>
        /// The item removed as a 1-element list, if something was removed; an empty list if no item
        /// was found at that position.
        /// </returns>
        public List<T> Remove(Point position) => _spatialMap.Remove(position);

        /// <inheritdoc/>
        public bool TryRemove(Point position) => _spatialMap.TryRemove(position);

        /// <summary>
        /// Removes whatever is at the given position, if anything, and returns the item removed as a
        /// 1-element IEnumerable. Returns nothing if no item was at the position specified.
        /// </summary>
        /// <remarks>
        /// Since this implementation guarantees that only one item can be at any given
        /// location at a time, the returned value is guaranteed to be either nothing or a single element.
        /// </remarks>
        /// <param name="x">X-value of the position to remove item from.</param>
        /// <param name="y">Y-value of the position to remove item from.</param>
        /// <returns>
        /// The item removed as a 1-element IEnumerable, if something was removed; nothing if no item
        /// was found at that position.
        /// </returns>
        public List<T> Remove(int x, int y) => _spatialMap.Remove(x, y);

        /// <inheritdoc/>
        public bool TryRemove(int x, int y) => _spatialMap.TryRemove(x, y);
        #endregion

        #region Item Handlers
        private void OnItemAdded(object? sender, ItemEventArgs<T> e)
        {
            e.Item.PositionChanging += ItemOnPositionChanging;
        }

        private void ItemOnPositionChanging(object? sender, ValueChangedEventArgs<Point> e)
        {
            if (sender != null)
                _spatialMap.Move((T)sender, e.NewValue);
        }

        private void OnItemRemoved(object? sender, ItemEventArgs<T> e)
        {
            e.Item.PositionChanging -= ItemOnPositionChanging;
        }
        #endregion
    }

    /// <summary>
    /// A version of <see cref="SpatialMap{T}"/> which takes items that implement <see cref="IPositionable"/>,
    /// and uses that interface's properties/events to automatically ensure items are recorded at the proper positions
    /// in the spatial map when they move and that the position fields are updated if the spatial map's move functions
    /// are used.
    /// </summary>
    /// <remarks>
    /// This class automatically keeps the spatial map position of each object synced up with their
    /// <see cref="IPositionable.Position"/> property; you may either use the Move functions of the spatial map,
    /// in which case the Position fields of the objects are updated as appropriate, or you may change the Position
    /// field, in which case the spatial map position is updated to match.
    ///
    /// If you want to manually control the positions of items in the spatial map, you should use
    /// <see cref="SpatialMap{T}"/> instead.
    /// </remarks>
    /// <typeparam name="T">The type of object that will be contained by this spatial map.</typeparam>
    public sealed class AutoSyncSpatialMap<T> : AutoSyncAdvancedSpatialMap<T> where T : class, IHasID, IPositionable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pointComparer">
        /// Equality comparer to use for comparison and hashing of points, as object are added to/removed from/moved
        /// around the spatial map.  Be especially mindful of the efficiency of its GetHashCode function, as it will
        /// determine the efficiency of many SpatialMap functions.  Defaults to the default equality comparer for
        /// Point.
        /// </param>
        /// <param name="initialCapacity">
        /// The initial maximum number of elements the SpatialMap can hold before it has to
        /// internally resize data structures. Defaults to 32.
        /// </param>
        public AutoSyncSpatialMap(IEqualityComparer<Point>? pointComparer = null, int initialCapacity = 32)
            : base(new IDComparer<T>(), pointComparer, initialCapacity)
        { }
    }
}

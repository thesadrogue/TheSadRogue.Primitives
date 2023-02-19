using System;
using System.Collections;
using System.Collections.Generic;
using SadRogue.Primitives.Pooling;

namespace SadRogue.Primitives.SpatialMaps
{
    /// <summary>
    /// A version of <see cref="AdvancedMultiSpatialMap{T}"/> which takes items that implement <see cref="IPositionable"/>,
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
    /// <see cref="AdvancedMultiSpatialMap{T}"/> instead.
    /// </remarks>
    /// <typeparam name="T">The type of object that will be contained by this spatial map.</typeparam>
    public class AutoSyncAdvancedMultiSpatialMap<T> : ISpatialMap<T>
        where T : class, IPositionable
    {
        private readonly AdvancedMultiSpatialMap<T> _multiSpatialMap;

        /// <inheritdoc />
        public int Count => _multiSpatialMap.Count;

        /// <inheritdoc />
        public IEnumerable<T> Items => _multiSpatialMap.Items;

        /// <inheritdoc />
        public IEnumerable<Point> Positions => _multiSpatialMap.Positions;

        /// <inheritdoc />
        public event EventHandler<ItemEventArgs<T>>? ItemAdded
        {
            add => _multiSpatialMap.ItemAdded += value;
            remove => _multiSpatialMap.ItemAdded -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ItemMovedEventArgs<T>>? ItemMoved
        {
            add => _multiSpatialMap.ItemMoved += value;
            remove => _multiSpatialMap.ItemMoved -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ItemEventArgs<T>>? ItemRemoved
        {
            add => _multiSpatialMap.ItemRemoved += value;
            remove => _multiSpatialMap.ItemRemoved -= value;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="itemComparer">
        /// Equality comparer to use for comparison and hashing of type T. Be especially mindful of the
        /// efficiency of its GetHashCode function, as it will determine the efficiency of many AdvancedMultiSpatialMap
        /// functions.
        /// </param>
        /// <param name="pointComparer">
        /// Equality comparer to use for comparison and hashing of points, as object are added to/removed from/moved
        /// around the spatial map.  Be especially mindful of the efficiency of its GetHashCode function, as it will
        /// determine the efficiency of many AdvancedMultiSpatialMap functions.  Defaults to the default equality
        /// comparer for Point, which uses a fairly efficient generalized hashing algorithm.
        /// </param>
        /// <param name="initialCapacity">
        /// The initial maximum number of elements the AdvancedMultiSpatialMap can hold before it has to
        /// internally resize data structures. Defaults to 32.
        /// </param>
        public AutoSyncAdvancedMultiSpatialMap(IEqualityComparer<T> itemComparer, IEqualityComparer<Point>? pointComparer = null,
                                               int initialCapacity = 32)
            : this(itemComparer, new ListPool<T>(50, 16), pointComparer, initialCapacity)
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="itemComparer">
        /// Equality comparer to use for comparison and hashing of type T. Be especially mindful of the
        /// efficiency of its GetHashCode function, as it will determine the efficiency of many AdvancedMultiSpatialMap
        /// functions.
        /// </param>
        /// <param name="listPool">
        /// The list pool implementation to use.  Specify <see cref="NoPoolingListPool{T}"/> to disable pooling entirely.
        /// This implementation _may_ be shared with other spatial maps if you wish, however be aware that no thread safety is implemented
        /// by the default list pool implementations or the spatial map itself.
        /// </param>
        /// <param name="pointComparer">
        /// Equality comparer to use for comparison and hashing of points, as object are added to/removed from/moved
        /// around the spatial map.  Be especially mindful of the efficiency of its GetHashCode function, as it will
        /// determine the efficiency of many AdvancedMultiSpatialMap functions.  Defaults to the default equality
        /// comparer for Point, which uses a fairly efficient generalized hashing algorithm.
        /// </param>
        /// <param name="initialCapacity">
        /// The initial maximum number of elements the AdvancedMultiSpatialMap can hold before it has to
        /// internally resize data structures. Defaults to 32.
        /// </param>
        public AutoSyncAdvancedMultiSpatialMap(IEqualityComparer<T> itemComparer, IListPool<T> listPool,
                                               IEqualityComparer<Point>? pointComparer = null,
                                               int initialCapacity = 32)
        {
            _multiSpatialMap = new AdvancedMultiSpatialMap<T>(itemComparer, listPool, pointComparer, initialCapacity);

            _multiSpatialMap.ItemAdded += OnItemAdded;
            _multiSpatialMap.ItemRemoved += OnItemRemoved;
        }

        #region Enumeration
        /// <summary>
        /// Used by foreach loop, so that the class will give ISpatialTuple objects when used in a
        /// foreach loop. Generally should never be called explicitly.
        /// </summary>
        /// <returns>An enumerator for the spatial map.</returns>
        public IEnumerator<ItemPositionPair<T>> GetEnumerator() => _multiSpatialMap.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_multiSpatialMap).GetEnumerator();
        #endregion

        /// <inheritdoc />
        public IReadOnlySpatialMap<T> AsReadOnly() => _multiSpatialMap.AsReadOnly();

        #region Add
        /// <summary>
        /// Returns true if the given item can be added at the given position, eg. if the item is not already in the spatial map;
        /// false otherwise.
        /// </summary>
        /// <param name="newItem">Item to add.</param>
        /// <param name="position">Position to add item to.</param>
        /// <returns>True if the item can be successfully added at the position given; false otherwise.</returns>
        public bool CanAdd(T newItem, Point position) => _multiSpatialMap.CanAdd(newItem, position);

        /// <summary>
        /// Returns true if the given item can be added at the given position, eg. if the item is not already in the spatial map;
        /// false otherwise.
        /// </summary>
        /// <param name="newItem">Item to add.</param>
        /// <param name="x">X-value of the position to add item to.</param>
        /// <param name="y">Y-value of the position to add item to.</param>
        /// <returns>True if the item can be successfully added at the position given; false otherwise.</returns>
        public bool CanAdd(T newItem, int x, int y) => _multiSpatialMap.CanAdd(newItem, x, y);

        /// <summary>
        /// Adds the given item at its position, provided the item is not already in the
        /// spatial map. If the item is already added, throws ArgumentException.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item) => _multiSpatialMap.Add(item, item.Position);

        /// <summary>
        /// Changes the position field of the given item to the given value, and then adds it to the spatial map, provided
        /// the item is not already in the spatial map. If the item is already added, throws ArgumentException.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="position">The position at which to add the new item.</param>
        public void Add(T item, Point position)
        {
            item.Position = position;
            _multiSpatialMap.Add(item, position);
        }

        /// <summary>
        /// Changes the position field of the given item to the given value, and then adds it to the spatial map, provided
        /// the item is not already in the spatial map. If the item is already added, throws ArgumentException.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="x">x-value of the position to add item to.</param>
        /// <param name="y">y-value of the position to add item to.</param>
        public void Add(T item, int x, int y)
        {
            var position = new Point(x, y);

            item.Position = position;
            _multiSpatialMap.Add(item, position);
        }

        /// <summary>
        /// Adds the given item at its position, provided the item is not already in the spatial map. If the item is
        /// already added, returns false.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>True if the item was successfully added; false otherwise.</returns>
        public bool TryAdd(T item) => _multiSpatialMap.TryAdd(item, item.Position);

        /// <summary>
        /// Changes the position field of the given item to the given value, and then adds it to the spatial map, provided
        /// the item is not already in the spatial map. If the item is already added, nothing is changed and the function returns false.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="position">The position at which to add the new item.</param>
        /// <returns>True if the item was successfully added; false otherwise.</returns>
        public bool TryAdd(T item, Point position)
        {

            if (!_multiSpatialMap.TryAdd(item, position)) return false;
            item.Position = position;
            return true;
        }

        /// <summary>
        /// Changes the position field of the given item to the given value, and then adds it to the spatial map, provided
        /// the item is not already in the spatial map. If the item is already added, nothing is changed and the function returns false.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="x">x-value of the position to add item to.  Must match the item's Position field.</param>
        /// <param name="y">y-value of the position to add item to.  Must match the item's Position field.</param>
        /// <returns>True if the item was successfully added; false otherwise.</returns>
        public bool TryAdd(T item, int x, int y) => TryAdd(item, new Point(x, y));
        #endregion

        #region Move
        /// <summary>
        /// Returns true if the given item can be moved from its current location to the specified one,
        /// eg. the item is contained within the spatial map; false otherwise.
        /// </summary>
        /// <param name="item">Item to move.</param>
        /// <param name="target">Location to move item to.</param>
        /// <returns>true if the given item can be moved to the given position; false otherwise.</returns>
        public bool CanMove(T item, Point target)
            => _multiSpatialMap.CanMove(item, target);

        /// <summary>
        /// Returns true if the given item can be moved from its current location to the specified one,
        /// eg. the item is contained within the spatial map; false otherwise.
        /// </summary>
        /// <param name="item">Item to move.</param>
        /// <param name="targetX">X-value of the location to move item to.</param>
        /// <param name="targetY">Y-value of the location to move item to.</param>
        /// <returns>true if the given item can be moved to the given position; false otherwise.</returns>
        public bool CanMove(T item, int targetX, int targetY)
            => _multiSpatialMap.CanMove(item, targetX, targetY);

        /// <inheritdoc />
        public bool CanMoveAll(Point current, Point target)
            => _multiSpatialMap.CanMoveAll(current, target);

        /// <inheritdoc />
        public bool CanMoveAll(int currentX, int currentY, int targetX, int targetY)
            => _multiSpatialMap.CanMoveAll(currentX, currentY, targetX, targetY);

        /// <summary>
        /// Moves the item specified to the position specified, updating its <see cref="IPositionable.Position"/> field
        /// accordingly. If the item does not exist in the spatial map or is already at the target position, the function
        /// throws ArgumentException.
        /// </summary>
        /// <param name="item">The item to move.</param>
        /// <param name="target">The position to move it to.</param>
        public void Move(T item, Point target)
        {
            item.Position = target;
        }

        /// <summary>
        /// Moves the item specified to the position specified, updating its <see cref="IPositionable.Position"/> field
        /// accordingly. If the item does not exist in the spatial map or is already at the target position, the function
        /// throws ArgumentException.
        /// </summary>
        /// <param name="item">The item to move.</param>
        /// <param name="targetX">X-value of the location to move it to.</param>
        /// <param name="targetY">Y-value of the location to move it to.</param>
        public void Move(T item, int targetX, int targetY)
        {
            item.Position = new Point(targetX, targetY);
        }

        /// <inheritdoc />
        public bool TryMove(T item, Point target)
        {
            if (!_multiSpatialMap.TryMove(item, target))
                return false;

            item.Position = target;
            return true;

        }

        /// <inheritdoc />
        public bool TryMove(T item, int targetX, int targetY) => TryMove(item, new Point(targetX, targetY));

        /// <summary>
        /// Moves all items at the specified source location to the target location, updating their
        /// <see cref="IPositionable.Position"/> fields accordingly.  Throws ArgumentException if there are
        /// no items to be moved.
        /// </summary>
        /// <param name="current">Location to move items from.</param>
        /// <param name="target">Location to move items to.</param>
        public void MoveAll(Point current, Point target)
        {
            // Everything will move in MultiSpatialMap, so if this list contains any values, it's a complete list
            var movedItems = _multiSpatialMap.MoveValid(current, target);

            if (movedItems.Count == 0)
                throw new ArgumentException(
                    $"Tried to move all items from {current} in {GetType().Name}, but there was nothing at that position.",
                    nameof(current));

            foreach (var item in movedItems)
                item.Position = target;
        }

        /// <inheritdoc/>
        public bool TryMoveAll(Point current, Point target)
        {
            // Everything will move in MultiSpatialMap, so if this list contains any values, it's a complete list
            var movedItems = _multiSpatialMap.MoveValid(current, target);

            if (movedItems.Count == 0)
                return false;

            foreach (var item in movedItems)
                item.Position = target;

            return true;
        }

        /// <summary>
        /// Moves all items at the specified source location to the target location, updating their
        /// <see cref="IPositionable.Position"/> fields accordingly.  Throws ArgumentException if there are
        /// no items to be moved.
        /// </summary>
        /// <param name="currentX">X-value of the location to move items from.</param>
        /// <param name="currentY">Y-value of the location to move items from.</param>
        /// <param name="targetX">X-value of the location to move items to.</param>
        /// <param name="targetY">Y-value of the location to move items to.</param>
        public void MoveAll(int currentX, int currentY, int targetX, int targetY)
            => MoveAll(new Point(currentX, currentY), new Point(targetX, targetY));

        /// <inheritdoc/>
        public bool TryMoveAll(int currentX, int currentY, int targetX, int targetY)
            => TryMoveAll(new Point(currentX, currentY), new Point(targetX, targetY));

        /// <inheritdoc />
        public List<T> MoveValid(Point current, Point target)
        {
            var list = _multiSpatialMap.MoveValid(current, target);
            foreach (var obj in list)
                obj.Position = target;

            return list;
        }

        /// <inheritdoc />
        public void MoveValid(Point current, Point target, List<T> itemsMovedOutput)
        {
            int idx = itemsMovedOutput.Count;
            _multiSpatialMap.MoveValid(current, target, itemsMovedOutput);

            int count = itemsMovedOutput.Count;
            for (int i = idx; i < count; i++)
                itemsMovedOutput[i].Position = target;
        }

        /// <inheritdoc />
        public List<T> MoveValid(int currentX, int currentY, int targetX, int targetY)
            => MoveValid(new Point(currentX, currentY), new Point(targetX, targetY));

        /// <inheritdoc />
        public void MoveValid(int currentX, int currentY, int targetX, int targetY, List<T> itemsMovedOutput)
            => MoveValid(new Point(currentX, currentY), new Point(targetX, targetY), itemsMovedOutput);
        #endregion

        #region Contains
        /// <inheritdoc />
        public bool Contains(T item) => _multiSpatialMap.Contains(item);

        /// <inheritdoc />
        public bool Contains(Point position) => _multiSpatialMap.Contains(position);

        /// <inheritdoc />
        public bool Contains(int x, int y) => _multiSpatialMap.Contains(x, y);
        #endregion

        #region Get Items/Positions

        /// <summary>
        /// Gets the item(s) at the given position if there are any items, or returns
        /// nothing if there is nothing at that position.
        /// </summary>
        /// <param name="position">The position to return the item(s) for.</param>
        /// <returns>
        /// The item(s) at the given position if there are any items, or nothing if there is nothing
        /// at that position.
        /// </returns>
        public ListEnumerator<T> GetItemsAt(Point position) => _multiSpatialMap.GetItemsAt(position);

        IEnumerable<T> IReadOnlySpatialMap<T>.GetItemsAt(Point position) => _multiSpatialMap.GetItemsAt(position);

        /// <summary>
        /// Gets the item(s) at the given position if there are any items, or returns
        /// nothing if there is nothing at that position.
        /// </summary>
        /// <param name="x">The x-value of the position to return the item(s) for.</param>
        /// <param name="y">The y-value of the position to return the item(s) for.</param>
        /// <returns>
        /// The item(s) at the given position if there are any items, or nothing if there is nothing
        /// at that position.
        /// </returns>
        public ListEnumerator<T> GetItemsAt(int x, int y) => _multiSpatialMap.GetItemsAt(new Point(x, y));

        IEnumerable<T> IReadOnlySpatialMap<T>.GetItemsAt(int x, int y) => _multiSpatialMap.GetItemsAt(x, y);

        /// <inheritdoc />
        public Point? GetPositionOfOrNull(T item) => _multiSpatialMap.GetPositionOfOrNull(item);

        /// <inheritdoc />
        public bool TryGetPositionOf(T item, out Point position) => _multiSpatialMap.TryGetPositionOf(item, out position);

        /// <inheritdoc />
        public Point GetPositionOf(T item) => _multiSpatialMap.GetPositionOf(item);
        #endregion


        /// <summary>
        /// Returns a string representation of the spatial map, allowing display of the
        /// spatial map's items in a specified way.
        /// </summary>
        /// <param name="itemStringifier">Function that turns an item into a string.</param>
        /// <returns>A string representation of the spatial map.</returns>
        public string ToString(Func<T, string> itemStringifier) => _multiSpatialMap.ToString(itemStringifier);

        /// <summary>
        /// Returns a string representation of the spatial map.
        /// </summary>
        /// <returns>A string representation of the spatial map.</returns>
        public override string ToString() => _multiSpatialMap.ToString();

        #region Clear/Remove
        /// <inheritdoc />
        public void Clear() => _multiSpatialMap.Clear();

        /// <summary>
        /// Removes the item specified, if it exists.  Throws ArgumentException if the item is
        /// not in the spatial map.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void Remove(T item) => _multiSpatialMap.Remove(item);

        /// <summary>
        /// Removes the item specified, if it exists.  If the item is not in the spatial map, returns false.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was successfully removed; false otherwise.</returns>
        public bool TryRemove(T item) => _multiSpatialMap.TryRemove(item);

        /// <inheritdoc />
        public List<T> Remove(Point position) => _multiSpatialMap.Remove(position);

        /// <inheritdoc/>
        public bool TryRemove(Point position) => _multiSpatialMap.TryRemove(position);

        /// <inheritdoc />
        public List<T> Remove(int x, int y) => _multiSpatialMap.Remove(x, y);

        /// <inheritdoc />
        public bool TryRemove(int x, int y) => _multiSpatialMap.TryRemove(x, y);
        #endregion

        #region Item Handlers
        private void OnItemAdded(object? sender, ItemEventArgs<T> e)
        {
            e.Item.PositionChanging += ItemOnPositionChanging;
        }

        private void ItemOnPositionChanging(object? sender, ValueChangedEventArgs<Point> e)
        {
            if (sender != null)
                _multiSpatialMap.Move((T)sender, e.NewValue);
        }

        private void OnItemRemoved(object? sender, ItemEventArgs<T> e)
        {
            e.Item.PositionChanging -= ItemOnPositionChanging;
        }
        #endregion
    }

    /// <summary>
    /// A version of <see cref="MultiSpatialMap{T}"/> which takes items that implement <see cref="IPositionable"/>,
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
    /// <see cref="MultiSpatialMap{T}"/> instead.
    /// </remarks>
    /// <typeparam name="T">The type of object that will be contained by this spatial map.</typeparam>
    public sealed class AutoSyncMultiSpatialMap<T> : AutoSyncAdvancedMultiSpatialMap<T> where T : class, IHasID, IPositionable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="listPool">
        /// The list pool implementation to use.  Specify <see cref="NoPoolingListPool{T}"/> to disable pooling entirely.
        /// This implementation _may_ be shared with other spatial maps if you wish, however be aware that no thread safety is implemented
        /// by the default list pool implementations or the spatial map itself.
        /// </param>
        /// <param name="pointComparer">
        /// Equality comparer to use for comparison and hashing of points, as object are added to/removed from/moved
        /// around the spatial map.  Be especially mindful of the efficiency of its GetHashCode function, as it will
        /// determine the efficiency of many MultiSpatialMap functions.  Defaults to the default equality
        /// comparer for Point, which uses a fairly efficient generalized hashing algorithm.
        /// </param>
        /// <param name="initialCapacity">
        /// The initial maximum number of elements the spatial map can hold before it has to
        /// internally resize data structures. Defaults to 32.
        /// </param>
        public AutoSyncMultiSpatialMap(IListPool<T> listPool, IEqualityComparer<Point>? pointComparer = null, int initialCapacity = 32)
            : base(new IDComparer<T>(), listPool, pointComparer, initialCapacity)
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pointComparer">
        /// Equality comparer to use for comparison and hashing of points, as object are added to/removed from/moved
        /// around the spatial map.  Be especially mindful of the efficiency of its GetHashCode function, as it will
        /// determine the efficiency of many MultiSpatialMap functions.  Defaults to the default equality
        /// comparer for Point, which uses a fairly efficient generalized hashing algorithm.
        /// </param>
        /// <param name="initialCapacity">
        /// The initial maximum number of elements the spatial map can hold before it has to
        /// internally resize data structures. Defaults to 32.
        /// </param>
        public AutoSyncMultiSpatialMap(IEqualityComparer<Point>? pointComparer = null, int initialCapacity = 32)
            : base(new IDComparer<T>(), pointComparer, initialCapacity)
        { }
    }
}

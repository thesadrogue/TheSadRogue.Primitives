using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives.Pooling;

namespace SadRogue.Primitives.SpatialMaps
{
    /// <summary>
    /// A version of <see cref="AdvancedLayeredSpatialMap{T}"/> which takes items that implement <see cref="IPositionable"/>,
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
    /// <see cref="AdvancedLayeredSpatialMap{T}"/> instead.
    /// </remarks>
    /// <typeparam name="T">The type of object that will be contained by this spatial map.</typeparam>
    public class AutoSyncAdvancedLayeredSpatialMap<T> : ISpatialMap<T>, IReadOnlyLayeredSpatialMap<T>
        where T : class, IHasLayer, IPositionable
    {
        private readonly AdvancedLayeredSpatialMap<T> _layeredSpatialMap;

        /// <inheritdoc />
        public int Count => _layeredSpatialMap.Count;

        /// <inheritdoc />
        public IEnumerable<T> Items => _layeredSpatialMap.Items;

        /// <inheritdoc />
        public IEnumerable<Point> Positions => _layeredSpatialMap.Positions;

        /// <inheritdoc />
        public event EventHandler<ItemEventArgs<T>>? ItemAdded
        {
            add => _layeredSpatialMap.ItemAdded += value;
            remove => _layeredSpatialMap.ItemAdded -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ItemMovedEventArgs<T>>? ItemMoved
        {
            add => _layeredSpatialMap.ItemMoved += value;
            remove => _layeredSpatialMap.ItemMoved -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ItemEventArgs<T>>? ItemRemoved
        {
            add => _layeredSpatialMap.ItemRemoved += value;
            remove => _layeredSpatialMap.ItemRemoved -= value;
        }

        /// <inheritdoc />
        public LayerMasker LayerMasker => _layeredSpatialMap.LayerMasker;

        /// <inheritdoc />
        public IEnumerable<IReadOnlySpatialMap<T>> Layers => _layeredSpatialMap.Layers;

        /// <inheritdoc />
        public int NumberOfLayers => _layeredSpatialMap.NumberOfLayers;

        /// <inheritdoc />
        public int StartingLayer => _layeredSpatialMap.StartingLayer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="itemComparer">
        /// Equality comparer to use for comparison and hashing of type T. Be especially mindful of the
        /// efficiency of its GetHashCode function, as it will determine the efficiency of
        /// many AdvancedLayeredSpatialMap functions.
        /// </param>
        /// <param name="numberOfLayers">Number of layers to include.</param>
        /// <param name="customListPoolCreator">
        /// A function used to determine the list pool implementation used for the spatial maps which support multiple
        /// items in a location (if any).  The function takes the layer it is creating the pool for as a parameter.
        /// If no custom creator is specified, a ListPool is used.
        /// </param>
        /// <param name="pointComparer">
        /// Equality comparer to use for comparison and hashing of points, as object are added to/removed from/moved
        /// around the spatial map.  Be especially mindful of the efficiency of its GetHashCode function, as it will
        /// determine the efficiency of many AdvancedLayeredSpatialMap functions.  Defaults to the default equality
        /// comparer for Point, which uses a fairly efficient generalized hashing algorithm.
        /// </param>
        /// <param name="startingLayer">Index to use for the first layer.</param>
        /// <param name="layersSupportingMultipleItems">
        /// A layer mask indicating which layers should support multiple items residing at the same
        /// location on that layer. Defaults to no layers.
        /// </param>
        public AutoSyncAdvancedLayeredSpatialMap(IEqualityComparer<T> itemComparer,
                                         int numberOfLayers, Func<int, IListPool<T>>? customListPoolCreator = null,
                                         IEqualityComparer<Point>? pointComparer = null, int startingLayer = 0,
                                         uint layersSupportingMultipleItems = 0)
        {
            _layeredSpatialMap = new AdvancedLayeredSpatialMap<T>(itemComparer, numberOfLayers, customListPoolCreator,
                pointComparer, startingLayer, layersSupportingMultipleItems);

            _layeredSpatialMap.ItemAdded += OnItemAdded;
            _layeredSpatialMap.ItemRemoved += OnItemRemoved;
        }

        #region Enumeration
        /// <summary>
        /// Used by foreach loop, so that the class will give ISpatialTuple objects when used in a
        /// foreach loop. Generally should never be called explicitly.
        /// </summary>
        /// <returns>An enumerator for the spatial map</returns>
        public IEnumerator<ItemPositionPair<T>> GetEnumerator() => _layeredSpatialMap.GetEnumerator();

        /// <summary>
        /// Generic iterator used internally by foreach loops.
        /// </summary>
        /// <returns>Enumerator to ISpatialTuple instances.</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_layeredSpatialMap).GetEnumerator();
        #endregion

        /// <inheritdoc />
        public IReadOnlyLayeredSpatialMap<T> AsReadOnly() => _layeredSpatialMap.AsReadOnly();

        IReadOnlySpatialMap<T> IReadOnlySpatialMap<T>.AsReadOnly() => _layeredSpatialMap.AsReadOnly();

        #region Add

        /// <summary>
        /// Returns true if the given item can be added at its current position, eg. it is on a layer in the spatial map and its
        /// layer will accept it; false otherwise.
        /// </summary>
        /// <param name="newItem">Item to add.</param>
        /// <returns>True if the item can be successfully added at its current position; false otherwise.</returns>
        public bool CanAdd(T newItem) => _layeredSpatialMap.CanAdd(newItem, newItem.Position);

        /// <summary>
        /// Returns true if the given item can be added at the given position, eg. it is on a layer in the spatial map and its
        /// layer will accept it; false otherwise.
        /// </summary>
        /// <param name="newItem">Item to add.</param>
        /// <param name="position">Position to add item to.</param>
        /// <returns>True if the item can be successfully added at the position given; false otherwise.</returns>
        public bool CanAdd(T newItem, Point position) => _layeredSpatialMap.CanAdd(newItem, position);

        /// <summary>
        /// Returns true if the given item can be added at the given position, eg. it is on a layer in the spatial map and its
        /// layer will accept it; false otherwise.
        /// </summary>
        /// <param name="newItem">Item to add.</param>
        /// <param name="x">X-value of the position to add item to.</param>
        /// <param name="y">Y-value of the position to add item to.</param>
        /// <returns>True if the item can be successfully added at the position given; false otherwise.</returns>
        public bool CanAdd(T newItem, int x, int y) => _layeredSpatialMap.CanAdd(newItem, x, y);

        /// <summary>
        /// Adds the given item at its position on the correct layer.  ArgumentException is thrown if the layer is
        /// invalid or the item otherwise cannot be added to its layer.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void Add(T item) => _layeredSpatialMap.Add(item, item.Position);

        /// <summary>
        /// Changes the position field of the given item to the given value, and then adds it to the spatial map on the
        /// correct layer.  ArgumentException is thrown if the layer is invalid or the item otherwise cannot be added to
        /// its layer.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="position">Position to add item at.</param>
        public void Add(T item, Point position)
        {
            item.Position = position;
            _layeredSpatialMap.Add(item, position);
        }

        /// <summary>
        /// Changes the position field of the given item to the given value, and then adds it to the spatial map on the
        /// correct layer.  ArgumentException is thrown if the layer is invalid or the item otherwise cannot be added to
        /// its layer.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="x">X-value of position to add item at.</param>
        /// <param name="y">Y-value of position to add item at.</param>
        public void Add(T item, int x, int y) => Add(item, new Point(x, y));

        /// <summary>
        /// Adds the given item at its position on the correct layer.  Returns false if the layer is
        /// invalid or the item otherwise cannot be added to its layer.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>True if the item was successfully added; false otherwise.</returns>
        public bool TryAdd(T item) => _layeredSpatialMap.TryAdd(item, item.Position);

        /// <summary>
        /// Changes the position field of the given item to the given value, and then adds it to the spatial map on the
        /// correct layer.  If the layer is invalid or the item otherwise cannot be added to its layer, does nothing and
        /// returns false.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="position">Position to add item at.</param>
        /// <returns>True if the item was added, false otherwise.</returns>
        public bool TryAdd(T item, Point position)
        {
            if (!_layeredSpatialMap.TryAdd(item, position))
                return false;

            item.Position = position;
            return true;
        }

        /// <summary>
        /// Changes the position field of the given item to the given value, and then adds it to the spatial map on the
        /// correct layer.  If the layer is invalid or the item otherwise cannot be added to its layer, does nothing and
        /// returns false.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="x">X-value of position to add item at.</param>
        /// <param name="y">Y-value of position to add item at.</param>
        /// <returns>True if the item was added, false otherwise.</returns>
        public bool TryAdd(T item, int x, int y) => TryAdd(item, new Point(x, y));
        #endregion

        #region Move
        /// <summary>
        /// Returns true if the given item can be moved from its current location to the specified one, eg. it is in the spatial
        /// map and its layer will accept it at the new position; false otherwise.
        /// </summary>
        /// <param name="item">Item to move.</param>
        /// <param name="target">Location to move item to.</param>
        /// <returns>true if the given item can be moved to the given position; false otherwise.</returns>
        public bool CanMove(T item, Point target) => _layeredSpatialMap.CanMove(item, target);

        /// <summary>
        /// Returns true if the given item can be moved from its current location to the specified one, eg. it is in the spatial
        /// map and its layer will
        /// accept it at the new position; false otherwise.
        /// </summary>
        /// <param name="item">Item to move.</param>
        /// <param name="targetX">X-value of the location to move item to.</param>
        /// <param name="targetY">Y-value of the location to move item to.</param>
        /// <returns>true if the given item can be moved to the given position; false otherwise.</returns>
        public bool CanMove(T item, int targetX, int targetY) => _layeredSpatialMap.CanMove(item, targetX, targetY);

        /// <inheritdoc />
        public bool CanMoveAll(Point current, Point target, uint layerMask = uint.MaxValue) => _layeredSpatialMap.CanMoveAll(current, target, layerMask);

        /// <inheritdoc />
        public bool CanMoveAll(int currentX, int currentY, int targetX, int targetY, uint layerMask = uint.MaxValue) => _layeredSpatialMap.CanMoveAll(currentX, currentY, targetX, targetY, layerMask);

        /// <inheritdoc />
        bool IReadOnlySpatialMap<T>.CanMoveAll(Point current, Point target) => _layeredSpatialMap.CanMoveAll(current, target);

        /// <inheritdoc />
        bool IReadOnlySpatialMap<T>.CanMoveAll(int currentX, int currentY, int targetX, int targetY) => _layeredSpatialMap.CanMoveAll(currentX, currentY, targetX, targetY);

        /// <summary>
        /// Moves the item specified to the position specified, updating its <see cref="IPositionable.Position"/> field
        /// accordingly. Throws ArgumentException if either the item given
        /// isn't in the spatial map, or if the layer that the item resides on is configured to allow only one item per
        /// location at any given time and there is already an item at <paramref name="target" />.
        /// </summary>
        /// <param name="item">The item to move.</param>
        /// <param name="target">Position to move the given item to.</param>
        public void Move(T item, Point target)
        {
            if (!Contains(item)) throw new ArgumentException(
                $"Tried to move item in {GetType().Name}, but the item does not exist.",
                nameof(item));

            item.Position = target;
        }

        /// <summary>
        /// Moves the item specified to the position specified, updating its <see cref="IPositionable.Position"/> field
        /// accordingly. Throws ArgumentException if either the item given
        /// isn't in the spatial map, or if the layer that the item resides on is configured to allow only one item per
        /// location at any given time and there is already an item at the target position.
        /// </summary>
        /// <param name="item">The item to move.</param>
        /// <param name="targetX">X-value of position to move the given item to.</param>
        /// <param name="targetY">Y-value of position to move the given item to.</param>
        public void Move(T item, int targetX, int targetY)
        {
            item.Position = new Point(targetX, targetY);
        }

        /// <inheritdoc />
        public bool TryMove(T item, Point target)
        {
            if (!_layeredSpatialMap.TryMove(item, target))
                return false;

            item.Position = target;
            return true;
        }

        /// <inheritdoc />
        public bool TryMove(T item, int targetX, int targetY) => TryMove(item, new Point(targetX, targetY));

        /// <summary>
        /// Moves all items that are on layers in <paramref name="layerMask" /> at the specified source location to the target
        /// location, updating their <see cref="IPositionable.Position"/> fields accordingly.  Throws ArgumentException
        /// if one or more items cannot be moved or there are no items to be moved.
        /// </summary>
        /// <param name="current">Location to move items from.</param>
        /// <param name="target">Location to move items to.</param>
        /// <param name="layerMask">The layer mask to use to find items.</param>
        public void MoveAll(Point current, Point target, uint layerMask = uint.MaxValue)
        {
            var moved = _layeredSpatialMap.GetItemsAt(current, layerMask).ToArray();

            // It will be better to call MoveAll/MoveValid, rather than depend on the position updates, since this can save
            // an entire list's worth of allocation if the underlying map being moved is a MultiSpatialMap.
            //
            // Since MoveAll is not generally considered to be a function you should call if the possibility of failure
            // is expected, we'll get a list of the things we're going to move first and use the MoveAll implementation
            // of LayeredSpatialMap which avoids the allocation of something like MoveValid.
            _layeredSpatialMap.MoveAll(current, target, layerMask);

            foreach (var item in moved)
                item.Position = target;
        }

        /// <summary>
        /// Moves all items that are on layers in <paramref name="layerMask" /> at the specified source location to the target
        /// location, updating their <see cref="IPositionable.Position"/> fields accordingly.  Throws ArgumentException
        /// if one or more items cannot be moved or there are no items to be moved.
        /// </summary>
        /// <param name="currentX">X-value of the location to move items from.</param>
        /// <param name="currentY">Y-value of the location to move items from.</param>
        /// <param name="targetX">X-value of the location to move items to.</param>
        /// <param name="targetY">Y-value of the location to move items to.</param>
        /// <param name="layerMask">The layer mask to use to find items.</param>
        public void MoveAll(int currentX, int currentY, int targetX, int targetY, uint layerMask = uint.MaxValue)
            => MoveAll(new Point(currentX, currentY), new Point(targetX, targetY), layerMask);

        /// <inheritdoc />
        void ISpatialMap<T>.MoveAll(Point current, Point target) => MoveAll(current, target);

        /// <inheritdoc />
        void ISpatialMap<T>.MoveAll(int currentX, int currentY, int targetX, int targetY)
            => MoveAll(new Point(currentX, currentY), new Point(targetX, targetY));

        /// <summary>
        /// Moves all items that are on layers in <paramref name="layerMask" /> at the specified source location to the
        /// target location, updating their Position fields accordingly.  Returns false and moves nothing if one or more
        /// items cannot be moved or there are no items to be moved.
        /// </summary>
        /// <param name="current">Location to move items from.</param>
        /// <param name="target">Location to move items to.</param>
        /// <param name="layerMask">The layer mask to use to find items.</param>
        /// <returns>
        /// True if all items at <paramref name="current"/> on layers within the mask given were moved to
        /// <paramref name="target"/>; false otherwise.
        /// </returns>
        public bool TryMoveAll(Point current, Point target, uint layerMask = uint.MaxValue)
        {
            // It will be better to call MoveValid, rather than depend on the position updates, since this can save
            // an entire list's worth of allocation if the underlying map being moved is a MultiSpatialMap.
            //
            // LayeredSpatialMap implements MoveAll by checking if all items can move first, then moving them.
            // In order to save some enumeration, we'll replicate that functionality here but use MoveValid once we
            // know all items to can move, as an efficient way of generating the list of things we moved.
            //
            // This is also consistent with TryMoveAll being a function you call if failure is a potentially expected
            // outcome; by checking first, we delay allocation of any arrays or lists until we're sure we need to allocate
            // them.
            if (!_layeredSpatialMap.CanMoveAll(current, target, layerMask))
                return false;

            var moved = _layeredSpatialMap.MoveValid(current, target, layerMask);

            foreach (var item in moved)
                item.Position = target;

            return true;
        }

        /// <summary>
        /// Moves all items that are on layers in <paramref name="layerMask" /> at the specified source location to the
        /// target location, updating their Position fields accordingly.  Returns false and moves nothing if one or more
        /// items cannot be moved or there are no items to be moved.
        /// </summary>
        /// <param name="currentX">X-value of the location to move items from.</param>
        /// <param name="currentY">Y-value of the location to move items from.</param>
        /// <param name="targetX">X-value of the location to move items to.</param>
        /// <param name="targetY">Y-value of the location to move items to.</param>
        /// <param name="layerMask">The layer mask to use to find items.</param>
        /// <returns>
        /// True if all items at the current position on layers within the mask given were moved to
        /// the target position; false otherwise.
        /// </returns>
        public bool TryMoveAll(int currentX, int currentY, int targetX, int targetY, uint layerMask = uint.MaxValue)
            => TryMoveAll(new Point(currentX, currentY), new Point(targetX, targetY), layerMask);

        /// <inheritdoc />
        bool ISpatialMap<T>.TryMoveAll(Point current, Point target) => TryMoveAll(current, target);

        /// <inheritdoc />
        bool ISpatialMap<T>.TryMoveAll(int currentX, int currentY, int targetX, int targetY)
            => TryMoveAll(new Point(currentX, currentY), new Point(targetX, targetY));

        /// <summary>
        /// Moves all items that can be moved, that are at the given position and on any layer specified by the given layer
        /// mask, to the new position, updating their Position fields accordingly. If no layer mask is specified,
        /// defaults to all layers.
        /// </summary>
        /// <param name="current">Position to move all items from.</param>
        /// <param name="target">Position to move all items to.</param>
        /// <param name="layerMask">
        /// Layer mask specifying which layers to search for items on. Defaults to all layers.
        /// </param>
        /// <returns>All items moved.</returns>
        public List<T> MoveValid(Point current, Point target, uint layerMask = uint.MaxValue)
        {
            var list = _layeredSpatialMap.MoveValid(current, target, layerMask);
            foreach (var obj in list)
                obj.Position = target;

            return list;
        }

        /// <summary>
        /// Moves all items that can be moved, that are at the given position and on any layer specified by the given layer
        /// mask, to the new position, updating their Position fields accordingly. If no layer mask is specified,
        /// defaults to all layers.
        /// </summary>
        /// <param name="currentX">X-value of the position to move items from.</param>
        /// <param name="currentY">Y-value of the position to move items from.</param>
        /// <param name="targetX">X-value of the position to move items to.</param>
        /// <param name="targetY">Y-value of the position to move items from.</param>
        /// <param name="layerMask">
        /// Layer mask specifying which layers to search for items on. Defaults to all layers.
        /// </param>
        /// <returns>All items moved.</returns>
        public List<T> MoveValid(int currentX, int currentY, int targetX, int targetY, uint layerMask = uint.MaxValue)
            => MoveValid(new Point(currentX, currentY), new Point(targetX, targetY), layerMask);

        /// <summary>
        /// Moves all items that can be moved, that are at the given position and on any layer specified by the given layer
        /// mask, to the new position, updating their Position fields accordingly. If no layer mask is specified,
        /// defaults to all layers.
        /// </summary>
        /// <param name="current">Position to move items from.</param>
        /// <param name="target">Position to move items to.</param>
        /// <param name="itemsMovedOutput">List in which to place all moved items.</param>
        /// <param name="layerMask">
        /// Layer mask specifying which layers to search for items on. Defaults to all layers.
        /// </param>
        public void MoveValid(Point current, Point target, List<T> itemsMovedOutput, uint layerMask = uint.MaxValue)
        {
            int idx = itemsMovedOutput.Count;
            _layeredSpatialMap.MoveValid(current, target, itemsMovedOutput, layerMask);

            int count = itemsMovedOutput.Count;
            for (int i = idx; i < count; i++)
                itemsMovedOutput[i].Position = target;
        }

        /// <summary>
        /// Moves all items that can be moved, that are at the given position and on any layer specified by the given layer
        /// mask, to the new position, updating their Position fields accordingly. If no layer mask is specified,
        /// defaults to all layers.
        /// </summary>
        /// <param name="currentX">X-value of the position to move items from.</param>
        /// <param name="currentY">Y-value of the position to move items from.</param>
        /// <param name="targetX">X-value of the position to move items to.</param>
        /// <param name="targetY">Y-value of the position to move items from.</param>
        /// <param name="itemsMovedOutput">List in which to place all moved items.</param>
        /// <param name="layerMask">
        /// Layer mask specifying which layers to search for items on. Defaults to all layers.
        /// </param>
        public void MoveValid(int currentX, int currentY, int targetX, int targetY, List<T> itemsMovedOutput,
                              uint layerMask = uint.MaxValue)
            => MoveValid(new Point(currentX, currentY), new Point(targetX, targetY), itemsMovedOutput, layerMask);

        List<T> ISpatialMap<T>.MoveValid(Point current, Point target)
            => MoveValid(current, target);

        void ISpatialMap<T>.MoveValid(Point current, Point target, List<T> itemsMovedOutput)
            => MoveValid(current, target, itemsMovedOutput);

        List<T> ISpatialMap<T>.MoveValid(int currentX, int currentY, int targetX, int targetY)
            => MoveValid(new Point(currentX, currentY), new Point(targetX, targetY));

        void ISpatialMap<T>.MoveValid(int currentX, int currentY, int targetX, int targetY, List<T> itemsMovedOutput)
            => MoveValid(new Point(currentX, currentY), new Point(targetX, targetY), itemsMovedOutput);
        #endregion

        #region Contains
        /// <inheritdoc />
        public bool Contains(T item) => _layeredSpatialMap.Contains(item);

        /// <inheritdoc />
        bool IReadOnlySpatialMap<T>.Contains(Point position) => _layeredSpatialMap.Contains(position);

        bool IReadOnlySpatialMap<T>.Contains(int x, int y) => _layeredSpatialMap.Contains(x, y);

        /// <inheritdoc />
        public bool Contains(Point position, uint layerMask = uint.MaxValue) => _layeredSpatialMap.Contains(position, layerMask);

        /// <inheritdoc />
        public bool Contains(int x, int y, uint layerMask = uint.MaxValue) => _layeredSpatialMap.Contains(x, y, layerMask);
        #endregion

        #region Get Items/Positions/Layers
        /// <inheritdoc />
        IEnumerable<T> IReadOnlySpatialMap<T>.GetItemsAt(Point position) => _layeredSpatialMap.GetItemsAt(position);

        /// <inheritdoc />
        IEnumerable<T> IReadOnlySpatialMap<T>.GetItemsAt(int x, int y) => _layeredSpatialMap.GetItemsAt(x, y);

        /// <inheritdoc />
        public IEnumerable<T> GetItemsAt(Point position, uint layerMask = uint.MaxValue) => _layeredSpatialMap.GetItemsAt(position, layerMask);

        /// <inheritdoc />
        public IEnumerable<T> GetItemsAt(int x, int y, uint layerMask = uint.MaxValue) => _layeredSpatialMap.GetItemsAt(x, y, layerMask);

        /// <inheritdoc />
        public Point? GetPositionOfOrNull(T item) => _layeredSpatialMap.GetPositionOfOrNull(item);

        /// <inheritdoc />
        public bool TryGetPositionOf(T item, out Point position) => _layeredSpatialMap.TryGetPositionOf(item, out position);

        /// <inheritdoc />
        public Point GetPositionOf(T item) => _layeredSpatialMap.GetPositionOf(item);

        /// <inheritdoc />
        public IReadOnlySpatialMap<T> GetLayer(int layer) => _layeredSpatialMap.GetLayer(layer);

        /// <inheritdoc />
        public IEnumerable<IReadOnlySpatialMap<T>> GetLayersInMask(uint layerMask = uint.MaxValue)
            => _layeredSpatialMap.GetLayersInMask(layerMask);
        #endregion

        /// <summary>
        /// Returns a string representation of the spatial map.
        /// </summary>
        /// <returns>A string representation of the spatial map.</returns>
        public override string ToString() => _layeredSpatialMap.ToString();

        /// <summary>
        /// Returns a string representation of each item in the spatial map, with elements
        /// displayed in the specified way.
        /// </summary>
        /// <param name="elementStringifier">
        /// A function that takes an element of type T and produces the string that should
        /// represent it in the output.
        /// </param>
        /// <returns>A string representing each layer in the spatial map, with each element displayed in the specified way.</returns>
        public string ToString(Func<T, string> elementStringifier) => _layeredSpatialMap.ToString(elementStringifier);

        #region Clear/Remove

        /// <inheritdoc />
        public void Clear() => _layeredSpatialMap.Clear();

        /// <inheritdoc />
        public void Remove(T item) => _layeredSpatialMap.Remove(item);

        List<T> ISpatialMap<T>.Remove(Point position) => _layeredSpatialMap.Remove(position);

        List<T> ISpatialMap<T>.Remove(int x, int y) => _layeredSpatialMap.Remove(x, y);

        /// <inheritdoc />
        public bool TryRemove(T item) => _layeredSpatialMap.TryRemove(item);
        bool ISpatialMap<T>.TryRemove(Point position) => _layeredSpatialMap.TryRemove(position);

        bool ISpatialMap<T>.TryRemove(int x, int y) => _layeredSpatialMap.TryRemove(x, y);

        /// <summary>
        /// Removes all items at the specified location that are on any layer included in the given
        /// layer mask from the spatial map. Returns any items that were removed. Defaults to searching
        /// for items on all layers.
        /// </summary>
        /// <param name="position">Position to remove items from.</param>
        /// <param name="layerMask">
        /// The layer mask indicating which layers to search for items. Defaults to all layers.
        /// </param>
        /// <returns>Any items that were removed, or nothing if no items were removed.</returns>
        public List<T> Remove(Point position, uint layerMask = uint.MaxValue)
            => _layeredSpatialMap.Remove(position, layerMask);

        /// <summary>
        /// Removes all items at the specified location that are on any layer included in the given
        /// layer mask from the spatial map. Returns any items that were removed. Defaults to searching
        /// for items on all layers.
        /// </summary>
        /// <param name="x">X-value of the position to remove items from.</param>
        /// <param name="y">Y-value of the position to remove items from.</param>
        /// <param name="layerMask">
        /// The layer mask indicating which layers to search for items. Defaults to all layers.
        /// </param>
        /// <returns>Any items that were removed, or nothing if no items were removed.</returns>
        public List<T> Remove(int x, int y, uint layerMask = uint.MaxValue)
            => _layeredSpatialMap.Remove(x, y, layerMask);

        /// <summary>
        /// Attempts to remove all items at the specified location that are on any layer included in the given
        /// layer mask from the spatial map. Returns true if the items were successfully removed; false if one or more
        /// failed.
        /// </summary>
        /// <param name="position">Position to remove items from.</param>
        /// <param name="layerMask">
        /// The layer mask indicating which layers to search for items. Defaults to all layers.
        /// </param>
        /// <returns>True if the items were successfully removed; false otherwise</returns>
        public bool TryRemove(Point position, uint layerMask = uint.MaxValue)
            => _layeredSpatialMap.TryRemove(position, layerMask);

        /// <summary>
        /// Attempts to remove all items at the specified location that are on any layer included in the given
        /// layer mask from the spatial map. Returns true if the items were successfully removed; false if one or more
        /// failed.
        /// </summary>
        /// <param name="x">X-value of the position to remove items from.</param>
        /// <param name="y">Y-value of the position to remove items from.</param>
        /// <param name="layerMask">
        /// The layer mask indicating which layers to search for items. Defaults to all layers.
        /// </param>
        /// <returns>True if the items were successfully removed; false otherwise</returns>
        public bool TryRemove(int x, int y, uint layerMask = uint.MaxValue)
            => _layeredSpatialMap.TryRemove(x, y, layerMask);
        #endregion

        #region Item Handlers
        private void OnItemAdded(object? sender, ItemEventArgs<T> e)
        {
            e.Item.PositionChanging += ItemOnPositionChanging;
        }

        private void ItemOnPositionChanging(object? sender, ValueChangedEventArgs<Point> e)
        {
            if (sender != null)
                _layeredSpatialMap.Move((T)sender, e.NewValue);
        }

        private void OnItemRemoved(object? sender, ItemEventArgs<T> e)
        {
            e.Item.PositionChanging -= ItemOnPositionChanging;
        }
        #endregion
    }

    /// <summary>
    /// A version of <see cref="LayeredSpatialMap{T}"/> which takes items that implement <see cref="IPositionable"/>,
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
    /// <see cref="LayeredSpatialMap{T}"/> instead.
    /// </remarks>
    /// <typeparam name="T">The type of object that will be contained by this spatial map.</typeparam>
    public sealed class AutoSyncLayeredSpatialMap<T> : AutoSyncAdvancedLayeredSpatialMap<T>
        where T : class, IHasID, IPositionable, IHasLayer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="numberOfLayers">Number of layers to include.</param>
        /// <param name="customListPoolCreator">
        /// A function used to determine the list pool implementation used for the spatial maps which support multiple
        /// items in a location (if any).  The function takes the layer it is creating the pool for as a parameter.
        /// If no custom creator is specified, a ListPool is used.
        /// </param>
        /// <param name="pointComparer">
        /// Equality comparer to use for comparison and hashing of points, as object are added to/removed from/moved
        /// around the spatial map.  Be especially mindful of the efficiency of its GetHashCode function, as it will
        /// determine the efficiency of many AdvancedLayeredSpatialMap functions.  Defaults to the default equality
        /// comparer for Point, which uses a fairly efficient generalized hashing algorithm.
        /// </param>
        /// <param name="startingLayer">Index to use for the first layer.</param>
        /// <param name="layersSupportingMultipleItems">
        /// A layer mask indicating which layers should support multiple items residing at the same
        /// location on that layer. Defaults to no layers.
        /// </param>
        public AutoSyncLayeredSpatialMap(int numberOfLayers,
                                         Func<int, IListPool<T>>? customListPoolCreator = null,
                                         IEqualityComparer<Point>? pointComparer = null, int startingLayer = 0,
                                         uint layersSupportingMultipleItems = 0)
            : base(new IDComparer<T>(), numberOfLayers, customListPoolCreator, pointComparer, startingLayer,
                layersSupportingMultipleItems)
        { }
    }
}

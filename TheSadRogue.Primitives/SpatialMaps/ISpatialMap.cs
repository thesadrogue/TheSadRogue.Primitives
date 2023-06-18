using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SadRogue.Primitives.GridViews;

namespace SadRogue.Primitives.SpatialMaps
{
    /// <summary>
    /// Interface representing the common interface of a "spatial map", which is designed to be a convenient and efficient
    /// way to store items that reside on a grid and their locations.  If you're about to use a List to
    /// store all the objects on a grid, consider using one of the provided ISpatialMap implementation instead.
    /// </summary>
    /// <remarks>
    /// When representing objects considered to be located on a 2D grid, there are two main categories of use cases that
    /// seem to be encountered repeatedly.  The first is when you have a grid with well defined bounds, and intend to
    /// place or calculate one object or value for each location on the grid; the typical solution for this use case is
    /// to use an array to store the items (or an <see cref="ArrayView{T}"/>, which may be more convenient).
    ///
    /// The other use case is when you have either an unbounded grid, or more commonly a series of objects where there is
    /// not likely to be exactly one object per position.  This makes the array solution much less viable, since it
    /// would waste a lot of memory.  Storing a list of all the objects is another possible option; but while this is fast
    /// for iterating over all objects on the grid, it makes retrieving the object (or all objects) at a specific position
    /// very inefficient.  The traditional answer to that is something like Dictionary&lt;Point, T&gt;, but this can be
    /// complex to implement depending on the situation.  There are multiple constraints you might want to enforce; some
    /// instances might want to ensure that a maximum of only one object is associated with a given location, whereas
    /// others might want to allow _multiple_ objects at a location.  The latter is particularly troublesome to implement
    /// efficiently in a way that minimizes allocations.
    ///
    /// A "spatial map", therefore, is a data structure designed to implement position-to-object mappings like the ones
    /// described above simply and efficiently.  It acts as an abstraction over all of the above types of constraints
    /// and more; and the library provides multiple implementations of the interface which are suited to various use
    /// cases.
    ///
    /// Spatial maps allow you to add items at arbitrary positions, move items around, and remove them, all typically
    /// in constant time, rather than time proportional to the number of items stored.  They also provide linear-time
    /// iteration through all items in the structure (just like Dictionary does), although also like Dictionary, this
    /// iteration will usually not be as fast as iterating through the items in a List, even though it shares the
    /// same asymptotic characteristics.  Additionally, events are provided for when items are added, moved, and removed,
    /// which can allow you to respond to these as needed.
    ///
    /// One common use case is using spatial maps to store objects that already have a Position field of some sort.  In
    /// these cases, you will typically want to update the object's position in the spatial map whenever the object moves,
    /// so the spatial map's recorded position and the object's recorded position stay in sync.
    /// </remarks>
    /// <typeparam name="T">The type of object that will be contained by the spatial map.</typeparam>
    [PublicAPI]
    public interface ISpatialMap<T> : IReadOnlySpatialMap<T>
        where T : notnull
    {
        /// <summary>
        /// Tries to add the given item at the given position, and throws ArgumentException if the item cannot be added.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="position">Position to add item to.</param>
        void Add(T item, Point position);

        /// <summary>
        /// Tries to add the given item at the given position, and throws ArgumentException if the item cannot be added.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="x">X-value of the position to add item to.</param>
        /// <param name="y">Y-value of the position to add item to.</param>
        void Add(T item, int x, int y);

        /// <summary>
        /// Tries to add the given item at the given position.  Does nothing and returns false if the item cannot be added.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="position">Position to add item to.</param>
        /// <returns>True if the item was successfully added; false otherwise.</returns>
        bool TryAdd(T item, Point position);

        /// <summary>
        /// Tries to add the given item at the given position.  Does nothing and returns false if the item cannot be added.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="x">X-value of the position to add item to.</param>
        /// <param name="y">Y-value of the position to add item to.</param>
        /// <returns>True if the item was successfully added; false otherwise.</returns>
        bool TryAdd(T item, int x, int y);

        /// <summary>
        /// Clears all items out of the spatial map.
        /// </summary>
        void Clear();

        /// <summary>
        /// Moves the given item from its current location to the specified one. Throws ArgumentException if the item
        /// cannot be moved.
        /// </summary>
        /// <param name="item">Item to move.</param>
        /// <param name="target">Location to move item to.</param>
        void Move(T item, Point target);

        /// <summary>
        /// Moves the given item from its current location to the specified one. Throws ArgumentException if the item
        /// cannot be moved.
        /// </summary>
        /// <param name="item">Item to move</param>
        /// <param name="targetX">X-value of the location to move item to.</param>
        /// <param name="targetY">Y-value of the location to move item to.</param>
        void Move(T item, int targetX, int targetY);

        /// <summary>
        /// Attempts to move the given item from its current location to the specified one. Does nothing and returns
        /// false if the item cannot be moved to the given location.
        /// </summary>
        /// <param name="item">Item to move.</param>
        /// <param name="target">Location to move item to.</param>
        /// <returns>True if the item was moved; false if not.</returns>
        bool TryMove(T item, Point target);

        /// <summary>
        /// Attempts to move the given item from its current location to the specified one. Does nothing and returns
        /// false if the item cannot be moved to the given location.
        /// </summary>
        /// <param name="item">Item to move.</param>
        /// <param name="targetX">X-value of the location to move item to.</param>
        /// <param name="targetY">Y-value of the location to move item to.</param>
        /// <returns>True if the item was moved; false if not.</returns>
        bool TryMove(T item, int targetX, int targetY);

        /// <summary>
        /// Moves all items at the specified source location to the target location.  Throws ArgumentException if one or
        /// more items cannot be moved or there are no items to be moved.
        /// </summary>
        /// <param name="current">Location to move items from.</param>
        /// <param name="target">Location to move items to.</param>
        void MoveAll(Point current, Point target);

        /// <summary>
        /// Moves all items at the specified source location to the target location.  Returns false if one or
        /// more items cannot be moved or there are no items to be moved.
        /// </summary>
        /// <param name="current">Location to move items from.</param>
        /// <param name="target">Location to move items to.</param>
        /// <returns>True if all items at <paramref name="current"/> were moved to <paramref name="target"/>; false otherwise.</returns>
        bool TryMoveAll(Point current, Point target);

        /// <summary>
        /// Moves all items at the specified source location to the target location.  Throws ArgumentException if one or
        /// more items cannot be moved or there are no items to be moved.
        /// </summary>
        /// <param name="currentX">X-value of the location to move items from.</param>
        /// <param name="currentY">Y-value of the location to move items from.</param>
        /// <param name="targetX">X-value of the location to move items to.</param>
        /// <param name="targetY">Y-value of the location to move items to.</param>
        void MoveAll(int currentX, int currentY, int targetX, int targetY);

        /// <summary>
        /// Moves all items at the specified source location to the target location.  Returns false if one or
        /// more items cannot be moved or there are no items to be moved.
        /// </summary>
        /// <param name="currentX">X-value of the location to move items from.</param>
        /// <param name="currentY">Y-value of the location to move items from.</param>
        /// <param name="targetX">X-value of the location to move items to.</param>
        /// <param name="targetY">Y-value of the location to move items to.</param>
        /// <returns>True if all items at (currentX, currentY) were moved to (targetX, targetY); false otherwise.</returns>
        bool TryMoveAll(int currentX, int currentY, int targetX, int targetY);

        /// <summary>
        /// Moves all items at the specified source location that can be moved to the target location. Returns all items that were
        /// moved.
        /// </summary>
        /// <param name="current">Location to move items from.</param>
        /// <param name="target">Location to move items to.</param>
        /// <returns>All items that were moved, or an empty list if no items were moved.</returns>
        List<T> MoveValid(Point current, Point target);

        /// <summary>
        /// Moves all items at the specified source location that can be moved to the target location. Adds all items that were
        /// moved to the given list.
        /// </summary>
        /// <param name="current">Location to move items from.</param>
        /// <param name="target">Location to move items to.</param>
        /// <param name="itemsMovedOutput">A list to which all items successfully moved are added.</param>
        void MoveValid(Point current, Point target, List<T> itemsMovedOutput);

        /// <summary>
        /// Moves all items at the specified location that can be moved to the target one. Returns all items that were moved.
        /// </summary>
        /// <param name="currentX">X-value of the location to move items from.</param>
        /// <param name="currentY">Y-value of the location to move items from.</param>
        /// <param name="targetX">X-value of the location to move items to.</param>
        /// <param name="targetY">Y-value of the location to move items to.</param>
        /// <returns>All items that were moved, or nothing if no items were moved.</returns>
        List<T> MoveValid(int currentX, int currentY, int targetX, int targetY);

        /// <summary>
        /// Moves all items at the specified source location that can be moved to the target location. Adds all items that were
        /// moved to the given list.
        /// </summary>
        /// <param name="currentX">X-value of the location to move items from.</param>
        /// <param name="currentY">Y-value of the location to move items from.</param>
        /// <param name="targetX">X-value of the location to move items to.</param>
        /// <param name="targetY">Y-value of the location to move items to.</param>
        /// <param name="itemsMovedOutput">A list to which all items successfully moved are added.</param>
        void MoveValid(int currentX, int currentY, int targetX, int targetY, List<T> itemsMovedOutput);

        /// <summary>
        /// Removes the given item from the spatial map.  Throws ArgumentException if the item cannot be removed.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        void Remove(T item);

        /// <summary>
        /// Attempts to remove the given item from the spatial map.  Does nothing and return false if the item cannot be
        /// removed.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was successfully removed; false otherwise.</returns>
        bool TryRemove(T item);

        /// <summary>
        /// Removes all items at the specified location from the spatial map. Returns all items
        /// that were removed.
        /// </summary>
        /// <param name="position">Position to remove items from.</param>
        /// <returns>All items that were removed, or an empty list if no items were removed.</returns>
        List<T> Remove(Point position);

        /// <summary>
        /// Attempts to remove all items at the specified location from the spatial map. Returns true if the items
        /// were successfully removed; false if one or more failed.
        /// </summary>
        /// <param name="position">Position to remove items from.</param>
        /// <returns>True if the items were successfully removed; false otherwise</returns>
        bool TryRemove(Point position);

        /// <summary>
        /// Removes all items at the specified location from the spatial map. Returns all items
        /// that were removed.
        /// </summary>
        /// <param name="x">X-value of the position to remove items from.</param>
        /// <param name="y">Y-value of the position to remove items from.</param>
        /// <returns>All items that were removed, or nothing if no items were removed.</returns>
        List<T> Remove(int x, int y);

        /// <summary>
        /// Attempts to remove all items at the specified location from the spatial map. Returns true if the items
        /// were successfully removed; false if one or more failed.
        /// </summary>
        /// <param name="x">X-value of the position to remove items from.</param>
        /// <param name="y">Y-value of the position to remove items from.</param>
        /// <returns>True if the items were successfully removed; false otherwise</returns>
        bool TryRemove(int x, int y);
    }

    /// <summary>
    /// Event arguments for spatial map events pertaining to an item (<see cref="IReadOnlySpatialMap{T}.ItemAdded" />,
    /// <see cref="IReadOnlySpatialMap{T}.ItemRemoved" />, etc.)
    /// </summary>
    /// <typeparam name="T">Type of item.</typeparam>
    [PublicAPI]
    public class ItemEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="item">Item being represented.</param>
        /// <param name="position">Current position of the item.</param>
        public ItemEventArgs(T item, Point position)
        {
            Item = item;
            Position = position;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="item">Item being represented.</param>
        /// <param name="x">X-value of the current position of the item.</param>
        /// <param name="y">Y-value of the current position of the item.</param>
        public ItemEventArgs(T item, int x, int y)
            : this(item, new Point(x, y))
        { }

        /// <summary>
        /// Item being represented.
        /// </summary>
        public T Item { get; private set; }

        /// <summary>
        /// Current position of that item at time of event.
        /// </summary>
        public Point Position { get; private set; }
    }

    /// <summary>
    /// Event arguments for spatial maps <see cref="IReadOnlySpatialMap{T}.ItemMoved" /> event.
    /// </summary>
    /// <typeparam name="T">Type of item being stored.</typeparam>
    [PublicAPI]
    public class ItemMovedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="item">Item being represented.</param>
        /// <param name="oldPosition">Position of item before it was moved.</param>
        /// <param name="newPosition">Position of item after it has been moved.</param>
        public ItemMovedEventArgs(T item, Point oldPosition, Point newPosition)
        {
            Item = item;
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="item">Item being represented.</param>
        /// <param name="oldPositionX">X-value of the position of item before it was moved.</param>
        /// <param name="oldPositionY">Y-value of the position of item before it was moved.</param>
        /// <param name="newPositionX">X-value of the position of item after it has been moved.</param>
        /// <param name="newPositionY">Y-value of the position of item after it has been moved.</param>
        public ItemMovedEventArgs(T item, int oldPositionX, int oldPositionY, int newPositionX, int newPositionY)
            : this(item, new Point(oldPositionX, oldPositionY), new Point(newPositionX, newPositionY))
        { }

        /// <summary>
        /// Item being represented.
        /// </summary>
        public T Item { get; private set; }

        /// <summary>
        /// Position of item after it has been moved.
        /// </summary>
        public Point NewPosition { get; private set; }

        /// <summary>
        /// Position of item before it was moved.
        /// </summary>
        public Point OldPosition { get; private set; }
    }

    /// <summary>
    /// Class intended for comparing/hashing objects that implement <see cref="IHasID" />. Type T must be a
    /// reference type.
    /// </summary>
    /// <typeparam name="T">
    /// Type of object being compared. Type T must be a reference type that implements <see cref="IHasID" />.
    /// </typeparam>
    public class IDComparer<T> : IEqualityComparer<T> where T : class, IHasID
    {
        /// <summary>
        /// Equality comparison. Performs comparison via the object's <see cref="object.ReferenceEquals(object, object)" />
        /// function.
        /// </summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>True if the objects are considered equal, false otherwise.</returns>
        public bool Equals(T? x, T? y) => ReferenceEquals(x, y);

        /// <summary>
        /// Generates a hash based on the object's ID.GetHashCode() function.
        /// </summary>
        /// <param name="obj">Object to generate the hash for.</param>
        /// <returns>The hash of the object, based on its ID.</returns>
        public int GetHashCode(T obj) => obj.ID.GetHashCode();
    }
}

using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace SadRogue.Primitives.SpatialMaps
{
    /// <summary>
    /// A custom enumerator used to iterate over all positions at a given location in a layered spatial map implementation efficiently.
    ///
    /// Generally, you should use <see cref="IReadOnlyLayeredSpatialMap{T}.GetItemsAt(Point, uint)"/> (or one of its overloads)
    /// to get an instance of this, rather than creating one yourself.
    /// </summary>
    /// <remarks>
    /// This type is a struct, and as such is notably more efficient when used in a foreach loop than a function returning
    /// IEnumerable&lt;T&gt; by using "yield return".  This type does implement <see cref="IEnumerable{T}"/>,
    /// so you can pass it to functions which require one (for example, System.LINQ).  However, this will have reduced
    /// performance due to boxing of the iterator.
    /// </remarks>
    [PublicAPI]
    public struct ReadOnlyLayeredSpatialMapItemsAtEnumerator<T> : IEnumerable<T>, IEnumerator<T>
        where T : IHasLayer
    {
        private enum State
        {
            MultiSpatialMap,
            Generic,
            NextLayer,
            Done
        }

        // Suppress warning stating to use auto-property because we want to guarantee micro-performance
        // characteristics.
#pragma warning disable IDE0032 // Use auto property
        private T _current;
#pragma warning restore IDE0032 // Use auto property

        /// <summary>
        /// The current value for enumeration.
        /// </summary>
        public T Current => _current;

        private LayerMaskEnumerator _layerIdxEnumerator;
        private State _state;
        private readonly IReadOnlyLayeredSpatialMap<T> _map;
        private readonly Point _position;

        // We intentionally use instances of the structs instead of a single field of IEnumerable
        // in order to avoid boxing.
        private ListEnumerator<T> _multiSpatialEnumerator;
        private IEnumerator<T>? _genericEnumerator;

        object IEnumerator.Current => _current;

        /// <summary>
        /// Creates an enumerator which iterates over all items at the given point in the spatial map given, which are on layers in
        /// the given layer mask.
        /// </summary>
        /// <param name="map">The spatial map to check for items in.</param>
        /// <param name="position">The position to retrieve items at.</param>
        /// <param name="layerMask">The layer mask specifying layers to check.</param>
        public ReadOnlyLayeredSpatialMapItemsAtEnumerator(IReadOnlyLayeredSpatialMap<T> map, Point position, uint layerMask)
        {
            _map = map;
            _layerIdxEnumerator = map.LayerMasker.Layers(layerMask >> map.StartingLayer << map.StartingLayer);

            _current = default!; // Set in MoveNext; undefined behavior to access before MoveNext is called
            _multiSpatialEnumerator = default;
            _genericEnumerator = null;
            _position = position;

            _state = State.NextLayer;
        }

        /// <summary>
        /// Advances the iterator to the next item.
        /// </summary>
        /// <returns>True if the a new item at the position given within the specified layers; false otherwise.</returns>
        public bool MoveNext()
        {
            switch (_state)
            {
                case State.Done:
                    return false;
                case State.MultiSpatialMap:
                    if (_multiSpatialEnumerator.MoveNext())
                    {
                        _current = _multiSpatialEnumerator.Current;
                        return true;
                    }
                    goto case State.NextLayer;

                case State.Generic:
                    if (_genericEnumerator!.MoveNext())
                    {
                        _current = _genericEnumerator.Current;
                        return true;
                    }
                    goto case State.NextLayer;

                case State.NextLayer:
                    while (true) // Find layer
                    {
                        if (!_layerIdxEnumerator.MoveNext())
                        {
                            _state = State.Done;
                            return false;
                        }

                        var layer = _map.GetLayer(_layerIdxEnumerator.Current);
                        switch (layer)
                        {
                            case AdvancedSpatialMap<T> spatialMap:
                                if (spatialMap.TryGetItem(_position, out T val))
                                {
                                    _current = val;
                                    _state = State.NextLayer;
                                    return true;
                                }
                                break;
                            case AdvancedMultiSpatialMap<T> multiSpatialMap:
                                _multiSpatialEnumerator = multiSpatialMap.GetItemsAt(_position);
                                if (_multiSpatialEnumerator.MoveNext())
                                {
                                    _current = _multiSpatialEnumerator.Current;
                                    _state = State.MultiSpatialMap;
                                    return true;
                                }
                                break;
                            default:
                                _genericEnumerator = layer.GetItemsAt(_position).GetEnumerator();
                                if (_genericEnumerator.MoveNext())
                                {
                                    _current = _genericEnumerator.Current;
                                    _state = State.Generic;
                                    return true;
                                }
                                break;
                        }
                    }
            }

            // Unreachable
            return false;
        }

        /// <summary>
        /// Returns this enumerator.
        /// </summary>
        /// <returns>This enumerator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyLayeredSpatialMapItemsAtEnumerator<T> GetEnumerator() => this;

        // Explicitly implemented to ensure we prefer the non-boxing versions where possible
        #region Explicit Interface Implementations

        /// <inheritdoc />
        void IEnumerator.Reset()
        {
            ((IEnumerator)_layerIdxEnumerator).Reset();
            _state = State.NextLayer;
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        void IDisposable.Dispose()
        {
            _genericEnumerator?.Dispose();
            ((IDisposable)_layerIdxEnumerator).Dispose();
            _multiSpatialEnumerator.Dispose();
        }
        #endregion
    }

    /// <summary>
    /// Interface implementing only the read-only functions for <see cref="LayeredSpatialMap{T}" />/
    /// <see cref="AdvancedLayeredSpatialMap{T}" />.
    /// </summary>
    /// <typeparam name="T">
    /// Type of element stored in the layered spatial map -- must implement <see cref="IHasLayer" />.
    /// </typeparam>
    [PublicAPI]
    public interface IReadOnlyLayeredSpatialMap<T> : IReadOnlySpatialMap<T> where T : IHasLayer
    {
        /// <summary>
        /// Object used to get layer masks as they pertain to this spatial map.
        /// </summary>
        LayerMasker LayerMasker { get; }

        /// <summary>
        /// Gets read-only spatial maps representing each layer. To access a specific layer, instead
        /// use <see cref="GetLayer(int)" />.
        /// </summary>
        IEnumerable<IReadOnlySpatialMap<T>> Layers { get; }

        /// <summary>
        /// Gets the number of layers contained in the spatial map.
        /// </summary>
        int NumberOfLayers { get; }

        /// <summary>
        /// Starting index for layers contained in this spatial map.
        /// </summary>
        int StartingLayer { get; }

        /// <summary>
        /// Returns a read-only reference to the spatial map. Convenient for "safely" exposing the
        /// spatial map as a property.
        /// </summary>
        /// <returns>The current spatial map, as a "read-only" reference.</returns>
        new IReadOnlyLayeredSpatialMap<T> AsReadOnly();


        /// <summary>
        /// Returns whether or not there is an item in the spatial map at the given position that
        /// is on a layer included in the given layer mask. Defaults to searching on all layers.
        /// </summary>
        /// <param name="position">The position to check for.</param>
        /// <param name="layerMask">
        /// Layer mask that indicates which layers to check. Defaults to all layers.
        /// </param>
        /// <returns>
        /// True if there is some item at the given position on a layer included in the given layer
        /// mask, false if not.
        /// </returns>
        bool Contains(Point position, uint layerMask = uint.MaxValue);

        /// <summary>
        /// Returns whether or not there is an item in the data structure at the given position, that
        /// is on a layer included in the given layer mask.
        /// </summary>
        /// <param name="x">X-value of the position to check for.</param>
        /// <param name="y">Y-value of the position to check for.</param>
        /// <param name="layerMask">
        /// Layer mask that indicates which layers to check. Defaults to all layers.
        /// </param>
        /// <returns>
        /// True if there is some item at the given position on a layer included in the given layer
        /// mask, false if not.
        /// </returns>
        bool Contains(int x, int y, uint layerMask = uint.MaxValue);

        /// <summary>
        /// Gets the item(s) associated with the given position that reside on any layer included in
        /// the given layer mask. Returns nothing if there is nothing at that position on a layer
        /// included in the given layer mask.
        /// </summary>
        /// <param name="position">The position to return the item(s) for.</param>
        /// <param name="layerMask">
        /// Layer mask that indicates which layers to check. Defaults to all layers.
        /// </param>
        /// <returns>
        /// The item(s) at the given position that reside on a layer included in the layer mask if
        /// there are any items, or nothing if there is nothing at that position.
        /// </returns>
        ReadOnlyLayeredSpatialMapItemsAtEnumerator<T> GetItemsAt(Point position, uint layerMask = uint.MaxValue);

        /// <summary>
        /// Gets the item(s) associated with the given position that reside on any layer included in
        /// the given layer mask. Returns nothing if there is nothing at that position on a layer
        /// included in the given layer mask.
        /// </summary>
        /// <param name="x">X-value of the position to return the item(s) for.</param>
        /// <param name="y">Y-value of the position to return the item(s) for.</param>
        /// <param name="layerMask">
        /// Layer mask that indicates which layers to check. Defaults to all layers.
        /// </param>
        /// <returns>
        /// The item(s) at the given position that reside on a layer included in the layer mask if
        /// there are any items, or nothing if there is nothing at that position.
        /// </returns>
        ReadOnlyLayeredSpatialMapItemsAtEnumerator<T> GetItemsAt(int x, int y, uint layerMask = uint.MaxValue);

        /// <summary>
        /// Gets a read-only spatial map representing the layer specified.
        /// </summary>
        /// <param name="layer">The layer to retrieve.</param>
        /// <returns>The IReadOnlySpatialMap that represents the given layer.</returns>
        IReadOnlySpatialMap<T> GetLayer(int layer);

        /// <summary>
        /// Returns read-only spatial maps that represent each layer included in the given layer
        /// mask. Defaults to all layers.
        /// </summary>
        /// <param name="layerMask">
        /// Layer mask indicating which layers to return. Defaults to all layers.
        /// </param>
        /// <returns>Read-only spatial maps representing each layer in the given layer mask.</returns>
        IEnumerable<IReadOnlySpatialMap<T>> GetLayersInMask(uint layerMask = uint.MaxValue);

        /// <summary>
        /// Returns true if there are items at <paramref name="current" /> on one or more of the layers specified by the layer
        /// mask,
        /// and all items on those layers at that position can be moved to <paramref name="target" />; false otherwise.
        /// </summary>
        /// <param name="current">Location to move items from.</param>
        /// <param name="target">Location to move items to.</param>
        /// <param name="layerMask">Layer mask indicating which layers to check items on.</param>
        /// <returns>
        /// true if all items at the position current can be moved to the position target; false if one or more items
        /// cannot be moved or there are no items to move.
        /// </returns>
        public bool CanMoveAll(Point current, Point target, uint layerMask = uint.MaxValue);

        /// <summary>
        /// Returns true if there are items at the current position on one or more of the layers specified by the layer mask,
        /// and all items on those layers at that position can be moved to the target position; false otherwise.
        /// </summary>
        /// <param name="currentX">X-value of the location to move items from.</param>
        /// <param name="currentY">Y-value of the location to move items from.</param>
        /// <param name="targetX">X-value of the location to move items to.</param>
        /// <param name="targetY">Y-value of the location to move items to.</param>
        /// <param name="layerMask">Layer mask indicating which layers to check items on.</param>
        /// <returns>
        /// true if all items at the position current can be moved to the position target; false if one or more items
        /// cannot be moved or there are no items to move.
        /// </returns>
        public bool CanMoveAll(int currentX, int currentY, int targetX, int targetY, uint layerMask = uint.MaxValue);
    }
}

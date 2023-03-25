using System;
using System.Collections;
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
        /// Creates an enumerator which iterates over all items at the given point in the map given, which are on layers in
        /// the given layer mask.
        /// </summary>
        /// <param name="map">The spatial map to check for items in.</param>
        /// <param name="position">The position to retrieve items at.</param>
        /// <param name="layerMask">The layer mask specifying layers to check.</param>
        public ReadOnlyLayeredSpatialMapItemsAtEnumerator(IReadOnlyLayeredSpatialMap<T> map, Point position, uint layerMask)
        {
            _map = map;
            _layerIdxEnumerator = map.LayerMasker.Layers(layerMask);

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

        /// <summary>
        /// This iterator does not support resetting.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
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
}

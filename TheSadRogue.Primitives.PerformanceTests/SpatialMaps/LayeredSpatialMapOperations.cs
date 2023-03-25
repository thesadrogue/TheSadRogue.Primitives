using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using JetBrains.Annotations;
using SadRogue.Primitives;
using SadRogue.Primitives.SpatialMaps;

namespace TheSadRogue.Primitives.PerformanceTests.SpatialMaps
{
    /// <summary>
    /// An alternative custom enumerator for layered spatial map items at a position, that uses the IEnumerable
    /// interface to store the underlying iterator, instead of breaking it into separate fields.
    /// </summary>
    /// <typeparam name="T">Type of items stored in the spatial map.</typeparam>
    public struct ReadOnlyLayeredSpatialMapItemsAtEnumeratorInterface<T> : IEnumerable<T>, IEnumerator<T>
        where T : IHasLayer
    {
        // Suppress warning stating to use auto-property because we want to guarantee micro-performance
        // characteristics.
#pragma warning disable IDE0032 // Use auto property
        private T _current;
#pragma warning restore IDE0032 // Use auto property

        /// <summary>
        /// The current value for enumeration.
        /// </summary>
        public T Current => _current;

        private readonly IReadOnlyLayeredSpatialMap<T> _map;
        private LayerMaskEnumerator _layerIdxEnumerator;
        private IEnumerator<T>? _currentLayerEnumerator;
        private readonly Point _position;
        private int _state;

        object IEnumerator.Current => _current;

        public ReadOnlyLayeredSpatialMapItemsAtEnumeratorInterface(IReadOnlyLayeredSpatialMap<T> map, Point position, uint layerMask)
        {
            _map = map;
            _position = position;
            _layerIdxEnumerator = map.LayerMasker.Layers(layerMask);

            _currentLayerEnumerator = null;
            _state = 1; // Next layer
            _current = default!; // Set in MoveNext before use
        }

        public bool MoveNext()
        {
            switch (_state)
            {
                case 2: // Done
                    return false;
                case 0: // Current iterator
                    if (_currentLayerEnumerator!.MoveNext())
                    {
                        _current = _currentLayerEnumerator.Current;
                        return true;
                    }
                    _state = 1;
                    goto case 1;
                case 1: // Next layer
                    while (true) // Find layer
                    {
                        if (!_layerIdxEnumerator.MoveNext())
                        {
                            _state = 2; // Done
                            return false;
                        }

                        var layer = _map.GetLayer(_layerIdxEnumerator.Current);
                        _currentLayerEnumerator = layer.GetItemsAt(_position).GetEnumerator();

                        if (_currentLayerEnumerator.MoveNext())
                        {
                            _current = _currentLayerEnumerator.Current;
                            _state = 0; // Current iterator
                            return true;
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
        public ReadOnlyLayeredSpatialMapItemsAtEnumeratorInterface<T> GetEnumerator() => this;

        // Explicitly implemented to ensure we prefer the non-boxing versions where possible
        #region Explicit Interface Implementations

        /// <summary>
        /// This iterator does not support resetting.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        void IEnumerator.Reset()
        {
            ((IEnumerator)_layerIdxEnumerator).Reset();
            _state = 1; // Next layer
            _currentLayerEnumerator = null;
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        void IDisposable.Dispose()
        { }
        #endregion
    }

    /// <summary>
    /// A set of GetItemsAt implementations which use different implementation strategies.
    /// </summary>
    public static class CustomLayeredSpatialMapGetPositionsAtExtensions
    {
        public static ReadOnlyLayeredSpatialMapItemsAtEnumeratorInterface<T> GetItemsAtCustomEnumeratorInterface<T>(this IReadOnlyLayeredSpatialMap<T> map, Point position, uint layerMask)
            where T : IHasLayer
            => new(map, position, layerMask);

        public static IEnumerable<T> GetItemsAtYieldReturn<T>(this IReadOnlyLayeredSpatialMap<T> map, Point position,
            uint layerMask)
            where T : IHasLayer
        {
            foreach (int layerNumber in map.LayerMasker.Layers(layerMask))
                foreach (var item in map.GetLayer(layerNumber).GetItemsAt(position))
                    yield return item;
        }
    }

    /// <summary>
    /// Basic benchmarks for LayeredSpatialMap, roughly equivalent to the benchmarks for other spatial map implementations.
    /// </summary>
    /// <remarks>
    /// These benchmarks make all layers MultiSpatialMaps (eg. support multiple items), to represent a more or less
    /// "worst case" for memory usage performance of many functions, and secondarily to replicate what is found by
    /// default in GoRogue's Map.  There is some benefit to testing SpatialMap as well (to see how much the overhead
    /// of LayeredSpatialMap affects an underlying layer implementation that takes less overall time than MultiSpatialMap,
    /// however for simplicity, these tests ignore that case.
    /// </remarks>
    public class LayeredSpatialMapOperations
    {
        private readonly Point _initialPosition = (0, 1);
        private readonly Point _moveToPosition = (5, 6);
        private readonly Point _addPosition = (1, 1);
        private readonly IDLayerObject _addedObject = new(0);
        private readonly IDLayerObject _trackedObject = new(0);
        private readonly int _width = 10;
        private LayeredSpatialMap<IDLayerObject> _testMap = null!;

        [UsedImplicitly]
        [Params(1, 10, 50, 100)]
        public int NumEntities;

        [UsedImplicitly]
        [Params(1, 2, 3)]
        public int NumLayers;

        [GlobalSetup(Targets = new[]{ nameof(MoveTwice), nameof(TryMoveTwiceOriginal), nameof(TryMoveTwice), nameof(AddAndRemove), nameof(TryAddAndRemoveOriginal),
            nameof(TryAddAndRemove), nameof(GetItemsAt), nameof(GetItemsAtCustomEnumerator), nameof(GetItemsAtCustomEnumeratorInterface), nameof(GetItemsAtYieldReturn) })]
        public void GlobalSetupObjectsAtMoveToLocation()
        {
            _testMap = new LayeredSpatialMap<IDLayerObject>(NumLayers, layersSupportingMultipleItems: uint.MaxValue) { { _trackedObject, _initialPosition } };

            // Put other entities on the map (on each layer)
            for (int i = 0; i < NumLayers; i++)
            {
                int idx = -1;
                var layer = _testMap.GetLayer(i);
                while (layer.Count < NumEntities)
                {
                    idx += 1;
                    _testMap.Add(new IDLayerObject(i), Point.FromIndex(idx, _width));
                }
            }

        }

        [GlobalSetup(Targets = new[] { nameof(MoveAllTwice), nameof(MoveValidTwice), nameof(TryMoveAllTwice) })]
        public void GlobalSetupNoObjectsAtMoveToLocation()
        {
            _testMap = new LayeredSpatialMap<IDLayerObject>(NumLayers, layersSupportingMultipleItems: uint.MaxValue) { { _trackedObject, _initialPosition } };

            // Put other entities on the map, avoiding the starting point (on each layer)
            for (int i = 0; i < NumLayers; i++)
            {
                int idx = -1;
                var layer = _testMap.GetLayer(i);
                while (layer.Count < NumEntities)
                {
                    idx += 1;
                    var point = Point.FromIndex(idx, _width);
                    if (point != _moveToPosition)
                        _testMap.Add(new IDLayerObject(i), Point.FromIndex(idx, _width));
                }
            }
        }

        [Benchmark]
        public int MoveTwice()
        {
            _testMap.Move(_trackedObject, _moveToPosition);
            _testMap.Move(_trackedObject, _initialPosition); // Move it back to not spoil next benchmark
            return _testMap.Count; // Ensure nothing is optimized out
        }

        [Benchmark]
        public int TryMoveTwiceOriginal()
        {
            if (_testMap.CanMove(_trackedObject, _moveToPosition))
            {
                _testMap.Move(_trackedObject, _moveToPosition);
                _testMap.Move(_trackedObject, _initialPosition);
            }

            return _testMap.Count;
        }

        [Benchmark]
        public int TryMoveTwice()
        {
            _testMap.TryMove(_trackedObject, _moveToPosition);
            _testMap.TryMove(_trackedObject, _initialPosition);

            return _testMap.Count;
        }

        [Benchmark]
        public int MoveAllTwice()
        {
            _testMap.MoveAll(_initialPosition, _moveToPosition);
            // Move it back to not spoil next benchmark.  Valid since the GlobalSetup function used for this benchmark
            // doesn't put anything at _moveToPosition in the initial state.
            _testMap.MoveAll(_moveToPosition, _initialPosition);
            return _testMap.Count; // Ensure nothing is optimized out
        }

        [Benchmark]
        public int TryMoveAllTwice()
        {
            _testMap.TryMoveAll(_initialPosition, _moveToPosition);
            // Move it back to not spoil next benchmark.  Valid since the GlobalSetup function used for this benchmark
            // doesn't put anything at _moveToPosition in the initial state.
            _testMap.TryMoveAll(_moveToPosition, _initialPosition);
            return _testMap.Count; // Ensure nothing is optimized out
        }

        [Benchmark]
        public (List<IDLayerObject> l1, List<IDLayerObject> l2) MoveValidTwice()
        {
            var list1 = _testMap.MoveValid(_initialPosition, _moveToPosition);
            // Move it back to not spoil next benchmark.  Valid since the GlobalSetup function used for this benchmark
            // doesn't put anything at _moveToPosition in the initial state.
            var list2 = _testMap.MoveValid(_moveToPosition, _initialPosition);
            return (list1, list2); // Ensure nothing is optimized out
        }

        [Benchmark]
        public int AddAndRemove()
        {
            _testMap.Add(_addedObject, _addPosition);
            _testMap.Remove(_addedObject); // Must remove as well to avoid spoiling next invocation

            return _testMap.Count;
        }

        [Benchmark]
        public int TryAddAndRemoveOriginal()
        {
            if (_testMap.CanAdd(_addedObject, _addPosition))
            {
                _testMap.Add(_addedObject, _addPosition);
                _testMap.Remove(_addedObject);
            }

            return _testMap.Count;
        }

        [Benchmark]
        public int TryAddAndRemove()
        {
            _testMap.TryAdd(_addedObject, _addPosition);
            _testMap.TryRemove(_addedObject);

            return _testMap.Count;
        }

        [Benchmark]
        public uint GetItemsAt()
        {
            // Could use Consumer.Consume here, but this will stay consistent with other cases, which must _not_ use consume in order to avoid boxing
            uint sum = 0;
            foreach (var i in _testMap.GetItemsAt(_initialPosition))
                sum += i.ID;

            return sum;
        }

        [Benchmark]
        public uint GetItemsAtCustomEnumerator()
        {
            uint sum = 0;
            foreach (var i in new ReadOnlyLayeredSpatialMapItemsAtEnumerator<IDLayerObject>(_testMap, _initialPosition, uint.MaxValue))
                sum += i.ID;

            return sum;
        }

        [Benchmark]
        public uint GetItemsAtCustomEnumeratorInterface()
        {
            uint sum = 0;
            foreach (var i in _testMap.GetItemsAtCustomEnumeratorInterface(_initialPosition, uint.MaxValue))
                sum += i.ID;

            return sum;
        }

        [Benchmark]
        public uint GetItemsAtYieldReturn()
        {
            uint sum = 0;
            foreach (var i in _testMap.GetItemsAtYieldReturn(_initialPosition, uint.MaxValue))
                sum += i.ID;

            return sum;
        }
    }
}

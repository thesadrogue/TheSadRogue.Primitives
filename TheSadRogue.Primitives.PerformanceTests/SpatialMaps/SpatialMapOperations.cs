﻿using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using JetBrains.Annotations;
using SadRogue.Primitives;
using SadRogue.Primitives.SpatialMaps;

namespace TheSadRogue.Primitives.PerformanceTests.SpatialMaps
{
    /// <summary>
    /// A set of GetItemsAt implementations which use different implementation strategies.
    /// </summary>
    public static class CustomSpatialMapGetPositionsAtExtensions
    {
        public static IEnumerable<T> GetItemsAtYieldReturn<T>(this AdvancedSpatialMap<T> map, Point position)
            where T : notnull
        {
            if (map.TryGetItem(position, out T? val))
                yield return val;
        }
    }

    public class SpatialMapOperations
    {
        private readonly Point _initialPosition = (0, 1);
        private readonly Point _moveToPosition = (5, 6);
        private readonly Point _addPosition = (1, 1);
        private readonly IDObject _addedObject = new();
        private readonly IDObject _trackedObject = new();
        private readonly int _width = 10;
        private SpatialMap<IDObject> _testMap = null!;

        [UsedImplicitly]
        [Params(1, 10, 50, 100)]
        public int NumEntities;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _testMap = new SpatialMap<IDObject> { { _trackedObject, _initialPosition } };

            // Put other entities on the map, steering clear of the three points we need to remain clear to support
            // benchmarked adds/removes.
            int idx = -1;
            while (_testMap.Count < NumEntities)
            {
                idx += 1;

                if (idx == _initialPosition.ToIndex(_width) || idx == _moveToPosition.ToIndex(_width) || idx == _addPosition.ToIndex(_width)) continue;
                _testMap.Add(new IDObject(), Point.FromIndex(idx, _width));
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
        public (List<IDObject> l1, List<IDObject> l2) MoveValidTwice()
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
            foreach (var i in new SpatialMapItemsAtEnumerator<IDObject>(_testMap, _initialPosition))
                sum += i.ID;

            return sum;
        }

        [Benchmark]
        public uint GetItemsAtYieldReturn()
        {
            uint sum = 0;
            foreach (var i in _testMap.GetItemsAtYieldReturn(_initialPosition))
                sum += i.ID;

            return sum;
        }
    }
}

using BenchmarkDotNet.Attributes;
using JetBrains.Annotations;
using SadRogue.Primitives;
using SadRogue.Primitives.SpatialMaps;

namespace TheSadRogue.Primitives.PerformanceTests.SpatialMaps;

public class LayeredSpatialMapAutoSync
{
    private readonly Point _initialPosition = (0, 1);
        private readonly Point _moveToPosition = (5, 6);
        private readonly IDPositionLayerObject _trackedObject = new(layer: 0);
        private readonly int _width = 10;
        private AutoSyncLayeredSpatialMap<IDPositionLayerObject> _testMap = null!;

        [UsedImplicitly]
        [Params(1, 10, 50, 100)]
        public int NumEntities;

        [UsedImplicitly]
        [Params(1, 2, 3)]
        public int NumLayers;

        [GlobalSetup(Targets = new[] { nameof(MoveTwice), nameof(MoveTwiceUsingPositionField)})]
        public void GlobalSetupObjectsAtMoveToLocation()
        {
            _testMap = new AutoSyncLayeredSpatialMap<IDPositionLayerObject>(NumLayers, layersSupportingMultipleItems: uint.MaxValue) { { _trackedObject, _initialPosition } };

            // Put other entities on the map (on each layer)
            for (int i = 0; i < NumLayers; i++)
            {
                int idx = -1;
                var layer = _testMap.GetLayer(i);
                while (layer.Count < NumEntities)
                {
                    idx += 1;
                    _testMap.Add(new IDPositionLayerObject(i){Position = Point.FromIndex(idx, _width)});
                }
            }

        }

        [GlobalSetup(Targets = new[] { nameof(MoveAllTwice)})]
        public void GlobalSetupNoObjectsAtMoveToLocation()
        {
            _testMap = new AutoSyncLayeredSpatialMap<IDPositionLayerObject>(NumLayers, layersSupportingMultipleItems: uint.MaxValue) { { _trackedObject, _initialPosition } };

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
                        _testMap.Add(new IDPositionLayerObject(i){Position = Point.FromIndex(idx, _width)});
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
        public int MoveTwiceUsingPositionField()
        {
            _trackedObject.Position = _moveToPosition;
            _trackedObject.Position = _initialPosition; // Move it back to not spoil next benchmark
            return _testMap.Count; // Ensure nothing is optimized out
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
}

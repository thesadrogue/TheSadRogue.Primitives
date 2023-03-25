using BenchmarkDotNet.Attributes;
using JetBrains.Annotations;
using SadRogue.Primitives;
using SadRogue.Primitives.SpatialMaps;

namespace TheSadRogue.Primitives.PerformanceTests.SpatialMaps;

public class MultiSpatialMapAutoSync
{
    private readonly Point _initialPosition = (0, 1);
    private readonly Point _moveToPosition = (5, 6);

    private readonly IDPositionLayerObject _trackedLayerObject = new();

    private readonly int _width = 10;

    private AutoSyncMultiSpatialMap<IDPositionLayerObject> _testMap = null!;

    [UsedImplicitly]
    [Params(1, 10, 50, 100)]
    public int NumEntities;

    [GlobalSetup(Targets = new[] { nameof(MoveTwice), nameof(MoveTwiceUsingPositionField)})]
    public void GlobalSetupObjectsAtMoveToLocation()
    {
        _trackedLayerObject.Position = _initialPosition;
        _testMap = new AutoSyncMultiSpatialMap<IDPositionLayerObject> {  _trackedLayerObject  };

        // Put other entities on the map
        int idx = -1;
        while (_testMap.Count < NumEntities)
        {
            idx += 1;
            var obj = new IDPositionLayerObject { Position = Point.FromIndex(idx, _width) };
            _testMap.Add(obj);
        }
    }

    [GlobalSetup(Targets = new[] { nameof(MoveAllTwice)})]
    public void GlobalSetupNoObjectsAtMoveToLocation()
    {
        _trackedLayerObject.Position = _initialPosition;
        _testMap = new AutoSyncMultiSpatialMap<IDPositionLayerObject> { _trackedLayerObject };

        // Put other entities on the map, avoiding the starting point
        int idx = -1;
        while (_testMap.Count < NumEntities)
        {
            idx += 1;
            var point = Point.FromIndex(idx, _width);
            if (point != _moveToPosition)
                _testMap.Add(new IDPositionLayerObject {Position = point});
        }
    }

    [Benchmark]
    public int MoveTwice()
    {
        _testMap.Move(_trackedLayerObject, _moveToPosition);
        _testMap.Move(_trackedLayerObject, _initialPosition); // Move it back to not spoil next benchmark
        return _testMap.Count; // Ensure nothing is optimized out
    }

    [Benchmark]
    public int MoveTwiceUsingPositionField()
    {
        _trackedLayerObject.Position = _moveToPosition;
        _trackedLayerObject.Position = _initialPosition; // Move it back to not spoil next benchmark
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

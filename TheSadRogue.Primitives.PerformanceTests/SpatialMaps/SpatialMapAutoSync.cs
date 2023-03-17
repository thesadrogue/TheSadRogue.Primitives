using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;
using SadRogue.Primitives.SpatialMaps;

namespace TheSadRogue.Primitives.PerformanceTests.SpatialMaps;

public class SpatialMapAutoSync
{
    private readonly Point _initialPosition = (0, 1);
    private readonly Point _moveToPosition = (5, 6);
    private readonly Point _addPosition = (1, 1);
    private readonly IDPositionLayerObject _trackedLayerObject = new();
    private readonly int _width = 10;
    private AutoSyncSpatialMap<IDPositionLayerObject> _testMap = null!;

    [Params(1, 10, 50, 100)]
    public int NumEntities;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _testMap = new AutoSyncSpatialMap<IDPositionLayerObject> { { _trackedLayerObject, _initialPosition } };

        // Put other entities on the map, steering clear of the three points we need to remain clear to support
        // benchmarked adds/removes.
        int idx = -1;
        while (_testMap.Count < NumEntities)
        {
            idx += 1;

            if (idx == _initialPosition.ToIndex(_width) || idx == _moveToPosition.ToIndex(_width) || idx == _addPosition.ToIndex(_width)) continue;

            var obj = new IDPositionLayerObject { Position = Point.FromIndex(idx, _width) };
            _testMap.Add(obj);
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

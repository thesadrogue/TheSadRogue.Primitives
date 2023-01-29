using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests;

public class LineTests
{
    [ParamsSource(nameof(TestCases))]
    public (Point start, Point end) LineToDraw;

    private Consumer _consumer = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _consumer = new Consumer();
    }

    public static IEnumerable<(Point start, Point end)> TestCases()
    {
        yield return ((1, 1), (25, 50));
        yield return ((1, 1), (50, 25));
        yield return ((25, 50), (1, 1));
        yield return ((50, 25), (1, 1));
        // These lines have manhattan distance == chebyshev distance,  These cases are useful to help prove that
        // Orthogonal is about as fast as the others, if measured proportional to the appropriate distance calculation
        // (eg. the length of the line)
        yield return ((1, 1), (50, 1));
        yield return ((1, 1), (1, 25));
    }

    [Benchmark]
    [ArgumentsSource(nameof(GoRogueLineAlgorithms))]
    public void GetPointsOnLine(Lines.Algorithm algo)
        => Lines.GetLine(LineToDraw.start, LineToDraw.end, algo).Consume(_consumer);

    public static IEnumerable<Lines.Algorithm> GoRogueLineAlgorithms()
        => Enum.GetValues<Lines.Algorithm>();
}

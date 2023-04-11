using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using JetBrains.Annotations;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests;

public class CustomListEnumeratorTests
{
    [UsedImplicitly]
    [Params(5, 25, 50, 100)]
    public int Size;

    private List<int> _list = null!;
    private IReadOnlyList<int> _readOnlyList = null!;
    private IEnumerable<int> _iEnumerable = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _list = new List<int>(Size);
        for (int i = 0; i < Size; i++)
            _list.Add(i);

        _readOnlyList = _list;
        _iEnumerable = _list;
    }

    [Benchmark]
    public int SumInts()
    {
        int sum = 0;
        foreach (int item in _list)
            sum += item;

        return sum;
    }

    [Benchmark]
    public int SumIntsReadOnly()
    {
        int sum = 0;
        foreach (int item in _readOnlyList)
            sum += item;

        return sum;
    }

    [Benchmark]
    public int SumIntsIEnumerable()
    {
        int sum = 0;
        foreach (int item in _iEnumerable)
            sum += item;

        return sum;
    }

    [Benchmark]
    public int SumIntsCustomEnumerator()
    {
        int sum = 0;
        foreach (int item in new ListEnumerator<int>(_list))
            sum += item;

        return sum;
    }

    [Benchmark]
    public int SumIntsReadOnlyCustomEnumerator()
    {
        int sum = 0;
        foreach (int item in new ReadOnlyListEnumerator<int>(_readOnlyList))
            sum += item;

        return sum;
    }

    [Benchmark]
    public int SumIntsLinq()
        => _list.Sum();

    [Benchmark]
    public int SumIntsLinqCustomEnumerator()
        => new ListEnumerator<int>(_list).Sum();
}

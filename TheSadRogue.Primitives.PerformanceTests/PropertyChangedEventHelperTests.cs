using System;
using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests;

public class EventTestClass
{
    private int _changedValue;
    public int ChangedValue
    {
        get => _changedValue;
        set => this.SafelySetProperty(ref _changedValue, value, ChangedValueChanged);
    }
    public event EventHandler<ValueChangedEventArgs<int>>? ChangedValueChanged;

    private int _changingValue;
    public int ChangingValue
    {
        get => _changingValue;
        set => this.SafelySetProperty(ref _changingValue, value, ChangingValueChanging, ChangingValueChanged);
    }
    public event EventHandler<ValueChangedEventArgs<int>>? ChangingValueChanging;
    public event EventHandler<ValueChangedEventArgs<int>>? ChangingValueChanged;
}

public class PropertyChangedEventHelperTests
{
    private EventTestClass _eventTest = null!;
    private int _counter1;
    private int _counter2;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _eventTest = new EventTestClass();
        _counter1 = _counter2 = 1;
        _eventTest.ChangedValueChanged += (_, _) => _counter1++;
        _eventTest.ChangingValueChanging += (_, _) => _counter2++;

        _eventTest.ChangingValueChanged += (_, _) => _counter2++;
    }

    [Benchmark]
    public EventTestClass ChangedBasic()
    {
        _eventTest.ChangedValue = _counter1;
        return _eventTest;
    }

    [Benchmark]
    public EventTestClass ChangingAndChangedBasic()
    {
        _eventTest.ChangingValue = _counter2;
        return _eventTest;
    }
}

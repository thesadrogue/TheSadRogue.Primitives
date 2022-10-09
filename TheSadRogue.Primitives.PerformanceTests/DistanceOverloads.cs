using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests
{
    public class DistanceOverloads
    {
        [Params(4)]
        public int DeltaX;

        [Params(7)]
        public int DeltaY;

        [ParamsAllValues]
        public Distance.Types DistanceType;

        private Distance _distance = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _distance = DistanceType;
        }

        [Benchmark]
        public double CalculateDouble() => _distance.Calculate((double)DeltaX, (double)DeltaY);

        [Benchmark]
        public double CalculateInt() => _distance.Calculate(DeltaX, DeltaY);
    }
}

using System;
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

        //[ParamsAllValues]
        //public Distance.Types DistanceType;

        [Benchmark]
        public double ChebyshevDouble()
        {
            double dx = Math.Abs(DeltaX);
            double dy = Math.Abs(DeltaY);

            return Math.Max(dx, dy);
        }

        [Benchmark]
        public double ChebyshevDoubleNoNaN()
        {
            double dx = Math.Abs(DeltaX);
            double dy = Math.Abs(DeltaY);

            return dx > dy ? dx : dy;
        }

        [Benchmark]
        public double ChebyshevInt()
        {
            int dx = Math.Abs(DeltaX);
            int dy = Math.Abs(DeltaY);

            return Math.Max(dx, dy);
        }
    }
}

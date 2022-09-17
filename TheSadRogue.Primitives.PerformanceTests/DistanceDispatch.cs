using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Iced.Intel;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests
{
    public abstract class AbstractDistance
    {
        public static readonly DistanceChebyshev Chebyshev = new DistanceChebyshev();

        public static readonly DistanceEuclidean Euclidean = new DistanceEuclidean();

        public static readonly DistanceManhattan Manhattan = new DistanceManhattan();

        public abstract Distance.Types Type { get; }

        public abstract double Calculate(double dx, double dy);

        [Pure]
        public static implicit operator Distance.Types(AbstractDistance distance) => distance.Type;

        [Pure]
        public static implicit operator AbstractDistance(Distance.Types type) => type switch
        {
            Distance.Types.Manhattan => Manhattan,
            Distance.Types.Euclidean => Euclidean,
            Distance.Types.Chebyshev => Chebyshev,
            _ => throw new Exception($"Could not convert {nameof(Distance.Types)} to {nameof(Distance)} -- this is a bug!")
        };
    }

    public sealed class DistanceManhattan : AbstractDistance
    {
        public override Distance.Types Type => Distance.Types.Manhattan;

        public override double Calculate(double dx, double dy)
        {
            dx = Math.Abs(dx);
            dy = Math.Abs(dy);

            return dx + dy;
        }
    }

    public sealed class DistanceChebyshev : AbstractDistance
    {
        public override Distance.Types Type => Distance.Types.Chebyshev;

        public override double Calculate(double dx, double dy)
        {
            dx = Math.Abs(dx);
            dy = Math.Abs(dy);

            return Math.Max(dx, dy);
        }
    }

    public sealed class DistanceEuclidean : AbstractDistance
    {
        public override Distance.Types Type => Distance.Types.Euclidean;

        public override double Calculate(double dx, double dy)
        {
            dx = Math.Abs(dx);
            dy = Math.Abs(dy);

            return Math.Sqrt(dx * dx + dy * dy);
        }
    }


    public class DistanceDispatch
    {
        [Params(4)]
        public int DeltaX;

        [Params(7)]
        public int DeltaY;

        [ParamsAllValues]
        public Distance.Types DistanceType;

        private Distance _distance;

        private AbstractDistance _distanceAbstract;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _distance = DistanceType;
            _distanceAbstract = DistanceType;
        }

        [Benchmark]
        public double DistanceWithSwitch() => _distance.Calculate(DeltaX, DeltaY);

        [Benchmark]
        public double DistanceWithAbstract() => _distanceAbstract.Calculate(DeltaX, DeltaY);
    }
}

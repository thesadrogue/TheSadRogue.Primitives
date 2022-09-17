using System;
using System.Diagnostics.Contracts;
using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests
{
    public readonly struct SwitchExpressionDistance
    {
        public static readonly SwitchExpressionDistance Chebyshev = new(Distance.Types.Chebyshev);

        public static readonly SwitchExpressionDistance Euclidean = new(Distance.Types.Euclidean);

        public static readonly SwitchExpressionDistance Manhattan = new(Distance.Types.Manhattan);

        public readonly Distance.Types Type;


        private SwitchExpressionDistance(Distance.Types type) => Type = type;

        [Pure]
        public double Calculate(double dx, double dy) => Type switch
        {
            Distance.Types.Chebyshev => Math.Max(Math.Abs(dx),
                Math.Abs(dy)), // Radius is the longest axial distance
            Distance.Types.Manhattan => Math.Abs(dx) + Math.Abs(dy), // Simply manhattan distance
            Distance.Types.Euclidean => Math.Sqrt(dx * dx + dy * dy), // Spherical radius
            _ => throw new NotSupportedException(
                $"{nameof(Calculate)} does not support distance calculation {this}: this is a bug!")
        };

        [Pure]
        public static implicit operator SwitchExpressionDistance(Distance.Types type) => type switch
        {
            Distance.Types.Manhattan => Manhattan,
            Distance.Types.Euclidean => Euclidean,
            Distance.Types.Chebyshev => Chebyshev,
            _ => throw new Exception(
                $"Could not convert {nameof(Distance.Types)} to {nameof(Distance)} -- this is a bug!")
        };
    }

    public readonly struct SwitchStatementDistance
    {
        public static readonly SwitchStatementDistance Chebyshev = new(Distance.Types.Chebyshev);

        public static readonly SwitchStatementDistance Euclidean = new(Distance.Types.Euclidean);

        public static readonly SwitchStatementDistance Manhattan = new(Distance.Types.Manhattan);

        public readonly Distance.Types Type;


        private SwitchStatementDistance(Distance.Types type) => Type = type;

        [Pure]
        public double Calculate(double dx, double dy)
        {
            switch (Type)
            {
                case Distance.Types.Chebyshev:
                    return Math.Max(Math.Abs(dx), Math.Abs(dy));
                case Distance.Types.Euclidean:
                    return Math.Sqrt(dx * dx + dy * dy);
                case Distance.Types.Manhattan:
                    return Math.Abs(dx) + Math.Abs(dy);
                default:
                    return 0;
            }
        }

        [Pure]
        public static implicit operator SwitchStatementDistance(Distance.Types type) => type switch
        {
            Distance.Types.Manhattan => Manhattan,
            Distance.Types.Euclidean => Euclidean,
            Distance.Types.Chebyshev => Chebyshev,
            _ => throw new Exception(
                $"Could not convert {nameof(Distance.Types)} to {nameof(Distance)} -- this is a bug!")
        };
    }

    public readonly struct DistanceFunc
    {
        public static readonly DistanceFunc Chebyshev = new(Distance.Types.Chebyshev);

        public static readonly DistanceFunc Euclidean = new(Distance.Types.Euclidean);

        public static readonly DistanceFunc Manhattan = new(Distance.Types.Manhattan);

        public readonly Distance.Types Type;

        private readonly Func<double, double, double> _func;

        private DistanceFunc(Distance.Types type)
        {

            Type = type;
            _func = Type switch
            {
                Distance.Types.Chebyshev => ChebyshevDistance,
                Distance.Types.Manhattan => ManhattanDistance,
                Distance.Types.Euclidean => EuclideanDistance,
                _ => throw new Exception("Bad")
            };
        }

        [Pure]
        public double Calculate(double dx, double dy) => _func(dx, dy);

        private static double ChebyshevDistance(double dx, double dy) => Math.Max(Math.Abs(dx), Math.Abs(dy));
        private static double ManhattanDistance(double dx, double dy) => Math.Abs(dx) + Math.Abs(dy);
        private static double EuclideanDistance(double dx, double dy) => Math.Sqrt(dx * dx + dy * dy);

    }


    public class DistanceDispatch
    {
        [Params(4)]
        public int DeltaX;

        [Params(7)]
        public int DeltaY;

        [ParamsAllValues]
        public Distance.Types DistanceType;

        private Distance _distance = null!;
        private SwitchExpressionDistance _switchExpressionDistance;
        private SwitchStatementDistance _switchStatementDistance;
        private SwitchStatementDistance _funcDistance;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _distance = DistanceType;
            _switchExpressionDistance = DistanceType;
            _switchStatementDistance = DistanceType;
            _funcDistance = DistanceType;
        }

        [Benchmark]
        public double DistanceSwitchExpression() => _switchExpressionDistance.Calculate(DeltaX, DeltaY);

        [Benchmark]
        public double DistanceSwitchStatement() => _switchStatementDistance.Calculate(DeltaX, DeltaY);

        [Benchmark]
        public double DistanceFunc() => _funcDistance.Calculate(DeltaX, DeltaY);

        [Benchmark]
        public double Distance() => _distance.Calculate(DeltaX, DeltaY);
    }
}

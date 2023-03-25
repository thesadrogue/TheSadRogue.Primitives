using System;
using BenchmarkDotNet.Attributes;
using JetBrains.Annotations;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests
{
    public readonly struct SwitchExpressionDistance
    {
        private static readonly SwitchExpressionDistance s_chebyshev = new(Distance.Types.Chebyshev);

        private static readonly SwitchExpressionDistance s_euclidean = new(Distance.Types.Euclidean);

        private static readonly SwitchExpressionDistance s_manhattan = new(Distance.Types.Manhattan);

        private readonly Distance.Types _type;


        private SwitchExpressionDistance(Distance.Types type) => _type = type;

        [System.Diagnostics.Contracts.Pure]
        public double Calculate(double dx, double dy) => _type switch
        {
            Distance.Types.Chebyshev => Math.Max(Math.Abs(dx),
                Math.Abs(dy)), // Radius is the longest axial distance
            Distance.Types.Manhattan => Math.Abs(dx) + Math.Abs(dy), // Simply manhattan distance
            Distance.Types.Euclidean => Math.Sqrt(dx * dx + dy * dy), // Spherical radius
            _ => throw new NotSupportedException(
                $"{nameof(Calculate)} does not support distance calculation {this}: this is a bug!")
        };

        [System.Diagnostics.Contracts.Pure]
        public static implicit operator SwitchExpressionDistance(Distance.Types type) => type switch
        {
            Distance.Types.Manhattan => s_manhattan,
            Distance.Types.Euclidean => s_euclidean,
            Distance.Types.Chebyshev => s_chebyshev,
            _ => throw new Exception(
                $"Could not convert {nameof(Distance.Types)} to {nameof(Distance)} -- this is a bug!")
        };
    }

    public readonly struct SwitchStatementDistance
    {
        private static readonly SwitchStatementDistance s_chebyshev = new(Distance.Types.Chebyshev);

        private static readonly SwitchStatementDistance s_euclidean = new(Distance.Types.Euclidean);

        private static readonly SwitchStatementDistance s_manhattan = new(Distance.Types.Manhattan);

        private readonly Distance.Types _type;


        private SwitchStatementDistance(Distance.Types type) => _type = type;

        [System.Diagnostics.Contracts.Pure]
        public double Calculate(double dx, double dy)
        {
            switch (_type)
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

        [System.Diagnostics.Contracts.Pure]
        public static implicit operator SwitchStatementDistance(Distance.Types type) => type switch
        {
            Distance.Types.Manhattan => s_manhattan,
            Distance.Types.Euclidean => s_euclidean,
            Distance.Types.Chebyshev => s_chebyshev,
            _ => throw new Exception(
                $"Could not convert {nameof(Distance.Types)} to {nameof(Distance)} -- this is a bug!")
        };
    }

    public readonly struct DistanceFunc
    {
        private static readonly DistanceFunc s_chebyshev = new(Distance.Types.Chebyshev);

        private static readonly DistanceFunc s_euclidean = new(Distance.Types.Euclidean);

        private static readonly DistanceFunc s_manhattan = new(Distance.Types.Manhattan);

        private readonly Func<double, double, double> _func;

        private DistanceFunc(Distance.Types type)
        {
            _func = type switch
            {
                Distance.Types.Chebyshev => ChebyshevDistance,
                Distance.Types.Manhattan => ManhattanDistance,
                Distance.Types.Euclidean => EuclideanDistance,
                _ => throw new Exception("Bad")
            };
        }

        [System.Diagnostics.Contracts.Pure]
        public static implicit operator DistanceFunc(Distance.Types type) => type switch
        {
            Distance.Types.Manhattan => s_manhattan,
            Distance.Types.Euclidean => s_euclidean,
            Distance.Types.Chebyshev => s_chebyshev,
            _ => throw new Exception(
                $"Could not convert {nameof(Distance.Types)} to {nameof(Distance)} -- this is a bug!")
        };

        [System.Diagnostics.Contracts.Pure]
        public double Calculate(double dx, double dy) => _func(dx, dy);

        private static double ChebyshevDistance(double dx, double dy) => Math.Max(Math.Abs(dx), Math.Abs(dy));
        private static double ManhattanDistance(double dx, double dy) => Math.Abs(dx) + Math.Abs(dy);
        private static double EuclideanDistance(double dx, double dy) => Math.Sqrt(dx * dx + dy * dy);

    }


    public class DistanceDispatch
    {
        [UsedImplicitly]
        [Params(4)]
        public int DeltaX;

        [UsedImplicitly]
        [Params(7)]
        public int DeltaY;

        [UsedImplicitly]
        [ParamsAllValues]
        public Distance.Types DistanceType;

        private Distance _distance = null!;
        private SwitchExpressionDistance _switchExpressionDistance;
        private SwitchStatementDistance _switchStatementDistance;
        private DistanceFunc _funcDistance;

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

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
            Distance.Types.Euclidean => Math.Sqrt(dx * dx + dy * dy) // Spherical radius
        };


        [Pure]
        public double Calculate(int dx, int dy) => Type switch
        {
            Distance.Types.Chebyshev => Math.Max(Math.Abs(dx),
                Math.Abs(dy)), // Radius is the longest axial distance
            Distance.Types.Manhattan => Math.Abs(dx) + Math.Abs(dy), // Simply manhattan distance
            Distance.Types.Euclidean => Math.Sqrt(dx * dx + dy * dy) // Spherical radius
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
        public double Calculate(int dx, int dy)
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

    public readonly struct IfStatementDistance
    {
        public static readonly IfStatementDistance Chebyshev = new(Distance.Types.Chebyshev);

        public static readonly IfStatementDistance Euclidean = new(Distance.Types.Euclidean);

        public static readonly IfStatementDistance Manhattan = new(Distance.Types.Manhattan);

        public readonly Distance.Types Type;


        private IfStatementDistance(Distance.Types type) => Type = type;

        [Pure]
        public double Calculate(double dx, double dy)
        {
            if (Type == Distance.Types.Chebyshev)
                return Math.Max(Math.Abs(dx), Math.Abs(dy));
            if (Type == Distance.Types.Euclidean)
                return Math.Sqrt(dx * dx + dy * dy);

            return Math.Abs(dx) + Math.Abs(dy);
        }

        [Pure]
        public double Calculate(int dx, int dy)
        {
            if (Type == Distance.Types.Chebyshev)
                return Math.Max(Math.Abs(dx), Math.Abs(dy));
            if (Type == Distance.Types.Euclidean)
                return Math.Sqrt(dx * dx + dy * dy);

            return Math.Abs(dx) + Math.Abs(dy);
        }

        [Pure]
        public static implicit operator IfStatementDistance(Distance.Types type) => type switch
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
        private readonly Func<int, int, double> _funcInt;

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

            _funcInt = Type switch
            {
                Distance.Types.Chebyshev => ChebyshevDistance,
                Distance.Types.Manhattan => ManhattanDistance,
                Distance.Types.Euclidean => EuclideanDistance,
                _ => throw new Exception("Bad")
            };
        }

        [Pure]
        public double Calculate(double dx, double dy) => _func(dx, dy);

        [Pure]
        public double Calculate(int dx, int dy) => _funcInt(dx, dy);

        private static double ChebyshevDistance(double dx, double dy) => Math.Max(Math.Abs(dx), Math.Abs(dy));
        private static double ChebyshevDistance(int dx, int dy) => Math.Max(Math.Abs(dx), Math.Abs(dy));
        private static double ManhattanDistance(double dx, double dy) => Math.Abs(dx) + Math.Abs(dy);
        private static double ManhattanDistance(int dx, int dy) => Math.Abs(dx) + Math.Abs(dy);
        private static double EuclideanDistance(double dx, double dy) => Math.Sqrt(dx * dx + dy * dy);
        private static double EuclideanDistance(int dx, int dy) => Math.Sqrt(dx * dx + dy * dy);

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
        private SwitchStatementDistance _ifStatementDistance;
        private SwitchStatementDistance _funcDistance;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _distance = DistanceType;
            _switchExpressionDistance = DistanceType;
            _switchStatementDistance = DistanceType;
            _ifStatementDistance = DistanceType;
            _funcDistance = DistanceType;
        }

        [Benchmark]
        public double DistanceSwitchExpression() => _switchExpressionDistance.Calculate((double)DeltaX, DeltaY);

        [Benchmark]
        public double DistanceSwitchExpressionInt() => _switchExpressionDistance.Calculate(DeltaX, DeltaY);

        [Benchmark]
        public double DistanceSwitchStatement() => _switchStatementDistance.Calculate((double)DeltaX, DeltaY);

        [Benchmark]
        public double DistanceSwitchStatementInt() => _switchStatementDistance.Calculate(DeltaX, DeltaY);

        [Benchmark]
        public double DistanceIfStatement() => _ifStatementDistance.Calculate((double)DeltaX, DeltaY);

        [Benchmark]
        public double DistanceIfStatementInt() => _ifStatementDistance.Calculate(DeltaX, DeltaY);

        [Benchmark]
        public double DistanceFunc() => _funcDistance.Calculate((double)DeltaX, DeltaY);

        [Benchmark]
        public double DistanceFuncInt() => _funcDistance.Calculate(DeltaX, DeltaY);

        [Benchmark]
        public double Distance() => _distance.Calculate((double)DeltaX, DeltaY);

        [Benchmark]
        public double DistanceInt() => _distance.Calculate(DeltaX, DeltaY);
    }
}

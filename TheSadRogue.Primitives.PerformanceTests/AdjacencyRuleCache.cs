using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests
{
    /// <summary>
    /// Performance tests demonstrating the benefit of keeping cached versions of DirectionsOfNeighbors type functions
    /// in AdjacencyRule
    /// </summary>
    public class AdjacencyRuleCache
    {
        [ParamsAllValues]
        public AdjacencyRule.Types RuleType;

        private AdjacencyRule _rule;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _rule = RuleType;
        }

        [Benchmark]
        public int NeighborsViaEnumerable()
        {
            int sum = 0;
            foreach (var dir in _rule.DirectionsOfNeighbors())
                sum += (int)dir.Type;

            return sum;
        }

        [Benchmark]
        public int NeighborsViaCache()
        {
            int sum = 0;
            for (int i = 0; i < _rule.DirectionsOfNeighborsCache.Length; i++)
                sum += (int)_rule.DirectionsOfNeighborsCache[i].Type;

            return sum;
        }

        [Benchmark]
        public int NeighborsClockwiseViaEnumerable()
        {
            int sum = 0;
            foreach (var dir in _rule.DirectionsOfNeighborsClockwise())
                sum += (int)dir.Type;

            return sum;
        }

        [Benchmark]
        public int NeighborsClockwiseViaCache()
        {
            int sum = 0;
            for (int i = 0; i < _rule.DirectionsOfNeighborsClockwiseCache.Length; i++)
                sum += (int)_rule.DirectionsOfNeighborsClockwiseCache[i].Type;

            return sum;
        }

        [Benchmark]
        public int NeighborsCounterClockwiseViaEnumerable()
        {
            int sum = 0;
            foreach (var dir in _rule.DirectionsOfNeighborsCounterClockwise())
                sum += (int)dir.Type;

            return sum;
        }

        [Benchmark]
        public int NeighborsCounterClockwiseViaCache()
        {
            int sum = 0;
            for (int i = 0; i < _rule.DirectionsOfNeighborsCounterClockwiseCache.Length; i++)
                sum += (int)_rule.DirectionsOfNeighborsCounterClockwiseCache[i].Type;

            return sum;
        }
    }
}

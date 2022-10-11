using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace TheSadRogue.Primitives.PerformanceTests
{
    /// <summary>
    /// Defines two configurations; "default" which is the default mode, and
    /// "Dynamic PGO", which will enable .NET 6's Dynamic PGO feature for the tests.
    /// </summary>
    internal class DynamicPGOConfig : ManualConfig
    {
        public DynamicPGOConfig()
        {
            // Use .NET 6.0 default mode:
            AddJob(Job.Default.WithId("Default mode"));

            // Use Dynamic PGO mode:
            AddJob(Job.Default.WithId("Dynamic PGO")
                .WithEnvironmentVariables(
                    new EnvironmentVariable("DOTNET_TieredPGO", "1"),
                    new EnvironmentVariable("DOTNET_TC_QuickJitForLoops", "1"),
                    new EnvironmentVariable("DOTNET_ReadyToRun", "0")));
        }
    }
}

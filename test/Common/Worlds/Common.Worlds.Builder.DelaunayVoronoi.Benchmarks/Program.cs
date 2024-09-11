using BenchmarkDotNet.Running;
using Common.Worlds.Builder.DelaunayVoronoi.Benchmarks;

#pragma warning disable CA1852  // Seal internal types
BenchmarkRunner.Run<BuilderBenchmarks>();
#pragma warning restore CA1852

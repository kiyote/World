using BenchmarkDotNet.Running;
using Common.Worlds.Builder.DelaunayVoronoi.Benchmarks;

#pragma warning disable CA1812 // CA1812 does not understand top-level statements
#pragma warning disable CA1852
BenchmarkRunner.Run<BuilderBenchmarks>();
#pragma warning restore CA1852
#pragma warning restore CA1812

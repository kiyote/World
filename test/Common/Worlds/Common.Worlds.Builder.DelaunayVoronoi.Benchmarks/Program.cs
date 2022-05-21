using BenchmarkDotNet.Running;
using Common.Worlds.Builder.DelaunayVoronoi.Benchmarks;

#pragma warning disable CA1812
BenchmarkRunner.Run<BuilderBenchmarks>();
#pragma warning restore CA1812

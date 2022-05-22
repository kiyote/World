using BenchmarkDotNet.Running;
using Common.Worlds.Builder.DelaunayVoronoi.Benchmarks;

#pragma warning disable CA1812 // CA1812 does not understand top-level statements
BenchmarkRunner.Run<BuilderBenchmarks>();
#pragma warning restore CA1812

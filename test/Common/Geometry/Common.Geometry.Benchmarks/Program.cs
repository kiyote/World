using BenchmarkDotNet.Running;
using Common.Geometry.Benchmarks;

#pragma warning disable CA1812
#pragma warning disable CA1852
BenchmarkRunner.Run<GeometryBenchmarks>();
#pragma warning restore CA1852
#pragma warning restore CA1812

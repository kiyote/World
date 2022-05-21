using BenchmarkDotNet.Running;
using Common.Geometry.Benchmarks;

#pragma warning disable CA1812
BenchmarkRunner.Run<GeometryBenchmarks>();
#pragma warning restore CA1812

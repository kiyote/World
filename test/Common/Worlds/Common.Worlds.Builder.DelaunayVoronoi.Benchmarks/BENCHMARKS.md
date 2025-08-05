Size: 500x500, CellSize: 20, Distance: 5

```
BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.5247/22H2/2022Update)
Intel Core i7-9700K CPU 3.60GHz (Coffee Lake), 1 CPU, 8 logical and 8 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 8.0.12 (8.0.1224.60305), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.12 (8.0.1224.60305), X64 RyuJIT AVX2
```

| Method            | Mean         | Error       | StdDev      | Allocated    |
|------------------ |-------------:|------------:|------------:|-------------:|
| VoronoiBuilder    |     840.1 us |    15.75 us |    13.96 us |    844.75 KB |
| LandformBuilder   | 147,077.7 us | 2,878.99 us | 4,396.52 us | 157842.05 KB |
| SaltwaterBuilder  |  53,910.1 us |   373.13 us |   330.77 us |    1691.2 KB |
| FreshwaterBuilder |   1,276.1 us |     6.03 us |     4.71 us |     33.71 KB |
| LakeBuilder       |     520.5 us |     7.88 us |     6.58 us |    306.84 KB |
| CoastBuilder      |   2,825.7 us |    54.34 us |    48.17 us |    401.87 KB |

|            Method |     Mean |    Error |   StdDev | Allocated |
|------------------ |---------:|---------:|---------:|----------:|
| DelaunatorFactory | 38.50 ms | 0.166 ms | 0.138 ms |     70 MB |
|    VoronoiBuilder | 46.40 ms | 0.154 ms | 0.144 ms |     71 MB |

Add IPointFactory to VoronoiBuilder
|            Method |     Mean |    Error |   StdDev | Allocated |
|------------------ |---------:|---------:|---------:|----------:|
| DelaunatorFactory | 38.87 ms | 0.632 ms | 0.591 ms |     70 MB |
|    VoronoiBuilder | 41.55 ms | 0.606 ms | 0.567 ms |     71 MB |

With fixed DelaunatorFactory
|            Method |       Mean |    Error |   StdDev | Allocated |
|------------------ |-----------:|---------:|---------:|----------:|
| DelaunatorFactory |   336.5 us |  0.55 us |  0.49 us |    214 KB |
|    VoronoiBuilder | 2,897.2 us | 27.55 us | 25.77 us |  1,389 KB |

Measure LandformBuilder
|            Method |        Mean |    Error |   StdDev | Allocated |
|------------------ |------------:|---------:|---------:|----------:|
| DelaunatorFactory |    338.2 us |  6.58 us |  6.16 us |    214 KB |
|    VoronoiBuilder |  2,884.2 us |  8.88 us |  7.87 us |  1,389 KB |
|   LandformBuilder | 11,069.3 us | 70.90 us | 62.86 us |  1,856 KB |


|              Method |      Mean |     Error |    StdDev | Allocated |
|-------------------- |----------:|----------:|----------:|----------:|
| PointFactory_Random |  1.638 ms | 0.0047 ms | 0.0042 ms |     31 KB |
|   DelaunatorFactory | 37.575 ms | 0.1125 ms | 0.0997 ms | 71,793 KB |

Fixing distinct input processing bug
|              Method |       Mean |    Error |  StdDev | Allocated |
|-------------------- |-----------:|---------:|--------:|----------:|
| PointFactory_Random | 1,646.2 us | 10.27 us | 9.10 us |     31 KB |
|   DelaunatorFactory |   334.7 us |  1.66 us | 1.47 us |    214 KB |

Adding some more benchmarks
|              Method |       Mean |   Error |  StdDev |   Allocated |
|-------------------- |-----------:|--------:|--------:|------------:|
| PointFactory_Random | 1,579.4 us | 2.90 us | 2.42 us |    32,057 B |
|   DelaunatorFactory |   334.2 us | 1.24 us | 1.16 us |   219,240 B |
|      VoronoiFactory |   937.8 us | 7.16 us | 6.35 us | 1,171,321 B |
|      PointInPolygon |   111.5 us | 0.98 us | 0.82 us |           - |

Deduping in VoronoiFactory during add instead of Distinct()
|              Method |       Mean |    Error |  StdDev | Allocated |
|-------------------- |-----------:|---------:|--------:|----------:|
| PointFactory_Random | 1,585.3 us | 10.46 us | 9.28 us |  32,057 B |
|   DelaunatorFactory |   340.7 us |  4.61 us | 4.53 us | 219,368 B |
|      VoronoiFactory |   692.9 us |  3.14 us | 2.94 us | 802,801 B |
|      PointInPolygon |   111.3 us |  0.23 us | 0.18 us |         - |

Removing some duplication
|              Method |        Mean |     Error |    StdDev | Allocated |
|-------------------- |------------:|----------:|----------:|----------:|
| PointFactory_Random |  1,587.1 us |   8.59 us |   8.04 us |  32,057 B |
|   DelaunatorFactory |    336.8 us |   1.17 us |   1.04 us | 219,360 B |
|      VoronoiFactory | 73,559.8 us | 678.94 us | 601.86 us | 984,743 B |
|      PointInPolygon |    113.6 us |   2.10 us |   2.16 us |         - |

Changing to using Distinct in MakeEdges
|              Method |        Mean |    Error |   StdDev |   Allocated |
|-------------------- |------------:|---------:|---------:|------------:|
| PointFactory_Random |  1,625.1 us |  4.42 us |  4.13 us |    32,057 B |
|   DelaunatorFactory |    332.5 us |  2.71 us |  2.53 us |   219,248 B |
|      VoronoiFactory | 19,765.6 us | 43.88 us | 38.89 us | 1,054,095 B |
|      PointInPolygon |    112.6 us |  0.70 us |  0.62 us |           - |

Allowing duplicate circumcenters
|              Method |       Mean |   Error |  StdDev | Allocated |
|-------------------- |-----------:|--------:|--------:|----------:|
| PointFactory_Random | 1,582.7 us | 2.94 us | 2.60 us |  32,057 B |
|   DelaunatorFactory |   329.2 us | 0.77 us | 0.72 us | 219,328 B |
|      VoronoiFactory |   690.4 us | 6.22 us | 5.82 us | 802,417 B |
|      PointInPolygon |   111.1 us | 0.16 us | 0.13 us |         - |

Merging MakeNeighbourMap and MakeNeighbours
|              Method |       Mean |   Error |  StdDev | Allocated |
|-------------------- |-----------:|--------:|--------:|----------:|
| PointFactory_Random | 1,583.4 us | 2.46 us | 2.18 us |  32,057 B |
|   DelaunatorFactory |   332.2 us | 0.72 us | 0.64 us | 219,288 B |
|      VoronoiFactory |   647.2 us | 3.31 us | 2.77 us | 712,313 B |
|      PointInPolygon |   110.9 us | 0.36 us | 0.32 us |         - |

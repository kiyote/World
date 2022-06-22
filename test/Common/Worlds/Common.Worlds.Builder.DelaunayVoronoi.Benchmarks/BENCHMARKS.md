|            Method |         Mean |        Error |       StdDev | Allocated |
|------------------ |-------------:|-------------:|-------------:|----------:|
| DelaunatorFactory |    354.85 us |     4.703 us |     4.169 us |    214 KB |
|    VoronoiBuilder |  2,736.70 us |    12.853 us |    10.732 us |    941 KB |
|   LandformBuilder | 11,702.90 us |    93.962 us |    87.892 us |  1,252 KB |
|  MountainsBuilder | 82,443.83 us | 2,977.000 us | 8,684.047 us |     22 KB |
|      HillsBuilder |     26.04 us |     0.074 us |     0.061 us |      9 KB |
|  SaltwaterBuilder | 15,070.36 us |    53.280 us |    49.838 us |     83 KB |
| FreshwaterBuilder |     65.63 us |     0.488 us |     0.457 us |      2 KB |

MountainsBuilder with IVoronoiCellLocator
|------------------ |-------------:|-----------:|-----------:|----------:|
| DelaunatorFactory |    343.58 us |   2.606 us |   2.437 us |    214 KB |
|    VoronoiBuilder |  3,768.65 us |  59.265 us |  55.436 us |  1,191 KB |
|   LandformBuilder | 13,070.15 us | 114.587 us | 101.578 us |  1,616 KB |
|  MountainsBuilder |    898.38 us |   7.115 us |   6.307 us |     89 KB |
|      HillsBuilder |     33.19 us |   0.492 us |   0.460 us |      9 KB |
|  SaltwaterBuilder | 13,028.27 us | 180.132 us | 159.682 us |     80 KB |
| FreshwaterBuilder |     84.49 us |   0.209 us |   0.195 us |      2 KB |

Adding additional classes
|             Method |          Mean |      Error |     StdDev | Allocated |
|------------------- |--------------:|-----------:|-----------:|----------:|
|  DelaunatorFactory |     329.58 us |   1.720 us |   1.525 us |    214 KB |
|     VoronoiBuilder |   3,721.55 us |  12.992 us |  10.849 us |  1,191 KB |
|    LandformBuilder |  13,049.51 us |  47.637 us |  39.779 us |  1,617 KB |
|   MountainsBuilder |     890.94 us |   7.404 us |   6.564 us |     90 KB |
|       HillsBuilder |      31.77 us |   0.133 us |   0.125 us |      9 KB |
|   SaltwaterBuilder |  12,632.06 us |  56.568 us |  52.914 us |     80 KB |
|  FreshwaterBuilder |      85.59 us |   0.438 us |   0.342 us |      2 KB |
| TemperatureBuilder |  35,919.54 us | 207.264 us | 183.734 us |    297 KB |
|     AirflowBuilder | 115,298.85 us | 525.887 us | 466.185 us |  5,837 KB |
|    MoistureBuilder | 123,630.65 us | 822.487 us | 686.814 us |  9,338 KB |

Switch to IVoronoi
|             Method |          Mean |        Error |       StdDev | Allocated |
|------------------- |--------------:|-------------:|-------------:|----------:|
|  DelaunatorFactory |     343.19 us |     3.381 us |     3.163 us |    214 KB |
|     VoronoiBuilder |   4,261.14 us |    32.108 us |    28.463 us |  1,509 KB |
|    LandformBuilder |  13,870.80 us |   266.435 us |   249.223 us |  2,029 KB |
|   MountainsBuilder |     904.85 us |     8.821 us |     7.819 us |     89 KB |
|       HillsBuilder |      32.87 us |     0.505 us |     0.472 us |      9 KB |
|   SaltwaterBuilder |  12,878.78 us |   229.954 us |   215.100 us |     80 KB |
|  FreshwaterBuilder |      87.41 us |     0.637 us |     0.596 us |      2 KB |
| TemperatureBuilder |  36,137.46 us |   242.893 us |   215.319 us |    297 KB |
|     AirflowBuilder | 115,655.54 us |   925.294 us |   865.521 us |  5,837 KB |
|    MoistureBuilder | 126,395.34 us | 1,518.689 us | 1,420.582 us |  9,338 KB |

TemperatureBuilder to use ISearchableVoronoi
|             Method |          Mean |      Error |     StdDev | Allocated |
|------------------- |--------------:|-----------:|-----------:|----------:|
|  DelaunatorFactory |     358.99 us |   6.916 us |   6.792 us |    214 KB |
|     VoronoiBuilder |   4,364.00 us |  35.333 us |  31.322 us |  1,509 KB |
|    LandformBuilder |  14,107.74 us | 266.026 us | 284.644 us |  2,029 KB |
|   MountainsBuilder |     911.32 us |  17.331 us |  17.797 us |     89 KB |
|       HillsBuilder |      32.90 us |   0.307 us |   0.287 us |      9 KB |
|   SaltwaterBuilder |  13,454.80 us | 136.117 us | 113.664 us |     80 KB |
|  FreshwaterBuilder |      86.46 us |   1.540 us |   1.441 us |      2 KB |
| TemperatureBuilder |   1,181.11 us |   8.124 us |   6.784 us |    287 KB |
|     AirflowBuilder | 115,257.61 us | 885.208 us | 828.024 us |  5,837 KB |
|    MoistureBuilder | 122,804.65 us | 528.090 us | 468.138 us |  9,339 KB |

Removing VoronoiEdgeDetector
|             Method |         Mean |      Error |     StdDev | Allocated |
|------------------- |-------------:|-----------:|-----------:|----------:|
|  DelaunatorFactory |    348.80 us |   2.090 us |   1.746 us |    214 KB |
|     VoronoiBuilder |  4,479.82 us |  69.081 us |  61.238 us |  1,510 KB |
|    LandformBuilder | 14,170.52 us | 148.113 us | 138.545 us |  2,029 KB |
|   MountainsBuilder |    905.71 us |  15.143 us |  14.164 us |     90 KB |
|       HillsBuilder |     32.24 us |   0.436 us |   0.364 us |      9 KB |
|   SaltwaterBuilder | 13,618.36 us | 268.534 us | 611.588 us |     80 KB |
|  FreshwaterBuilder |     87.43 us |   1.107 us |   0.864 us |      2 KB |
| TemperatureBuilder |  1,203.55 us |  12.742 us |  11.919 us |    287 KB |
|     AirflowBuilder | 41,253.16 us | 522.279 us | 488.540 us |  5,302 KB |
|    MoistureBuilder | 48,967.51 us | 625.326 us | 584.931 us |  8,277 KB |
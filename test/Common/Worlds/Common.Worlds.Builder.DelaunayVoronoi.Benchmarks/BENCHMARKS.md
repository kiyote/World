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


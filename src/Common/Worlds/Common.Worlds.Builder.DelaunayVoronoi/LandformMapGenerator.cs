using Common.Buffers;
using Common.Buffers.Float;
using Common.Geometry;
using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class LandformMapGenerator : ILandformMapGenerator {

	private readonly IDelaunatorFactory _delaunatorFactory;
	private readonly IVoronoiFactory _voronoiFactory;
	private readonly IRandom _random;
	private readonly IBufferFactory _bufferFactory;
	private readonly IGeometry _geometry;
	private readonly IMountainRangeBuilder _mountainRangeBuilder;

	public LandformMapGenerator(
		IRandom random,
		IDelaunatorFactory delaunatorFactory,
		IVoronoiFactory voronoiFactory,
		IBufferFactory bufferFactory,
		IGeometry geometry,
		IMountainRangeBuilder mountainRangeBuilder
	) {
		_random = random;
		_delaunatorFactory = delaunatorFactory;
		_voronoiFactory = voronoiFactory;
		_bufferFactory = bufferFactory;
		_geometry = geometry;
		_mountainRangeBuilder = mountainRangeBuilder;
	}

	IBuffer<float> ILandformMapGenerator.Create(
		long seed,
		Size size,
		INeighbourLocator neighbourLocator
	) {
		// Get the rough outline of the land
		Voronoi roughVoronoi = FindRoughLandforms( size, out HashSet<Cell> landforms );

		// Generate finer detail of the rough landform
		Voronoi fineVoronoi = GenerateFineLandforms( landforms, size, out HashSet<Cell> fineLandforms );

		// Sprinkle some mountains in
		AddMountains( fineLandforms, size, fineVoronoi, out HashSet<Cell> mountains );

		// Fringe the mountains with hills
		AddHills( fineVoronoi, fineLandforms, mountains, out HashSet<Cell> hills );

		// Fringe the landform with coast, detect ocean
		AddSaltwater( size, fineVoronoi, fineLandforms, out HashSet<Cell> coast, out HashSet<Cell> ocean );

		// Finally, figure out what's left as lakes
		AddLakes( fineVoronoi, fineLandforms, coast, ocean, out HashSet<Cell> lakes );


		IBuffer<float> heightmap = _bufferFactory.Create<float>( size );
		foreach( Cell cell in fineLandforms ) {
			_geometry.RasterizePolygon( cell.Points, ( int x, int y ) => {
				if( x >= 0 && x < size.Columns && y >= 0 && y < size.Rows ) {
					heightmap[x, y] = 0.5f;
				}
			} );
		}

		foreach( Cell cell in ocean ) {
			_geometry.RasterizePolygon( cell.Points, ( int x, int y ) => {
				if( x >= 0 && x < size.Columns && y >= 0 && y < size.Rows ) {
					heightmap[x, y] = 0.15f;
				}
			} );
		}

		foreach( Cell cell in coast ) {
			_geometry.RasterizePolygon( cell.Points, ( int x, int y ) => {
				if( x >= 0 && x < size.Columns && y >= 0 && y < size.Rows ) {
					heightmap[x, y] = 0.20f;
				}
			} );
		}

		foreach( Cell cell in lakes ) {
			_geometry.RasterizePolygon( cell.Points, ( int x, int y ) => {
				if( x >= 0 && x < size.Columns && y >= 0 && y < size.Rows ) {
					heightmap[x, y] = 0.3f;
				}
			} );
		}

		foreach( Cell cell in hills ) {
			_geometry.RasterizePolygon( cell.Points, ( int x, int y ) => {
				if( x >= 0 && x < size.Columns && y >= 0 && y < size.Rows ) {
					heightmap[x, y] = 0.75f;
				}
			} );
		}

		foreach( Cell cell in mountains ) {
			_geometry.RasterizePolygon( cell.Points, ( int x, int y ) => {
				if( x >= 0 && x < size.Columns && y >= 0 && y < size.Rows ) {
					heightmap[x, y] = 1.0f;
				}
			} );
		}

		return heightmap;
	}

	private Voronoi GenerateVoronoi(
		Size size,
		int pointCount
	) {
		List<Point> points = new List<Point>();
		while( points.Count < pointCount ) {
			Point newPoint = new Point(
				_random.NextInt( size.Columns ),
				_random.NextInt( size.Rows )
			);
			if( !points.Any( p => Math.Abs( p.X - newPoint.X ) <= 1 && Math.Abs( p.Y - newPoint.Y ) <= 1 ) ) {
				points.Add( newPoint );
			};
		}
		Delaunator delaunator = _delaunatorFactory.Create( points );
		Voronoi voronoi = _voronoiFactory.Create( delaunator, size.Columns, size.Rows );

		return voronoi;
	}

	private Voronoi FindRoughLandforms(
		Size size,
		out HashSet<Cell> roughLandforms
	) {
		Voronoi voronoi = GenerateVoronoi( size, 100 );

		// Get the seeds of the landforms
		List<Cell> cells = voronoi.Cells.Where( c => !c.IsOpen ).ToList();
		int desiredCount = (int)( cells.Count * 0.3 );
		roughLandforms = new HashSet<Cell>();
		do {
			Cell cell = cells[_random.NextInt( cells.Count )];
			roughLandforms.Add( cell );
			cells.Remove( cell );
		} while( roughLandforms.Count < desiredCount );

		// Add the landforms neighbours to beef the shape up
		HashSet<Cell> result = new HashSet<Cell>();
		foreach( Cell seedCell in roughLandforms ) {
			result.Add( seedCell );
			foreach( Cell neighbourCell in voronoi.Neighbours[seedCell].Where( c => !c.IsOpen ) ) {
				result.Add( neighbourCell );
			}
		}
		roughLandforms = result;

		return voronoi;
	}

	private Voronoi GenerateFineLandforms(
		HashSet<Cell> roughLandforms,
		Size size,
		out HashSet<Cell> fineLandforms
	) {
		int fineCount = size.Columns * size.Rows / 200;
		Voronoi voronoi = GenerateVoronoi( size, fineCount );

		fineLandforms = new HashSet<Cell>();
		foreach( Cell fineCell in voronoi.Cells.Where( c => !c.IsOpen ) ) {
			foreach( Cell roughCell in roughLandforms ) {
				if( _geometry.PointInPolygon( roughCell.Points, fineCell.Center ) ) {
					fineLandforms.Add( fineCell );
				}
			}
		}

		return voronoi;
	}

	private void AddMountains(
		HashSet<Cell> fineLandforms,
		Size size,
		Voronoi fineVoronoi,
		out HashSet<Cell> mountains
	) {
		mountains = _mountainRangeBuilder.BuildRanges( size, fineVoronoi, fineLandforms );
	}

	private static void AddHills(
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains,
		out HashSet<Cell> hills
	) {
		hills = new HashSet<Cell>();
		foreach( Cell mountain in mountains ) {
			foreach( Cell neighbour in fineVoronoi.Neighbours[mountain] ) {
				if( !mountains.Contains( neighbour ) && fineLandforms.Contains( neighbour ) ) {
					hills.Add( neighbour );
				}
			}
		}
	}

	private void AddSaltwater(
		Size size,
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		out HashSet<Cell> coast,
		out HashSet<Cell> ocean
	) {
		Cell start = fineVoronoi.Cells[0];
		for( int i = 0; i < size.Columns; i += 10 ) {
			foreach( Cell cell in fineVoronoi.Cells ) {
				if( _geometry.PointInPolygon( cell.Points, i, 0 ) ) {
					if (!fineLandforms.Contains(cell)) {
						start = cell;
					}
					break;
				}
			}
		}

		Queue<Cell> queue = new Queue<Cell>();
		queue.Enqueue( start );

		List<Cell> visited = new List<Cell>();
		coast = new HashSet<Cell>();
		ocean = new HashSet<Cell>();
		while( queue.Any() ) {
			Cell cell = queue.Dequeue();
			if( !visited.Contains( cell ) ) {
				visited.Add( cell );

				foreach( Cell neighbour in fineVoronoi.Neighbours[cell] ) {
					if( !fineLandforms.Contains( neighbour ) ) {
						queue.Enqueue( neighbour );
					}
				}
				if( !coast.Contains( cell ) ) {
					bool isCoast = false;
					foreach( Cell neighbour in fineVoronoi.Neighbours[cell] ) {
						if( fineLandforms.Contains( neighbour ) ) {
							isCoast = true;
							break;
						}
					}
					if (isCoast) {
						coast.Add( cell );
					} else {
						ocean.Add( cell );
					}
				}
			}
		}
	}

	private static void AddLakes(
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> ocean,
		HashSet<Cell> coast,
		out HashSet<Cell> lakes
	) {
		lakes = new HashSet<Cell>();
		foreach (Cell cell in fineVoronoi.Cells) {
			if (!fineLandforms.Contains(cell)) {
				if (!ocean.Contains(cell)
					&& !coast.Contains(cell)
				) {
					lakes.Add( cell );
				}
			}
		}
	}

}

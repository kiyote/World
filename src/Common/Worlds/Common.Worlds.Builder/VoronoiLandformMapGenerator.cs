using Common.Buffers;
using Common.Buffers.Float;
using Common.Geometry;
using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder;

internal sealed class VoronoiLandformMapGenerator : ILandformMapGenerator {

	private readonly IDelaunatorFactory _delaunatorFactory;
	private readonly IVoronoiFactory _voronoiFactory;
	private readonly IRandom _random;
	private readonly IBufferFactory _bufferFactory;	
	private readonly IGeometry _geometry;
	private readonly IMountainRangeBuilder _mountainRangeBuilder;

	public VoronoiLandformMapGenerator(
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
		Voronoi roughVoronoi = FindRoughLandforms( size, out List<Cell> landforms );

		// Generate finer detail of the rough landform
		Voronoi fineVoronoi = GenerateFineLandforms( landforms, size, out List<Cell> fineLandforms );

		// Sprinkle some mountains in
		AddMountains( fineLandforms, size, fineVoronoi, out List<Cell> mountains );

		AddHills( fineVoronoi, fineLandforms, mountains, out List<Cell> hills );


		IBuffer<float> heightmap = _bufferFactory.Create<float>( size );
		foreach( Cell cell in fineLandforms ) {
			_geometry.RasterizePolygon( cell.Points, ( int x, int y ) => {
				if( x >= 0 && x < size.Columns && y >= 0 && y < size.Rows ) {
					heightmap[x, y] = 0.5f;
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
		out List<Cell> roughLandforms
	) {
		Voronoi voronoi = GenerateVoronoi( size, 100 );

		// Get the seeds of the landforms
		List<Cell> cells = voronoi.Cells.Where( c => !c.IsOpen ).ToList();
		int desiredCount = (int)( cells.Count * 0.3 );
		roughLandforms = new List<Cell>();
		do {
			Cell cell = cells[_random.NextInt( cells.Count )];
			roughLandforms.Add( cell );
			cells.Remove( cell );
		} while( roughLandforms.Count < desiredCount );

		// Add the landforms neighbours to beef the shape up
		List<Cell> result = new List<Cell>();
		foreach( Cell seedCell in roughLandforms ) {
			result.Add( seedCell );
			foreach( Cell neighbourCell in voronoi.Neighbours[seedCell].Where( c => !c.IsOpen ) ) {
				if( !result.Contains( neighbourCell ) ) {
					result.Add( neighbourCell );
				}
			}
		}
		roughLandforms = result;

		return voronoi;
	}

	private Voronoi GenerateFineLandforms(
		List<Cell> roughLandforms,
		Size size,
		out List<Cell> fineLandforms
	) {
		int fineCount = size.Columns * size.Rows / 200;
		Voronoi voronoi = GenerateVoronoi( size, fineCount );

		fineLandforms = new List<Cell>();
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
		List<Cell> fineLandforms,
		Size size,
		Voronoi fineVoronoi,
		out List<Cell> mountains
	) {
		mountains = _mountainRangeBuilder.BuildRanges( size, fineVoronoi, fineLandforms );
	}

	private static void AddHills(
		Voronoi fineVoronoi,
		List<Cell> fineLandforms,
		List<Cell> mountains,
		out List<Cell> hills
	) {
		hills = new List<Cell>();
		foreach (Cell mountain in mountains) {
			foreach (Cell neighbour in fineVoronoi.Neighbours[mountain]) {
				if (!mountains.Contains(neighbour) && fineLandforms.Contains(neighbour)) {
					if (!hills.Contains(neighbour)) {
						hills.Add( neighbour );
					}
				}
			}
		}
	}

}

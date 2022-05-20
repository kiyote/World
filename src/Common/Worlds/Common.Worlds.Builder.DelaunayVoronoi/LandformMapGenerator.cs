using Common.Buffers;
using Common.Buffers.Float;
using Common.Geometry;
using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class LandformMapGenerator : ILandformMapGenerator {

	private readonly IRandom _random;
	private readonly IBufferFactory _bufferFactory;
	private readonly IGeometry _geometry;
	private readonly IMountainsBuilder _mountainRangeBuilder;
	private readonly IVoronoiBuilder _voronoiBuilder;
	private readonly IHillsBuilder _hillsBuilder;
	private readonly ISaltwaterBuilder _saltwaterBuilder;
	private readonly IFreshwaterBuilder _freshwaterBuilder;

	public LandformMapGenerator(
		IRandom random,
		IBufferFactory bufferFactory,
		IGeometry geometry,
		IMountainsBuilder mountainRangeBuilder,
		IVoronoiBuilder voronoiBuilder,
		IHillsBuilder hillsBuilder,
		ISaltwaterBuilder saltwaterBuilder,
		IFreshwaterBuilder freshwaterBuilder
	) {
		_random = random;
		_bufferFactory = bufferFactory;
		_geometry = geometry;
		_mountainRangeBuilder = mountainRangeBuilder;
		_voronoiBuilder = voronoiBuilder;
		_hillsBuilder = hillsBuilder;
		_saltwaterBuilder = saltwaterBuilder;
		_freshwaterBuilder = freshwaterBuilder;
	}

	LandformMaps ILandformMapGenerator.Create(
		long seed,
		Size size,
		INeighbourLocator neighbourLocator
	) {
		// Get the rough outline of the land
		Voronoi roughVoronoi = FindRoughLandforms( size, out HashSet<Cell> landforms );

		// Generate finer detail of the rough landform
		Voronoi fineVoronoi = GenerateFineLandforms( landforms, size, out HashSet<Cell> fineLandforms );

		HashSet<Cell> mountains = _mountainRangeBuilder.Create( size, fineVoronoi, fineLandforms );
		HashSet<Cell> hills = _hillsBuilder.Create( fineVoronoi, fineLandforms, mountains );
		HashSet<Cell> ocean = _saltwaterBuilder.Create( size, fineVoronoi, fineLandforms );
		HashSet<Cell> lakes = _freshwaterBuilder.Create( fineVoronoi, fineLandforms, ocean );

		IBuffer<bool> saltwater = _bufferFactory.Create( size, true );
		IBuffer<float> heightmap = _bufferFactory.Create<float>( size );
		foreach( Cell cell in fineLandforms ) {
			_geometry.RasterizePolygon( cell.Points, ( int x, int y ) => {
				if( x >= 0 && x < size.Columns && y >= 0 && y < size.Rows ) {
					heightmap[x, y] = 0.5f;
					saltwater[x, y] = false;
				}
			} );
		}

		foreach( Cell cell in ocean ) {
			// If it's coast, we raise it up a bit
			float height = 0.0f;
			foreach( Cell neighbour in fineVoronoi.Neighbours[cell] ) {
				if( fineLandforms.Contains( neighbour ) ) {
					height = 0.25f;
					break;
				}
			}

			_geometry.RasterizePolygon( cell.Points, ( int x, int y ) => {
				if( x >= 0 && x < size.Columns && y >= 0 && y < size.Rows ) {
					heightmap[x, y] = height;
				}
			} );
		}

		IBuffer<bool> freshwater = _bufferFactory.Create<bool>( size );
		foreach( Cell cell in lakes ) {
			_geometry.RasterizePolygon( cell.Points, ( int x, int y ) => {
				if( x >= 0 && x < size.Columns && y >= 0 && y < size.Rows ) {
					heightmap[x, y] = 0.25f;
					freshwater[x, y] = true;
					saltwater[x, y] = false;
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

		/* Box-blur the landform to smooth out the voronoi shapes
		IBuffer<float> blur = _bufferFactory.Create<float>( size );
		_floatBufferOperators.HorizonalBlur( heightmap, 15, blur );
		_floatBufferOperators.VerticalBlur( blur, 15, heightmap );
		*/

		IBuffer<float> temperature = _bufferFactory.Create<float>( size ); //**** Update this

		return new LandformMaps(
			heightmap,
			saltwater,
			freshwater,
			temperature
		);
	}

	private Voronoi FindRoughLandforms(
		Size size,
		out HashSet<Cell> roughLandforms
	) {
		Voronoi voronoi = _voronoiBuilder.Create( size, 100 );

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
		Voronoi voronoi = _voronoiBuilder.Create( size, fineCount );

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
}

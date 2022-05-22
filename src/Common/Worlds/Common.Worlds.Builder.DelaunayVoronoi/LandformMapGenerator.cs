using Common.Buffers;
using Common.Buffers.Float;
using Common.Geometry;
using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class LandformMapGenerator : ILandformMapGenerator {

	private readonly IBufferFactory _bufferFactory;
	private readonly IGeometry _geometry;
	private readonly IMountainsBuilder _mountainsBuilder;
	private readonly ILandformBuilder _landformBuilder;
	private readonly IHillsBuilder _hillsBuilder;
	private readonly ISaltwaterBuilder _saltwaterBuilder;
	private readonly IFreshwaterBuilder _freshwaterBuilder;
	private readonly IFloatBufferOperators _floatBufferOperators;

	public LandformMapGenerator(
		IFloatBufferOperators floatBufferOperators,
		IBufferFactory bufferFactory,
		IGeometry geometry,
		IMountainsBuilder mountainsBuilder,
		ILandformBuilder landformBuilder,
		IHillsBuilder hillsBuilder,
		ISaltwaterBuilder saltwaterBuilder,
		IFreshwaterBuilder freshwaterBuilder
	) {
		_floatBufferOperators = floatBufferOperators;
		_bufferFactory = bufferFactory;
		_geometry = geometry;
		_landformBuilder = landformBuilder;
		_mountainsBuilder = mountainsBuilder;
		_hillsBuilder = hillsBuilder;
		_saltwaterBuilder = saltwaterBuilder;
		_freshwaterBuilder = freshwaterBuilder;
	}

	LandformMaps ILandformMapGenerator.Create(
		long seed,
		Size size,
		INeighbourLocator neighbourLocator
	) {
		HashSet<Cell> fineLandforms = _landformBuilder.Create( size, out Voronoi fineVoronoi );

		HashSet<Cell> mountains = _mountainsBuilder.Create( size, fineVoronoi, fineLandforms );
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

		IBuffer<float> temperature = _bufferFactory.Create<float>( size );
		for( int r = 0; r < size.Rows; r++) {
			float temp = (size.Rows / 2) - Math.Abs( r - ( size.Rows / 2 ) );
			for (int c = 0; c < size.Columns; c++) {
				temperature[c, r] = temp;
			}
		}

		IBuffer<float> tempBuffer = _bufferFactory.Create<float>( size );
		_floatBufferOperators.Invert( heightmap, tempBuffer );

		_floatBufferOperators.Normalize( temperature, 0.0f, 1.0f, temperature );
		_floatBufferOperators.Multiply( temperature, tempBuffer, temperature );

		_floatBufferOperators.HorizonalBlur( temperature, 15, tempBuffer );
		_floatBufferOperators.VerticalBlur( tempBuffer, 15, temperature );

		return new LandformMaps(
			heightmap,
			saltwater,
			freshwater,
			temperature
		);
	}
}

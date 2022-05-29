using Common.Buffers;
using Common.Buffers.Float;
using Common.Geometry;
using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class VoronoiWorldMapGenerator : IWorldMapGenerator {

	private readonly IBufferFactory _bufferFactory;
	private readonly IBufferOperator _bufferOperator;
	private readonly IGeometry _geometry;
	private readonly IMountainsBuilder _mountainsBuilder;
	private readonly ILandformBuilder _landformBuilder;
	private readonly IHillsBuilder _hillsBuilder;
	private readonly ISaltwaterBuilder _saltwaterBuilder;
	private readonly IFreshwaterBuilder _freshwaterBuilder;
	private readonly IFloatBufferOperators _floatBufferOperators;

	public VoronoiWorldMapGenerator(
		IBufferOperator bufferOperator,
		IFloatBufferOperators floatBufferOperators,
		IBufferFactory bufferFactory,
		IGeometry geometry,
		IMountainsBuilder mountainsBuilder,
		ILandformBuilder landformBuilder,
		IHillsBuilder hillsBuilder,
		ISaltwaterBuilder saltwaterBuilder,
		IFreshwaterBuilder freshwaterBuilder
	) {
		_bufferOperator = bufferOperator;
		_floatBufferOperators = floatBufferOperators;
		_bufferFactory = bufferFactory;
		_geometry = geometry;
		_landformBuilder = landformBuilder;
		_mountainsBuilder = mountainsBuilder;
		_hillsBuilder = hillsBuilder;
		_saltwaterBuilder = saltwaterBuilder;
		_freshwaterBuilder = freshwaterBuilder;
	}

	WorldMaps IWorldMapGenerator.Create(
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

		_floatBufferOperators.HorizontalBlur( temperature, 15, tempBuffer );
		_floatBufferOperators.VerticalBlur( tempBuffer, 15, temperature );

		IBuffer<float> moisture = _bufferFactory.Create<float>( size );
		_bufferOperator.Perform(
			moisture,
			( int x, int y, IBuffer<float> moisture, float value ) => {
				if( saltwater[x, y]
					|| freshwater[x, y]
				) {
					return 1.0f;
				}
				return value;
			},
			moisture
		);

		_floatBufferOperators.Add( moisture, temperature, moisture );
		_floatBufferOperators.Normalize( moisture, 0.0f, 1.0f, moisture );

		//_floatBufferOperators.HorizontalBlur( moisture, 25, tempBuffer );
		//_floatBufferOperators.VerticalBlur( tempBuffer, 25, moisture );
		
		//_floatBufferOperators.VerticalBlur( heightmap, 5, tempBuffer );
		//_floatBufferOperators.HorizontalBlur( tempBuffer, 5, heightmap );

		float[] ranges = new float[] { 0.0f, 0.25f, 0.6f, 0.9f, float.MaxValue };

		IBuffer<float> quantized = _bufferFactory.Create<float>( size );
		_floatBufferOperators.Quantize( heightmap, ranges, quantized );
		IBuffer<TileTerrain> terrain = _bufferFactory.Create<TileTerrain>( size );
		_bufferOperator.Perform(
			quantized,
			( int c, int r, float floatValue ) => {
				if( freshwater[c, r] ) {
					return TileTerrain.Lake;
				}

				int value = (int)floatValue;
				if( value == 0 ) {
					return TileTerrain.Ocean;
				} else if( value == 1 ) {
					return TileTerrain.Coast;
				} else if( value == 2 ) {
					return TileTerrain.Plain;
				} else if( value == 3 ) {
					return TileTerrain.Hill;
				} else if( value == 4 ) {
					return TileTerrain.Mountain;
				} else {
					throw new InvalidOperationException();
				}
			},
			terrain
		);

		IBuffer<TileFeature> feature = _bufferFactory.Create<TileFeature>( size );

		return new WorldMaps(
			terrain,
			feature
		);
	}
}

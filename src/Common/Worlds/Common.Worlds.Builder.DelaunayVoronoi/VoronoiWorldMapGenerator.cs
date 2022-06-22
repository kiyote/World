using Common.Buffers;
using Common.Geometry;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class VoronoiWorldMapGenerator : IWorldMapGenerator {

	private readonly IBufferFactory _bufferFactory;
	private readonly IGeometry _geometry;
	private readonly IMountainsBuilder _mountainsBuilder;
	private readonly ILandformBuilder _landformBuilder;
	private readonly IHillsBuilder _hillsBuilder;
	private readonly ISaltwaterBuilder _saltwaterBuilder;
	private readonly IFreshwaterBuilder _freshwaterBuilder;
	private readonly ITemperatureBuilder _temperatureBuilder;
	private readonly IAirflowBuilder _airflowBuilder;
	private readonly IMoistureBuilder _moistureBuilder;
	private readonly IForestBuilder _forestBuilder;
	private readonly IDesertBuilder _desertBuilder;
	
	public VoronoiWorldMapGenerator(
		IBufferFactory bufferFactory,
		IGeometry geometry,
		IMountainsBuilder mountainsBuilder,
		ILandformBuilder landformBuilder,
		IHillsBuilder hillsBuilder,
		ISaltwaterBuilder saltwaterBuilder,
		IFreshwaterBuilder freshwaterBuilder,
		ITemperatureBuilder temperatureBuilder,
		IAirflowBuilder airflowBuilder,
		IMoistureBuilder moistureBuilder,
		IForestBuilder forestBuilder,
		IDesertBuilder desertBuilder
	) {
		_bufferFactory = bufferFactory;
		_geometry = geometry;
		_landformBuilder = landformBuilder;
		_mountainsBuilder = mountainsBuilder;
		_hillsBuilder = hillsBuilder;
		_saltwaterBuilder = saltwaterBuilder;
		_freshwaterBuilder = freshwaterBuilder;
		_temperatureBuilder = temperatureBuilder;
		_airflowBuilder = airflowBuilder;
		_moistureBuilder = moistureBuilder;
		_forestBuilder = forestBuilder;
		_desertBuilder = desertBuilder;
	}

	WorldMaps IWorldMapGenerator.Create(
		long seed,
		Size size,
		INeighbourLocator neighbourLocator
	) {
		HashSet<Cell> fineLandforms = _landformBuilder.Create( size, out ISearchableVoronoi voronoi );

		HashSet<Cell> mountains = _mountainsBuilder.Create( size, voronoi, fineLandforms );
		HashSet<Cell> hills = _hillsBuilder.Create( voronoi, fineLandforms, mountains );
		HashSet<Cell> oceans = _saltwaterBuilder.Create( size, voronoi, fineLandforms );
		HashSet<Cell> lakes = _freshwaterBuilder.Create( voronoi, fineLandforms, oceans );
		Dictionary<Cell, float> temperatures = _temperatureBuilder.Create( size, voronoi, fineLandforms, mountains, hills, oceans, lakes );
		Dictionary<Cell, float> airflows = _airflowBuilder.Create( size, voronoi, fineLandforms, mountains, hills );
		Dictionary<Cell, float> moistures = _moistureBuilder.Create( size, voronoi, fineLandforms, oceans, lakes, temperatures, airflows );
		HashSet<Cell> forests = _forestBuilder.Create( voronoi, fineLandforms, mountains, hills, temperatures, moistures );
		HashSet<Cell> deserts = _desertBuilder.Create( fineLandforms, mountains, hills, moistures );

		HashSet<Cell> coast = new HashSet<Cell>();
		foreach( Cell land in fineLandforms ) {
			foreach( Cell neighbour in voronoi.Neighbours[land] ) {
				if( oceans.Contains( neighbour ) ) {
					coast.Add( neighbour );
				}
			}
		}

		IBuffer<TileFeature> feature = _bufferFactory.Create( size, TileFeature.None );
		IBuffer<TileTerrain> terrain = _bufferFactory.Create( size, TileTerrain.Ocean );

		RenderTerrain( terrain, coast, TileTerrain.Coast );
		RenderTerrain( terrain, fineLandforms, TileTerrain.Plain );
		RenderTerrain( terrain, lakes, TileTerrain.Lake );
		RenderTerrain( terrain, hills, TileTerrain.Hill );
		RenderTerrain( terrain, mountains, TileTerrain.Mountain );

		RenderFeature( feature, forests, cell => {
			TileFeature tileFeature = TileFeature.TemperateForest;
			float temperature = temperatures[cell];
			if( temperature < 0.3f ) {
				tileFeature = TileFeature.BorealForest;
			} else if( temperature > 0.8f ) {
				tileFeature = TileFeature.TropicalForest;
			}
			return tileFeature;
		} );

		RenderFeature( feature, deserts, cell => {
			TileFeature tileFeature = TileFeature.RockyDesert;
			float temperature = temperatures[cell];
			if( temperature > 0.8f ) {
				tileFeature = TileFeature.SandyDesert;
			} else if( temperature < 0.2f ) {
				tileFeature = TileFeature.Tundra;
			}
			return tileFeature;
		} );

		return new WorldMaps(
			terrain,
			feature
		);
	}

	private void RenderTerrain(
		IBuffer<TileTerrain> buffer,
		HashSet<Cell> cells,
		TileTerrain terrain
	) {
		foreach( Cell cell in cells ) {
			_geometry.RasterizePolygon( cell.Points, ( x, y ) => {
				buffer[x, y] = terrain;
			} );
		}
	}


	private void RenderFeature(
		IBuffer<TileFeature> buffer,
		HashSet<Cell> cells,			
		Func<Cell, TileFeature> calculateFeature
	) {
		foreach( Cell cell in cells ) {
			TileFeature feature = calculateFeature( cell );
			_geometry.RasterizePolygon( cell.Points, ( x, y ) => {
				buffer[x, y] |= feature;
			} );
		}
	}
}

using Kiyote.Buffers;
using Kiyote.Geometry.Rasterizers;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class VoronoiWorldMapGenerator : IWorldMapGenerator {

	private readonly IBufferFactory _bufferFactory;
	private readonly IRasterizer _rasterizer;
	private readonly ILandformBuilder _landformBuilder;
	private readonly ISaltwaterFinder _saltwaterBuilder;
	private readonly IFreshwaterFinder _freshwaterBuilder;
	private readonly ILakeFinder _lakeBuilder;
	private readonly ICoastFinder _coastBuilder;

	private readonly ITectonicPlateBuilder _tectonicPlateBuilder;
	private readonly IInlandDistanceFinder _inlandDistanceFinder;
	private readonly IElevationBuilder _elevationBuilder;
	private readonly IElevationScaler _elevationScaler;

	public VoronoiWorldMapGenerator(
		IBufferFactory bufferFactory,
		IRasterizer rasterizer,
		ILandformBuilder landformBuilder,
		ISaltwaterFinder saltwaterBuilder,
		IFreshwaterFinder freshwaterBuilder,
		ILakeFinder lakeBuilder,
		ICoastFinder coastBuilder,
		ITectonicPlateBuilder tectonicPlateBuilder,
		IInlandDistanceFinder inlandDistanceFinder,
		IElevationBuilder elevationBuilder,
		IElevationScaler elevationScaler
	) {
		_bufferFactory = bufferFactory;
		_rasterizer = rasterizer;
		_landformBuilder = landformBuilder;
		_saltwaterBuilder = saltwaterBuilder;
		_freshwaterBuilder = freshwaterBuilder;
		_lakeBuilder = lakeBuilder;
		_coastBuilder = coastBuilder;

		_tectonicPlateBuilder = tectonicPlateBuilder;
		_inlandDistanceFinder = inlandDistanceFinder;
		_elevationBuilder = elevationBuilder;
		_elevationScaler = elevationScaler;
	}

	async Task<WorldMaps> IWorldMapGenerator.CreateAsync(
		long seed,
		ISize size,
		INeighbourLocator neighbourLocator,
		CancellationToken cancellationToken
	) {
		TectonicPlates tectonicPlates = _tectonicPlateBuilder.Create( size );
		// Creates the cells that will be above sea level.
		Landform landform = await _landformBuilder.CreateAsync( size, tectonicPlates, cancellationToken ).ConfigureAwait( false );

		// Floodfills the map from the edge to find all "not land" cells that
		// are not fully surrounded by land.
		IReadOnlySet<Cell> saltwater = _saltwaterBuilder.Find( size, landform.Map, landform.Cells );

		// Finds all ocean cells that are adjacent to land
		IReadOnlySet<Cell> coast = _coastBuilder.Find( size, landform.Map, landform.Cells, saltwater );
		// Calculate the inland distance to the ocean for each cell.
		IReadOnlyDictionary<Cell, float> inlandDistance = await _inlandDistanceFinder.CreateAsync( size, landform.Map, landform.Cells, coast, cancellationToken ).ConfigureAwait( false );

		// Apply an elevation to the cells
		IReadOnlyDictionary<Cell, float> elevation = await _elevationBuilder.CreateAsync( size, tectonicPlates, landform.Map, landform.Cells, inlandDistance, cancellationToken ).ConfigureAwait( false );

		Dictionary<Cell, float> scaledElevation = [];
		float minElevation = float.MaxValue;
		float maxElevation = float.MinValue;
		foreach( KeyValuePair<Cell, float> kvp in elevation ) {
			float scaled = kvp.Value;
			//float scaled = _elevationScaler.Scale( kvp.Value );
			if( scaled < minElevation ) {
				minElevation = scaled;
			}
			if( scaled > maxElevation ) {
				maxElevation = scaled;
			}
			scaledElevation[kvp.Key] = scaled;
		}
		// Normalize the elevation to be between 0 and 1
		float range = ( maxElevation - minElevation );
		foreach( KeyValuePair<Cell, float> kvp in scaledElevation ) {
			scaledElevation[kvp.Key] = ( kvp.Value - minElevation ) / range;
		}

		// Prepares the various features of the map.

		// Finds all cells that are "not land" that are not saltwater.
		IReadOnlySet<Cell> freshwater = _freshwaterBuilder.Create( size, landform.Map, landform.Cells, saltwater );
		// Determines all freshwater cells that are adjacent to other freshwater
		// cells. ie - This will contain the "clumps" of freshwater cells.
		IReadOnlyList<IReadOnlySet<Cell>> lakes = _lakeBuilder.Finder( size, landform.Map, landform.Cells, saltwater, freshwater );

		// Now we can render the map.

		// This will indicate what type of terrain is present. (Grassland, ocean,
		// whatever)  This underlies whatever feature may be present.  For example
		// there could be forest present on a Plain.  The terrain is still Plains,
		// but there is a feature of a Forest present in that terrain.
		IBuffer<TileTerrain> terrain = _bufferFactory.Create( size, TileTerrain.Ocean );

		foreach( KeyValuePair<Cell, float> kvp in scaledElevation ) {
			Cell cell = kvp.Key;
			float height = kvp.Value;
			TileTerrain tileTerrain;
			if( height > 0.6f ) {
				tileTerrain = TileTerrain.Mountain;
			} else if( height > 0.5f ) {
				tileTerrain = TileTerrain.Hill;
			} else if( height > 0.4f ) {
				tileTerrain = TileTerrain.Highland;
			} else {
				tileTerrain = TileTerrain.Plain;
			}
			_rasterizer.Rasterize( cell.Polygon.Points, ( x, y ) => {
				terrain[x, y] = tileTerrain;
			} );
		}

		// Apply the coastline
		RenderTerrain( terrain, coast, TileTerrain.Coast );
		// Paint the freshwater
		RenderTerrain( terrain, freshwater, TileTerrain.Lake );

		// TODO - Something with this.
		IBuffer<TileFeature> feature = _bufferFactory.Create( size, TileFeature.None );

		return new WorldMaps(
			terrain,
			feature
		);
	}

	private void RenderTerrain(
		IBuffer<TileTerrain> buffer,
		IReadOnlySet<Cell> cells,
		TileTerrain terrain
	) {
		foreach( Cell cell in cells ) {
			_rasterizer.Rasterize( cell.Polygon.Points, ( x, y ) => {
				buffer[x, y] = terrain;
			} );
		}
	}


	private void RenderFeature(
		IBuffer<TileFeature> buffer,
		IReadOnlySet<Cell> cells,
		Func<Cell, TileFeature> calculateFeature
	) {
		foreach( Cell cell in cells ) {
			TileFeature feature = calculateFeature( cell );
			_rasterizer.Rasterize( cell.Polygon.Points, ( x, y ) => {
				buffer[x, y] |= feature;
			} );
		}
	}
}

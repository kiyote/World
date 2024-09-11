using Common.Buffers;
using Kiyote.Geometry.Rasterizers;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class VoronoiWorldMapGenerator : IWorldMapGenerator {

	private readonly IBufferFactory _bufferFactory;
	private readonly IRasterizer _rasterizer;
	private readonly ILandformBuilder _landformBuilder;
	private readonly ISaltwaterBuilder _saltwaterBuilder;
	private readonly IFreshwaterBuilder _freshwaterBuilder;
	private readonly ILakeBuilder _lakeBuilder;
	
	public VoronoiWorldMapGenerator(
		IBufferFactory bufferFactory,
		IRasterizer rasterizer,
		ILandformBuilder landformBuilder,
		ISaltwaterBuilder saltwaterBuilder,
		IFreshwaterBuilder freshwaterBuilder,
		ILakeBuilder lakeBuilder
	) {
		_bufferFactory = bufferFactory;
		_rasterizer = rasterizer;
		_landformBuilder = landformBuilder;
		_saltwaterBuilder = saltwaterBuilder;
		_freshwaterBuilder = freshwaterBuilder;
		_lakeBuilder = lakeBuilder;
	}

	WorldMaps IWorldMapGenerator.Create(
		long seed,
		ISize size,
		INeighbourLocator neighbourLocator
	) {
		IReadOnlySet<Cell> landform = _landformBuilder.Create( size, out ISearchableVoronoi map );
		IReadOnlySet<Cell> saltwater = _saltwaterBuilder.Create( size, map, landform );
		IReadOnlySet<Cell> freshwater = _freshwaterBuilder.Create( size, map, landform, saltwater );
		IReadOnlyList<IReadOnlySet<Cell>> lakes = _lakeBuilder.Create( size, map, landform, saltwater, freshwater );

		HashSet<Cell> coast = [];
		foreach( Cell land in landform ) {
			foreach( Cell neighbour in map.Neighbours[land] ) {
				if( saltwater.Contains( neighbour ) ) {
					coast.Add( neighbour );
				}
			}
		}

		IBuffer<TileFeature> feature = _bufferFactory.Create( size, TileFeature.None );
		IBuffer<TileTerrain> terrain = _bufferFactory.Create( size, TileTerrain.Ocean );

		RenderTerrain( terrain, coast, TileTerrain.Coast );
		RenderTerrain( terrain, landform, TileTerrain.Plain );
		RenderTerrain( terrain, freshwater, TileTerrain.Lake );

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

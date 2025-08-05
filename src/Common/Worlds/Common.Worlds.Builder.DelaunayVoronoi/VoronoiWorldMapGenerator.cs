using Kiyote.Buffers;
using Kiyote.Geometry.Rasterizers;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class VoronoiWorldMapGenerator : IWorldMapGenerator {

	private readonly IBufferFactory _bufferFactory;
	private readonly IRasterizer _rasterizer;
	private readonly ILandformBuilder _landformBuilder;
	private readonly ISaltwaterBuilder _saltwaterBuilder;
	private readonly IFreshwaterBuilder _freshwaterBuilder;
	private readonly ILakeBuilder _lakeBuilder;
	private readonly ICoastBuilder _coastBuilder;

	public VoronoiWorldMapGenerator(
		IBufferFactory bufferFactory,
		IRasterizer rasterizer,
		ILandformBuilder landformBuilder,
		ISaltwaterBuilder saltwaterBuilder,
		IFreshwaterBuilder freshwaterBuilder,
		ILakeBuilder lakeBuilder,
		ICoastBuilder coastBuilder
	) {
		_bufferFactory = bufferFactory;
		_rasterizer = rasterizer;
		_landformBuilder = landformBuilder;
		_saltwaterBuilder = saltwaterBuilder;
		_freshwaterBuilder = freshwaterBuilder;
		_lakeBuilder = lakeBuilder;
		_coastBuilder = coastBuilder;
	}

	WorldMaps IWorldMapGenerator.Create(
		long seed,
		ISize size,
		INeighbourLocator neighbourLocator
	) {
		// Creates the cells that will be above sea level.
		IReadOnlySet<Cell> landform = _landformBuilder.Create( size, out ISearchableVoronoi map );
		// Floodfills the map from the edge to find all "not land" cells that
		// are not fully surrounded by land.
		IReadOnlySet<Cell> saltwater = _saltwaterBuilder.Create( size, map, landform );
		// Finds all cells that are "not land" that are not saltwater.
		IReadOnlySet<Cell> freshwater = _freshwaterBuilder.Create( size, map, landform, saltwater );
		// Determines all freshwater cells that are adjacent to other freshwater
		// cells. ie - This will contain the "clumps" of freshwater cells.
		IReadOnlyList<IReadOnlySet<Cell>> lakes = _lakeBuilder.Create( size, map, landform, saltwater, freshwater );
		// Finds all ocean cells that are adjacent to land
		IReadOnlySet<Cell> coast = _coastBuilder.Create( size, map, landform, saltwater );

		// This will indicate what type of terrain is present. (Grassland, ocean,
		// whatever)  This underlies whatever feature may be present.  For example
		// there could be forest present on a Plain.  The terrain is still Plains,
		// but there is a feature of a Forest present in that terrain.
		IBuffer<TileTerrain> terrain = _bufferFactory.Create( size, TileTerrain.Ocean );
		// Apply the coastline
		RenderTerrain( terrain, coast, TileTerrain.Coast );
		// Render the land
		RenderTerrain( terrain, landform, TileTerrain.Plain );
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

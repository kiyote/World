namespace Common.Worlds.Builder.DelaunayVoronoi;

internal class TectonicLandformBuilder : ILandformBuilder {

	/// <summary>
	/// The number of rough cells either horizontally or vertically across the
	/// map, using whichever dimension is smaller.
	/// </summary>
	/// <remarks>
	/// The size of these rough cells will be proportional to the size of the
	/// map divided by the number of cells.
	/// </remarks>
	public const int RoughCellCount = 6;

	public const int MinimumCellSize = 5;

	public const float MinimumDensity = 0.5f;
	public const float MaximumDensity = 0.7f;

	private readonly IRandom _random;
	private readonly IVoronoiBuilder _voronoiBuilder;
	private readonly ITectonicPlateBuilder _tectonicPlateBuilder;
	private readonly ISearchableVoronoiFactory _searchableVoronoiFactory;

	public TectonicLandformBuilder(
		IRandom random,
		ITectonicPlateBuilder tectonicPlateBuilder,
		IVoronoiBuilder voronoiBuilder,
		ISearchableVoronoiFactory searchableVoronoiFactory
	) {
		_random = random;
		_voronoiBuilder = voronoiBuilder;
		_tectonicPlateBuilder = tectonicPlateBuilder;
		_searchableVoronoiFactory = searchableVoronoiFactory;
	}

	IReadOnlySet<Cell> ILandformBuilder.Create(
		ISize size,
		TectonicPlates tectonicPlates,
		out ISearchableVoronoi map
	) {
		HashSet<Cell> cells;
		IVoronoi landform;
		float terrainDensity;
		do {
			// Find all the cells that fall on the edges of the tectonic plates
			int cellSize = Math.Min( size.Width, size.Height ) / RoughCellCount;
			cells = [];
			landform = _voronoiBuilder.Create( size, cellSize );
			foreach( Edge edge in tectonicPlates.Plates.Edges ) {
				if( ( _random.NextFloat() > 0.5f ) ) {
					foreach( Cell cell in landform.Cells ) {
						if( !cell.IsOpen
							&& cell.Polygon.HasIntersection( edge )
						) {
							cells.Add( cell );
						}
					}
				}
			}

			while( cellSize >= ( MinimumCellSize * 2 ) ) {
				cellSize /= 2;
				landform = _voronoiBuilder.Create( size, cellSize );
				ISearchableVoronoi searchableLandform = _searchableVoronoiFactory.Create( landform, size );

				HashSet<Cell> newCells = [];
				foreach( Cell oldCell in cells ) {
					IReadOnlyList<Cell> searchCells = searchableLandform.Search( oldCell.BoundingBox );
					foreach( Cell newCell in searchCells ) {
						if( !newCell.IsOpen
							&& newCell.Polygon.HasIntersection( oldCell.Polygon )
						) {
							newCells.Add( newCell );
						}
					}
				}
				cells = newCells;
			}


			foreach( Cell cell in landform.Cells ) {
				if( cells.Contains( cell ) ) {
					continue;
				}
				IReadOnlyList<Cell> neighbours = landform.Neighbours[cell];
				bool surrounded = true;
				foreach( Cell neighbour in neighbours ) {
					if( !cells.Contains( neighbour ) ) {
						surrounded = false;
						break;
					}
				}
				if( surrounded ) {
					cells.Add( cell );
				}
			}

			terrainDensity = (float)cells.Count / (float)landform.Cells.Count;
		} while( terrainDensity < MinimumDensity
			|| terrainDensity > MaximumDensity
		);


		map = _searchableVoronoiFactory.Create( landform, size );
		return cells.ToHashSet();
	}
}

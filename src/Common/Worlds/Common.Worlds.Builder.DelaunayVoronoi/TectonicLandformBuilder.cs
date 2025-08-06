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
	public const int RoughCellCount = 3;

	public const int MinimumCellSize = 5;

	public const float MinimumDensity = 0.5f;
	public const float MaximumDensity = 0.7f;

	private readonly IVoronoiBuilder _voronoiBuilder;
	private readonly ISearchableVoronoiFactory _searchableVoronoiFactory;

	public TectonicLandformBuilder(
		IVoronoiBuilder voronoiBuilder,
		ISearchableVoronoiFactory searchableVoronoiFactory
	) {
		_voronoiBuilder = voronoiBuilder;
		_searchableVoronoiFactory = searchableVoronoiFactory;
	}

	IReadOnlySet<Cell> ILandformBuilder.Create(
		ISize size,
		out ISearchableVoronoi map
	) {
		HashSet<Cell> cells;
		IVoronoi landform;
		float terrainDensity = 0.0f;
		do {
			int cellSize = Math.Min( size.Width, size.Height ) / RoughCellCount;
			IVoronoi plates = _voronoiBuilder.Create( size, cellSize );

			// Find all the cells that fall on the edges of the tectonic plates
			cellSize /= 2;
			cells = [];
			landform = _voronoiBuilder.Create( size, cellSize );
			foreach( Edge edge in plates.Edges ) {
				if( ( edge.GetHashCode() & 0x1111 ) == 0x1111 ) {
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

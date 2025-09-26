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
	public const float MaximumDensity = 0.8f;

	private readonly IRandom _random;
	private readonly IVoronoiBuilder _voronoiBuilder;
	private readonly ISearchableVoronoiFactory _searchableVoronoiFactory;
	private readonly IBuilderMonitor? _monitor;

	public TectonicLandformBuilder(
		IRandom random,
		IVoronoiBuilder voronoiBuilder,
		ISearchableVoronoiFactory searchableVoronoiFactory,
		IBuilderMonitor? monitor
	) {
		_random = random;
		_voronoiBuilder = voronoiBuilder;
		_searchableVoronoiFactory = searchableVoronoiFactory;
		_monitor = monitor;
	}

	async Task<Landform> ILandformBuilder.CreateAsync(
		ISize size,
		TectonicPlates tectonicPlates,
		CancellationToken cancellationToken
	) {
		HashSet<Cell> cells;
		IVoronoi landform;
		float terrainDensity;
		int monitorStage = 1;
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

			if( _monitor is not null ) {
				await _monitor.LandformStageAsync( size, monitorStage, cells, cancellationToken ).ConfigureAwait( false );
				terrainDensity = (float)cells.Count / (float)landform.Cells.Count;
				await _monitor.LandformStageMessageAsync( $"Stage {monitorStage} density: {terrainDensity:P2}", cancellationToken ).ConfigureAwait( false );
			}

			while( cellSize >= ( MinimumCellSize * 3 ) ) {
				cellSize /= 3;
				monitorStage += 1;
				landform = _voronoiBuilder.Create( size, cellSize );
				ISearchableVoronoi searchableLandform = _searchableVoronoiFactory.Create( landform, size );

				HashSet<Cell> newCells = [];
				foreach( Cell cell in cells ) {
					IReadOnlyList<Cell> landformCells = searchableLandform.Search( cell.BoundingBox );
					foreach( Cell newCell in landformCells ) {
						/*
						if( !newCell.IsOpen ) {							
							foreach( Point p in newCell.Polygon.Points ) {
								if( cell.Polygon.Contains( p ) ) {
									newCells.Add( newCell );
									break;
								}
							}
						}
						*/
						if (!newCell.IsOpen
							&& newCell.Polygon.HasOverlap( cell.Polygon )
						) {
							newCells.Add( newCell );
						}
					}
				}

				cells = newCells;

				if( _monitor is not null ) {
					await _monitor.LandformStageAsync( size, monitorStage, cells, cancellationToken ).ConfigureAwait( false );
					terrainDensity = (float)cells.Count / (float)landform.Cells.Count;
					await _monitor.LandformStageMessageAsync( $"Stage {monitorStage} density: {terrainDensity:P2}", cancellationToken ).ConfigureAwait( false );
				}
			}

			terrainDensity = (float)cells.Count / (float)landform.Cells.Count;

		} while( terrainDensity < MinimumDensity
			|| terrainDensity > MaximumDensity
		);

		return new Landform(
			cells.ToHashSet(),
			_searchableVoronoiFactory.Create( landform, size )
		);
	}
}

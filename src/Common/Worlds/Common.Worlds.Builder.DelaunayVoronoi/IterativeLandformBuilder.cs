
namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class IterativeLandformBuilder : ILandformBuilder {

	/// <summary>
	/// The number of rough cells either horizontally or vertically across the
	/// map, using whichever dimension is smaller.
	/// </summary>
	/// <remarks>
	/// The size of these rough cells will be proportional to the size of the
	/// map divided by the number of cells.
	/// </remarks>
	public const int RoughCellCount = 20;

	/// <summary>
	/// The desired size of the cells on the final iteration of the generated
	/// landform map.
	/// </summary>
	/// <remarks>
	/// Map iterations continue until a landform map is generate where the cells
	/// are within one halving of this size. (ie - A value of 5 means all cells
	/// will be between 5 and 10)
	/// </remarks>
	public const int SmallestCellSize = 5;

	/// <summary>
	/// Determines how many of the cells in a generated voronoi will be considered
	/// for inclusion in the landform, in the range of 0.0 -> 1.0.
	/// </summary>
	/// <remarks>
	/// Higher values mean more of the map will be covered in land.
	/// </remarks>
	public const float LandformDensity = 0.15f;

	private readonly IRandom _random;
	private readonly IVoronoiBuilder _voronoiBuilder;

	public IterativeLandformBuilder(
		IRandom random,
		IVoronoiBuilder voronoiBuilder
	) {
		_random = random;
		_voronoiBuilder = voronoiBuilder;
	}

	IReadOnlySet<Cell> ILandformBuilder.Create(
		ISize size,
		out ISearchableVoronoi map
	) {
		int cellSize = Math.Min( size.Width, size.Height ) / RoughCellCount;
		HashSet<Cell> landforms = CreateLandform( size, cellSize );

		int distance = cellSize;
		do {
			// Create the finer landforms by generating a map with cells half
			// the size of the parent.  Then, check each of those finer cells
			// to see if they are contained by the rougher parent cell.  If they
			// are, they are part of the new iteration of the landform.
			// This means that the larger cells are the _largest_ possible area
			// and every refinement makes the landmass smaller, thus it cannot
			// spill outside of the desired area.
			distance /= 2;
			map = _voronoiBuilder.Create( size, distance );

			HashSet<Cell> finerLandforms = [];
			foreach( Cell roughCell in landforms ) {
				// Find all the fine cells that fall within the bounding box
				// of the rough cell.
				Rect bounds = roughCell.BoundingBox;
				IReadOnlyList<Cell> fineCells = map.Search( bounds );

				foreach( Cell fineCell in fineCells ) {
					if( roughCell.Polygon.Contains( fineCell.Center ) ) {
						// Only make this land if all of its neighbours are closed,
						// otherwise you'll have land with an Open water neighbour
						// which leads to weird degenerate cases when trying to
						// render the coast.
						bool openNeighbours = map.Neighbours[fineCell].Any( c => c.IsOpen );
						if( !openNeighbours ) {
							finerLandforms.Add( fineCell );
						}
					}
				}
			}

			landforms = finerLandforms;

			// Repeat until the cells are the desired size
		} while( distance >= 5 );

		return landforms;
	}

	private HashSet<Cell> CreateLandform(
		ISize size,
		int cellSize
	) {
		IVoronoi roughVoronoi = _voronoiBuilder.Create( size, cellSize );

		// Get the seeds of the landforms by finding all cells that are fully
		// contained within the diagram. (Only cells that go outside the bounds
		// of the ISize are open). These become the "candidiate" cells.
		List<Cell> cells = roughVoronoi.Cells.Where( c => !c.IsOpen ).ToList();
		int desiredCount = (int)( cells.Count * LandformDensity );
		HashSet<Cell> roughLandforms = [];
		while( roughLandforms.Count < desiredCount ) {
			// Guess a random point in the map and see if it falls inside a cell.
			Point location = new Point(
				_random.NextInt( size.Width ),
				_random.NextInt( size.Height )
			);
			Cell? target = null;
			// Check the candidate cells to see if that point is inside.
			foreach( Cell cell in cells ) {
				if( cell.Polygon.Contains( location ) ) {
					target = cell;
					break;
				}
			}
			// If we found a cell at that random point then try adding it
			// to the roughLandforms hashset and remove the added cell from the
			// set of candidate cells.
			if( target is not null ) {
				roughLandforms.Add( target );
				cells.Remove( target );
			}
		}

		// Add the landforms neighbours to beef the shape up. That is, for
		// ever cell we're adding, also add the neighbouring cells to "inflate"
		// the shape.
		HashSet<Cell> result = [];
		foreach( Cell seedCell in roughLandforms ) {
			result.Add( seedCell );
			IEnumerable<Cell> closedNeighbours = roughVoronoi.Neighbours[seedCell].Where( c => !c.IsOpen );
			foreach( Cell neighbourCell in closedNeighbours ) {
				result.Add( neighbourCell );
			}
		}
		roughLandforms = result;

		return roughLandforms;
	}
}

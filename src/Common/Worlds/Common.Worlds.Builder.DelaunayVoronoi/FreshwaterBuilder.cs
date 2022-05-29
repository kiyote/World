namespace Common.Worlds.Builder.DelaunayVoronoi;

internal class FreshwaterBuilder : IFreshwaterBuilder {
	HashSet<Cell> IFreshwaterBuilder.Create(
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> saltwater
	) {
		HashSet<Cell> freshwater = new HashSet<Cell>();
		foreach( Cell cell in fineVoronoi.Cells ) {
			if( !fineLandforms.Contains( cell ) ) {
				if( !saltwater.Contains( cell ) ) {
					freshwater.Add( cell );
				}
			}
		}

		return freshwater;
	}
}


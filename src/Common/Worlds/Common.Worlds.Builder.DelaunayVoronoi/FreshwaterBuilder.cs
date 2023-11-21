namespace Common.Worlds.Builder.DelaunayVoronoi;

internal class FreshwaterBuilder : IFreshwaterBuilder {
	HashSet<Cell> IFreshwaterBuilder.Create(
		IVoronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> saltwater
	) {
		HashSet<Cell> freshwater = [];
		foreach( Cell cell in fineVoronoi.Cells ) {
			if( !fineLandforms.Contains( cell )
				&& !saltwater.Contains( cell )
			) {
				freshwater.Add( cell );
			}
		}

		return freshwater;
	}
}


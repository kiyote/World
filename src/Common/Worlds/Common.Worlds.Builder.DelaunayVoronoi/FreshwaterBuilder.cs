namespace Common.Worlds.Builder.DelaunayVoronoi;

internal class FreshwaterBuilder : IFreshwaterBuilder {
	HashSet<Cell> IFreshwaterBuilder.Create(
		ISize size,
		IVoronoi map,
		HashSet<Cell> landform,
		HashSet<Cell> saltwater
	) {
		HashSet<Cell> freshwater = [];
		foreach( Cell cell in map.Cells ) {
			if( !landform.Contains( cell )
				&& !saltwater.Contains( cell )
			) {
				freshwater.Add( cell );
			}
		}

		return freshwater;
	}
}


﻿namespace Common.Worlds.Builder.DelaunayVoronoi;

internal class FreshwaterBuilder : IFreshwaterBuilder {
	IReadOnlySet<Cell> IFreshwaterBuilder.Create(
		ISize size,
		IVoronoi map,
		IReadOnlySet<Cell> landform,
		IReadOnlySet<Cell> saltwater
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


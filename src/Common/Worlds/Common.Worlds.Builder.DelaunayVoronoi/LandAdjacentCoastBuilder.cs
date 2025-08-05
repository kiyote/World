namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class LandAdjacentCoastBuilder : ICoastBuilder {
	IReadOnlySet<Cell> ICoastBuilder.Create(
		ISize size,
		ISearchableVoronoi map,
		IReadOnlySet<Cell> landform,
		IReadOnlySet<Cell> saltwater
	) {
		HashSet<Cell> coast = [];
		foreach( Cell land in landform ) {
			foreach( Cell neighbour in map.Neighbours[land] ) {
				if( saltwater.Contains( neighbour ) ) {
					coast.Add( neighbour );
				}
			}
		}

		return coast;
	}
}

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class HillsBuilder : IHillsBuilder {

	HashSet<Cell> IHillsBuilder.Create(
		IVoronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains
	) {
		HashSet<Cell> hills = new HashSet<Cell>();
		foreach( Cell mountain in mountains ) {
			foreach( Cell neighbour in fineVoronoi.Neighbours[mountain] ) {
				if( !mountains.Contains( neighbour ) && fineLandforms.Contains( neighbour ) ) {
					hills.Add( neighbour );
				}
			}
		}
		return hills;
	}
}

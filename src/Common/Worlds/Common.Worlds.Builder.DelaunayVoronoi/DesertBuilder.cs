namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class DesertBuilder : IDesertBuilder {
	HashSet<Cell> IDesertBuilder.Create(
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains,
		HashSet<Cell> hills,
		Dictionary<Cell, float> moistures
	) {
		HashSet<Cell> result = new HashSet<Cell>();

		foreach (Cell cell in fineLandforms) {
			float moisture = moistures[cell];
			if (mountains.Contains(cell)
				|| hills.Contains(cell)
			) {
				continue;
			}

			if (moisture < 0.1f) {
				result.Add( cell );
			}
		}

		return result;
	}
}

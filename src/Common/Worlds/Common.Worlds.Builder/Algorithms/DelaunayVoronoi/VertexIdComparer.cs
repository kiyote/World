namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

public class VertexIdComparer : IComparer<Vertex> {
	public int Compare(
		Vertex? x,
		Vertex? y
	) {
		if (
			x is null
			|| y is null
		) {
			throw new InvalidOperationException();
		}
		return x.Id.CompareTo( y.Id );
	}
}

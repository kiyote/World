namespace Common.Worlds.Builder.Algorithms.ConvexHull;

internal sealed record HullEdge(
	IPoint First,
	IPoint Second
) : IEdge {

	public HullEdge(
		IEdge edge
	) : this( edge.First, edge.Second ) {
	}
}

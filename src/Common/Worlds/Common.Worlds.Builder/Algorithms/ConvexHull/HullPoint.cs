namespace Common.Worlds.Builder.Algorithms.ConvexHull;

internal sealed record HullPoint( int X, int Y ) : IPoint {
	public HullPoint(
		IPoint point
	): this( point.X, point.Y ) {
	}
}


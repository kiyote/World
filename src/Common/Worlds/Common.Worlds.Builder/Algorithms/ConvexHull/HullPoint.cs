namespace Common.Worlds.Builder.Algorithms.ConvexHull;

internal sealed record HullPoint( float X, float Y ) : IPoint {
	public HullPoint(
		IPoint point
	): this( point.X, point.Y ) {
	}
}


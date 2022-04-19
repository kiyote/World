namespace Common.Worlds.Builder.Algorithms.Delaunay;

internal sealed record Triangle( Point A, Point B, Point C ) : ITriangle {
	IPoint ITriangle.A => A;

	IPoint ITriangle.B => B;

	IPoint ITriangle.C => C;
}


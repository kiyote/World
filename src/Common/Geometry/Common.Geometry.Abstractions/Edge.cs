namespace Common.Geometry;

public sealed class Edge : IEquatable<Edge> {

	public Edge(
		Point a,
		Point b
	) {
		A = a;
		B = b;
	}

	public Point A { get; init; }

	public Point B { get; init; }

	public bool Equals(
		Point a,
		Point b
	) {
		return ( A.Equals( a ) && B.Equals( b ) )
			|| ( A.Equals( b ) && B.Equals( a ) );
	}

	public bool Equals(
		Edge? other
	) {
		if( other is null ) {
			return false;
		}

		return Equals( other.A, other.B );
	}

	public override bool Equals(
		object? obj
	) {
		return Equals( obj as Edge );
	}

	public override int GetHashCode() {
		return HashCode.Combine( A, B );
	}
}

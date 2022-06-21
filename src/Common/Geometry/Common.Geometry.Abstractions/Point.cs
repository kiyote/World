namespace Common.Geometry;

public sealed class Point : IPoint {

	public Point(
		int x,
		int y
	) {
		X = x;
		Y = y;
	}

	public int X { get; init; }

	public int Y { get; init; }

	public bool Equals(
		int x,
		int y
	) {
		return X == x && Y == y;
	}

	public bool Equals(
		IPoint? other
	) {
		if( other is null ) {
			return false;
		}

		return Equals( other.X, other.Y );
	}

	public override bool Equals(
		object? obj
	) {
		if( obj is null ) {
			return false;
		}

		return Equals( obj as IPoint );
	}

	public override int GetHashCode() {
		return HashCode.Combine( X, Y );
	}
}



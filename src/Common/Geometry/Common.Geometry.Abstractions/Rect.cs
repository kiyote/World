namespace Common.Geometry;

public sealed class Rect : IRect {

	public Rect(
		IPoint topLeft,
		IPoint bottomRight
	) {
		TopLeft = topLeft;
		BottomRight = bottomRight;
	}

	public Rect(
		int topX,
		int topY,
		int bottomX,
		int bottomY
	) {
		TopLeft = new Point( topX, topY );
		BottomRight = new Point( bottomX, bottomY );
	}

	public IPoint TopLeft { get; init; }

	public IPoint BottomRight { get; init; }

	IPoint IRect.TopLeft => TopLeft;

	IPoint IRect.BottomRight => BottomRight;

	public bool Equals(
		IPoint topLeft,
		IPoint bottomRight
	) {
		return TopLeft == topLeft && BottomRight == bottomRight;
	}

	public bool Equals(
		IRect? other
	) {
		if( other is null ) {
			return false;
		}

		return Equals( other.TopLeft, other.BottomRight );
	}

	public override bool Equals(
		object? obj
	) {
		if( obj is null ) {
			return false;
		}

		return Equals( obj as IRect );
	}

	public override int GetHashCode() {
		return HashCode.Combine( TopLeft, BottomRight );
	}

	bool IRect.Contains(
		int x,
		int y
	) {
		if (x >= TopLeft.X
			&& x <= BottomRight.X
			&& y >= TopLeft.Y
			&& y <= BottomRight.Y
		) {
			return true;
		}

		return false;
	}
}



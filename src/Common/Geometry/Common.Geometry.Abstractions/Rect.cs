﻿namespace Common.Geometry;

public sealed class Rect : IRect {

	public Rect(
		IPoint topLeft,
		IPoint bottomRight
	) {
		TopLeft = topLeft;
		BottomRight = bottomRight;
		Width = bottomRight.X - topLeft.X;
		Height = bottomRight.Y - topLeft.Y;
	}

	public Rect(
		int topX,
		int topY,
		int bottomX,
		int bottomY
	) : this( new Point( topX, topY ), new Point( bottomX, bottomY ) ) {
	}

	public IPoint TopLeft { get; init; }

	public IPoint BottomRight { get; init; }

	public int Width { get; init; }

	public int Height { get; init; }

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

	public bool Contains(
		int x,
		int y
	) {
		if( x >= TopLeft.X
			&& x <= BottomRight.X
			&& y >= TopLeft.Y
			&& y <= BottomRight.Y
		) {
			return true;
		}

		return false;
	}

	public bool Intersects(
		IRect rect
	) {
		return TopLeft.X + Width >= rect.TopLeft.X
			 && TopLeft.X <= rect.TopLeft.X + rect.Width
			 && TopLeft.Y + Height >= rect.TopLeft.Y
			 && TopLeft.Y <= rect.TopLeft.Y + rect.Height;
	}

	public bool Contains(
		IRect rect
	) {
		return TopLeft.X <= rect.TopLeft.X
			&& TopLeft.Y <= rect.TopLeft.Y
			&& BottomRight.X >= rect.BottomRight.X
			&& BottomRight.Y >= rect.BottomRight.Y;
	}
}



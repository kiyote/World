﻿namespace Common.Geometry;

public interface IRect: IEquatable<IRect> {
	public IPoint TopLeft { get; }

	public IPoint BottomRight { get; }

	public int Width { get; }

	public int Height { get; }

	public bool Contains( int x, int y );

	public bool Intersects( IRect rect );

	public bool Contains( IRect rect );
}

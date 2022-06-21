namespace Common.Geometry;

public interface IRect {
	public IPoint TopLeft { get; }

	public IPoint BottomRight { get; }

	public bool Contains( int x, int y );
}

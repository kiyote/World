namespace Common.Geometry;

public interface IPoint : IEquatable<IPoint> {
	public int X { get; }

	public int Y { get; }
}

namespace Common.Worlds.Builder.DelaunayVoronoi;

public interface IPoint: IEquatable<IPoint> {
	public int X { get; }

	public int Y { get; }
}

namespace Common.Worlds.Builder.DelaunayVoronoi;

public interface IDelaunatorFactory {
	Delaunator Create( IEnumerable<IPoint> points );
}

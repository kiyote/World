namespace Common.Geometry.DelaunayVoronoi;

public interface IDelaunatorFactory {
	Delaunator Create( IEnumerable<IPoint> points );
}

namespace Common.Worlds.Builder.Algorithms.Delaunay;

public interface IDelaunayFactory {
	IReadOnlyList<IEdge> Create( IEnumerable<IPoint> points );
}

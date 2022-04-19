namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

public interface IVoronoiFactory {
	Voronoi Create(
		DelaunayGraph delaunay
	);
}

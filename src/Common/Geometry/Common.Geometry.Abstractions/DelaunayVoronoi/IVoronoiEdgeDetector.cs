using Common.Core;

namespace Common.Geometry.DelaunayVoronoi;

public interface IVoronoiEdgeDetector {
	HashSet<Cell> Find(
		Size size,
		Voronoi voronoi,
		VoronoiEdge edge
	);
}

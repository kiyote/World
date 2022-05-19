using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IVoronoiBuilder {

	Voronoi Create(
		Size size,
		int pointCount
	);
}

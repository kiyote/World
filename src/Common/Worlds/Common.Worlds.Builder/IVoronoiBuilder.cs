using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder;

internal interface IVoronoiBuilder {

	Voronoi Create(
		Size size,
		int pointCount
	);
}

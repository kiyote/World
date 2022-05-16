namespace Common.Geometry.DelaunayVoronoi;

public interface IVoronoiFactory {

	Voronoi Create(
		Delaunator delaunator,
		int width,
		int height
	);

}

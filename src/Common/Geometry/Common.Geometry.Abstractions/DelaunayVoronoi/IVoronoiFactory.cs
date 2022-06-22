namespace Common.Geometry.DelaunayVoronoi;

public interface IVoronoiFactory {

	IVoronoi Create(
		Delaunator delaunator,
		int width,
		int height
	);

}

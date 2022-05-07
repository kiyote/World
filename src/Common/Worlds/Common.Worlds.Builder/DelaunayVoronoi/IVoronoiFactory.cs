namespace Common.Worlds.Builder.DelaunayVoronoi;

public interface IVoronoiFactory {

	Voronoi Create(
		Delaunator delaunator,
		int width,
		int height
	);

	Voronoi Create(
		Delaunator delaunator,
		int width,
		int height,
		bool closeCells
	);

}

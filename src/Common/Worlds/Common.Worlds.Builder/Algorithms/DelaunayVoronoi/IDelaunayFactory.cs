namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;
public interface IDelaunayFactory {

	Delaunay Create(
		IList<Vertex> input
	);
}


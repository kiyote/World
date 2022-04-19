namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;
public interface IDelaunayFactory {

	DelaunayGraph Create(
		IList<Vertex> input
	);
}


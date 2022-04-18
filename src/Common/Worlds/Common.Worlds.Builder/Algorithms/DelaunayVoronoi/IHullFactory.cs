namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;
internal interface IHullFactory {

	Hull Create(
		IList<Vertex> input
	);
}

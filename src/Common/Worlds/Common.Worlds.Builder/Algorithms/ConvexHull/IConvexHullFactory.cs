namespace Common.Worlds.Builder.Algorithms.ConvexHull;

public interface IConvexHullFactory {
	IReadOnlyList<IEdge> Create(
		IEnumerable<IPoint> points
	);
}

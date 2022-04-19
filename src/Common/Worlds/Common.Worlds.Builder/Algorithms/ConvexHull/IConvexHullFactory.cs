namespace Common.Worlds.Builder.Algorithms.ConvexHull;

public interface IConvexHullFactory {
	IReadOnlyList<IPoint> Create(
		IEnumerable<IPoint> points
	);
}

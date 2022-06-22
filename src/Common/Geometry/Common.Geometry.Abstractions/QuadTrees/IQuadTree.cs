namespace Common.Geometry.QuadTrees;

public interface IQuadTree<T> where T : IRect {
	int Count { get; }

	//void ForEach( Action<QuadTreeNode<T>> action );

	void Insert( T item );

	IReadOnlyList<T> Query( IRect area );
}

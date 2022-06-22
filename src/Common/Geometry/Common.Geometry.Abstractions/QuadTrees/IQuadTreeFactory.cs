namespace Common.Geometry.QuadTrees;

public interface IQuadTreeFactory {
	IQuadTree<T> Create<T>( IRect area ) where T: IRect;
}

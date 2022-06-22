namespace Common.Geometry.QuadTrees;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class SimpleQuadTreeFactory : IQuadTreeFactory {
	IQuadTree<T> IQuadTreeFactory.Create<T>( IRect area ) {
		return new SimpleQuadTree<T>( area );
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Geometry.QuadTrees;

public class QuadTree<T> where T : IRect {
	/// <summary>
	/// The root QuadTreeNode
	/// </summary>
	private readonly QuadTreeNode<T> m_root;

	/// <summary>
	/// The bounds of this QuadTree
	/// </summary>
	private readonly IRect m_rectangle;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="rectangle"></param>
	public QuadTree( IRect rectangle ) {
		m_rectangle = rectangle;
		m_root = new QuadTreeNode<T>( m_rectangle );
	}

	/// <summary>
	/// Get the count of items in the QuadTree
	/// </summary>
	public int Count => m_root.Count;

	/// <summary>
	/// Insert the feature into the QuadTree
	/// </summary>
	/// <param name="item"></param>
	public void Insert( T item ) {
		m_root.Insert( item );
	}

	/// <summary>
	/// Query the QuadTree, returning the items that are in the given area
	/// </summary>
	/// <param name="area"></param>
	/// <returns></returns>
	public IReadOnlyList<T> Query( IRect area ) {
		return m_root.Query( area );
	}

	/// <summary>
	/// Do the specified action for each item in the quadtree
	/// </summary>
	/// <param name="action"></param>
	public void ForEach( Action<QuadTreeNode<T>> action ) {
		m_root.ForEach( action );
	}

}

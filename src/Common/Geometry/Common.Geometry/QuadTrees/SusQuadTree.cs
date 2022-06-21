namespace Common.Geometry.QuadTrees;

using System.Collections.Generic;
using System;

public class SusQuadTree<T> where T : IRect {

	private readonly List<T> _objects;
	private readonly SusQuadTree<T>?[] _nodes;
	private readonly IRect _area;
	private readonly int _level;
	private const int MaxLevel = 4;

	public SusQuadTree(
		IRect area
	) : this( area, 0 ) {
	}

	public SusQuadTree(
		IRect area,
		int level
	) {
		_level = level;
		_objects = new List<T>( 4 );
		_nodes = new SusQuadTree<T>[4];
		_area = area;
	}

	public void Insert(
		T obj
	) {
		if( _nodes[0] is not null ) {
			int index = GetIndex( obj );
			if( index != -1 ) {
				_nodes[index]?.Insert( obj );
				return;
			}
		}

		_objects.Add( obj );

		if( _objects.Count > 4
			&& _level < MaxLevel
		) {
			if( _nodes[0] is null ) {
				Split();
			}
			int i = 0;
			while( i < _objects.Count ) {
				int index = GetIndex( _objects[i] );
				if( index != -1 ) {
					T objToMove = _objects[i];
					_objects.Remove( objToMove );
					_nodes[index]?.Insert( objToMove );
				} else {
					i++;
				}
			}
		}
	}

	public IReadOnlyList<T> GetObjects(
		T obj
	) {
		List<T> all = new List<T>( _objects );
		all.AddRange( GetChildObjects( obj ) );
		all.Remove( obj );
		return all;
	}

	private IReadOnlyList<T> GetChildObjects(
		T obj
	) {
		if( _nodes[0] is null ) {
			return Array.Empty<T>();
		}

		int index = GetIndex( obj );
		if( index != -1 ) {
			return _nodes[index]?.GetObjects( obj ) ?? Array.Empty<T>();
		}
		List<T> all = new List<T>();
		for( int i = 0; i < _nodes.Length; i++ ) {
			all.AddRange( _nodes[i]?.GetObjects() ?? Array.Empty<T>() );
		}

		return all;
	}

	public IReadOnlyList<T> GetObjects() {
		List<T> all = new List<T>( _objects );
		if( _nodes[0] is not null ) {
			for( int i = 0; i < _nodes.Length; i++ ) {
				all.AddRange( _nodes[i]?.GetObjects() ?? Array.Empty<T>() );
			}
		}

		return all;
	}

	private void Split() {
		int subWidth = _area.Width / 2;
		int subHeight = _area.Height / 2;
		int x = _area.TopLeft.X;
		int y = _area.TopLeft.Y;

		_nodes[2] = new SusQuadTree<T>( new Rect( x + subWidth, y + subHeight, x + subWidth + subWidth, y + subHeight + subHeight ), _level + 1 );
		_nodes[3] = new SusQuadTree<T>( new Rect( x, y + subHeight, x + subWidth, y + subHeight + subHeight ), _level + 1 );
		_nodes[1] = new SusQuadTree<T>( new Rect( x, y, x + subWidth, x + subHeight ), _level + 1 );
		_nodes[0] = new SusQuadTree<T>( new Rect( x + subWidth, y, x + subWidth + subWidth, y + subHeight ), _level + 1 );
	}

	private int GetIndex(
		T obj
	) {
		int index = -1;
		int vMid = _area.TopLeft.X + ( _area.Width / 2 );
		int hMid = _area.TopLeft.Y + ( _area.Height / 2 );

		bool topQuadrant = obj.TopLeft.Y > hMid;
		bool bottomQuadrant = obj.TopLeft.Y < hMid && obj.TopLeft.Y + obj.Height < hMid;

		if( obj.TopLeft.X < vMid && obj.TopLeft.X + obj.Width < vMid ) {
			if( topQuadrant ) {
				index = 1;
			} else if( bottomQuadrant ) {
				index = 2;
			}
		} else if( obj.TopLeft.X > vMid ) {
			if( topQuadrant ) {
				index = 0;
			} else if( bottomQuadrant ) {
				index = 3;
			}
		}

		return index;
	}

}

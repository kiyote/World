namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

internal sealed class ConnectorList {
	private SimplexConnector? _first;
	private SimplexConnector? _last;

	public SimplexConnector? First => _first;

	/// <summary>
	/// Adds a face to the list.
	/// </summary>
	public void Add(
		SimplexConnector element
	) {
		if( _last != null ) {
			_last.Next = element;
		}
		element.Previous = _last;
		_last = element;
		if( _first == null ) {
			_first = element;
		}
	}

	/// <summary>
	/// Removes the element from the list.
	/// </summary>
	public void Remove(
		SimplexConnector connector
	) {
		if( connector.Previous != null ) {
			connector.Previous.Next = connector.Next;
		} else {
			_first = connector.Next;
		}

		if( connector.Next != null ) {
			connector.Next.Previous = connector.Previous;
		} else {
			_last = connector.Previous;
		}

		connector.Next = null;
		connector.Previous = null;
	}
}

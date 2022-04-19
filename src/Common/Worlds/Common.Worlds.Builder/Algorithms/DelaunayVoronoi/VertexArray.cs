namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

internal sealed class VertexArray {

	public static readonly VertexArray Empty = new VertexArray();

	private Vertex[] _items;
	private int _count;
	private int _capacity;

	public VertexArray() {
		_items = Array.Empty<Vertex>();
	}

	/// <summary>
	/// Number of elements present in the buffer.
	/// </summary>
	public int Count => _count;

	/// <summary>
	/// Get the i-th element.
	/// </summary>
	public Vertex this[int i] {
		get { return _items[i]; }
	}

	/// <summary>
	/// Adds a vertex to the buffer.
	/// </summary>
	public void Add(
		Vertex item
	) {
		EnsureCapacity();
		_items[_count++] = item;
	}

	/// <summary>
	/// Sets the Count to 0, otherwise does nothing.
	/// </summary>
	public void Clear() {
		_count = 0;
	}

	private void EnsureCapacity() {
		if( _count + 1 > _capacity ) {
			if( _capacity == 0 ) {
				_capacity = 4;
			} else {
				_capacity = 2 * _capacity;
			}
			Array.Resize( ref _items, _capacity );
		}
	}
}

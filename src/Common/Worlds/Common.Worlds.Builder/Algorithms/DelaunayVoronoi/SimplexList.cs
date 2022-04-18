namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

internal sealed class SimplexList {

	private SimplexWrap? _last;

	public SimplexWrap? First { get; set; }

	public void Clear() {
		First = null;
		_last = null;
	}

	public void Add(
		SimplexWrap face
	) {
		if( face.InList ) {
			if( First!.VerticesBeyond.Count < face.VerticesBeyond.Count ) {
				Remove( face );
				AddFirst( face );
			}
			return;
		}

		face.InList = true;

		if(
			First != null
			&& First.VerticesBeyond.Count < face.VerticesBeyond.Count
		) {
			First.Previous = face;
			face.Next = First;
			First = face;
		} else {
			if( _last != null ) {
				_last.Next = face;
			}

			face.Previous = _last;
			_last = face;

			if( First == null ) {
				First = face;
			}
		}
	}

	public void Remove(
		SimplexWrap face
	) {
		if( !face.InList ) {
			return;
		}

		face.InList = false;

		if( face.Previous != null ) {
			face.Previous.Next = face.Next;
		} else {
			First = face.Next;
		}

		if( face.Next != null ) {
			face.Next.Previous = face.Previous;
		} else {
			_last = face.Previous;
		}

		face.Next = null;
		face.Previous = null;
	}

	private void AddFirst(
		SimplexWrap face
	) {
		face.InList = true;
		First!.Previous = face;
		face.Next = First;
		First = face;
	}
}

namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

internal class SimplexConnector {
	private const int Dimension = 2;

	internal SimplexConnector() {
		Vertices = new int[Dimension];
	}

	public SimplexWrap? Face { get; set; }

	public int EdgeIndex { get; private set; }

	public int[] Vertices { get; private set; }

	public uint HashCode { get; private set; }

	public SimplexConnector? Previous { get; set; }

	public SimplexConnector? Next { get; set; }

	public void Update(
		SimplexWrap face,
		int edgeIndex,
		int dim
	) {
		Face = face;
		EdgeIndex = edgeIndex;

		uint hashCode = 31;

		Vertex[] vs = face.Vertices;
		for( int i = 0, c = 0; i < dim; i++ ) {
			if( i != edgeIndex ) {
				int v = vs[i].Id;
				Vertices[c++] = v;
				hashCode += unchecked((23 * hashCode) + (uint)v);
			}
		}

		HashCode = hashCode;
	}

	public static bool AreConnectable(
		SimplexConnector a,
		SimplexConnector b,
		int dim
	) {
		if( a.HashCode != b.HashCode ) {
			return false;
		}

		int n = dim - 1;
		int[] av = a.Vertices;
		int[] bv = b.Vertices;
		for( int i = 0; i < n; i++ ) {
			if( av[i] != bv[i] ) {
				return false;
			}
		}

		return true;
	}

	public static void Connect(
		SimplexConnector a,
		SimplexConnector b
	) {
		// The faces will never be null by the time we are connecting things
		a.Face!.AdjacentFaces[a.EdgeIndex] = b.Face!;
		b.Face!.AdjacentFaces[b.EdgeIndex] = a.Face!;
	}
}

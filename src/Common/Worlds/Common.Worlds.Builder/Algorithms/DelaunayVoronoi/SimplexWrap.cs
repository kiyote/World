namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

internal sealed class SimplexWrap {

	public SimplexWrap(
		int dimension,
		VertexBuffer beyondList
	) {
		AdjacentFaces = new SimplexWrap[dimension];
		VerticesBeyond = beyondList;
		Normal = new float[dimension];
		Vertices = new Vertex[dimension];
	}

	public SimplexWrap?[] AdjacentFaces { get; init; }

	public VertexBuffer VerticesBeyond { get; set; }

	public Vertex? FurthestVertex { get; set; }

	public Vertex[] Vertices { get; set; }

	public float[] Normal { get; init; }

	public bool IsNormalFlipped { get; set; }

	public float Offset { get; set; }

	public int Tag { get; set; }

	public SimplexWrap? Previous { get; set; }

	public SimplexWrap? Next { get; set; }

	public bool InList { get; set; }
}

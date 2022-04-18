namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

#pragma warning disable CA1819
public sealed class Simplex {

	public Simplex(
		int dimension
	) {

		Dimension = dimension;
		Adjacent = new Simplex[dimension];
		Normal = new float[dimension];
		Centroid = new float[dimension];
		Vertices = new Vertex[dimension];

	}

	public int Dimension { get; init; }

	/// <summary>
	/// The simplexs adjacent to this simplex
	/// For 2D a simplex will be a segment and it with have two adjacent segments joining it.
	/// For 3D a simplex will be a triangle and it with have three adjacent triangles joining it.
	/// </summary>
	public Simplex?[] Adjacent { get; init; }

	/// <summary>
	/// The vertices that make up the simplex.
	/// For 2D a face will be 2 vertices making a line.
	/// For 3D a face will be 3 vertices making a triangle.
	/// </summary>
	public Vertex[] Vertices { get; init; }

	/// <summary>
	/// The simplexs normal.
	/// </summary>
	public float[] Normal { get; init; }

	/// <summary>
	/// The simplexs centroid.
	/// </summary>
	public float[] Centroid { get; init; }

	public int Tag { get; set; }

	public bool IsNormalFlipped { get; set; }

	public bool Remove(
		Simplex simplex
	) {
		for( int i = 0; i < Adjacent.Length; i++ ) {
			if( Adjacent[i] is null ) {
				continue;
			}

			if( ReferenceEquals( Adjacent[i], simplex ) ) {
				Adjacent[i] = null;
				return true;
			}
		}

		return false;
	}

	public void CalculateCentroid() {
		Centroid[0] = ( Vertices[0].Position[0] + Vertices[1].Position[0] + Vertices[2].Position[0] ) / 3.0f;
		Centroid[1] = ( Vertices[0].Position[1] + Vertices[1].Position[1] + Vertices[2].Position[1] ) / 3.0f;
		Centroid[2] = ( Vertices[0].Position[2] + Vertices[1].Position[2] + Vertices[2].Position[2] ) / 3.0f;
	}
}
#pragma warning restore CA1819

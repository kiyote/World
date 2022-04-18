namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

internal static class MathHelper {

	/// <summary>
	/// Squared length of the vector.
	/// </summary>
	public static float LengthSquared(
		float[] x
	) {
		float norm = 0;
		for( int i = 0; i < x.Length; i++ ) {
			float t = x[i];
			norm += t * t;
		}
		return norm;
	}

	/// <summary>
	/// Subtracts vectors x and y and stores the result to target.
	/// </summary>
	public static void SubtractFast(
		float[] x,
		float[] y,
		float[] target
	) {
		int d = x.Length;
		for( int i = 0; i < d; i++ ) {
			target[i] = x[i] - y[i];
		}
	}

	/// <summary>
	/// Finds normal vector of a hyper-plane given by vertices.
	/// Stores the results to normalData.
	/// </summary>
	public static void FindNormalVector(
		Vertex[] vertices,
		float[] normal
	) {
		float[] ntX = new float[4];
		float[] ntY = new float[4];
		SubtractFast( vertices[1].Position, vertices[0].Position, ntX );
		SubtractFast( vertices[2].Position, vertices[1].Position, ntY );

		float[] x = ntX;
		float[] y = ntY;

		float nx = ( x[1] * y[2] ) - ( x[2] * y[1] );
		float ny = ( x[2] * y[0] ) - ( x[0] * y[2] );
		float nz = ( x[0] * y[1] ) - ( x[1] * y[0] );

		float norm = (float)Math.Sqrt( ( nx * nx ) + ( ny * ny ) + ( nz * nz ) );

		float f = 1.0f / norm;
		normal[0] = f * nx;
		normal[1] = f * ny;
		normal[2] = f * nz;
	}

	/// <summary>
	/// Check if the vertex is "visible" from the face.
	/// The vertex is "over face" if the return value is > Constants.PlaneDistanceTolerance.
	/// </summary>
	/// <returns>The vertex is "over face" if the result is positive.</returns>
	public static float GetVertexDistance(
		Vertex v,
		SimplexWrap f
	) {
		float[] normal = f.Normal;
		float[] p = v.Position;
		float distance = f.Offset;
		for( int i = 0; i < v.Dimension; i++ ) {
			distance += normal[i] * p[i];
		}
		return distance;
	}
}

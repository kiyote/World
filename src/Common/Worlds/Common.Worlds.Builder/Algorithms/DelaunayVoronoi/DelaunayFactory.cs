namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

internal class DelaunayFactory : IDelaunayFactory {

	DelaunayGraph IDelaunayFactory.Create(
		IList<Vertex> input
	) {
		int dimension = 2;
		List<DelaunayCell> cells = new List<DelaunayCell>();
		List<Vertex> vertices = new List<Vertex>();

		int count = input.Count;
		for( int i = 0; i < count; i++ ) {
			float lenSq = input[i].SqrMagnitude;

			float[] v = input[i].Position;
			Array.Resize( ref v, dimension + 1 );
			input[i].Position = v;

			input[i].Position[dimension] = lenSq;
		}

		IHullFactory hullFactory = new HullFactory();
		Hull hull = hullFactory.Create( input );
		/*
		var hull = new Hull( dimension + 1 );
		hull.Generate( input );
		*/

		for( int i = 0; i < count; i++ ) {
			float[] v = input[i].Position;
			Array.Resize( ref v, dimension );
			input[i].Position = v;
		}

		vertices.AddRange( hull.Vertices );

		count = hull.Simplexes.Count;

		for( int i = 0; i < count; i++ ) {

			Simplex simplex = hull.Simplexes[i];

			if( simplex.Normal[dimension] >= 0.0f ) {
				for( int j = 0; j < simplex.Adjacent.Length; j++ ) {
					Simplex? adjacentSimplex = simplex.Adjacent[j];
					if( adjacentSimplex is not null ) {
						adjacentSimplex.Remove( simplex );
					}
				}
			} else {
				DelaunayCell cell = CreateCell( simplex );
				cells.Add( cell );
			}
		}

		for( int i = 0; i < vertices.Count; i++ ) {
			vertices[i].Tag = i;
		}

		for( int i = 0; i < cells.Count; i++ ) {
			cells[i].CircumCenter.Id = i;
			cells[i].Simplex.Tag = i;
		}

		return new DelaunayGraph(
			vertices,
			cells,
			hull.Centroid
		);
	}

	private static DelaunayCell CreateCell(
		Simplex simplex
	) {
		// From MathWorld: http://mathworld.wolfram.com/Circumcircle.html
		float[,] matrixBuffer = new float[3, 3];
		Vertex[] verts = simplex.Vertices;

		// x, y, 1
		for( int i = 0; i < 3; i++ ) {
			matrixBuffer[i, 0] = verts[i].Position[0];
			matrixBuffer[i, 1] = verts[i].Position[1];
			matrixBuffer[i, 2] = 1;
		}

		float a = Determinant( matrixBuffer );

		// size, y, 1
		for( int i = 0; i < 3; i++ ) {
			matrixBuffer[i, 0] = verts[i].SqrMagnitude;
		}

		float dx = -Determinant( matrixBuffer );

		// size, x, 1
		for( int i = 0; i < 3; i++ ) {
			matrixBuffer[i, 1] = verts[i].Position[0];
		}

		float dy = Determinant( matrixBuffer );

		// size, x, y
		for( int i = 0; i < 3; i++ ) {
			matrixBuffer[i, 2] = verts[i].Position[1];
		}
		float c = -Determinant( matrixBuffer );

		float s = -1.0f / ( 2.0f * a );

		float[] circumCenter = new float[2];
		circumCenter[0] = s * dx;
		circumCenter[1] = s * dy;

		float radius = Math.Abs( s ) * (float)Math.Sqrt( ( dx * dx ) + ( dy * dy ) - ( 4.0 * a * c ) );

		return new DelaunayCell( simplex, new Vertex() { Position = circumCenter }, radius );
	}

	private static float Determinant(
		float[,] matrixBuffer
	) {
		float fCofactor00 = ( matrixBuffer[1, 1] * matrixBuffer[2, 2] ) - ( matrixBuffer[1, 2] * matrixBuffer[2, 1] );
		float fCofactor10 = ( matrixBuffer[1, 2] * matrixBuffer[2, 0] ) - ( matrixBuffer[1, 0] * matrixBuffer[2, 2] );
		float fCofactor20 = ( matrixBuffer[1, 0] * matrixBuffer[2, 1] ) - ( matrixBuffer[1, 1] * matrixBuffer[2, 0] );

		float fDet = ( matrixBuffer[0, 0] * fCofactor00 ) + ( matrixBuffer[0, 1] * fCofactor10 ) + ( matrixBuffer[0, 2] * fCofactor20 );

		return fDet;
	}
}



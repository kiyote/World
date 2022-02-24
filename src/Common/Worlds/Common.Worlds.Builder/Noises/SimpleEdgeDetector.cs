﻿namespace Common.Worlds.Builder.Noises;

internal class SimpleEdgeDetector : IEdgeDetector {

	private readonly INeighbourLocator _neighbourLocator;

	public SimpleEdgeDetector(
		INeighbourLocator neighbourLocator
	) {
		_neighbourLocator = neighbourLocator;
	}

	float[,] IEdgeDetector.Detect(
		ref float[,] source,
		float threshold
	) {
		int rows = source.GetLength( 0 );
		int columns = source.GetLength( 1 );
		float[,] result = new float[columns, rows];
		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				if( source[c, r] == 0.0f ) {
					bool isEdge = false;
					IEnumerable<(int x, int y)> neighbours = _neighbourLocator.GetNeighbours( columns, rows, c, r );
					foreach( (int x, int y) in neighbours ) {
						if( source[x, y] > threshold ) {
							isEdge = true;
						}
					}
					result[c, r] = isEdge ? 1.0f : 0.0f;
				}
			}
		}

		return result;
	}
}


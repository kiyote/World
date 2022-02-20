namespace Common.Worlds.Builder.Noises;

internal class SimpleNoiseInverter : INoiseInverter {
	float[,] INoiseInverter.Invert(
		float[,] source
	) {
		int rows = source.GetLength( 0 );
		int columns = source.GetLength( 1 );
		float[,] result = new float[columns, rows];
		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				float value = source[c, r];
				result[c, r] = 1.0f - value;
			}
		}

		return result;
	}
}

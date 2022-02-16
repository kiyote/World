namespace Server.Service.WorldBuilders;

internal class SimpleNoiseThresholder : INoiseThresholder {
	float[,] INoiseThresholder.Range(
		float[,] source,
		float min,
		float max
	) {
		int rows = source.GetLength( 0 );
		int columns = source.GetLength( 1 );
		float[,] result = new float[columns, rows];
		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				float value = source[c, r];
				if ((value >= min) && (value <= max)) {
					result[c, r] = 1.0f;
				} else {
					result[c, r] = 0.0f;
				}
			}
		}

		return result;
	}

	float[,] INoiseThresholder.Threshold(
		float[,] source,
		float threshold
	) {
		int rows = source.GetLength( 0 );
		int columns = source.GetLength( 1 );
		float[,] result = new float[columns, rows];
		for (int r = 0; r < rows; r++) {
			for (int c = 0; c < columns; c++) {
				float value = source[c, r];
				result[c, r] = value >= threshold ? 1.0f : 0.0f;
			}
		}

		return result;
	}
}

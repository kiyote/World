using Common.Worlds.Builder.Noises;

namespace Common.Worlds.Builder;

internal class RandomLandformGenerator : ILandformGenerator {

	public const int SeedCount = 100;
	private readonly IRandom _random;
	private readonly INoiseProvider _noiseProvider;
	private readonly INoiseOperator _noiseOperator;

	public RandomLandformGenerator(
		IRandom random,
		INoiseProvider noiseProvider,
		INoiseOperator noiseOperator
	) {
		_random = random;
		_noiseProvider = noiseProvider;
		_noiseOperator = noiseOperator;
	}

	/**
	 * Simplex version
	float[,] ILandformGenerator.Create(
		long seed,
		Size size,
		INeighbourLocator neighbourLocator,
		ref float[,] probabilityMask
	) {
		float[,] raw = _noiseProvider.Random( seed, size.Rows, size.Columns, 12.0f );

		raw = _noiseOperator.Add( ref raw, 2.0f, false );

		for (int i = 0; i < 1; i++) {
			for( int r = 0; r < size.Rows; r++ ) {
				for( int c = 0; c < size.Columns; c++ ) {
					float chance = (float)_random.NextDouble();
					if( chance > probabilityMask[c, r] ) {
						raw[c, r] -= 0.1f;
					}
				}
			}
		}

		return _noiseOperator.Normalize( ref raw );
	}
	*/

	Buffer<float> ILandformGenerator.Create(
		long seed,
		Size size,
		INeighbourLocator neighbourLocator,
		Buffer<float> probabilityMask
	) {
		var output = new Buffer<float>( size );
		( (ILandformGenerator)this ).Create( seed, size, neighbourLocator, probabilityMask, output );
		return output;
	}

	ILandformGenerator ILandformGenerator.Create(
		long seed,
		Size size,
		INeighbourLocator neighbourLocator,
		Buffer<float> probabilityMask,
		Buffer<float> output
	) {
		var swap1 = new Buffer<float>( output.Size );
		for( int i = 0; i < SeedCount; i++ ) {
			int c = _random.NextInt( size.Columns );
			int r = _random.NextInt( size.Rows );
			swap1[r][c] = 1.0f;
		}

		var swap2 = new Buffer<float>( output.Size );
		for( int i = 0; i < 100; i++) {
			Pass( size, neighbourLocator, swap1, probabilityMask, swap2 );
			// Swap the buffers so that `output` is the result of the operation
			(swap1, swap2) = (swap2, swap1);
		}
		output.CopyFrom( swap1 );

		return this;
	}

	private void Pass(
		Size size,
		INeighbourLocator neighbourLocator,
		Buffer<float> source,
		Buffer<float> probabilityMask,
		Buffer<float> output
	) {
		for( int r = 0; r < size.Rows; r++ ) {
			for( int c = 0; c < size.Columns; c++ ) {
				float value = source[r][c];

				if( value == 1.0f ) {
					double chance = probabilityMask[r][c] * _random.NextDouble();
					if (chance >= 0.25) {
						IEnumerable<(int column, int row)> neighbours = neighbourLocator.GetNeighbours( size.Columns, size.Rows, c, r );
						foreach( (int column, int row) in neighbours ) {
							output[row][column] = 1.0f;
						}
					}
				}
			}
		}
	}
}

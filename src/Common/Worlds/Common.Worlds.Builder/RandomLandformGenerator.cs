using Common.Worlds.Builder.Noises;

namespace Common.Worlds.Builder;

internal class RandomLandformGenerator : ILandformGenerator {

	public const int SeedCount = 1000;
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

	Buffer<float> ILandformGenerator.Create(
		long seed,
		Size size,
		INeighbourLocator neighbourLocator,
		Buffer<float> probabilityMask
	) {
		var output = new Buffer<float>( size );
		( this as ILandformGenerator ).Create( seed, size, neighbourLocator, probabilityMask, output );
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
		for( int i = 0; i < 50; i++) {
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
					if (chance >= 0.2) {
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

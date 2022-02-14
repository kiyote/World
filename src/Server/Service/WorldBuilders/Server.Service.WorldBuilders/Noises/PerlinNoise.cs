namespace Server.Service.WorldBuilders.Noises;

internal class PerlinNoise : INoiseProvider {

	private readonly IRandom _random;

	public PerlinNoise(
		IRandom random
	) {
		_random = random;
	}

	float[,] INoiseProvider.Generate(
		int rows,
		int columns
	) {
		int[] perm = new int[512];
		for (int i = 0; i < perm.Length; i++) {
			perm[i] = i;
		}
		perm = perm.OrderBy( item => _random.NextInt() ).ToArray();

		float[,] result = new float[columns, rows];
		for( int r = 0; r < rows; r++ ) {
			float fr = (float)r / (float)rows;
			for( int c = 0; c < columns; c++ ) {
				float fc = (float)c / (float)columns;
				float value = OctaveNoise( fc, fr, 4, 2, perm );
				result[c, r] = value;
			}
		}
		return result;
	}

	private static float OctaveNoise(
		float x,
		float y,
		int octaves,
		double persistence,
		int[] perm
	) {
		double total = 0;
		double frequency = 1;
		double amplitude = 1;
		double maxValue = 0;  // Used for normalizing result to 0.0 - 1.0
		for( int i = 0; i < octaves; i++ ) {
			total += Noise( (float)(x * frequency), (float)(y * frequency), perm ) * amplitude;

			maxValue += amplitude;

			amplitude *= persistence;
			frequency *= 2;
		}

		return (float)(total / maxValue);
	}

	private static float Noise(
		float x,
		float y,
		int[] perm
	) {
		int X = (int)Math.Floor( x ) & 0xff;
		int Y = (int)Math.Floor( y ) & 0xff;
		x -= (int)Math.Floor( x );
		y -= (int)Math.Floor( y );
		float u = Fade( x );
		float v = Fade( y );
		int A = ( perm[X] + Y ) & 0xff;
		int B = ( perm[X + 1] + Y ) & 0xff;
		return Lerp(
			v,
			Lerp(
				u,
				Grad( perm[A], x, y ),
				Grad( perm[B], x - 1, y )
			),
			Lerp(
				u,
				Grad( perm[A + 1], x, y - 1 ),
				Grad( perm[B + 1], x - 1, y - 1 )
			)
		);
	}

	private static float Fade(
		float t
	) {
		return t * t * t * ( ( t * ( ( t * 6 ) - 15 ) ) + 10 );
	}

	private static float Lerp(
		float t,
		float a,
		float b
	) {
		return a + ( t * ( b - a ) );
	}

	private static float Grad(
		int hash,
		float x,
		float y
	) {
		return ( ( hash & 1 ) == 0 ? x : -x ) + ( ( hash & 2 ) == 0 ? y : -y );
	}
}


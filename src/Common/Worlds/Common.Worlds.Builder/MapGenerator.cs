using Common.Worlds.Builder.Noises;

namespace Common.Worlds.Builder;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal class MapGenerator : IMapGenerator {

	private readonly IRandom _random;
	private readonly INoiseProvider _noiseProvider;
	private readonly INoiseOperator _noiseOperator;
	private readonly INoiseMaskGenerator _noiseMaskGenerator;

	public const float Frequency = 6.00f;

	public const float OceanMin = float.MinValue;
	public const float OceanMax = 0.2f;
	public const float CoastMin = OceanMax;
	public const float CoastMax = 0.4f;
	public const float GrassMin = CoastMax;
	public const float GrassMax = 0.6f;
	public const float HillMin = GrassMax;
	public const float HillMax = 0.8f;
	public const float MountainMin = HillMax;
	public const float MountainMax = float.MaxValue;

	public MapGenerator(
		IRandom random,
		INoiseProvider noiseProvider,
		INoiseOperator noiseOperator,
		INoiseMaskGenerator noiseMaskGenerator
	) {
		_random = random;
		_noiseProvider = noiseProvider;
		_noiseOperator = noiseOperator;
		_noiseMaskGenerator = noiseMaskGenerator;
	}

	TileTerrain[,] IMapGenerator.GenerateTerrain(
		string seed,
		Size size
	) {
		long code = Hash.GetLong( seed );
		return ( this as IMapGenerator ).GenerateTerrain( code, size );
	}

	TileTerrain[,] IMapGenerator.GenerateTerrain(
		long seed,
		Size size
	) {
		int columns = size.Columns;
		int rows = size.Rows;
		float[,] terrainMask = _noiseMaskGenerator.Circle( size );

		_random.Reinitialise( (int)seed );
		float[,] raw = _noiseProvider.Random( seed, rows, columns, 2.0f );
		float[,] higherOrder = _noiseProvider.Random( seed, rows, columns, 10.0f );
		//raw = _noiseOperator.Add( ref raw, ref higherOrder, true );
		raw = _noiseOperator.Multiply( ref raw, ref terrainMask, true );
		//raw = _noiseOperator.Normalize( ref raw );
		//raw = gradient;
		float[,] mountains = _noiseOperator.Range( ref raw, MountainMin, MountainMax );
		float[,] hills = _noiseOperator.Range( ref raw, HillMin, HillMax );
		float[,] grass = _noiseOperator.Range( ref raw, GrassMin, GrassMax );
		float[,] coast = _noiseOperator.Range( ref raw, CoastMin, CoastMax );
		float[,] grassEdges = _noiseOperator.EdgeDetect( ref grass, 0.0f );
		coast = _noiseOperator.Or( ref coast, 0.0f, ref grassEdges, 0.0f );
		float[,] ocean = _noiseOperator.Range( ref raw, OceanMin, OceanMax );

		// Seed in forests
		float[,] forest = _noiseProvider.Random( seed, rows, columns, 10.0f );
		forest = _noiseOperator.Range( ref forest, 0.55f, 1.00f );
		forest = _noiseOperator.And( ref grass, 0.0f, ref forest, 0.0f );

		TileTerrain[,] terrain = new TileTerrain[columns, rows];
		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				if( mountains[c, r] > 0.0f ) {
					terrain[c, r] = TileTerrain.Mountain;
				} else if( hills[c, r] > 0.0f ) {
					terrain[c, r] = TileTerrain.Hill;
				} else if( forest[c, r] > 0.0f ) {
					terrain[c, r] = TileTerrain.Forest;
				} else if( grass[c, r] > 0.0f ) {
					terrain[c, r] = TileTerrain.Grass;
				} else if( coast[c, r] > 0.0f ) {
					terrain[c, r] = TileTerrain.Coast;
				} else if( ocean[c, r] > 0.0f ) {
					terrain[c, r] = TileTerrain.Ocean;
				} else {
					throw new InvalidOperationException();
				}
			}
		}

		return terrain;
	}
}

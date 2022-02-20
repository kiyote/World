using Common.Worlds.Builder.Noises;

namespace Common.Worlds.Builder;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal class MapGenerator : IMapGenerator {

	private readonly IRandom _random;
	private readonly INoiseProvider _noiseProvider;
	private readonly INoiseThresholder _noiseThresholder;

	public const float Frequency = 2.0f;
	public const float Epsilon = 0.00001f;
	public const float MountainMin = HillMax + Epsilon;
	public const float HillMin = 0.75f;
	public const float HillMax = 0.9f;

	public MapGenerator(
		IRandom random,
		INoiseProvider noiseProvider,
		INoiseThresholder noiseThresholder
	) {
		_random = random;
		_noiseProvider = noiseProvider;
		_noiseThresholder = noiseThresholder;
	}

	public TileTerrain[,] GenerateTerrain(
		string seed,
		int rows,
		int columns
	) {
		int code = seed.GetHashCode( StringComparison.Ordinal );
		_random.Reinitialise( code );
		float[,] raw = _noiseProvider.Generate( ( code << 32 ) | code, rows, columns, Frequency );
		float[,] mountains = _noiseThresholder.Threshold( raw, MountainMin );
		float[,] hills = _noiseThresholder.Range( raw, HillMin, HillMax );

		TileTerrain[,] terrain = new TileTerrain[columns, rows];
		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				if( mountains[c, r] > 0.0f ) {
					terrain[c, r] = TileTerrain.Mountain;
				} else if( hills[c, r] > 0.0f ) {
					terrain[c, r] = TileTerrain.Hill;
				} else {
					terrain[c, r] = TileTerrain.Plain;
				}
			}
		}

		return terrain;
	}

}

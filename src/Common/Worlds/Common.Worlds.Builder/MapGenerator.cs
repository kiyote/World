using Common.Worlds.Builder.Noises;

namespace Common.Worlds.Builder;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal class MapGenerator : IMapGenerator {

	private readonly IRandom _random;
	private readonly INoiseProvider _noiseProvider;
	private readonly INoiseThresholder _noiseThresholder;
	private readonly IEdgeDetector _edgeDetector;
	private readonly ILogicalOperator _logicalOperator;

	public const float Frequency = 4.00f;
	public const float Epsilon = 0.00001f;

	public const float OceanMin = 0.00f;
	public const float OceanMax = 0.20f;
	public const float CoastMin = OceanMax + Epsilon;
	public const float CoastMax = 0.22f;
	public const float GrassMin = CoastMax + Epsilon;
	public const float GrassMax = 0.87f;
	public const float HillMin = GrassMax + Epsilon;
	public const float HillMax = 0.92f;
	public const float MountainMin = HillMax + Epsilon;
	public const float MountainMax = 1.00f;

	public MapGenerator(
		IRandom random,
		INoiseProvider noiseProvider,
		INoiseThresholder noiseThresholder,
		IEdgeDetector edgeDetector,
		ILogicalOperator logicalOperator
	) {
		_random = random;
		_noiseProvider = noiseProvider;
		_noiseThresholder = noiseThresholder;
		_edgeDetector = edgeDetector;
		_logicalOperator = logicalOperator;
	}

	public TileTerrain[,] GenerateTerrain(
		string seed,
		int rows,
		int columns
	) {
		long code = Hash.GetLong( seed );
		_random.Reinitialise( (int)code );
		float[,] raw = _noiseProvider.Generate( code, rows, columns, Frequency );
		float[,] mountains = _noiseThresholder.Range( ref raw, MountainMin, MountainMax );
		float[,] hills = _noiseThresholder.Range( ref raw, HillMin, HillMax );
		float[,] grass = _noiseThresholder.Range( ref raw, GrassMin, GrassMax );
		float[,] coast = _noiseThresholder.Range( ref raw, CoastMin, CoastMax );
		float[,] grassEdges = _edgeDetector.Detect( ref grass, 0.0f );
		coast = _logicalOperator.PerformOr( ref coast, 0.0f, ref grassEdges, 0.0f );
		float[,] ocean = _noiseThresholder.Range( ref raw, OceanMin, OceanMax );

		TileTerrain[,] terrain = new TileTerrain[columns, rows];
		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				if( mountains[c, r] > 0.0f ) {
					terrain[c, r] = TileTerrain.Mountain;
				} else if( hills[c, r] > 0.0f ) {
					terrain[c, r] = TileTerrain.Hill;
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

using Common.Worlds.Builder.Noises;

namespace Common.Worlds.Builder;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal class MapGenerator : IMapGenerator {

	private readonly IRandom _random;
	private readonly INoiseProvider _noiseProvider;
	private readonly INoiseOperator _noiseOperator;
	private readonly INoiseMaskGenerator _noiseMaskGenerator;
	private readonly ILandformGenerator _landformGenerator;
	private readonly INeighbourLocator _neighbourLocator;

	public const float Frequency = 6.00f;

	public const float OceanMin = float.MinValue;
	public const float OceanMax = 0.25f;
	public const float CoastMin = OceanMax;
	public const float CoastMax = CoastMin + 0.05f;
	public const float GrassMin = CoastMax;
	public const float GrassMax = 0.8f;
	public const float HillMin = GrassMax;
	public const float HillMax = 0.95f;
	public const float MountainMin = HillMax;
	public const float MountainMax = float.MaxValue;

	public MapGenerator(
		IRandom random,
		INoiseProvider noiseProvider,
		INoiseOperator noiseOperator,
		INoiseMaskGenerator noiseMaskGenerator,
		ILandformGenerator landformGenerator,
		INeighbourLocator neighbourLocator
	) {
		_random = random;
		_noiseProvider = noiseProvider;
		_noiseOperator = noiseOperator;
		_noiseMaskGenerator = noiseMaskGenerator;
		_landformGenerator = landformGenerator;
		_neighbourLocator = neighbourLocator;
	}

	Buffer<TileTerrain> IMapGenerator.GenerateTerrain(
		string seed,
		Size size
	) {
		long code = Hash.GetLong( seed );
		return ( this as IMapGenerator ).GenerateTerrain( code, size );
	}

	Buffer<TileTerrain> IMapGenerator.GenerateTerrain(
		long seed,
		Size size
	) {
		int columns = size.Columns;
		int rows = size.Rows;
		Buffer<float> shapeMask = _noiseMaskGenerator.Circle( size );

		_random.Reinitialise( (int)seed );

		Buffer<float> terrainMask = _landformGenerator.Create( seed, size, _neighbourLocator, shapeMask );

		Buffer<float> raw = _noiseProvider.Random( seed, size.Rows, size.Columns, 32.0f );
		_noiseOperator.Add( raw, shapeMask, false, raw );
		_noiseOperator.Mask( raw, terrainMask, 0.0f, raw );
//		_noiseOperator.Multiply( raw, shapeMask, false, raw );
		_noiseOperator.Normalize( raw, raw );
		_noiseOperator.Denoise( raw, raw );

		Buffer<float> coast = _noiseOperator.EdgeDetect( terrainMask, 0.1f );
		_noiseOperator.Mask( coast, terrainMask, 0.0f, coast );

		float[] bands = new float[] {
			0.3f,
			0.4f,
			0.6f,
			float.MaxValue
		};
		float level = 1.0f / (bands.Length + 1);

		/*
		raw = _noiseOperator.Multiply( ref raw, ref terrainMask, false );

		raw = _noiseOperator.Normalize( ref raw );
		raw = _noiseOperator.Quantize( ref raw, bands );
		*/
		Buffer<float> output = new Buffer<float>( raw.Size );
		_noiseOperator.Denoise( raw, output );
		raw = output;

		Buffer<TileTerrain> terrain = new Buffer<TileTerrain>( columns, rows );
		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				if( raw[r][c] >= (level * 4.0f) ) {
					terrain[r][c] = TileTerrain.Mountain;
				} else if( raw[r][c] >= (level * 3.0f) ) {
					terrain[r][c] = TileTerrain.Hill;
				} else if( raw[r][c] >= (level * 2.0f) ) {
					terrain[r][c] = TileTerrain.Grass;
				} else if(
					raw[r][c] >= (level * 1.0f)
					|| coast[r][c] == 1.0f
				) {
					terrain[r][c] = TileTerrain.Coast;
				} else {
					terrain[r][c] = TileTerrain.Ocean;
				}
			}
		}

		/*
		raw = _noiseOperator.Normalize( ref raw );
		TileTerrain[,] terrain = new TileTerrain[columns, rows];
		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				if( raw[c, r] > 0.0f ) {
					terrain[c, r] = TileTerrain.Grass;
				} else { 
					terrain[c, r] = TileTerrain.Ocean;
				}
			}
		}
		*/

		/*
		float[,] raw = _noiseProvider.Random( seed, rows, columns, 8.0f );
		float[,] higherOrder = _noiseProvider.Random( seed, rows, columns, 32.0f );
		higherOrder = _noiseOperator.Multiply( ref higherOrder, 0.5f, true );
		raw = _noiseOperator.Add( ref raw, ref higherOrder, true );
		raw = _noiseOperator.Multiply( ref raw, ref terrainMask, true );
		*/

		/*
		raw = _noiseOperator.Normalize( ref raw );
		float[,] mountains = _noiseOperator.Range( ref raw, MountainMin, MountainMax );
		float[,] hills = _noiseOperator.Range( ref raw, HillMin, HillMax );
		hills = _noiseOperator.BinaryFilter( ref hills );
		float[,] grass = _noiseOperator.Range( ref raw, GrassMin, GrassMax );
		grass = _noiseOperator.BinaryFilter( ref grass );
		float[,] coast = _noiseOperator.Range( ref raw, CoastMin, CoastMax );
		coast = _noiseOperator.BinaryFilter( ref coast );
		float[,] grassEdges = _noiseOperator.EdgeDetect( ref grass, 0.0f );
		coast = _noiseOperator.Or( ref coast, 0.0f, ref grassEdges, 0.0f );
		float[,] ocean = _noiseOperator.Range( ref raw, OceanMin, OceanMax );
		ocean = _noiseOperator.BinaryFilter( ref ocean );

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
		*/

		return terrain;
	}
}

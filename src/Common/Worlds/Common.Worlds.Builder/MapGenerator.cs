using Common.Buffer;
using Common.Buffer.Unit;
using Common.Worlds.Builder.Noises;

namespace Common.Worlds.Builder;

/*
[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal class MapGenerator : IMapGenerator {

	private readonly IRandom _random;
	private readonly INoiseProvider _noiseProvider;
	private readonly IFloatBufferArithmeticOperators _bufferOperators;
	private readonly INoiseMaskGenerator _noiseMaskGenerator;
	private readonly ILandformGenerator _landformGenerator;
	private readonly INeighbourLocator _neighbourLocator;
	private readonly IBufferFactory _bufferFactory;

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
		IFloatBufferArithmeticOperators bufferOperators,
		INoiseMaskGenerator noiseMaskGenerator,
		ILandformGenerator landformGenerator,
		INeighbourLocator neighbourLocator,
		IBufferFactory bufferFactory
	) {
		_random = random;
		_noiseProvider = noiseProvider;
		_bufferOperators = bufferOperators;
		_noiseMaskGenerator = noiseMaskGenerator;
		_landformGenerator = landformGenerator;
		_neighbourLocator = neighbourLocator;
		_bufferFactory = bufferFactory;
	}

	IBuffer<TileTerrain> IMapGenerator.GenerateTerrain(
		string seed,
		Size size
	) {
		long code = Hash.GetLong( seed );
		return ( this as IMapGenerator ).GenerateTerrain( code, size );
	}

	IBuffer<TileTerrain> IMapGenerator.GenerateTerrain(
		long seed,
		Size size
	) {
		int columns = size.Columns;
		int rows = size.Rows;

		_random.Reinitialise( (int)seed );

		IBuffer<float> terrainMask = _landformGenerator.Create( seed, size, _neighbourLocator );

		//Buffer<float> raw = _noiseProvider.Random( seed, size.Rows, size.Columns, 32.0f );
		IBuffer<float> raw = _bufferFactory.Create<float>( size );
		_bufferOperators.Add( raw, 1.0f, true, raw );
		//_noiseOperator.Add( raw, shapeMask, false, raw );
		_bufferOperator.Mask( raw, terrainMask, 0.0f, raw );
//		_noiseOperator.Multiply( raw, shapeMask, false, raw );
		_bufferOperator.Normalize( raw, raw );
		_bufferOperator.Denoise( raw, raw );

		IBuffer<float> coast = _bufferOperator.EdgeDetect( terrainMask, 0.1f );
		_bufferOperator.Mask( coast, terrainMask, 0.0f, coast );

		float[] bands = new float[] {
			0.3f,
			0.4f,
			0.6f,
			float.MaxValue
		};
		float level = 1.0f / (bands.Length + 1);

		IBuffer<float> output = _bufferFactory.Create<float>( raw.Size );
		_bufferOperator.Denoise( raw, output );
		raw = output;

		IBuffer<TileTerrain> terrain = _bufferFactory.Create<TileTerrain>( columns, rows );
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


		return terrain;
	}
}
*/

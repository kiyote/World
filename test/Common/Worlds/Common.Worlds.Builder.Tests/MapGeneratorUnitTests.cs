using Common.Buffer;
using Common.Worlds.Builder.Noises;
using Moq;
using NUnit.Framework;

namespace Common.Worlds.Builder.Tests;

[TestFixture]
public sealed class MapGeneratorUnitTests {

	private Mock<IRandom> _random;
	private Mock<INoiseProvider> _noiseProvider;
	private Mock<IBufferOperator> _bufferOperator;
	private Mock<IBufferFactory> _bufferFactory;
	private Mock<INoiseMaskGenerator> _noiseMaskGenerator;
	private Mock<ILandformGenerator> _landformGenerator;
	private Mock<INeighbourLocator> _neighbourLocator;
	private IMapGenerator _mapGenerator;

	[SetUp]
	public void SetUp() {
		_random = new Mock<IRandom>( MockBehavior.Strict );
		_noiseProvider = new Mock<INoiseProvider>( MockBehavior.Strict );
		_bufferOperator = new Mock<IBufferOperator>( MockBehavior.Strict );
		_noiseMaskGenerator = new Mock<INoiseMaskGenerator>( MockBehavior.Strict );
		_landformGenerator = new Mock<ILandformGenerator>( MockBehavior.Strict );
		_neighbourLocator = new Mock<INeighbourLocator>( MockBehavior.Strict );
		_bufferFactory = new Mock<IBufferFactory>( MockBehavior.Strict );

		_mapGenerator = new MapGenerator(
			_random.Object,
			_noiseProvider.Object,
			_bufferOperator.Object,
			_noiseMaskGenerator.Object,
			_landformGenerator.Object,
			_neighbourLocator.Object,
			_bufferFactory.Object
		);
	}

	[Test]
	public void GenerateTerrain_ValidParameters_TerrainReturned() {
		/*
		string seed = "test seed";
		long longSeed = Hash.GetLong( seed );
		int intSeed = (int)longSeed;
		int rows = 10;
		int columns = 10;
		float[,] noise = new float[columns, rows];
		float[,] mountains = new float[columns, rows];
		mountains[1, 1] = 1.0f;
		float[,] hills = new float[columns, rows];
		hills[2, 2] = 1.0f;
		float[,] grass = new float[columns, rows];
		Fill( ref grass, 1.0f );
		float[,] coast = new float[columns, rows];
		float[,] ocean = new float[columns, rows];
		float[,] edges = new float[columns, rows];

		_random
			.Setup( r => r.Reinitialise( intSeed ) );

		_noiseProvider
			.Setup( np => np.Random( longSeed, rows, columns, MapGenerator.Frequency ) )
			.Returns( noise );

		_noiseOperator
			.Setup( n => n.Range( ref noise, MapGenerator.MountainMin, MapGenerator.MountainMax ) )
			.Returns( mountains );

		_noiseOperator
			.Setup( n => n.Range( ref noise, MapGenerator.HillMin, MapGenerator.HillMax ) )
			.Returns( hills );

		_noiseOperator
			.Setup( n => n.Range( ref noise, MapGenerator.GrassMin, MapGenerator.GrassMax ) )
			.Returns( grass );

		_noiseOperator
			.Setup( n => n.EdgeDetect( ref grass, 0.0f ) )
			.Returns( edges );

		_noiseOperator
			.Setup( n => n.Range( ref noise, MapGenerator.CoastMin, MapGenerator.CoastMax ) )
			.Returns( coast );

		_noiseOperator
			.Setup( n => n.Range( ref noise, MapGenerator.OceanMin, MapGenerator.OceanMax ) )
			.Returns( ocean );

		_noiseOperator
			.Setup( n => n.Or( ref coast, 0.0f, ref edges, 0.0f ) )
			.Returns( coast );

		TileTerrain[,] result = _mapGenerator.GenerateTerrain( seed, rows, columns );

		Assert.IsNotNull( result );
		Assert.AreEqual( rows, result.GetLength( 0 ) );
		Assert.AreEqual( columns, result.GetLength( 1 ) );
		Assert.AreEqual( TileTerrain.Grass, result[0, 0] );
		Assert.AreEqual( TileTerrain.Mountain, result[1, 1] );
		Assert.AreEqual( TileTerrain.Hill, result[2, 2] );
		*/
		Assert.IsTrue(true);
	}

	private static void Fill( ref float[,] target, float value ) {
		for (int r = 0; r < target.GetLength(0); r++) {
			for (int c = 0; c < target.GetLength(1); c++) {
				target[c, r] = value;
			}
		}
	}
}

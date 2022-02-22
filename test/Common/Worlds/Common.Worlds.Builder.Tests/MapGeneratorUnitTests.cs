using Common.Worlds.Builder.Noises;
using Moq;
using NUnit.Framework;

namespace Common.Worlds.Builder.Tests;

[TestFixture]
public sealed class MapGeneratorUnitTests {

	private Mock<IRandom> _random;
	private Mock<INoiseProvider> _noiseProvider;
	private Mock<INoiseThresholder> _noiseThresholder;
	private IMapGenerator _mapGenerator;

	[SetUp]
	public void SetUp() {
		_random = new Mock<IRandom>( MockBehavior.Strict );
		_noiseProvider = new Mock<INoiseProvider>( MockBehavior.Strict );
		_noiseThresholder = new Mock<INoiseThresholder>( MockBehavior.Strict );	

		_mapGenerator = new MapGenerator(
			_random.Object,
			_noiseProvider.Object,
			_noiseThresholder.Object
		);
	}

	[Test]
	public void GenerateTerrain_ValidParameters_TerrainReturned() {
		string seed = "test seed";
		int intSeed = seed.GetHashCode( StringComparison.Ordinal );
		long longSeed = (intSeed << 32) | intSeed;
		int rows = 10;
		int columns = 10;
		float[,] noise = new float[columns, rows];
		float[,] mountains = new float[columns, rows];
		mountains[1, 1] = 1.0f;
		float[,] hills = new float[columns, rows];
		hills[2, 2] = 1.0f;

		_random
			.Setup( r => r.Reinitialise( intSeed ) );

		_noiseProvider
			.Setup( np => np.Generate( longSeed, rows, columns, MapGenerator.Frequency ) )
			.Returns( noise );

		_noiseThresholder
			.Setup( nt => nt.Threshold( noise, MapGenerator.MountainMin ) )
			.Returns( mountains );

		_noiseThresholder
			.Setup( nt => nt.Range( noise, MapGenerator.HillMin, MapGenerator.HillMax ) )
			.Returns( hills );

		TileTerrain[,] result = _mapGenerator.GenerateTerrain( seed, rows, columns );

		Assert.IsNotNull( result );
		Assert.AreEqual( rows, result.GetLength( 0 ) );
		Assert.AreEqual( columns, result.GetLength( 1 ) );
		Assert.AreEqual( TileTerrain.Plain, result[0, 0] );
		Assert.AreEqual( TileTerrain.Mountain, result[1, 1] );
		Assert.AreEqual( TileTerrain.Hill, result[2, 2] );
	}
}

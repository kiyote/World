using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Common.Worlds.Builder.Noises;

namespace Common.Worlds.Builder;

public class SimpleNoiseThresholderTests {

	private INoiseProvider _noise;
	private INoiseThresholder _thresholder;

	[SetUp]
	public void Setup() {
		_noise = new OpenSimplexNoise();

		_thresholder = new SimpleNoiseThresholder();
	}

	[Test]
	public void Threshold_OnePixelAboveThreshold_OnlyOneSet() {
		float[,] noise = new float[2, 2];
		noise[0, 0] = 1.0f;
		noise[1, 0] = 0.75f;
		noise[0, 1] = 0.25f;
		noise[1, 1] = 0.0f;

		float[,] result = _thresholder.Threshold( noise, 0.76f );

		Assert.AreEqual( 1.0f, result[0, 0] );
		Assert.AreEqual( 0.0f, result[1, 0] );
		Assert.AreEqual( 0.0f, result[0, 1] );
		Assert.AreEqual( 0.0f, result[1, 1] );
	}

	[Test]
	public void Range_OnePixelInRange_OnlyOneSet() {
		float[,] noise = new float[2, 2];
		noise[0, 0] = 1.0f;
		noise[1, 0] = 0.75f;
		noise[0, 1] = 0.25f;
		noise[1, 1] = 0.0f;
		float[,] result = _thresholder.Range( noise, 0.75f, 0.99f );

		Assert.AreEqual( 0.0f, result[0, 0] );
		Assert.AreEqual( 1.0f, result[1, 0] );
		Assert.AreEqual( 0.0f, result[0, 1] );
		Assert.AreEqual( 0.0f, result[1, 1] );
	}

	[Test]
	[Ignore( "Used to generate visible output." )]
	public void Generate_Image() {
		int width = 512;
		int height = 512;

		FastRandom random = new FastRandom();
		long seed = random.NextLong();

		float[,] noise = _noise.Generate( seed, height, width, 2.0f );
		float[,] threshold = _thresholder.Threshold( noise, 0.75f );

		using var img = new Image<Rgb24>( width, height );
		for( int r = 0; r < height; r++ ) {
			for( int c = 0; c < width; c++ ) {
				float value = noise[c, r];

				img[c, r] = new Rgb24( (byte)( 255.0f * value ), (byte)( 255.0f * value ), (byte)( 255.0f * value ) );
			}
		}
		img.SaveAsBmp( @"c:\temp\noise_threshold_orig.bmp" );

		using var thr = new Image<Rgb24>( width, height );
		for( int r = 0; r < height; r++ ) {
			for( int c = 0; c < width; c++ ) {
				float value = threshold[c, r];

				thr[c, r] = new Rgb24( (byte)( 255.0f * value ), (byte)( 255.0f * value ), (byte)( 255.0f * value ) );
			}
		}
		thr.SaveAsBmp( @"c:\temp\noise_threshold_threshold.bmp" );
	}
}

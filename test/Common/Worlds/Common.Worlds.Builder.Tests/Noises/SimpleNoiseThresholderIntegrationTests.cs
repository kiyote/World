using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Common.Worlds.Builder.Noises.Test;

public class SimpleNoiseThresholderIntegrationTests {

	private INoiseProvider _noise;
	private INoiseOperator _operator;

	[SetUp]
	public void Setup() {
		_noise = new OpenSimplexNoise();

		_operator = new SimpleNoiseOperator( null );
	}

	[Test]
	public void Threshold_OnePixelAboveThreshold_OnlyOneSet() {
		var noise = new Buffer<float>(2, 2);
		noise[0][0] = 1.0f;
		noise[1][0] = 0.75f;
		noise[0][1] = 0.25f;
		noise[1][1] = 0.0f;

		Buffer<float> result = _operator.GateHigh( noise, 0.76f );

		Assert.AreEqual( 1.0f, result[0][0] );
		Assert.AreEqual( 0.0f, result[1][0] );
		Assert.AreEqual( 0.0f, result[0][1] );
		Assert.AreEqual( 0.0f, result[1][1] );
	}

	[Test]
	public void Range_OnePixelInRange_OnlyOneSet() {
		var noise = new Buffer<float>( 2, 2 );
		noise[0][0] = 1.0f;
		noise[1][0] = 0.75f;
		noise[0][1] = 0.25f;
		noise[1][1] = 0.0f;
		Buffer<float> result = _operator.Range( noise, 0.74f, 0.9f );

		Assert.AreEqual( 0.0f, result[0][0] );
		Assert.AreEqual( 1.0f, result[1][0] );
		Assert.AreEqual( 0.0f, result[0][1] );
		Assert.AreEqual( 0.0f, result[1][1] );
	}

	[Test]
	[Ignore( "Used to generate visible output." )]
	public void Generate_Image() {
		int width = 512;
		int height = 512;

		FastRandom random = new FastRandom();
		long seed = random.NextLong();

		Buffer<float> noise = _noise.Random( seed, height, width, 2.0f );
		Buffer<float> threshold = _operator.GateHigh( noise, 0.75f );

		using var img = new Image<Rgb24>( width, height );
		for( int r = 0; r < height; r++ ) {
			for( int c = 0; c < width; c++ ) {
				float value = noise[r][c];

				img[c, r] = new Rgb24( (byte)( 255.0f * value ), (byte)( 255.0f * value ), (byte)( 255.0f * value ) );
			}
		}
		img.SaveAsBmp( @"c:\temp\noise_threshold_orig.bmp" );

		using var thr = new Image<Rgb24>( width, height );
		for( int r = 0; r < height; r++ ) {
			for( int c = 0; c < width; c++ ) {
				float value = threshold[r][c];

				thr[c, r] = new Rgb24( (byte)( 255.0f * value ), (byte)( 255.0f * value ), (byte)( 255.0f * value ) );
			}
		}
		thr.SaveAsBmp( @"c:\temp\noise_threshold_threshold.bmp" );
	}
}

using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Common.Worlds.Builder.Noises.Tests;

public class OpenSimplexNoiseTests {

	private INoiseProvider _noise;

	[SetUp]
	public void Setup() {
		_noise = new OpenSimplexNoise();
	}

	[Test]
	[Ignore( "Used to generate visible output." )]
	public void Generate_Image() {
		int width = 512;
		int height = 512;

		FastRandom random = new FastRandom();
		long seed = random.NextLong();

		using var img = new Image<Rgb24>( width, height );
		Buffer<float> noise = _noise.Random( seed, height, width, 2.0f );
		for( int r = 0; r < height; r++ ) {
			for( int c = 0; c < width; c++ ) {
				float value = noise[r][c];

				img[c, r] = new Rgb24( (byte)( 255.0f * value ), (byte)( 255.0f * value ), (byte)( 255.0f * value ) );
			}
		}
		img.SaveAsBmp( @"c:\temp\noise.bmp" );
	}
}

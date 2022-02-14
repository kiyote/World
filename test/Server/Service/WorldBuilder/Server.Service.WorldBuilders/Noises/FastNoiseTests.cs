using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Server.Service.WorldBuilders.Noises;

public class FastNoiseTests {

	private INoiseProvider _noise;

	[SetUp]
	public void Setup() {
		_noise = new FastNoiseLite();
	}

	[Test]
	[Ignore("Produces actual image output.")]
	public void Generate_Image() {
		int width = 512;
		int height = 512;

		using var img = new Image<Rgb24>( width, height );
		float[,] noise = _noise.Generate( height, width );
		for( int r = 0; r < height; r++ ) {
			for( int c = 0; c < width; c++ ) {
				float value = noise[c, r];
				value++;
				value /= 2.0f;

				img[c, r] = new Rgb24( (byte)( 255 * value ), (byte)( 255 * value ), (byte)( 255 * value ) );
			}
		}
		img.SaveAsBmp( @"c:\temp\noise.bmp" );
	}
}

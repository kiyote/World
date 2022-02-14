using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Server.Service.WorldBuilders.Noises;

public class PerlinNoiseTests {

	private INoiseProvider _noise;

	[SetUp]
	public void Setup() {
		IRandom random = new FastRandom();
		_noise = new PerlinNoise( random );
	}

	[Test]
	[Ignore("Used to generate visible output.")]
	public void Generate_Image() {
		int width = 512;
		int height = 512;

		using var img = new Image<Rgb24>( width, height );
		float[,] noise = _noise.Generate( height, width );
		for( int r = 0; r < height; r++ ) {
			for( int c = 0; c < width; c++ ) {
				float value = noise[c, r];

				img[c, r] = new Rgb24( (byte)(255 * value), (byte)(255 * value), (byte)(255 * value) );
			}
		}
		img.SaveAsBmp( @"c:\temp\noise.bmp" );
	}
}

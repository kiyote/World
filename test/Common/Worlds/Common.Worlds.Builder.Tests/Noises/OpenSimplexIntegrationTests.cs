using Common.Buffer;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Common.Worlds.Builder.Noises.Tests;

public class OpenSimplexNoiseIntegrationTests {

	private IServiceProvider _provider;
	private IServiceScope _scope;

	private INoiseProvider _noise;
	private IBufferFactory _bufferFactory;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		var services = new ServiceCollection();
		services.AddArrayBufferFactory();
		_provider = services.BuildServiceProvider();
	}

	[SetUp]
	public void Setup() {
		_scope = _provider.CreateScope();
		_bufferFactory = _scope.ServiceProvider.GetRequiredService<IBufferFactory>();

		_noise = new OpenSimplexNoise();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	[Ignore( "Used to generate visible output." )]
	public void Generate_Image() {
		int width = 512;
		int height = 512;

		FastRandom random = new FastRandom();
		long seed = random.NextLong();

		using var img = new Image<Rgb24>( width, height );
		IBuffer<float> noise = _bufferFactory.Create<float>( width, height );
		_noise.Random( seed, 2.0f, noise );
		for( int r = 0; r < height; r++ ) {
			for( int c = 0; c < width; c++ ) {
				float value = noise[r][c];

				img[c, r] = new Rgb24( (byte)( 255.0f * value ), (byte)( 255.0f * value ), (byte)( 255.0f * value ) );
			}
		}
		img.SaveAsBmp( @"c:\temp\noise.bmp" );
	}
}

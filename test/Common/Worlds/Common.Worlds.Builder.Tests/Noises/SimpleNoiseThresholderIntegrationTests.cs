using Common.Buffer;
using Common.Buffer.FloatingPoint;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Common.Worlds.Builder.Noises.Test;

public class SimpleNoiseThresholderIntegrationTests {

	private IServiceProvider _provider;
	private IServiceScope _scope;

	private INoiseProvider _noise;
	private IFloatBufferFilterOperators _filterOperators;
	private IFloatBufferClippingOperators _clippingOperators;
	private IBufferFactory _bufferFactory;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		var services = new ServiceCollection();
		services.AddArrayBuffer();
		services.AddFloatingPointBufferOperators();
		services.AddWorldBuilder();

		_provider = services.BuildServiceProvider();
	}

	[SetUp]
	public void Setup() {
		_scope = _provider.CreateScope();
		_filterOperators = _scope.ServiceProvider.GetRequiredService<IFloatBufferFilterOperators>();
		_clippingOperators = _scope.ServiceProvider.GetRequiredService<IFloatBufferClippingOperators>();
		_bufferFactory = _scope.ServiceProvider.GetRequiredService<IBufferFactory>();

		_noise = new OpenSimplexNoise();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	public void Threshold_OnePixelAboveThreshold_OnlyOneSet() {
		IBuffer<float> noise = _bufferFactory.Create<float>(2, 2);
		noise[0][0] = 1.0f;
		noise[1][0] = 0.75f;
		noise[0][1] = 0.25f;
		noise[1][1] = 0.0f;

		IBuffer<float> result = _filterOperators.GateHigh( noise, 0.76f );

		Assert.AreEqual( 1.0f, result[0][0] );
		Assert.AreEqual( 0.0f, result[1][0] );
		Assert.AreEqual( 0.0f, result[0][1] );
		Assert.AreEqual( 0.0f, result[1][1] );
	}

	[Test]
	public void Range_OnePixelInRange_OnlyOneSet() {
		IBuffer<float> noise = _bufferFactory.Create<float>( 2, 2 );
		noise[0][0] = 1.0f;
		noise[1][0] = 0.75f;
		noise[0][1] = 0.25f;
		noise[1][1] = 0.0f;
		IBuffer<float> result = _clippingOperators.Range( noise, 0.74f, 0.9f );

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

		IBuffer<float> noise = _bufferFactory.Create<float>( width, height );
		_noise.Random( seed, 2.0f, noise );
		IBuffer<float> threshold = _filterOperators.GateHigh( noise, 0.75f );

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

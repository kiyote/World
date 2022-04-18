using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Common.Worlds.Builder.Generators.Tests;

[TestFixture]
public sealed class VoronoiGeneratorIntegrationTests {

	private string _folder;
	private IRandom _random;
	private IServiceScope _scope;
	private IServiceProvider _provider;
	private VoronoiGenerator _generator;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string testRoot = Path.Combine( Path.GetTempPath(), "world" );
		Directory.CreateDirectory( testRoot );
		_folder = Path.Combine( testRoot, Guid.NewGuid().ToString( "N" ) );
		Directory.CreateDirectory( _folder );

		var services = new ServiceCollection();
		services.AddCore();

		_provider = services.BuildServiceProvider();
	}

	[OneTimeTearDown]
	public void OneTimeTearDown() {
		Directory.Delete( _folder, true );
	}

	[SetUp]
	public void SetUp() {
		_scope = _provider.CreateScope();

		_random = _scope.ServiceProvider.GetRequiredService<IRandom>();

		_generator = new VoronoiGenerator(
			_random
		);
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
		_scope = null;
	}

	[Test]
	[Ignore( "Used to generate visual output for inspection." )]
	public void Visualize() {
		int cells = 2000;
		Core.Size size = new Core.Size( 1000, 1000 );
		Buffer<float> result = _generator.Generate( size, cells );

		Rgba32[] colours = new Rgba32[cells];
		for (int i = 0; i < colours.Length; i++) {
			colours[i] =
				new Rgba32(
					(byte)( _random.NextInt( 128 ) + 128 ),
					(byte)( _random.NextInt( 128 ) + 128 ),
					(byte)( _random.NextInt( 128 ) + 128 )
				);
		}		

		int[] histogram = new int[cells];
		using Image<Rgba32> image = new Image<Rgba32>( size.Columns, size.Rows );
		for (int row = 0; row < size.Rows; row++) {
			for (int column = 0; column < size.Columns; column++) {
				int cell = (int)result[row][column];
				image[column, row] = colours[cell];

				histogram[cell]++;
			}
		}

		image.SaveAsPng( Path.Combine( _folder, "voronoi.png" ) );
		cells = 0;
	}

}

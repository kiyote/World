using Common.Buffer;
using Common.Buffer.Unit;
using Common.Worlds.Builder.DelaunayVoronoi;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Size = Common.Core.Size;

namespace Common.Worlds.Builder.Tests;

#pragma warning disable CA1812
[TestFixture]
internal sealed class IslandLandformGeneratorIntegrationTests {

	private ILandformGenerator _generator;
	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, Guid.NewGuid().ToString( "N" ) );
		Directory.CreateDirectory( _folder );
		var services = new ServiceCollection();
		services.AddCore();
		services.AddArrayBufferFactory();
		services.AddWorldBuilder();

		_provider = services.BuildServiceProvider();
	}

	[OneTimeTearDown]
	public void OneTimeTearDown() {
		Directory.Delete( _folder, true );
	}

	[SetUp]
	public void SetUp() {
		_scope = _provider.CreateScope();

		_generator = new IslandLandformGenerator(
			_scope.ServiceProvider.GetRequiredService<IRandom>(),
			_scope.ServiceProvider.GetRequiredService<IDelaunatorFactory>(),
			_scope.ServiceProvider.GetRequiredService<IVoronoiFactory>(),
			_scope.ServiceProvider.GetRequiredService<IUnitBufferClippingOperators>(),
			_scope.ServiceProvider.GetRequiredService<IBufferFactory>()
		);
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	[Ignore("Used to visualize output for inspection.")]
	public void Visualize() {
		Size size = new Size( 1000, 1000 );
		IBuffer<float> terrain = _generator.Create(
			123L,
			size,
			new HexNeighbourLocator()
		);


		using Image<Rgba32> image = new Image<Rgba32>( size.Columns, size.Rows );

		Rgba32[] colours = new Rgba32[] {
			Color.DarkBlue,
			Color.DarkGreen,
			Color.Green,
			Color.OliveDrab,
			Color.DarkKhaki,
			Color.Khaki,
			Color.Orange,
			Color.OrangeRed,
			Color.White,
		};

		for( int row = 0; row < size.Rows; row++) {
			for (int column = 0; column < size.Columns; column++) {
				float height = Math.Min( terrain[row][column], 1.0f );

				int index = (int)(NonLinearQuantizer( height ) * (colours.Length - 1));
				//int index = (int)( height * ( colours.Length - 1 ) );
				image[column, row] = colours[index];
			}
		}

		image.Save( Path.Combine( _folder, "terrain.png" ) );
	}

	private static float NonLinearQuantizer(
		float value
	) {
		double x =  Math.PI  * value;
		return (float)((0.1 * Math.Tan( 0.89 * ( x - 1.6) )) + 0.6);
	}
}
#pragma warning restore CA1812

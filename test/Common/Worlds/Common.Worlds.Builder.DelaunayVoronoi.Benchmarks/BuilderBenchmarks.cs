using BenchmarkDotNet.Attributes;
using Common.Core;
using Common.Geometry;
using Common.Geometry.DelaunayVoronoi;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Worlds.Builder.DelaunayVoronoi.Benchmarks;

[MemoryDiagnoser(false)]
public class BuilderBenchmarks {

	private readonly Size _size;
	private readonly ILandformBuilder _landformBuilder;
	private readonly IVoronoiBuilder _voronoiBuilder;
	private readonly IDelaunatorFactory _delaunatorFactory;
	private readonly IReadOnlyList<IPoint> _points;

	public BuilderBenchmarks() {
		var services = new ServiceCollection();
		services.AddCore();
		services.AddDelaunayVoronoiWorldBuilder();
		ServiceProvider provider = services.BuildServiceProvider();
		_landformBuilder = provider.GetRequiredService<ILandformBuilder>();
		_voronoiBuilder = provider.GetRequiredService<IVoronoiBuilder>();
		_delaunatorFactory = provider.GetRequiredService<IDelaunatorFactory>();

		_size = new Size( 500, 500 );

		IPointFactory pointsFactory = provider.GetRequiredService<IPointFactory>();
		_points = pointsFactory.Random( 1000, _size, 1 );
	}

	[Benchmark]
	public void DelaunatorFactory() {
		_delaunatorFactory.Create( _points );
	}

	[Benchmark]
	public void VoronoiBuilder() {
		_voronoiBuilder.Create( _size, 1000 );
	}

	[Benchmark]
	public void LandformBuilder() {
		_landformBuilder.Create( _size, out Voronoi _ );
	}
}


using BenchmarkDotNet.Attributes;
using Common.Core;
using Common.Geometry;
using Common.Geometry.DelaunayVoronoi;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Worlds.Builder.DelaunayVoronoi.Benchmarks;

[MemoryDiagnoser(false)]
public class BuilderBenchmarks {

	private readonly Size _size;
	private readonly IReadOnlyList<IPoint> _points;
	private readonly Voronoi _voronoi;
	private readonly HashSet<Cell> _fineLandforms;
	private readonly HashSet<Cell> _mountains;
	private readonly HashSet<Cell> _saltwater;

	private readonly ILandformBuilder _landformBuilder;
	private readonly IVoronoiBuilder _voronoiBuilder;
	private readonly IDelaunatorFactory _delaunatorFactory;
	private readonly IMountainsBuilder _mountainsBuilder;
	private readonly IHillsBuilder _hillsBuilder;
	private readonly ISaltwaterBuilder _saltwaterBuilder;
	private readonly IFreshwaterBuilder _freshwaterBuilder;

	public BuilderBenchmarks() {
		var services = new ServiceCollection();
		services.AddCommonCore();
		services.AddCommonWorldsBuilderDelaunayVoronoi();
		ServiceProvider provider = services.BuildServiceProvider();
		_landformBuilder = provider.GetRequiredService<ILandformBuilder>();
		_voronoiBuilder = provider.GetRequiredService<IVoronoiBuilder>();
		_delaunatorFactory = provider.GetRequiredService<IDelaunatorFactory>();
		_mountainsBuilder = provider.GetRequiredService<IMountainsBuilder>();
		_hillsBuilder = provider.GetRequiredService<IHillsBuilder>();
		_saltwaterBuilder = provider.GetRequiredService<ISaltwaterBuilder>();
		_freshwaterBuilder = provider.GetRequiredService<IFreshwaterBuilder>();

		_size = new Size( 500, 500 );

		IPointFactory pointsFactory = provider.GetRequiredService<IPointFactory>();
		IRandom random = provider.GetRequiredService<IRandom>();
		random.Reinitialise( 0x78901234 );
		_points = pointsFactory.Random( 1000, _size, 1 );
		_fineLandforms = _landformBuilder.Create( _size, out _voronoi );
		_mountains = _mountainsBuilder.Create( _size, _voronoi, _fineLandforms );
		_saltwater = _saltwaterBuilder.Create( _size, _voronoi, _fineLandforms );
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

	[Benchmark]
	public void MountainsBuilder() {
		_mountainsBuilder.Create( _size, _voronoi, _fineLandforms );
	}

	[Benchmark]
	public void HillsBuilder() {
		_hillsBuilder.Create( _voronoi, _fineLandforms, _mountains );
	}

	[Benchmark]
	public void SaltwaterBuilder() {
		_saltwaterBuilder.Create( _size, _voronoi, _fineLandforms );
	}

	[Benchmark]
	public void FreshwaterBuilder() {
		_freshwaterBuilder.Create( _voronoi, _fineLandforms, _saltwater );
	}
}


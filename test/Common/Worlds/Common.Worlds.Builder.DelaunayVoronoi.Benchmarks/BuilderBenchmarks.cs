using BenchmarkDotNet.Attributes;
using Kiyote.Geometry;
using Kiyote.Geometry.DelaunayVoronoi;
using Kiyote.Geometry.Randomization;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Worlds.Builder.DelaunayVoronoi.Benchmarks;

[MemoryDiagnoser(false)]
public class BuilderBenchmarks {

	/*
	private readonly ISize _size;
	private readonly IReadOnlyList<Point> _points;
	private readonly ISearchableVoronoi _map;
	private readonly IReadOnlySet<Cell> _landform;
	private readonly IReadOnlySet<Cell> _saltwater;
	private readonly IReadOnlySet<Cell> _freshwater;
	private readonly IReadOnlyList<IReadOnlySet<Cell>> _lakes;

	private readonly ILandformBuilder _landformBuilder;
	private readonly IVoronoiBuilder _voronoiBuilder;
	private readonly ISaltwaterBuilder _saltwaterBuilder;
	private readonly IFreshwaterBuilder _freshwaterBuilder;
	private readonly ILakeBuilder _lakeBuilder;
	private readonly ICoastBuilder _coastBuilder;

	public BuilderBenchmarks() {
		var services = new ServiceCollection();
		services.AddRandomization();
		services.AddDelaunayVoronoi();
		services.AddDelaunayVoronoiWorldBuilder();
		ServiceProvider provider = services.BuildServiceProvider();
		_landformBuilder = provider.GetRequiredService<ILandformBuilder>();
		_voronoiBuilder = provider.GetRequiredService<IVoronoiBuilder>();
		_saltwaterBuilder = provider.GetRequiredService<ISaltwaterBuilder>();
		_freshwaterBuilder = provider.GetRequiredService<IFreshwaterBuilder>();
		_lakeBuilder = provider.GetRequiredService<ILakeBuilder>();
		_coastBuilder = provider.GetRequiredService<ICoastBuilder>();

		_size = new Point( 500, 500 );

		IPointFactory pointsFactory = provider.GetRequiredService<IPointFactory>();
		IRandom random = provider.GetRequiredService<IRandom>();
		random.Reinitialise( 0x78901234 );
		_points = pointsFactory.Fill( _size, 5 );
		_landform = _landformBuilder.Create( _size, out _map );
		_saltwater = _saltwaterBuilder.Create( _size, _map, _landform );
		_freshwater = _freshwaterBuilder.Create( _size, _map, _landform, _saltwater );
		_lakes = _lakeBuilder.Create( _size, _map, _landform, _saltwater, _freshwater );
	}

	[Benchmark]
	public void VoronoiBuilder() {
		_voronoiBuilder.Create( _size, 20 );
	}

	[Benchmark]
	public void LandformBuilder() {
		_landformBuilder.Create( _size, out ISearchableVoronoi _ );
	}

	[Benchmark]
	public void SaltwaterBuilder() {
		_saltwaterBuilder.Create( _size, _map, _landform );
	}

	[Benchmark]
	public void FreshwaterBuilder() {
		_freshwaterBuilder.Create( _size, _map, _landform, _saltwater );
	}

	[Benchmark]
	public void LakeBuilder() {
		_lakeBuilder.Create( _size, _map, _landform, _saltwater, _freshwater );
	}

	[Benchmark]
	public void CoastBuilder() {
		_coastBuilder.Create( _size, _map, _landform, _saltwater );
	}
	*/
}


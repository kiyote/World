using BenchmarkDotNet.Attributes;
using Kiyote.Geometry;
using Kiyote.Geometry.DelaunayVoronoi;
using Kiyote.Geometry.Randomization;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Worlds.Builder.DelaunayVoronoi.Benchmarks;

[MemoryDiagnoser(false)]
public class BuilderBenchmarks {

	private readonly ISize _size;
	private readonly IReadOnlyList<Point> _points;
	private readonly ISearchableVoronoi _voronoi;
	private readonly HashSet<Cell> _fineLandforms;
	private readonly HashSet<Cell> _mountains;
	private readonly HashSet<Cell> _hills;
	private readonly HashSet<Cell> _saltwater;
	private readonly HashSet<Cell> _freshwater;
	private readonly Dictionary<Cell, float> _temperatures;
	private readonly Dictionary<Cell, float> _airflows;
	private readonly Dictionary<Cell, float> _moistures;

	private readonly ILandformBuilder _landformBuilder;
	private readonly IVoronoiBuilder _voronoiBuilder;
	private readonly IMountainsBuilder _mountainsBuilder;
	private readonly IHillsBuilder _hillsBuilder;
	private readonly ISaltwaterBuilder _saltwaterBuilder;
	private readonly IFreshwaterBuilder _freshwaterBuilder;
	private readonly ITemperatureBuilder _temperatureBuilder;
	private readonly IAirflowBuilder _airflowBuilder;
	private readonly IMoistureBuilder _moistureBuilder;

	public BuilderBenchmarks() {
		var services = new ServiceCollection();
		services.AddRandomization();
		services.AddDelaunayVoronoi();
		services.AddDelaunayVoronoiWorldBuilder();
		ServiceProvider provider = services.BuildServiceProvider();
		_landformBuilder = provider.GetRequiredService<ILandformBuilder>();
		_voronoiBuilder = provider.GetRequiredService<IVoronoiBuilder>();
		_mountainsBuilder = provider.GetRequiredService<IMountainsBuilder>();
		_hillsBuilder = provider.GetRequiredService<IHillsBuilder>();
		_saltwaterBuilder = provider.GetRequiredService<ISaltwaterBuilder>();
		_freshwaterBuilder = provider.GetRequiredService<IFreshwaterBuilder>();
		_temperatureBuilder = provider.GetRequiredService<ITemperatureBuilder>();
		_airflowBuilder = provider.GetRequiredService<IAirflowBuilder>();
		_moistureBuilder = provider.GetRequiredService<IMoistureBuilder>();

		_size = new Point( 500, 500 );

		IPointFactory pointsFactory = provider.GetRequiredService<IPointFactory>();
		IRandom random = provider.GetRequiredService<IRandom>();
		random.Reinitialise( 0x78901234 );
		_points = pointsFactory.Fill( _size, 1 );
		_fineLandforms = _landformBuilder.Create( _size, out _voronoi );
		_mountains = _mountainsBuilder.Create( _size, _voronoi, _fineLandforms );
		_hills = _hillsBuilder.Create( _voronoi, _fineLandforms, _mountains );
		_saltwater = _saltwaterBuilder.Create( _size, _voronoi, _fineLandforms );
		_freshwater = _freshwaterBuilder.Create( _voronoi, _fineLandforms, _saltwater );
		_temperatures = _temperatureBuilder.Create( _size, _voronoi, _fineLandforms, _mountains, _hills, _saltwater, _freshwater );
		_airflows = _airflowBuilder.Create( _size, _voronoi, _fineLandforms, _mountains, _hills );
		_moistures = _moistureBuilder.Create( _size, _voronoi, _fineLandforms, _saltwater, _freshwater, _temperatures, _airflows );
	}

	[Benchmark]
	public void VoronoiBuilder() {
		_voronoiBuilder.Create( _size, 1000 );
	}

	[Benchmark]
	public void LandformBuilder() {
		_landformBuilder.Create( _size, out ISearchableVoronoi _ );
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

	[Benchmark]
	public void TemperatureBuilder() {
		_temperatureBuilder.Create( _size, _voronoi, _fineLandforms, _mountains, _hills, _saltwater, _freshwater );
	}

	[Benchmark]
	public void AirflowBuilder() {
		_airflowBuilder.Create( _size, _voronoi, _fineLandforms, _mountains, _hills );
	}

	[Benchmark]
	public void MoistureBuilder() {
		_moistureBuilder.Create( _size, _voronoi, _fineLandforms, _saltwater, _freshwater, _temperatures, _airflows );
	}
}


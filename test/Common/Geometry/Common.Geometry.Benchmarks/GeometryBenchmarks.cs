using BenchmarkDotNet.Attributes;
using Common.Core;
using Common.Geometry.DelaunayVoronoi;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Geometry.Benchmarks;

[MemoryDiagnoser( false )]
public class GeometryBenchmarks {

	private readonly int _count;
	private readonly Size _size;
	private readonly IReadOnlyList<IPoint> _points;
	private readonly IPointFactory _pointFactory;
	private readonly IDelaunatorFactory _delaunatorFactory;

	public GeometryBenchmarks() {
		var services = new ServiceCollection();
		services.AddCore();
		services.AddGeometry();
		ServiceProvider provider = services.BuildServiceProvider();
		_pointFactory = provider.GetRequiredService<IPointFactory>();
		_delaunatorFactory = provider.GetRequiredService<IDelaunatorFactory>();

		_count = 1000;
		_size = new Size( 500, 500 );
		_points = _pointFactory.Random( _count, _size, 1 );
	}

	[Benchmark]
	public void PointFactory_Random() {
		_pointFactory.Random( _count, _size, 1 );
	}

	[Benchmark]
	public void DelaunatorFactory() {
		_delaunatorFactory.Create( _points );
	}
}


using BenchmarkDotNet.Attributes;
using Common.Core;
using Common.Geometry.DelaunayVoronoi;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Geometry.Benchmarks;

[MemoryDiagnoser( false )]
public class GeometryBenchmarks {

	private readonly IPoint _point;
	private readonly int _count;
	private readonly Size _size;
	private readonly Delaunator _delaunator;
	private readonly IReadOnlyList<IPoint> _points;
	private readonly IReadOnlyList<IReadOnlyList<IPoint>> _polygons;
	private readonly IPointFactory _pointFactory;
	private readonly IDelaunatorFactory _delaunatorFactory;
	private readonly IVoronoiFactory _voronoiFactory;
	private readonly IGeometry _geometry;

	public GeometryBenchmarks() {
		var services = new ServiceCollection();
		services.AddCommonCore();
		services.AddCommonGeometry();
		ServiceProvider provider = services.BuildServiceProvider();
		_pointFactory = provider.GetRequiredService<IPointFactory>();
		_delaunatorFactory = provider.GetRequiredService<IDelaunatorFactory>();
		_voronoiFactory = provider.GetRequiredService<IVoronoiFactory>();
		_geometry = provider.GetRequiredService<IGeometry>();

		_count = 1000;
		_size = new Size( 500, 500 );
		_points = _pointFactory.Random( _count, _size, 1 );
		_point = new Point( 250, 250 );


		_delaunator = _delaunatorFactory.Create( _points );
		IVoronoi voronoi = _voronoiFactory.Create( _delaunator, _size.Columns, _size.Rows );
		_polygons = voronoi.Cells.Select( c => c.Points ).ToList();
	}

	[Benchmark]
	public void PointFactory_Random() {
		_pointFactory.Random( _count, _size, 1 );
	}

	[Benchmark]
	public void DelaunatorFactory() {
		_delaunatorFactory.Create( _points );
	}

	[Benchmark]
	public void VoronoiFactory() {
		_voronoiFactory.Create( _delaunator, _size.Columns, _size.Rows );
	}

	[Benchmark]
	public void PointInPolygon() {
		for (int i = 0; i < _polygons.Count; i++) {
			_geometry.PointInPolygon( _polygons[i], _point );
		}
	}
}


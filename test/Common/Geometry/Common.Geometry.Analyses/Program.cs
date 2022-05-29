using Common.Core;
using Common.Geometry.DelaunayVoronoi;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Geometry.Analyses;

public static class Program {
	public static void Main( string[] args ) {
		ServiceCollection services = new ServiceCollection();
		services.AddCommonCore();
		services.AddCommonGeometry();
		ServiceProvider provider = services.BuildServiceProvider();
		IPointFactory pointFactory = provider.GetRequiredService<IPointFactory>();
		IDelaunatorFactory delaunatorFactory = provider.GetRequiredService<IDelaunatorFactory>();
		IVoronoiFactory voronoiFactory = provider.GetRequiredService<IVoronoiFactory>();

		Console.WriteLine( "Ready to run, press any key." );
		Console.ReadKey();

		//PointFactoryAnalyzer.Analyze( pointFactory );
		//DelaunatorFactoryAnalyzer.Analyze( pointFactory, delaunatorFactory );
		VoronoiFactoryAnalyzer.Analyze( pointFactory, delaunatorFactory, voronoiFactory );

		Console.WriteLine( "Execution completed, press any key." );
		Console.ReadKey();
	}
}


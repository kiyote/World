
using Common.Worlds.Builder.DelaunayVoronoi;
using Kiyote.Geometry;
using Kiyote.Geometry.DelaunayVoronoi;
using Kiyote.Geometry.Randomization;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddRandomization();
services.AddDelaunayVoronoi();
services.AddDelaunayVoronoiWorldBuilder();
ServiceProvider provider = services.BuildServiceProvider();
ILandformBuilder landformBuilder = provider.GetRequiredService<ILandformBuilder>();

/*
ISize size = new Point( 500, 500 );

IPointFactory pointsFactory = provider.GetRequiredService<IPointFactory>();
IRandom random = provider.GetRequiredService<IRandom>();
random.Reinitialise( 0x78901234 );
for (int i = 0; i < 100; i++) {
	Console.SetCursorPosition( 0, 0 );
	Console.WriteLine( $"Iteration: {i + 1}" );
	_ = landformBuilder.Create( size, out ISearchableVoronoi _ );
}
*/

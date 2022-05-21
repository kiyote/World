using Common.Core;
using Common.Geometry;
using Common.Geometry.DelaunayVoronoi;
using Microsoft.Extensions.DependencyInjection;

ServiceCollection services = new ServiceCollection();
services.AddCore();
services.AddGeometry();
ServiceProvider provider = services.BuildServiceProvider();
IPointFactory pointFactory = provider.GetRequiredService<IPointFactory>();
IDelaunatorFactory delaunatorFactory = provider.GetRequiredService<IDelaunatorFactory>();

int pointCount = 100;
Size size = new Size( 500, 500 );
IReadOnlyList<IPoint> points = pointFactory.Random( pointCount, size, 1 );

Console.WriteLine( "Ready to run, press any key." );
Console.ReadKey();

Delaunator delaunator = delaunatorFactory.Create( points );

Console.WriteLine( $"Coords size: {delaunator.Coords.Count}" );
Console.WriteLine( $"Half-Edges size: {delaunator.HalfEdges.Count}" );
Console.WriteLine( $"Hull size: {delaunator.Hull.Count}" );
Console.WriteLine( $"Triangles size: {delaunator.Triangles.Count}" );

Console.WriteLine( "Execution completed, press any key." );
Console.ReadKey();

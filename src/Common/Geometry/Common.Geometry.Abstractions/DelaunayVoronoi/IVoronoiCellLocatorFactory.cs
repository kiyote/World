using Common.Core;

namespace Common.Geometry.DelaunayVoronoi;

public interface IVoronoiCellLocatorFactory {

	IVoronoiCellLocator Create( Voronoi voronoi, Size size );
}

using Common.Core;

namespace Common.Geometry.DelaunayVoronoi;

public interface ISearchableVoronoiFactory {

	ISearchableVoronoi Create( IVoronoi voronoi, Size size );
}

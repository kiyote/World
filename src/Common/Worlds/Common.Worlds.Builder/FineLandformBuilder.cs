using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Geometry;
using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder;

internal sealed class FineLandformBuilder : IFineLandformBuilder {

	private readonly IVoronoiBuilder _voronoiBuilder;
	private readonly IGeometry _geometry;

	public FineLandformBuilder(
		IVoronoiBuilder voronoiBuilder,
		IGeometry geometry
	) {
		_voronoiBuilder = voronoiBuilder;
		_geometry = geometry;
	}

	Voronoi IFineLandformBuilder.Create(
		Size size,
		int pointCount,
		List<Cell> roughLandforms,
		out List<Cell> fineLandforms
	) {
		Voronoi voronoi = _voronoiBuilder.Create( size, pointCount );

		fineLandforms = new List<Cell>();
		foreach( Cell fineCell in voronoi.Cells.Where( c => !c.IsOpen ) ) {
			foreach( Cell roughCell in roughLandforms ) {
				if( _geometry.PointInPolygon( roughCell.Points, fineCell.Center ) ) {
					fineLandforms.Add( fineCell );
				}
			}
		}

		return voronoi;
	}
}

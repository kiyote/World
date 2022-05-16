using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Geometry;
using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder;

internal sealed class MountainRangeBuilder : IMountainRangeBuilder {

	private readonly IRandom _random;
	private readonly IGeometry _geometry;

	public MountainRangeBuilder(
		IRandom random,
		IGeometry geometry
	) {
		_random = random;
		_geometry = geometry;
	}


	List<Cell> IMountainRangeBuilder.BuildRanges(
		Size size,
		Voronoi fineVoronoi,
		List<Cell> fineLandforms
	) {
		List<Cell> result = new List<Cell>();
		do {
			List<Edge> lines = GetMountainLines( size, size.Columns / 100 );
			result.AddRange( BuildRanges( fineVoronoi, fineLandforms, lines ) );
		} while( result.Count < size.Columns / 10 );
		return result.Distinct().ToList();
	}

	internal List<Cell> BuildRanges(
		Voronoi fineVoronoi,
		List<Cell> fineLandforms,
		List<Edge> lines
	) {
		List<Cell> result = new List<Cell>();
		foreach (Edge edge in lines) {
			_geometry.RasterizeLine( edge.A, edge.B, ( int x, int y ) => {
				foreach( Cell fineCell in fineLandforms ) {
					if( _geometry.PointInPolygon( fineCell.Points, x, y ) ) {
						if( !result.Contains( fineCell ) ) {
							result.Add( fineCell );
						}
					}
				}
			} );
		}
		// This prevents the mountains from being up against water
		result = result.Where( mountainCell => {
			bool allowed = true;
			foreach( Cell neighbourCell in fineVoronoi.Neighbours[mountainCell] ) {
				if( !fineLandforms.Contains( neighbourCell ) ) {
					allowed = false;
				}
			}
			return allowed;
		} ).ToList();

		return result;
	}

	internal List<Edge> GetMountainLines(
		Size size,
		int count
	) {		
		List<Edge> result = new List<Edge>();


		// Get a rough direction for the mountain range
		float direction = _random.NextInt( 90 ) - 45;
		// Start it somewhere middle-top -ish
		int widthRange = size.Columns / 2;
		Point start = new Point(
			_random.NextInt( widthRange ) + ( widthRange / 2 ),
			_random.NextInt( size.Rows / 4)
		);
		int segmentRange = ( size.Columns / 12 );

		do {
			int segmentLength = _random.NextInt( segmentRange ) + ( segmentRange / 2 );
			Point next = new Point(
				start.X + (int)(Math.Sin( Math.PI / 180 * direction ) * segmentLength),
				start.Y + (int)(Math.Cos( Math.PI / 180 * direction ) * segmentLength)
			);
			result.Add( new Edge( start, next ) );
			start = next;
			direction += (_random.NextInt( 30 ) - 15);

		} while( result.Count < count );

		/*
		for (int i = 0; i < count; i++ ) {
			Point p1;
			Point p2;
			do {
				p1 = new Point(
					_random.NextInt( size.Columns ),
					_random.NextInt( size.Rows )
				);

				p2 = new Point(
					_random.NextInt( size.Columns ),
					_random.NextInt( size.Rows )
				);

			} while(
				( _geometry.LineLength( p1, p2 ) > ( size.Columns / 2 ) )
				|| ( _geometry.LineLength( p1, p2 ) < ( size.Columns / 12 ) )
			);

			result.Add( new Edge( p1, p2 ) );
		}
		*/

		return result;
	}
}

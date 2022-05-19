﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Geometry;
using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder.DelaunayVoronoi;

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


	HashSet<Cell> IMountainRangeBuilder.BuildRanges(
		Size size,
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms
	) {
		HashSet<Cell> result = new HashSet<Cell>();
		do {
			List<Edge> lines = GetMountainLines( size, size.Columns / 100 );
			List<Cell> rangeCells = BuildRanges( fineVoronoi, size, fineLandforms, lines );
			foreach (Cell cell in rangeCells) {
				result.Add( cell );
			};
		} while( result.Count < size.Columns / 12 );
		return result;
	}

	internal List<Cell> BuildRanges(
		Voronoi fineVoronoi,
		Size size,
		HashSet<Cell> fineLandforms,
		List<Edge> lines
	) {
		List<Cell> result = new List<Cell>();
		lines = lines.OrderBy( l => _random.NextInt() ).Take( (int)(lines.Count * 0.75) ).ToList();
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

		return result;
	}
}
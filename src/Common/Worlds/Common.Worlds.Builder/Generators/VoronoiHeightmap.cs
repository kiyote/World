using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

namespace Common.Worlds.Builder.Generators;

internal class VoronoiHeightmap {

	private readonly IRandom _random;

	public VoronoiHeightmap(
		IRandom random
	) {
		_random = random;
	}

	public Dictionary<VoronoiRegion, float> Generate(
		int width,
		int height,
		IEnumerable<VoronoiRegion> regions
	) {

		Dictionary<VoronoiRegion, List<VoronoiRegion>> neighbours = CalculateConnections( regions, width, height );

		Dictionary<VoronoiRegion, float> heights = new Dictionary<VoronoiRegion, float>();

		for (int i = 0; i < 10; i++) {
			AddIsland( neighbours, heights );
		}

		return heights;
	}

	private void AddIsland(
		Dictionary<VoronoiRegion, List<VoronoiRegion>> neighbours,
		Dictionary<VoronoiRegion, float> heights
	) {
		int seed = _random.NextInt( neighbours.Keys.Count );
		VoronoiRegion seedRegion = neighbours.Keys.ElementAt( seed );
		heights[seedRegion] = 1.0f;
		Queue<VoronoiRegion> queue = new Queue<VoronoiRegion>();
		queue.Enqueue( seedRegion );
		float sharpness = 0.2f;
		float altitude = 1.0f;
		float radius = 0.5f;
		while( queue.Any() && altitude > 0.1f ) {
			VoronoiRegion region = queue.Dequeue();
			altitude = heights[region] * radius;
			foreach( VoronoiRegion neighbour in neighbours[region] ) {
				if( !heights.ContainsKey( neighbour ) ) {
					float mod = ( _random.NextFloat() * sharpness ) + 1.1f - sharpness;
					heights[neighbour] = altitude * mod;
					if( heights[neighbour] > 1.0f ) {
						heights[neighbour] = 1;
					}
					queue.Enqueue( neighbour );
				}
			}
		}
	}

	public IEnumerable<VoronoiRegion> GenerateVoronoi(
		int width,
		int height
	) {
		List<Vertex> vertices = new List<Vertex>();
		int NumberOfVertices = width * height / 250;
		float xSize = width / 2.0f;
		float ySize = height / 2.0f;
		for( int i = 0; i < NumberOfVertices; i++ ) {
			float x = _random.NextFloat( -xSize, xSize );
			float y = _random.NextFloat( -ySize, ySize );

			vertices.Add( new Vertex( x, y ) );
		}
		IDelaunayFactory delaunayFactory = new DelaunayFactory();
		DelaunayGraph delaunay = delaunayFactory.Create( vertices );
		IVoronoiFactory voronoiFactory = new VoronoiFactory();
		Voronoi voronoi = voronoiFactory.Create( delaunay );

		return voronoi.Regions.Where( r => !r.Cells.Where( c => !InBound( c.CircumCenter, xSize, ySize ) ).Any() );
	}

	private static Dictionary<VoronoiRegion, List<VoronoiRegion>> CalculateConnections(
		IEnumerable<VoronoiRegion> regions,
		int width,
		int height
	) {
		float xSize = width / 2.0f;
		float ySize = height / 2.0f;

		var result = new Dictionary<VoronoiRegion, List<VoronoiRegion>>();
		foreach( VoronoiRegion region in regions ) {
			List<VoronoiRegion> neighbours = new List<VoronoiRegion>();
			foreach( Edge edge in region.Edges ) {
				neighbours.AddRange(
					regions
						.Where( vr => {
							return vr != region								
								&& vr.Edges.Contains( edge );
							} )
						.Distinct()
				);
			}
			result[region] = neighbours;
		}

		return result;
	}

	private static bool InBound(
		Vertex v,
		float xSize,
		float ySize
	) {
		if( v.X < -xSize || v.X > xSize ) {
			return false;
		}
		if( v.Y < -ySize || v.Y > ySize ) {
			return false;
		}

		return true;
	}
}

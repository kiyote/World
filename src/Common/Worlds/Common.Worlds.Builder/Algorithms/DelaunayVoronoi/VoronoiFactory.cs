namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;
internal class VoronoiFactory : IVoronoiFactory {

	Voronoi IVoronoiFactory.Create(
		Delaunay delaunay
	) {
		List<Region> regions = new List<Region>();

		for( int i = 0; i < delaunay.Vertices.Count; i++ ) {

			Vertex vertex = delaunay.Vertices[i];

			List<Cell> cells = new List<Cell>();
			for( int j = 0; j < delaunay.Cells.Count; j++ ) {
				Simplex simplex = delaunay.Cells[j].Simplex;

				for( int k = 0; k < simplex.Vertices.Length; k++ ) {
					if( simplex.Vertices[k].Tag == vertex.Tag ) {
						cells.Add( delaunay.Cells[j] );
						break;
					}
				}
			}

			if( cells.Count > 0 ) {

				Dictionary<int, Cell> neighbourCell = new Dictionary<int, Cell>();

				for( int j = 0; j < cells.Count; j++ ) {
					neighbourCell.Add( cells[j].CircumCenter.Id, cells[j] );
				}

				List<Edge> regionEdges = new List<Edge>();
				for( int j = 0; j < cells.Count; j++ ) {
					Simplex simplex = cells[j].Simplex;

					for( int k = 0; k < simplex.Adjacent.Length; k++ ) {
						Simplex? adjacentSimplex = simplex.Adjacent[k];
						if( adjacentSimplex is null ) {
							continue;
						}

						int tag = adjacentSimplex.Tag;
						if( neighbourCell.ContainsKey( tag ) ) {
							Edge edge = new Edge( cells[j], neighbourCell[tag] );
							regionEdges.Add( edge );
						}
					}
				}

				Region region = new Region( cells, regionEdges );
				regions.Add( region );
			}
		}

		return new Voronoi( regions );
	}
}

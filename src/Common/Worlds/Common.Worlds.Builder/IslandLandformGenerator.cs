using Common.Worlds.Builder.DelaunayVoronoi;
using Common.Buffer;
using Common.Buffer.Unit;

namespace Common.Worlds.Builder;

internal class IslandLandformGenerator : ILandformGenerator {

	public const int SeedCount = 6000;
	private readonly IDelaunatorFactory _delaunatorFactory;
	private readonly IVoronoiFactory _voronoiFactory;
	private readonly IRandom _random;
	private readonly IUnitBufferClippingOperators _bufferOperator;
	private readonly IBufferFactory _bufferFactory;

	public IslandLandformGenerator(
		IRandom random,
		IDelaunatorFactory delaunatorFactory,
		IVoronoiFactory voronoiFactory,
		IUnitBufferClippingOperators bufferOperator,
		IBufferFactory bufferFactory
	) {
		_random = random;
		_delaunatorFactory = delaunatorFactory;
		_voronoiFactory = voronoiFactory;
		_bufferOperator = bufferOperator;
		_bufferFactory = bufferFactory;
	}

	IBuffer<float> ILandformGenerator.Create(
		long seed,
		Size size,
		INeighbourLocator neighbourLocator
	) {
		List<Point> points = new List<Point>();
		while( points.Count < SeedCount ) {
			Point newPoint = new Point(
				_random.NextInt( size.Columns ),
				_random.NextInt( size.Rows )
			);
			if( !points.Any( p => Math.Abs( p.X - newPoint.X ) <= 1  && Math.Abs( p.Y - newPoint.Y ) <= 1 ) ) {
				points.Add( newPoint );
			};

		}
		Delaunator delaunator = _delaunatorFactory.Create( points );
		Voronoi voronoi = _voronoiFactory.Create( delaunator, size.Columns, size.Rows, true );

		Dictionary<Cell, HeightCell> heights = new Dictionary<Cell, HeightCell>();
		AddLargeIsland( size, voronoi, heights );
		for( int i = 0; i < 15; i++) {
			foreach (HeightCell heightCell in heights.Values) {
				heightCell.Used = false;
			}
			AddSmallIsland( size, voronoi, heights );
		}

		IBuffer<float> heightmap = _bufferFactory.Create<float>( size );
		foreach (Cell cell in heights.Keys) {
			PolygonRasterizer.Fill( cell.Points, ( int x, int y ) => {
				if (x >= 0 && x < size.Columns && y >= 0 && y < size.Rows) {
					heightmap[y][x] = heights[cell].Height;
				}
			} );
		}

		_bufferOperator.Normalize( heightmap, heightmap );

		return heightmap;
	}

	private void AddLargeIsland(
		Size size,
		Voronoi voronoi,
		Dictionary<Cell, HeightCell> heights
	) {
		Cell start;
		do {
			int seedIndex = _random.NextInt( voronoi.Cells.Count );
			start = voronoi.Cells[seedIndex];
		} while(
			start.IsOpen
			|| start.Circumcenter.X < size.Columns * 0.33
			|| start.Circumcenter.X > size.Columns * 0.66
			|| start.Circumcenter.Y < size.Rows * 0.33
			|| start.Circumcenter.Y > size.Rows * 0.66
			|| GetOrCreate( heights, start ).Height > 0.1
		);		 

		float height = 0.8f;
		float radius = 0.875f;
		float sharpness = 0.2f;

		HeightCell heightCell = GetOrCreate( heights, start );
		heightCell.Height = height;
		heightCell.Used = true;

		Queue<Cell> queue = new Queue<Cell>();
		queue.Enqueue( start );
		while (queue.Any() && height > 0.01f) {
			Cell cell = queue.Dequeue();
			heightCell = GetOrCreate( heights, cell );
			height = heightCell.Height * radius;
			foreach (Cell neighbour in voronoi.Neighbours[cell].Where( c => !c.IsOpen )) {
				heightCell = GetOrCreate( heights, neighbour );
				if (!heightCell.Used) {
					float mod = (_random.NextFloat() * sharpness) + 1.1f - sharpness;
					heightCell.Height += height * mod;
					/*
					if (heightCell.Height > 1.0f) {
						heightCell.Height = 1.0f;
					}
					*/
					heightCell.Used = true;
					queue.Enqueue( neighbour );
				}
			}
		}
	}

	private void AddSmallIsland(
		Size size,
		Voronoi voronoi,
		Dictionary<Cell, HeightCell> heights
	) {
		Cell start;
		do {
			int seedIndex = _random.NextInt( voronoi.Cells.Count );
			start = voronoi.Cells[seedIndex];
		} while(
			start.IsOpen
			|| start.Circumcenter.X < size.Columns * 0.15
			|| start.Circumcenter.X > size.Columns * 0.85
			|| start.Circumcenter.Y < size.Rows * 0.15
			|| start.Circumcenter.Y > size.Rows * 0.85
			|| GetOrCreate( heights, start ).Height < 0.01
		);

		float height = 0.6f;
		float radius = 0.9f;

		HeightCell heightCell = GetOrCreate( heights, start );
		heightCell.Height = height;
		heightCell.Used = true;

		Queue<Cell> queue = new Queue<Cell>();
		queue.Enqueue( start );
		while( queue.Any() && height > 0.01f ) {
			Cell cell = queue.Dequeue();
			heightCell = GetOrCreate( heights, cell );
			height *= radius;
			foreach( Cell neighbour in voronoi.Neighbours[cell].Where( c => !c.IsOpen ) ) {
				heightCell = GetOrCreate( heights, neighbour );
				if( !heightCell.Used ) {
					heightCell.Height += height;
					if( heightCell.Height > 1.0f ) {
						heightCell.Height = 1.0f;
					}
					heightCell.Used = true;
					queue.Enqueue( neighbour );
				}
			}
		}
	}

	private static HeightCell GetOrCreate(
		Dictionary<Cell, HeightCell> heights,
		Cell cell
	) {
		if( !heights.ContainsKey( cell ) ) {
			heights[cell] = new HeightCell( cell );
		}

		return heights[cell];
	}
}

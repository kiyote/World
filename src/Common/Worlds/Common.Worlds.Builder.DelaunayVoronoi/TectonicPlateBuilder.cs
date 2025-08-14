using System.Numerics;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class TectonicPlateBuilder : ITectonicPlateBuilder {

	private readonly IRandom _random;
	private readonly IVoronoiBuilder _voronoiBuilder;

	private const int RoughCellCount = 4;

	public TectonicPlateBuilder(
		IRandom random,
		IVoronoiBuilder voronoiBuilder
	) {
		_random = random;
		_voronoiBuilder = voronoiBuilder;
	}

	TectonicPlates ITectonicPlateBuilder.Create(
		ISize size
	) {
		int cellSize = Math.Min( size.Width, size.Height ) / RoughCellCount;
		IVoronoi plates = _voronoiBuilder.Create( size, cellSize );

		Dictionary<Cell, Vector2> velocities = [];
		foreach (Cell cell in plates.Cells) {
			velocities[cell] = new Vector2(
				_random.NextFloat(-1.0f, 1.0f),
				_random.NextFloat(-1.0f, 1.0f)
			);
		}

		return new TectonicPlates(
			plates,
			velocities
		);
	}
}

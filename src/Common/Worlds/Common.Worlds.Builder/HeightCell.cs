using Common.Worlds.Builder.DelaunayVoronoi;

namespace Common.Worlds.Builder;

internal sealed class HeightCell {

	public HeightCell(
		Cell cell
	) {
		Cell = cell;
	}

	public Cell Cell { get; init; }

	public float Height { get; set; }

	public bool Used { get; set; }
}

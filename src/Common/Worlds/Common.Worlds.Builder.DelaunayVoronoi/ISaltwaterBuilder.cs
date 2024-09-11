﻿namespace Common.Worlds.Builder.DelaunayVoronoi;

public interface ISaltwaterBuilder {
	IReadOnlySet<Cell> Create(
		ISize size,
		IVoronoi map,
		IReadOnlySet<Cell> landform
	);
}

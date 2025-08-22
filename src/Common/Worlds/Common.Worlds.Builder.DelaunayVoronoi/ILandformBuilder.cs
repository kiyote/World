namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface ILandformBuilder {
	Task<Landform> CreateAsync(
		ISize size,
		TectonicPlates tectonicPlates,
		CancellationToken cancellationToken
	);
}

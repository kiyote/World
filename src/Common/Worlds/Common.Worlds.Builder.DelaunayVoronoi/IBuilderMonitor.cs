namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IBuilderMonitor {

	Task LandformStageAsync(
		ISize size,
		int stage,
		IReadOnlySet<Cell> cells,
		CancellationToken cancellationToken
	);

	Task LandformStageMessageAsync(
		string message,
		CancellationToken cancellationToken
	);

}

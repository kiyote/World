namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

internal sealed record DeferredSimplex(
	SimplexWrap Face,
	SimplexWrap Pivot,
	SimplexWrap OldFace,
	int FaceIndex,
	int PivotIndex
);


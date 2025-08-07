using System.Numerics;

namespace Common.Worlds.Builder.DelaunayVoronoi;

public sealed record TectonicPlates(
	IVoronoi Plates,
	IReadOnlyDictionary<Cell, Vector2> Velocity
);

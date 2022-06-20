namespace Common.Worlds;

[Flags]
public enum TileFeature: int {
	None = 0,

	TemperateForest = 1,
	TropicalForest = 2,
	BorealForest = 3,
	Tundra = 4,
	RockyDesert = 5,
	SandyDesert = 6
}

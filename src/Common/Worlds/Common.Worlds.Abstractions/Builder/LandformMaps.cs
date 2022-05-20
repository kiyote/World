using Common.Buffers;

namespace Common.Worlds.Builder;

public sealed record LandformMaps(
	IBuffer<float> Height,
	IBuffer<bool> SaltWater,
	IBuffer<bool> FreshWater,
	IBuffer<float> Temperature
);

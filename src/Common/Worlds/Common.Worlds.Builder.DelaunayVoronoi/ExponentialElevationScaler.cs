namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class ExponentialElevationScaler : IElevationScaler {

	float IElevationScaler.Scale(
		float input
	) {
		return (float)( Math.Pow( input, 3 ) - Math.Pow( input, 2 ) + input);
	}

}

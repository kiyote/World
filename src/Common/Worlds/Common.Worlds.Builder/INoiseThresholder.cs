namespace Common.Worlds.Builder;

internal interface INoiseThresholder {

	float[,] Threshold( float[,] source, float threshold );

	float[,] Range( float[,] source, float min, float max );
}


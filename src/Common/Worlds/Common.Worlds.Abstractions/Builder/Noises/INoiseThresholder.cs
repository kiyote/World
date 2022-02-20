namespace Common.Worlds.Builder.Noises;

public interface INoiseThresholder {

	float[,] Threshold( float[,] source, float threshold );

	float[,] Range( float[,] source, float min, float max );
}


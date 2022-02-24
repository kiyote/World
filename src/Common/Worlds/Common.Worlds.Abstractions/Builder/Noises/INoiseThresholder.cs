namespace Common.Worlds.Builder.Noises;

public interface INoiseThresholder {

	float[,] Threshold( ref float[,] source, float threshold );

	float[,] Range( ref float[,] source, float min, float max );
}


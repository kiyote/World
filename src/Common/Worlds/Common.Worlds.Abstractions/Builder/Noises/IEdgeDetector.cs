namespace Common.Worlds.Builder.Noises;

public interface IEdgeDetector {

	float[,] Detect( ref float[,] source, float threshold );
}


namespace Common.Worlds.Builder.Noises;

public interface INoiseInverter {

	float[,] Invert( ref float[,] source );
}


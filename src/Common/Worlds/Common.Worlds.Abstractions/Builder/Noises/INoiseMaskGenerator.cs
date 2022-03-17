namespace Common.Worlds.Builder.Noises;

public interface INoiseMaskGenerator {
	INoiseMaskGenerator Circle( Size size, Buffer<float> output );
	Buffer<float> Circle( Size size );
}


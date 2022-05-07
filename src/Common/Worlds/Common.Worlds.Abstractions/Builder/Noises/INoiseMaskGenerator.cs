using Common.Buffer;

namespace Common.Worlds.Builder.Noises;

public interface INoiseMaskGenerator {
	INoiseMaskGenerator Circle( Size size, IBuffer<float> output );
	IBuffer<float> Circle( Size size );
}


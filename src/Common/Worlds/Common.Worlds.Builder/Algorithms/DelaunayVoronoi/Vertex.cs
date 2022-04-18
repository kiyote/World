namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

#pragma warning disable CA1819
public sealed class Vertex  {

	public Vertex() {
		Position = new float[2];
	}

	public Vertex(
		float x,
		float y
	) {
		Position = new float[] { x, y };
	}

	public float X {
		get { return Position[0]; }
	}

	public float Y {
		get { return Position[1]; }
	}

	public int Dimension => Position.Length;

	public int Id { get; set; }

	public int Tag { get; set; }

	public float[] Position { get; set; }

	public float SqrMagnitude {
		get {
			float sum = 0.0f;

			for( int i = 0; i < Dimension; i++ ) {
				sum += Position[i] * Position[i];
			}

			return sum;

		}
	}
}
#pragma warning restore CA1819

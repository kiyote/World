namespace Common.Core;

public interface IRandom {
	int NextInt();
	int NextInt( int upperBound );
	int NextInt( int lowerBound, int upperBound );
	bool NextBool();
	void NextBytes( byte[] buffer );
	void NextBytes( byte[] buffer, int bound1, int bound2 );
	float NextFloat();
	float NextFloat( float upperBound );
	float NextFloat( float lowerBound, float upperBound );
	double NextDouble();
	uint NextUInt();
	void Reinitialise( int seed );
}

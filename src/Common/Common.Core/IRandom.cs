namespace Common.Core;

public interface IRandom {
	int NextInt();
	int NextInt( int upperBound );
	int NextInt( int lowerBound, int upperBound );
	bool NextBool();
	void NextBytes( byte[] buffer );
	void NextBytes( byte[] buffer, int bound1, int bound2 );
	double NextDouble();
	uint NextUInt();
	void Reinitialise( int seed );
}

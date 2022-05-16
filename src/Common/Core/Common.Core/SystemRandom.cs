using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
[System.Diagnostics.CodeAnalysis.SuppressMessage( "Security", "CA5394:Do not use insecure randomness", Justification = "It's a game." )]
internal sealed class SystemRandom : IRandom {

	private Random _random;

	public SystemRandom() {
		_random = new Random();
	}

	bool IRandom.NextBool() {
		return _random.Next( 2 ) == 1;
	}

	void IRandom.NextBytes( byte[] buffer ) {
		_random.NextBytes( buffer );
	}

	void IRandom.NextBytes( byte[] buffer, int bound1, int bound2 ) {
		for (int i = 0; i < buffer.Length; i++) {
			buffer[i] = (byte)_random.Next( bound1, bound2 );
		}
	}

	double IRandom.NextDouble() {
		return _random.NextDouble();
	}

	float IRandom.NextFloat() {
		return _random.NextSingle();
	}

	float IRandom.NextFloat( float upperBound ) {
		return _random.NextSingle() * upperBound;
	}

	float IRandom.NextFloat( float lowerBound, float upperBound ) {
		float range = upperBound - lowerBound;
		return ( _random.NextSingle() * range ) + lowerBound;
	}

	int IRandom.NextInt() {
		return _random.Next();
	}

	int IRandom.NextInt( int upperBound ) {
		return _random.Next( upperBound );
	}

	int IRandom.NextInt( int lowerBound, int upperBound ) {
		return _random.Next( lowerBound, upperBound );
	}

	uint IRandom.NextUInt() {
		return (uint)_random.NextInt64();
	}

	void IRandom.Reinitialise( int seed ) {
		_random = new Random( seed );
	}
}

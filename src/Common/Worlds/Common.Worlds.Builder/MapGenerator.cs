namespace Common.Worlds.Builder;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal class MapGenerator {

	private readonly IRandom _random;

	public MapGenerator(
		IRandom random
	) {
		_random = random;
	}

}

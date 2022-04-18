using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Worlds.Builder.Generators;
internal class VoronoiGenerator {

	private readonly IRandom _random;

	public VoronoiGenerator(
		IRandom random
	) {
		_random = random;
	}

	public Buffer<float> Generate(
		Size size,
		int cells
	) {
		Location[] locations = new Location[cells];
		for (int i = 0; i < cells; i++) {
			locations[i] =
				new Location(
					_random.NextInt( size.Columns ),
					_random.NextInt( size.Rows )
				);
		}

		return Rasterize( size, locations );
	}

	private static Buffer<float> Rasterize(
		Size size,
		Location[] locations
	) {
		Buffer<float> result = new Buffer<float>( size );


		for( int row = 0; row < size.Rows; row++ ) {
			for( int column = 0; column < size.Columns; column++ ) {
				result[row][column] = ClosestLocation( column, row, locations );
			}
		}

		return result;
	}

	private static int ClosestLocation(
		int column,
		int row,
		Location[] locations
	) {
		int[] distances = new int[locations.Length];
		for (int i = 0; i < locations.Length; i++) {
			distances[i] =
				(int)(Math.Sqrt(
					Math.Pow( ( locations[i].Column - column ), 2 )
					+ Math.Pow( ( locations[i].Row - row ), 2 )
				) * 100.0);
		}

		return Array.IndexOf( distances, distances.Min() );
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class ForestBuilder : IForestBuilder {

	private readonly IRandom _random;

	public ForestBuilder(
		IRandom random
	) {
		_random = random;
	}

	HashSet<Cell> IForestBuilder.Create(
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains,
		HashSet<Cell> hills,
		Dictionary<Cell, float> temperatures,
		Dictionary<Cell, float> moistures
	) {
		HashSet<Cell> result = new HashSet<Cell>();
		foreach( Cell cell in fineLandforms ) {
			float temperature = temperatures[cell];
			float moisture = moistures[cell];

			//TODO: Fix this - mountains and hills can be forested, but
			// we have no way of rendering that currently
			if( mountains.Contains( cell )
				|| hills.Contains( cell )
			) {
				continue;
			}

			if( moisture < 0.2f ) {
				continue;
			}

			float chance = 0.0f;
			chance += ( 1.0f * ( temperature * moisture ) );
			if( hills.Contains( cell ) ) {
				chance /= 2.0f;
			} else if( mountains.Contains( cell ) ) {
				chance = 0.0f;
			}

			float value = _random.NextFloat();
			if( value < chance ) {
				result.Add( cell );
			}
		}

		return result;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Common.Worlds.Builder.Algorithms.Delaunay.Tests;

[TestFixture]
public sealed class SHullFactoryUnitTests {

	private IDelaunayFactory _factory;

	[SetUp]
	public void SetUp() {
		_factory = new SHullFactory();
	}

	[Test]
	public void Create_ValidPoints_SOMETHING() {
		IRandom random = new FastRandom();
		List<Point> points = new List<Point>();
		for( int i = 0; i < 3; i++) {
			points.Add( new Point(
				random.NextInt( 100 ),
				random.NextInt( 100 )
			) );
		}

		IReadOnlyList<IEdge> result = _factory.Create( points );

		Assert.IsNotNull( result );
	}
}

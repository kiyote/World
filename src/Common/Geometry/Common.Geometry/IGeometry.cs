namespace Common.Geometry;

public interface IGeometry {

	bool PointInPolygon( IReadOnlyList<IPoint> polygon, IPoint point );

	bool PointInPolygon( IReadOnlyList<IPoint> polygon, int x, int y );

	int LineLength( IPoint p1, IPoint p2 );


	void RasterizePolygon( IReadOnlyList<IPoint> polygon, Action<int, int> callback );

	void RasterizeLine( IPoint p1, IPoint p2, Action<int, int> callback );
}

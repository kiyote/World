namespace Common.Renderers;

public interface IWorldRenderer {
	Task RenderAsync( CancellationToken cancellationToken );
}

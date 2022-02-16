namespace Common.Manager.Renderers;

public interface IWorldRenderer {
	Task RenderAsync( CancellationToken cancellationToken );
}

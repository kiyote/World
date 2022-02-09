namespace Common.Service.Renderers;

public interface IWorldRenderer {
	Task RenderAsync( CancellationToken cancellationToken );
}

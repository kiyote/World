namespace Common.Manager.Renderers.Bitmap;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class BitmapWorldRenderer : IWorldRenderer {
	Task IWorldRenderer.RenderAsync(
		CancellationToken cancellationToken
	) {
		return Task.CompletedTask;
	}
}


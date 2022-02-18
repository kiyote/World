using Common.Files.Manager.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Server.Files.Manager.Repositories.Disk;

public static class ExtensionMethods {

	public static IServiceCollection AddDiskFileRepository(
		this IServiceCollection services
	) {
		services.AddSingleton<DiskFileRepository>();
		services.AddSingleton<IFileContentRepository>( s => s.GetRequiredService<DiskFileRepository>() );
		services.AddSingleton<IFileMetadataRepository>( s => s.GetRequiredService<DiskFileRepository>() );

		return services;
	}
}

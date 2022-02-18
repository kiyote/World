using Microsoft.Extensions.DependencyInjection;

namespace Server.Files.Manager.Disk;

public static class ExtensionMethods {

	public static IServiceCollection AddDiskFileManager(
		this IServiceCollection services
	) {
		services.AddSingleton<DiskFileManager>();
		services.AddSingleton<IDiskFileManager, DiskFileManager>();

		return services;
	}
}

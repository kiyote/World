using Microsoft.Extensions.DependencyInjection;

namespace Common.Files.Manager.Resource;

public static class ExtensionMethods {

	public static IServiceCollection AddResourceFileManager(
		this IServiceCollection services
	) {
		services.AddSingleton<IResourceFileRepository, ResourceFileRepository>();
		services.AddSingleton<IResourceFileManager, ResourceFileManager>();

		return services;
	}
}



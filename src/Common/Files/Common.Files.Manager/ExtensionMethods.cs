using Microsoft.Extensions.DependencyInjection;

namespace Common.Files.Manager;

public static class ExtensionMethods {

	public static IServiceCollection AddFileManager(
		this IServiceCollection services
	) {
		services.AddSingleton<IFileManager, FileManager>();

		return services;
	}
}


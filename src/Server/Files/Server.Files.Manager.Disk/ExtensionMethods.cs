using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.IO.Abstractions;

namespace Server.Files.Manager.Disk;

public static class ExtensionMethods {

	public static IServiceCollection AddDiskFileManager(
		this IServiceCollection services,
		string diskFolder
	) {
		services.TryAddSingleton<IFileSystem, FileSystem>();
		services.AddSingleton<IFileFolderProvider>( ( _ ) => new FileFolderProvider( diskFolder ) );
		services.AddSingleton<IDiskFileRepository, DiskFileRepository>();
		services.AddSingleton<IDiskFileManager, DiskFileManager>();

		return services;
	}
}

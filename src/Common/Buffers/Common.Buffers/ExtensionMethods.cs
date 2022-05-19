﻿using Microsoft.Extensions.DependencyInjection;

namespace Common.Buffers;

public static class ExtensionMethods {

	public static IServiceCollection AddArrayBuffer(
		this IServiceCollection services
	) {
		services.AddSingleton<IBufferOperator, BufferOperator>();
		services.AddSingleton<IBufferFactory, ArrayBufferFactory>();

		return services;
	}
}
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Titansmasher.Services.Configuration.Interfaces;

namespace Titansmasher.Services.Configuration.Extensions
{
    public static class ServiceCollectionExtensions
    {
        #region AddConfigService

        #region Default Implimentation

        public static IServiceCollection AddSingletonConfigService(this IServiceCollection collection)
            => collection.AddSingleton<IConfigService, ConfigService>();

        public static IServiceCollection AddScopedConfigService(this IServiceCollection collection)
            => collection.AddScoped<IConfigService, ConfigService>();

        public static IServiceCollection AddTransientConfigService(this IServiceCollection collection)
            => collection.AddTransient<IConfigService, ConfigService>();

        #endregion Default Implimentation

        #region Custom Implimentation

        public static IServiceCollection AddSingletonConfigService<TService>(this IServiceCollection collection) where TService : class, IConfigService
            => collection.AddSingleton<IConfigService, TService>();

        public static IServiceCollection AddScopedConfigService<TService>(this IServiceCollection collection) where TService : class, IConfigService
            => collection.AddScoped<IConfigService, TService>();

        public static IServiceCollection AddTransientConfigService<TService>(this IServiceCollection collection) where TService : class, IConfigService
            => collection.AddTransient<IConfigService, TService>();

        #endregion Custom Implimentation

        #endregion AddConfigService

        #region AddConfig

        public static IServiceCollection AddSingletonConfig<TConfig>(this IServiceCollection collection) where TConfig : class, new()
        {
            if (!collection.Any(d => d.ServiceType == typeof(IConfigService)))
                collection.AddTransientConfigService();
            return collection.AddSingleton(p => p.GetRequiredService<IConfigService>().Request<TConfig>());
        }

        public static IServiceCollection AddScopedConfig<TConfig>(this IServiceCollection collection) where TConfig : class, new()
        {
            if (!collection.Any(d => d.ServiceType == typeof(IConfigService)))
                collection.AddTransientConfigService();
            return collection.AddScoped(p => p.GetRequiredService<IConfigService>().Request<TConfig>());
        }

        public static IServiceCollection AddTransientConfig<TConfig>(this IServiceCollection collection) where TConfig : class, new()
        {
            if (!collection.Any(d => d.ServiceType == typeof(IConfigService)))
                collection.AddTransientConfigService();
            return collection.AddTransient(p => p.GetRequiredService<IConfigService>().Request<TConfig>());
        }

        #endregion AddConfig
    }
}
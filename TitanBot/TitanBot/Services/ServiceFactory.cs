using Microsoft.Extensions.DependencyInjection;
using System;

namespace TitanBot.Services
{
    public class ServiceFactory : IServiceProvider
    {
        private readonly Action<IServiceCollection> _buildAction;
        private readonly IServiceProvider _serviceProvider;

        public ServiceFactory(Action<IServiceCollection> action)
        {
            _buildAction = action;
            _serviceProvider = GetServiceCollection().BuildServiceProvider();
        }

        public object GetService(Type serviceType)
            => _serviceProvider.GetService(serviceType);

        private IServiceCollection GetServiceCollection()
        {
            var collection = new ServiceCollection();

            collection.AddSingleton(p => GetServiceCollection());
            collection.AddSingleton(p => p);

            _buildAction?.Invoke(collection);

            return collection;
        }
    }
}
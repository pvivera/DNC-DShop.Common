using Microsoft.Extensions.DependencyInjection;

namespace DShop.Common.Dispatchers
{
    public static class Extensions
    {
        public static void AddDispatchers(this IServiceCollection services)
        {
            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
            services.AddScoped<IDispatcher, Dispatcher>();
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        }
    }
}
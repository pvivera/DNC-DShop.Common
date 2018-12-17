using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DShop.Common.MailKit
{
    public static class Extensions
    {
        public static void AddMailKit(this IServiceCollection services)
        {
            services.AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();

                var options = configuration.GetOptions<MailKitOptions>("mailkit");

                return options;
            });
        }
    }
}

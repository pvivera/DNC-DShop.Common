using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DShop.Common.MailKit
{
    public static class Extensions
    {
        public static void AddMailKit(this IServiceCollection services)
        {
            services.AddSingleton(context =>
            {
                var configuration = context.GetService<IConfiguration>();

                var options = configuration.GetOptions<MailKitOptions>("mailkit");

                return options;
            });
        }
    }
}

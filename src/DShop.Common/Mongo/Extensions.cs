using DShop.Common.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace DShop.Common.Mongo
{
    public static class Extensions
    {
        public static void AddMongo(this IServiceCollection services)
        {
            services.AddSingleton(context =>
            {
                var configuration = context.GetService<IConfiguration>();
                var options = configuration.GetOptions<MongoDbOptions>("mongo");

                return options;
            });

            services.AddSingleton(context =>
            {
                var options = context.GetService<MongoDbOptions>();

                return new MongoClient(options.ConnectionString);
            });

            services.AddScoped(context =>
            {
                var options = context.GetService<MongoDbOptions>();
                var client = context.GetService<MongoClient>();
                return client.GetDatabase(options.Database);

            });

            services.AddScoped<IMongoDbInitializer, MongoDbInitializer>();

            services.AddScoped<IMongoDbSeeder, MongoDbSeeder>();
        }

        public static void AddMongoRepository<TEntity>(this IServiceCollection services, string collectionName)
            where TEntity : IIdentifiable
            => services.AddScoped<IMongoRepository<TEntity>>(ctx =>
                new MongoRepository<TEntity>(ctx.GetService<IMongoDatabase>(), collectionName));
    }
}
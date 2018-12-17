using System;
using System.Reflection;
using DShop.Common.Handlers;
using DShop.Common.Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Configuration;
using RawRabbit.Enrichers.MessageContext;
using RawRabbit.Instantiation;

namespace DShop.Common.RabbitMq
{
    public static class Extensions
    {
        public static IBusSubscriber UseRabbitMq(this IApplicationBuilder app)
            => new BusSubscriber(app);

        public static void AddRabbitMq(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(context =>
            {
                var configuration = context.GetService<IConfiguration>();
                var options = configuration.GetOptions<RabbitMqOptions>("rabbitMq");

                return options;
            });

            serviceCollection.AddSingleton(context =>
            {
                var configuration = context.GetService<IConfiguration>();
                var options = configuration.GetOptions<RawRabbitConfiguration>("rabbitMq");

                return options;
            });

            serviceCollection.Scan(scan => scan
                .FromCallingAssembly()
                .AddClasses(classes=>classes.AssignableTo(typeof(IEventHandler<>))).AsImplementedInterfaces().WithTransientLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>))).AsImplementedInterfaces().WithTransientLifetime()
            );

            serviceCollection.AddTransient<IHandler, Handler>();
            serviceCollection.AddTransient<IBusPublisher, BusPublisher>();

            ConfigureBus(serviceCollection);
        }

        private static void ConfigureBus(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IInstanceFactory>(context =>
            {
                var options = context.GetService<RabbitMqOptions>();
                var configuration = context.GetService<RawRabbitConfiguration>();
                var namingConventions = new CustomNamingConventions(options.Namespace);

                return RawRabbitFactory.CreateInstanceFactory(new RawRabbitOptions
                {
                    DependencyInjection = ioc =>
                    {
                        ioc.AddSingleton(options);
                        ioc.AddSingleton(configuration);
                        ioc.AddSingleton<INamingConventions>(namingConventions);
                    },
                    Plugins = p => p
                        .UseAttributeRouting()
                        .UseMessageContext<CorrelationContext>()
                        .UseContextForwarding()
                });
            });

            serviceCollection.AddScoped(context => context.GetService<IInstanceFactory>().Create());
        }

        private class CustomNamingConventions : NamingConventions
        {
            public CustomNamingConventions(string defaultNamespace)
            {
                ExchangeNamingConvention = type => GetExchangeName(defaultNamespace, type);
            }

            private static string GetExchangeName(string defaultNamespace, Type type)
                => $"{GetNamespace(defaultNamespace, type)}{type.Name.Underscore()}".ToLowerInvariant();

            private static string GetNamespace(string defaultNamespace, Type type)
            {
                var @namespace = type.GetCustomAttribute<MessageNamespaceAttribute>()?.Namespace ?? defaultNamespace;

                return string.IsNullOrWhiteSpace(@namespace) ? string.Empty : $"{@namespace}.";
            }
        }
    }
}
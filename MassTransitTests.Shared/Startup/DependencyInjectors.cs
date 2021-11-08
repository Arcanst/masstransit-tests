using System;
using System.Linq;
using System.Reflection;
using System.Text;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransitTests.DataTransferObjects.Commands;
using RabbitMQ.Client;

namespace MassTransitTests.Shared.Startup
{
    public static class DependencyInjectors
    {
        public static IRabbitMqBusFactoryConfigurator AddEventConsumer<TConsumer>(
            this IRabbitMqBusFactoryConfigurator config,
            IRegistration context,
            string queueName = null)
            where TConsumer : class, IConsumer
        {
            if (string.IsNullOrEmpty(queueName))
            {
                config.ReceiveEndpoint(e => { e.ConfigureConsumer<TConsumer>(context); });
            }
            else
            {
                config.ReceiveEndpoint(queueName, e => { e.ConfigureConsumer<TConsumer>(context); });
            }

            return config;
        }
        
        public static IRabbitMqBusFactoryConfigurator AddEventConsumer(
            this IRabbitMqBusFactoryConfigurator config,
            Type consumerType,
            IRegistration context,
            string queueName = null)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                config.ReceiveEndpoint(e =>
                {
                    e.ConfigureConsumer(context, consumerType);
                });
            }
            else
            {
                config.ReceiveEndpoint(queueName, e =>
                {
                    e.ConfigureConsumer(context, consumerType);
                });
            }

            return config;
        }

        public static IRabbitMqBusFactoryConfigurator AddQueryConsumer<TConsumer>(
            this IRabbitMqBusFactoryConfigurator config,
            IRegistration context)
            where TConsumer : class, IConsumer
        {
            return AddNonEventConsumer<TConsumer>(config, context);
        }
        
        public static IRabbitMqBusFactoryConfigurator AddQueryConsumer(
            this IRabbitMqBusFactoryConfigurator config,
            Type consumerType,
            IRegistration context)
        {
            return AddNonEventConsumer(consumerType, config, context);
        }

        public static IRabbitMqBusFactoryConfigurator AddCommandConsumer<TConsumer>(
            this IRabbitMqBusFactoryConfigurator config,
            IRegistration context)
            where TConsumer : class, IConsumer
        {
            return AddNonEventConsumer<TConsumer>(config, context);
        }
        
        public static IRabbitMqBusFactoryConfigurator AddCommandConsumer(
            this IRabbitMqBusFactoryConfigurator config,
            Type consumerType,
            IRegistration context)
        {
            return AddNonEventConsumer(consumerType, config, context);
        }
        
        private static IRabbitMqBusFactoryConfigurator AddNonEventConsumer<TConsumer>(
            IRabbitMqBusFactoryConfigurator config,
            IRegistration context)
            where TConsumer : class, IConsumer
        {
            return AddNonEventConsumer(typeof(TConsumer), config, context);
        }

        private static IRabbitMqBusFactoryConfigurator AddNonEventConsumer(
            Type consumerType,
            IRabbitMqBusFactoryConfigurator config,
            IRegistration context)
        {
            var routingKey = Assembly.GetEntryAssembly().GetName().Name;
            var messageType = consumerType
                .GetInterfaces()
                ?.First(i => i.IsGenericType)
                ?.GetGenericArguments()
                ?.First();

            if (messageType == null)
            {
                throw new InvalidOperationException();
            }
            
            var exchangeName = new StringBuilder(messageType.FullName)
                .Replace($".{messageType.Name}", string.Empty)
                .Append($":{messageType.Name}")
                .ToString();
            
            config.Send<TestCommand>(c =>
            {
                c.UseRoutingKeyFormatter(x => routingKey);
            });
            
            config.ReceiveEndpoint(e =>
            {
                e.ConfigureConsumeTopology = false;
                e.ExchangeType = ExchangeType.Direct;

                e.ConfigureConsumer(context, consumerType);
                e.Bind(exchangeName, b =>
                {
                    e.ExchangeType = ExchangeType.Direct;
                    b.RoutingKey = routingKey;
                });
            });

            return config;
        }
    }
}

using System;
using System.Linq;
using System.Reflection;
using System.Text;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.RabbitMqTransport.Topology;
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

        public static IRabbitMqBusFactoryConfigurator AddQueryConsumer<TConsumer>(
            this IRabbitMqBusFactoryConfigurator config,
            IRegistration context)
            where TConsumer : class, IConsumer
        {
            return AddNonEventConsumer<TConsumer>(config, context);
        }

        public static IRabbitMqBusFactoryConfigurator AddCommandConsumer<TConsumer>(
            this IRabbitMqBusFactoryConfigurator config,
            IRegistration context)
            where TConsumer : class, IConsumer
        {
            return AddNonEventConsumer<TConsumer>(config, context);
        }

        private static IRabbitMqBusFactoryConfigurator AddNonEventConsumer<TConsumer>(
            IRabbitMqBusFactoryConfigurator config,
            IRegistration context)
            where TConsumer : class, IConsumer
        {
            var routingKey = Assembly.GetEntryAssembly().GetName().Name;
            var messageType = typeof(TConsumer)
                .GetInterfaces()
                ?.First(i => i.IsGenericType)
                ?.GetGenericArguments()
                ?.First();

            if (messageType == null)
            {
                throw new InvalidOperationException(
                    $"Message type could not be extracted from the consumer type. ConsumerTypeName=[{typeof(TConsumer).Name}]");
            }

            config.ReceiveEndpoint(e =>
            {
                // var exchangeName = new StringBuilder(messageType.FullName)
                //     .Replace($".{messageType.Name}", string.Empty)
                //     .Append($":{messageType.Name}")
                //     .ToString();
                
                var exchangeName = messageType.FullName;
                
                e.ConfigureConsumeTopology = false;
                e.ExchangeType = ExchangeType.Direct;

                e.Consumer<TConsumer>(context);
                e.Bind(exchangeName, b =>
                {
                    e.ExchangeType = ExchangeType.Direct;
                    b.RoutingKey = routingKey;
                });
            });

            config.Send<TestCommand>(c =>
            {
                c.UseRoutingKeyFormatter(x => routingKey);
            });
            
            config.Publish<TestCommand>(c =>
            {
                c.ExchangeType = ExchangeType.Direct;
            });

            // Action<IRabbitMqMessageSendTopologyConfigurator<TestCommand>> action = c =>
            // {
            //     c.UseRoutingKeyFormatter(x => routingKey);
            // };

            // var sendConfigurationMethod =
            //     typeof(IRabbitMqBusFactoryConfigurator).GetMethod(nameof(IRabbitMqBusFactoryConfigurator.Send));
            //
            // sendConfigurationMethod
            //     .MakeGenericMethod(messageType)
            //     .Invoke(config, new [] { action });

            return config;
        }
    }
}
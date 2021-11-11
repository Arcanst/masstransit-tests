using System.Reflection;
using System.Text;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.Topology;
using MassTransitTests.DataTransferObjects;
using RabbitMQ.Client;

namespace MassTransitTests.Shared.Startup
{
    public static class DependencyInjectors
    {
        public static IRabbitMqBusFactoryConfigurator AddEventConsumer<TMessage, TConsumer>(
            this IRabbitMqBusFactoryConfigurator config,
            IRegistration context,
            string queueName = null)
            where TMessage : class, IMessage
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

        public static IRabbitMqBusFactoryConfigurator AddQueryConsumer<TMessage, TConsumer>(
            this IRabbitMqBusFactoryConfigurator config,
            IRegistration context)
            where TMessage : class, IMessage
            where TConsumer : class, IConsumer
        {
            return AddNonEventConsumer<TMessage, TConsumer>(config, context);
        }

        public static IRabbitMqBusFactoryConfigurator AddCommandConsumer<TMessage, TConsumer>(
            this IRabbitMqBusFactoryConfigurator config,
            IRegistration context)
            where TMessage : class, IMessage
            where TConsumer : class, IConsumer
        {
            return AddNonEventConsumer<TMessage, TConsumer>(config, context);
        }

        private static IRabbitMqBusFactoryConfigurator AddNonEventConsumer<TMessage, TConsumer>(
            IRabbitMqBusFactoryConfigurator config,
            IRegistration context)
            where TMessage : class, IMessage
            where TConsumer : class, IConsumer
        {
            var routingKey = Assembly.GetEntryAssembly().GetName().Name;
            var endpointName = $"{routingKey}-{typeof(TMessage).Name}";
            var customEntityNameFormatter = new CustomEntityNameFormatter();

            SetupSendTopology<TMessage>(config, routingKey, endpointName);

            config.ReceiveEndpoint(customEntityNameFormatter.FormatEntityName<TMessage>(), e =>
            {
                e.ConfigureConsumeTopology = false;
                e.ExchangeType = ExchangeType.Direct;

                e.Bind<TMessage>(b =>
                {
                    b.RoutingKey = routingKey;
                    b.ExchangeType = ExchangeType.Direct;
                });
                e.Consumer<TConsumer>(context);
            });
            
            config.MessageTopology.SetEntityNameFormatter(customEntityNameFormatter);

            return config;
        }

        /// <summary>
        /// Used by _bus.Send, _bus.Publish and _bus.Request methods.
        /// Afaik it does not change anything in the topology, just specifies how to use it when it's set up.
        /// </summary>
        private static void SetupSendTopology<TMessage>(IRabbitMqBusFactoryConfigurator config, string routingKey, string endpointName)
            where TMessage : class, IMessage
        {
            // config.Message<TMessage>(c =>
            // {
            //     c.SetEntityName(endpointName);
            // });

            config.Send<TMessage>(c =>
            {
                c.UseRoutingKeyFormatter(x => routingKey);
            });
            
            config.Publish<TMessage>(c =>
            {
                c.ExchangeType = ExchangeType.Direct;
            });
        }
    }
    
    internal sealed class CustomEntityNameFormatter : IEntityNameFormatter
    {
        public string FormatEntityName<TMessage>()
        {
            var routingKey = Assembly.GetEntryAssembly().GetName().Name;
            var endpointName = $"{typeof(TMessage).Name}.{routingKey}";

            return endpointName;
        }
    }
}
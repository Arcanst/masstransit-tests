using System;
using System.Collections.Generic;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using MassTransitTests.DataTransferObjects;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransitTests.Shared.Startup
{
    public abstract class SharedStartup
    {
        private delegate IRabbitMqBusFactoryConfigurator ConsumerRegistration(
            IRabbitMqBusFactoryConfigurator rabbitMqBusFactoryConfigurator, IRegistration registration);

        private readonly ICollection<Action<IServiceCollectionBusConfigurator>> _consumerRegistrationActions;

        private readonly
            ICollection<ConsumerRegistration>
            _commandConsumerRegistrationActions;
        
        private readonly
            ICollection<ConsumerRegistration>
            _queryConsumerRegistrationActions;
        
        private readonly
            ICollection<ConsumerRegistration>
            _eventConsumerRegistrationActions;

        protected SharedStartup()
        {
            _commandConsumerRegistrationActions = new List<ConsumerRegistration>();
            _queryConsumerRegistrationActions = new List<ConsumerRegistration>();
            _eventConsumerRegistrationActions = new List<ConsumerRegistration>();
            _consumerRegistrationActions = new List<Action<IServiceCollectionBusConfigurator>>();
        }

        protected void RegisterQueryConsumer<TQuery, TQueryConsumer>()
            where TQuery : class, IMessage
            where TQueryConsumer : class, IConsumer
        {
            _queryConsumerRegistrationActions.Add((config, registration)
                => config.AddQueryConsumer<TQuery, TQueryConsumer>(registration));
            _consumerRegistrationActions.Add(c => c.AddConsumer<TQueryConsumer>());
        }
        
        protected void RegisterCommandConsumer<TCommand, TCommandConsumer>()
            where TCommand : class, IMessage
            where TCommandConsumer : class, IConsumer
        {
            _commandConsumerRegistrationActions.Add((config, registration) =>
                config.AddCommandConsumer<TCommand, TCommandConsumer>(registration));
            _consumerRegistrationActions.Add(c => c.AddConsumer<TCommandConsumer>());
        }
        
        protected void RegisterEventConsumer<TEvent, TEventConsumer>()
            where TEvent : class, IMessage
            where TEventConsumer : class, IConsumer
        {
            _eventConsumerRegistrationActions.Add((config, registration) =>
                config.AddEventConsumer<TEvent, TEventConsumer>(registration));
            _consumerRegistrationActions.Add(c => c.AddConsumer<TEventConsumer>());
        }

        protected IServiceCollection AddMassTransit(IServiceCollection services)
        {
            services.AddMassTransit(massTransitConfig =>
            {
                // foreach (var consumerRegistrationAction in _consumerRegistrationActions)
                // {
                //     consumerRegistrationAction(massTransitConfig);
                // }

                massTransitConfig.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host("localhost", "/", config =>
                    {
                        config.Username("user");
                        config.Password("user");
                    });

                    RegisterQueryConsumers(cfg, ctx);
                    RegisterCommandConsumers(cfg, ctx);
                    RegisterEventConsumers(cfg, ctx);

                    cfg.ConfigureEndpoints(ctx);
                });
            });
            
            services.AddMassTransitHostedService();

            void RegisterQueryConsumers(IRabbitMqBusFactoryConfigurator config, IRegistration context)
            {
                foreach (var consumerRegistration in _queryConsumerRegistrationActions)
                {
                    consumerRegistration(config, context);
                }
            }
            
            void RegisterCommandConsumers(IRabbitMqBusFactoryConfigurator config, IRegistration context)
            {
                foreach (var consumerRegistration in _commandConsumerRegistrationActions)
                {
                    consumerRegistration(config, context);
                }
            }
            
            void RegisterEventConsumers(IRabbitMqBusFactoryConfigurator config, IRegistration context)
            {
                foreach (var consumerRegistration in _eventConsumerRegistrationActions)
                {
                    consumerRegistration(config, context);
                }
            }

            return services;
        }
    }
}
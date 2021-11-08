using System;
using System.Collections.Generic;
using System.Linq;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransitTests.Shared.Startup
{
    public abstract class SharedStartup
    {
        private readonly ICollection<Type> _registeredQueryConsumerTypes;
        private readonly ICollection<Type> _registeredCommandConsumerTypes;
        private readonly ICollection<Type> _registeredEventConsumerTypes;

        private readonly ICollection<Action<IServiceCollectionBusConfigurator>> _consumerRegistrations;
        private readonly ICollection<Func<IRabbitMqBusFactoryConfigurator, IRegistration, IRabbitMqBusFactoryConfigurator>> _sth;

        protected SharedStartup()
        {
            _registeredQueryConsumerTypes = new List<Type>();
            _registeredCommandConsumerTypes = new List<Type>();
            _registeredEventConsumerTypes = new List<Type>();
            _sth = new List<Func<IRabbitMqBusFactoryConfigurator, IRegistration, IRabbitMqBusFactoryConfigurator>>();
            _consumerRegistrations = new List<Action<IServiceCollectionBusConfigurator>>();
        }

        protected void RegisterQueryConsumer<TQueryConsumer>()
        {
            _registeredQueryConsumerTypes.Add(typeof(TQueryConsumer));
        }
        
        protected void RegisterCommandConsumer<TCommandConsumer>()
            where TCommandConsumer : class, IConsumer
        {
            _sth.Add((config, registration) => config.AddCommandConsumer<TCommandConsumer>(registration));
            _consumerRegistrations.Add(c => c.AddConsumer<TCommandConsumer>());
        }
        
        protected void RegisterCommandConsumerOld<TCommandConsumer>()
        {
            _registeredCommandConsumerTypes.Add(typeof(TCommandConsumer));
        }
        
        protected void RegisterEventConsumer<TEventConsumer>()
        {
            _registeredEventConsumerTypes.Add(typeof(TEventConsumer));
        }

        protected IServiceCollection AddMassTransit(IServiceCollection services)
        {
            services.AddMassTransit(massTransitConfig =>
            {
                var allConsumerTypes = _registeredQueryConsumerTypes
                    .Union(_registeredCommandConsumerTypes)
                    .Union(_registeredEventConsumerTypes);
                
                foreach (var registrationAction in _consumerRegistrations)
                {
                    registrationAction(massTransitConfig);
                }

                massTransitConfig.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host("localhost", "/", config =>
                    {
                        config.Username("rabbitmquser");
                        config.Password("DEBmbwkSrzy9D1T9cJfa");
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
                foreach (var registeredConsumerType in _registeredQueryConsumerTypes)
                {
                    config.AddQueryConsumer(registeredConsumerType, context);
                }
            }
            
            void RegisterCommandConsumers(IRabbitMqBusFactoryConfigurator config, IRegistration context)
            {
                foreach (var configurator in _sth)
                {
                    configurator(config, context);
                }
            }
            
            void RegisterEventConsumers(IRabbitMqBusFactoryConfigurator config, IRegistration context)
            {
                foreach (var registeredConsumerType in _registeredEventConsumerTypes)
                {
                    config.AddEventConsumer(registeredConsumerType, context);
                }
            }

            return services;
        }
    }
}
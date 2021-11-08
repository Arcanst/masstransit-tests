using MassTransit;
using MassTransitTests.DataTransferObjects.Queries;
using MassTransitTests.Shared.Consumers.Commands;
using MassTransitTests.Shared.Consumers.Events;
using MassTransitTests.Shared.Consumers.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MassTransitTests.Worker
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMassTransit(massTransitConfig =>
            {
                // round robin
                massTransitConfig.AddConsumer<GetDataQueryConsumer>();
                massTransitConfig.AddConsumer<SendToOneOfManyCommandConsumer>();

                massTransitConfig.AddRequestClient<GetDataQuery>();

                massTransitConfig.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host("localhost", "/", config =>
                    {
                        config.Username("guest");
                        config.Password("guest");
                    });

                    cfg.ConfigureEndpoints(ctx);

                    // this needs to be done per broadcast consumer
                    cfg.ReceiveEndpoint(e =>
                    {
                        e.Consumer<TranslationChangedEventConsumer>();
                    });
                });
            });

            services.AddMassTransitHostedService();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }
    }
}

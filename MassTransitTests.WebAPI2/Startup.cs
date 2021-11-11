using MassTransit;
using MassTransitTests.DataTransferObjects.Commands;
using MassTransitTests.Library1;
using MassTransitTests.Library1.Consumers;
using MassTransitTests.Shared.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MassTransitTests.WebAPI2
{
    public class Startup : SharedStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMediator();

            services.AddTransient<ILibrary1Service, Library1Service>();
            
            RegisterConsumers();
            AddMassTransit(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        
        private void RegisterConsumers()
        {
            RegisterCommandConsumer<TestCommand, TestCommandConsumer>();
        }
    }
}
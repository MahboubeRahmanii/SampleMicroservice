using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Ordering.Application;
using Ordering.Infrastructure;
using System.Reflection;
using MediatR;
using Ordering.Application.Features.Orders.Queries.GetOrdersList;
using MassTransit;
using EventBus.Messages.Common;
using Ordering.Api.EventBusConsumer;

namespace Ordering.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddApplicationServices();
            services.AddInfrastructureServices(Configuration);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering.Api", Version = "v1" });
            });

            services.AddAutoMapper(typeof(Startup));

            services.AddMassTransit(conifg =>
            {
                conifg.AddConsumer<BasketCheckoutConsumer>();
                conifg.UsingRabbitMq((ctx, conf) =>
                {
                    conf.Host(Configuration.GetValue<string>("EventBusSettings:HostAddress"));
                    conf.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
                    {
                        c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
                    });
                });
            });

            services.AddMassTransitHostedService();
            services.AddScoped<BasketCheckoutConsumer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering.Api v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

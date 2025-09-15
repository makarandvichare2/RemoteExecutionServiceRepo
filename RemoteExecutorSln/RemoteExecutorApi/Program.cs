
using Ardalis.ListStartupServices;
using Ardalis.SharedKernel;
using BikeShop.API.Controllers;
using FluentValidation;
using MediatR;
using RemoteExecutorGateWayApi.Commands;
using System.Reflection;

namespace RemoteExecutorApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            ConfigureMediatR(builder);
            builder.Services.AddInfrastructureServices(builder.Configuration);

            var app = builder.Build();

            app.UseCors(x => x
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
                app.UseShowAllServicesMiddleware();
            }

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }

        static void ConfigureMediatR(WebApplicationBuilder builder)
        {
            var mediatRAssemblies = new[]
            {
                Assembly.GetAssembly(typeof(PowershellRequestCommand)), // UseCases
                Assembly.GetAssembly(typeof(HttpRequestCommand)), // UseCases
            };
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies!));
            builder.Services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(HttpRequestValidator)));
            builder.Services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(PowershellRequestValidator)));
            builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            builder.Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
            builder.Services.AddScoped(typeof(ICommandFactory), typeof(CommandFactory));
        }
    }
}

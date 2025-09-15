using RemoteExecutorApi.API.Controllers;
using RemoteExecutorApi.API.Validators;
using FluentValidation;
using RemoteExecutorGateWayApi.Services;
using RemoteExecutorGateWayApi.ViewModels.Requests;
using RemoteExecutorGateWayApi.Controllers;

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
            builder.Services.AddHealthChecks();
            ConfigureDependencies(builder);

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
            }

            app.UseAuthorization();
            app.MapControllers();
            app.MapHealthChecks("/ping");
            app.Run();
        }

        static void ConfigureDependencies(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped(typeof(AbstractValidator<HttpExecutorRequest>), typeof(HttpRequestValidator));
            builder.Services.AddScoped(typeof(AbstractValidator<PowerShellExecutorRequest>), typeof(PowershellRequestValidator));
            builder.Services.AddScoped(typeof(AbstractValidator<ExecutorJsonRequest>), typeof(ExecutorJsonRequestValidator));
            builder.Services.AddScoped(typeof(IOrchestratorService), typeof(OrchestratorService));
            builder.Services.AddScoped(typeof(IHttpExecutorService), typeof(HttpExecutorService));
            builder.Services.AddScoped(typeof(IPowershellExecutorService), typeof(PowershellExecutorService));
        }
    }
}

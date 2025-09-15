using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using System;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
      this IServiceCollection services,
      ConfigurationManager config)
    {
        //services.AddScoped<IListBikeQueryService, ListBikeQueryService>();
        //services.AddScoped<IGetBikeQueryService, GetBikeQueryService>();

        return services;
    }
}
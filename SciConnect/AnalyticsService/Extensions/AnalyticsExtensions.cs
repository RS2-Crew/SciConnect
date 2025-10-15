using AnalyticsService.Data;
using AnalyticsService.Services;
using Microsoft.EntityFrameworkCore;

namespace AnalyticsService.Extensions
{
    public static class AnalyticsExtensions
    {
        public static IServiceCollection ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AnalyticsContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                       .EnableSensitiveDataLogging(false)
                       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            return services;
        }

        public static IServiceCollection ConfigureMiscellaneousServices(this IServiceCollection services)
        {
            services.AddScoped<IAnalyticsService, AnalyticsServiceImpl>();
            
            return services;
        }
    }
}

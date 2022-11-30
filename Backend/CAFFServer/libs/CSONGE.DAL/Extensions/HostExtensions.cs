using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CSONGE.Dal.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext, TProgram>(this IHost host)
            where TContext : DbContext
            where TProgram : class
        {
            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var logger = serviceProvider.GetRequiredService<ILogger<TProgram>>();
                var context = serviceProvider.GetRequiredService<TContext>();

                logger.LogInformation("Start migrating database");
                
                context.Database.Migrate();
                logger.LogInformation("Database migration finished");
            }

            return host;
        }
    }
}

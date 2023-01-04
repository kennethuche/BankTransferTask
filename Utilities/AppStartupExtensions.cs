using BankTransferTask.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace BankTransferTask.Utilities;

public static class AppStartupExtensions
    {

    public static void AutoMigrateDatabase(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var factory = serviceScope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var context = factory.CreateDbContext();
        try
        {
        
            context.Database.Migrate();
            logger.LogInformation("Migration successful");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to migrate with error {e.Message}", e.Message);
        }
    }
}


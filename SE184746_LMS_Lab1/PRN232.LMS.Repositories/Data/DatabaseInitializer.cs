using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PRN232.LMS.Repositories.Data;

public static class DatabaseInitializer
{
    private const string SeedResourceName = "PRN232.LMS.Repositories.SeedData.SeedData.sql";
    private const int MaxRetries = 30;

    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LmsDbContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(DatabaseInitializer));

        for (var attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                await context.Database.MigrateAsync(cancellationToken);
                await SeedAsync(context, cancellationToken);
                logger.LogInformation("Database migrated and seeded successfully.");
                return;
            }
            catch (Exception ex) when (attempt < MaxRetries)
            {
                logger.LogWarning(
                    ex,
                    "Database not ready. Retrying in 2 seconds ({Attempt}/{MaxRetries})...",
                    attempt,
                    MaxRetries);

                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }
    }

    private static async Task SeedAsync(LmsDbContext context, CancellationToken cancellationToken)
    {
        if (await context.Semesters.AnyAsync(cancellationToken))
        {
            return;
        }

        var sql = await ReadSeedScriptAsync(cancellationToken);
        await context.Database.ExecuteSqlRawAsync(sql, cancellationToken);
    }

    private static async Task<string> ReadSeedScriptAsync(CancellationToken cancellationToken)
    {
        var assembly = typeof(DatabaseInitializer).Assembly;
        await using var stream = assembly.GetManifestResourceStream(SeedResourceName)
            ?? throw new InvalidOperationException($"Embedded resource not found: {SeedResourceName}");

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(cancellationToken);
    }
}

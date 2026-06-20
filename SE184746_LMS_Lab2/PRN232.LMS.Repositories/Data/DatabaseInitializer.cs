using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PRN232.LMS.Repositories.Entities;

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
        if (!await context.Semesters.AnyAsync(cancellationToken))
        {
            var sql = await ReadSeedScriptAsync(cancellationToken);
            await context.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }

        await SeedUsersAsync(context, cancellationToken);
    }

    private static async Task SeedUsersAsync(LmsDbContext context, CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var users = new List<User>
        {
            new()
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin"
            },
            new()
            {
                Username = "user",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                Role = "User"
            }
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync(cancellationToken);
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

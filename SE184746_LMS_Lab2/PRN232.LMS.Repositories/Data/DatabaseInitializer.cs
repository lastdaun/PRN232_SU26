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
        await EnsureUserAsync(context, "admin", "123456", "Admin", cancellationToken);
        await EnsureUserAsync(context, "user",  "123456", "User",  cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task EnsureUserAsync(
        LmsDbContext context, string username, string password, string role,
        CancellationToken cancellationToken)
    {
        var existing = await context.Users
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

        if (existing == null)
        {
            context.Users.Add(new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role
            });
        }
        else if (!BCrypt.Net.BCrypt.Verify(password, existing.PasswordHash))
        {
            existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            existing.Role = role;
        }
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

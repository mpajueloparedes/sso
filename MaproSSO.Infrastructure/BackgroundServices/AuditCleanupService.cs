using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MaproSSO.Application.Common.Interfaces;

namespace MaproSSO.Infrastructure.BackgroundServices;

public class AuditCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuditCleanupService> _logger;
    private readonly AuditCleanupOptions _options;

    public AuditCleanupService(
        IServiceProvider serviceProvider,
        ILogger<AuditCleanupService> logger,
        IOptions<AuditCleanupOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupOldLogs();
                await Task.Delay(TimeSpan.FromHours(_options.CleanupIntervalHours), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during audit log cleanup");
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken); // Wait 30 minutes before retry
            }
        }
    }

    private async Task CleanupOldLogs()
    {
        using var scope = _serviceProvider.CreateScope();
        var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();

        var cutoffDate = DateTime.UtcNow.AddDays(-_options.RetentionDays);

        _logger.LogInformation("Starting audit log cleanup for logs older than {CutoffDate}", cutoffDate);

        await auditService.CleanupOldLogsAsync(cutoffDate);

        _logger.LogInformation("Audit log cleanup completed");
    }
}

public class AuditCleanupOptions
{
    public const string SectionName = "AuditCleanup";

    public int RetentionDays { get; set; } = 90; // Keep logs for 90 days by default
    public int CleanupIntervalHours { get; set; } = 24; // Run cleanup daily
}
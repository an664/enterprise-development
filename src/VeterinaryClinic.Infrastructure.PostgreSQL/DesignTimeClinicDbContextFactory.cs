using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VeterinaryClinic.Infrastructure.PostgreSQL;

/// <summary>
/// Позволяет инструментам миграций построить модель без запуска API и Aspire.
/// </summary>
public class DesignTimeClinicDbContextFactory : IDesignTimeDbContextFactory<ClinicDbContext>
{
    /// <inheritdoc />
    public ClinicDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ClinicDbContext>().UseNpgsql().Options;
        return new(options);
    }
}

using LivvaPlatform.API.IdentityAndProfileManagement.Infrastructure.Persistence.EFC.Configuration.Extensions;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Infrastructure.Persistence.EFC.Configuration.Extensions;
using LivvaPlatform.API.PolicyManagement.Infrastructure.Persistence.EFC.Configuration.Extensions;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
///     Application database context
/// </summary>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Apply audit timestamp interceptor for all IAuditableEntity implementations
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Identity and Profile Management Bounded Context
        builder.ApplyIdentityAndProfileManagementConfiguration();

        // Insurance Offering and Applications Bounded Context
        builder.ApplyInsuranceOfferingAndApplicationsConfiguration();

        // Policy Management Bounded Context
        builder.ApplyPolicyManagementConfiguration();

        builder.UseSnakeCaseNamingConvention();
    }
}
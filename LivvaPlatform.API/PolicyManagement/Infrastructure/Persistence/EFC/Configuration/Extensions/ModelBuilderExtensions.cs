using LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.PolicyManagement.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LivvaPlatform.API.PolicyManagement.Infrastructure.Persistence.EFC.Configuration.Extensions;

/// <summary>
///     Entity configuration of the Policy Management bounded context.
/// </summary>
public static class ModelBuilderExtensions
{
    private static readonly ValueConverter<DateOnly, DateTime> DateConverter =
        new(date => date.ToDateTime(TimeOnly.MinValue), value => DateOnly.FromDateTime(value));

    /// <summary>
    ///     Applies the mapping of the Policies, PolicyCoverages, PolicyDocuments and Renewals tables, including the
    ///     demonstration policies of the Web Application.
    /// </summary>
    /// <param name="builder">The model builder.</param>
    public static void ApplyPolicyManagementConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Policy>(policy =>
        {
            policy.HasKey(p => p.Id);
            policy.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            policy.Property(p => p.UserId).IsRequired();
            policy.Property(p => p.ApplicationId).IsRequired();
            policy.Property(p => p.PolicyNumber).IsRequired().HasMaxLength(50);
            policy.HasIndex(p => p.PolicyNumber).IsUnique();
            policy.Property(p => p.InsuranceType).IsRequired().HasConversion<string>().HasMaxLength(20);
            policy.Property(p => p.StartDate).IsRequired().HasConversion(DateConverter).HasColumnType("date");
            policy.Property(p => p.ExpirationDate).IsRequired().HasConversion(DateConverter).HasColumnType("date");
            policy.Property(p => p.PremiumAmount).IsRequired().HasPrecision(12, 2);
            policy.Property(p => p.CoverageAmount).IsRequired().HasPrecision(12, 2);
            policy.Property(p => p.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
            policy.Ignore(p => p.CanBeRenewed);
            policy.HasData(
                new { Id = 1L, UserId = 1L, ApplicationId = 1L, PolicyNumber = "LIV-VEH-0001", InsuranceType = InsuranceType.VEHICLE, StartDate = new DateOnly(2026, 1, 15), ExpirationDate = new DateOnly(2027, 1, 14), PremiumAmount = 1200m, CoverageAmount = 60000m, Status = PolicyStatus.ACTIVE, CreatedAt = (DateTimeOffset?)Date(2026, 1, 15), UpdatedAt = (DateTimeOffset?)Date(2026, 1, 15) },
                new { Id = 2L, UserId = 1L, ApplicationId = 2L, PolicyNumber = "LIV-LIF-0002", InsuranceType = InsuranceType.LIFE, StartDate = new DateOnly(2026, 3, 1), ExpirationDate = new DateOnly(2027, 2, 28), PremiumAmount = 960m, CoverageAmount = 100000m, Status = PolicyStatus.ACTIVE, CreatedAt = (DateTimeOffset?)Date(2026, 3, 1), UpdatedAt = (DateTimeOffset?)Date(2026, 3, 1) },
                new { Id = 3L, UserId = 1L, ApplicationId = 3L, PolicyNumber = "LIV-VEH-0003", InsuranceType = InsuranceType.VEHICLE, StartDate = new DateOnly(2025, 8, 1), ExpirationDate = new DateOnly(2026, 7, 31), PremiumAmount = 1080m, CoverageAmount = 45000m, Status = PolicyStatus.EXPIRED, CreatedAt = (DateTimeOffset?)Date(2025, 8, 1), UpdatedAt = (DateTimeOffset?)Date(2026, 7, 31) });
        });

        builder.Entity<PolicyCoverage>(coverage =>
        {
            coverage.HasKey(c => c.Id);
            coverage.Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
            coverage.HasOne<Policy>().WithMany().HasForeignKey(c => c.PolicyId).OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_policy_coverages_policy");
            coverage.Property(c => c.Name).IsRequired().HasMaxLength(150);
            coverage.Property(c => c.Description).HasMaxLength(500);
            coverage.Property(c => c.CoverageLimit).IsRequired().HasPrecision(12, 2);
            coverage.HasData(
                new { Id = 1L, PolicyId = 1L, Name = "Vehicle damage", Description = "Coverage for damage to the insured vehicle.", CoverageLimit = 60000m, CreatedAt = (DateTimeOffset?)Date(2026, 1, 15), UpdatedAt = (DateTimeOffset?)Date(2026, 1, 15) },
                new { Id = 2L, PolicyId = 1L, Name = "Third-party liability", Description = "Coverage for damage caused to third parties.", CoverageLimit = 30000m, CreatedAt = (DateTimeOffset?)Date(2026, 1, 15), UpdatedAt = (DateTimeOffset?)Date(2026, 1, 15) },
                new { Id = 3L, PolicyId = 2L, Name = "Life coverage", Description = "Financial protection for registered beneficiaries.", CoverageLimit = 100000m, CreatedAt = (DateTimeOffset?)Date(2026, 3, 1), UpdatedAt = (DateTimeOffset?)Date(2026, 3, 1) });
        });

        builder.Entity<PolicyDocument>(document =>
        {
            document.HasKey(d => d.Id);
            document.Property(d => d.Id).IsRequired().ValueGeneratedOnAdd();
            document.HasOne<Policy>().WithMany().HasForeignKey(d => d.PolicyId).OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_policy_documents_policy");
            document.Property(d => d.DocumentType).IsRequired().HasMaxLength(50);
            document.Property(d => d.Name).IsRequired().HasMaxLength(150);
            document.Property(d => d.FileUrl).HasMaxLength(500);
            document.HasData(
                new { Id = 1L, PolicyId = 1L, DocumentType = "policy", Name = "Vehicle policy document", FileUrl = (string?)"", CreatedAt = (DateTimeOffset?)Date(2026, 1, 15), UpdatedAt = (DateTimeOffset?)Date(2026, 1, 15) },
                new { Id = 2L, PolicyId = 2L, DocumentType = "policy", Name = "Life policy document", FileUrl = (string?)"", CreatedAt = (DateTimeOffset?)Date(2026, 3, 1), UpdatedAt = (DateTimeOffset?)Date(2026, 3, 1) });
        });

        builder.Entity<Renewal>(renewal =>
        {
            renewal.HasKey(r => r.Id);
            renewal.Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();
            renewal.HasOne<Policy>().WithMany().HasForeignKey(r => r.PolicyId).OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_renewals_policy");
            renewal.Property(r => r.RequestedAt).IsRequired();
            renewal.Property(r => r.ProposedExpirationDate).HasConversion(DateConverter).HasColumnType("date");
            renewal.Property(r => r.ProposedPremium).HasPrecision(12, 2);
            renewal.Property(r => r.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
            renewal.Ignore(r => r.IsOpen);
        });
    }

    private static DateTimeOffset Date(int year, int month, int day) => new(year, month, day, 10, 0, 0, TimeSpan.Zero);
}

using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Infrastructure.Persistence.EFC.Configuration.Extensions;

/// <summary>
///     Entity configuration of the Claims and Indemnities bounded context.
/// </summary>
public static class ModelBuilderExtensions
{
    private static readonly ValueConverter<DateOnly, DateTime> DateConverter =
        new(date => date.ToDateTime(TimeOnly.MinValue),
            value => DateOnly.FromDateTime(value));

    /// <summary>
    ///     Applies the mapping of vehicle claims, indemnity requests and their status histories.
    /// </summary>
    /// <param name="builder">The model builder.</param>
    public static void ApplyClaimsAndIndemnitiesConfiguration(this ModelBuilder builder)
    {
        builder.Entity<VehicleClaim>(claim =>
        {
            claim.HasKey(c => c.Id);
            claim.Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();

            claim.Property(c => c.UserId).IsRequired();
            claim.Property(c => c.PolicyId).IsRequired();

            claim.Property(c => c.ClaimNumber)
                .IsRequired()
                .HasMaxLength(50);

            claim.HasIndex(c => c.ClaimNumber).IsUnique();
            claim.HasIndex(c => c.UserId);
            claim.HasIndex(c => c.PolicyId);

            claim.Property(c => c.IncidentDate)
                .IsRequired()
                .HasConversion(DateConverter)
                .HasColumnType("date");

            claim.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(1000);

            claim.Property(c => c.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            claim.Property(c => c.CompensationAmount)
                .HasPrecision(12, 2);

            claim.Property(c => c.ResolvedAt);

            claim.HasMany(c => c.StatusHistories)
                .WithOne()
                .HasForeignKey(h => h.ClaimId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_claim_status_histories_vehicle_claim");
        });

        builder.Entity<ClaimStatusHistory>(history =>
        {
            history.HasKey(h => h.Id);
            history.Property(h => h.Id).IsRequired().ValueGeneratedOnAdd();

            history.Property(h => h.ClaimId).IsRequired();

            history.Property(h => h.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            history.Property(h => h.Comment)
                .IsRequired()
                .HasMaxLength(500);

            history.Property(h => h.ChangedAt).IsRequired();

            history.HasIndex(h => h.ClaimId);
        });

        builder.Entity<IndemnityRequest>(request =>
        {
            request.HasKey(r => r.Id);
            request.Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();

            request.Property(r => r.PolicyId).IsRequired();
            request.Property(r => r.BeneficiaryId).IsRequired();

            request.HasIndex(r => r.PolicyId);
            request.HasIndex(r => r.BeneficiaryId);

            request.Property(r => r.RequestDate)
                .IsRequired()
                .HasConversion(DateConverter)
                .HasColumnType("date");

            request.Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(1000);

            request.Property(r => r.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            request.Property(r => r.IndemnityAmount)
                .HasPrecision(12, 2);

            request.Property(r => r.ResolvedAt);

            request.HasMany(r => r.StatusHistories)
                .WithOne()
                .HasForeignKey(h => h.IndemnityRequestId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_indemnity_status_histories_request");
        });

        builder.Entity<IndemnityStatusHistory>(history =>
        {
            history.HasKey(h => h.Id);
            history.Property(h => h.Id).IsRequired().ValueGeneratedOnAdd();

            history.Property(h => h.IndemnityRequestId).IsRequired();

            history.Property(h => h.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            history.Property(h => h.Comment)
                .IsRequired()
                .HasMaxLength(500);

            history.Property(h => h.ChangedAt).IsRequired();

            history.HasIndex(h => h.IndemnityRequestId);
        });
    }
}

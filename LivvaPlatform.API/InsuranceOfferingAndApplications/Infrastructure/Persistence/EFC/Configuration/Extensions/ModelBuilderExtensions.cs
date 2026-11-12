using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Infrastructure.Persistence.EFC.Configuration.Extensions;

/// <summary>
///     Entity configuration of the Insurance Offering and Applications bounded context.
/// </summary>
public static class ModelBuilderExtensions
{
    private static readonly DateTimeOffset SeedDate = new(2026, 9, 1, 10, 0, 0, TimeSpan.Zero);

    /// <summary>
    ///     Applies the mapping of the Insurers, InsuranceProducts, InsuranceApplications, VehicleInsuranceApplications,
    ///     LifeInsuranceApplications, Vehicles and Beneficiaries tables, including the reference data of insurers and products.
    /// </summary>
    /// <param name="builder">The model builder.</param>
    public static void ApplyInsuranceOfferingAndApplicationsConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Insurer>(insurer =>
        {
            insurer.HasKey(i => i.Id);
            insurer.Property(i => i.Id).IsRequired().ValueGeneratedOnAdd();
            insurer.Property(i => i.ExternalReference).IsRequired().HasMaxLength(50);
            insurer.HasIndex(i => i.ExternalReference).IsUnique();
            insurer.Property(i => i.Name).IsRequired().HasMaxLength(150);
            insurer.Property(i => i.Active).IsRequired();
            insurer.HasData(
                new { Id = 1L, ExternalReference = "INS-ANDINA", Name = "Protección Andina", Active = true, CreatedAt = (DateTimeOffset?)SeedDate, UpdatedAt = (DateTimeOffset?)SeedDate },
                new { Id = 2L, ExternalReference = "INS-HORIZONTE", Name = "Horizonte Seguros", Active = true, CreatedAt = (DateTimeOffset?)SeedDate, UpdatedAt = (DateTimeOffset?)SeedDate },
                new { Id = 3L, ExternalReference = "INS-CONFIANZA", Name = "Confianza Vida", Active = true, CreatedAt = (DateTimeOffset?)SeedDate, UpdatedAt = (DateTimeOffset?)SeedDate });
        });

        builder.Entity<InsuranceProduct>(product =>
        {
            product.HasKey(p => p.Id);
            product.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            product.Property(p => p.InsurerId).IsRequired();
            product.HasOne<Insurer>().WithMany().HasForeignKey(p => p.InsurerId)
                .OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_insurance_products_insurer");
            product.Property(p => p.Name).IsRequired().HasMaxLength(150);
            product.Property(p => p.InsuranceType).IsRequired().HasConversion<string>().HasMaxLength(20);
            product.Property(p => p.CoverageAmount).IsRequired().HasPrecision(12, 2);
            product.Property(p => p.PremiumAmount).IsRequired().HasPrecision(12, 2);
            product.Property(p => p.Active).IsRequired();
            product.HasData(
                Product(1, 1, "Auto Esencial", InsuranceType.VEHICLE, 35000, 650),
                Product(2, 1, "Auto Integral", InsuranceType.VEHICLE, 70000, 1100),
                Product(3, 2, "Vehículo Total", InsuranceType.VEHICLE, 100000, 1450),
                Product(4, 2, "Vida Familiar", InsuranceType.LIFE, 80000, 780),
                Product(5, 3, "Vida Protección", InsuranceType.LIFE, 120000, 1050),
                Product(6, 3, "Vida Plus", InsuranceType.LIFE, 200000, 1600));
        });

        builder.Entity<InsuranceApplication>(application =>
        {
            application.HasKey(a => a.Id);
            application.Property(a => a.Id).IsRequired().ValueGeneratedOnAdd();
            application.Property(a => a.UserId).IsRequired();
            application.Property(a => a.ProductId).IsRequired();
            application.HasOne<InsuranceProduct>().WithMany().HasForeignKey(a => a.ProductId)
                .OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_insurance_applications_product");
            application.Property(a => a.RequestedCoverage).IsRequired().HasPrecision(12, 2);
            application.Property(a => a.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        });

        builder.Entity<VehicleInsuranceApplication>(details =>
        {
            details.HasKey(d => d.Id);
            details.Property(d => d.Id).IsRequired().ValueGeneratedOnAdd();
            details.HasIndex(d => d.ApplicationId).IsUnique();
            details.HasOne<InsuranceApplication>().WithOne().HasForeignKey<VehicleInsuranceApplication>(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade).HasConstraintName("fk_vehicle_insurance_applications_application");
            details.HasOne<Vehicle>().WithMany().HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_vehicle_insurance_applications_vehicle");
        });

        builder.Entity<LifeInsuranceApplication>(details =>
        {
            details.HasKey(d => d.Id);
            details.Property(d => d.Id).IsRequired().ValueGeneratedOnAdd();
            details.HasIndex(d => d.ApplicationId).IsUnique();
            details.HasOne<InsuranceApplication>().WithOne().HasForeignKey<LifeInsuranceApplication>(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade).HasConstraintName("fk_life_insurance_applications_application");
        });

        builder.Entity<Vehicle>(vehicle =>
        {
            vehicle.HasKey(v => v.Id);
            vehicle.Property(v => v.Id).IsRequired().ValueGeneratedOnAdd();
            vehicle.Property(v => v.UserId).IsRequired();
            vehicle.Property(v => v.LicensePlate).IsRequired().HasMaxLength(20);
            vehicle.HasIndex(v => v.LicensePlate).IsUnique();
            vehicle.Property(v => v.Brand).IsRequired().HasMaxLength(100);
            vehicle.Property(v => v.Model).IsRequired().HasMaxLength(100);
            vehicle.Property(v => v.ManufactureYear).IsRequired();
            vehicle.Property(v => v.EstimatedValue).IsRequired().HasPrecision(12, 2);
        });

        builder.Entity<Beneficiary>(beneficiary =>
        {
            beneficiary.HasKey(b => b.Id);
            beneficiary.Property(b => b.Id).IsRequired().ValueGeneratedOnAdd();
            beneficiary.Property(b => b.PolicyId).IsRequired();
            beneficiary.Property(b => b.FullName).IsRequired().HasMaxLength(200);
            beneficiary.Property(b => b.DocumentType).IsRequired().HasMaxLength(30);
            beneficiary.Property(b => b.DocumentNumber).IsRequired().HasMaxLength(30);
            beneficiary.Property(b => b.Relationship).IsRequired().HasMaxLength(50);
            beneficiary.Property(b => b.BirthDate)
                .HasConversion(new ValueConverter<DateOnly, DateTime>(
                    date => date.ToDateTime(TimeOnly.MinValue), value => DateOnly.FromDateTime(value)))
                .HasColumnType("date");
            beneficiary.Property(b => b.ParticipationPercentage).IsRequired().HasPrecision(5, 2);
        });
    }

    private static object Product(long id, long insurerId, string name, InsuranceType type, decimal coverage, decimal premium) =>
        new
        {
            Id = id, InsurerId = insurerId, Name = name, InsuranceType = type, CoverageAmount = coverage,
            PremiumAmount = premium, Active = true, CreatedAt = (DateTimeOffset?)SeedDate, UpdatedAt = (DateTimeOffset?)SeedDate
        };
}
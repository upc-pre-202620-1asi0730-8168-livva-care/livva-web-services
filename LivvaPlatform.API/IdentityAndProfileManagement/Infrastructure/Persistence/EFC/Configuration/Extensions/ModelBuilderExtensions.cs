using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Infrastructure.Persistence.EFC.Configuration.Extensions;

/// <summary>
///     Entity configuration of the Identity and Profile Management bounded context.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    ///     Applies the mapping of the Users and UserProfiles tables.
    /// </summary>
    /// <param name="builder">The model builder.</param>
    public static void ApplyIdentityAndProfileManagementConfiguration(this ModelBuilder builder)
    {
        builder.Entity<User>(user =>
        {
            user.HasKey(u => u.Id);
            user.Property(u => u.Id).IsRequired().ValueGeneratedOnAdd();
            user.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            user.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            user.Property(u => u.Email).IsRequired().HasMaxLength(255);
            user.HasIndex(u => u.Email).IsUnique();
            user.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
            user.Property(u => u.Phone).HasMaxLength(30);
            user.Property(u => u.Role).IsRequired().HasConversion<string>().HasMaxLength(30);
            user.Property(u => u.Active).IsRequired().HasDefaultValue(true);
            user.Property(u => u.CreatedAt).IsRequired();
            user.Property(u => u.UpdatedAt).IsRequired();
        });

        builder.Entity<UserProfile>(profile =>
        {
            profile.HasKey(p => p.Id);
            profile.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            profile.Property(p => p.UserId).IsRequired();
            profile.HasIndex(p => p.UserId).IsUnique();
            profile.HasOne<User>().WithOne().HasForeignKey<UserProfile>(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
            profile.Property(p => p.DocumentType).IsRequired().HasMaxLength(30);
            profile.Property(p => p.DocumentNumber).IsRequired().HasMaxLength(30);
            profile.Property(p => p.BirthDate)
                .HasConversion(new ValueConverter<DateOnly, DateTime>(
                    date => date.ToDateTime(TimeOnly.MinValue), value => DateOnly.FromDateTime(value)))
                .HasColumnType("date");
            profile.Property(p => p.Address).HasMaxLength(255);
            profile.Property(p => p.District).HasMaxLength(100);
            profile.Property(p => p.Province).HasMaxLength(100);
            profile.Property(p => p.Department).HasMaxLength(100);
            profile.Property(p => p.Country).HasMaxLength(100);
            profile.Property(p => p.CreatedAt).IsRequired();
            profile.Property(p => p.UpdatedAt).IsRequired();
        });
    }
}
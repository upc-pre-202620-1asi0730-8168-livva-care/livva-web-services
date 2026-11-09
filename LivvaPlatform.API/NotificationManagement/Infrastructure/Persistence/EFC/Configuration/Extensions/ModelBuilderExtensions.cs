using LivvaPlatform.API.NotificationManagement.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LivvaPlatform.API.NotificationManagement.Infrastructure.Persistence.EFC.Configuration.Extensions;

/// <summary>
///     Entity configuration of the Notification Management bounded context.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    ///     Applies the mapping of the Notifications table.
    /// </summary>
    /// <param name="builder">The model builder.</param>
    public static void ApplyNotificationManagementConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Notification>(notification =>
        {
            notification.HasKey(n => n.Id);
            notification.Property(n => n.Id).IsRequired().ValueGeneratedOnAdd();
            notification.Property(n => n.UserId).IsRequired();
            notification.HasIndex(n => n.UserId);
            notification.Property(n => n.Type).IsRequired().HasMaxLength(50);
            notification.Property(n => n.PolicyNumber).HasMaxLength(50);
            notification.Property(n => n.ExpirationDate)
                .HasConversion(new ValueConverter<DateOnly, DateTime>(
                    date => date.ToDateTime(TimeOnly.MinValue), value => DateOnly.FromDateTime(value)))
                .HasColumnType("date");
            notification.Property(n => n.IsRead).IsRequired().HasDefaultValue(false);
        });
    }
}
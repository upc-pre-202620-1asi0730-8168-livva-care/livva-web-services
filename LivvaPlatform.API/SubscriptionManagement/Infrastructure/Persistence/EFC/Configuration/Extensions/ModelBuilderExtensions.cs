using System.Text.Json;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LivvaPlatform.API.SubscriptionManagement.Infrastructure.Persistence.EFC.Configuration.Extensions;

/// <summary>
///     Entity configuration of the Subscription Management bounded context.
/// </summary>
public static class ModelBuilderExtensions
{
    private static readonly DateTimeOffset SeedDate = new(2026, 10, 5, 0, 0, 0, TimeSpan.Zero);

    /// <summary>
    ///     Applies the mapping of the SubscriptionPlans, UserSubscriptions and SubscriptionPayments tables, including
    ///     the plans of the Web Application.
    /// </summary>
    /// <param name="builder">The model builder.</param>
    public static void ApplySubscriptionManagementConfiguration(this ModelBuilder builder)
    {
        builder.Entity<SubscriptionPlan>(plan =>
        {
            plan.HasKey(p => p.Id);
            plan.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            plan.Property(p => p.Name).IsRequired().HasMaxLength(100);
            plan.Property(p => p.Description).HasMaxLength(500);
            plan.Property(p => p.MonthlyPrice).IsRequired().HasPrecision(12, 2);
            plan.Property(p => p.AnnualPrice).IsRequired().HasPrecision(12, 2);
            plan.Property(p => p.Benefits)
                .HasConversion(
                    benefits => JsonSerializer.Serialize(benefits, JsonSerializerOptions.Default),
                    json => JsonSerializer.Deserialize<List<string>>(json, JsonSerializerOptions.Default) ?? new List<string>(),
                    new ValueComparer<List<string>>(
                        (left, right) => (left ?? new List<string>()).SequenceEqual(right ?? new List<string>()),
                        benefits => benefits.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
                        benefits => benefits.ToList()))
                .HasColumnType("json")
                .IsRequired();
            plan.Property(p => p.Active).IsRequired();
            plan.HasData(
                new { Id = 1L, Name = "basic", Description = (string?)"basicDescription", MonthlyPrice = 19m, AnnualPrice = 190m, Benefits = new List<string> { "basicFeature1", "basicFeature2", "basicFeature3", "basicFeature4" }, Active = true, CreatedAt = (DateTimeOffset?)SeedDate, UpdatedAt = (DateTimeOffset?)SeedDate },
                new { Id = 2L, Name = "pro", Description = (string?)"proDescription", MonthlyPrice = 39m, AnnualPrice = 390m, Benefits = new List<string> { "proFeature1", "proFeature2", "proFeature3", "proFeature4" }, Active = true, CreatedAt = (DateTimeOffset?)SeedDate, UpdatedAt = (DateTimeOffset?)SeedDate });
        });

        builder.Entity<UserSubscription>(subscription =>
        {
            subscription.HasKey(s => s.Id);
            subscription.Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
            subscription.Property(s => s.UserId).IsRequired();
            subscription.HasOne<SubscriptionPlan>().WithMany().HasForeignKey(s => s.PlanId).OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_user_subscriptions_plan");
            subscription.Property(s => s.BillingCycle).IsRequired().HasConversion<string>().HasMaxLength(20);
            subscription.Property(s => s.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
            subscription.Property(s => s.StartDate).IsRequired();
            subscription.Property(s => s.NextBillingDate).IsRequired();
        });

        builder.Entity<SubscriptionPayment>(payment =>
        {
            payment.HasKey(p => p.Id);
            payment.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            payment.HasOne<UserSubscription>().WithMany().HasForeignKey(p => p.UserSubscriptionId).OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_subscription_payments_subscription");
            payment.Property(p => p.UserId).IsRequired();
            payment.Property(p => p.PlanId).IsRequired();
            payment.Property(p => p.Provider).IsRequired().HasMaxLength(50);
            payment.Property(p => p.ProviderPaymentId).HasMaxLength(100);
            payment.Property(p => p.Amount).IsRequired().HasPrecision(12, 2);
            payment.Property(p => p.Currency).IsRequired().HasMaxLength(3);
            payment.Property(p => p.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        });
    }
}
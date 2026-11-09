namespace LivvaPlatform.API.SubscriptionManagement.Interfaces.REST.Resources;

/// <summary>A subscription plan returned by the API.</summary>
public record SubscriptionPlanResource(long Id, string Name, string? Description, decimal MonthlyPrice, decimal AnnualPrice, IReadOnlyList<string> Benefits, bool Active, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt);

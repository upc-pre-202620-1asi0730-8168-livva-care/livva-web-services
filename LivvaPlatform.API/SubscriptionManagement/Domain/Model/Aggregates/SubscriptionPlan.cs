using LivvaPlatform.API.SubscriptionManagement.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;

/// <summary>
///     Represents a Livva subscription plan.
/// </summary>
/// <remarks>Plans are loaded as reference data. The name, description and benefits are i18n keys of the Web Application.</remarks>
public partial class SubscriptionPlan
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected SubscriptionPlan()
    {
        Name = string.Empty;
        Benefits = [];
    }

    /// <summary>Gets the identifier of the plan.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the name of the plan.</summary>
    public string Name { get; private set; }

    /// <summary>Gets the description of the plan.</summary>
    public string? Description { get; private set; }

    /// <summary>Gets the monthly price.</summary>
    public decimal MonthlyPrice { get; private set; }

    /// <summary>Gets the annual price.</summary>
    public decimal AnnualPrice { get; private set; }

    /// <summary>Gets the benefits of the plan.</summary>
    public List<string> Benefits { get; private set; }

    /// <summary>Gets a value indicating whether the plan is active.</summary>
    public bool Active { get; private set; }

    /// <summary>
    ///     Gets the price of the plan for a billing cycle.
    /// </summary>
    /// <param name="billingCycle">The billing cycle.</param>
    /// <returns>The price.</returns>
    public decimal PriceFor(BillingCycle billingCycle) =>
        billingCycle == BillingCycle.ANNUAL ? AnnualPrice : MonthlyPrice;

    /// <summary>
    ///     Updates the prices of the plan.
    /// </summary>
    /// <param name="monthlyPrice">The new monthly price.</param>
    /// <param name="annualPrice">The new annual price.</param>
    public void UpdatePrices(decimal monthlyPrice, decimal annualPrice)
    {
        if (monthlyPrice <= 0 || annualPrice <= 0) throw new ArgumentException("The prices must be positive.");
        MonthlyPrice = monthlyPrice;
        AnnualPrice = annualPrice;
    }

    /// <summary>Activates the plan.</summary>
    public void Activate() => Active = true;

    /// <summary>Deactivates the plan.</summary>
    public void Deactivate() => Active = false;
}
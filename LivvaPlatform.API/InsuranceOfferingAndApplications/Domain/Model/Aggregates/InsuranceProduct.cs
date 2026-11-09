using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;

/// <summary>
///     Represents an insurance product offered by an insurer.
/// </summary>
/// <remarks>Insurance products are loaded as reference data.</remarks>
public partial class InsuranceProduct
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected InsuranceProduct()
    {
        Name = string.Empty;
    }

    /// <summary>Gets the identifier of the product.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the identifier of the insurer that offers the product.</summary>
    public long InsurerId { get; private set; }

    /// <summary>Gets the name of the product.</summary>
    public string Name { get; private set; }

    /// <summary>Gets the type of insurance.</summary>
    public InsuranceType InsuranceType { get; private set; }

    /// <summary>Gets the coverage amount.</summary>
    public decimal CoverageAmount { get; private set; }

    /// <summary>Gets the premium amount.</summary>
    public decimal PremiumAmount { get; private set; }

    /// <summary>Gets a value indicating whether the product is active.</summary>
    public bool Active { get; private set; }

    /// <summary>Activates the product.</summary>
    public void Activate() => Active = true;

    /// <summary>Deactivates the product.</summary>
    public void Deactivate() => Active = false;
}
namespace LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;

/// <summary>
///     Represents a coverage included in a policy.
/// </summary>
public partial class PolicyCoverage
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected PolicyCoverage()
    {
        Name = string.Empty;
    }

    /// <summary>Gets the identifier of the coverage.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the identifier of the policy.</summary>
    public long PolicyId { get; private set; }

    /// <summary>Gets the name of the coverage.</summary>
    public string Name { get; private set; }

    /// <summary>Gets the description of the coverage.</summary>
    public string? Description { get; private set; }

    /// <summary>Gets the coverage limit.</summary>
    public decimal CoverageLimit { get; private set; }
}
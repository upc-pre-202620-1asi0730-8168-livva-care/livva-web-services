using LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.PolicyManagement.Interfaces.REST.Resources;
using LivvaPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.PolicyManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles resources from the aggregates of the Policy Management bounded context.
/// </summary>
public static class ResourceAssembler
{
    /// <summary>Converts a policy into a resource.</summary>
    /// <param name="e">The policy.</param>
    /// <returns>The policy resource.</returns>
    public static PolicyResource ToResource(Policy e) =>
        new(e.Id, e.UserId, e.ApplicationId, e.PolicyNumber, EnumValueConverter.ToValue(e.InsuranceType), e.StartDate,
            e.ExpirationDate, e.PremiumAmount, e.CoverageAmount, EnumValueConverter.ToValue(e.Status), e.CreatedAt, e.UpdatedAt);

    /// <summary>Converts a coverage into a resource.</summary>
    /// <param name="e">The coverage.</param>
    /// <returns>The coverage resource.</returns>
    public static PolicyCoverageResource ToResource(PolicyCoverage e) =>
        new(e.Id, e.PolicyId, e.Name, e.Description, e.CoverageLimit, e.CreatedAt, e.UpdatedAt);

    /// <summary>Converts a document into a resource.</summary>
    /// <param name="e">The document.</param>
    /// <returns>The document resource.</returns>
    public static PolicyDocumentResource ToResource(PolicyDocument e) =>
        new(e.Id, e.PolicyId, e.DocumentType, e.Name, e.FileUrl, e.CreatedAt, e.UpdatedAt);

    /// <summary>Converts a renewal into a resource.</summary>
    /// <param name="e">The renewal.</param>
    /// <returns>The renewal resource.</returns>
    public static RenewalResource ToResource(Renewal e) =>
        new(e.Id, e.PolicyId, e.RequestedAt, e.ProposedExpirationDate, e.ProposedPremium,
            EnumValueConverter.ToValue(e.Status), e.ResolvedAt, e.CreatedAt, e.UpdatedAt);
}
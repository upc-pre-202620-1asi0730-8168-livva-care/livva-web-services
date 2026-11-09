namespace LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;

/// <summary>
///     Represents a document associated with a policy.
/// </summary>
public partial class PolicyDocument
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected PolicyDocument()
    {
        DocumentType = string.Empty;
        Name = string.Empty;
    }

    /// <summary>Gets the identifier of the document.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the identifier of the policy.</summary>
    public long PolicyId { get; private set; }

    /// <summary>Gets the type of document (for example, policy).</summary>
    public string DocumentType { get; private set; }

    /// <summary>Gets the name of the document.</summary>
    public string Name { get; private set; }

    /// <summary>Gets the URL of the file.</summary>
    public string? FileUrl { get; private set; }
}
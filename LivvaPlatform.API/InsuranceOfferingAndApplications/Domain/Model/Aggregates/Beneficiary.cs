using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;

/// <summary>
///     Represents a beneficiary designated in a life insurance policy.
/// </summary>
public partial class Beneficiary
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Beneficiary()
    {
        FullName = string.Empty;
        DocumentType = string.Empty;
        DocumentNumber = string.Empty;
        Relationship = string.Empty;
    }

    /// <summary>
    ///     Registers a beneficiary from a creation command.
    /// </summary>
    /// <param name="command">The create beneficiary command.</param>
    /// <exception cref="ArgumentException">Thrown when a value is not valid.</exception>
    public Beneficiary(CreateBeneficiaryCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.PolicyId <= 0) throw new ArgumentException("The policy identifier must be positive.", nameof(command));
        PolicyId = command.PolicyId;
        FullName = Required(command.FullName);
        DocumentType = Required(command.DocumentType);
        DocumentNumber = Required(command.DocumentNumber);
        Relationship = Required(command.Relationship);
        BirthDate = command.BirthDate;
        ParticipationPercentage = ValidPercentage(command.ParticipationPercentage);
    }

    /// <summary>Gets the identifier of the beneficiary.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the identifier of the life insurance policy.</summary>
    public long PolicyId { get; private set; }

    /// <summary>Gets the full name.</summary>
    public string FullName { get; private set; }

    /// <summary>Gets the identity document type.</summary>
    public string DocumentType { get; private set; }

    /// <summary>Gets the identity document number.</summary>
    public string DocumentNumber { get; private set; }

    /// <summary>Gets the relationship with the policyholder (for example, spouse or child).</summary>
    public string Relationship { get; private set; }

    /// <summary>Gets the birth date.</summary>
    public DateOnly? BirthDate { get; private set; }

    /// <summary>Gets the participation percentage in the benefit (0 to 100).</summary>
    public decimal ParticipationPercentage { get; private set; }

    /// <summary>
    ///     Updates the beneficiary information. Null values keep the current value.
    /// </summary>
    /// <param name="command">The update beneficiary command.</param>
    public void UpdateInformation(UpdateBeneficiaryCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.FullName is not null) FullName = Required(command.FullName);
        if (command.DocumentType is not null) DocumentType = Required(command.DocumentType);
        if (command.DocumentNumber is not null) DocumentNumber = Required(command.DocumentNumber);
        if (command.Relationship is not null) Relationship = Required(command.Relationship);
        if (command.BirthDate is not null) BirthDate = command.BirthDate;
        if (command.ParticipationPercentage is not null)
            ParticipationPercentage = ValidPercentage(command.ParticipationPercentage.Value);
    }

    private static string Required(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim();
    }

    private static decimal ValidPercentage(decimal value) =>
        value is > 0 and <= 100
            ? value
            : throw new ArgumentException("The participation percentage must be between 0 and 100.", nameof(value));
}
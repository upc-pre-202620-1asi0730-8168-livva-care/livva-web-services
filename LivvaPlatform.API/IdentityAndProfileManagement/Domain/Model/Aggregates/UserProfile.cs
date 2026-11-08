using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Commands;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;

/// <summary>
///     Represents the personal profile of a user.
/// </summary>
/// <remarks>
///     Each user owns at most one profile. Audit timestamps are managed by the persistence layer.
/// </remarks>
public partial class UserProfile
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected UserProfile()
    {
        DocumentType = string.Empty;
        DocumentNumber = string.Empty;
    }

    /// <summary>
    ///     Creates a new profile from a creation command.
    /// </summary>
    /// <param name="command">The create user profile command.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when a required value is empty.</exception>
    public UserProfile(CreateUserProfileCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.DocumentType);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.DocumentNumber);
        if (command.UserId <= 0) throw new ArgumentException("The user identifier must be positive.", nameof(command));

        UserId = command.UserId;
        DocumentType = command.DocumentType.Trim();
        DocumentNumber = command.DocumentNumber.Trim();
        BirthDate = command.BirthDate;
        Address = Clean(command.Address);
        District = Clean(command.District);
        Province = Clean(command.Province);
        Department = Clean(command.Department);
        Country = Clean(command.Country);
    }

    /// <summary>
    ///     Gets the server-generated identifier of the profile.
    /// </summary>
    public long Id { get; private set; }

    /// <summary>
    ///     Gets the identifier of the user that owns the profile.
    /// </summary>
    public long UserId { get; private set; }

    /// <summary>
    ///     Gets the identity document type.
    /// </summary>
    public string DocumentType { get; private set; }

    /// <summary>
    ///     Gets the identity document number.
    /// </summary>
    public string DocumentNumber { get; private set; }

    /// <summary>
    ///     Gets the birth date.
    /// </summary>
    public DateOnly? BirthDate { get; private set; }

    /// <summary>
    ///     Gets the address.
    /// </summary>
    public string? Address { get; private set; }

    /// <summary>
    ///     Gets the district.
    /// </summary>
    public string? District { get; private set; }

    /// <summary>
    ///     Gets the province.
    /// </summary>
    public string? Province { get; private set; }

    /// <summary>
    ///     Gets the department.
    /// </summary>
    public string? Department { get; private set; }

    /// <summary>
    ///     Gets the country.
    /// </summary>
    public string? Country { get; private set; }

    /// <summary>
    ///     Updates the identity document and birth date. Null values keep the current value.
    /// </summary>
    /// <param name="command">The update command.</param>
    public void UpdatePersonalData(UpdateUserProfileCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.DocumentType is not null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(command.DocumentType);
            DocumentType = command.DocumentType.Trim();
        }

        if (command.DocumentNumber is not null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(command.DocumentNumber);
            DocumentNumber = command.DocumentNumber.Trim();
        }

        if (command.BirthDate is not null) BirthDate = command.BirthDate;
    }

    /// <summary>
    ///     Updates the address data. Null values keep the current value.
    /// </summary>
    /// <param name="command">The update command.</param>
    public void UpdateAddress(UpdateUserProfileCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.Address is not null) Address = Clean(command.Address);
        if (command.District is not null) District = Clean(command.District);
        if (command.Province is not null) Province = Clean(command.Province);
        if (command.Department is not null) Department = Clean(command.Department);
        if (command.Country is not null) Country = Clean(command.Country);
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
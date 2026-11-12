using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Commands;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;

/// <summary>
///     Represents a registered user of the Livva platform.
/// </summary>
/// <remarks>
///     This aggregate root stores the account data of a user. The email is unique and is stored in lower case.
///     The password is never stored in plain text; only its BCrypt hash is kept.
///     Audit timestamps (CreatedAt, UpdatedAt) are managed by the persistence layer.
/// </remarks>
public partial class User
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected User()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        PasswordHash = string.Empty;
    }

    /// <summary>
    ///     Creates a new customer account from a sign-up command.
    /// </summary>
    /// <param name="command">The sign-up command.</param>
    /// <param name="passwordHash">The hash of the password provided in the command.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when a required value is empty.</exception>
    public User(SignUpCommand command, string passwordHash)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.FirstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.LastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        FirstName = command.FirstName.Trim();
        LastName = command.LastName.Trim();
        Email = NormalizeEmail(command.Email);
        PasswordHash = passwordHash;
        Phone = string.IsNullOrWhiteSpace(command.Phone) ? null : command.Phone.Trim();
        Role = UserRole.CUSTOMER;
        Active = true;
    }

    /// <summary>
    ///     Gets the server-generated identifier of the user.
    /// </summary>
    public long Id { get; private set; }

    /// <summary>
    ///     Gets the first name of the user.
    /// </summary>
    public string FirstName { get; private set; }

    /// <summary>
    ///     Gets the last name of the user.
    /// </summary>
    public string LastName { get; private set; }

    /// <summary>
    ///     Gets the email address of the user, stored in lower case.
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    ///     Gets the BCrypt hash of the password of the user.
    /// </summary>
    public string PasswordHash { get; private set; }

    /// <summary>
    ///     Gets the phone number of the user.
    /// </summary>
    public string? Phone { get; private set; }

    /// <summary>
    ///     Gets the role of the user.
    /// </summary>
    public UserRole Role { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether the account is active.
    /// </summary>
    public bool Active { get; private set; }

    /// <summary>
    ///     Normalizes an email address by trimming it and converting it to lower case.
    /// </summary>
    /// <param name="email">The email address to normalize.</param>
    /// <returns>The normalized email address.</returns>
    public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    /// <summary>
    ///     Updates the personal data of the user. Null values keep the current value.
    /// </summary>
    /// <param name="command">The update command.</param>
    /// <exception cref="ArgumentException">Thrown when a provided name is empty.</exception>
    public void UpdateProfile(UpdateUserCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.FirstName is not null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(command.FirstName);
            FirstName = command.FirstName.Trim();
        }

        if (command.LastName is not null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(command.LastName);
            LastName = command.LastName.Trim();
        }

        if (command.Phone is not null) Phone = string.IsNullOrWhiteSpace(command.Phone) ? null : command.Phone.Trim();
    }

    /// <summary>
    ///     Deactivates the account so that it can no longer sign in.
    /// </summary>
    public void Deactivate() => Active = false;
}
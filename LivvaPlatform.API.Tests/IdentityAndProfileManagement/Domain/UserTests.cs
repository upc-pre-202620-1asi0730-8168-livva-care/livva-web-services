using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Commands;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.Tests.IdentityAndProfileManagement.Domain;

/// <summary>
///     Unit tests for the <see cref="User" /> aggregate.
/// </summary>
public class UserTests
{
    private static SignUpCommand ValidCommand() =>
        new(" Ana ", " Torres ", "  Ana.Torres@Livva.PE ", "secret123", " +51 999 999 999 ");

    [Fact]
    public void Constructor_ShouldCreateActiveCustomerWithNormalizedData()
    {
        var user = new User(ValidCommand(), "hash");

        Assert.Equal("Ana", user.FirstName);
        Assert.Equal("Torres", user.LastName);
        Assert.Equal("ana.torres@livva.pe", user.Email);
        Assert.Equal("+51 999 999 999", user.Phone);
        Assert.Equal("hash", user.PasswordHash);
        Assert.Equal(UserRole.CUSTOMER, user.Role);
        Assert.True(user.Active);
    }

    [Fact]
    public void Constructor_WithEmptyFirstName_ShouldThrow()
    {
        var command = ValidCommand() with { FirstName = " " };

        Assert.ThrowsAny<ArgumentException>(() => new User(command, "hash"));
    }

    [Fact]
    public void UpdateProfile_ShouldOnlyChangeProvidedFields()
    {
        var user = new User(ValidCommand(), "hash");

        user.UpdateProfile(new UpdateUserCommand(1, null, "Rivera", null));

        Assert.Equal("Ana", user.FirstName);
        Assert.Equal("Rivera", user.LastName);
        Assert.Equal("+51 999 999 999", user.Phone);
    }

    [Fact]
    public void Deactivate_ShouldMarkUserAsInactive()
    {
        var user = new User(ValidCommand(), "hash");

        user.Deactivate();

        Assert.False(user.Active);
    }
}
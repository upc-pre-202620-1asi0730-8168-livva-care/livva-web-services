using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Commands;

namespace LivvaPlatform.API.Tests.IdentityAndProfileManagement.Domain;

/// <summary>
///     Unit tests for the <see cref="UserProfile" /> aggregate.
/// </summary>
public class UserProfileTests
{
    private static CreateUserProfileCommand ValidCommand() =>
        new(1, "DNI", "12345678", new DateOnly(2002, 5, 15), "Av. Primavera 123", "Miraflores", "Lima", "Lima",
            "Peru");

    [Fact]
    public void Constructor_ShouldCopyAllFields()
    {
        var profile = new UserProfile(ValidCommand());

        Assert.Equal(1, profile.UserId);
        Assert.Equal("DNI", profile.DocumentType);
        Assert.Equal("12345678", profile.DocumentNumber);
        Assert.Equal(new DateOnly(2002, 5, 15), profile.BirthDate);
        Assert.Equal("Miraflores", profile.District);
        Assert.Equal("Peru", profile.Country);
    }

    [Fact]
    public void Constructor_WithInvalidUserId_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => new UserProfile(ValidCommand() with { UserId = 0 }));
    }

    [Fact]
    public void Update_ShouldKeepOmittedFields()
    {
        var profile = new UserProfile(ValidCommand());
        var command = new UpdateUserProfileCommand(1, null, null, null, null, "San Isidro", null, null, null);

        profile.UpdatePersonalData(command);
        profile.UpdateAddress(command);

        Assert.Equal("12345678", profile.DocumentNumber);
        Assert.Equal("San Isidro", profile.District);
        Assert.Equal("Av. Primavera 123", profile.Address);
    }
}
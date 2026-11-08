using LivvaPlatform.API.IdentityAndProfileManagement.Infrastructure.Hashing.BCrypt.Services;

namespace LivvaPlatform.API.Tests.IdentityAndProfileManagement.Infrastructure;

/// <summary>
///     Unit tests for the BCrypt <see cref="HashingService" />.
/// </summary>
public class HashingServiceTests
{
    private readonly HashingService _service = new();

    [Fact]
    public void HashPassword_ShouldNotReturnPlainText_AndShouldVerify()
    {
        var hash = _service.HashPassword("secret123");

        Assert.NotEqual("secret123", hash);
        Assert.True(_service.VerifyPassword("secret123", hash));
        Assert.False(_service.VerifyPassword("other", hash));
    }

    [Fact]
    public void VerifyPassword_WithInvalidHash_ShouldReturnFalse()
    {
        Assert.False(_service.VerifyPassword("secret123", "not-a-bcrypt-hash"));
    }
}
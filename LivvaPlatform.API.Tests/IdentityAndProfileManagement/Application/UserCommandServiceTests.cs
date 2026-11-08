using LivvaPlatform.API.IdentityAndProfileManagement.Application.Errors;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.Internal.CommandServices;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.Internal.OutboundServices;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.Model;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Commands;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Repositories;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace LivvaPlatform.API.Tests.IdentityAndProfileManagement.Application;

/// <summary>
///     Unit tests for <see cref="UserCommandService" />.
/// </summary>
public class UserCommandServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IHashingService> _hashingService = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private UserCommandService CreateService() => new(_userRepository.Object, _hashingService.Object,
        _tokenService.Object, _unitOfWork.Object, NullLogger<UserCommandService>.Instance);

    private static SignUpCommand SignUp() => new("Ana", "Torres", "ana@livva.pe", "secret123", null);

    [Fact]
    public async Task SignUp_WithNewEmail_ShouldHashPasswordAndPersistUser()
    {
        _hashingService.Setup(h => h.HashPassword("secret123")).Returns("hashed");

        var result = await CreateService().Handle(SignUp());

        var success = Assert.IsType<Result<User, SignUpError>.Success>(result);
        Assert.Equal("hashed", success.Value.PasswordHash);
        _userRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SignUp_WithRegisteredEmail_ShouldFailWithoutPersisting()
    {
        _userRepository.Setup(r => r.ExistsByEmailAsync("ana@livva.pe", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await CreateService().Handle(SignUp());

        var failure = Assert.IsType<Result<User, SignUpError>.Failure>(result);
        Assert.Equal(SignUpError.EmailAlreadyRegistered, failure.Error);
        _userRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SignIn_WithValidCredentials_ShouldReturnToken()
    {
        var user = new User(SignUp(), "hashed");
        _userRepository.Setup(r => r.FindByEmailAsync("ana@livva.pe", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _hashingService.Setup(h => h.VerifyPassword("secret123", "hashed")).Returns(true);
        _tokenService.Setup(t => t.GenerateToken(user)).Returns("token");

        var result = await CreateService().Handle(new SignInCommand("ANA@livva.pe", "secret123"));

        var success = Assert.IsType<Result<AuthenticatedUser, SignInError>.Success>(result);
        Assert.Equal("token", success.Value.Token);
    }

    [Fact]
    public async Task SignIn_WithWrongPassword_ShouldFail()
    {
        var user = new User(SignUp(), "hashed");
        _userRepository.Setup(r => r.FindByEmailAsync("ana@livva.pe", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _hashingService.Setup(h => h.VerifyPassword("wrong", "hashed")).Returns(false);

        var result = await CreateService().Handle(new SignInCommand("ana@livva.pe", "wrong"));

        var failure = Assert.IsType<Result<AuthenticatedUser, SignInError>.Failure>(result);
        Assert.Equal(SignInError.InvalidCredentials, failure.Error);
        _tokenService.Verify(t => t.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUser_WhenUserDoesNotExist_ShouldFail()
    {
        var result = await CreateService().Handle(new UpdateUserCommand(99, "Ana", null, null));

        var failure = Assert.IsType<Result<User, UpdateUserError>.Failure>(result);
        Assert.Equal(UpdateUserError.UserNotFound, failure.Error);
    }
}
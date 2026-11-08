using LivvaPlatform.API.Shared.Infrastructure.Interfaces.ASP.Configuration.Extensions;
using PersistenceExtensions = LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions.StringExtensions;

namespace LivvaPlatform.API.Tests.Shared.Infrastructure;

/// <summary>
///     Unit tests for the naming convention string extensions.
/// </summary>
public class StringExtensionsTests
{
    [Theory]
    [InlineData("OrderItems", "order-items")]
    [InlineData("Profiles", "profiles")]
    public void ToKebabCase_ShouldConvertPascalCase(string input, string expected)
    {
        Assert.Equal(expected, input.ToKebabCase());
    }

    [Theory]
    [InlineData("OrderItem", "order_item")]
    [InlineData("CreatedAt", "created_at")]
    public void ToSnakeCase_ShouldConvertPascalCase(string input, string expected)
    {
        Assert.Equal(expected, PersistenceExtensions.ToSnakeCase(input));
    }

    [Fact]
    public void ToPlural_ShouldPluralizeTableNames()
    {
        Assert.Equal("profiles", PersistenceExtensions.ToPlural("profile"));
    }
}
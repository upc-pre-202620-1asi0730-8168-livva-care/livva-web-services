using System.Net;

namespace LivvaPlatform.API.IntegrationTests;

/// <summary>
///     Integration tests for the API documentation endpoints.
/// </summary>
public class OpenApiTests(LivvaWebApplicationFactory factory) : IClassFixture<LivvaWebApplicationFactory>
{
    [Theory]
    [InlineData("/openapi/v1.json")]
    [InlineData("/scalar")]
    [InlineData("/swagger/index.html")]
    public async Task DocumentationEndpoints_ShouldReturnOk(string url)
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task OpenApiDocument_ShouldDescribeLivvaPlatformApi()
    {
        var client = factory.CreateClient();

        var content = await client.GetStringAsync("/openapi/v1.json");

        Assert.Contains("LivvaPlatform.API", content);
    }
}
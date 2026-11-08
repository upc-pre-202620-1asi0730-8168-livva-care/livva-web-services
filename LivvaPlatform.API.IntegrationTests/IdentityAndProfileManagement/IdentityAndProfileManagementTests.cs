using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace LivvaPlatform.API.IntegrationTests.IdentityAndProfileManagement;

/// <summary>
///     Integration tests for the authentication, users and user profiles endpoints.
/// </summary>
public class IdentityAndProfileManagementTests(LivvaWebApplicationFactory factory)
    : IClassFixture<LivvaWebApplicationFactory>
{
    private static string NewEmail() => $"user{Guid.NewGuid():N}@livva.pe";

    private async Task<(long Id, string Token)> SignUpAndSignInAsync(HttpClient client, string email)
    {
        var signUp = await client.PostAsJsonAsync("/api/v1/authentication/sign-up",
            new { firstName = "Ana", lastName = "Torres", email, password = "secret123", phone = "+51 999 999 999" });
        Assert.Equal(HttpStatusCode.Created, signUp.StatusCode);

        var signIn = await client.PostAsJsonAsync("/api/v1/authentication/sign-in",
            new { email, password = "secret123" });
        Assert.Equal(HttpStatusCode.OK, signIn.StatusCode);
        var body = await signIn.Content.ReadFromJsonAsync<JsonElement>();
        return (body.GetProperty("id").GetInt64(), body.GetProperty("token").GetString()!);
    }

    [Fact]
    public async Task SignUp_ShouldReturnCreatedUserWithoutPasswordHash()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/authentication/sign-up",
            new { firstName = "Ana", lastName = "Torres", email = NewEmail(), password = "secret123" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("passwordHash", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("\"role\":\"CUSTOMER\"", content);
    }

    [Fact]
    public async Task SignUp_WithDuplicateEmail_ShouldReturnConflict()
    {
        var client = factory.CreateClient();
        var email = NewEmail();
        await SignUpAndSignInAsync(client, email);

        var response = await client.PostAsJsonAsync("/api/v1/authentication/sign-up",
            new { firstName = "Ana", lastName = "Torres", email, password = "secret123" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task SignUp_WithInvalidData_ShouldReturnBadRequest()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/authentication/sign-up",
            new { firstName = "A", lastName = "Torres", email = "not-an-email", password = "123" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SignIn_WithWrongPassword_ShouldReturnUnauthorized()
    {
        var client = factory.CreateClient();
        var email = NewEmail();
        await SignUpAndSignInAsync(client, email);

        var response = await client.PostAsJsonAsync("/api/v1/authentication/sign-in",
            new { email, password = "wrong-password" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Users_WithoutToken_ShouldReturnUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/users");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Users_FilterByEmailAndPatch_ShouldWork()
    {
        var client = factory.CreateClient();
        var email = NewEmail();
        var (id, token) = await SignUpAndSignInAsync(client, email);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var users = await client.GetFromJsonAsync<JsonElement>($"/api/v1/users?email={email}");
        Assert.Equal(1, users.GetArrayLength());
        Assert.False(users[0].TryGetProperty("passwordHash", out _));

        var patch = await client.PatchAsJsonAsync($"/api/v1/users/{id}", new { phone = "+51 911 111 111" });
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
        var updated = await patch.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("+51 911 111 111", updated.GetProperty("phone").GetString());
        Assert.Equal("Ana", updated.GetProperty("firstName").GetString());
    }

    [Fact]
    public async Task UserProfiles_CreateQueryAndPatch_ShouldWork()
    {
        var client = factory.CreateClient();
        var (userId, token) = await SignUpAndSignInAsync(client, NewEmail());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var create = await client.PostAsJsonAsync("/api/v1/user-profiles", new
        {
            userId, documentType = "DNI", documentNumber = "12345678", birthDate = "2002-05-15",
            address = "Av. Primavera 123", district = "Miraflores", province = "Lima", department = "Lima",
            country = "Peru"
        });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var profileId = (await create.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt64();

        var duplicate = await client.PostAsJsonAsync("/api/v1/user-profiles",
            new { userId, documentType = "DNI", documentNumber = "87654321" });
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);

        var byUser = await client.GetFromJsonAsync<JsonElement>($"/api/v1/user-profiles?userId={userId}");
        Assert.Equal(1, byUser.GetArrayLength());
        Assert.Equal("Miraflores", byUser[0].GetProperty("district").GetString());

        var patch = await client.PatchAsJsonAsync($"/api/v1/user-profiles/{profileId}",
            new { district = "San Isidro" });
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
        var updated = await patch.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("San Isidro", updated.GetProperty("district").GetString());
        Assert.Equal("12345678", updated.GetProperty("documentNumber").GetString());
    }

    [Fact]
    public async Task UserProfiles_ForUnknownUser_ShouldReturnNotFound()
    {
        var client = factory.CreateClient();
        var (_, token) = await SignUpAndSignInAsync(client, NewEmail());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/api/v1/user-profiles",
            new { userId = 999999, documentType = "DNI", documentNumber = "12345678" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
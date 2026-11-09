using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace LivvaPlatform.API.IntegrationTests.InsuranceOfferingAndApplications;

/// <summary>
///     Integration tests for the Insurance Offering and Applications endpoints.
/// </summary>
public class InsuranceOfferingAndApplicationsTests(LivvaWebApplicationFactory factory)
    : IClassFixture<LivvaWebApplicationFactory>
{
    private async Task<(HttpClient Client, long UserId)> AuthenticatedClientAsync()
    {
        var client = factory.CreateClient();
        var email = $"user{Guid.NewGuid():N}@livva.pe";
        await client.PostAsJsonAsync("/api/v1/authentication/sign-up",
            new { firstName = "Ana", lastName = "Torres", email, password = "secret123" });
        var signIn = await client.PostAsJsonAsync("/api/v1/authentication/sign-in", new { email, password = "secret123" });
        var body = await signIn.Content.ReadFromJsonAsync<JsonElement>();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", body.GetProperty("token").GetString());
        return (client, body.GetProperty("id").GetInt64());
    }

    private static string Plate() => $"T{Random.Shared.Next(10000, 99999)}";

    [Fact]
    public async Task Offering_ShouldReturnSeededInsurersAndFilterProducts()
    {
        var (client, _) = await AuthenticatedClientAsync();

        var insurers = await client.GetFromJsonAsync<JsonElement>("/api/v1/insurers");
        Assert.Equal(3, insurers.GetArrayLength());

        var lifeProducts = await client.GetFromJsonAsync<JsonElement>("/api/v1/insurance-products?insuranceType=life");
        Assert.Equal(3, lifeProducts.GetArrayLength());
        Assert.All(lifeProducts.EnumerateArray(), p => Assert.Equal("life", p.GetProperty("insuranceType").GetString()));

        var invalid = await client.GetAsync("/api/v1/insurance-products?insuranceType=health");
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
    }

    [Fact]
    public async Task Offering_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await factory.CreateClient().GetAsync("/api/v1/insurers");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Vehicles_CreateFilterPatchAndDelete_ShouldWork()
    {
        var (client, userId) = await AuthenticatedClientAsync();
        var plate = Plate();

        var create = await client.PostAsJsonAsync("/api/v1/vehicles", new
        {
            userId, licensePlate = plate, brand = "Toyota", model = "Corolla", manufactureYear = 2022, estimatedValue = 65000
        });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var vehicleId = (await create.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt64();

        var duplicate = await client.PostAsJsonAsync("/api/v1/vehicles", new
        {
            userId, licensePlate = plate, brand = "Kia", model = "Rio", manufactureYear = 2020, estimatedValue = 40000
        });
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);

        var mine = await client.GetFromJsonAsync<JsonElement>($"/api/v1/vehicles?userId={userId}");
        Assert.Equal(1, mine.GetArrayLength());

        var patch = await client.PatchAsJsonAsync($"/api/v1/vehicles/{vehicleId}", new { estimatedValue = 60000 });
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
        Assert.Equal(60000, (await patch.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("estimatedValue").GetDecimal());

        var delete = await client.DeleteAsync($"/api/v1/vehicles/{vehicleId}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
    }

    [Fact]
    public async Task VehicleApplication_FullFlowAsUsedByWebApplication_ShouldWork()
    {
        var (client, userId) = await AuthenticatedClientAsync();
        var vehicle = await client.PostAsJsonAsync("/api/v1/vehicles", new
        {
            userId, licensePlate = Plate(), brand = "Toyota", model = "Corolla", manufactureYear = 2022, estimatedValue = 65000
        });
        var vehicleId = (await vehicle.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt64();

        var application = await client.PostAsJsonAsync("/api/v1/insurance-applications",
            new { userId, productId = 3, requestedCoverage = 100000, status = "approved" });
        Assert.Equal(HttpStatusCode.Created, application.StatusCode);
        var created = await application.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("pending", created.GetProperty("status").GetString());
        var applicationId = created.GetProperty("id").GetInt64();

        var details = await client.PostAsJsonAsync("/api/v1/vehicle-insurance-applications", new { applicationId, vehicleId });
        Assert.Equal(HttpStatusCode.Created, details.StatusCode);

        var lifeOnVehicleProduct = await client.PostAsJsonAsync("/api/v1/life-insurance-applications", new { applicationId });
        Assert.Equal(HttpStatusCode.Conflict, lifeOnVehicleProduct.StatusCode);

        var deleteVehicleInUse = await client.DeleteAsync($"/api/v1/vehicles/{vehicleId}");
        Assert.Equal(HttpStatusCode.Conflict, deleteVehicleInUse.StatusCode);

        var mine = await client.GetFromJsonAsync<JsonElement>($"/api/v1/insurance-applications?userId={userId}");
        Assert.Equal(1, mine.GetArrayLength());

        var delete = await client.DeleteAsync($"/api/v1/insurance-applications/{applicationId}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        var remaining = await client.GetFromJsonAsync<JsonElement>($"/api/v1/vehicle-insurance-applications?applicationId={applicationId}");
        Assert.Equal(0, remaining.GetArrayLength());
    }

    [Fact]
    public async Task LifeApplication_ForLifeProduct_ShouldWork()
    {
        var (client, userId) = await AuthenticatedClientAsync();
        var application = await client.PostAsJsonAsync("/api/v1/insurance-applications",
            new { userId, productId = 4, requestedCoverage = 80000 });
        var applicationId = (await application.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt64();

        var details = await client.PostAsJsonAsync("/api/v1/life-insurance-applications", new { applicationId });
        Assert.Equal(HttpStatusCode.Created, details.StatusCode);

        var again = await client.PostAsJsonAsync("/api/v1/life-insurance-applications", new { applicationId });
        Assert.Equal(HttpStatusCode.Conflict, again.StatusCode);
    }

    [Fact]
    public async Task Beneficiaries_CreateFilterAndPatch_ShouldWork()
    {
        var (client, _) = await AuthenticatedClientAsync();
        var policyId = Random.Shared.Next(1000, 999999);

        var create = await client.PostAsJsonAsync("/api/v1/beneficiaries", new
        {
            policyId, fullName = "María Fernanda López", documentType = "DNI", documentNumber = "45871236",
            relationship = "spouse", birthDate = "1988-06-15", participationPercentage = 50
        });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var beneficiaryId = (await create.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt64();

        var invalid = await client.PostAsJsonAsync("/api/v1/beneficiaries", new
        {
            policyId, fullName = "Otro", documentType = "DNI", documentNumber = "1", relationship = "child",
            participationPercentage = 150
        });
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);

        var byPolicy = await client.GetFromJsonAsync<JsonElement>($"/api/v1/beneficiaries?policyId={policyId}");
        Assert.Equal(1, byPolicy.GetArrayLength());

        var patch = await client.PatchAsJsonAsync($"/api/v1/beneficiaries/{beneficiaryId}", new { participationPercentage = 40 });
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
        Assert.Equal(40, (await patch.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("participationPercentage").GetDecimal());
    }
}
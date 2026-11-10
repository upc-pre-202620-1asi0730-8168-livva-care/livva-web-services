
using System.Text;
using System.Text.Json.Serialization;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.ACL;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.Internal.CommandServices;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.Internal.OutboundServices;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.Internal.QueryServices;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.Services;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Repositories;
using LivvaPlatform.API.IdentityAndProfileManagement.Infrastructure.Hashing.BCrypt.Services;
using LivvaPlatform.API.IdentityAndProfileManagement.Infrastructure.Persistence.EFC.Repositories;
using LivvaPlatform.API.IdentityAndProfileManagement.Infrastructure.Tokens.JWT.Configuration;
using LivvaPlatform.API.IdentityAndProfileManagement.Infrastructure.Tokens.JWT.Services;
using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.ACL;

using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Internal.CommandServices;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Internal.QueryServices;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Services;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Infrastructure.Persistence.EFC.Repositories;

using LivvaPlatform.API.PolicyManagement.Application.ACL;
using LivvaPlatform.API.PolicyManagement.Application.Internal.CommandServices;
using LivvaPlatform.API.PolicyManagement.Application.Internal.QueryServices;
using LivvaPlatform.API.PolicyManagement.Application.Services;
using LivvaPlatform.API.PolicyManagement.Domain.Repositories;
using LivvaPlatform.API.PolicyManagement.Infrastructure.Persistence.EFC.Repositories;
using LivvaPlatform.API.PolicyManagement.Interfaces.ACL;

using LivvaPlatform.API.SubscriptionManagement.Application.Internal.CommandServices;
using LivvaPlatform.API.SubscriptionManagement.Application.Internal.OutboundServices;
using LivvaPlatform.API.SubscriptionManagement.Application.Internal.QueryServices;
using LivvaPlatform.API.SubscriptionManagement.Application.Services;
using LivvaPlatform.API.SubscriptionManagement.Domain.Repositories;
using LivvaPlatform.API.SubscriptionManagement.Infrastructure.Payments.MercadoPago;
using LivvaPlatform.API.SubscriptionManagement.Infrastructure.Payments.MercadoPago.Services;
using LivvaPlatform.API.SubscriptionManagement.Infrastructure.Persistence.EFC.Repositories;

using LivvaPlatform.API.NotificationManagement.Application.Internal.CommandServices;
using LivvaPlatform.API.NotificationManagement.Application.Internal.QueryServices;
using LivvaPlatform.API.NotificationManagement.Application.Services;
using LivvaPlatform.API.NotificationManagement.Domain.Repositories;
using LivvaPlatform.API.NotificationManagement.Infrastructure.Persistence.EFC.Repositories;

using LivvaPlatform.API.ClaimsAndIndemnities.Application.Internal.CommandServices;
using LivvaPlatform.API.ClaimsAndIndemnities.Application.Internal.QueryServices;
using LivvaPlatform.API.ClaimsAndIndemnities.Application.Services;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Repositories;
using LivvaPlatform.API.ClaimsAndIndemnities.Infrastructure.Persistence.EFC.Repositories;

using LivvaPlatform.API.Resources;
using LivvaPlatform.API.Shared.Domain.Repositories;
using LivvaPlatform.API.Shared.Infrastructure.Interfaces.ASP.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure Lower Case URLs
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Localization Configuration
builder.Services.AddLocalization();

// Configure Kebab Case Route Naming Convention
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .AddDataAnnotationsLocalization(options =>
        options.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(SharedResource)))
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var localizer = context.HttpContext.RequestServices
                .GetRequiredService<IStringLocalizer<SharedResource>>();
            var problemDetailsFactory = context.HttpContext.RequestServices
                .GetRequiredService<ProblemDetailsFactory>();
            var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                context.HttpContext,
                context.ModelState,
                statusCode: StatusCodes.Status400BadRequest,
                title: localizer["InvalidModelStateTitle"].Value);

            var result = new BadRequestObjectResult(problemDetails);
            result.ContentTypes.Add("application/problem+json");
            return result;
        };
    });

// Register RFC 7807 ProblemDetails payloads for centralized exception handling.
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        if (context.ProblemDetails.Status is null or >= 500)
        {
            var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
            context.ProblemDetails.Title ??= localizer["UnexpectedServerError"].Value;
            context.ProblemDetails.Detail ??= localizer["UnexpectedErrorProcessingRequest"].Value;
        }
    };
});

// Generate the OpenAPI document with ASP.NET Core's built-in support (XML comments are included automatically).
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

// CORS Configuration
// Allowed origins are read from Cors:AllowedOrigins (comma-separated). When empty, any origin is allowed.
var allowedOrigins = (builder.Configuration["Cors:AllowedOrigins"] ?? string.Empty)
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
builder.Services.AddCors(options =>
{
    options.AddPolicy("LivvaCorsPolicy", policy =>
    {
        if (allowedOrigins.Length == 0) policy.AllowAnyOrigin();
        else policy.WithOrigins(allowedOrigins);
        policy.AllowAnyMethod().AllowAnyHeader();
    });
});

// JWT Authentication Configuration
// The secret supports %VARIABLE% expansion (for example, %JWT_SECRET% on Railway).
var tokenSettings = builder.Configuration.GetSection(TokenSettings.SectionName).Get<TokenSettings>() ?? new TokenSettings();
tokenSettings.Secret = Environment.ExpandEnvironmentVariables(tokenSettings.Secret);
if (string.IsNullOrWhiteSpace(tokenSettings.Secret) || tokenSettings.Secret.Contains('%') || tokenSettings.Secret.Length < 32)
    throw new InvalidOperationException("TokenSettings:Secret must be configured with at least 32 characters.");
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection(TokenSettings.SectionName));
builder.Services.PostConfigure<TokenSettings>(settings => settings.Secret = tokenSettings.Secret);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = tokenSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = tokenSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Secret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();

// Add Database Connection

// Configure Database Context and route EF logs through the app logger pipeline.
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionStringTemplate))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    var connectionString = Environment.ExpandEnvironmentVariables(connectionStringTemplate);
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    options.UseMySQL(connectionString)
        .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
        .EnableDetailedErrors();

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

// Configure Dependency Injection

// Shared Bounded Context Injection Configuration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Identity and Profile Management Bounded Context Injection Configuration
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IUserProfileCommandService, UserProfileCommandService>();
builder.Services.AddScoped<IUserProfileQueryService, UserProfileQueryService>();
builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();

// Insurance Offering and Applications Bounded Context Injection Configuration
builder.Services.AddScoped<IInsurerRepository, InsurerRepository>();
builder.Services.AddScoped<IInsuranceProductRepository, InsuranceProductRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IInsuranceApplicationRepository, InsuranceApplicationRepository>();
builder.Services.AddScoped<IVehicleInsuranceApplicationRepository, VehicleInsuranceApplicationRepository>();
builder.Services.AddScoped<ILifeInsuranceApplicationRepository, LifeInsuranceApplicationRepository>();
builder.Services.AddScoped<IBeneficiaryRepository, BeneficiaryRepository>();
builder.Services.AddScoped<IOfferingQueryService, OfferingQueryService>();
builder.Services.AddScoped<IVehicleCommandService, VehicleCommandService>();
builder.Services.AddScoped<IVehicleQueryService, VehicleQueryService>();
builder.Services.AddScoped<IInsuranceApplicationCommandService, InsuranceApplicationCommandService>();
builder.Services.AddScoped<IInsuranceApplicationQueryService, InsuranceApplicationQueryService>();
builder.Services.AddScoped<IBeneficiaryCommandService, BeneficiaryCommandService>();
builder.Services.AddScoped<IBeneficiaryQueryService, BeneficiaryQueryService>();

// Policy Management Bounded Context Injection Configuration
builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();
builder.Services.AddScoped<IPolicyCoverageRepository, PolicyCoverageRepository>();
builder.Services.AddScoped<IPolicyDocumentRepository, PolicyDocumentRepository>();
builder.Services.AddScoped<IRenewalRepository, RenewalRepository>();
builder.Services.AddScoped<IPolicyCommandService, PolicyCommandService>();
builder.Services.AddScoped<IPolicyQueryService, PolicyQueryService>();
builder.Services.AddScoped<IPolicyManagementContextFacade, PolicyManagementContextFacade>();

// Subscription Management Bounded Context Injection Configuration
builder.Services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
builder.Services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
builder.Services.AddScoped<ISubscriptionPaymentRepository, SubscriptionPaymentRepository>();
builder.Services.AddScoped<ISubscriptionCommandService, SubscriptionCommandService>();
builder.Services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();

// Notification Management Bounded Context Injection Configuration
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationCommandService, NotificationCommandService>();
builder.Services.AddScoped<INotificationQueryService, NotificationQueryService>();

// Claims and Indemnities Bounded Context Injection Configuration
builder.Services.AddScoped<IVehicleClaimRepository, VehicleClaimRepository>();
builder.Services.AddScoped<IIndemnityRequestRepository, IndemnityRequestRepository>();

builder.Services.AddScoped<IVehicleClaimCommandService, VehicleClaimCommandService>();
builder.Services.AddScoped<IVehicleClaimQueryService, VehicleClaimQueryService>();

builder.Services.AddScoped<IIndemnityRequestCommandService, IndemnityRequestCommandService>();
builder.Services.AddScoped<IIndemnityRequestQueryService, IndemnityRequestQueryService>();

// Mercado Pago: the access token comes from MERCADO_PAGO_ACCESS_TOKEN or MercadoPago:AccessToken (user secrets).
builder.Services.Configure<MercadoPagoSettings>(builder.Configuration.GetSection(MercadoPagoSettings.SectionName));
builder.Services.PostConfigure<MercadoPagoSettings>(settings =>
{
    if (string.IsNullOrWhiteSpace(settings.AccessToken))
        settings.AccessToken = builder.Configuration["MERCADO_PAGO_ACCESS_TOKEN"] ?? string.Empty;
});
builder.Services.AddHttpClient<IPaymentProviderService, MercadoPagoPaymentService>((serviceProvider, client) =>
{
    var baseUrl = builder.Configuration[$"{MercadoPagoSettings.SectionName}:BaseUrl"] ?? "https://api.mercadopago.com/";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// Apply pending migrations on startup (safe to call even when schema is up to date).
// Non-relational providers (used by integration tests) create the schema directly.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (context.Database.IsRelational()) context.Database.Migrate();
    else context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Localization must run before OpenAPI so generated documentation follows the request culture.
string[] supportedCultures = ["en", "en-US", "es", "es-PE", "es-419"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
localizationOptions.ApplyCurrentCultureToResponseHeaders = true;
app.UseRequestLocalization(localizationOptions);

app.UseCors("LivvaCorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// OpenAPI, Scalar and Swagger UI are available in all environments.
app.MapOpenApi();
app.MapScalarApiReference();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Livva Platform API v1"));
app.MapControllers();

app.Run();

/// <summary>
///     Entry point marker used by <c>WebApplicationFactory</c> in integration tests.
/// </summary>
public partial class Program;

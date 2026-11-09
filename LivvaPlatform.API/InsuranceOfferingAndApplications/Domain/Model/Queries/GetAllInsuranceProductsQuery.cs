using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Queries;

/// <summary>
///     Query to get insurance products, optionally filtered by type and insurer.
/// </summary>
public record GetAllInsuranceProductsQuery(InsuranceType? InsuranceType, long? InsurerId);

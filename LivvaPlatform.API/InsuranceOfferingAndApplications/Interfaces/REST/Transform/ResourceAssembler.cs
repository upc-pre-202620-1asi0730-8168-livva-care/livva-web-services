using LivvaPlatform.API.Shared.Domain.Model.ValueObjects;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.ValueObjects;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Resources;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Transform;

/// <summary>
///     Assembles resources from aggregates and commands from resources for the Insurance Offering and Applications
///     bounded context.
/// </summary>
public static class ResourceAssembler
{
    /// <summary>Converts an insurer into a resource.</summary>
    /// <param name="e">The insurer.</param>
    /// <returns>The insurer resource.</returns>
    public static InsurerResource ToResource(Insurer e) =>
        new(e.Id, e.ExternalReference, e.Name, e.Active, e.CreatedAt, e.UpdatedAt);

    /// <summary>Converts an insurance product into a resource.</summary>
    /// <param name="e">The insurance product.</param>
    /// <returns>The insurance product resource.</returns>
    public static InsuranceProductResource ToResource(InsuranceProduct e) =>
        new(e.Id, e.InsurerId, e.Name, EnumValueConverter.ToValue(e.InsuranceType), e.CoverageAmount, e.PremiumAmount,
            e.Active, e.CreatedAt, e.UpdatedAt);

    /// <summary>Converts a vehicle into a resource.</summary>
    /// <param name="e">The vehicle.</param>
    /// <returns>The vehicle resource.</returns>
    public static VehicleResource ToResource(Vehicle e) =>
        new(e.Id, e.UserId, e.LicensePlate, e.Brand, e.Model, e.ManufactureYear, e.EstimatedValue, e.CreatedAt, e.UpdatedAt);

    /// <summary>Converts an insurance application into a resource.</summary>
    /// <param name="e">The insurance application.</param>
    /// <returns>The insurance application resource.</returns>
    public static InsuranceApplicationResource ToResource(InsuranceApplication e) =>
        new(e.Id, e.UserId, e.ProductId, e.RequestedCoverage, EnumValueConverter.ToValue(e.Status), e.CreatedAt, e.UpdatedAt);

    /// <summary>Converts vehicle details into a resource.</summary>
    /// <param name="e">The vehicle details.</param>
    /// <returns>The vehicle insurance application resource.</returns>
    public static VehicleInsuranceApplicationResource ToResource(VehicleInsuranceApplication e) =>
        new(e.Id, e.ApplicationId, e.VehicleId);

    /// <summary>Converts life details into a resource.</summary>
    /// <param name="e">The life details.</param>
    /// <returns>The life insurance application resource.</returns>
    public static LifeInsuranceApplicationResource ToResource(LifeInsuranceApplication e) =>
        new(e.Id, e.ApplicationId);

    /// <summary>Converts a beneficiary into a resource.</summary>
    /// <param name="e">The beneficiary.</param>
    /// <returns>The beneficiary resource.</returns>
    public static BeneficiaryResource ToResource(Beneficiary e) =>
        new(e.Id, e.PolicyId, e.FullName, e.DocumentType, e.DocumentNumber, e.Relationship, e.BirthDate,
            e.ParticipationPercentage, e.CreatedAt, e.UpdatedAt);

    /// <summary>Converts a resource into a create vehicle command.</summary>
    /// <param name="r">The resource.</param>
    /// <returns>The command.</returns>
    public static CreateVehicleCommand ToCommand(CreateVehicleResource r) =>
        new(r.UserId, r.LicensePlate, r.Brand, r.Model, r.ManufactureYear, r.EstimatedValue);

    /// <summary>Converts a resource into an update vehicle command.</summary>
    /// <param name="vehicleId">The identifier of the vehicle.</param>
    /// <param name="r">The resource.</param>
    /// <returns>The command.</returns>
    public static UpdateVehicleCommand ToCommand(long vehicleId, UpdateVehicleResource r) =>
        new(vehicleId, r.LicensePlate, r.Brand, r.Model, r.ManufactureYear, r.EstimatedValue);

    /// <summary>Converts a resource into a create insurance application command.</summary>
    /// <param name="r">The resource.</param>
    /// <returns>The command.</returns>
    public static CreateInsuranceApplicationCommand ToCommand(CreateInsuranceApplicationResource r) =>
        new(r.UserId, r.ProductId, r.RequestedCoverage);

    /// <summary>Converts a resource into a create vehicle insurance application command.</summary>
    /// <param name="r">The resource.</param>
    /// <returns>The command.</returns>
    public static CreateVehicleInsuranceApplicationCommand ToCommand(CreateVehicleInsuranceApplicationResource r) =>
        new(r.ApplicationId, r.VehicleId);

    /// <summary>Converts a resource into a create life insurance application command.</summary>
    /// <param name="r">The resource.</param>
    /// <returns>The command.</returns>
    public static CreateLifeInsuranceApplicationCommand ToCommand(CreateLifeInsuranceApplicationResource r) =>
        new(r.ApplicationId);

    /// <summary>Converts a resource into a create beneficiary command.</summary>
    /// <param name="r">The resource.</param>
    /// <returns>The command.</returns>
    public static CreateBeneficiaryCommand ToCommand(CreateBeneficiaryResource r) =>
        new(r.PolicyId, r.FullName, r.DocumentType, r.DocumentNumber, r.Relationship, r.BirthDate, r.ParticipationPercentage);

    /// <summary>Converts a resource into an update beneficiary command.</summary>
    /// <param name="beneficiaryId">The identifier of the beneficiary.</param>
    /// <param name="r">The resource.</param>
    /// <returns>The command.</returns>
    public static UpdateBeneficiaryCommand ToCommand(long beneficiaryId, UpdateBeneficiaryResource r) =>
        new(beneficiaryId, r.FullName, r.DocumentType, r.DocumentNumber, r.Relationship, r.BirthDate, r.ParticipationPercentage);
}
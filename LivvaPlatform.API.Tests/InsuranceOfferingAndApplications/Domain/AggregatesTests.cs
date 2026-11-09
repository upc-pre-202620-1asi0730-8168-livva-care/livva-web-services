using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.Tests.InsuranceOfferingAndApplications.Domain;

/// <summary>
///     Unit tests for the aggregates of the Insurance Offering and Applications bounded context.
/// </summary>
public class AggregatesTests
{
    [Fact]
    public void Vehicle_ShouldNormalizeLicensePlate()
    {
        var vehicle = new Vehicle(new CreateVehicleCommand(1, " abc-123 ", "Toyota", "Corolla", 2022, 65000));

        Assert.Equal("ABC-123", vehicle.LicensePlate);
    }

    [Theory]
    [InlineData(1800, 65000)]
    [InlineData(2022, 0)]
    public void Vehicle_WithInvalidYearOrValue_ShouldThrow(int year, decimal value)
    {
        Assert.ThrowsAny<ArgumentException>(() =>
            new Vehicle(new CreateVehicleCommand(1, "ABC-123", "Toyota", "Corolla", year, value)));
    }

    [Fact]
    public void Vehicle_Update_ShouldKeepOmittedFields()
    {
        var vehicle = new Vehicle(new CreateVehicleCommand(1, "ABC-123", "Toyota", "Corolla", 2022, 65000));

        vehicle.UpdateInformation(new UpdateVehicleCommand(1, null, null, "Yaris", null, null));

        Assert.Equal("Yaris", vehicle.Model);
        Assert.Equal("Toyota", vehicle.Brand);
    }

    [Fact]
    public void InsuranceApplication_ShouldStartPendingAndFollowStatusFlow()
    {
        var application = new InsuranceApplication(new CreateInsuranceApplicationCommand(1, 3, 100000));

        Assert.Equal(RequestStatus.PENDING, application.Status);
        application.StartEvaluation();
        application.Approve();
        Assert.Equal(RequestStatus.APPROVED, application.Status);
        Assert.Throws<InvalidOperationException>(() => application.Cancel());
    }

    [Fact]
    public void InsuranceApplication_WithNonPositiveCoverage_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => new InsuranceApplication(new CreateInsuranceApplicationCommand(1, 3, 0)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(120)]
    public void Beneficiary_WithInvalidPercentage_ShouldThrow(decimal percentage)
    {
        Assert.Throws<ArgumentException>(() => new Beneficiary(
            new CreateBeneficiaryCommand(2, "María López", "DNI", "45871236", "spouse", null, percentage)));
    }

    [Fact]
    public void EnumValueConverter_ShouldUseLowerCaseValues()
    {
        Assert.Equal("under_review", EnumValueConverter.ToValue(RequestStatus.UNDER_REVIEW));
        Assert.True(EnumValueConverter.TryParse<InsuranceType>("vehicle", out var type));
        Assert.Equal(InsuranceType.VEHICLE, type);
        Assert.False(EnumValueConverter.TryParse<InsuranceType>("health", out _));
    }
}
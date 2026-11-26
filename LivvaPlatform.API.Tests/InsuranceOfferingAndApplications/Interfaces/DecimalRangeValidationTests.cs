using System.ComponentModel.DataAnnotations;
using System.Globalization;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Resources;

namespace LivvaPlatform.API.Tests.InsuranceOfferingAndApplications.Interfaces;

/// <summary>
///     Regression tests for decimal range validation under cultures that use a comma as decimal separator.
/// </summary>
public class DecimalRangeValidationTests
{
    [Theory]
    [InlineData("es-PE")]
    [InlineData("es-419")]
    [InlineData("en-US")]
    public void CreateVehicleResource_IsValid_WhenTheRequestCultureUsesACommaDecimalSeparator(string cultureName)
    {
        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo(cultureName);

        try
        {
            var resource = new CreateVehicleResource(1, "CAZ-123", "Chevrolet", "Aveo", 2020, 67000m);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(resource, new ValidationContext(resource), results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public void CreateVehicleResource_IsInvalid_WhenTheEstimatedValueIsZero()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("es-PE");

        try
        {
            var resource = new CreateVehicleResource(1, "CAZ-123", "Chevrolet", "Aveo", 2020, 0m);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(resource, new ValidationContext(resource), results, true);

            Assert.False(isValid);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}

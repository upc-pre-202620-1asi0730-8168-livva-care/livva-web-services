using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;

/// <summary>
///     Represents a vehicle registered by its owner.
/// </summary>
/// <remarks>The license plate is unique and is stored in upper case.</remarks>
public partial class Vehicle
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Vehicle()
    {
        LicensePlate = string.Empty;
        Brand = string.Empty;
        Model = string.Empty;
    }

    /// <summary>
    ///     Registers a vehicle from a creation command.
    /// </summary>
    /// <param name="command">The create vehicle command.</param>
    /// <exception cref="ArgumentException">Thrown when a value is not valid.</exception>
    public Vehicle(CreateVehicleCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        UserId = command.UserId;
        LicensePlate = NormalizeLicensePlate(command.LicensePlate);
        Brand = Required(command.Brand);
        Model = Required(command.Model);
        ManufactureYear = ValidYear(command.ManufactureYear);
        EstimatedValue = ValidValue(command.EstimatedValue);
    }

    /// <summary>Gets the identifier of the vehicle.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the identifier of the owner user.</summary>
    public long UserId { get; private set; }

    /// <summary>Gets the license plate.</summary>
    public string LicensePlate { get; private set; }

    /// <summary>Gets the brand.</summary>
    public string Brand { get; private set; }

    /// <summary>Gets the model.</summary>
    public string Model { get; private set; }

    /// <summary>Gets the manufacture year.</summary>
    public int ManufactureYear { get; private set; }

    /// <summary>Gets the estimated value.</summary>
    public decimal EstimatedValue { get; private set; }

    /// <summary>
    ///     Normalizes a license plate by trimming it and converting it to upper case.
    /// </summary>
    /// <param name="licensePlate">The license plate.</param>
    /// <returns>The normalized license plate.</returns>
    public static string NormalizeLicensePlate(string licensePlate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(licensePlate);
        return licensePlate.Trim().ToUpperInvariant();
    }

    /// <summary>
    ///     Updates the vehicle information. Null values keep the current value.
    /// </summary>
    /// <param name="command">The update vehicle command.</param>
    public void UpdateInformation(UpdateVehicleCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.LicensePlate is not null) LicensePlate = NormalizeLicensePlate(command.LicensePlate);
        if (command.Brand is not null) Brand = Required(command.Brand);
        if (command.Model is not null) Model = Required(command.Model);
        if (command.ManufactureYear is not null) ManufactureYear = ValidYear(command.ManufactureYear.Value);
        if (command.EstimatedValue is not null) EstimatedValue = ValidValue(command.EstimatedValue.Value);
    }

    private static string Required(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim();
    }

    private static int ValidYear(int year) =>
        year >= 1900 && year <= DateTime.UtcNow.Year + 1
            ? year
            : throw new ArgumentException("The manufacture year is not valid.", nameof(year));

    private static decimal ValidValue(decimal value) =>
        value > 0 ? value : throw new ArgumentException("The estimated value must be positive.", nameof(value));
}
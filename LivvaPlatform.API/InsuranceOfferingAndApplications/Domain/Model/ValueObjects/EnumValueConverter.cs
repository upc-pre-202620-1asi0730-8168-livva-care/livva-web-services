namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.ValueObjects;

/// <summary>
///     Converts domain enumerations to and from the lower-case values used by the Web Application
///     (for example, <c>UNDER_REVIEW</c> is exchanged as <c>under_review</c>).
/// </summary>
public static class EnumValueConverter
{
    /// <summary>
    ///     Converts an enumeration value to its lower-case representation.
    /// </summary>
    /// <param name="value">The enumeration value.</param>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <returns>The lower-case representation.</returns>
    public static string ToValue<TEnum>(TEnum value) where TEnum : struct, Enum =>
        value.ToString().ToLowerInvariant();

    /// <summary>
    ///     Parses a value case-insensitively.
    /// </summary>
    /// <param name="value">The value to parse.</param>
    /// <param name="result">The parsed enumeration value.</param>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <returns>True if the value is valid; otherwise, false.</returns>
    public static bool TryParse<TEnum>(string? value, out TEnum result) where TEnum : struct, Enum
    {
        result = default;
        return !string.IsNullOrWhiteSpace(value) && !int.TryParse(value, out _) &&
               Enum.TryParse(value.Trim(), true, out result);
    }
}
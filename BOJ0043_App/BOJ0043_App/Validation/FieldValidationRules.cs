using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace BOJ0043_App.Validation
{
    public class RequiredFieldValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult(false, "Toto pole je povinné.");
            return ValidationResult.ValidResult;
        }
    }

    public class EmailValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var email = value?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(email))
                return new ValidationResult(false, "Toto pole je povinné.");
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return new ValidationResult(false, "Neplatný formát emailu.");
            return ValidationResult.ValidResult;
        }
    }

    public class DoubleValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.ValidResult; // Not required, just format check
            var str = value.ToString().Replace(',', '.'); // allow both comma and dot
            if (!double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                return new ValidationResult(false, "Zadejte platné číslo.");
            return ValidationResult.ValidResult;
        }
    }
}

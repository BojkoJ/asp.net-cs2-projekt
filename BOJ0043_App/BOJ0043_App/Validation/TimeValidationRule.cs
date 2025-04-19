using System.Globalization;
using System.Windows.Controls;

namespace BOJ0043_App.Validation
{
    public class TimeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string time = value as string ?? string.Empty;
            if (string.IsNullOrWhiteSpace(time))
                return new ValidationResult(false, "Čas je povinný.");
            if (TimeSpan.TryParse(time, out _))
                return ValidationResult.ValidResult;
            return new ValidationResult(false, "Zadejte čas ve formátu HH:mm.");
        }
    }
}

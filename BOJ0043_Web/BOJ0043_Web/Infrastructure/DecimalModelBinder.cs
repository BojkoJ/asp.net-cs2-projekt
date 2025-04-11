using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BOJ0043_Web.Infrastructure
{
    public class DecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            // Získání hodnoty z value providera
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            // Nastavení attempt value
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
            
            // Získání textové hodnoty z value providera
            var value = valueProviderResult.FirstValue;
            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            // Pokus o převod na decimal s explicitním použitím tečky jako oddělovače
            if (decimal.TryParse(value.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalValue))
            {
                bindingContext.Result = ModelBindingResult.Success(decimalValue);
                return Task.CompletedTask;
            }

            // Pokud převod selhal, přidání chyby
            bindingContext.ModelState.TryAddModelError(
                bindingContext.ModelName,
                $"Hodnota '{value}' není platnou decimální hodnotou.");

            return Task.CompletedTask;
        }
    }
}

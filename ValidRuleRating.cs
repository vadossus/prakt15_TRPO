using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace prakt15_TRPO
{
    public class ValidRuleRating : ValidationRule
    {
        public double Min { get; set; } = 0;
        public double Max { get; set; } = 5.0;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string stringValue = (value as string)?.Replace('.', ',');

            if (double.TryParse(stringValue, out double result))
            {
                if (result >= Min && result <= Max)
                {
                    return ValidationResult.ValidResult;
                }
                return new ValidationResult(false, $"Рейтинг должен быть от {Min} до {Max}");
            }
            return new ValidationResult(false, "Введите корректное число");
        }


    }
}

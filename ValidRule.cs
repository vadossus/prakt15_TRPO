using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace prakt15_TRPO
{
    public class ValidRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = (value ?? "").ToString().Trim();
            if (string.IsNullOrEmpty(input))
            {
                return new ValidationResult(false, "Поле обязательно для заполнения");
            }

            return ValidationResult.ValidResult;
        }
    }
}

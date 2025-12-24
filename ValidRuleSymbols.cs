using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace prakt15_TRPO
{
    public class ValidRuleSymbols : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = (value ?? "").ToString().Trim();

            if (!Regex.IsMatch(input, @"^[0-9]*\.?[0-9]+$"))
            {
                return new ValidationResult(false, "Допустимы только цифры и точка (.)");
            }

            return ValidationResult.ValidResult;
        }
    }
}

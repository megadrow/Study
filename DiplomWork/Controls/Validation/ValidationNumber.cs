using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Controls.Validation
{
    public class ValidationNumber : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var str = value.ToString();
            int res;
            if (!int.TryParse(str, out res))
            {
                return new ValidationResult(false, "Illegal characters or ");
            }

            if (res < 0)
            {
                return new ValidationResult(false, "Значение должно быть больше нуля");
            }

            return new ValidationResult(true, null);
        }
    }
}

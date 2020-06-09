using System;
using System.Collections.Generic;
using System.Text;

namespace PhillipsConversion.Lightsaber
{
    public class PricingHelper
    {
        public static decimal Ceiling(decimal value, int precision)
        {
            bool isNegative = false;
            if (value < 0)
            {
                isNegative = true;
                value = Math.Abs(value);
            }

            double decimalPrecision = Math.Abs(precision);
            decimal coefficient = Convert.ToDecimal(Math.Pow(10, decimalPrecision));
            var result = Math.Ceiling(value * coefficient) / coefficient;

            return isNegative ? -result : result;
        }
    }
}

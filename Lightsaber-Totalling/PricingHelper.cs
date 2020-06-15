using Apttus.Lightsaber.Pricing.Common.Constants;
using System;

namespace Apttus.Lightsaber.Phillips.Totalling
{
    public class PricingHelper
    {
        // <summary>
        /// apply precision rounding to the input value based on rounding mode.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="roundingMode"></param>
        /// <returns></returns>
        public static decimal? ApplyRounding(decimal? value, int precision, RoundingMode roundingMode)
        {
            decimal? roundedValue = null;
            switch (roundingMode)
            {
                case RoundingMode.HALF_UP:
                    roundedValue = Math.Round(value.GetValueOrDefault(), precision, MidpointRounding.AwayFromZero);
                    break;
                case RoundingMode.UP:
                    roundedValue = Ceiling(value.GetValueOrDefault(), precision);
                    break;
                case RoundingMode.DOWN:
                    roundedValue = Floor(value.GetValueOrDefault(), precision);
                    break;
                case RoundingMode.HALF_DOWN:
                    roundedValue = HalfDown(value.GetValueOrDefault(), precision);
                    break;
                default: // HALF_EVEN.
                    roundedValue = Math.Round(value.GetValueOrDefault(), precision, MidpointRounding.ToEven);
                    break;
            }

            return roundedValue;
        }

        /// <summary>
        /// Math.Ceiling with Precision support.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        private static decimal Ceiling(decimal value, int precision)
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

        /// <summary>
        /// Math.Floor with Precision support.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        private static decimal Floor(decimal value, int precision)
        {
            bool isNegative = false;
            if (value < 0)
            {
                isNegative = true;
                value = Math.Abs(value);
            }

            double decimalPrecision = Math.Abs(precision);
            decimal coefficient = Convert.ToDecimal(Math.Pow(10, decimalPrecision));
            var result = Math.Floor(value * coefficient) / coefficient;

            return isNegative ? -result : result;
        }

        /// <summary>
        /// Half Down Rounding Calculation.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        private static decimal HalfDown(decimal value, int precision)
        {
            bool isNegative = false;
            if (value < 0)
            {
                isNegative = true;
                value = Math.Abs(value);
            }

            // calculate the half point value in last precision digit.
            // if precision 0, it will be 0.5m,
            // if precision 1, it will be 0.05m, and so on.
            decimal halfValue = 0.5m / Convert.ToDecimal(Math.Pow(10, precision));

            // add above value and calculate first round half toward positive infinity.
            decimal floorValue = value;
            decimal result = Floor(floorValue + halfValue, precision);

            // If the fraction part of the original value is last precision halfValue, 
            // subtract last precision digit by 1,
            // if precision 0, it will be 1m,
            // if precision 1, it will be 0.1m, and so on.
            if (floorValue - Floor(floorValue, precision) == halfValue)
            {
                result -= 1m / Convert.ToDecimal(Math.Pow(10, precision));
            }

            return isNegative ? -result : result;
        }
    }
}

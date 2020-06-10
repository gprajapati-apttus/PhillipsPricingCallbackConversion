using Apttus.Lightsaber.Pricing.Common.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhillipsConversion.Lightsaber
{
    public class PricingHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static decimal? ComputeTerm(string frequency, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue && endDate >= startDate)
            {
                var numDays = (endDate.Value.Date - startDate.Value.Date).Days + 1;

                switch (frequency)
                {
                    case CommonConstants.FrequencyMonthly:
                        return ComputeSellingMonthlyTerm(startDate.Value, endDate.Value);

                    case CommonConstants.FrequencyQuarterly:
                        return ComputeSellingMonthlyTerm(startDate.Value, endDate.Value) / 3;

                    case CommonConstants.FrequencyHalfYearly:
                        return ComputeSellingMonthlyTerm(startDate.Value, endDate.Value) / 6;

                    case CommonConstants.FrequencyYearly:
                        return ComputeSellingMonthlyTerm(startDate.Value, endDate.Value) / 12;

                    case CommonConstants.FrequencyDaily:
                        return numDays;
                }
            }

            return 1;
        }

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

        private static decimal ComputeSellingMonthlyTerm(DateTime startDate, DateTime endDate)
        {
            // Use case like 15 Sep 2019 to 14 Feb 2020, where the end day is an immediate day before the start day in end date month
            // In such cases, monthly term would be squarely the number of months in difference
            int months = (endDate.Year - startDate.Year) * 12 + (endDate.Month - startDate.Month);
            var extraDays = endDate.Subtract(startDate.AddMonths(months)).TotalDays + 1;

            if (extraDays == 0)
            {
                return months;
            }

            /* For remaning use cases we will divide complete term into three below parts
               1. Initial term : Starting calendar month (partial term) and would be represeted as ratio number of covered days to total number of days.
               2. Mid term : Mid duration which covers all complete calendar months. This gives part of selling term as complete interger.
               3. Last term : Ending calendar month (partial term) and would be represeted as ratio number of covered days to total number of days.
            */

            // Initial term
            int totalDaysInMonth = DateTime.DaysInMonth(startDate.Year, startDate.Month);
            decimal coveredDays = totalDaysInMonth - startDate.Day + 1;
            decimal initialTerm = coveredDays / totalDaysInMonth;

            // Mid term
            int midTerm = (endDate.Year - startDate.Year) * 12 + (endDate.Month - startDate.Month - 1);

            // Last term
            decimal lastTerm = (decimal)endDate.Day / DateTime.DaysInMonth(endDate.Year, endDate.Month);

            return initialTerm + midTerm + lastTerm;
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

using Apttus.Lightsaber.Pricing.Common.Models;

namespace Apttus.Lightsaber.Phillips.Pricing
{
    public static class Extension
    {
        public static decimal GetValuetOrDefault(this LineItemModel lineItemModel, string fieldName, decimal defaultValue)
        {
            decimal? fieldValue;

            if (!fieldName.Contains("."))
            {
                fieldValue = lineItemModel.Get<decimal?>(fieldName);
            }
            else
            {
                fieldValue = lineItemModel.GetLookupValue<decimal?>(fieldName);
            }

            if (fieldValue != null)
            {
                return fieldValue.Value;
            }

            return defaultValue;
        }
    }
}

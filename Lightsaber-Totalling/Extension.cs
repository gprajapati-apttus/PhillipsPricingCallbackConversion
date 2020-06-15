using Apttus.Lightsaber.Pricing.Common.Models;

namespace Apttus.Lightsaber.Phillips.Totalling
{
    public static class Extension
    {
        public static decimal GetValuetOrDefault(this LineItemModel lineItemModel, string fieldName, decimal defaultValue)
        {
            var fieldValue = lineItemModel.Get<decimal?>(fieldName);

            if (fieldValue != null)
                return fieldValue.Value;

            return defaultValue;
        }
    }
}

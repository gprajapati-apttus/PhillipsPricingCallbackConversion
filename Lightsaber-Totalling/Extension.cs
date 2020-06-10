using Apttus.Lightsaber.Pricing.Common.Messages.Cart;
using Apttus.Lightsaber.Pricing.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhillipsConversion.Totalling
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

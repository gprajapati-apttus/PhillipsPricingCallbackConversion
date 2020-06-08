using Apttus.Lightsaber.Pricing.Common.Models;
using Apttus.Lightsaber.Pricing.Common.Constants;
using System;
using System.Collections.Generic;
using Apttus.Lightsaber.Pricing.Common.Messages;
using System.Threading.Tasks;
using Apttus.Lightsaber.Extensibility.Framework.Library.Implementation;
using Apttus.Lightsaber.Extensibility.Framework.Library.Interfaces;
using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Extensibility.Framework.Library.Common;
using PhillipsConversion.Lightsaber;

namespace PhillipsConversion
{
    public class Lightsaber_CustomPricingCallBackHelper_Ultra
    {
        private Dictionary<string, object> proposal;
        private IDBHelper dBHelper = null;

        public Lightsaber_CustomPricingCallBackHelper_Ultra(Dictionary<string, object> proposal, IDBHelper dBHelper)
        {
            this.proposal = proposal;
            this.dBHelper = dBHelper;
        }

        internal async Task populateExtendedQuantity(List<LineItemModel> batchLineItems)
        {
            foreach (LineItemModel batchLineItem in batchLineItems)
            {
                decimal extendedQuantity = batchLineItem.GetQuantity();
                decimal quantity = batchLineItem.GetQuantity();

                if (batchLineItem.GetLineType() == LineType.Option)
                {
                    ProductLineItemModel productLineItemModel = batchLineItem.GetRootParentLineItem();
                    decimal bundleQuantity = productLineItemModel.GetPrimaryLineItem().GetQuantity();
                    quantity = bundleQuantity;
                    extendedQuantity = bundleQuantity * batchLineItem.GetQuantity();

                    batchLineItem.Entity.IsOptional = productLineItemModel.GetPrimaryLineItem().IsOptional();
                }

                batchLineItem.Set(LineItemCustomField.APTS_Extended_Quantity__c, extendedQuantity);
                batchLineItem.Set(LineItemCustomField.APTS_Bundle_Quantity__c, quantity);
            }

            await Task.CompletedTask;
        }

        internal async Task<Dictionary<string, PriceListItemQueryModel>> populateSPOOPLIDictionary(List<LineItemModel> batchLineItems)
        {
            HashSet<string> spooProdIds = new HashSet<string>();
            Dictionary<string, PriceListItemQueryModel>  pliSPOODictionary = new Dictionary<string, PriceListItemQueryModel>();

            foreach (LineItemModel batchLineItem in batchLineItems)
            {
                if (!string.IsNullOrWhiteSpace(batchLineItem.Get<string>(LineItemCustomField.Apttus_Config2__ProductId__r_APTS_SPOO_Type__c)))
                    spooProdIds.Add(batchLineItem.GetProductOrOptionId());
            }

            if (spooProdIds.Count > 0)
            {
                var query = QueryHelper.GetPLIPriceMultiplierQuery(spooProdIds);

                List<PriceListItemQueryModel> spooPriceListItems = await dBHelper.FindAsync<PriceListItemQueryModel>(query);
                foreach (var spooPriceListItem in spooPriceListItems)
                {
                    pliSPOODictionary.Add(spooPriceListItem.Id, spooPriceListItem);
                }   
            }

            return pliSPOODictionary;
        }

        internal void populateMapPliAgreementContractsBeforePricing(List<LineItemModel> lineItems)
        {
            throw new NotImplementedException();
        }
    }
}

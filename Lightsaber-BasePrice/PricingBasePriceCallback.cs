using Apttus.Lightsaber.Extensibility.Framework.Library;
using Apttus.Lightsaber.Pricing.Common.Callback;
using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Pricing.Common.Messages;
using Apttus.Lightsaber.Pricing.Common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apttus.Lightsaber.Phillips.Pricing
{
    public class PricingBasePriceCallback : CodeExtensibility, IPricingBasePriceCallback
    {
        private PricingBasePriceCallbackHelper pcbHelper = null;
        private List<LineItemModel> batchLineItems = null;

        public async Task BeforePricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            batchLineItems = batchPriceRequest.LineItems.SelectMany(x => x.ChargeLines).ToList();

            var proposalSO = batchPriceRequest.Cart.Get<Dictionary<string, object>>(Constants.PROPOSAL);
            pcbHelper = new PricingBasePriceCallbackHelper(proposalSO, GetDBHelper(), GetPricingHelper());

            await pcbHelper.PopulateExtendedQuantity(batchLineItems);
        }

        public async Task OnPricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            var namBundleDictionary = await pcbHelper.QueryAndPopulateNAMBundleDictionary(batchPriceRequest.CartContext.LineItems.SelectMany(x => x.ChargeLines).ToList());
            var pliSPOODictionary = await pcbHelper.PopulateSPOOPLIDictionary(batchLineItems);
            var pliDictionary = await pcbHelper.QueryAndPopulatePliCustomFields(batchLineItems);
            var agreementTierDictionary = await pcbHelper.QueryAndPopulateAgreementTiers(pliDictionary);


            foreach (var batchLineItem in batchLineItems)
            {
                PriceListItemModel priceListItemModel = batchLineItem.GetPriceListItem();
                PriceListItem priceListItemEntity = priceListItemModel.Entity;

                await pcbHelper.CalculateSPOOPricing(batchLineItem, pliSPOODictionary);

                await pcbHelper.CalculateExtendedListPriceAndOptionUnitPrice(batchLineItem);

                if (batchLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Apttus_Config2__ProductType__c) != Constants.SYSTEM_TYPE_SERVICE)
                {
                    await pcbHelper.PopulateTier(batchLineItem, pliDictionary, agreementTierDictionary);
                }

                await pcbHelper.UpdateNAMBundleOptions(batchLineItem, namBundleDictionary);
            }
        }

        public async Task AfterPricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            await Task.CompletedTask;
        }
    }
}

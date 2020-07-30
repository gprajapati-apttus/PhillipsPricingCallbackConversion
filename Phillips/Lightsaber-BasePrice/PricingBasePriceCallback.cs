using Apttus.Lightsaber.Extensibility.Framework.Library;
using Apttus.Lightsaber.Pricing.Common.Callback;
using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Pricing.Common.Messages;
using Apttus.Lightsaber.Pricing.Common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apttus.Lightsaber.Phillips.Common;
using LineItem = Apttus.Lightsaber.Phillips.Common.LineItem;

namespace Apttus.Lightsaber.Phillips.Pricing
{
    public class PricingBasePriceCallback : CodeExtensibility, IPricingBasePriceCallback
    {
        private PricingBasePriceCallbackHelper pcbHelper = null;

        public async Task BeforePricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            var batchLineItems = batchPriceRequest.LineItems.SelectMany(x => x.ChargeLines).Select(s => new LineItem(s)).ToList();

            var proposalSO = Proposal.Create(batchPriceRequest.Cart); 
            pcbHelper = new PricingBasePriceCallbackHelper(proposalSO, GetDBHelper(), GetPricingHelper());

            await pcbHelper.PopulateExtendedQuantity(batchLineItems);
        }

        public async Task OnPricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            var batchLineItems = batchPriceRequest.LineItems.SelectMany(x => x.ChargeLines).Select(s => new LineItem(s)).ToList();

            var namBundleDictionary = await pcbHelper.QueryAndPopulateNAMBundleDictionary(batchPriceRequest.CartContext.LineItems.SelectMany(x => x.ChargeLines).Select(s => new LineItem(s)).ToList());
            var pliSPOODictionary = await pcbHelper.PopulateSPOOPLIDictionary(batchLineItems);
            var pliDictionary = await pcbHelper.QueryAndPopulatePliCustomFields(batchLineItems);
            var agreementTierDictionary = await pcbHelper.QueryAndPopulateAgreementTiers(pliDictionary);

            foreach (var batchLineItem in batchLineItems)
            {
                PriceListItemModel priceListItemModel = batchLineItem.GetPriceListItem();
                PriceListItem priceListItemEntity = priceListItemModel.Entity;

                await pcbHelper.CalculateSPOOPricing(batchLineItem, pliSPOODictionary);

                await pcbHelper.CalculateExtendedListPriceAndOptionUnitPrice(batchLineItem);

                if (batchLineItem.Apttus_Config2__ProductId__r_Apttus_Config2__ProductType__c != Constants.SYSTEM_TYPE_SERVICE)
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

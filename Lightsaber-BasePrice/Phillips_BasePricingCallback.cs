using Apttus.Lightsaber.Extensibility.Framework.Library;
using Apttus.Lightsaber.Pricing.Common.Callback;
using Apttus.Lightsaber.Pricing.Common.Messages;
using Apttus.Lightsaber.Pricing.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhillipsConversion.Lightsaber;
using Apttus.Lightsaber.Pricing.Common.Entities;

namespace PhillipsConversion
{
    public class Phillips_BasePricingCallback : CodeExtensibilityFramework, IPricingBasePriceCallback
    {
        private Lightsaber_BasePriceCallBackHelper_Ultra pcbHelper_Ultra = null;
        private List<LineItemModel> batchLineItems = null;

        public async Task BeforePricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            batchLineItems = batchPriceRequest.LineItems.SelectMany(x => x.ChargeLines).ToList();

            //GP: Need to confirm from the Lightsaber UI integration team on how proposal data will be available in cart, can this Get method used with concrete type to convert it? 
            var proposalSO = batchPriceRequest.Cart.Get<Dictionary<string, object>>("Apttus_Proposal__Proposal__c");
            pcbHelper_Ultra = new Lightsaber_BasePriceCallBackHelper_Ultra(proposalSO, GetDBHelper());

            await pcbHelper_Ultra.populateExtendedQuantity(batchLineItems);
        }

        public async Task OnPricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            var namBundleDictionary = await pcbHelper_Ultra.queryAndPopulateNAMBundleDictionary(batchPriceRequest.CartContext.LineItems.SelectMany(x => x.ChargeLines).ToList());
            var pliSPOODictionary = await pcbHelper_Ultra.populateSPOOPLIDictionary(batchLineItems);
            var pliDictionary = await pcbHelper_Ultra.queryAndPopulatePliCustomFields(batchLineItems);
            var agreementTierDictionary = await pcbHelper_Ultra.queryAndPopulateAgreementTiers(pliDictionary);


            foreach (var batchLineItem in batchLineItems)
            {
                PriceListItemModel priceListItemModel = batchLineItem.GetPriceListItem();
                PriceListItem priceListItemEntity = priceListItemModel.Entity;

                await pcbHelper_Ultra.calculateSPOOPricing(batchLineItem, pliSPOODictionary);

                await pcbHelper_Ultra.calculateExtendedListPriceAndOptionUnitPrice(batchLineItem);

                if (batchLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Apttus_Config2__ProductType__c) != Constants.SYSTEM_TYPE_SERVICE)
                {
                    await pcbHelper_Ultra.populateTier(batchLineItem, pliDictionary, agreementTierDictionary);
                }

                await pcbHelper_Ultra.UpdateNAMBundleOptions(batchLineItem, namBundleDictionary);
            }
        }

        public async Task AfterPricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            await Task.CompletedTask;
        }
    }
}

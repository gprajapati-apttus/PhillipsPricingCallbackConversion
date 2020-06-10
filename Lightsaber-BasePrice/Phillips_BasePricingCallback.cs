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

                var namBundleOptionKey = batchLineItem.Entity.OptionId + batchLineItem.Entity.ProductId;
                
                //NAM Bundle related logic.
                if (batchLineItem.IsOptionLine() && !batchLineItem.Get<bool>(LineItemCustomField.Apttus_Config2__OptionId__r_APTS_Local_Bundle__c) && namBundleDictionary.ContainsKey(namBundleOptionKey))
                {
                    LocalBundleHeaderQueryModel componentSO = namBundleDictionary[namBundleOptionKey];
                    if (componentSO != null)
                    {
                        batchLineItem.Set(LineItemCustomField.APTS_Local_Bundle_Header__c, componentSO.APTS_Local_Bundle_Header__r.Id);
                        batchLineItem.Set(LineItemCustomField.APTS_Local_Bundle_Component__c, componentSO.Id);
                        batchLineItem.Entity.IsQuantityModifiable = false;
                        batchLineItem.Set(LineItemCustomField.APTS_Local_Bundle_Component_Flag__c, true);

                        priceListItemEntity.AllowManualAdjustment = false;
                        priceListItemEntity.AllocateGroupAdjustment = false;
                        priceListItemEntity.ListPrice = 0;
                        priceListItemEntity.MinPrice = 0;

                        batchLineItem.Set(LineItemCustomField.APTS_ContractDiscount__c, null);
                        batchLineItem.Entity.MinPrice = 0;
                        batchLineItem.Set(LineItemCustomField.APTS_Extended_List_Price__c, 0);
                        batchLineItem.Set(LineItemCustomField.APTS_Option_Unit_Price__c, 0);
                    }
                }
            }
        }

        public async Task AfterPricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            await Task.CompletedTask;
        }
    }
}

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
        private const string SYSTEM_TYPE_DEMO = "Demo";
        private const string SYSTEM_TYPE_3RD_PARTY = "3rd Party";
        private const string SYSTEM_TYPE_PHILIPS = "Philips";
        private const string SYSTEM_TYPE_SERVICE = "Service";
        private readonly List<string> listTradeSpoo = new List<string> { "Trade-In", "Trade-In PO", "Trade-In Return" };

        private Lightsaber_CustomPricingCallBackHelper_Ultra pcbHelper_Ultra = null;
        private List<LineItemModel> batchLineItems = null;

        public async Task BeforePricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            batchLineItems = batchPriceRequest.LineItems.SelectMany(x => x.ChargeLines).ToList();

            var proposalSO = batchPriceRequest.Cart.Get<Dictionary<string, object>>("Apttus_Proposal__Proposal__c");
            pcbHelper_Ultra = new Lightsaber_CustomPricingCallBackHelper_Ultra(proposalSO, GetDBHelper());

            await pcbHelper_Ultra.populateExtendedQuantity(batchLineItems);
        }

        public async Task OnPricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            Dictionary<string, PriceListItemQueryModel> pliSPOODictionary = await pcbHelper_Ultra.populateSPOOPLIDictionary(batchLineItems);
            

            foreach (var batchLineItem in batchLineItems)
            {
                PriceListItemModel priceListItemModel = batchLineItem.GetPriceListItem();
                string lineItemSPOOType = batchLineItem.Get<string>(LineItemCustomField.Apttus_Config2__ProductId__r_APTS_SPOO_Type__c);

                if (!string.IsNullOrWhiteSpace(lineItemSPOOType))
                {
                    decimal costPrice, listPriceMultiplier, minPriceMultiplier, targetPriceMultiplier, escPriceMultiplier, listPrice, minPrice, targetPrice, escPrice, fairMarketValue;
                    string systemType, description, spooCategory, valuationClass;

                    listPriceMultiplier = pliSPOODictionary[priceListItemModel.Entity.Id].GetListPriceMultipler();
                    minPriceMultiplier = pliSPOODictionary[priceListItemModel.Entity.Id].GetMinimumPriceMultiplier();
                    targetPriceMultiplier = pliSPOODictionary[priceListItemModel.Entity.Id].GetTargetPriceMultipler();
                    escPriceMultiplier = pliSPOODictionary[priceListItemModel.Entity.Id].GetEscalationPriceMultiplier();

                    costPrice = batchLineItem.GetOrDefault(LineItemCustomField.Apttus_Config2__AttributeValueId__r_APTS_Cost_Price__c, 0);
                    fairMarketValue = batchLineItem.GetOrDefault(LineItemCustomField.Apttus_Config2__AttributeValueId__r_APTS_Fair_Market_Value__c, 0);
                    listPrice = batchLineItem.GetOrDefault(LineItemCustomField.Apttus_Config2__AttributeValueId__r_APTS_List_Price__c, 0);
                    
                    if (lineItemSPOOType == SYSTEM_TYPE_SERVICE)
                    {
                        description = batchLineItem.Get<string>(LineItemCustomField.Apttus_Config2__AttributeValueId__r_APTS_Service_Plan_Name__c);
                    }
                    else
                    {
                        description = batchLineItem.Get<string>(LineItemCustomField.Apttus_Config2__AttributeValueId__r_APTS_Product_Name__c);
                    }

                    spooCategory = batchLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_ProductCode);
                    valuationClass = batchLineItem.Get<string>(LineItemCustomField.Apttus_Config2__ProductId__r_APTS_Valuation_Class__c);

                    if (lineItemSPOOType == SYSTEM_TYPE_3RD_PARTY || lineItemSPOOType == SYSTEM_TYPE_PHILIPS || lineItemSPOOType == SYSTEM_TYPE_DEMO)
                    {
                        priceListItemModel.Entity.ListPrice = costPrice * listPriceMultiplier;
                    }
                    else if (lineItemSPOOType == SYSTEM_TYPE_SERVICE)
                    {
                        priceListItemModel.Entity.ListPrice = listPrice;
                    }
                    else if (listTradeSpoo.Contains(lineItemSPOOType))
                    {  
                        priceListItemModel.Entity.ListPrice = listPrice * -1;
                    }

                    decimal sellingTerm = batchLineItem.GetOrDefault(LineItem.PropertyNames.SellingTerm, 1);
                    lineItem.APTS_Extended_List_Price__c = itemSO.Apttus_Config2__ListPrice__c != null ? formatPrecisionCeiling(itemSO.Apttus_Config2__ListPrice__c) * lineItem.APTS_Extended_Quantity__c * sellingTerm : 0;
                    lineItem.APTS_Option_Unit_Price__c = itemSO.Apttus_Config2__ListPrice__c != null ? (formatPrecisionCeiling(itemSO.Apttus_Config2__ListPrice__c) * lineItem.APTS_Extended_Quantity__c) / lineItem.Apttus_Config2__Quantity__c * sellingTerm : 0;

                    if (listTradeSpoo.Contains(lineItemSPOOType))
                    { 
                        lineItem.APTS_Target_Price_SPOO__c = fairMarketValue * targetPriceMultiplier * -1;
                        lineItem.Apttus_Config2__MinPrice__c = fairMarketValue * minPriceMultiplier * -1;
                        lineItem.APTS_Escalation_Price_SPOO__c = fairMarketValue * escPriceMultiplier * -1;
                    }
                    else
                    {
                        lineItem.APTS_Target_Price_SPOO__c = costPrice * targetPriceMultiplier;
                        lineItem.Apttus_Config2__MinPrice__c = costPrice * minPriceMultiplier;//DE59103 - Updated field, previous: APTS_Minimum_Price_SPOO__c
                        lineItem.APTS_Escalation_Price_SPOO__c = costPrice * escPriceMultiplier;
                    }

                    if (spooCategory != null)
                        lineItem.APTS_Product_ID_SPOO__c = spooCategory;

                    lineItem.APTS_System_Type__c = lineItemSPOOType;

                    if (description != null && !description.equals(''))
                        lineItem.Apttus_Config2__Description__c = description;

                    if (valuationClass != null)
                        lineItem.APTS_Valuation_Class__c = valuationClass;
                }
                else
                {
                    lineItem.APTS_Valuation_Class__c = lineItem.Apttus_Config2__OptionId__c != null && lineItem.Apttus_Config2__OptionId__r.APTS_Valuation_Class__c != null ? lineItem.Apttus_Config2__OptionId__r.APTS_Valuation_Class__c : lineItem.Apttus_Config2__ProductId__r.APTS_Valuation_Class__c;
                }
            }
        }

        public async Task AfterPricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            await Task.CompletedTask;
        }
    }
}

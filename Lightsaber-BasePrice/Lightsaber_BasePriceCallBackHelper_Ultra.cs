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
using System.Linq;

namespace PhillipsConversion
{
    public class Lightsaber_BasePriceCallBackHelper_Ultra
    {
        private Dictionary<string, object> proposal;
        private IDBHelper dBHelper = null;

        public Lightsaber_BasePriceCallBackHelper_Ultra(Dictionary<string, object> proposal, IDBHelper dBHelper)
        {
            this.proposal = proposal;
            this.dBHelper = dBHelper;
        }

        internal async Task populateExtendedQuantity(List<LineItemModel> batchLineItems)
        {
            foreach (LineItemModel batchLineItem in batchLineItems)
            {
                batchLineItem.Set(LineItemCustomField.APTS_Local_Bundle_Header__c, null);
                batchLineItem.Set(LineItemCustomField.APTS_Local_Bundle_Component__c, null);

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

        public async Task<Dictionary<string, LocalBundleHeaderQueryModel>> queryAndPopulateNAMBundleDictionary(List<LineItemModel> batchLineItems)
        {
            HashSet<string> localBundleOptionSet = new HashSet<string>();
            HashSet<decimal> lineNumWithLocalBundleSet = new HashSet<decimal>();
            Dictionary<string, LocalBundleHeaderQueryModel> namBundleDictionary = new Dictionary<string, LocalBundleHeaderQueryModel>();

            foreach (LineItemModel batchLineItem in batchLineItems)
            {
                if (batchLineItem.IsOptionLine() && batchLineItem.Get<bool>(LineItemCustomField.Apttus_Config2__OptionId__r_APTS_Local_Bundle__c))
                {
                    lineNumWithLocalBundleSet.Add(batchLineItem.GetLineNumber());
                    localBundleOptionSet.Add(batchLineItem.Entity.ProductId + batchLineItem.Entity.OptionId);
                }
            }

            var query = QueryHelper.GetNAMBundleQuery(localBundleOptionSet);

            List<LocalBundleHeaderQueryModel> namBundleItems = await dBHelper.FindAsync<LocalBundleHeaderQueryModel>(query);
            foreach (var namBundleItem in namBundleItems)
            {
                namBundleDictionary.Add(namBundleItem.APTS_Component__c + namBundleItem.APTS_Local_Bundle_Header__r.APTS_Parent_Bundle__c, namBundleItem);
            }

            //for (Apttus_Config2.LineItem lineItemMO : allLines)
            //{
            //    Apttus_Config2__LineItem__c liSO = lineItemMO.getLineItemSO();
            //    if (liSO.Apttus_Config2__OptionId__c != null && liSO.Apttus_Config2__OptionId__r.APTS_Local_Bundle__c)
            //    {
            //        lineNumWithLocalBundleSet.add(liSO.Apttus_Config2__LineNumber__c);
            //        localBundleOptionSet.add(liSO.Apttus_Config2__ProductId__c + '' + liSO.Apttus_Config2__OptionId__c);
            //    }
            //}

            //List<APTS_Local_Bundle_Header__c> localBundleHeaderSOList = [select id, APTS_Local_Bundle__c, APTS_Parent_Bundle__c, APTS_Parent_Local_Bundle__c, (select id, APTS_Component__c, APTS_Local_Bundle_Header__c from Local_Bundle_Components__r where APTS_Active__c = TRUE) from APTS_Local_Bundle_Header__c where APTS_Active__c = TRUE AND APTS_Parent_Local_Bundle__c IN: localBundleOptionSet];

            //for (APTS_Local_Bundle_Header__c localBundleHeaderSO : localBundleHeaderSOList)
            //{
            //    List<APTS_Local_Bundle_Component__c> componentSOList = localBundleHeaderSO.Local_Bundle_Components__r;
            //    for (APTS_Local_Bundle_Component__c componentSO : componentSOList)
            //    {
            //        componentSOMap.put(componentSO.APTS_Component__c + '' + localBundleHeaderSO.APTS_Parent_Bundle__c, componentSO);
            //    }
            //}

            return namBundleDictionary;
        }

        public async Task calculateSPOOPricing(LineItemModel batchLineItem, Dictionary<string, PriceListItemQueryModel>  pliSPOODictionary)
        {
            PriceListItemModel priceListItemModel = batchLineItem.GetPriceListItem();
            PriceListItem priceListItemEntity = priceListItemModel.Entity;
            string lineItemSPOOType = batchLineItem.Get<string>(LineItemCustomField.Apttus_Config2__ProductId__r_APTS_SPOO_Type__c);

            if (!string.IsNullOrWhiteSpace(lineItemSPOOType))
            {
                decimal costPrice, listPriceMultiplier, minPriceMultiplier, targetPriceMultiplier, escPriceMultiplier, listPrice, fairMarketValue;
                string description, spooCategory, valuationClass;

                listPriceMultiplier = pliSPOODictionary[priceListItemEntity.Id].GetListPriceMultipler();
                minPriceMultiplier = pliSPOODictionary[priceListItemEntity.Id].GetMinimumPriceMultiplier();
                targetPriceMultiplier = pliSPOODictionary[priceListItemEntity.Id].GetTargetPriceMultipler();
                escPriceMultiplier = pliSPOODictionary[priceListItemEntity.Id].GetEscalationPriceMultiplier();

                costPrice = batchLineItem.GetValuetOrDefault(LineItemCustomField.Apttus_Config2__AttributeValueId__r_APTS_Cost_Price__c, 0);
                fairMarketValue = batchLineItem.GetValuetOrDefault(LineItemCustomField.Apttus_Config2__AttributeValueId__r_APTS_Fair_Market_Value__c, 0);
                listPrice = batchLineItem.GetValuetOrDefault(LineItemCustomField.Apttus_Config2__AttributeValueId__r_APTS_List_Price__c, 0);

                if (lineItemSPOOType == Constants.SYSTEM_TYPE_SERVICE)
                {
                    description = batchLineItem.Get<string>(LineItemCustomField.Apttus_Config2__AttributeValueId__r_APTS_Service_Plan_Name__c);
                }
                else
                {
                    description = batchLineItem.Get<string>(LineItemCustomField.Apttus_Config2__AttributeValueId__r_APTS_Product_Name__c);
                }

                spooCategory = batchLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_ProductCode);
                valuationClass = batchLineItem.Get<string>(LineItemCustomField.Apttus_Config2__ProductId__r_APTS_Valuation_Class__c);

                if (lineItemSPOOType == Constants.SYSTEM_TYPE_3RD_PARTY || lineItemSPOOType == Constants.SYSTEM_TYPE_PHILIPS || lineItemSPOOType == Constants.SYSTEM_TYPE_DEMO)
                {
                    priceListItemEntity.ListPrice = costPrice * listPriceMultiplier;
                }
                else if (lineItemSPOOType == Constants.SYSTEM_TYPE_SERVICE)
                {
                    priceListItemEntity.ListPrice = listPrice;
                }
                else if (Constants.listTradeSpoo.Contains(lineItemSPOOType))
                {
                    priceListItemEntity.ListPrice = listPrice * -1;
                }

                decimal sellingTerm = batchLineItem.GetValuetOrDefault(LineItem.PropertyNames.SellingTerm, 1);
                decimal? extendedListPrice = priceListItemEntity.ListPrice.HasValue
                                                ? formatPrecisionCeiling(priceListItemEntity.ListPrice.Value) * batchLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_Quantity__c) * sellingTerm
                                                : 0;
                decimal? optionUnitPrice = priceListItemEntity.ListPrice.HasValue
                                                ? (formatPrecisionCeiling(priceListItemEntity.ListPrice.Value) * batchLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_Quantity__c)) / batchLineItem.GetQuantity() * sellingTerm
                                                : 0;

                batchLineItem.Set(LineItemCustomField.APTS_Extended_List_Price__c, extendedListPrice);
                batchLineItem.Set(LineItemCustomField.APTS_Option_Unit_Price__c, optionUnitPrice);

                if (Constants.listTradeSpoo.Contains(lineItemSPOOType))
                {
                    batchLineItem.Set(LineItemCustomField.APTS_Target_Price_SPOO__c, fairMarketValue * targetPriceMultiplier * -1);
                    batchLineItem.Entity.MinPrice = fairMarketValue * minPriceMultiplier * -1;
                    batchLineItem.Set(LineItemCustomField.APTS_Escalation_Price_SPOO__c, fairMarketValue * escPriceMultiplier * -1);
                }
                else
                {
                    batchLineItem.Set(LineItemCustomField.APTS_Target_Price_SPOO__c, costPrice * targetPriceMultiplier);
                    batchLineItem.Entity.MinPrice = costPrice * minPriceMultiplier;
                    batchLineItem.Set(LineItemCustomField.APTS_Escalation_Price_SPOO__c, costPrice * escPriceMultiplier);
                }

                if (!string.IsNullOrWhiteSpace(spooCategory))
                {
                    batchLineItem.Set(LineItemCustomField.APTS_Product_ID_SPOO__c, spooCategory);
                }

                batchLineItem.Set(LineItemCustomField.APTS_System_Type__c, lineItemSPOOType);

                if (!string.IsNullOrWhiteSpace(description))
                {
                    batchLineItem.Set(LineItemStandardField.Apttus_Config2__Description__c, description);
                }

                if (!string.IsNullOrWhiteSpace(valuationClass))
                {
                    batchLineItem.Set(LineItemCustomField.APTS_Valuation_Class__c, valuationClass);
                }
            }
            else
            {
                string valuationClass = batchLineItem.IsOptionLine() && !string.IsNullOrWhiteSpace(batchLineItem.Get<string>(LineItemCustomField.Apttus_Config2__OptionId__r_APTS_Valuation_Class__c))
                    ? batchLineItem.Get<string>(LineItemCustomField.Apttus_Config2__OptionId__r_APTS_Valuation_Class__c)
                    : batchLineItem.Get<string>(LineItemCustomField.Apttus_Config2__ProductId__r_APTS_Valuation_Class__c);

                batchLineItem.Set(LineItemCustomField.APTS_Valuation_Class__c, valuationClass);
            }

            await Task.CompletedTask;
        }

        public async Task calculateExtendedListPriceAndOptionUnitPrice(LineItemModel batchLineItem)
        {
            PriceListItemModel priceListItemModel = batchLineItem.GetPriceListItem();
            PriceListItem priceListItemEntity = priceListItemModel.Entity;

            if (batchLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Apttus_Config2__ProductType__c) == Constants.SYSTEM_TYPE_SERVICE)
            {
                var serviceListPrice = batchLineItem.Get<decimal?>(LineItemCustomField.APTS_Service_List_Price__c);
                var minimumServicePrice = batchLineItem.Get<decimal?>(LineItemCustomField.APTS_Minimum_Price_Service__c);
                var serviceCost = batchLineItem.Get<decimal?>(LineItemCustomField.APTS_Cost_Service__c);

                if (serviceListPrice.HasValue)
                {
                    priceListItemEntity.ListPrice = serviceListPrice.Value;
                }

                if (minimumServicePrice.HasValue)
                {
                    batchLineItem.Entity.MinPrice = minimumServicePrice.Value;
                }

                if (serviceCost.HasValue)
                {
                    priceListItemEntity.Cost = serviceCost.Value;
                    batchLineItem.Entity.BaseCost = serviceCost.Value;
                }

                decimal sellingTerm = batchLineItem.GetValuetOrDefault(LineItem.PropertyNames.SellingTerm, 1);
                decimal sapListPrice = batchLineItem.GetValuetOrDefault(LineItemCustomField.APTS_Service_List_Price__c, 0);
                decimal? extPrice = sapListPrice * batchLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_Quantity__c) * sellingTerm;
                batchLineItem.Set(LineItemCustomField.APTS_Extended_List_Price__c, extPrice);

                decimal lineExtQ = batchLineItem.GetValuetOrDefault(LineItemCustomField.APTS_Extended_Quantity__c, 0);
                decimal lineQ = batchLineItem.GetQuantity();
                batchLineItem.Set(LineItemCustomField.APTS_Option_Unit_Price__c, (sapListPrice * lineExtQ) / lineQ);
            }
            else
            {

                decimal sellingTerm = batchLineItem.GetValuetOrDefault(LineItem.PropertyNames.SellingTerm, 1);
                decimal? sapListPrice = priceListItemEntity.ListPrice;
                decimal lineExtQ = batchLineItem.GetValuetOrDefault(LineItemCustomField.APTS_Extended_Quantity__c, 0);
                decimal? extPrice = sapListPrice * lineExtQ * sellingTerm;
                batchLineItem.Set(LineItemCustomField.APTS_Extended_List_Price__c, extPrice);

                decimal lineQ = batchLineItem.GetQuantity();
                batchLineItem.Set(LineItemCustomField.APTS_Option_Unit_Price__c, (sapListPrice * lineExtQ) / lineQ);
            }

            await Task.CompletedTask;
        }

        public async Task populateTier(LineItemModel batchLineItem, Dictionary<string, PriceListItemQueryModel> pliDictionary, Dictionary<string, string> agreementTierDictionary)
        
        {
            PriceListItemModel priceListItemModel = batchLineItem.GetPriceListItem();
            PriceListItem priceListItemEntity = priceListItemModel.Entity;

            if (pliDictionary.Count > 0 && pliDictionary.ContainsKey(priceListItemEntity.Id))
            {
                string agreementGroup = pliDictionary[priceListItemEntity.Id].APTS_Agreement_Group__c;

                if (!string.IsNullOrWhiteSpace(agreementGroup) && agreementTierDictionary[agreementGroup] != null)
                {
                    var strVolumeTier = agreementTierDictionary[agreementGroup];
                    batchLineItem.Set(LineItemCustomField.APTS_VolumeTier__c, strVolumeTier);

                    priceListItemEntity.ListPrice = (Constants.TIER_1 == strVolumeTier ? pliDictionary[priceListItemEntity.Id].APTS_Tier_1_List_Price__c
                                                          : Constants.TIER_2 == strVolumeTier ? pliDictionary[priceListItemEntity.Id].APTS_Tier_2_List_Price__c
                                                          : Constants.TIER_3 == strVolumeTier ? pliDictionary[priceListItemEntity.Id].APTS_Tier_3_List_Price__c
                                                          : Constants.TIER_4 == strVolumeTier ? pliDictionary[priceListItemEntity.Id].APTS_Tier_4_List_Price__c
                                                          : null);

                    batchLineItem.Set(LineItemCustomField.APTS_ContractDiscount__c, (Constants.TIER_1 == strVolumeTier ? pliDictionary[priceListItemEntity.Id].APTS_Tier_1_Discount__c
                                                    : Constants.TIER_2 == strVolumeTier ? pliDictionary[priceListItemEntity.Id].APTS_Tier_2_Discount__c
                                                    : Constants.TIER_3 == strVolumeTier ? pliDictionary[priceListItemEntity.Id].APTS_Tier_3_Discount__c
                                                    : Constants.TIER_4 == strVolumeTier ? pliDictionary[priceListItemEntity.Id].APTS_Tier_4_Discount__c
                                                    : null));
                }
            }
            else
            {
                //Clearing contract related fields. 
                batchLineItem.Set(LineItemCustomField.APTS_VolumeTier__c, null);
                batchLineItem.Set(LineItemCustomField.APTS_ContractDiscount__c, null);
            }

            await Task.CompletedTask;
        }

        public async Task<Dictionary<string, PriceListItemQueryModel>> queryAndPopulatePliCustomFields(List<LineItemModel> lineItems)
        {
            Dictionary<string, PriceListItemQueryModel> pliDictionary = new Dictionary<string, PriceListItemQueryModel>();
            HashSet<string> prodIds = new HashSet<string>();
            HashSet<string> contractNumberSet = new HashSet<string>();
            var priceListItemIdSet = lineItems.Select(li => li.GetPriceListItem().Entity.Id).ToHashSet();

            if (proposal["Account_Sold_to__c"] != null)
            {
                var pliTierQuery = QueryHelper.GetPLITierQuery(priceListItemIdSet);

                List<PriceListItemQueryModel> pliTierDetails = await dBHelper.FindAsync<PriceListItemQueryModel>(pliTierQuery);
                foreach (var pliTierDetail in pliTierDetails)
                {
                    pliDictionary.Add(pliTierDetail.Id, pliTierDetail);
                }
            }

            return pliDictionary;
        }

        public async Task<Dictionary<string, string>> queryAndPopulateAgreementTiers(Dictionary<string, PriceListItemQueryModel> pliDictionary)
        {
            Dictionary<string, string> agreementTierDictionary = new Dictionary<string, string>();
            
            if (proposal["Account_Sold_to__c"] != null)
            {
                HashSet<string> pliRelatedAgreementSet = pliDictionary.Values.Where(pli => pli.Apttus_Config2__PriceListId__r.APTS_Related_Agreement__c != null).
                                                        Select(pli => pli.Apttus_Config2__PriceListId__r.APTS_Related_Agreement__c).ToHashSet();

                var agreementTierQuery = QueryHelper.GetAgreementTierQuery(pliRelatedAgreementSet, proposal["Account_Sold_to__c"] as string);

                List<AccountContracttQueryModel> agreementTierDetails = await dBHelper.FindAsync<AccountContracttQueryModel>(agreementTierQuery);
                foreach (var agreementTierDetail in agreementTierDetails)
                {
                    agreementTierDictionary.Add(agreementTierDetail.APTS_Agreement_Group__c, agreementTierDetail.APTS_Volume_Tier__c);
                }
            }

            return agreementTierDictionary;
        }

        public decimal formatPrecisionCeiling(decimal fieldValue)
        {
            var result = PricingHelper.ApplyRounding(fieldValue, 2, RoundingMode.UP);
            return result.Value;
        }
    }
}

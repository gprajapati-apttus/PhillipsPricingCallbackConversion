using Apttus.Lightsaber.Extensibility.Framework.Library.Implementation;
using Apttus.Lightsaber.Extensibility.Framework.Library.Interfaces;
using Apttus.Lightsaber.Pricing.Common.Constants;
using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Pricing.Common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apttus.Lightsaber.Phillips.Pricing
{
    public class PricingBasePriceCallbackHelper
    {
        private readonly Dictionary<string, object> proposal = null;
        HashSet<decimal> lineNumWithLocalBundleSet = new HashSet<decimal>();
        private readonly IDBHelper dBHelper = null;
        private readonly IPricingHelper pricingHelper = null;

        public PricingBasePriceCallbackHelper(Dictionary<string, object> proposal, IDBHelper dBHelper, IPricingHelper pricingHelper)
        {
            this.proposal = proposal;
            this.dBHelper = dBHelper;
            this.pricingHelper = pricingHelper;
        }

        internal async Task PopulateExtendedQuantity(List<LineItemModel> batchLineItems)
        {
            foreach (LineItemModel batchLineItem in batchLineItems)
            {
                batchLineItem.Set(LineItemCustomField.APTS_Local_Bundle_Header__c, null);
                batchLineItem.Set(LineItemCustomField.APTS_Local_Bundle_Component__c, null);

                decimal extendedQuantity = batchLineItem.GetQuantity();
                decimal quantity = batchLineItem.GetQuantity();

                if (batchLineItem.IsOptionLine())
                {
                    LineItemModel rootBundleLineItemModel = batchLineItem.GetRootParentLineItem().GetPrimaryLineItem();
                    decimal bundleQuantity = rootBundleLineItemModel.GetQuantity();
                    quantity = bundleQuantity;
                    extendedQuantity = bundleQuantity * batchLineItem.GetQuantity();

                    batchLineItem.Entity.IsOptional = rootBundleLineItemModel.IsOptional();
                }

                batchLineItem.Set(LineItemCustomField.APTS_Extended_Quantity__c, extendedQuantity);
                batchLineItem.Set(LineItemCustomField.APTS_Bundle_Quantity__c, quantity);
            }

            await Task.CompletedTask;
        }

        internal async Task<Dictionary<string, PriceListItemQueryModel>> PopulateSPOOPLIDictionary(List<LineItemModel> batchLineItems)
        {
            HashSet<string> spooProdIds = new HashSet<string>();
            Dictionary<string, PriceListItemQueryModel>  pliSPOODictionary = new Dictionary<string, PriceListItemQueryModel>();

            foreach (LineItemModel batchLineItem in batchLineItems)
            {
                if (!string.IsNullOrWhiteSpace(batchLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_APTS_SPOO_Type__c)))
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

        public async Task<Dictionary<string, LocalBundleHeaderQueryModel>> QueryAndPopulateNAMBundleDictionary(List<LineItemModel> cartLineItems)
        {
            HashSet<string> localBundleOptionSet = new HashSet<string>();
            Dictionary<string, LocalBundleHeaderQueryModel> namBundleDictionary = new Dictionary<string, LocalBundleHeaderQueryModel>();

            foreach (LineItemModel cartLineItem in cartLineItems)
            {
                if (cartLineItem.IsOptionLine() && cartLineItem.GetLookupValue<bool>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_APTS_Local_Bundle__c))
                {
                    lineNumWithLocalBundleSet.Add(cartLineItem.GetLineNumber());
                    localBundleOptionSet.Add(cartLineItem.Entity.ProductId + cartLineItem.Entity.OptionId);
                }
            }

            var query = QueryHelper.GetNAMBundleQuery(localBundleOptionSet);

            List<LocalBundleHeaderQueryModel> namBundleItems = await dBHelper.FindAsync<LocalBundleHeaderQueryModel>(query);
            foreach (var namBundleItem in namBundleItems)
            {
                namBundleDictionary.Add(namBundleItem.APTS_Component__c + namBundleItem.APTS_Local_Bundle_Header__r.APTS_Parent_Bundle__c, namBundleItem);
            }

            return namBundleDictionary;
        }

        public async Task UpdateNAMBundleOptions(LineItemModel batchLineItem, Dictionary<string, LocalBundleHeaderQueryModel> namBundleDictionary)
        {
            if (batchLineItem.IsOptionLine() && !batchLineItem.GetLookupValue<bool>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_APTS_Local_Bundle__c) && lineNumWithLocalBundleSet.Contains(batchLineItem.GetLineNumber()))
            {
                var namBundleOptionKey = batchLineItem.Entity.OptionId + batchLineItem.Entity.ProductId;
                var priceListItemEntity = batchLineItem.GetPriceListItem().Entity;
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

            await Task.CompletedTask;
        }

        public async Task CalculateSPOOPricing(LineItemModel batchLineItem, Dictionary<string, PriceListItemQueryModel>  pliSPOODictionary)
        {
            PriceListItemModel priceListItemModel = batchLineItem.GetPriceListItem();
            PriceListItem priceListItemEntity = priceListItemModel.Entity;
            string lineItemSPOOType = batchLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_APTS_SPOO_Type__c);

            if (!string.IsNullOrWhiteSpace(lineItemSPOOType))
            {
                decimal costPrice, listPriceMultiplier, minPriceMultiplier, targetPriceMultiplier, escPriceMultiplier, listPrice, fairMarketValue;
                string description, spooCategory, valuationClass;

                listPriceMultiplier = pliSPOODictionary[priceListItemEntity.Id].GetListPriceMultipler();
                minPriceMultiplier = pliSPOODictionary[priceListItemEntity.Id].GetMinimumPriceMultiplier();
                targetPriceMultiplier = pliSPOODictionary[priceListItemEntity.Id].GetTargetPriceMultipler();
                escPriceMultiplier = pliSPOODictionary[priceListItemEntity.Id].GetEscalationPriceMultiplier();

                costPrice = batchLineItem.GetValuetOrDefault(LineItemStandardRelationshipField.Apttus_Config2__AttributeValueId__r_APTS_Cost_Price__c, 0);
                fairMarketValue = batchLineItem.GetValuetOrDefault(LineItemStandardRelationshipField.Apttus_Config2__AttributeValueId__r_APTS_Fair_Market_Value__c, 0);
                listPrice = batchLineItem.GetValuetOrDefault(LineItemStandardRelationshipField.Apttus_Config2__AttributeValueId__r_APTS_List_Price__c, 0);

                if (lineItemSPOOType == Constants.SYSTEM_TYPE_SERVICE)
                {
                    description = batchLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__AttributeValueId__r_APTS_Service_Plan_Name__c);
                }
                else
                {
                    description = batchLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__AttributeValueId__r_APTS_Product_Name__c);
                }

                spooCategory = batchLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_ProductCode);
                valuationClass = batchLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_APTS_Valuation_Class__c);

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
                                                ? FormatPrecisionCeiling(priceListItemEntity.ListPrice.Value) * batchLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_Quantity__c) * sellingTerm
                                                : 0;
                decimal? optionUnitPrice = priceListItemEntity.ListPrice.HasValue
                                                ? (FormatPrecisionCeiling(priceListItemEntity.ListPrice.Value) * batchLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_Quantity__c)) / batchLineItem.GetQuantity() * sellingTerm
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
                string valuationClass = batchLineItem.IsOptionLine() && !string.IsNullOrWhiteSpace(batchLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_APTS_Valuation_Class__c))
                    ? batchLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_APTS_Valuation_Class__c)
                    : batchLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_APTS_Valuation_Class__c);

                batchLineItem.Set(LineItemCustomField.APTS_Valuation_Class__c, valuationClass);
            }

            await Task.CompletedTask;
        }

        public async Task CalculateExtendedListPriceAndOptionUnitPrice(LineItemModel batchLineItem)
        {
            PriceListItemModel priceListItemModel = batchLineItem.GetPriceListItem();
            PriceListItem priceListItemEntity = priceListItemModel.Entity;

            if (batchLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Apttus_Config2__ProductType__c) == Constants.SYSTEM_TYPE_SERVICE)
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

        public async Task PopulateTier(LineItemModel batchLineItem, Dictionary<string, PriceListItemQueryModel> pliDictionary, Dictionary<string, string> agreementTierDictionary)
        
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

        public async Task<Dictionary<string, PriceListItemQueryModel>> QueryAndPopulatePliCustomFields(List<LineItemModel> lineItems)
        {
            Dictionary<string, PriceListItemQueryModel> pliDictionary = new Dictionary<string, PriceListItemQueryModel>();
            HashSet<string> prodIds = new HashSet<string>();
            HashSet<string> contractNumberSet = new HashSet<string>();
            var priceListItemIdSet = lineItems.Select(li => li.GetPriceListItem().Entity.Id).ToHashSet();

            if (proposal[ProposalField.Account_Sold_to__c] != null)
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

        public async Task<Dictionary<string, string>> QueryAndPopulateAgreementTiers(Dictionary<string, PriceListItemQueryModel> pliDictionary)
        {
            Dictionary<string, string> agreementTierDictionary = new Dictionary<string, string>();
            
            if (proposal[ProposalField.Account_Sold_to__c] != null)
            {
                HashSet<string> pliRelatedAgreementSet = pliDictionary.Values.Where(pli => pli.Apttus_Config2__PriceListId__r.APTS_Related_Agreement__c != null).
                                                        Select(pli => pli.Apttus_Config2__PriceListId__r.APTS_Related_Agreement__c).ToHashSet();

                List<AccountContractQueryModel> agreementTierDetails = await QueryHelper.ExecuteAgreementTierQuery(dBHelper, pliRelatedAgreementSet, proposal[ProposalField.Account_Sold_to__c] as string);
                foreach (var agreementTierDetail in agreementTierDetails)
                {
                    agreementTierDictionary.Add(agreementTierDetail.APTS_Agreement_Group__c, agreementTierDetail.APTS_Volume_Tier__c);
                }
            }

            return agreementTierDictionary;
        }

        private decimal FormatPrecisionCeiling(decimal fieldValue)
        {
            var result = pricingHelper.ApplyRounding(fieldValue, 2, RoundingMode.UP);
            return result.Value;
        }
    }
}

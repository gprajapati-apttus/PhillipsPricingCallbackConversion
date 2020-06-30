using Apttus.Lightsaber.Extensibility.Framework.Library;
using Apttus.Lightsaber.Extensibility.Framework.Library.Interfaces;
using Apttus.Lightsaber.Pricing.Common.Callback;
using Apttus.Lightsaber.Pricing.Common.Constants;
using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Pricing.Common.Formula;
using Apttus.Lightsaber.Pricing.Common.Messages;
using Apttus.Lightsaber.Pricing.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apttus.Lightsaber.Nokia.Pricing
{
    public class PricingBasePriceCallback : CodeExtensibility, IPricingBasePriceCallback
    {
        private List<LineItemModel> batchLineItems = null;
        private List<LineItemModel> cartLineItems = null;
        private Dictionary<string, LineItemModel> lineItemObjectMap = new Dictionary<string, LineItemModel>();
        private Proposal proposal = null;
        private IDBHelper dBHelper = null;
        private IPricingHelper pricingHelper = null;

        public async Task BeforePricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            batchLineItems = batchPriceRequest.LineItems.SelectMany(x => x.ChargeLines).ToList();
            cartLineItems = batchPriceRequest.CartContext.LineItems.SelectMany(x => x.ChargeLines).ToList();

            proposal = new Proposal(batchPriceRequest.Cart.Get<Dictionary<string, object>>(Constants.PROPOSAL));
            dBHelper = GetDBHelper();
            pricingHelper = GetPricingHelper();

            foreach (var lineItem in cartLineItems)
            {
                if (lineItem.GetLineType() == LineType.ProductService)
                {
                    lineItemObjectMap.Add(Constants.NOKIA_PRODUCT_SERVICES + Constants.NOKIA_UNDERSCORE + lineItem.GetLineNumber(), lineItem);
                }
            }

            if (Constants.QUOTE_TYPE_INDIRECTCPQ.equalsIgnoreCase(proposal.Quote_Type__c))
            {
                foreach (var batchLineItem in batchLineItems)
                {
                    if (batchLineItem.Entity.BasePrice.HasValue && batchLineItem.Entity.PriceIncludedInBundle == false &&
                        batchLineItem.Entity.BasePrice.Value != pricingHelper.ApplyRounding(batchLineItem.Entity.BasePrice.Value, 2, RoundingMode.HALF_UP))
                    {
                        //First Time
                        batchLineItem.Entity.BasePriceOverride = pricingHelper.ApplyRounding(batchLineItem.Entity.BasePrice.Value, 2, RoundingMode.HALF_UP);
                        batchLineItem.Entity.BasePrice = pricingHelper.ApplyRounding(batchLineItem.Entity.BasePrice.Value, 2, RoundingMode.HALF_UP);
                    }

                    string partNumber = getPartNumber(batchLineItem);

                    if (partNumber != null && partNumber.equalsIgnoreCase(Constants.MAINTY2CODE))
                    {
                        batchLineItem.Entity.LineSequence = 997;
                    }
                    else if (partNumber != null && partNumber.equalsIgnoreCase(Constants.MAINTY1CODE))
                    {
                        batchLineItem.Entity.LineSequence = 996;
                    }
                    else if (partNumber != null && partNumber.equalsIgnoreCase(Constants.SSPCODE))
                    {
                        batchLineItem.Entity.LineSequence = 998;
                    }
                    else if (partNumber != null && partNumber.equalsIgnoreCase(Constants.SRS))
                    {
                        batchLineItem.Entity.LineSequence = 999;
                    }
                }
            }

            foreach (var cartLineItem in cartLineItems)
            {
                PriceListItemModel priceListItemModel = cartLineItem.GetPriceListItem();
                PriceListItem priceListItemEntity = priceListItemModel.Entity;

                if (Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Quote_Type__c))
                {

                }
            }

            await Task.CompletedTask;
        }

        public async Task OnPricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            //IsLeo()
            //Is1Year()

            decimal? defaultExchangeRate = null;
            List<string> productList = new List<string>();
            Dictionary<string, decimal?> productPriceMap = new Dictionary<string, decimal?>();
            Dictionary<string, decimal?> productCostMap = new Dictionary<string, decimal?>();
            HashSet<string> ProductId_set = new HashSet<string>();
            Dictionary<string, ProductExtensionsQueryModel> productExtensitonMap = new Dictionary<string, ProductExtensionsQueryModel>();
            List<MNDirectProductMapQueryModel> MN_Direct_Products_List = new List<MNDirectProductMapQueryModel>();
            List<DirectPortfolioGeneralSettingQueryModel> portfolioSettingList = new List<DirectPortfolioGeneralSettingQueryModel>();
            Dictionary<string, MaintenanceAndSSPRuleQueryModel> maintenanceSSPRuleMap_EP = new Dictionary<string, MaintenanceAndSSPRuleQueryModel>();
            Dictionary<string, List<NokiaMaintenanceAndSSPRulesQueryModel>> maintenanceSSPRuleMap = new Dictionary<string, List<NokiaMaintenanceAndSSPRulesQueryModel>>();
            Dictionary<string, List<decimal?>> tierDiscountRuleMap = new Dictionary<string, List<decimal?>>();
            List<SSPSRSDefaultValuesQueryModel> sspSRSDefaultsList = new List<SSPSRSDefaultValuesQueryModel>();
            Dictionary<string, string> mapPliType = new Dictionary<string, string>();

            foreach (var batchLineItem in batchLineItems)
            {
                PriceListItemModel priceListItemModel = batchLineItem.GetPriceListItem();
                PriceListItem priceListItemEntity = priceListItemModel.Entity;

                if (Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Quote_Type__c))
                {
                    productList.Add(batchLineItem.GetProductOrOptionId());

                    if (proposal.NokiaCPQ_Portfolio__c == Constants.QTC || 
                        (proposal.NokiaCPQ_Portfolio__c == Constants.NOKIA_IP_ROUTING && 
                        proposal.Is_List_Price_Only__c == false))
                    {
                        ProductId_set.Add(batchLineItem.GetProductOrOptionId());
                    }
                }
            }

            if (Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Quote_Type__c))
            {
                var defaultExchangeRateQuery = QueryHelper.GetDefaultExchangeRateQuery(proposal.CurrencyIsoCode);
                defaultExchangeRate = (await dBHelper.FindAsync<CurrencyTypeQueryModel>(defaultExchangeRateQuery)).FirstOrDefault()?.ConversionRate;

                var productExtensionsQuery = QueryHelper.GetProductExtensionsQuery(ProductId_set, proposal.CurrencyIsoCode);
                List<ProductExtensionsQueryModel> Prod_extList = await dBHelper.FindAsync<ProductExtensionsQueryModel>(productExtensionsQuery);

                foreach (ProductExtensionsQueryModel Prod_ext in Prod_extList)
                {
                    productExtensitonMap.Add(Prod_ext.Product__c, Prod_ext);
                }

                if (Constants.AIRSCALE_WIFI_STRING.equalsIgnoreCase(proposal.NokiaCPQ_Portfolio__c))
                {
                    var mnDirectProductMapQuery = QueryHelper.GetMNDirectProductMapQuery();
                    MN_Direct_Products_List = await dBHelper.FindAsync<MNDirectProductMapQueryModel>(productExtensionsQuery);
                }

                var directPortfolioGeneralSettingQuery = QueryHelper.GetDirectPortfolioGeneralSettingQuery(proposal.NokiaCPQ_Portfolio__c);
                portfolioSettingList = await dBHelper.FindAsync<DirectPortfolioGeneralSettingQueryModel>(directPortfolioGeneralSettingQuery);

                if (Constants.NOKIA_IP_ROUTING.equalsIgnoreCase(proposal.NokiaCPQ_Portfolio__c) && proposal.Is_List_Price_Only__c == false)
                {
                    var maintenanceAndSSPRuleQuery = QueryHelper.GetMaintenanceAndSSPRuleQuery(
                        proposal.Apttus_Proposal__Account__r_GEOLevel1ID__c, proposal.NokiaCPQ_Maintenance_Type__c);
                    var maintenanceSSPRuleList = await dBHelper.FindAsync<MaintenanceAndSSPRuleQueryModel>(maintenanceAndSSPRuleQuery);

                    foreach(var maintenanceSSPRule in maintenanceSSPRuleList)
                    {
                        if (maintenanceSSPRule.Maintenance_Type__c != null && maintenanceSSPRule.Maintenance_Category__c != null)
                        {
                            var key = maintenanceSSPRule.Maintenance_Type__c + Constants.NOKIA_STRING_APPENDER + maintenanceSSPRule.Maintenance_Category__c;
                            maintenanceSSPRuleMap_EP.Add(key, maintenanceSSPRule);
                        }
                    }
                }
            }

            if (Constants.QUOTE_TYPE_INDIRECTCPQ.equalsIgnoreCase(proposal.Quote_Type__c))
            {
                var nokiaMaintenanceAndSSPRulesQuery = QueryHelper.GetNokiaMaintenanceAndSSPRulesQuery(proposal);
                var nokiaMaintenanceSSPRules = await dBHelper.FindAsync<NokiaMaintenanceAndSSPRulesQueryModel>(nokiaMaintenanceAndSSPRulesQuery);

                foreach (var nokiaMaintenanceSSPRule in nokiaMaintenanceSSPRules)
                {
                    if (nokiaMaintenanceSSPRule.NokiaCPQ_Pricing_Cluster__c.equalsIgnoreCase(proposal.NokiaCPQ_Maintenance_Accreditation__r_Pricing_Cluster__c) 
                        || nokiaMaintenanceSSPRule.NokiaCPQ_Pricing_Cluster__c.equalsIgnoreCase(proposal.NokiaProductAccreditation__r_Pricing_Cluster__c))
                    {
                        var key = nokiaMaintenanceSSPRule.NokiaCPQ_Pricing_Cluster__c + Constants.NOKIA_STRING_APPENDER + nokiaMaintenanceSSPRule.NokiaCPQ_Product_Discount_Category__c + 
                            Constants.NOKIA_STRING_APPENDER + nokiaMaintenanceSSPRule.NokiaCPQ_withPMA__c + Constants.NOKIA_STRING_APPENDER + nokiaMaintenanceSSPRule.NokiaCPQ_Maintenance_Type__c;

                        if (maintenanceSSPRuleMap.ContainsKey(key))
                        {
                            maintenanceSSPRuleMap[key].Add(nokiaMaintenanceSSPRule);
                        }
                        else
                        {
                            maintenanceSSPRuleMap.Add(key, new List<NokiaMaintenanceAndSSPRulesQueryModel> { nokiaMaintenanceSSPRule });
                        }
                    }
                }

                var tierDiscountDetailQuery = QueryHelper.GetTierDiscountDetailQuery(proposal.Apttus_Proposal__Account__r_Partner_Program__c,
                    proposal.Apttus_Proposal__Account__r_Partner_Type__c);

                var tierDiscountDetailQueryModels = await dBHelper.FindAsync<TierDiscountDetailQueryModel>(tierDiscountDetailQuery);

                foreach(var tierDiscountDetailQueryModel in tierDiscountDetailQueryModels)
                {
                    var key = tierDiscountDetailQueryModel.NokiaCPQ_Tier_Type__c + Constants.NOKIA_STRING_APPENDER +
                        tierDiscountDetailQueryModel.NokiaCPQ_Partner_Type__c + Constants.NOKIA_STRING_APPENDER +
                        tierDiscountDetailQueryModel.NokiaCPQ_Pricing_Tier__c + Constants.NOKIA_STRING_APPENDER +
                        tierDiscountDetailQueryModel.Nokia_CPQ_Partner_Program__c;

                    List<decimal?> tierDiscountRuleList = new List<decimal?>();
                    tierDiscountRuleList.Add(tierDiscountDetailQueryModel.NokiaCPQ_Tier_Discount__c);

                    tierDiscountRuleMap.Add(key, tierDiscountRuleList);
                }

                var sspSRSDefaultValuesQuery = QueryHelper.GetSSPSRSDefaultValuesQuery(proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c));
                sspSRSDefaultsList = await dBHelper.FindAsync<SSPSRSDefaultValuesQueryModel>(sspSRSDefaultValuesQuery);

                var portfolio = proposal.NokiaCPQ_Portfolio__c;

                if ((portfolio.equalsIgnoreCase(Constants.NOKIA_FIXED_ACCESS_POL) ||
                    portfolio.equalsIgnoreCase(Constants.NOKIA_FIXED_ACCESS_FBA)) ||
                    portfolio.equalsIgnoreCase(Constants.Nokia_FASTMILE))
                {

                    var indirectMarketPriceListQuery = QueryHelper.GetIndirectMarketPriceListQuery();
                    var priceListCollection = await dBHelper.FindAsync<PriceListQueryModel>(indirectMarketPriceListQuery);

                    foreach(var priceList in priceListCollection)
                    {
                        mapPliType.Add(priceList.Id, priceList.PriceList_Type__c);
                    }
                }
            }

            var countryPriceListItemQuery = QueryHelper.GetCountryPriceListItemQuery(productList);
            var countryPriceListItems = await dBHelper.FindAsync<CountryPriceListItemQueryModel>(countryPriceListItemQuery);

            foreach (CountryPriceListItemQueryModel pli in countryPriceListItems)
            {
                productPriceMap.Add(pli.Apttus_Config2__ProductId__c, pli.Apttus_Config2__ListPrice__c);
                productCostMap.Add(pli.Apttus_Config2__ProductId__c, pli.Apttus_Config2__Cost__c);
            }

            //GP: OnPriceItemSet method
            foreach (var batchLineItem in batchLineItems)
            {
                PriceListItemModel priceListItemModel = batchLineItem.GetPriceListItem();
                PriceListItem priceListItemEntity = priceListItemModel.Entity;

                string partNumber = getPartNumber(batchLineItem);
                string productDiscountCat = getProductDiscountCategory(batchLineItem);
                string configType = getConfigType(batchLineItem);

                if (Constants.QUOTE_TYPE_INDIRECTCPQ.equalsIgnoreCase(proposal.Quote_Type__c) ||
                    Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Quote_Type__c))
                {
                    batchLineItem.Set(LineItemCustomField.NokiaCPQ_Account_Region__c, proposal.Apttus_Proposal__Account__r_GEOLevel1ID__c);
                }

                if (Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Quote_Type__c))
                {
                    //Replacing item.Portfolio_from_Quote_Line_Item__c with 'proposal.NokiaCPQ_Portfolio__c'
                    if (proposal.NokiaCPQ_Portfolio__c == Constants.AIRSCALE_WIFI_STRING
                       && configType == Constants.NOKIA_STANDALONE &&
                       batchLineItem.GetLineType() == LineType.Option)
                    {
                        batchLineItem.Set(LineItemCustomField.NokiaCPQ_Is_SI__c, true);
                    }

                    //R-6456,6667 update QTC Line Item with Price point info from Product Extension and BG, BU info from Product2
                    if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase("QTC"))
                    {
                        batchLineItem.Set(LineItemCustomField.NokiaCPQ_Org__c, proposal.Apttus_Proposal__Account__r_L7Name__c);
                        batchLineItem.Set(LineItemCustomField.NokiaCPQ_BU__c, batchLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Family));
                        batchLineItem.Set(LineItemCustomField.NokiaCPQ_BG__c, batchLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Business_Group__c));

                        if (productExtensitonMap.ContainsKey(batchLineItem.Entity.ProductId) && productExtensitonMap[batchLineItem.Entity.ProductId] != null)
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Custom_Bid__c, productExtensitonMap[batchLineItem.Entity.ProductId].Custom_Bid__c);
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Floor_Price__c, productExtensitonMap[batchLineItem.Entity.ProductId].Floor_Price__c == null ? null : productExtensitonMap[batchLineItem.Entity.ProductId].Floor_Price__c);
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Market_Price__c, productExtensitonMap[batchLineItem.Entity.ProductId].Market_Price__c == null ? null : productExtensitonMap[batchLineItem.Entity.ProductId].Market_Price__c);
                            batchLineItem.Set(LineItemCustomField.Product_Extension__c, productExtensitonMap[batchLineItem.Entity.ProductId].Id);
                        }
                        else
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Floor_Price__c, null);
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Market_Price__c, null);
                        }
                    }

                    //R-6508,6510 Logic to stamp Maint Y1, Y2, SSP, SRS rates onto line item
                    ////R-6500 update Enterprise Line Item with FLoor Price info from Product Extension
                    if (proposal.Is_List_Price_Only__c != null && proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_IP_ROUTING))
                    {
                        batchLineItem.Set(LineItemCustomField.Is_List_Price_Only__c, proposal.Is_List_Price_Only__c);
                        if (proposal.Is_List_Price_Only__c == false)
                        {
                            if (batchLineItem.GetLineType() == LineType.ProductService &&
                                productExtensitonMap.ContainsKey(batchLineItem.Entity.ProductId) &&
                                productExtensitonMap[batchLineItem.Entity.ProductId] != null &&
                                productExtensitonMap[batchLineItem.Entity.ProductId].Floor_Price__c != null)
                            {
                                batchLineItem.Set(LineItemCustomField.NokiaCPQ_Floor_Price__c, productExtensitonMap[batchLineItem.Entity.ProductId].Floor_Price__c == null ? null : productExtensitonMap[batchLineItem.Entity.ProductId].Floor_Price__c);
                            }
                            else if (batchLineItem.GetLineType() == LineType.Option &&
                                productExtensitonMap.ContainsKey(batchLineItem.Entity.OptionId) &&
                                productExtensitonMap[batchLineItem.Entity.OptionId] != null &&
                         productExtensitonMap[batchLineItem.Entity.OptionId].Floor_Price__c != null)
                            {
                                batchLineItem.Set(LineItemCustomField.NokiaCPQ_Floor_Price__c, productExtensitonMap[batchLineItem.Entity.OptionId].Floor_Price__c == null ? null : productExtensitonMap[batchLineItem.Entity.OptionId].Floor_Price__c);
                            }
                            else
                            {
                                batchLineItem.Set(LineItemCustomField.NokiaCPQ_Floor_Price__c, null);
                            }
                        }
                    }

                    //GP: Start from here
                    var nokiaSSPSRSProdDiscount_EP = new MaintenanceAndSSPRuleQueryModel();

                    decimal? unlimitedSSP = 0;
                    decimal? biennialSSP = 0;
                    decimal? unlimitedSRS = 0;
                    decimal? biennialSRS = 0;
                    decimal? serviceRateY1 = 0;
                    decimal? serviceRateY2 = 0;

                    if (Constants.NOKIA_IP_ROUTING.equalsIgnoreCase(proposal.NokiaCPQ_Portfolio__c) && proposal.Is_List_Price_Only__c == false && batchLineItem.Get<bool?>(LineItemCustomField.is_Custom_Product__c) == false &&
                        partNumber != null && !partNumber.Contains(Constants.MAINTY1CODE) &&
                        !partNumber.Contains(Constants.MAINTY2CODE) &&
                        !partNumber.Contains(Constants.SSPCODE) &&
                        !partNumber.Contains(Constants.SRS))
                    {
                        if (maintenanceSSPRuleMap_EP != null && maintenanceSSPRuleMap_EP.ContainsKey(proposal.NokiaCPQ_Maintenance_Type__c + Constants.NOKIA_STRING_APPENDER + productDiscountCat) &&
                            maintenanceSSPRuleMap_EP[proposal.NokiaCPQ_Maintenance_Type__c + Constants.NOKIA_STRING_APPENDER + productDiscountCat] != null)
                        {
                            nokiaSSPSRSProdDiscount_EP = maintenanceSSPRuleMap_EP[proposal.NokiaCPQ_Maintenance_Type__c + Constants.NOKIA_STRING_APPENDER + productDiscountCat];

                            if (nokiaSSPSRSProdDiscount_EP != null)
                            {
                                //SSP Rate assignment
                                if (nokiaSSPSRSProdDiscount_EP.Unlimited_SSP_Discount__c == null)
                                {
                                    unlimitedSSP = 0;
                                }
                                else
                                {
                                    unlimitedSSP = nokiaSSPSRSProdDiscount_EP.Unlimited_SSP_Discount__c;
                                }
                                if (nokiaSSPSRSProdDiscount_EP.Biennial_SSP_Discount__c == null)
                                {
                                    biennialSSP = 0;
                                }
                                else
                                {
                                    biennialSSP = nokiaSSPSRSProdDiscount_EP.Biennial_SSP_Discount__c;
                                }
                                // SRS Rate assignment 
                                if (nokiaSSPSRSProdDiscount_EP.Unlimited_SRS_Discount__c == null)
                                {
                                    unlimitedSRS = 0;
                                }
                                else
                                {
                                    unlimitedSRS = nokiaSSPSRSProdDiscount_EP.Unlimited_SRS_Discount__c;
                                }
                                if (nokiaSSPSRSProdDiscount_EP.Biennial_SRS_Discount__c == null)
                                {
                                    biennialSRS = 0;
                                }
                                else
                                {
                                    biennialSRS = nokiaSSPSRSProdDiscount_EP.Biennial_SRS_Discount__c;
                                }

                                // Year1, Year 2 Rate assignment
                                if (nokiaSSPSRSProdDiscount_EP.Maintenance_Category__c == null)
                                {
                                    serviceRateY1 = 0;
                                }
                                else
                                {
                                    serviceRateY1 = nokiaSSPSRSProdDiscount_EP.Service_Rate_Y1__c;
                                }
                                if (nokiaSSPSRSProdDiscount_EP.Maintenance_Category__c == null)
                                {
                                    serviceRateY2 = 0;
                                }
                                else
                                {
                                    serviceRateY2 = nokiaSSPSRSProdDiscount_EP.Service_Rate_Y2__c;
                                }
                            }
                        }

                        if (proposal.NokiaCPQ_Maintenance_Type__c != null)
                        {
                            batchLineItem.Set(LineItemCustomField.Nokia_Maint_Y1_Per__c, serviceRateY1 * 100);
                            batchLineItem.Set(LineItemCustomField.Nokia_Maint_Y2_Per__c, serviceRateY2 * 100);
                        }

                        var isSSPProduct = batchLineItem.GetLookupValue<bool?>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_IsSSP__c);

                        if (isSSPProduct == false && proposal.NokiaCPQ_Maintenance_Type__c != null && proposal.NokiaCPQ_SSP_Level__c != null && 
                            Constants.NOKIA_UNLIMITED.equalsIgnoreCase(proposal.NokiaCPQ_SSP_Level__c))
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_SSP_Rate__c, unlimitedSSP * 100);
                        }
                        else if (isSSPProduct == false && proposal.NokiaCPQ_Maintenance_Type__c != null && proposal.NokiaCPQ_SSP_Level__c != null && 
                            Constants.NOKIA_BIENNIAL.equalsIgnoreCase(proposal.NokiaCPQ_SSP_Level__c))
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_SSP_Rate__c, biennialSSP * 100);
                        }

                        if (isSSPProduct == true && proposal.NokiaCPQ_Maintenance_Type__c != null && proposal.NokiaCPQ_SRS_Level__c != null && 
                            Constants.NOKIA_UNLIMITED.equalsIgnoreCase(proposal.NokiaCPQ_SRS_Level__c))
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_SRS_Rate__c, unlimitedSRS * 100);
                        }
                        else if (isSSPProduct == true && proposal.NokiaCPQ_Maintenance_Type__c != null && proposal.NokiaCPQ_SRS_Level__c != null && 
                            Constants.NOKIA_BIENNIAL.equalsIgnoreCase(proposal.NokiaCPQ_SRS_Level__c))
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_SRS_Rate__c, biennialSRS * 100);
                        }
                    }

                    if (batchLineItem.Entity.PriceListId == batchLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__PriceListItemId__r_Apttus_Config2__PriceListId__c))
                    {
                        if (batchLineItem.GetLineType() == LineType.Option && proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_SOFTWARE) &&
                            configType.equalsIgnoreCase("Standalone") && IsOptionLineFromSubBundle(batchLineItem))
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_IRP__c, 0);
                        }
                        else
                        {
                            if (batchLineItem.Get<bool?>(LineItemCustomField.is_Custom_Product__c) == false)
                            {
                                batchLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_IRP__c,
                                    pricingHelper.ApplyRounding((batchLineItem.Entity.ListPrice * (proposal.exchange_rate__c)), 5, RoundingMode.HALF_UP));
                            }
                        }

                        //Setting the Cost
                        if (!portfolioSettingList.isEmpty() && portfolioSettingList[0].Cost_Calculation_In_PCB__c == true)
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_Cost__c, 0);

                            if (batchLineItem.Entity.Cost != null)
                            {
                                if (batchLineItem.Get<bool?>(LineItemCustomField.Advanced_pricing_done__c) == false)
                                {
                                    batchLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_Cost__c,
                                        pricingHelper.ApplyRounding(((batchLineItem.Entity.Cost / defaultExchangeRate) * (proposal.exchange_rate__c)), 5, RoundingMode.HALF_UP));
                                }
                                else if (batchLineItem.Get<bool?>(LineItemCustomField.Advanced_pricing_done__c) == true && 
                                    batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_Cost__c) != null)
                                {
                                    batchLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_Cost__c, 
                                        pricingHelper.ApplyRounding(((batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_Cost__c) / defaultExchangeRate) * (proposal.exchange_rate__c)), 5, RoundingMode.HALF_UP));
                                }
                            }
                        }
                        else if (!portfolioSettingList.isEmpty() && batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_Cost_Initial__c) != null)
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_Cost__c,
                                pricingHelper.ApplyRounding(((batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_Cost_Initial__c) / defaultExchangeRate) * (proposal.exchange_rate__c)), 5, RoundingMode.HALF_UP));
                        }
                    }
                    else if (productPriceMap != null)
                    {
                        if (batchLineItem.GetLineType() == LineType.Option  && proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_SOFTWARE) && 
                            configType.equalsIgnoreCase("Standalone") && IsOptionLineFromSubBundle(batchLineItem))
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_IRP__c, 0);
                        }
                        else
                        {
                            if (batchLineItem.GetLineType() == LineType.ProductService)
                            {
                                batchLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_IRP__c,
                                    pricingHelper.ApplyRounding((productPriceMap[batchLineItem.Entity.ProductId] * (proposal.exchange_rate__c)), 5, RoundingMode.HALF_UP));
                            }
                            else
                            {
                                batchLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_IRP__c, 
                                    pricingHelper.ApplyRounding((productPriceMap[batchLineItem.Entity.OptionId] * (proposal.exchange_rate__c)), 5, RoundingMode.HALF_UP));
                            }
                        }

                        //Setting the Cost
                        if (!portfolioSettingList.isEmpty() && portfolioSettingList[0].Cost_Calculation_In_PCB__c == true)
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_Cost__c, 0);
                            if (batchLineItem.GetLineType() == LineType.ProductService && productCostMap[batchLineItem.Entity.ProductId] != null)
                            {
                                //ADDED BY PRIYANKA 
                                if (batchLineItem.Get<bool?>(LineItemCustomField.Advanced_pricing_done__c) == true)
                                {
                                    batchLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_Cost__c,
                                        pricingHelper.ApplyRounding(((batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_Cost__c) / defaultExchangeRate) * (proposal.exchange_rate__c)), 5, RoundingMode.HALF_UP));
                                }
                                else
                                {
                                    batchLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_Cost__c,
                                        pricingHelper.ApplyRounding((productCostMap[batchLineItem.Entity.ProductId] * (proposal.exchange_rate__c)), 5, RoundingMode.HALF_UP));
                                }
                            }
                            else if (batchLineItem.GetLineType() != LineType.ProductService && productCostMap[batchLineItem.Entity.OptionId] != null)
                            {
                                //ADDED BY PRIYANKA 
                                if (batchLineItem.Get<bool?>(LineItemCustomField.Advanced_pricing_done__c) == true)
                                {
                                    batchLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_Cost__c,
                                        pricingHelper.ApplyRounding(((batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_Cost__c) / defaultExchangeRate) * (proposal.exchange_rate__c)), 5, RoundingMode.HALF_UP));
                                }
                                else
                                {
                                    batchLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_Cost__c,
                                        pricingHelper.ApplyRounding((productCostMap[batchLineItem.Entity.OptionId] * (proposal.exchange_rate__c)), 5, RoundingMode.HALF_UP));
                                }
                            }
                        }
                        else if (!portfolioSettingList.isEmpty() && batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_Cost_Initial__c) != null)
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_Cost__c,
                                pricingHelper.ApplyRounding(((batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_Cost_Initial__c) / defaultExchangeRate) * (proposal.exchange_rate__c)), 5, RoundingMode.HALF_UP));
                        }
                    }

                    batchLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_IRP__c,
                        pricingHelper.ApplyRounding(batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_IRP__c), 2, RoundingMode.HALF_UP));

                    if (batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_Cost__c) != null)
                        batchLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_Cost__c,
                            pricingHelper.ApplyRounding(batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_Cost__c), 2, RoundingMode.HALF_UP));

                    if (!IsOptionLineFromSubBundle(batchLineItem))
                    {
                        batchLineItem.Set(LineItemCustomField.NokiaCPQ_Is_Direct_Option__c, true);
                    }

                    //The piece of code mentioned below is used fro addingm Maintenance line on MN Direct quotes
                    if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING))
                    {
                        foreach (MNDirectProductMapQueryModel MN_Direct_rec in MN_Direct_Products_List)
                        {
                            if (MN_Direct_rec.NokiaCPQ_Product_Code__c.Contains(partNumber))
                            {
                                batchLineItem.Set(LineItemCustomField.NokiaCPQ_Product_Type__c, MN_Direct_rec.NokiaCPQ_Product_Type__c);
                            }
                        }
                    }
                }

                //DSI-811 for DS Team Option Quantity to be calculated from the bundle
                if (proposal.Quote_Type__c.equalsIgnoreCase(Constants.Direct_DS))
                {
                    int quantityBundle = 1;
                    if (batchLineItem.GetLineType() == LineType.Option)
                    {
                        var key = Constants.NOKIA_PRODUCT_SERVICES + Constants.NOKIA_UNDERSCORE + batchLineItem.GetLineNumber();
                        if (lineItemObjectMap.ContainsKey(key) && lineItemObjectMap[key] != null)
                        {
                            if (lineItemObjectMap[key].Entity.Quantity != null)
                            {
                                quantityBundle = Convert.ToInt32(Math.Ceiling(lineItemObjectMap[key].GetQuantity()));
                                batchLineItem.Set(LineItemCustomField.Total_Option_Quantity__c, quantityBundle * batchLineItem.GetQuantity());
                            }
                        }
                    }
                }

                if (proposal.Quote_Type__c.equalsIgnoreCase(Constants.QUOTE_TYPE_INDIRECTCPQ))
                {
                    if (batchLineItem.Get<bool?>(LineItemCustomField.is_Custom_Product__c) == true)
                    {
                        string[] arrbasrValues = batchLineItem.Get<string>(LineItemCustomField.CustomProductValue__c).split(";");
                        if (arrbasrValues.size() > 0)
                        {
                            batchLineItem.Entity.BasePriceOverride = Convert.ToDecimal(arrbasrValues[2]);
                            batchLineItem.Entity.BasePrice = Convert.ToDecimal(arrbasrValues[1]);
                            batchLineItem.Entity.ListPrice = Convert.ToDecimal(arrbasrValues[0]);
                            priceListItemEntity.ListPrice = Convert.ToDecimal(arrbasrValues[0]);
                        }
                    }

                    /*Start Performance Formula Fields*/
                    string dummyBundleLI = string.Empty;
                    if (batchLineItem.GetLineType() == LineType.ProductService)
                    {
                        dummyBundleLI = batchLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Is_Dummy_Bundle_CPQ__c);
                    }
                    else
                    {
                        dummyBundleLI = batchLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Is_Dummy_Bundle_CPQ__c); 
                    }
                    /*End Performance Formula Fields*/

                    if (Constants.DEFAULTPENDING.equals(batchLineItem.Get<string>(LineItemStandardField.Apttus_Config2__ConfigStatus__c)) && Constants.NOKIA_YES.equals(dummyBundleLI) && 
                        batchLineItem.GetLineType() == LineType.Option)
                    {
                        batchLineItem.Set(LineItemStandardField.Apttus_Config2__ConfigStatus__c, Constants.COMPLETE_MSG);
                    }
                    
                    if (batchLineItem != null && batchLineItem.Get<string>(LineItemCustomField.Source__c) != null && 
                        (batchLineItem.Get<string>(LineItemCustomField.Source__c).equalsIgnoreCase(Constants.NOKIA_EPT) ||
                        batchLineItem.Get<string>(LineItemCustomField.Source__c).equalsIgnoreCase(Constants.WAVELITESOURCE)))
                    { //Added by RG for Wavelite check
                        batchLineItem.Set(LineItemStandardField.Apttus_Config2__IsReadOnly__c, true);
                    }

                    //Req-5817
                    if (proposal.NokiaProductAccreditation__r_NokiaCPQ_Incoterm_Percentage__c != null)
                    {
                        batchLineItem.Set(LineItemCustomField.NokiaCPQ_IncotermNew__c, proposal.NokiaProductAccreditation__r_NokiaCPQ_Incoterm_Percentage__c);
                    }

                    //Req : 5260
                    if (batchLineItem.Entity.PriceListId != priceListItemEntity.PriceListId)
                    {
                        if (mapPliType[batchLineItem.Entity.PriceListId] == "Indirect Market")
                        {
                            batchLineItem.Set(LineItemCustomField.Is_Contract_Pricing_2__c, false);
                        }
                        else
                        {
                            batchLineItem.Set(LineItemCustomField.Is_Contract_Pricing_2__c, true);
                        }
                    }

                    string itemName = batchLineItem.Entity.ChargeType;
                    if (!(itemName.Contains(Constants.NOKIA_ACCRED_TYPE_MAINTENANCE) || itemName.Contains(Constants.NOKIA_PRODUCT_NAME_SSP) || itemName.Contains(Constants.NOKIA_PRODUCT_NAME_SRS)))
                    {
                        int quantityBundle = 1;
                        if (batchLineItem.GetLineType() == LineType.Option)
                        {
                            if (lineItemObjectMap.ContainsKey(Constants.NOKIA_PRODUCT_SERVICES + Constants.NOKIA_UNDERSCORE + batchLineItem.GetLineNumber()) &&
                                lineItemObjectMap[Constants.NOKIA_PRODUCT_SERVICES + Constants.NOKIA_UNDERSCORE + batchLineItem.GetLineNumber()] != null)
                            {
                                if (lineItemObjectMap[Constants.NOKIA_PRODUCT_SERVICES + Constants.NOKIA_UNDERSCORE + batchLineItem.GetLineNumber()].Entity.Quantity != null)
                                {
                                    quantityBundle = Convert.ToInt32(Math.Ceiling(lineItemObjectMap[Constants.NOKIA_PRODUCT_SERVICES + Constants.NOKIA_UNDERSCORE + batchLineItem.GetLineNumber()].Entity.Quantity.Value));
                                    batchLineItem.Set(LineItemCustomField.Total_Option_Quantity__c, quantityBundle * batchLineItem.GetQuantity());
                                }
                            }
                        }

                        if (proposal.NokiaProductAccreditation__c != null)
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQAccreditationType__c, proposal.NokiaProductAccreditation__r_Pricing_Accreditation__c);
                            batchLineItem.Set(LineItemCustomField.Nokia_Pricing_Cluster__c, proposal.NokiaProductAccreditation__r_Pricing_Cluster__c);
                        }

                        if (proposal.NokiaCPQ_Maintenance_Accreditation__c != null)
                        {
                            if (proposal.NokiaCPQ_LEO_Discount__c == true)
                            {
                                batchLineItem.Set(LineItemCustomField.Nokia_Maintenance_Level__c, Constants.NOKIA_LEO);
                            }
                            //Heema Change for Defect 14394 start
                            else if (proposal.NokiaCPQ_Maintenance_Level__c.equalsIgnoreCase(Constants.NOKIA_YES))
                            {
                                batchLineItem.Set(LineItemCustomField.Nokia_Maintenance_Level__c, Constants.Nokia_Brand);
                            }
                            //Heema Change : Defect 14394 End
                            else
                            {
                                batchLineItem.Set(LineItemCustomField.Nokia_Maintenance_Level__c, proposal.NokiaCPQ_Maintenance_Accreditation__r_Pricing_Accreditation__c);
                            }

                            batchLineItem.Set(LineItemCustomField.Nokia_Maint_Pricing_Cluster__c, proposal.NokiaCPQ_Maintenance_Accreditation__r_Pricing_Cluster__c);
                        }

                        //Varsha: start: Changes for req #4920 : added check for Tier Discount applicable
                        var key = Constants.NOKIA_MAINTENANCE_STRING_APPENDER + proposal.Apttus_Proposal__Account__r_Partner_Type__c + Constants.NOKIA_STRING_APPENDER + batchLineItem.Get<string>(LineItemCustomField.Nokia_Maintenance_Level__c) + Constants.NOKIA_STRING_APPENDER + proposal.Apttus_Proposal__Account__r_Partner_Program__c;
                        if (tierDiscountRuleMap.Count > 0 && tierDiscountRuleMap.ContainsKey(key) && tierDiscountRuleMap[key] != null &&
                        tierDiscountRuleMap[key].size() > 0 && !sspSRSDefaultsList.isEmpty() && sspSRSDefaultsList[0].Tier_Discount_Applicable__c == true)
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Maint_Accreditation_Discount__c, tierDiscountRuleMap[key][0]);
                        }
                        else
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Maint_Accreditation_Discount__c, 0);
                        }

                        key = Constants.NOKIA_PRODUCT_STRING_APPENDER + proposal.Apttus_Proposal__Account__r_Partner_Type__c + Constants.NOKIA_STRING_APPENDER + batchLineItem.Get<string>(LineItemCustomField.NokiaCPQAccreditationType__c) + Constants.NOKIA_STRING_APPENDER + proposal.Apttus_Proposal__Account__r_Partner_Program__c;
                        if (tierDiscountRuleMap.Count > 0 && tierDiscountRuleMap.ContainsKey(key) && tierDiscountRuleMap[key] != null &&
                        tierDiscountRuleMap[key].size() > 0 && !sspSRSDefaultsList.isEmpty() && sspSRSDefaultsList[0].Tier_Discount_Applicable__c == true)
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Accreditation_Discount__c,
                                tierDiscountRuleMap[key][0]);
                        }
                        else
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Accreditation_Discount__c, 0);
                        }
                        //Varsha: end: Changes for req #4920 : added check for Tier Discount applicable

                        if (proposal.NokiaCPQ_IsPMA__c == true && !sspSRSDefaultsList.isEmpty() && sspSRSDefaultsList[0].AccountLevel_Discount_Applicable__c == true)
                        {
                            if (proposal.Apttus_Proposal__Account__r_NokiaCPQ_Renewal__c == true)
                            {
                                key = Constants.INCENTIVES_STRING_APPENDER + proposal.Apttus_Proposal__Account__r_Partner_Type__c + Constants.NOKIA_STRING_APPENDER + Constants.RENEWAL_STRING_APPENDER + proposal.Apttus_Proposal__Account__r_Partner_Program__c;

                                if (tierDiscountRuleMap.Any() && tierDiscountRuleMap.ContainsKey(key) && tierDiscountRuleMap[key] != null && tierDiscountRuleMap[key].size() > 0)
                                {
                                    batchLineItem.Set(LineItemCustomField.NokiaCPQ_Renewal_Per__c, tierDiscountRuleMap[key][0]);
                                }
                            }
                            if (proposal.Apttus_Proposal__Account__r_NokiaCPQ_Attachment__c == true)
                            {
                                key = Constants.INCENTIVES_STRING_APPENDER + proposal.Apttus_Proposal__Account__r_Partner_Type__c + Constants.NOKIA_STRING_APPENDER + Constants.ATTACHMENT_STRING_APPENDER + proposal.Apttus_Proposal__Account__r_Partner_Program__c;

                                if (tierDiscountRuleMap.Any() && tierDiscountRuleMap.ContainsKey(key) &&  tierDiscountRuleMap[key] != null && tierDiscountRuleMap[key].size() > 0)
                                {
                                    batchLineItem.Set(LineItemCustomField.NokiaCPQ_Attachment_Per__c, tierDiscountRuleMap[key][0]);
                                }
                            }
                            if (proposal.Apttus_Proposal__Account__r_NokiaCPQ_Performance__c == true)
                            {
                                key = Constants.INCENTIVES_STRING_APPENDER + proposal.Apttus_Proposal__Account__r_Partner_Type__c + Constants.NOKIA_STRING_APPENDER + Constants.PERFORMANCE_STRING_APPENDER + proposal.Apttus_Proposal__Account__r_Partner_Program__c;
                                if (tierDiscountRuleMap.Any() && tierDiscountRuleMap.ContainsKey(key) && tierDiscountRuleMap[key] != null && tierDiscountRuleMap[key].size() > 0)
                                {
                                    batchLineItem.Set(LineItemCustomField.NokiaCPQ_Performance_Per__c, tierDiscountRuleMap[key][0]);
                                }
                            }
                        }

                        if (Convert.ToInt32(proposal.NokiaCPQ_No_of_Years__c) >= 3 && !sspSRSDefaultsList.isEmpty() && sspSRSDefaultsList[0].Multi_Year_Discount_Applicable__c == true)
                        {
                            if (tierDiscountRuleMap.Any() && 
                                tierDiscountRuleMap[Constants.INCENTIVES_STRING_APPENDER + proposal.Apttus_Proposal__Account__r_Partner_Type__c + Constants.NOKIA_STRING_APPENDER + Constants.MULTIYR_STRING_APPENDER + proposal.Apttus_Proposal__Account__r_Partner_Program__c] != null && 
                                tierDiscountRuleMap[Constants.INCENTIVES_STRING_APPENDER + proposal.Apttus_Proposal__Account__r_Partner_Type__c + Constants.NOKIA_STRING_APPENDER + Constants.MULTIYR_STRING_APPENDER + proposal.Apttus_Proposal__Account__r_Partner_Program__c].Count > 0)
                            {
                                batchLineItem.Set(LineItemCustomField.NokiaCPQ_Multi_Yr_Per__c,
                                    tierDiscountRuleMap[Constants.INCENTIVES_STRING_APPENDER + proposal.Apttus_Proposal__Account__r_Partner_Type__c + Constants.NOKIA_STRING_APPENDER + Constants.MULTIYR_STRING_APPENDER + proposal.Apttus_Proposal__Account__r_Partner_Program__c][0]);
                            }
                        }

                        if (batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Attachment_Per__c) == null)
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Attachment_Per__c, 0);
                        }
                        if (batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Renewal_Per__c) == null)
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Renewal_Per__c, 0);
                        }
                        if (batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Performance_Per__c) == null)
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Performance_Per__c, 0);
                        }
                        if (batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Multi_Yr_Per__c) == null && Convert.ToInt32(proposal.NokiaCPQ_No_of_Years__c) < 3)
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Multi_Yr_Per__c, 0);
                        }

                        batchLineItem.Set(LineItemCustomField.NokiaCPQ_Total_Maintenance_Discount__c, 
                            batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Accreditation_Discount__c) +
                            batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Attachment_Per__c) + batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Renewal_Per__c) +
                            batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Performance_Per__c) + batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Multi_Yr_Per__c));

                        var nokiaSSPSRSProdDiscount = new NokiaMaintenanceAndSSPRulesQueryModel();
                        decimal? productCatDiscount = 0;
                        decimal? unlimitedSSP = 0;
                        decimal? biennialSSP = 0;
                        decimal? serviceRateY1 = 0;
                        decimal? serviceRateY2 = 0;

                        key = batchLineItem.Get<string>(LineItemCustomField.Nokia_Pricing_Cluster__c) + Constants.NOKIA_STRING_APPENDER + productDiscountCat + Constants.NOKIA_STRING_APPENDER + proposal.NokiaCPQ_IsPMA__c + Constants.NOKIA_STRING_APPENDER + proposal.NokiaCPQ_Maintenance_Type__c;
                        if (maintenanceSSPRuleMap != null &&
                            maintenanceSSPRuleMap.ContainsKey(key) &&
                            maintenanceSSPRuleMap[key] != null && 
                            maintenanceSSPRuleMap[key].Count > 0)
                        {
                            nokiaSSPSRSProdDiscount = maintenanceSSPRuleMap[key][0];
                            if (nokiaSSPSRSProdDiscount != null)
                            {
                                if (nokiaSSPSRSProdDiscount.NokiaCPQ_Product_Discount_Category_per__c == null)
                                {
                                    productCatDiscount = 0;
                                }
                                else
                                {
                                    productCatDiscount = nokiaSSPSRSProdDiscount.NokiaCPQ_Product_Discount_Category_per__c;
                                }
                                if (nokiaSSPSRSProdDiscount.NokiaCPQ_Unlimited_SSP_Discount__c == null)
                                {
                                    unlimitedSSP = 0;
                                }
                                else
                                {
                                    unlimitedSSP = nokiaSSPSRSProdDiscount.NokiaCPQ_Unlimited_SSP_Discount__c;
                                }
                                if (nokiaSSPSRSProdDiscount.NokiaCPQ_Biennial_SSP_Discount__c == null)
                                {
                                    biennialSSP = 0;
                                }
                                else
                                {
                                    biennialSSP = nokiaSSPSRSProdDiscount.NokiaCPQ_Biennial_SSP_Discount__c;
                                }
                            }
                        }

                        NokiaMaintenanceAndSSPRulesQueryModel nokiaMaintenanceProdDiscount;
                        key = batchLineItem.Get<string>(LineItemCustomField.Nokia_Maint_Pricing_Cluster__c) + Constants.NOKIA_STRING_APPENDER + productDiscountCat + Constants.NOKIA_STRING_APPENDER + proposal.NokiaCPQ_IsPMA__c + Constants.NOKIA_STRING_APPENDER + proposal.NokiaCPQ_Maintenance_Type__c;
                        
                        if (maintenanceSSPRuleMap != null &&
                            maintenanceSSPRuleMap.ContainsKey(key) &&
                            maintenanceSSPRuleMap[key] != null && 
                            maintenanceSSPRuleMap[key].Count > 0)
                        {
                            nokiaMaintenanceProdDiscount = maintenanceSSPRuleMap[key][0];
                            if (nokiaMaintenanceProdDiscount != null)
                            {
                                if (nokiaMaintenanceProdDiscount.NokiaCPQ_Service_Rate_Y1__c == null || batchLineItem.Get<bool?>(LineItemCustomField.NokiaCPQ_Spare__c) == true || 
                                    (proposal.NokiaCPQ_Is_Maintenance_Quote__c == true && proposal.Warranty_credit__c != null && proposal.Warranty_credit__c.equalsIgnoreCase(Constants.NOKIA_NO)) || 
                                    (batchLineItem.GetLineType() == LineType.Option && batchLineItem.Get<bool?>(LineItemCustomField.NokiaCPQ_Static_Bundle_Option__c) == true && 
                                    (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_FIXED_ACCESS_POL) || proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_FIXED_ACCESS_FBA)))
                                )
                                {
                                    serviceRateY1 = 0;
                                }
                                else
                                {
                                    serviceRateY1 = nokiaMaintenanceProdDiscount.NokiaCPQ_Service_Rate_Y1__c;
                                }
                                if (nokiaMaintenanceProdDiscount.NokiaCPQ_Service_Rate_Y2__c == null || batchLineItem.Get<bool?>(LineItemCustomField.NokiaCPQ_Spare__c) == true || Is1Year() ||
                                    (batchLineItem.GetLineType() == LineType.Option && batchLineItem.Get<bool?>(LineItemCustomField.NokiaCPQ_Static_Bundle_Option__c) == true && 
                                    (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_FIXED_ACCESS_POL) || proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_FIXED_ACCESS_FBA))))
                                {
                                    serviceRateY2 = 0;
                                }
                                else
                                {
                                    serviceRateY2 = nokiaMaintenanceProdDiscount.NokiaCPQ_Service_Rate_Y2__c;
                                }

                            }
                        }
                        //Heema : Req 6593 start:
                        if (proposal.NokiaCPQ_LEO_Discount__c == true && (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_FIXED_ACCESS_POL) || 
                            proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_FIXED_ACCESS_FBA)))
                        {
                            serviceRateY1 = 0.05m;
                            if (Is1Year())
                            {
                                serviceRateY2 = 0;
                            }
                            else
                            {
                                serviceRateY2 = 0.05m;
                            }
                        }
                        //Heema : Req 6593 End:

                        //Varsha: start: Changes for Req #4920
                        //Removed item.portfolio formula field and used proposal.NokiaCPQ_Portfolio__c
                        string portfolioFromProduct = getPortfolio(batchLineItem);
                        if (!sspSRSDefaultsList.isEmpty() && !string.IsNullOrEmpty(portfolioFromProduct) && sspSRSDefaultsList[0].Portfolio__c.equalsIgnoreCase(portfolioFromProduct))
                        {

                            if (proposal.NokiaCPQ_Maintenance_Type__c != null && batchLineItem.Get<bool?>(LineItemCustomField.is_Custom_Product__c) == false)
                            {
                                batchLineItem.Set(LineItemCustomField.Nokia_Maint_Y1_Per__c, serviceRateY1 * 100);
                                batchLineItem.Set(LineItemCustomField.Nokia_Maint_Y2_Per__c, serviceRateY2 * 100);
                            }

                            if (batchLineItem.Get<bool?>(LineItemCustomField.is_Custom_Product__c) == false)
                                batchLineItem.Set(LineItemCustomField.Nokia_CPQ_Maint_Prod_Cat_Disc__c, productCatDiscount * 100);

                            if (batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Accreditation_Discount__c) == null)
                            {
                                batchLineItem.Set(LineItemCustomField.NokiaCPQ_Accreditation_Discount__c, 0);
                            }
                            if (batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_CPQ_Maint_Prod_Cat_Disc__c) == null)
                            {
                                batchLineItem.Set(LineItemCustomField.Nokia_CPQ_Maint_Prod_Cat_Disc__c, 0);
                            }

                            //For portfolios eligible for SSP
                            if (sspSRSDefaultsList[0].SSP_Visible__c == true)
                            {
                                var isCustomProduct = batchLineItem.Get<bool?>(LineItemCustomField.is_custom_Product__c);
                                if (isCustomProduct == false)
                                {

                                    if (proposal.NokiaCPQ_SSP_Level__c != null && Constants.NOKIA_UNLIMITED.equalsIgnoreCase(proposal.NokiaCPQ_SSP_Level__c) && !IsLeo())
                                    {
                                        batchLineItem.Set(LineItemCustomField.NokiaCPQ_SSP_Rate__c, unlimitedSSP * 100);
                                    }
                                    else if (proposal.NokiaCPQ_SSP_Level__c != null && Constants.NOKIA_BIENNIAL.equalsIgnoreCase(proposal.NokiaCPQ_SSP_Level__c) && !IsLeo())
                                    {
                                        batchLineItem.Set(LineItemCustomField.NokiaCPQ_SSP_Rate__c, biennialSSP * 100);
                                    }
                                    if (batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_SSP_Rate__c) == null || IsLeo())
                                    {
                                        batchLineItem.Set(LineItemCustomField.NokiaCPQ_SSP_Rate__c, 0);
                                    }
                                }
                                if ((productDiscountCat != null && !productDiscountCat.Contains(Constants.NOKIA_NFM_P)) || isCustomProduct == true)
                                {
                                    if (priceListItemEntity.ListPrice != null && batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_SSP_Rate__c) != null)
                                    {
                                        //00209932 D-14423 Start Rounding issue
                                        batchLineItem.Set(LineItemCustomField.Nokia_SSP_List_Price__c,
                                            pricingHelper.ApplyRounding((priceListItemEntity.ListPrice * batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_SSP_Rate__c) * .01m), 2, RoundingMode.HALF_UP));
                                        //00209932 D-14423 End Rounding issue
                                    }
                                    if (batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_SSP_List_Price__c) != null &&
                                        batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_SSP_Rate__c) != null &&
                                        batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_CPQ_Maint_Prod_Cat_Disc__c) != null)
                                    {
                                        batchLineItem.Set(LineItemCustomField.Nokia_SSP_Base_Price__c, batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_SSP_List_Price__c) * 
                                            (1 - batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_CPQ_Maint_Prod_Cat_Disc__c) * .01m) * 
                                            (1 - batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Accreditation_Discount__c) * .01m));

                                        batchLineItem.Set(LineItemCustomField.Nokia_SSP_Base_Price__c,
                                            pricingHelper.ApplyRounding(batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_SSP_Base_Price__c),  2, RoundingMode.HALF_UP));
                                    }
                                    if (batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_SSP_Base_Price__c) != null && batchLineItem.Entity.Quantity != null)
                                    {
                                        batchLineItem.Set(LineItemCustomField.Nokia_SSP_Base_Extended_Price__c, 
                                            batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_SSP_Base_Price__c) * batchLineItem.GetQuantity() * quantityBundle);
                                    }
                                }
                            }

                            //For portfolios eligible for SRS
                            if (sspSRSDefaultsList[0].SRS_Visible__c == true)
                            {
                                // Replacing item.Portfolio_from_Quote_Line_Item__c with proposal.NokiaCPQ_Portfolio__c
                                if (((productDiscountCat != null && Labels.SRSPDC.Contains(productDiscountCat) || 
                                    (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_NUAGE) && batchLineItem.Entity.ProductId != null && 
                                    Constants.PRODUCTITEMTYPESOFTWARE.equalsIgnoreCase(batchLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_NokiaCPQ_Item_Type__c)))) && !IsLeo()) ||
                                    batchLineItem.Get<bool?>(LineItemCustomField.is_Custom_Product__c) == true)
                                {
                                    //system.debug('SRS calculation');
                                    if (priceListItemEntity.ListPrice != null && sspSRSDefaultsList[0].SRS_Percentage__c != null)
                                    {
                                        //00209932 D-14423 Start Rounding issue
                                        batchLineItem.Set(LineItemCustomField.Nokia_SRS_List_Price__c,
                                            pricingHelper.ApplyRounding(priceListItemEntity.ListPrice * sspSRSDefaultsList[0].SRS_Percentage__c, 2, RoundingMode.HALF_UP));
                                        //00209932 D-14423 End Rounding issue
                                    }
                                    // system.debug('SRS calculation' +item.Nokia_SRS_List_Price__c);
                                    if (batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_SRS_List_Price__c) != null &&
                                        batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_CPQ_Maint_Prod_Cat_Disc__c) != null)
                                    {
                                        batchLineItem.Set(LineItemCustomField.Nokia_SRS_Base_Price__c, batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_SRS_List_Price__c) * 
                                            (1 - batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_CPQ_Maint_Prod_Cat_Disc__c) * .01m) * 
                                            (1 - batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Accreditation_Discount__c) * .01m));

                                        batchLineItem.Set(LineItemCustomField.Nokia_SRS_Base_Price__c,
                                            pricingHelper.ApplyRounding(batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_SRS_Base_Price__c), 2, RoundingMode.HALF_UP));
                                    }
                                    if (batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_SRS_Base_Price__c) != null && batchLineItem.Entity.Quantity != null)
                                    {
                                        batchLineItem.Set(LineItemCustomField.Nokia_SRS_Base_Extended_Price__c,
                                            batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_SRS_Base_Price__c) * batchLineItem.GetQuantity() * quantityBundle);
                                    }
                                }
                            }
                        }
                        //Varsha: end: Changes for Req #4920

                        if (batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Total_Maintenance_Discount__c) == null)
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Total_Maintenance_Discount__c, 0);
                        }

                        if (priceListItemEntity.ListPrice != null && batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_Maint_Y1_Per__c) != null)
                        {
                            //00209932 D-14423 Start 
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Maint_Y1_List_Price__c, 
                                pricingHelper.ApplyRounding((priceListItemEntity.ListPrice * batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_Maint_Y1_Per__c) * .01m), 2, RoundingMode.HALF_UP));

                            batchLineItem.Set(LineItemCustomField.Nokia_Maint_Y1_Extended_List_Price__c,
                                pricingHelper.ApplyRounding((batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Y1_List_Price__c) * batchLineItem.GetQuantity() * quantityBundle), 2, RoundingMode.HALF_UP));
                            //00209932 D-14423 End 
                        }
                        if (priceListItemEntity.ListPrice != null && batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_Maint_Y2_Per__c) != null)
                        {
                            //00209932 D-14423 Start 
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_List_Price__c,
                                pricingHelper.ApplyRounding((priceListItemEntity.ListPrice * batchLineItem.Get<decimal?>(LineItemCustomField.Nokia_Maint_Y2_Per__c) * .01m), 2, RoundingMode.HALF_UP));

                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_List_Price__c,
                                pricingHelper.ApplyRounding((batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr2_List_Price__c) * batchLineItem.GetQuantity() * quantityBundle), 2, RoundingMode.HALF_UP));
                            //00209932 D-14423 End
                        }

                        if (batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Y1_List_Price__c) != null)
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Maint_Yr1_Base_Price__c,
                                batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Y1_List_Price__c) * (1 - batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Total_Maintenance_Discount__c) * .01m));

                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Maint_Yr1_Base_Price__c,
                                pricingHelper.ApplyRounding(batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr1_Base_Price__c), 2, RoundingMode.HALF_UP));
                        }

                        if (batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr2_List_Price__c) != null)
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Base_Price__c,
                                batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr2_List_Price__c) * (1 - batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Total_Maintenance_Discount__c) * .01m));
                            
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Base_Price__c,
                                pricingHelper.ApplyRounding(batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr2_Base_Price__c), 2, RoundingMode.HALF_UP));
                        }

                        if (batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr1_Base_Price__c) != null && batchLineItem.Entity.Quantity != null)
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c,
                                batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr1_Base_Price__c) * batchLineItem.GetQuantity() * quantityBundle);
                        }
                        if (batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr2_Base_Price__c) != null && batchLineItem.Entity.Quantity != null)
                        {
                            batchLineItem.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c,
                                batchLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr2_Base_Price__c) * batchLineItem.GetQuantity() * quantityBundle);
                        }

                        // End SSP for fn
                        if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_FIXED_ACCESS_POL) && priceListItemEntity.ListPrice > 0 &&
                            batchLineItem.Get<string>(LineItemCustomField.NokiaCPQ_Category__c) != Constants.NOKIA_PTP && batchLineItem.Get<decimal?>(LineItemCustomField.Product_Number_Of_Ports__c) > 0)
                        {
                            if (batchLineItem.Get<string>(LineItemCustomField.Nokia_Pricing_Cluster__c).Contains(Constants.NOKIA_NAMCLUSTER))
                            {
                                if (batchLineItem.GetLineType() == LineType.Option)
                                {
                                    batchLineItem.Set(LineItemCustomField.Total_ONT_Quantity__c,
                                        batchLineItem.Get<decimal?>(LineItemCustomField.Total_Option_Quantity__c));
                                }
                                else
                                {
                                    batchLineItem.Set(LineItemCustomField.Total_ONT_Quantity__c, batchLineItem.GetQuantity());
                                }
                            }
                            else
                            {
                                if (batchLineItem.GetLineType() == LineType.Option)
                                {
                                    batchLineItem.Set(LineItemCustomField.Total_ONT_Quantity__c,
                                        batchLineItem.Get<decimal?>(LineItemCustomField.Total_Option_Quantity__c) * batchLineItem.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Number_of_GE_Ports__c));
                                }
                                else
                                {
                                    batchLineItem.Set(LineItemCustomField.Total_ONT_Quantity__c,
                                        batchLineItem.GetQuantity() * batchLineItem.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Number_of_GE_Ports__c));
                                }
                            }
                        }

                        if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_FIXED_ACCESS_FBA))
                        {
                            if (batchLineItem.Entity.ListPrice > 0 && batchLineItem.Get<string>(LineItemCustomField.NokiaCPQ_Category__c) == Constants.NOKIA_PTP && batchLineItem.Get<decimal?>(LineItemCustomField.Product_Number_Of_Ports__c) > 0)
                            {
                                batchLineItem.Set(LineItemCustomField.Is_P2P__c, true);
                                if (batchLineItem.GetLineType() == LineType.Option)
                                {
                                    batchLineItem.Set(LineItemCustomField.Total_ONT_Quantity_P2P__c,
                                        batchLineItem.Get<decimal?>(LineItemCustomField.Total_Option_Quantity__c) * batchLineItem.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Number_of_GE_Ports__c));
                                }
                                else
                                {
                                    batchLineItem.Set(LineItemCustomField.Total_ONT_Quantity_P2P__c,
                                        batchLineItem.GetQuantity() * batchLineItem.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Number_of_GE_Ports__c));
                                }
                            }
                            if (batchLineItem.Entity.ListPrice > 0 && batchLineItem.Get<string>(LineItemCustomField.NokiaCPQ_Category__c) == Constants.NOKIA_ONT)
                            {
                                batchLineItem.Set(LineItemCustomField.Is_FBA__c, true);

                                if (batchLineItem.GetLineType() == LineType.Option)
                                {
                                    batchLineItem.Set(LineItemCustomField.Total_ONT_Quantity_FBA__c, batchLineItem.Get<string>(LineItemCustomField.Total_Option_Quantity__c));
                                }
                                else
                                {
                                    batchLineItem.Set(LineItemCustomField.Total_ONT_Quantity_FBA__c, batchLineItem.GetQuantity());
                                }
                            }
                        }
                        // End SSP for fn
                    }
                }
            }

            await Task.CompletedTask;
        }

        public async Task AfterPricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            await Task.CompletedTask;
        }

        private bool IsLeo()
        {
            return Is1Year() && proposal.NokiaCPQ_LEO_Discount__c == true;
        }

        private bool Is1Year()
        {
            return Constants.QUOTE_TYPE_INDIRECTCPQ.equalsIgnoreCase(proposal.Quote_Type__c)
                && Constants.NOKIA_1YEAR.equalsIgnoreCase(proposal.NokiaCPQ_No_of_Years__c);
        }

        private string getPartNumber(LineItemModel lineItemModel)
        {
            string partNumber = string.Empty;
            if (lineItemModel.Get<bool?>(LineItemCustomField.is_Custom_Product__c) ==  true)
            {
                partNumber = lineItemModel.Get<string>(LineItemCustomField.Custom_Product_Code__c);
            }
            else
            {
                if (lineItemModel.GetLineType() == LineType.ProductService)
                {
                    partNumber = lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_ProductCode);
                }
                else
                {
                    partNumber = lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_ProductCode);
                }
            }

            return partNumber;
        }

        string getConfigType(LineItemModel lineItemModel)
        {
            string configType = string.Empty;

            if (lineItemModel.GetLineType() == LineType.ProductService)
            {
                configType = lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Apttus_Config2__ConfigurationType__c);
            }
            else
            {
                configType = lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c);
            }

            return configType;
        }

        private string getProductDiscountCategory(LineItemModel lineItemModel)
        {
            string productDiscountCat = string.Empty;

            if (lineItemModel.GetLineType() == LineType.ProductService)
            {
                productDiscountCat = lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_NokiaCPQ_Product_Discount_Category__c);
            }
            else
            {
                productDiscountCat = lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_NokiaCPQ_Product_Discount_Category__c);
            }

            return productDiscountCat;
        }

        private bool IsOptionLineFromSubBundle(LineItemModel lineItemModel)
        {
            if (lineItemModel.GetLineType() != LineType.Option)
                return false;

            return lineItemModel.Entity.ParentBundleNumber != lineItemModel.GetRootParentLineItem().GetPrimaryLineNumber();
        }

        private string getPortfolio(LineItemModel item)
        {
            string portfolio = string.Empty;
            if (item.Entity.ProductId != null && item.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Portfolio__c) != null)
            {
                portfolio = item.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Portfolio__c);
            }

            return portfolio;
        }
    }
}

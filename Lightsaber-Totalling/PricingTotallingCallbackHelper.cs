using Apttus.Lightsaber.Extensibility.Framework.Library.Implementation;
using Apttus.Lightsaber.Extensibility.Framework.Library.Interfaces;
using Apttus.Lightsaber.Pricing.Common.Constants;
using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Pricing.Common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apttus.Lightsaber.Phillips.Totalling
{
    public class PricingTotallingCallbackHelper
    {
        private Dictionary<string, object> proposal;
        private Dictionary<string, PriceListItemQueryModel> pliDictionary;
        private IDBHelper dBHelper = null;
        private IPricingHelper pricingHelper = null;

        public PricingTotallingCallbackHelper(Dictionary<string, object> proposal, IDBHelper dBHelper, IPricingHelper pricingHelper)
        {
            this.proposal = proposal;
            this.dBHelper = dBHelper;
            this.pricingHelper = pricingHelper;
        }

        public async Task IncentiveAdjustmentUnitRounding(List<LineItemModel> cartLineItems)
        {
            foreach (LineItemModel cartLineItem in cartLineItems)
            {
                var cartLineItemEntity = cartLineItem.Entity;
                if (cartLineItemEntity.IncentiveBasePrice.HasValue && cartLineItemEntity.IncentiveBasePrice.Value != 0 && cartLineItemEntity.BasePrice.HasValue && cartLineItemEntity.BasePrice.Value != 0)
                {

                    decimal sellingTerm = cartLineItem.GetValuetOrDefault(LineItem.PropertyNames.SellingTerm, 1);
                    decimal lineItemQ = cartLineItem.GetQuantity();

                    decimal? unitIncentiveAmount = cartLineItemEntity.BasePrice - cartLineItemEntity.IncentiveBasePrice;
                    unitIncentiveAmount = unitIncentiveAmount.HasValue ? pricingHelper.ApplyRounding(unitIncentiveAmount.Value, 2, RoundingMode.HALF_EVEN) : unitIncentiveAmount;

                    cartLineItem.Set(LineItemCustomField.APTS_Unit_Incentive_Adj_Amount__c, unitIncentiveAmount);
                    cartLineItemEntity.IncentiveBasePrice = cartLineItemEntity.BasePrice - unitIncentiveAmount;
                    cartLineItemEntity.IncentiveAdjustmentAmount = unitIncentiveAmount * lineItemQ * sellingTerm * -1;
                }
            }

            await Task.CompletedTask;
        }

        public async Task SetDiscountWithAdjustmentSpread(List<LineItemModel> cartLineItems)
        {
            Dictionary<decimal, LineItemModel> bundleDiscountsDictionary = new Dictionary<decimal, LineItemModel>();

            foreach (var cartLineItem in cartLineItems)
            {
                decimal? promotionDiscount = null;
                
                if (cartLineItem.Entity.IncentiveAdjustmentAmount.HasValue)
                {
                    promotionDiscount = FormatPrecisionCeiling(cartLineItem.Entity.IncentiveAdjustmentAmount.Value) * -1;
                }

                cartLineItem.Set(LineItemCustomField.APTS_Promotion_Discount_Amount_c__c, promotionDiscount);

                if (!cartLineItem.IsOptionLine())
                {
                    bundleDiscountsDictionary.Add(cartLineItem.GetLineNumber(), cartLineItem);
                }

                decimal? adjustmentsAmount = bundleDiscountsDictionary[cartLineItem.GetLineNumber()].Entity.AdjustmentAmount;

                if (adjustmentsAmount.HasValue)
                {
                    adjustmentsAmount = FormatPrecisionCeiling(adjustmentsAmount.Value);
                }

                if(bundleDiscountsDictionary.ContainsKey(cartLineItem.GetLineNumber()) && cartLineItem.IsOptionLine() && cartLineItem.Entity.AllocateGroupAdjustment == true)
                {
                    if (bundleDiscountsDictionary[cartLineItem.GetLineNumber()].Entity.AdjustmentAmount != null &&
                           bundleDiscountsDictionary[cartLineItem.GetLineNumber()].Entity.AdjustmentType != null)
                    {
                        string bundleAdjTYpe = bundleDiscountsDictionary[cartLineItem.GetLineNumber()].Entity.AdjustmentType;
                        if (bundleAdjTYpe != Constants.PRICE_OVERRIDE && bundleAdjTYpe != Constants.DISCOUNT_AMOUNT)
                        {
                            cartLineItem.Entity.AdjustmentType = bundleAdjTYpe;
                            cartLineItem.Entity.AdjustmentAmount = adjustmentsAmount;
                        }
                    }
                    else if (cartLineItem.Entity.AdjustmentType == null)
                    {
                        cartLineItem.Entity.AdjustmentType = null;
                        cartLineItem.Entity.AdjustmentAmount = null;
                    }
                }
            }

            await Task.CompletedTask;
        }

        public async Task ComputeNetPriceAndNetAdjustment(List<LineItemModel> cartLineItems)
        {
            Dictionary<string, decimal> mapBundleAdjustments = new Dictionary<string, decimal>();
            Dictionary<string, PricePointsWrapper> mapPricePoints = new Dictionary<string, PricePointsWrapper>();

            foreach(var cartLineItem in cartLineItems)
            {
                cartLineItem.Entity.NetPrice = cartLineItem.GetLineType() == LineType.ProductService
                                    ? cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Solution_Offered_Price__c)
                                    : cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Offered_Price_c__c);

                var cartLineItemEntity = cartLineItem.Entity;
                string uniqIdentifier = cartLineItem.GetLineNumber() + cartLineItemEntity.ChargeType;
                
                if (cartLineItemEntity.OptionId == null)
                {
                    if (!mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.PrimaryLineNumber))
                    {
                        mapPricePoints.Add(uniqIdentifier, new PricePointsWrapper(cartLineItem, pliDictionary.GetValueOrDefault(cartLineItemEntity.PriceListItemId), pricingHelper));
                    }
                }
                else if (cartLineItemEntity.OptionId != null && cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c) 
                    == Constants.CONFIGURATIONTYPE_BUNDLE)
                {
                    if (!mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.PrimaryLineNumber))
                    {
                        mapPricePoints.Add(uniqIdentifier + cartLineItemEntity.PrimaryLineNumber, new PricePointsWrapper(cartLineItem, pliDictionary.GetValueOrDefault(cartLineItemEntity.PriceListItemId), pricingHelper));
                    }
                }
                else
                {
                    if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c).HasValue && !cartLineItem.IsOptional())
                    {
                        if (mapPricePoints.ContainsKey(uniqIdentifier))
                        {
                            mapPricePoints[uniqIdentifier].listPrice += cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c).Value;
                        }

                        if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                        {
                            mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].listPrice += cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c).Value;
                        }
                    }
                }
            }

            foreach (var cartLineItem in cartLineItems)
            {
                var cartLineItemEntity = cartLineItem.Entity;
                string uniqIdentifier = cartLineItem.GetLineNumber() + cartLineItemEntity.ChargeType;

                if (cartLineItemEntity.OptionId == null && cartLineItemEntity.AdjustmentType == null &&
                        cartLineItemEntity.AdjustmentAmount.HasValue && cartLineItemEntity.AdjustmentType == Constants.DISCOUNT_AMOUNT)
                {
                    decimal netAdjPercentage = 0;
                    decimal extendedListPrice = cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c).HasValue && cartLineItem.IsOptional() 
                        ? 0 : cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c).Value;

                    var solutionListPrice = mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.PrimaryLineNumber) 
                        ? mapPricePoints[uniqIdentifier + cartLineItemEntity.PrimaryLineNumber].listPrice + extendedListPrice 
                        : mapPricePoints[uniqIdentifier].listPrice + extendedListPrice;

                    cartLineItem.Set(LineItemCustomField.APTS_Solution_list_Price_c__c, solutionListPrice);
                    
                    if (solutionListPrice.HasValue)
                    {
                        netAdjPercentage = -1 * FormatPrecisionCeiling(cartLineItemEntity.AdjustmentAmount.Value) / FormatPrecisionCeiling(solutionListPrice.Value) * 100;
                    }

                    mapBundleAdjustments.Add(uniqIdentifier, netAdjPercentage);
                }
            }

            foreach (var cartLineItem in cartLineItems)
            {
                var cartLineItemEntity = cartLineItem.Entity;

                if (cartLineItemEntity.AdjustmentType != null && cartLineItemEntity.AdjustmentAmount != null)
                {
                    if (cartLineItemEntity.AdjustmentType == Constants.DISCOUNT_PERCENTAGE)
                    {
                        cartLineItemEntity.NetAdjustmentPercent = -1 * FormatPrecisionCeiling(cartLineItemEntity.AdjustmentAmount.Value);

                    }
                    else if (cartLineItemEntity.AdjustmentType == Constants.DISCOUNT_AMOUNT)
                    {
                        if (mapBundleAdjustments.ContainsKey(cartLineItem.GetLineNumber() + cartLineItemEntity.ChargeType) && cartLineItemEntity.AllocateGroupAdjustment == true)
                        {
                            cartLineItemEntity.NetAdjustmentPercent = mapBundleAdjustments[cartLineItem.GetLineNumber() + cartLineItemEntity.ChargeType];

                            if (cartLineItemEntity.OptionId != null)
                            {
                                cartLineItemEntity.AdjustmentAmount = 0;
                            }
                        }
                        else if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Philips_List_Price__c).HasValue 
                            && cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Philips_List_Price__c).Value != 0)
                        {
                            cartLineItemEntity.NetAdjustmentPercent = -1 * FormatPrecisionCeiling(cartLineItemEntity.AdjustmentAmount.Value) 
                                / FormatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Philips_List_Price__c).Value) * 100;
                        }
                    }
                }
                else if (mapBundleAdjustments.ContainsKey(cartLineItem.GetLineNumber() + cartLineItemEntity.ChargeType)
                         && cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Philips_List_Price__c).HasValue 
                         && cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Philips_List_Price__c).Value > 0
                         && cartLineItemEntity.AllocateGroupAdjustment == true)
                {
                    cartLineItemEntity.NetAdjustmentPercent = mapBundleAdjustments[cartLineItem.GetLineNumber() + cartLineItemEntity.ChargeType];
                }

                cartLineItem.Set(LineItemCustomField.APTS_Net_Adjustment_Percent_c__c, cartLineItemEntity.NetAdjustmentPercent);
            }

            await Task.CompletedTask;
        }

        public async Task CalculatePricePointsForBundle(List<LineItemModel> cartLineItems)
        {
            Dictionary<string, PricePointsWrapper> mapPricePoints = new Dictionary<string, PricePointsWrapper>();
            string bundleAdjustmentType = string.Empty;
            string uniqIdentifier_li = string.Empty;

            Dictionary<string, string> mapBundleAdjustment = new Dictionary<string, string>();

            foreach (var cartLineItem in cartLineItems)
            {
                var cartLineItemEntity = cartLineItem.Entity;
                var priceListItem = cartLineItem.GetPriceListItem();
                string uniqIdentifier = cartLineItem.GetLineNumber() + cartLineItemEntity.ChargeType;
                uniqIdentifier_li = uniqIdentifier;

                var productType = cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Apttus_Config2__ProductType__c);
                var configurationType = cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c);

                if (productType == Constants.SOLUTION_SUBSCRIPTION && (cartLineItem.GetLineType() == LineType.ProductService || configurationType == Constants.CONFIGURATIONTYPE_BUNDLE))
                {
                    uniqIdentifier_li = cartLineItem.GetLineNumber() + cartLineItemEntity.PrimaryLineNumber + cartLineItemEntity.ChargeType;
                }
                else if (productType == Constants.SOLUTION_SUBSCRIPTION && configurationType == Constants.CONFIGURATIONTYPE_OPTION)
                {
                    uniqIdentifier_li = cartLineItem.GetLineNumber() + cartLineItemEntity.ParentBundleNumber + cartLineItemEntity.ChargeType;
                }

                if (cartLineItemEntity.OptionId == null)
                {
                    if (!mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.PrimaryLineNumber))
                    {
                        mapPricePoints.Add(uniqIdentifier, new PricePointsWrapper(cartLineItem, pliDictionary.GetValueOrDefault(cartLineItemEntity.PriceListItemId), pricingHelper));
                    }

                    mapBundleAdjustment.Add(uniqIdentifier_li, cartLineItemEntity.AdjustmentType);
                }
                else if (cartLineItemEntity.OptionId != null && configurationType == Constants.CONFIGURATIONTYPE_BUNDLE && productType == Constants.SOLUTION_SUBSCRIPTION)
                {
                    if (!mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.PrimaryLineNumber))
                    {
                        mapPricePoints.Add(uniqIdentifier + cartLineItemEntity.PrimaryLineNumber, 
                            new PricePointsWrapper(cartLineItem, pliDictionary.GetValueOrDefault(cartLineItemEntity.PriceListItemId), pricingHelper));
                    }

                    mapBundleAdjustment.Add(uniqIdentifier_li, cartLineItemEntity.AdjustmentType);
                }
                else
                {
                    if (mapPricePoints.ContainsKey(uniqIdentifier))
                    {
                        decimal? targetPrice = 0;
                        decimal? minPrice = 0;
                        decimal? optEscPrice = 0;
                        decimal? optionOfferedPrice = 0;
                        decimal? sellingTerm = cartLineItem.GetValuetOrDefault(LineItem.PropertyNames.SellingTerm, 1);
                        decimal? extQty = cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_Quantity__c).HasValue 
                            && cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_Quantity__c).Value != 0 ? cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_Quantity__c).Value : 1;

                        bundleAdjustmentType = mapBundleAdjustment.GetValueOrDefault(uniqIdentifier_li) != null ? mapBundleAdjustment.GetValueOrDefault(uniqIdentifier_li) : string.Empty;

                        var extendedListPrice = cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c);

                        if (extendedListPrice.HasValue && !cartLineItem.IsOptional())
                        {
                            mapPricePoints[uniqIdentifier].listPrice += extendedListPrice.Value;

                            if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                            {
                                mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].listPrice += extendedListPrice.Value;
                            }
                        }

                        decimal listPrice = extendedListPrice.HasValue ? extendedListPrice.Value : 0;
                        decimal contractDiscount = cartLineItem.Get<decimal?>(LineItemCustomField.APTS_ContractDiscount__c).HasValue 
                            ? cartLineItem.Get<decimal?>(LineItemCustomField.APTS_ContractDiscount__c).Value : 0;
                        decimal contractAmt = listPrice * contractDiscount / 100;

                        cartLineItem.Set(LineItemCustomField.APTS_Contract_Discount_Amount__c, contractAmt);
                        decimal? netAdjPercent = cartLineItemEntity.NetAdjustmentPercent;

                        decimal? stdAdj = GetStrategicDiscount(netAdjPercent, cartLineItemEntity.AdjustmentType, listPrice);
                        decimal incAdjAmt = cartLineItemEntity.IncentiveAdjustmentAmount != null ? FormatPrecisionCeiling(cartLineItemEntity.IncentiveAdjustmentAmount.Value) : 0;

                        decimal? unitStrategicDiscountAmount = (listPrice / extQty / sellingTerm) * stdAdj / 100;
                        decimal? strategicDiscountAmount = listPrice * stdAdj / 100;

                        if (productType != null && productType.Contains("Solution"))
                        {
                            if (cartLineItem.Get<string>(LineItemCustomField.APTS_Billing_Plan__c) != null 
                                && cartLineItem.Get<string>(LineItemCustomField.APTS_Billing_Plan__c) == Constants.BILLING_PLAN_ANNUAL)
                            {
                                var roundedUnitStrategicDisount = pricingHelper.ApplyRounding(unitStrategicDiscountAmount * 12, 2, RoundingMode.HALF_EVEN);
                                cartLineItem.Set(LineItemCustomField.APTS_Unit_Strategic_Discount_Amount__c, roundedUnitStrategicDisount);
                            }
                            else
                            {
                                var roundedUnitStrategicDisount = pricingHelper.ApplyRounding(unitStrategicDiscountAmount, 2, RoundingMode.HALF_EVEN);
                                cartLineItem.Set(LineItemCustomField.APTS_Unit_Strategic_Discount_Amount__c, roundedUnitStrategicDisount);
                            }
                        }
                        else if (productType == Constants.SERVICE_PRODUCT_TYPE)
                        {
                            cartLineItem.Set(LineItemCustomField.APTS_Unit_Strategic_Discount_Amount__c, unitStrategicDiscountAmount);
                            cartLineItem.Set(LineItemCustomField.APTS_Strategic_Discount_Amount_c__c, strategicDiscountAmount);
                        }
                        else
                        {
                            var roundedUnitStrategicDisount = pricingHelper.ApplyRounding(unitStrategicDiscountAmount, 2, RoundingMode.HALF_EVEN);
                            cartLineItem.Set(LineItemCustomField.APTS_Unit_Strategic_Discount_Amount__c, roundedUnitStrategicDisount);

                            cartLineItem.Set(LineItemCustomField.APTS_Strategic_Discount_Amount_c__c, roundedUnitStrategicDisount * sellingTerm * extQty);
                        }

                        if (productType != null && productType.Contains("Solution"))
                        {
                            if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                            {
                                if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_SAP_List_Price__c).HasValue)
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].sapListPrice += cartLineItem.Get<decimal?>(LineItemCustomField.APTS_SAP_List_Price__c).Value;
                                }
                                if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Unit_Strategic_Discount_Amount__c).HasValue)
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].unitStrategicDiscountAmount 
                                        += cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Unit_Strategic_Discount_Amount__c).Value;
                                }
                            }
                        }

                        if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Target_Price__c).HasValue)
                        {
                            targetPrice = FormatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Target_Price__c).Value);
                            if (!cartLineItem.IsOptional() && targetPrice != null)
                            {
                                mapPricePoints[uniqIdentifier].targetPrice += targetPrice;
                                if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].targetPrice += targetPrice;
                            }
                        }

                        if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Minimum_Price__c).HasValue)
                        {
                            minPrice = FormatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Minimum_Price__c).Value);
                            if (!cartLineItem.IsOptional())
                            {
                                mapPricePoints[uniqIdentifier].minPrice += minPrice;
                                if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].minPrice += minPrice;
                                }
                            }
                        }

                        if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Escalation_Price__c).HasValue)
                        {
                            optEscPrice = FormatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Escalation_Price__c).Value);
                            if (!cartLineItem.IsOptional())
                            {
                                mapPricePoints[uniqIdentifier].preEscalationPrice += optEscPrice;
                                if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].preEscalationPrice += optEscPrice;
                                }
                            }
                        }

                        if (cartLineItemEntity.ListPrice.HasValue && cartLineItem.Get<bool?>(LineItemCustomField.APTS_Local_Bundle_Component_Flag__c) != true)
                        {
                            decimal? qty = cartLineItem.GetQuantity();

                            if (!cartLineItem.IsOptional())
                            {
                                var countryPriceListListPrice = pliDictionary.GetValueOrDefault(cartLineItemEntity.PriceListItemId)?.APTS_Country_Pricelist_List_Price__c;

                                mapPricePoints[uniqIdentifier].optionPrice += (countryPriceListListPrice == null) 
                                    ? FormatPrecisionCeiling(cartLineItemEntity.ListPrice.Value) * qty : countryPriceListListPrice.Value * qty;

                                if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].optionPrice += (countryPriceListListPrice == null) 
                                        ? FormatPrecisionCeiling(cartLineItemEntity.ListPrice.Value) * qty : countryPriceListListPrice.Value * qty;
                                }
                            }
                        }

                        if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Contract_Net_Price__c).HasValue && !cartLineItem.IsOptional())
                        {
                            mapPricePoints[uniqIdentifier].contractNetPrice += FormatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Contract_Net_Price__c).Value);

                            if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                            {
                                mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].contractNetPrice += 
                                    FormatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Contract_Net_Price__c).Value);
                            }
                        }

                        if (cartLineItemEntity.ExtendedPrice.HasValue && !cartLineItem.IsOptional())
                        {
                            mapPricePoints[uniqIdentifier].bundleExtendedPrice += mapPricePoints[uniqIdentifier].qty != null 
                                ? FormatPrecisionCeiling(cartLineItemEntity.ExtendedPrice.Value) * mapPricePoints[uniqIdentifier].qty : FormatPrecisionCeiling(cartLineItemEntity.ExtendedPrice.Value);

                            if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                            {
                                mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].bundleExtendedPrice += 
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].qty != null 
                                    ? FormatPrecisionCeiling(cartLineItemEntity.ExtendedPrice.Value) * mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].qty 
                                    : FormatPrecisionCeiling(cartLineItemEntity.ExtendedPrice.Value);
                            }
                        }

                        if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_ContractDiscount__c).HasValue && priceListItem.Entity.ListPrice.HasValue)
                        {
                            PricePointsWrapper points = mapPricePoints[uniqIdentifier];
                            if (points.solutionContractDiscountAmount == null)
                            {
                                points.solutionContractDiscountAmount = 0;
                            }

                            if (!cartLineItem.IsOptional())
                            {
                                mapPricePoints[uniqIdentifier].solutionContractDiscountAmount += contractAmt;
                                if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].solutionContractDiscountAmount += contractAmt;
                                }
                            }
                        }

                        if (cartLineItemEntity.IncentiveAdjustmentAmount.HasValue && cartLineItemEntity.IncentiveAdjustmentAmount.Value < 0 && !cartLineItem.IsOptional())
                        {
                            var unitIncentiveAdjAmount = cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Unit_Incentive_Adj_Amount__c);
                            mapPricePoints[uniqIdentifier].incentiveAdjAmount += (FormatPrecisionCeiling(cartLineItemEntity.IncentiveAdjustmentAmount.Value) * -1);

                            if (unitIncentiveAdjAmount.HasValue)
                            {
                                mapPricePoints[uniqIdentifier].solutionUnitIncentiveAmount += unitIncentiveAdjAmount.Value * -1;
                            }

                            if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                            {
                                mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].incentiveAdjAmount 
                                    += (FormatPrecisionCeiling(cartLineItemEntity.IncentiveAdjustmentAmount.Value) * -1);

                                if (unitIncentiveAdjAmount.HasValue)
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].solutionUnitIncentiveAmount += unitIncentiveAdjAmount.Value * -1;
                                }
                            }
                        }
                        else if (cartLineItemEntity.IncentiveAdjustmentAmount.HasValue && cartLineItemEntity.IncentiveAdjustmentAmount.Value > 0 && !cartLineItem.IsOptional())
                        {
                            var unitIncentiveAdjAmount = cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Unit_Incentive_Adj_Amount__c);
                            mapPricePoints[uniqIdentifier].incentiveAdjAmount += (FormatPrecisionCeiling(cartLineItemEntity.IncentiveAdjustmentAmount.Value));

                            if (unitIncentiveAdjAmount.HasValue)
                            {
                                mapPricePoints[uniqIdentifier].solutionUnitIncentiveAmount += unitIncentiveAdjAmount.Value;
                            }

                            if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                            {
                                mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].incentiveAdjAmount += (FormatPrecisionCeiling(cartLineItemEntity.IncentiveAdjustmentAmount.Value));

                                if (unitIncentiveAdjAmount.HasValue)
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].solutionUnitIncentiveAmount += unitIncentiveAdjAmount.Value;
                                }
                            }
                        }

                        if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Offered_Price_c__c).HasValue)
                        {
                            optionOfferedPrice = cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c).HasValue 
                                ? cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c).Value : 0;
                            
                            decimal? totalDiscounts = 0;
                            totalDiscounts = totalDiscounts - contractAmt;

                            if (cartLineItemEntity.IncentiveAdjustmentAmount.HasValue)
                            {
                                decimal incAdjAmt1 = FormatPrecisionCeiling(cartLineItemEntity.IncentiveAdjustmentAmount.Value);
                                totalDiscounts = totalDiscounts + incAdjAmt1;
                            }
                            if (cartLineItemEntity.NetAdjustmentPercent.HasValue)
                            {
                                totalDiscounts = totalDiscounts - cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Strategic_Discount_Amount_c__c);
                            }

                            optionOfferedPrice = optionOfferedPrice + totalDiscounts;

                            if (!cartLineItem.IsOptional() && optionOfferedPrice.HasValue)
                            {
                                mapPricePoints[uniqIdentifier].offeredPrice += optionOfferedPrice;

                                if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].offeredPrice += optionOfferedPrice;
                                }
                            }
                        }

                        if (optEscPrice != null && optEscPrice != 0)
                        {
                            var escalationPriceAttainment = (optionOfferedPrice / FormatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Escalation_Price__c).Value)) * 100;
                            cartLineItem.Set(LineItemCustomField.APTS_Escalation_Price_Attainment_c__c, escalationPriceAttainment);
                        }

                        if (targetPrice != null && targetPrice != 0)
                        {
                            var targetPriceAttainment = (optionOfferedPrice / (FormatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Target_Price__c).Value))) * 100;
                            cartLineItem.Set(LineItemCustomField.APTS_Target_Price_Attainment__c, targetPriceAttainment);
                        }

                        if (minPrice != null && minPrice != 0)
                        {
                            var minimumPriceAttainment = (optionOfferedPrice / (FormatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Minimum_Price__c).Value))) * 100;
                            cartLineItem.Set(LineItemCustomField.APTS_Minimum_Price_Attainment_c__c, minimumPriceAttainment);
                        }

                        cartLineItem.Set(LineItemCustomField.APTS_Price_Attainment_Color__c, 
                            GetColor(optionOfferedPrice, optEscPrice, minPrice, cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c)));
                    }
                }
            }

            foreach (var cartLineItem in cartLineItems)
            {
                var cartLineItemEntity = cartLineItem.Entity;

                decimal? sellingTerm = cartLineItemEntity.SellingTerm;
                decimal? extQty = cartLineItem.GetValuetOrDefault(LineItemCustomField.APTS_Extended_Quantity__c, 1);
                string uniqIdentifier = cartLineItem.GetLineNumber() + cartLineItemEntity.ChargeType;
                uniqIdentifier_li = uniqIdentifier;

                var productType = cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Apttus_Config2__ProductType__c);
                var configurationType = cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c);

                if (productType == Constants.SOLUTION_SUBSCRIPTION && (cartLineItem.GetLineType() == LineType.ProductService || configurationType == Constants.CONFIGURATIONTYPE_BUNDLE))
                {
                    uniqIdentifier_li = cartLineItem.GetLineNumber() + cartLineItemEntity.PrimaryLineNumber + cartLineItemEntity.ChargeType;
                }
                else if (productType == Constants.SOLUTION_SUBSCRIPTION && configurationType == Constants.CONFIGURATIONTYPE_OPTION)
                {
                    uniqIdentifier_li = cartLineItem.GetLineNumber() + cartLineItemEntity.ParentBundleNumber + cartLineItemEntity.ChargeType;
                }

                bundleAdjustmentType = mapBundleAdjustment[uniqIdentifier_li] != null ? mapBundleAdjustment[uniqIdentifier_li] : string.Empty;

                if (cartLineItemEntity.OptionId != null && configurationType == Constants.CONFIGURATIONTYPE_BUNDLE)
                {
                    uniqIdentifier += cartLineItemEntity.PrimaryLineNumber;
                }

                if (mapPricePoints.ContainsKey(uniqIdentifier))
                {
                    if (cartLineItemEntity.OptionId == null || (cartLineItemEntity.OptionId != null && configurationType == Constants.CONFIGURATIONTYPE_BUNDLE))
                    {
                        var extListPrice = cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c);

                        decimal extendedListPrice = cartLineItem.IsOptional() ? 0 : extListPrice.Value;
                        
                        if (extListPrice.HasValue)
                        {
                            cartLineItem.Set(LineItemCustomField.APTS_Solution_list_Price_c__c, mapPricePoints[uniqIdentifier].listPrice + extendedListPrice);
                        }

                        decimal listPrice = extListPrice.HasValue ? extListPrice.Value : 0;
                        decimal? bundleOfferedPrice = extListPrice.HasValue ? extListPrice.Value : 0;
                        decimal? totalDiscounts = 0;
                        decimal? contractAmt = 0;
                        decimal? unitContractAmt = 0;

                        if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_ContractDiscount__c).HasValue)
                        {
                            decimal contractDiscount = cartLineItem.Get<decimal?>(LineItemCustomField.APTS_ContractDiscount__c).Value;
                            unitContractAmt = ((bundleOfferedPrice / extQty / sellingTerm) * contractDiscount / 100);
                            unitContractAmt = pricingHelper.ApplyRounding(unitContractAmt, 2, RoundingMode.HALF_EVEN);
                            contractAmt = unitContractAmt * extQty * sellingTerm;
                            totalDiscounts = totalDiscounts - contractAmt;
                        }
                        if (cartLineItemEntity.IncentiveAdjustmentAmount.HasValue)
                        {
                            totalDiscounts = totalDiscounts + FormatPrecisionCeiling(cartLineItemEntity.IncentiveAdjustmentAmount.Value);
                        }
                        if (cartLineItemEntity.NetAdjustmentPercent.HasValue)
                        {
                            decimal? stdAdj = GetStrategicDiscount(cartLineItemEntity.NetAdjustmentPercent.Value, cartLineItemEntity.AdjustmentType, listPrice);
                            decimal? incAdjAmt = cartLineItemEntity.IncentiveAdjustmentAmount.HasValue ? FormatPrecisionCeiling(cartLineItemEntity.IncentiveAdjustmentAmount.Value) : 0;

                            decimal? unitStrategicDiscountAmount = (listPrice / extQty / sellingTerm) * stdAdj / 100;
                            decimal? strategicDiscountAmount = listPrice * stdAdj / 100;

                            if (productType == Constants.SERVICE_PRODUCT_TYPE)
                            {
                                cartLineItem.Set(LineItemCustomField.APTS_Unit_Strategic_Discount_Amount__c, unitStrategicDiscountAmount);
                                cartLineItem.Set(LineItemCustomField.APTS_Strategic_Discount_Amount_c__c, strategicDiscountAmount);
                            }
                            else
                            {
                                cartLineItem.Set(LineItemCustomField.APTS_Unit_Strategic_Discount_Amount__c, pricingHelper.ApplyRounding(unitStrategicDiscountAmount, 2, RoundingMode.HALF_EVEN));
                                cartLineItem.Set(LineItemCustomField.APTS_Strategic_Discount_Amount_c__c,
                                    pricingHelper.ApplyRounding(unitStrategicDiscountAmount, 2, RoundingMode.HALF_EVEN) * sellingTerm * extQty);
                            }

                            totalDiscounts = totalDiscounts - cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Strategic_Discount_Amount_c__c);
                        }

                        bundleOfferedPrice = bundleOfferedPrice + totalDiscounts;

                        if (productType != null && productType.Contains("Solution") && cartLineItemEntity.OptionId != null && configurationType == Constants.CONFIGURATIONTYPE_BUNDLE)
                        {
                            var billingPlan = cartLineItem.Get<string>(LineItemCustomField.APTS_Billing_Plan__c);

                            if (billingPlan != null && (billingPlan == Constants.BILLING_PLAN_MONTHLY || billingPlan == Constants.BILLING_PLAN_QUATERLY || billingPlan == Constants.BILLING_PLAN_ANNUAL))
                            {
                                cartLineItem.Set(LineItemCustomField.APTS_SAP_List_Price__c, mapPricePoints[uniqIdentifier].sapListPrice);
                            }

                            cartLineItem.Set(LineItemCustomField.APTS_Unit_Strategic_Discount_Amount__c, mapPricePoints[uniqIdentifier].unitStrategicDiscountAmount);
                        }
                        
                        decimal? optEscPrice = FormatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Escalation_Price__c).Value);
                        if (optEscPrice != null && optEscPrice != 0)
                        {
                            decimal? escalationPriceAttainment;
                            if (!Constants.listTradeSpoo.Contains(cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_APTS_SPOO_Type__c)))
                            {
                                escalationPriceAttainment = (bundleOfferedPrice / optEscPrice) * 100;
                            }
                            else
                            {
                                escalationPriceAttainment = bundleOfferedPrice != 0 ? (optEscPrice / bundleOfferedPrice) * 100 : 0;
                            }

                            cartLineItem.Set(LineItemCustomField.APTS_Escalation_Price_Attainment_c__c, escalationPriceAttainment);
                        }
                        decimal? minPrice = FormatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Minimum_Price__c));
                        if (minPrice != null && minPrice != 0)
                        {
                            decimal? minimumPriceAttainment;
                            if (!Constants.listTradeSpoo.Contains(cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_APTS_SPOO_Type__c)))
                            {
                                minimumPriceAttainment = (bundleOfferedPrice / minPrice) * 100;
                            }
                            else
                            {
                                minimumPriceAttainment = bundleOfferedPrice != 0 ? (minPrice / bundleOfferedPrice) * 100 : 0;
                            }

                            cartLineItem.Set(LineItemCustomField.APTS_Minimum_Price_Attainment_c__c, minimumPriceAttainment);
                        }
                        decimal? targetPrice = FormatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Target_Price__c));
                        if (targetPrice != null && targetPrice != 0)
                        {
                            decimal? targetPriceAttainment;
                            if (!Constants.listTradeSpoo.Contains(cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_APTS_SPOO_Type__c)))
                            {
                                targetPriceAttainment = (bundleOfferedPrice / targetPrice) * 100;
                            }
                            else
                            {
                                targetPriceAttainment = bundleOfferedPrice != 0 ? (targetPrice / bundleOfferedPrice) * 100 : 0;
                            }

                            cartLineItem.Set(LineItemCustomField.APTS_Target_Price_Attainment__c, targetPriceAttainment);
                        }

                        cartLineItem.Set(LineItemCustomField.APTS_Price_Attainment_Color__c, 
                            GetColor(bundleOfferedPrice, optEscPrice, minPrice, cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c)));

                        decimal? solMinPrice = cartLineItem.IsOptional() 
                            ? mapPricePoints[uniqIdentifier].minPrice 
                            : mapPricePoints[uniqIdentifier].minPrice + FormatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Minimum_Price__c));

                        cartLineItem.Set(LineItemCustomField.APTS_Minimum_Price_Bundle__c, solMinPrice);
                        decimal? solEscPrice = cartLineItem.IsOptional() ? mapPricePoints[uniqIdentifier].preEscalationPrice : mapPricePoints[uniqIdentifier].preEscalationPrice + optEscPrice;
                        cartLineItem.Set(LineItemCustomField.APTS_Escalation_Price_Bundle__c, solEscPrice);

                        if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Country_Target_Price__c).HasValue)
                        {
                            cartLineItem.Set(LineItemCustomField.APTS_Target_Price_Bundle__c,
                                cartLineItem.IsOptional() 
                                ? mapPricePoints[uniqIdentifier].targetPrice 
                                : mapPricePoints[uniqIdentifier].targetPrice + FormatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Target_Price__c))
                                );
                        }

                        cartLineItem.Set(LineItemCustomField.APTS_Option_List_Price__c, mapPricePoints[uniqIdentifier].optionPrice);
                        cartLineItem.Set(LineItemCustomField.APTS_Contract_Discount_Amount__c, mapPricePoints[uniqIdentifier].contractDiscountAmount);
                        cartLineItem.Set(LineItemCustomField.APTS_Solution_Contract_Discount_Amount__c, mapPricePoints[uniqIdentifier].solutionContractDiscountAmount);
                        cartLineItem.Set(LineItemCustomField.APTS_Incentive_Adjustment_Amount_Bundle__c, mapPricePoints[uniqIdentifier].incentiveAdjAmount);
                        cartLineItem.Set(LineItemCustomField.APTS_Solution_Unit_Incentive_Adj_Amount__c, mapPricePoints[uniqIdentifier].solutionUnitIncentiveAmount);

                        decimal? solOfferedPrice = mapPricePoints[uniqIdentifier].offeredPrice;
                        cartLineItem.Set(LineItemCustomField.APTS_Solution_Offered_Price_c__c, solOfferedPrice);
                        cartLineItemEntity.NetPrice = solOfferedPrice + bundleOfferedPrice;

                        if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Solution_List_Price_c__c).HasValue && cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Solution_Offered_Price__c).HasValue)
                        {
                            var totalResultingDiscountAmount = cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Solution_List_Price_c__c).Value - 
                                (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Solution_Offered_Price__c).Value + bundleOfferedPrice);
                            
                            cartLineItem.Set(LineItemCustomField.APTS_Total_Resulting_Discount_Amount__c, totalResultingDiscountAmount);
                        }

                        cartLineItem.Set(LineItemCustomField.APTS_Solution_Price_Attainment_Color__c, 
                            GetColor(solOfferedPrice + bundleOfferedPrice, solEscPrice, solMinPrice, cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c)));
                    }
                }
            }

            await Task.CompletedTask;
        }

        public async Task PopulatePLICustomFields(List<LineItemModel> cartLineItems)
        {
            pliDictionary = new Dictionary<string, PriceListItemQueryModel>();
            var priceListItemIdSet = cartLineItems.Select(li => li.GetPriceListItem().Entity.Id).ToHashSet();

            var pliQuery = QueryHelper.GetPLIQuery(priceListItemIdSet);

            List<PriceListItemQueryModel> pliDetails = await dBHelper.FindAsync<PriceListItemQueryModel>(pliQuery);
            foreach (var pliDetail in pliDetails)
            {
                pliDictionary.Add(pliDetail.Id, pliDetail);
            }
        }

        public async Task PopulateCustomFields(List<LineItemModel> cartLineItems)
        {
            foreach (var cartLineItem in cartLineItems)
            {
                var cartLineItemEntity = cartLineItem.Entity;

                var pliCustomFields = pliDictionary.GetValueOrDefault(cartLineItemEntity.PriceListItemId);

                if (pliCustomFields != null && pliCustomFields.APTS_Country_Pricelist_List_Price__c != null)
                {
                    cartLineItem.Set(LineItemCustomField.APTS_Payment_Term__c, pliCustomFields.Apttus_Config2__PriceListId__r.APTS_Payment_Term_Credit_Terms__c);
                    cartLineItem.Set(LineItemCustomField.APTS_Inco_Term__c, pliCustomFields.Apttus_Config2__PriceListId__r.APTS_Inco_Terms__c);
                }
                else
                {
                    cartLineItem.Set(LineItemCustomField.APTS_Payment_Term__c, proposal[ProposalField.Apttus_Proposal__Payment_Term__c]);
                    cartLineItem.Set(LineItemCustomField.APTS_Inco_Term__c, proposal[ProposalField.APTS_Inco_Term__c]);
                }

                if (cartLineItem.GetLineType() == LineType.Option)
                {
                    if (cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Main_Article_Group_ID__c) != null)
                        cartLineItem.Set(LineItemCustomField.APTS_MAG__c, cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Main_Article_Group_ID__c));

                    if (cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Business_Unit_ID__c) != null)
                        cartLineItem.Set(LineItemCustomField.APTS_Business_Unit__c, cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Business_Unit_ID__c));
                }
                else
                {
                    if (cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Main_Article_Group_ID__c) != null)
                        cartLineItem.Set(LineItemCustomField.APTS_MAG__c, cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Main_Article_Group_ID__c));

                    if (cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Business_Unit_ID__c) != null)
                        cartLineItem.Set(LineItemCustomField.APTS_Business_Unit__c, cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Business_Unit_ID__c));
                }

                var escalationPriceAttainment = cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Escalation_Price_Attainment__c);
                if (escalationPriceAttainment.HasValue)
                {
                    cartLineItem.Set(LineItemCustomField.APTS_Is_Escalation_Price_Attained__c, 
                        !((escalationPriceAttainment.Value < 100 && cartLineItemEntity.NetPrice > 0) || (escalationPriceAttainment.Value > 100 && cartLineItemEntity.NetPrice < 0)));
                }

                var minimumPriceAttainment = cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Minimum_Price_Attainment__c);
                if (minimumPriceAttainment.HasValue)
                {
                    cartLineItem.Set(LineItemCustomField.APTS_Is_Minimum_Price_Attained__c, 
                        !((minimumPriceAttainment.Value < 100 && cartLineItemEntity.NetPrice > 0) || (minimumPriceAttainment.Value > 100 && cartLineItemEntity.NetPrice < 0)));
                }
            }

            await Task.CompletedTask;
        }

        public async Task SetRollupsAndThresholdFlags(ProductConfigurationModel cart, List<LineItemModel> cartLineItems)
        {
            Dictionary<string, decimal?> mapClogToThresoldValue = new Dictionary<string, decimal?>();

            var thresholdApproversQuery = QueryHelper.GetThresholdApproversQuery(cart.Get<string>(CartCustomField.APTS_Sales_Organization__c));
            List<ThresholdApproverQueryModel> thresoldApprovers = await dBHelper.FindAsync<ThresholdApproverQueryModel>(thresholdApproversQuery);

            var procurementApprovalQuery = QueryHelper.GetProcurementApprovalQuery(cart.Get<string>(CartCustomField.APTS_Sales_Organization__c));
            List<ProcurementApprovalQueryModel> procurementApprovals = await dBHelper.FindAsync<ProcurementApprovalQueryModel>(procurementApprovalQuery);

            foreach (ProcurementApprovalQueryModel proc in procurementApprovals)
            {
                mapClogToThresoldValue.Add(proc.APTS_CLOGS__c, proc.APTS_Threshold_Value__c);
            }

            cart.Set(CartCustomField.APTS_3rd_Party_Total_Required__c, false);
            cart.Set(CartCustomField.APTS_Installation_Cost_Required__c, false);
            cart.Set(CartCustomField.APTS_Quote_Total_Required__c, false);
            cart.Set(CartCustomField.APTS_3rd_Party_Totals_PCB__c, 0);
            cart.Set(CartCustomField.APTS_Total_Net_Price_PCB__c, 0);
            cart.Set(CartCustomField.APTS_PM_Approval_Required__c, false);
            cart.Set(CartCustomField.APTS_Finance_Required__c, false);

            decimal? CalculatedThreshold_3rdP = 0;
            decimal? CalculatedThreshold_QT = 0;
            decimal? CalculatedThreshold_prjmgr = 0;

            CalculatedThreshold_prjmgr = cart.Get<decimal?>(CartCustomField.APTS_MCOS__c) + cart.Get<decimal?>(CartCustomField.APTS_DSE__c);

            foreach (var cartLineItem in cartLineItems)
            {
                var cartLineItemEntity = cartLineItem.Entity;

                if (cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_APTS_Type__c) == "SPOO"
                    && cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_APTS_SPOO_Type__c) == "3rd Party"
                    && cartLineItemEntity.NetPrice.HasValue)
                {
                    CalculatedThreshold_3rdP = CalculatedThreshold_3rdP + FormatPrecisionCeiling(cartLineItemEntity.NetPrice.Value);
                }

                if (cartLineItem.GetLineType() == LineType.ProductService && cartLineItemEntity.NetPrice.HasValue)
                {
                    CalculatedThreshold_QT = CalculatedThreshold_QT + FormatPrecisionCeiling(cartLineItemEntity.NetPrice.Value);
                }

                if (mapClogToThresoldValue.ContainsKey(cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_APTS_CLOGS__c))
                            && cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c) >= 
                            mapClogToThresoldValue.GetValueOrDefault(cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_APTS_CLOGS__c)))
                {
                    cartLineItem.Set(LineItemCustomField.APTS_Procurement_approval_needed__c, true);
                }
                else
                {
                    cartLineItem.Set(LineItemCustomField.APTS_Procurement_approval_needed__c, false);
                }
            }

            cart.Set(CartCustomField.APTS_3rd_Party_Totals_PCB__c, CalculatedThreshold_3rdP);
            cart.Set(CartCustomField.APTS_Total_Net_Price_PCB__c, CalculatedThreshold_QT);

            if (thresoldApprovers.Count > 0)
            {
                ThresholdApproverQueryModel objApprTH = thresoldApprovers[0];

                if (CalculatedThreshold_3rdP >= objApprTH.APTS_3rd_Party_Threshold__c)
                    cart.Set(CartCustomField.APTS_3rd_Party_Total_Required__c, true);

                if (CalculatedThreshold_QT >= objApprTH.APTS_Quote_Total_Threshold__c)
                    cart.Set(CartCustomField.APTS_Quote_Total_Required__c, true);

                if (cart.Get<decimal?>(CartCustomField.APTS_Turnover__c) > objApprTH.APTS_Turnover_Threshold__c || cart.Get<decimal?>(CartCustomField.APTS_FSE__c) > objApprTH.APTS_FSE_Threshold__c)
                    cart.Set(CartCustomField.APTS_Finance_Required__c, true);

                decimal? Install_Threshold = 0;
                decimal? PM_Percentage_Threshold = 0;

                if (objApprTH.APTS_Installation_Threshold__c != null)
                    Install_Threshold = objApprTH.APTS_Installation_Threshold__c;

                if (objApprTH.APTS_PM_Percentage_Threshold__c != null)
                    PM_Percentage_Threshold = objApprTH.APTS_PM_Percentage_Threshold__c;

                decimal? thresholdmgrvalue = 0;
                if (cart.Get<decimal?>(CartCustomField.APTS_Offer_Price__c).HasValue && cart.Get<decimal?>(CartCustomField.APTS_Offer_Price__c).Value > 0)
                {
                    thresholdmgrvalue = CalculatedThreshold_prjmgr / cart.Get<decimal?>(CartCustomField.APTS_Offer_Price__c).Value;
                }

                if (CalculatedThreshold_prjmgr > Install_Threshold || thresholdmgrvalue > PM_Percentage_Threshold)
                {
                    cart.Set(CartCustomField.APTS_PM_Approval_Required__c, true);
                }
            }
        }

        private decimal? GetStrategicDiscount(decimal? netAdj, string adjType, decimal? listPrice)
        {
            decimal? stdAdj = netAdj * -1;
            
            if(listPrice < 0 && adjType == Constants.DISCOUNT_PERCENTAGE) 
            {
                stdAdj = netAdj;
            }

            if (listPrice < 0 && adjType == Constants.DISCOUNT_AMOUNT)
            {
                stdAdj = netAdj;
            }

            return stdAdj;
        }

        private string GetColor(decimal? offeredPrice, decimal? escPrice, decimal? minPrice, decimal? listPrice)
        {
            string color = null;

            if (offeredPrice != null && escPrice != null && minPrice != null)
            {
                if (offeredPrice != null && offeredPrice == 0 && (listPrice == null || listPrice == 0))
                {
                    //Price = 0
                    color = null;
                }
                else if (offeredPrice >= escPrice)
                {
                    color = "White";
                }
                else if (offeredPrice < escPrice && offeredPrice > minPrice)
                {
                    //(Yellow)
                    color = "#FFFF00";
                }
                else if (offeredPrice <= minPrice)
                {
                    //(RED)
                    color = "#FF0000";
                }
            }

            return color;
        }

        private decimal FormatPrecisionCeiling(decimal? fieldValue)
        {
            var result = pricingHelper.ApplyRounding(fieldValue, 2, RoundingMode.UP);
            return result.Value;
        }
    }
}

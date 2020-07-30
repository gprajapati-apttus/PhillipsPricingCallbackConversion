using Apttus.Lightsaber.Extensibility.Framework.Library.Extension;
using Apttus.Lightsaber.Extensibility.Framework.Library.Interfaces;
using Apttus.Lightsaber.Pricing.Common.Constants;
using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Pricing.Common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apttus.Lightsaber.Phillips.Common;
using LineItem = Apttus.Lightsaber.Phillips.Common.LineItem;
using LineItemPropertyNames = Apttus.Lightsaber.Pricing.Common.Entities.LineItem.PropertyNames;

namespace Apttus.Lightsaber.Phillips.Totalling
{
    public class PricingTotallingCallbackHelper
    {
        private Proposal proposal;
        private Dictionary<string, PriceListItemQueryModel> pliDictionary;
        private IDBHelper dBHelper = null;
        private IPricingHelper pricingHelper = null;

        public PricingTotallingCallbackHelper(Proposal proposal, IDBHelper dBHelper, IPricingHelper pricingHelper)
        {
            this.proposal = proposal;
            this.dBHelper = dBHelper;
            this.pricingHelper = pricingHelper;
        }

        public async Task IncentiveAdjustmentUnitRounding(List<LineItem> cartLineItems)
        {
            foreach (LineItem cartLineItem in cartLineItems)
            {
                if (cartLineItem.IncentiveBasePrice.HasValue && cartLineItem.IncentiveBasePrice.Value != 0 && cartLineItem.BasePrice.HasValue && cartLineItem.BasePrice.Value != 0)
                {

                    decimal sellingTerm = cartLineItem.GetValuetOrDefault(LineItemPropertyNames.SellingTerm, 1);
                    decimal lineItemQ = cartLineItem.GetQuantity();

                    decimal? unitIncentiveAmount = cartLineItem.BasePrice - cartLineItem.IncentiveBasePrice;
                    unitIncentiveAmount = unitIncentiveAmount.HasValue ? pricingHelper.ApplyRounding(unitIncentiveAmount.Value, 2, RoundingMode.HALF_EVEN) : unitIncentiveAmount;

                    cartLineItem.APTS_Unit_Incentive_Adj_Amount__c = unitIncentiveAmount;
                    cartLineItem.IncentiveBasePrice = cartLineItem.BasePrice - unitIncentiveAmount;
                    cartLineItem.IncentiveAdjustmentAmount = unitIncentiveAmount * lineItemQ * sellingTerm * -1;
                }
            }

            await Task.CompletedTask;
        }

        public async Task SetDiscountWithAdjustmentSpread(List<LineItem> cartLineItems)
        {
            Dictionary<decimal, LineItem> bundleDiscountsDictionary = new Dictionary<decimal, LineItem>();

            foreach (var cartLineItem in cartLineItems)
            {
                decimal? promotionDiscount = null;
                
                if (cartLineItem.IncentiveAdjustmentAmount.HasValue)
                {
                    promotionDiscount = FormatPrecisionCeiling(cartLineItem.IncentiveAdjustmentAmount.Value) * -1;
                }

                cartLineItem.APTS_Promotion_Discount_Amount_c__c = promotionDiscount;

                if (!cartLineItem.IsOptionLine())
                {
                    bundleDiscountsDictionary.Add(cartLineItem.GetLineNumber(), cartLineItem);
                }

                decimal? adjustmentsAmount = bundleDiscountsDictionary[cartLineItem.GetLineNumber()].AdjustmentAmount;

                if (adjustmentsAmount.HasValue)
                {
                    adjustmentsAmount = FormatPrecisionCeiling(adjustmentsAmount.Value);
                }

                if(bundleDiscountsDictionary.ContainsKey(cartLineItem.GetLineNumber()) && cartLineItem.IsOptionLine() && cartLineItem.AllocateGroupAdjustment == true)
                {
                    if (bundleDiscountsDictionary[cartLineItem.GetLineNumber()].AdjustmentAmount != null &&
                           bundleDiscountsDictionary[cartLineItem.GetLineNumber()].AdjustmentType != null)
                    {
                        string bundleAdjTYpe = bundleDiscountsDictionary[cartLineItem.GetLineNumber()].AdjustmentType;
                        if (bundleAdjTYpe != Constants.PRICE_OVERRIDE && bundleAdjTYpe != Constants.DISCOUNT_AMOUNT)
                        {
                            cartLineItem.AdjustmentType = bundleAdjTYpe;
                            cartLineItem.AdjustmentAmount = adjustmentsAmount;
                        }
                    }
                    else if (cartLineItem.AdjustmentType == null)
                    {
                        cartLineItem.AdjustmentType = null;
                        cartLineItem.AdjustmentAmount = null;
                    }
                }
            }

            await Task.CompletedTask;
        }

        public async Task ComputeNetPriceAndNetAdjustment(List<LineItem> cartLineItems)
        {
            Dictionary<string, decimal> mapBundleAdjustments = new Dictionary<string, decimal>();
            Dictionary<string, PricePointsWrapper> mapPricePoints = new Dictionary<string, PricePointsWrapper>();

            foreach(var cartLineItem in cartLineItems)
            {
                cartLineItem.NetPrice = cartLineItem.GetLineType() == LineType.ProductService
                                    ? cartLineItem.APTS_Solution_Offered_Price__c
                                    : cartLineItem.APTS_Offered_Price_c__c;

                string uniqIdentifier = cartLineItem.GetLineNumber() + cartLineItem.ChargeType;
                
                if (cartLineItem.OptionId == null)
                {
                    if (!mapPricePoints.ContainsKey(uniqIdentifier + cartLineItem.PrimaryLineNumber))
                    {
                        mapPricePoints.Add(uniqIdentifier, new PricePointsWrapper(cartLineItem, pliDictionary.GetValueOrDefault(cartLineItem.PriceListItemId), pricingHelper));
                    }
                }
                else if (cartLineItem.OptionId != null && cartLineItem.Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c == Constants.CONFIGURATIONTYPE_BUNDLE)
                {
                    if (!mapPricePoints.ContainsKey(uniqIdentifier + cartLineItem.PrimaryLineNumber))
                    {
                        mapPricePoints.Add(uniqIdentifier + cartLineItem.PrimaryLineNumber, new PricePointsWrapper(cartLineItem, pliDictionary.GetValueOrDefault(cartLineItem.PriceListItemId), pricingHelper));
                    }
                }
                else
                {
                    if (cartLineItem.APTS_Extended_List_Price__c.HasValue && cartLineItem.IsOptional == false)
                    {
                        if (mapPricePoints.ContainsKey(uniqIdentifier))
                        {
                            mapPricePoints[uniqIdentifier].listPrice += cartLineItem.APTS_Extended_List_Price__c.Value;
                        }

                        if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItem.ParentBundleNumber))
                        {
                            mapPricePoints[uniqIdentifier + cartLineItem.ParentBundleNumber].listPrice += cartLineItem.APTS_Extended_List_Price__c.Value;
                        }
                    }
                }
            }

            foreach (var cartLineItem in cartLineItems)
            {
                string uniqIdentifier = cartLineItem.GetLineNumber() + cartLineItem.ChargeType;

                if (cartLineItem.OptionId == null && cartLineItem.AdjustmentType == null &&
                        cartLineItem.AdjustmentAmount.HasValue && cartLineItem.AdjustmentType == Constants.DISCOUNT_AMOUNT)
                {
                    decimal netAdjPercentage = 0;
                    decimal extendedListPrice = cartLineItem.APTS_Extended_List_Price__c.HasValue && cartLineItem.IsOptional == true 
                        ? 0 : cartLineItem.APTS_Extended_List_Price__c.Value;

                    var solutionListPrice = mapPricePoints.ContainsKey(uniqIdentifier + cartLineItem.PrimaryLineNumber) 
                        ? mapPricePoints[uniqIdentifier + cartLineItem.PrimaryLineNumber].listPrice + extendedListPrice 
                        : mapPricePoints[uniqIdentifier].listPrice + extendedListPrice;

                    cartLineItem.APTS_Solution_list_Price_c__c = solutionListPrice;
                    
                    if (solutionListPrice.HasValue)
                    {
                        netAdjPercentage = -1 * FormatPrecisionCeiling(cartLineItem.AdjustmentAmount.Value) / FormatPrecisionCeiling(solutionListPrice.Value) * 100;
                    }

                    mapBundleAdjustments.Add(uniqIdentifier, netAdjPercentage);
                }
            }

            foreach (var cartLineItem in cartLineItems)
            {
                if (cartLineItem.AdjustmentType != null && cartLineItem.AdjustmentAmount != null)
                {
                    if (cartLineItem.AdjustmentType == Constants.DISCOUNT_PERCENTAGE)
                    {
                        cartLineItem.NetAdjustmentPercent = -1 * FormatPrecisionCeiling(cartLineItem.AdjustmentAmount.Value);

                    }
                    else if (cartLineItem.AdjustmentType == Constants.DISCOUNT_AMOUNT)
                    {
                        if (mapBundleAdjustments.ContainsKey(cartLineItem.GetLineNumber() + cartLineItem.ChargeType) && cartLineItem.AllocateGroupAdjustment == true)
                        {
                            cartLineItem.NetAdjustmentPercent = mapBundleAdjustments[cartLineItem.GetLineNumber() + cartLineItem.ChargeType];

                            if (cartLineItem.OptionId != null)
                            {
                                cartLineItem.AdjustmentAmount = 0;
                            }
                        }
                        else if (cartLineItem.APTS_Philips_List_Price__c.HasValue 
                            && cartLineItem.APTS_Philips_List_Price__c.Value != 0)
                        {
                            cartLineItem.NetAdjustmentPercent = -1 * FormatPrecisionCeiling(cartLineItem.AdjustmentAmount.Value) 
                                / FormatPrecisionCeiling(cartLineItem.APTS_Philips_List_Price__c.Value) * 100;
                        }
                    }
                }
                else if (mapBundleAdjustments.ContainsKey(cartLineItem.GetLineNumber() + cartLineItem.ChargeType)
                         && cartLineItem.APTS_Philips_List_Price__c.HasValue 
                         && cartLineItem.APTS_Philips_List_Price__c.Value > 0
                         && cartLineItem.AllocateGroupAdjustment == true)
                {
                    cartLineItem.NetAdjustmentPercent = mapBundleAdjustments[cartLineItem.GetLineNumber() + cartLineItem.ChargeType];
                }

                cartLineItem.APTS_Net_Adjustment_Percent_c__c = cartLineItem.NetAdjustmentPercent;
            }

            await Task.CompletedTask;
        }

        public async Task CalculatePricePointsForBundle(List<LineItem> cartLineItems)
        {
            Dictionary<string, PricePointsWrapper> mapPricePoints = new Dictionary<string, PricePointsWrapper>();
            string bundleAdjustmentType = string.Empty;
            string uniqIdentifier_li = string.Empty;

            Dictionary<string, string> mapBundleAdjustment = new Dictionary<string, string>();

            foreach (var cartLineItem in cartLineItems)
            {
                var priceListItem = cartLineItem.GetPriceListItem();
                string uniqIdentifier = cartLineItem.GetLineNumber() + cartLineItem.ChargeType;
                uniqIdentifier_li = uniqIdentifier;

                var productType = cartLineItem.Apttus_Config2__ProductId__r_Apttus_Config2__ProductType__c;
                var configurationType = cartLineItem.Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c;

                if (productType == Constants.SOLUTION_SUBSCRIPTION && (cartLineItem.GetLineType() == LineType.ProductService || configurationType == Constants.CONFIGURATIONTYPE_BUNDLE))
                {
                    uniqIdentifier_li = cartLineItem.GetLineNumber() + cartLineItem.PrimaryLineNumber + cartLineItem.ChargeType;
                }
                else if (productType == Constants.SOLUTION_SUBSCRIPTION && configurationType == Constants.CONFIGURATIONTYPE_OPTION)
                {
                    uniqIdentifier_li = cartLineItem.GetLineNumber() + cartLineItem.ParentBundleNumber + cartLineItem.ChargeType;
                }

                if (cartLineItem.OptionId == null)
                {
                    if (!mapPricePoints.ContainsKey(uniqIdentifier + cartLineItem.PrimaryLineNumber))
                    {
                        mapPricePoints.Add(uniqIdentifier, new PricePointsWrapper(cartLineItem, pliDictionary.GetValueOrDefault(cartLineItem.PriceListItemId), pricingHelper));
                    }

                    mapBundleAdjustment.Add(uniqIdentifier_li, cartLineItem.AdjustmentType);
                }
                else if (cartLineItem.OptionId != null && configurationType == Constants.CONFIGURATIONTYPE_BUNDLE && productType == Constants.SOLUTION_SUBSCRIPTION)
                {
                    if (!mapPricePoints.ContainsKey(uniqIdentifier + cartLineItem.PrimaryLineNumber))
                    {
                        mapPricePoints.Add(uniqIdentifier + cartLineItem.PrimaryLineNumber, 
                            new PricePointsWrapper(cartLineItem, pliDictionary.GetValueOrDefault(cartLineItem.PriceListItemId), pricingHelper));
                    }

                    mapBundleAdjustment.Add(uniqIdentifier_li, cartLineItem.AdjustmentType);
                }
                else
                {
                    if (mapPricePoints.ContainsKey(uniqIdentifier))
                    {
                        decimal? targetPrice = 0;
                        decimal? minPrice = 0;
                        decimal? optEscPrice = 0;
                        decimal? optionOfferedPrice = 0;
                        decimal? sellingTerm = cartLineItem.GetValuetOrDefault(LineItemPropertyNames.SellingTerm, 1);
                        decimal? extQty = cartLineItem.APTS_Extended_Quantity__c.HasValue 
                            && cartLineItem.APTS_Extended_Quantity__c.Value != 0 ? cartLineItem.APTS_Extended_Quantity__c.Value : 1;

                        bundleAdjustmentType = mapBundleAdjustment.GetValueOrDefault(uniqIdentifier_li) != null ? mapBundleAdjustment.GetValueOrDefault(uniqIdentifier_li) : string.Empty;

                        var extendedListPrice = cartLineItem.APTS_Extended_List_Price__c;

                        if (extendedListPrice.HasValue && cartLineItem.IsOptional== false)
                        {
                            mapPricePoints[uniqIdentifier].listPrice += extendedListPrice.Value;

                            if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItem.ParentBundleNumber))
                            {
                                mapPricePoints[uniqIdentifier + cartLineItem.ParentBundleNumber].listPrice += extendedListPrice.Value;
                            }
                        }

                        decimal listPrice = extendedListPrice.HasValue ? extendedListPrice.Value : 0;
                        decimal contractDiscount = cartLineItem.APTS_ContractDiscount__c.HasValue 
                            ? cartLineItem.APTS_ContractDiscount__c.Value : 0;
                        decimal contractAmt = listPrice * contractDiscount / 100;

                        cartLineItem.APTS_Contract_Discount_Amount__c = contractAmt;
                        decimal? netAdjPercent = cartLineItem.NetAdjustmentPercent;

                        decimal? stdAdj = GetStrategicDiscount(netAdjPercent, cartLineItem.AdjustmentType, listPrice);
                        decimal incAdjAmt = cartLineItem.IncentiveAdjustmentAmount != null ? FormatPrecisionCeiling(cartLineItem.IncentiveAdjustmentAmount.Value) : 0;

                        decimal? unitStrategicDiscountAmount = (listPrice / extQty / sellingTerm) * stdAdj / 100;
                        decimal? strategicDiscountAmount = listPrice * stdAdj / 100;

                        if (productType != null && productType.Contains("Solution"))
                        {
                            if (cartLineItem.APTS_Billing_Plan__c != null && cartLineItem.APTS_Billing_Plan__c == Constants.BILLING_PLAN_ANNUAL)
                            {
                                var roundedUnitStrategicDisount = pricingHelper.ApplyRounding(unitStrategicDiscountAmount * 12, 2, RoundingMode.HALF_EVEN);
                                cartLineItem.APTS_Unit_Strategic_Discount_Amount__c = roundedUnitStrategicDisount;
                            }
                            else
                            {
                                var roundedUnitStrategicDisount = pricingHelper.ApplyRounding(unitStrategicDiscountAmount, 2, RoundingMode.HALF_EVEN);
                                cartLineItem.APTS_Unit_Strategic_Discount_Amount__c = roundedUnitStrategicDisount;
                            }
                        }
                        else if (productType == Constants.SERVICE_PRODUCT_TYPE)
                        {
                            cartLineItem.APTS_Unit_Strategic_Discount_Amount__c = unitStrategicDiscountAmount;
                            cartLineItem.APTS_Strategic_Discount_Amount_c__c = strategicDiscountAmount;
                        }
                        else
                        {
                            var roundedUnitStrategicDisount = pricingHelper.ApplyRounding(unitStrategicDiscountAmount, 2, RoundingMode.HALF_EVEN);
                            cartLineItem.APTS_Unit_Strategic_Discount_Amount__c = roundedUnitStrategicDisount;

                            cartLineItem.APTS_Strategic_Discount_Amount_c__c = roundedUnitStrategicDisount * sellingTerm * extQty;
                        }

                        if (productType != null && productType.Contains("Solution"))
                        {
                            if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItem.ParentBundleNumber))
                            {
                                if (cartLineItem.APTS_SAP_List_Price__c.HasValue)
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItem.ParentBundleNumber].sapListPrice += cartLineItem.APTS_SAP_List_Price__c.Value;
                                }
                                if (cartLineItem.APTS_Unit_Strategic_Discount_Amount__c.HasValue)
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItem.ParentBundleNumber].unitStrategicDiscountAmount 
                                        += cartLineItem.APTS_Unit_Strategic_Discount_Amount__c.Value;
                                }
                            }
                        }

                        if (cartLineItem.APTS_Target_Price__c.HasValue)
                        {
                            targetPrice = FormatPrecisionCeiling(cartLineItem.APTS_Target_Price__c.Value);
                            if (cartLineItem.IsOptional == false && targetPrice != null)
                            {
                                mapPricePoints[uniqIdentifier].targetPrice += targetPrice;
                                if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItem.ParentBundleNumber))
                                    mapPricePoints[uniqIdentifier + cartLineItem.ParentBundleNumber].targetPrice += targetPrice;
                            }
                        }

                        if (cartLineItem.APTS_Minimum_Price__c.HasValue)
                        {
                            minPrice = FormatPrecisionCeiling(cartLineItem.APTS_Minimum_Price__c.Value);
                            if (cartLineItem.IsOptional == false)
                            {
                                mapPricePoints[uniqIdentifier].minPrice += minPrice;
                                if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItem.ParentBundleNumber))
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItem.ParentBundleNumber].minPrice += minPrice;
                                }
                            }
                        }

                        if (cartLineItem.APTS_Escalation_Price__c.HasValue)
                        {
                            optEscPrice = FormatPrecisionCeiling(cartLineItem.APTS_Escalation_Price__c.Value);
                            if (cartLineItem.IsOptional == false)
                            {
                                mapPricePoints[uniqIdentifier].preEscalationPrice += optEscPrice;
                                if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItem.ParentBundleNumber))
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItem.ParentBundleNumber].preEscalationPrice += optEscPrice;
                                }
                            }
                        }

                        if (cartLineItem.ListPrice.HasValue && cartLineItem.APTS_Local_Bundle_Component_Flag__c != true)
                        {
                            decimal? qty = cartLineItem.GetQuantity();

                            if (cartLineItem.IsOptional == false)
                            {
                                var countryPriceListListPrice = pliDictionary.GetValueOrDefault(cartLineItem.PriceListItemId)?.APTS_Country_Pricelist_List_Price__c;

                                mapPricePoints[uniqIdentifier].optionPrice += (countryPriceListListPrice == null) 
                                    ? FormatPrecisionCeiling(cartLineItem.ListPrice.Value) * qty : countryPriceListListPrice.Value * qty;

                                if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItem.ParentBundleNumber))
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItem.ParentBundleNumber].optionPrice += (countryPriceListListPrice == null) 
                                        ? FormatPrecisionCeiling(cartLineItem.ListPrice.Value) * qty : countryPriceListListPrice.Value * qty;
                                }
                            }
                        }

                        if (cartLineItem.APTS_Contract_Net_Price__c.HasValue && cartLineItem.IsOptional == false)
                        {
                            mapPricePoints[uniqIdentifier].contractNetPrice += FormatPrecisionCeiling(cartLineItem.APTS_Contract_Net_Price__c.Value);

                            if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItem.ParentBundleNumber))
                            {
                                mapPricePoints[uniqIdentifier + cartLineItem.ParentBundleNumber].contractNetPrice += 
                                    FormatPrecisionCeiling(cartLineItem.APTS_Contract_Net_Price__c.Value);
                            }
                        }

                        if (cartLineItem.ExtendedPrice.HasValue && cartLineItem.IsOptional == false)
                        {
                            mapPricePoints[uniqIdentifier].bundleExtendedPrice += mapPricePoints[uniqIdentifier].qty != null 
                                ? FormatPrecisionCeiling(cartLineItem.ExtendedPrice.Value) * mapPricePoints[uniqIdentifier].qty : FormatPrecisionCeiling(cartLineItem.ExtendedPrice.Value);

                            if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItem.ParentBundleNumber))
                            {
                                mapPricePoints[uniqIdentifier + cartLineItem.ParentBundleNumber].bundleExtendedPrice += 
                                    mapPricePoints[uniqIdentifier + cartLineItem.ParentBundleNumber].qty != null 
                                    ? FormatPrecisionCeiling(cartLineItem.ExtendedPrice.Value) * mapPricePoints[uniqIdentifier + cartLineItem.ParentBundleNumber].qty 
                                    : FormatPrecisionCeiling(cartLineItem.ExtendedPrice.Value);
                            }
                        }

                        if (cartLineItem.APTS_ContractDiscount__c.HasValue && priceListItem.Entity.ListPrice.HasValue)
                        {
                            PricePointsWrapper points = mapPricePoints[uniqIdentifier];
                            if (points.solutionContractDiscountAmount == null)
                            {
                                points.solutionContractDiscountAmount = 0;
                            }

                            if (cartLineItem.IsOptional == false)
                            {
                                mapPricePoints[uniqIdentifier].solutionContractDiscountAmount += contractAmt;
                                if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItem.ParentBundleNumber))
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItem.ParentBundleNumber].solutionContractDiscountAmount += contractAmt;
                                }
                            }
                        }

                        if (cartLineItem.IncentiveAdjustmentAmount.HasValue && cartLineItem.IncentiveAdjustmentAmount.Value < 0 && cartLineItem.IsOptional == false)
                        {
                            var unitIncentiveAdjAmount = cartLineItem.APTS_Unit_Incentive_Adj_Amount__c;
                            mapPricePoints[uniqIdentifier].incentiveAdjAmount += (FormatPrecisionCeiling(cartLineItem.IncentiveAdjustmentAmount.Value) * -1);

                            if (unitIncentiveAdjAmount.HasValue)
                            {
                                mapPricePoints[uniqIdentifier].solutionUnitIncentiveAmount += unitIncentiveAdjAmount.Value * -1;
                            }

                            if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItem.ParentBundleNumber))
                            {
                                mapPricePoints[uniqIdentifier + cartLineItem.ParentBundleNumber].incentiveAdjAmount 
                                    += (FormatPrecisionCeiling(cartLineItem.IncentiveAdjustmentAmount.Value) * -1);

                                if (unitIncentiveAdjAmount.HasValue)
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItem.ParentBundleNumber].solutionUnitIncentiveAmount += unitIncentiveAdjAmount.Value * -1;
                                }
                            }
                        }
                        else if (cartLineItem.IncentiveAdjustmentAmount.HasValue && cartLineItem.IncentiveAdjustmentAmount.Value > 0 && cartLineItem.IsOptional == false)
                        {
                            var unitIncentiveAdjAmount = cartLineItem.APTS_Unit_Incentive_Adj_Amount__c;
                            mapPricePoints[uniqIdentifier].incentiveAdjAmount += (FormatPrecisionCeiling(cartLineItem.IncentiveAdjustmentAmount.Value));

                            if (unitIncentiveAdjAmount.HasValue)
                            {
                                mapPricePoints[uniqIdentifier].solutionUnitIncentiveAmount += unitIncentiveAdjAmount.Value;
                            }

                            if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItem.ParentBundleNumber))
                            {
                                mapPricePoints[uniqIdentifier + cartLineItem.ParentBundleNumber].incentiveAdjAmount += (FormatPrecisionCeiling(cartLineItem.IncentiveAdjustmentAmount.Value));

                                if (unitIncentiveAdjAmount.HasValue)
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItem.ParentBundleNumber].solutionUnitIncentiveAmount += unitIncentiveAdjAmount.Value;
                                }
                            }
                        }

                        if (cartLineItem.APTS_Offered_Price_c__c.HasValue)
                        {
                            optionOfferedPrice = cartLineItem.APTS_Extended_List_Price__c.HasValue 
                                ? cartLineItem.APTS_Extended_List_Price__c.Value : 0;
                            
                            decimal? totalDiscounts = 0;
                            totalDiscounts = totalDiscounts - contractAmt;

                            if (cartLineItem.IncentiveAdjustmentAmount.HasValue)
                            {
                                decimal incAdjAmt1 = FormatPrecisionCeiling(cartLineItem.IncentiveAdjustmentAmount.Value);
                                totalDiscounts = totalDiscounts + incAdjAmt1;
                            }
                            if (cartLineItem.NetAdjustmentPercent.HasValue)
                            {
                                totalDiscounts = totalDiscounts - cartLineItem.APTS_Strategic_Discount_Amount_c__c;
                            }

                            optionOfferedPrice = optionOfferedPrice + totalDiscounts;

                            if (cartLineItem.IsOptional == false && optionOfferedPrice.HasValue)
                            {
                                mapPricePoints[uniqIdentifier].offeredPrice += optionOfferedPrice;

                                if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItem.ParentBundleNumber))
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItem.ParentBundleNumber].offeredPrice += optionOfferedPrice;
                                }
                            }
                        }

                        if (optEscPrice != null && optEscPrice != 0)
                        {
                            var escalationPriceAttainment = (optionOfferedPrice / FormatPrecisionCeiling(cartLineItem.APTS_Escalation_Price__c.Value)) * 100;
                            cartLineItem.APTS_Escalation_Price_Attainment_c__c = escalationPriceAttainment;
                        }

                        if (targetPrice != null && targetPrice != 0)
                        {
                            var targetPriceAttainment = (optionOfferedPrice / (FormatPrecisionCeiling(cartLineItem.APTS_Target_Price__c.Value))) * 100;
                            cartLineItem.APTS_Target_Price_Attainment__c = targetPriceAttainment;
                        }

                        if (minPrice != null && minPrice != 0)
                        {
                            var minimumPriceAttainment = (optionOfferedPrice / (FormatPrecisionCeiling(cartLineItem.APTS_Minimum_Price__c.Value))) * 100;
                            cartLineItem.APTS_Minimum_Price_Attainment_c__c = minimumPriceAttainment;
                        }

                        cartLineItem.APTS_Price_Attainment_Color__c = 
                            GetColor(optionOfferedPrice, optEscPrice, minPrice, cartLineItem.APTS_Extended_List_Price__c);
                    }
                }
            }

            foreach (var cartLineItem in cartLineItems)
            {
                decimal? sellingTerm = cartLineItem.SellingTerm;
                decimal? extQty = cartLineItem.GetValuetOrDefault(LineItemCustomField.APTS_Extended_Quantity__c, 1);
                string uniqIdentifier = cartLineItem.GetLineNumber() + cartLineItem.ChargeType;
                uniqIdentifier_li = uniqIdentifier;

                var productType = cartLineItem.Apttus_Config2__ProductId__r_Apttus_Config2__ProductType__c;
                var configurationType = cartLineItem.Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c;

                if (productType == Constants.SOLUTION_SUBSCRIPTION && (cartLineItem.GetLineType() == LineType.ProductService || configurationType == Constants.CONFIGURATIONTYPE_BUNDLE))
                {
                    uniqIdentifier_li = cartLineItem.GetLineNumber() + cartLineItem.PrimaryLineNumber + cartLineItem.ChargeType;
                }
                else if (productType == Constants.SOLUTION_SUBSCRIPTION && configurationType == Constants.CONFIGURATIONTYPE_OPTION)
                {
                    uniqIdentifier_li = cartLineItem.GetLineNumber() + cartLineItem.ParentBundleNumber + cartLineItem.ChargeType;
                }

                bundleAdjustmentType = mapBundleAdjustment[uniqIdentifier_li] != null ? mapBundleAdjustment[uniqIdentifier_li] : string.Empty;

                if (cartLineItem.OptionId != null && configurationType == Constants.CONFIGURATIONTYPE_BUNDLE)
                {
                    uniqIdentifier += cartLineItem.PrimaryLineNumber;
                }

                if (mapPricePoints.ContainsKey(uniqIdentifier))
                {
                    if (cartLineItem.OptionId == null || (cartLineItem.OptionId != null && configurationType == Constants.CONFIGURATIONTYPE_BUNDLE))
                    {
                        var extListPrice = cartLineItem.APTS_Extended_List_Price__c;

                        decimal extendedListPrice = cartLineItem.IsOptional == true ? 0 : extListPrice.Value;
                        
                        if (extListPrice.HasValue)
                        {
                            cartLineItem.APTS_Solution_list_Price_c__c = mapPricePoints[uniqIdentifier].listPrice + extendedListPrice;
                        }

                        decimal listPrice = extListPrice.HasValue ? extListPrice.Value : 0;
                        decimal? bundleOfferedPrice = extListPrice.HasValue ? extListPrice.Value : 0;
                        decimal? totalDiscounts = 0;
                        decimal? contractAmt = 0;
                        decimal? unitContractAmt = 0;

                        if (cartLineItem.APTS_ContractDiscount__c.HasValue)
                        {
                            decimal contractDiscount = cartLineItem.APTS_ContractDiscount__c.Value;
                            unitContractAmt = ((bundleOfferedPrice / extQty / sellingTerm) * contractDiscount / 100);
                            unitContractAmt = pricingHelper.ApplyRounding(unitContractAmt, 2, RoundingMode.HALF_EVEN);
                            contractAmt = unitContractAmt * extQty * sellingTerm;
                            totalDiscounts = totalDiscounts - contractAmt;
                        }
                        if (cartLineItem.IncentiveAdjustmentAmount.HasValue)
                        {
                            totalDiscounts = totalDiscounts + FormatPrecisionCeiling(cartLineItem.IncentiveAdjustmentAmount.Value);
                        }
                        if (cartLineItem.NetAdjustmentPercent.HasValue)
                        {
                            decimal? stdAdj = GetStrategicDiscount(cartLineItem.NetAdjustmentPercent.Value, cartLineItem.AdjustmentType, listPrice);
                            decimal? incAdjAmt = cartLineItem.IncentiveAdjustmentAmount.HasValue ? FormatPrecisionCeiling(cartLineItem.IncentiveAdjustmentAmount.Value) : 0;

                            decimal? unitStrategicDiscountAmount = (listPrice / extQty / sellingTerm) * stdAdj / 100;
                            decimal? strategicDiscountAmount = listPrice * stdAdj / 100;

                            if (productType == Constants.SERVICE_PRODUCT_TYPE)
                            {
                                cartLineItem.APTS_Unit_Strategic_Discount_Amount__c = unitStrategicDiscountAmount;
                                cartLineItem.APTS_Strategic_Discount_Amount_c__c = strategicDiscountAmount;
                            }
                            else
                            {
                                cartLineItem.APTS_Unit_Strategic_Discount_Amount__c = pricingHelper.ApplyRounding(unitStrategicDiscountAmount, 2, RoundingMode.HALF_EVEN);
                                cartLineItem.APTS_Strategic_Discount_Amount_c__c =
                                    pricingHelper.ApplyRounding(unitStrategicDiscountAmount, 2, RoundingMode.HALF_EVEN) * sellingTerm * extQty;
                            }

                            totalDiscounts = totalDiscounts - cartLineItem.APTS_Strategic_Discount_Amount_c__c;
                        }

                        bundleOfferedPrice = bundleOfferedPrice + totalDiscounts;

                        if (productType != null && productType.Contains("Solution") && cartLineItem.OptionId != null && configurationType == Constants.CONFIGURATIONTYPE_BUNDLE)
                        {
                            var billingPlan = cartLineItem.APTS_Billing_Plan__c;

                            if (billingPlan != null && (billingPlan == Constants.BILLING_PLAN_MONTHLY || billingPlan == Constants.BILLING_PLAN_QUATERLY || billingPlan == Constants.BILLING_PLAN_ANNUAL))
                            {
                                cartLineItem.APTS_SAP_List_Price__c = mapPricePoints[uniqIdentifier].sapListPrice;
                            }

                            cartLineItem.APTS_Unit_Strategic_Discount_Amount__c = mapPricePoints[uniqIdentifier].unitStrategicDiscountAmount;
                        }
                        
                        decimal? optEscPrice = FormatPrecisionCeiling(cartLineItem.APTS_Escalation_Price__c.Value);
                        if (optEscPrice != null && optEscPrice != 0)
                        {
                            decimal? escalationPriceAttainment;
                            if (!Constants.listTradeSpoo.Contains(cartLineItem.Apttus_Config2__ProductId__r_APTS_SPOO_Type__c))
                            {
                                escalationPriceAttainment = (bundleOfferedPrice / optEscPrice) * 100;
                            }
                            else
                            {
                                escalationPriceAttainment = bundleOfferedPrice != 0 ? (optEscPrice / bundleOfferedPrice) * 100 : 0;
                            }

                            cartLineItem.APTS_Escalation_Price_Attainment_c__c = escalationPriceAttainment;
                        }
                        decimal? minPrice = FormatPrecisionCeiling(cartLineItem.APTS_Minimum_Price__c);
                        if (minPrice != null && minPrice != 0)
                        {
                            decimal? minimumPriceAttainment;
                            if (!Constants.listTradeSpoo.Contains(cartLineItem.Apttus_Config2__ProductId__r_APTS_SPOO_Type__c))
                            {
                                minimumPriceAttainment = (bundleOfferedPrice / minPrice) * 100;
                            }
                            else
                            {
                                minimumPriceAttainment = bundleOfferedPrice != 0 ? (minPrice / bundleOfferedPrice) * 100 : 0;
                            }

                            cartLineItem.APTS_Minimum_Price_Attainment_c__c = minimumPriceAttainment;
                        }
                        decimal? targetPrice = FormatPrecisionCeiling(cartLineItem.APTS_Target_Price__c);
                        if (targetPrice != null && targetPrice != 0)
                        {
                            decimal? targetPriceAttainment;
                            if (!Constants.listTradeSpoo.Contains(cartLineItem.Apttus_Config2__ProductId__r_APTS_SPOO_Type__c))
                            {
                                targetPriceAttainment = (bundleOfferedPrice / targetPrice) * 100;
                            }
                            else
                            {
                                targetPriceAttainment = bundleOfferedPrice != 0 ? (targetPrice / bundleOfferedPrice) * 100 : 0;
                            }

                            cartLineItem.APTS_Target_Price_Attainment__c = targetPriceAttainment;
                        }

                        cartLineItem.APTS_Price_Attainment_Color__c = 
                            GetColor(bundleOfferedPrice, optEscPrice, minPrice, cartLineItem.APTS_Extended_List_Price__c);

                        decimal? solMinPrice = cartLineItem.IsOptional == true 
                            ? mapPricePoints[uniqIdentifier].minPrice 
                            : mapPricePoints[uniqIdentifier].minPrice + FormatPrecisionCeiling(cartLineItem.APTS_Minimum_Price__c);

                        cartLineItem.APTS_Minimum_Price_Bundle__c = solMinPrice;
                        decimal? solEscPrice = cartLineItem.IsOptional == true ? mapPricePoints[uniqIdentifier].preEscalationPrice : mapPricePoints[uniqIdentifier].preEscalationPrice + optEscPrice;
                        cartLineItem.APTS_Escalation_Price_Bundle__c = solEscPrice;

                        if (cartLineItem.APTS_Country_Target_Price__c.HasValue)
                        {
                            cartLineItem.APTS_Target_Price_Bundle__c =
                                cartLineItem.IsOptional == true 
                                ? mapPricePoints[uniqIdentifier].targetPrice 
                                : mapPricePoints[uniqIdentifier].targetPrice + FormatPrecisionCeiling(cartLineItem.APTS_Target_Price__c);
                        }

                        cartLineItem.APTS_Option_List_Price__c = mapPricePoints[uniqIdentifier].optionPrice;
                        cartLineItem.APTS_Contract_Discount_Amount__c = mapPricePoints[uniqIdentifier].contractDiscountAmount;
                        cartLineItem.APTS_Solution_Contract_Discount_Amount__c = mapPricePoints[uniqIdentifier].solutionContractDiscountAmount;
                        cartLineItem.APTS_Incentive_Adjustment_Amount_Bundle__c = mapPricePoints[uniqIdentifier].incentiveAdjAmount;
                        cartLineItem.APTS_Solution_Unit_Incentive_Adj_Amount__c = mapPricePoints[uniqIdentifier].solutionUnitIncentiveAmount;

                        decimal? solOfferedPrice = mapPricePoints[uniqIdentifier].offeredPrice;
                        cartLineItem.APTS_Solution_Offered_Price_c__c = solOfferedPrice;
                        cartLineItem.NetPrice = solOfferedPrice + bundleOfferedPrice;

                        if (cartLineItem.APTS_Solution_List_Price_c__c.HasValue && cartLineItem.APTS_Solution_Offered_Price__c.HasValue)
                        {
                            var totalResultingDiscountAmount = cartLineItem.APTS_Solution_List_Price_c__c.Value - 
                                (cartLineItem.APTS_Solution_Offered_Price__c.Value + bundleOfferedPrice);
                            
                            cartLineItem.APTS_Total_Resulting_Discount_Amount__c = totalResultingDiscountAmount;
                        }

                        cartLineItem.APTS_Solution_Price_Attainment_Color__c = 
                            GetColor(solOfferedPrice + bundleOfferedPrice, solEscPrice, solMinPrice, cartLineItem.APTS_Extended_List_Price__c);
                    }
                }
            }

            await Task.CompletedTask;
        }

        public async Task PopulatePLICustomFields(List<LineItem> cartLineItems)
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

        public async Task PopulateCustomFields(List<LineItem> cartLineItems)
        {
            foreach (var cartLineItem in cartLineItems)
            {
                var pliCustomFields = pliDictionary.GetValueOrDefault(cartLineItem.PriceListItemId);

                if (pliCustomFields != null && pliCustomFields.APTS_Country_Pricelist_List_Price__c != null)
                {
                    cartLineItem.APTS_Payment_Term__c = pliCustomFields.Apttus_Config2__PriceListId__r.APTS_Payment_Term_Credit_Terms__c;
                    cartLineItem.APTS_Inco_Term__c = pliCustomFields.Apttus_Config2__PriceListId__r.APTS_Inco_Terms__c;
                }
                else
                {
                    cartLineItem.APTS_Payment_Term__c = proposal.Apttus_Proposal__Payment_Term__c;
                    cartLineItem.APTS_Inco_Term__c = proposal.APTS_Inco_Term__c;
                }

                if (cartLineItem.GetLineType() == LineType.Option)
                {
                    if (cartLineItem.Apttus_Config2__OptionId__r_Main_Article_Group_ID__c != null)
                        cartLineItem.APTS_MAG__c = cartLineItem.Apttus_Config2__OptionId__r_Main_Article_Group_ID__c;

                    if (cartLineItem.Apttus_Config2__OptionId__r_Business_Unit_ID__c != null)
                        cartLineItem.APTS_Business_Unit__c = cartLineItem.Apttus_Config2__OptionId__r_Business_Unit_ID__c;
                }
                else
                {
                    if (cartLineItem.Apttus_Config2__ProductId__r_Main_Article_Group_ID__c != null)
                        cartLineItem.APTS_MAG__c = cartLineItem.Apttus_Config2__ProductId__r_Main_Article_Group_ID__c;

                    if (cartLineItem.Apttus_Config2__ProductId__r_Business_Unit_ID__c != null)
                        cartLineItem.APTS_Business_Unit__c = cartLineItem.Apttus_Config2__ProductId__r_Business_Unit_ID__c;
                }

                var escalationPriceAttainment = cartLineItem.APTS_Escalation_Price_Attainment__c;
                if (escalationPriceAttainment.HasValue)
                {
                    cartLineItem.APTS_Is_Escalation_Price_Attained__c = 
                        !((escalationPriceAttainment.Value < 100 && cartLineItem.NetPrice > 0) || (escalationPriceAttainment.Value > 100 && cartLineItem.NetPrice < 0));
                }

                var minimumPriceAttainment = cartLineItem.APTS_Minimum_Price_Attainment__c;
                if (minimumPriceAttainment.HasValue)
                {
                    cartLineItem.APTS_Is_Minimum_Price_Attained__c =
                        !((minimumPriceAttainment.Value < 100 && cartLineItem.NetPrice > 0) || (minimumPriceAttainment.Value > 100 && cartLineItem.NetPrice < 0));
                }
            }

            await Task.CompletedTask;
        }

        public async Task SetRollupsAndThresholdFlags(ProductConfigurationModel cart, List<LineItem> cartLineItems)
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
                if (cartLineItem.Apttus_Config2__ProductId__r_APTS_Type__c == "SPOO"
                    && cartLineItem.Apttus_Config2__ProductId__r_APTS_SPOO_Type__c == "3rd Party"
                    && cartLineItem.NetPrice.HasValue)
                {
                    CalculatedThreshold_3rdP = CalculatedThreshold_3rdP + FormatPrecisionCeiling(cartLineItem.NetPrice.Value);
                }

                if (cartLineItem.GetLineType() == LineType.ProductService && cartLineItem.NetPrice.HasValue)
                {
                    CalculatedThreshold_QT = CalculatedThreshold_QT + FormatPrecisionCeiling(cartLineItem.NetPrice.Value);
                }

                if (mapClogToThresoldValue.ContainsKey(cartLineItem.Apttus_Config2__ProductId__r_APTS_CLOGS__c)
                            && cartLineItem.APTS_Extended_List_Price__c >= 
                            mapClogToThresoldValue.GetValueOrDefault(cartLineItem.Apttus_Config2__ProductId__r_APTS_CLOGS__c))
                {
                    cartLineItem.APTS_Procurement_approval_needed__c = true;
                }
                else
                {
                    cartLineItem.APTS_Procurement_approval_needed__c = false;
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

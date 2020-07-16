using Apttus.Lightsaber.Extensibility.Framework.Library;
using Apttus.Lightsaber.Extensibility.Framework.Library.Interfaces;
using Apttus.Lightsaber.Pricing.Common.Callback;
using Apttus.Lightsaber.Pricing.Common.Constants;
using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Pricing.Common.Formula;
using Apttus.Lightsaber.Pricing.Common.Messages.Cart;
using Apttus.Lightsaber.Pricing.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apttus.Lightsaber.Nokia.Common;
using LineItem = Apttus.Lightsaber.Nokia.Common.LineItem;

namespace Apttus.Lightsaber.Nokia.Totalling
{
    public class PricingTotallingCallback : CodeExtensibility, IPricingTotallingCallback
    {
        private List<LineItem> cartLineItems = null;
        private Proposal proposal = null;
        private IDBHelper dBHelper = null;
        private IPricingHelper pricingHelper = null;
        private decimal? conversionRate;
        private Dictionary<string, List<LineItem>> lineItemIRPMapDirect = new Dictionary<string, List<LineItem>>();
        private Dictionary<string, List<LineItem>> primaryNoLineItemList = new Dictionary<string, List<LineItem>>();
        private Dictionary<string, LineItem> lineItemObjectMap = new Dictionary<string, LineItem>();

        public async Task BeforePricingCartAdjustmentAsync(AggregateCartRequest aggregateCartRequest)
        {
            //GP: Start Method
            decimal? minMaintPrice_EP = null;
            decimal? minMaintPrice = null;
            string isIONExistingContract_EP = string.Empty;
            string maintenanceType = string.Empty;

            HashSet<string> sspFNSet = new HashSet<string>(Labels.FN_SSP_Product);

            cartLineItems = aggregateCartRequest.CartContext.LineItems.SelectMany(x => x.ChargeLines).Select(s => new LineItem(s)).ToList();
            proposal = new Proposal(aggregateCartRequest.Cart.Get<Dictionary<string, object>>(Constants.PROPOSAL));
            dBHelper = GetDBHelper();
            pricingHelper = GetPricingHelper();

            foreach (var lineItem in cartLineItems)
            {
                if (lineItem.GetLineType() == LineType.ProductService)
                {
                    lineItemObjectMap.TryAdd(Constants.NOKIA_PRODUCT_SERVICES + Constants.NOKIA_UNDERSCORE + lineItem.GetLineNumber(), lineItem);
                }
            }

            if (Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Quote_Type__c))
            {
                var defaultExchangeRateQuery = QueryHelper.GetDefaultExchangeRateQuery(proposal.CurrencyIsoCode);
                conversionRate = (await dBHelper.FindAsync<CurrencyTypeQueryModel>(defaultExchangeRateQuery)).FirstOrDefault()?.ConversionRate;

                if (proposal.NokiaCPQ_Portfolio__c == Constants.NOKIA_IP_ROUTING && proposal.Is_List_Price_Only__c == false)
                {
                    var shippingLocationQuery = QueryHelper.GetShippingLocationForDirectQuoteQuery(proposal.NokiaCPQ_Portfolio__c, proposal.NokiaCPQ_Maintenance_Type__c);
                    var shippingLocations = await dBHelper.FindAsync<ShippingLocationQueryModel>(shippingLocationQuery);

                    if (shippingLocations != null && shippingLocations.Count != 0)
                    {
                        if (proposal.CurrencyIsoCode == Constants.USDCURRENCY)
                        {
                            minMaintPrice_EP = shippingLocations[0].Min_Maint_USD__c;
                        }
                        else if (proposal.CurrencyIsoCode == Constants.EUR_CURR)
                        {
                            minMaintPrice_EP = shippingLocations[0].Min_Maint_EUR__c;
                        }
                    }

                    if (proposal.NokiaCPQ_Existing_IONMaint_Contract__c != null)
                    {
                        isIONExistingContract_EP = proposal.NokiaCPQ_Existing_IONMaint_Contract__c;
                    }
                }
            }

            if (Constants.QUOTE_TYPE_INDIRECTCPQ.equalsIgnoreCase(proposal.Quote_Type__c))
            {
                var shippingLocationQuery = QueryHelper.GetShippingLocationForIndirectQuoteQuery(proposal.NokiaCPQ_Maintenance_Accreditation__r_Portfolio__c,
                    proposal.NokiaCPQ_Maintenance_Accreditation__r_Pricing_Cluster__c);

                var shippingLocations = await dBHelper.FindAsync<ShippingLocationQueryModel>(shippingLocationQuery);

                if (shippingLocations != null && shippingLocations.Count != 0)
                {
                    if (proposal.CurrencyIsoCode == Constants.USDCURRENCY)
                    {
                        if (proposal.NokiaCPQ_LEO_Discount__c == true)
                        {
                            minMaintPrice = shippingLocations[0].LEO_Mini_Maint_USD__c;
                        }
                        else
                        {
                            minMaintPrice = shippingLocations[0].Min_Maint_USD__c;
                        }
                    }
                    else
                    {
                        if (proposal.NokiaCPQ_LEO_Discount__c == true)
                        {
                            minMaintPrice = shippingLocations[0].LEO_Mini_Maint_EUR__c;
                        }
                        else
                        {
                            minMaintPrice = shippingLocations[0].Min_Maint_EUR__c;
                        }
                    }
                }
            }

            Dictionary<int, LineItem> lineItemObjectMapDirect = new Dictionary<int, LineItem>();

            if (Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Quote_Type__c))
            {
                foreach (var cartLineItem in cartLineItems)
                {
                    string configType = GetConfigType(cartLineItem);
                    string irpMapKey = cartLineItem.GetLineNumber() + Constants.NOKIA_UNDERSCORE + cartLineItem.ChargeType;

                    if (lineItemIRPMapDirect.ContainsKey(irpMapKey))
                    {
                        lineItemIRPMapDirect[irpMapKey].Add(cartLineItem);
                    }
                    else
                    {
                        lineItemIRPMapDirect.Add(irpMapKey, new List<LineItem> { cartLineItem });
                    }

                    if (cartLineItem.GetLineType() != LineType.ProductService)
                    {
                        string linePrimaryNoChargeTypeKey = cartLineItem.ParentBundleNumber + Constants.NOKIA_UNDERSCORE + cartLineItem.ChargeType;
                        if (primaryNoLineItemList.ContainsKey(linePrimaryNoChargeTypeKey))
                        {
                            primaryNoLineItemList[linePrimaryNoChargeTypeKey].Add(cartLineItem);
                        }
                        else
                        {
                            primaryNoLineItemList.Add(linePrimaryNoChargeTypeKey, new List<LineItem> { cartLineItem });
                        }
                    }

                    if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING) && configType.equals(Constants.BUNDLE))
                    {
                        lineItemObjectMapDirect.Add(cartLineItem.PrimaryLineNumber, cartLineItem);
                    }
                }
            }

            //GP: BeforePricing Method
            string isIONExistingContract = string.Empty;

            if (proposal.NokiaCPQ_Existing_IONMaint_Contract__c != null)
            {
                isIONExistingContract = proposal.NokiaCPQ_Existing_IONMaint_Contract__c;
            }

            List<string> pdcList = new List<string>(Labels.SRSPDC);

            if (Constants.QUOTE_TYPE_INDIRECTCPQ.equalsIgnoreCase(proposal.Quote_Type__c))
            {
                decimal? totalExtendedMaintY1Price = 0;
                decimal? totalExtendedMaintY2Price = 0;
                decimal? totalExtendedSSPPrice = 0;
                decimal? totalExtendedSRSPrice = 0;

                decimal? totalOntQuantity = 0;
                decimal? totalFBAONTQty = 0;
                decimal? totalFBAP2PQty = 0;

                foreach (var cartLineItem in cartLineItems)
                {
                    if (cartLineItem.BasePrice.HasValue && cartLineItem.PriceIncludedInBundle == false &&
                        (!cartLineItem.BasePriceOverride.HasValue ||
                        (cartLineItem.BasePriceOverride.HasValue && cartLineItem.BasePriceOverride.Value != pricingHelper.ApplyRounding(cartLineItem.BasePrice.Value, 2, RoundingMode.HALF_UP))
                        ))
                    {
                        //Second time
                        cartLineItem.BasePriceOverride = pricingHelper.ApplyRounding(cartLineItem.BasePrice.Value, 2, RoundingMode.HALF_UP);
                        cartLineItem.PricingStatus = "Pending";
                    }

                    //GP: Commenting out below logic as this is already performed base price mode in beforepricing method 

                    //string partNumber = getPartNumber(item);
                    //if (partNumber != null && partNumber.equalsIgnoreCase(Constants.MAINTY2CODE))
                    //{
                    //    item.Apttus_Config2__LineSequence__c = 997;
                    //}
                    //else if (partNumber != null && partNumber.equalsIgnoreCase(Constants.MAINTY1CODE))
                    //{
                    //    item.Apttus_Config2__LineSequence__c = 996;

                    //}
                    //else if (partNumber != null && partNumber.equalsIgnoreCase(Constants.SSPCODE))
                    //{
                    //    item.Apttus_Config2__LineSequence__c = 998;
                    //}
                    //else if (partNumber != null && partNumber.equalsIgnoreCase(Constants.SRS))
                    //{
                    //    item.Apttus_Config2__LineSequence__c = 999;
                    //}

                    string productDiscountCat = getProductDiscountCategory(cartLineItem);
                    if ((cartLineItem.BasePrice.HasValue &&
                        cartLineItem.BasePrice.Value > 0 &&
                        cartLineItem.NokiaCPQ_Spare__c == false) ||
                        (cartLineItem.Apttus_Config2__IsHidden__c == true && cartLineItem.BasePrice.HasValue && cartLineItem.BasePrice.Value == 0))
                    {
                        if (cartLineItem.NokiaCPQ_Maint_Yr1_Extended_Price__c != null)
                        {
                            totalExtendedMaintY1Price = totalExtendedMaintY1Price + cartLineItem.NokiaCPQ_Maint_Yr1_Extended_Price__c.Value;
                        }
                        if (cartLineItem.NokiaCPQ_Maint_Yr2_Extended_Price__c != null)
                        {
                            totalExtendedMaintY2Price = totalExtendedMaintY2Price + cartLineItem.NokiaCPQ_Maint_Yr2_Extended_Price__c;
                        }

                        //Replace item.Portfolio_from_Quote_Line_Item__c formula field with 'this.proposalSO.NokiaCPQ_Portfolio__c', NokiaCPQ_Product_Discount_Category__c with value fetched from method
                        if (((productDiscountCat != null && !pdcList.isEmpty() && pdcList.Contains(productDiscountCat)) ||
                        (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_NUAGE) &&
                        Constants.PRODUCTITEMTYPESOFTWARE.equalsIgnoreCase(cartLineItem.Apttus_Config2__ProductId__r_NokiaCPQ_Item_Type__c))) || cartLineItem.Is_Custom_Product__c == true)
                        {
                            if (cartLineItem.Nokia_SRS_Base_Extended_Price__c != null)
                            {
                                totalExtendedSRSPrice = totalExtendedSRSPrice + cartLineItem.Nokia_SRS_Base_Extended_Price__c.Value;
                            }
                        }
                    }

                    if ((productDiscountCat != null && !productDiscountCat.Contains(Constants.NOKIA_NFM_P) && cartLineItem.NokiaCPQ_Spare__c == false) ||
                        cartLineItem.Is_Custom_Product__c == true)
                    {
                        if (cartLineItem.Nokia_SSP_Base_Extended_Price__c != null)
                        {
                            totalExtendedSSPPrice = totalExtendedSSPPrice + cartLineItem.Nokia_SSP_Base_Extended_Price__c.Value;
                        }
                    }

                    if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase("Fixed Access - POL") && cartLineItem.Total_ONT_Quantity__c != null)
                    {
                        totalOntQuantity = totalOntQuantity + cartLineItem.Total_ONT_Quantity__c.Value;
                    }
                    if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase("Fixed Access - FBA"))
                    {
                        if (cartLineItem.Total_ONT_Quantity_FBA__c != null)
                        {
                            totalFBAONTQty = totalFBAONTQty + cartLineItem.Total_ONT_Quantity_FBA__c.Value;
                        }
                        else if (cartLineItem.Total_ONT_Quantity_P2P__c != null)
                        {
                            totalFBAP2PQty = totalFBAP2PQty + cartLineItem.Total_ONT_Quantity_P2P__c.Value;
                        }
                    }
                }

                foreach (var cartLineItem in cartLineItems)
                {
                    string partNumber = getPartNumber(cartLineItem);
                    bool isUpdate = false;

                    //Execute third time only for Product/Service lines to complete price calculations after quantity update
                    if (cartLineItem.BasePrice.HasValue && cartLineItem.PriceIncludedInBundle == false &&
                        cartLineItem.GetLineType() == LineType.ProductService)
                    {
                        isUpdate = true;
                    }

                    if (cartLineItem.Is_Custom_Product__c == false && cartLineItem.Is_Contract_Pricing_2__c == true &&
                        cartLineItem.PriceListItemId != null)
                    {
                        cartLineItem.BasePriceOverride = cartLineItem.Apttus_Config2__PriceListItemId__r_Partner_Price__c;
                        cartLineItem.BasePrice = cartLineItem.BasePriceOverride;
                        isUpdate = true;
                    }

                    //Varsha: End: Changes in Sprint 7 for Req 3354
                    if (partNumber != null && partNumber.Contains(Constants.MAINTY1CODE))
                    {
                        if (totalExtendedMaintY1Price > 0)
                        {
                            if (Constants.NOKIA_NO.equalsIgnoreCase(isIONExistingContract) && totalExtendedMaintY1Price < minMaintPrice)
                            {
                                totalExtendedMaintY1Price = minMaintPrice;
                            }
                        }

                        //GP: Instead of looking up on the cart lineitem, we are using proposal for same
                        if (proposal.Maintenance_Y1__c != null)
                        {
                            cartLineItem.BasePriceOverride = proposal.Maintenance_Y1__c;
                            cartLineItem.BasePrice = proposal.Maintenance_Y1__c;
                        }
                        else
                        {
                            cartLineItem.BasePriceOverride = totalExtendedMaintY1Price;
                            cartLineItem.BasePrice = totalExtendedMaintY1Price;
                        }

                        isUpdate = true;
                    }
                    else if (partNumber != null && partNumber.Contains(Constants.MAINTY2CODE))
                    {
                        if (proposal.Maintenance_Y2__c != null)
                        {
                            cartLineItem.BasePriceOverride = proposal.Maintenance_Y2__c;
                            cartLineItem.BasePrice = proposal.Maintenance_Y2__c;
                        }
                        else
                        {
                            cartLineItem.BasePriceOverride = totalExtendedMaintY2Price;
                            cartLineItem.BasePrice = totalExtendedMaintY2Price;
                        }

                        isUpdate = true;
                    }
                    else if (cartLineItem.ChargeType.Contains(Constants.NOKIA_PRODUCT_NAME_SSP))
                    {
                        if (proposal.SSP__c != null)
                        {
                            cartLineItem.BasePriceOverride = proposal.SSP__c;
                            cartLineItem.BasePrice = proposal.SSP__c;
                        }
                        else if (IsLeo())
                        {
                            cartLineItem.BasePriceOverride = 0;
                            cartLineItem.BasePrice = 0; ;

                        }
                        else
                        {
                            cartLineItem.BasePriceOverride = totalExtendedSSPPrice;
                            cartLineItem.BasePrice = totalExtendedSSPPrice;
                        }

                        isUpdate = true;
                    }
                    else if (cartLineItem.ChargeType.Contains(Constants.NOKIA_PRODUCT_NAME_SRS))
                    {
                        if (proposal.SRS__c != null)
                        {
                            cartLineItem.BasePriceOverride = proposal.SRS__c;
                            cartLineItem.BasePrice = proposal.SRS__c;
                        }
                        else if (IsLeo())
                        {
                            cartLineItem.BasePriceOverride = 0;
                            cartLineItem.BasePrice = 0;

                        }
                        else
                        {
                            cartLineItem.BasePriceOverride = totalExtendedSRSPrice;
                            cartLineItem.BasePrice = totalExtendedSRSPrice;
                        }
                        isUpdate = true;
                    }

                    if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase("Fixed Access - POL") && sspFNSet.Contains(partNumber))
                    {
                        cartLineItem.Quantity = Convert.ToInt32(proposal.NokiaCPQ_No_Of_Years__c) * totalOntQuantity;
                        isUpdate = true;
                    }

                    if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase("Fixed Access - FBA") && sspFNSet.Contains(partNumber))
                    {
                        cartLineItem.Quantity = Convert.ToInt32(proposal.NokiaCPQ_No_Of_Years__c) * totalFBAONTQty +
                            Convert.ToInt32(proposal.NokiaCPQ_No_Of_Years__c) * totalFBAP2PQty;

                        isUpdate = true;
                    }

                    if (isUpdate)
                    {
                        cartLineItem.UpdatePrice(pricingHelper);
                    }
                }
            }

            if (Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Quote_Type__c))
            {
                //initialize the maintenance price
                decimal? totalExtendedMaintY1Price = 0;
                decimal? totalExtendedMaintY2Price = 0;

                Dictionary<string, LineItem> maintenanceLinesMap = new Dictionary<string, LineItem>();
                Dictionary<string, LineItem> maintenanceLinesMap_EP = new Dictionary<string, LineItem>();
                Dictionary<string, LineItem> productServiceMap = new Dictionary<string, LineItem>();
                Dictionary<string, LineItem> careSRSOptionMap = new Dictionary<string, LineItem>();

                //Dictionary<decimal?, decimal?> linenumberQuantity = new Dictionary<decimal?, decimal?>();

                foreach (var cartLineItem in cartLineItems)
                {
                    string partNumber = getPartNumber(cartLineItem);

                    //Logic from Workflow: Enable Manual Adjustment For Options
                    if ((proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_SOFTWARE) ||
                        (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_IP_ROUTING) &&
                        proposal.Is_List_Price_Only__c == false)) && !cartLineItem.ChargeType.equalsIgnoreCase(Constants.STANDARD))
                    {
                        cartLineItem.AllowManualAdjustment = false;
                    }
                    if (cartLineItem.BasePrice != null && cartLineItem.BasePrice > 0 && cartLineItem.ChargeType.equalsIgnoreCase("Standard Price") &&
                        !(cartLineItem.Source__c == "BOMXAE" && proposal.NokiaCPQ_Portfolio__c == "QTC"))
                    {

                        decimal? convertedBasePriceTwoDecimal = pricingHelper.ApplyRounding((cartLineItem.BasePrice / conversionRate) * (proposal.Exchange_Rate__c), 2, RoundingMode.HALF_UP);
                        decimal? convertedBasePriceFiveDecimal = pricingHelper.ApplyRounding((cartLineItem.BasePrice / conversionRate) * (proposal.Exchange_Rate__c), 5, RoundingMode.HALF_UP);

                        if (cartLineItem.PriceListId == cartLineItem.Apttus_Config2__PriceListItemId__r_Apttus_Config2__PriceListId__c &&
                            cartLineItem.BasePriceOverride != convertedBasePriceTwoDecimal)
                        {
                            cartLineItem.BasePriceOverride = convertedBasePriceFiveDecimal;
                            cartLineItem.BasePriceOverride = convertedBasePriceTwoDecimal;
                            cartLineItem.PricingStatus = "Pending";
                        }
                        else if (cartLineItem.PriceListId != cartLineItem.Apttus_Config2__PriceListItemId__r_Apttus_Config2__PriceListId__c &&
                            cartLineItem.BasePriceOverride != pricingHelper.ApplyRounding(cartLineItem.BasePrice, 2, RoundingMode.HALF_UP))
                        {
                            cartLineItem.BasePriceOverride = pricingHelper.ApplyRounding(cartLineItem.BasePrice, 5, RoundingMode.HALF_UP);
                            cartLineItem.BasePriceOverride = pricingHelper.ApplyRounding(cartLineItem.BasePrice, 2, RoundingMode.HALF_UP);
                            cartLineItem.PricingStatus = "Pending";
                        }
                    }
                    //To set quantities for other charge types on main bundle
                    //if (cartLineItem.ChargeType != null && cartLineItem.ChargeType.equalsIgnoreCase(Constants.STANDARD) && cartLineItem.GetLineType() == LineType.ProductService)
                    //{
                    //    linenumberQuantity.Add(cartLineItem.GetLineNumber(), cartLineItem.GetQuantity());
                    //}
                    //Map of Airscale wifi Maintenance lines
                    if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING))
                    {
                        if (partNumber != null && partNumber.Contains(Constants.MAINTY1CODE))
                        {
                            maintenanceLinesMap.Add("Year1", cartLineItem);
                        }
                        else if (partNumber != null && partNumber.Contains(Constants.MAINTY2CODE))
                        {
                            maintenanceLinesMap.Add("Year2", cartLineItem);
                        }
                    }
                    //Start: creating maps to limit for loop iterations
                    if (cartLineItem.GetLineType() == LineType.ProductService)
                    {
                        productServiceMap.Add(cartLineItem.Id, cartLineItem);
                    }
                    if ((cartLineItem.ChargeType.equalsIgnoreCase(Constants.NOKIA_YEAR1_MAINTENANCE) || cartLineItem.ChargeType.equalsIgnoreCase(Constants.NOKIA_SRS)) &&
                        cartLineItem.GetLineType() != LineType.ProductService)
                    {
                        careSRSOptionMap.Add(cartLineItem.Id, cartLineItem);
                    }
                    //End
                }

                //R-6508
                if (Constants.NOKIA_IP_ROUTING.equalsIgnoreCase(proposal.NokiaCPQ_Portfolio__c) && proposal.Is_List_Price_Only__c == false)
                {
                    foreach (var cartLineItem in cartLineItems)
                    {
                        string partNumber = getPartNumber(cartLineItem);

                        //Piece of code written below calculates Maintenance for each line item and saves it on line item for Direct Enterprise
                        if (partNumber != null &&
                              !partNumber.Contains(Constants.MAINTY1CODE) &&
                              !partNumber.Contains(Constants.MAINTY2CODE) &&
                              !partNumber.Contains(Constants.SSPCODE) &&
                              !partNumber.Contains(Constants.SRS) && cartLineItem.Is_List_Price_Only__c == false)
                        {
                            Dictionary<string, decimal?> maintenanceValueMap = calculateMaintenance_EP_Direct(cartLineItem, totalExtendedMaintY1Price, totalExtendedMaintY2Price, partNumber);
                            totalExtendedMaintY1Price = maintenanceValueMap["ExtendedMaintY1Price"];
                            totalExtendedMaintY2Price = maintenanceValueMap["ExtendedMaintY2Price"];
                        }

                        //Map Of Miantenance Line items for IP routing- Enterprise
                        if (partNumber != null && partNumber.Contains(Constants.MAINTY1CODE))
                        {
                            maintenanceLinesMap_EP.Add("Year1", cartLineItem);
                        }
                        else if (partNumber != null && partNumber.Contains(Constants.MAINTY2CODE))
                        {
                            maintenanceLinesMap_EP.Add("Year2", cartLineItem);
                        }
                    }

                    LineItem lineItemVarSO;
                    bool isUpdate = false;
                    if (maintenanceLinesMap_EP.Count > 0)
                    {
                        if (maintenanceLinesMap_EP.ContainsKey("Year1"))
                        {
                            lineItemVarSO = maintenanceLinesMap_EP["Year1"];

                            if (Constants.NOKIA_NO.equalsIgnoreCase(isIONExistingContract_EP) && minMaintPrice_EP != null && minMaintPrice_EP > totalExtendedMaintY1Price)
                            {
                                if (lineItemVarSO.BasePriceOverride != minMaintPrice_EP)
                                {
                                    lineItemVarSO.BasePriceOverride = minMaintPrice_EP;
                                    lineItemVarSO.NokiaCPQ_Unitary_IRP__c = minMaintPrice_EP;
                                    lineItemVarSO.LineSequence = 996;
                                    isUpdate = true;
                                }
                            }
                            else
                            {
                                if (lineItemVarSO.BasePriceOverride != totalExtendedMaintY1Price)
                                {
                                    lineItemVarSO.BasePriceOverride = totalExtendedMaintY1Price;
                                    lineItemVarSO.NokiaCPQ_Unitary_IRP__c = totalExtendedMaintY1Price;
                                    lineItemVarSO.LineSequence = 996;
                                    isUpdate = true;
                                }
                            }

                            if (isUpdate)
                            {
                                lineItemVarSO.UpdatePrice(pricingHelper);
                                isUpdate = true;
                            }
                        }
                        if (maintenanceLinesMap_EP.ContainsKey("Year2"))
                        {
                            lineItemVarSO = maintenanceLinesMap_EP["Year2"];

                            if (lineItemVarSO.BasePriceOverride != totalExtendedMaintY2Price)
                            {
                                lineItemVarSO.BasePriceOverride = totalExtendedMaintY2Price;
                                lineItemVarSO.NokiaCPQ_Unitary_IRP__c = totalExtendedMaintY2Price;
                                lineItemVarSO.LineSequence = 997;
                                isUpdate = true;
                            }

                            if (isUpdate)
                            {
                                lineItemVarSO.UpdatePrice(pricingHelper);
                                isUpdate = false;
                            }
                        }
                    }
                }
                //R-6508 End

                //Piyush Tawari Req 6229 Airscale Wifi Direct
                //Copy Discounts from Groups to SIs
                if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING))
                {
                    foreach (var cartLineItem in cartLineItems)
                    {
                        string partNumber = getPartNumber(cartLineItem);
                        string configType = getConfigType(cartLineItem);

                        if (!configType.equalsIgnoreCase("Bundle") && cartLineItem.GetLineType() == LineType.Option
                                && lineItemObjectMapDirect != null && cartLineItem.Advanced_Pricing_Done__c == false)
                        {
                            var parentBundleNumber = cartLineItem.ParentBundleNumber.Value;

                            if (lineItemObjectMapDirect.ContainsKey(parentBundleNumber) &&
                                cartLineItem.ParentBundleNumber.Value != cartLineItem.GetLineNumber())
                            {
                                if (lineItemObjectMapDirect[parentBundleNumber].AdjustmentType == Constants.DISCOUNT_PERCENT)
                                {
                                    cartLineItem.AdjustmentType = lineItemObjectMapDirect[parentBundleNumber].AdjustmentType;
                                    cartLineItem.AdjustmentAmount = lineItemObjectMapDirect[parentBundleNumber].AdjustmentAmount;
                                }
                                else if (lineItemObjectMapDirect[parentBundleNumber].AdjustmentType == Constants.DISCOUNT_AMOUNT)
                                {
                                    if ((lineItemObjectMapDirect[parentBundleNumber].NokiaCPQ_Extended_CLP__c != 0 ||
                                        lineItemObjectMapDirect[parentBundleNumber].NokiaCPQ_Extended_CLP__c != null) &&
                                        lineItemObjectMapDirect[parentBundleNumber].AdjustmentAmount != null)
                                    {
                                        decimal? disPer = 0;
                                        disPer = lineItemObjectMapDirect[parentBundleNumber].AdjustmentAmount / lineItemObjectMapDirect[parentBundleNumber].NokiaCPQ_Extended_CLP__c * 100;

                                        cartLineItem.AdjustmentType = Constants.DISCOUNT_PERCENT;
                                        cartLineItem.AdjustmentAmount = disPer;
                                    }
                                }
                                else if (lineItemObjectMapDirect[parentBundleNumber].AdjustmentType == null)
                                {
                                    cartLineItem.AdjustmentType = null;
                                    cartLineItem.AdjustmentAmount = null;
                                }
                            }
                        }
                        //Piece of cde written below calculates Maintenance for each line item and saves it on line item for Airscale Wifi
                        if (partNumber != null && !partNumber.Contains(Constants.MAINTY1CODE) && !partNumber.Contains(Constants.MAINTY2CODE))
                        {
                            Dictionary<string, decimal?> maintenanceValueMap = calculateMaintenance_MN_Direct(cartLineItem, totalExtendedMaintY1Price, totalExtendedMaintY2Price, partNumber, configType);

                            totalExtendedMaintY1Price = maintenanceValueMap["ExtendedMaintY1Price"];
                            totalExtendedMaintY2Price = maintenanceValueMap["ExtendedMaintY2Price"];
                        }
                        //if (cartLineItem.ChargeType != null && !cartLineItem.ChargeType.equalsIgnoreCase(Constants.STANDARD) && cartLineItem.GetLineType() == LineType.ProductService)
                        //{
                        //    if (linenumberQuantity.Count > 0 && linenumberQuantity.ContainsKey(cartLineItem.GetLineNumber()))
                        //    {
                        //        cartLineItem.Quantity = linenumberQuantity[cartLineItem.GetLineNumber()];

                        //    }
                        //}
                    }

                    foreach (var cartLineItem in cartLineItems)
                    {
                        string configType = getConfigType(cartLineItem);

                        /**Piyush Tawari Start**/
                        //Req-6228 MN Airscale wifi - Price for the groups to be aggregated from SI
                        //For Direct MN Airscale Wifi
                        //checking if its Group
                        if (cartLineItem.GetLineType() == LineType.Option && Constants.BUNDLE.equalsIgnoreCase(configType))
                        {
                            cartLineItem.NokiaCPQ_Unitary_Cost__c = 0;
                            cartLineItem.NCPQ_Unitary_CLP__c = 0;
                            cartLineItem.NokiaCPQ_Unitary_IRP__c = 0;
                            cartLineItem.NokiaCPQ_Extended_CUP__c = 0;
                            cartLineItem.NokiaCPQ_Extended_CNP__c = 0;

                            var primaryLineNumberChargeTypeKey = cartLineItem.PrimaryLineNumber + Constants.NOKIA_UNDERSCORE + cartLineItem.ChargeType;

                            if (primaryNoLineItemList.ContainsKey(primaryLineNumberChargeTypeKey))
                            {
                                foreach (var optionItem in primaryNoLineItemList[primaryLineNumberChargeTypeKey])
                                {
                                    //Stamping IRP at Group Level
                                    if (optionItem.NokiaCPQ_Unitary_IRP__c != null && optionItem.Quantity != null)
                                    {
                                        var unitaryIRP = pricingHelper.ApplyRounding(cartLineItem.NokiaCPQ_Unitary_IRP__c + optionItem.NokiaCPQ_Unitary_IRP__c * optionItem.GetQuantity(), 2, RoundingMode.HALF_UP);
                                        cartLineItem.NokiaCPQ_Unitary_IRP__c = unitaryIRP;
                                    }

                                    //stamping CLP at Group level
                                    if (optionItem.BasePriceOverride != null)
                                    {
                                        var roundedBasePriceOverride = pricingHelper.ApplyRounding(optionItem.BasePriceOverride, 2, RoundingMode.HALF_UP);
                                        var unitaryCLP = pricingHelper.ApplyRounding(cartLineItem.NCPQ_Unitary_CLP__c + roundedBasePriceOverride * optionItem.GetQuantity(), 2, RoundingMode.HALF_UP);

                                        cartLineItem.NCPQ_Unitary_CLP__c = unitaryCLP;
                                    }
                                    else if (optionItem.BasePrice != null)
                                    {
                                        var roundedBasePrice = pricingHelper.ApplyRounding(optionItem.BasePrice, 2, RoundingMode.HALF_UP);
                                        var unitaryCLP = pricingHelper.ApplyRounding(cartLineItem.NCPQ_Unitary_CLP__c + roundedBasePrice * optionItem.GetQuantity(), 2, RoundingMode.HALF_UP);
                                        cartLineItem.NCPQ_Unitary_CLP__c = unitaryCLP;
                                    }
                                    //Stamping CUP at Group level
                                    cartLineItem.NokiaCPQ_Extended_CUP__c = cartLineItem.NokiaCPQ_Extended_CUP__c + optionItem.AdjustedPrice;
                                    //stamping CNP at Group Level
                                    cartLineItem.NokiaCPQ_Extended_CNP__c = cartLineItem.NokiaCPQ_Extended_CNP__c + optionItem.NetPrice;

                                    if (optionItem.NokiaCPQ_Unitary_Cost__c != null && optionItem.Quantity != null)
                                    {
                                        var roundedUnitaryCost = pricingHelper.ApplyRounding(optionItem.NokiaCPQ_Unitary_Cost__c, 2, RoundingMode.HALF_UP);
                                        var unitaryCost = pricingHelper.ApplyRounding(cartLineItem.NokiaCPQ_Unitary_Cost__c + roundedUnitaryCost * optionItem.GetQuantity(), 2, RoundingMode.HALF_UP);
                                        cartLineItem.NokiaCPQ_Unitary_Cost__c = unitaryCost;
                                    }
                                }
                            }
                            if (cartLineItem.AdjustmentType == Constants.DISCOUNT_AMOUNT)
                                cartLineItem.BasePriceOverride = cartLineItem.AdjustmentAmount;
                            else
                                cartLineItem.BasePriceOverride = 0;
                        }/**Piyush Tawari End**/
                    }

                    //Calculate MaintY1 & MaintY2 for MN Direct(Airscale wifi)
                    LineItem lineItemVarSO;
                    if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING) && maintenanceLinesMap.Count > 0)
                    {
                        if (maintenanceLinesMap.ContainsKey("Year1"))
                        {
                            lineItemVarSO = maintenanceLinesMap["Year1"];

                            if (lineItemVarSO.PriceListId == lineItemVarSO.Apttus_Config2__PriceListItemId__r_Apttus_Config2__PriceListId__c)
                            {
                                lineItemVarSO.BasePriceOverride = totalExtendedMaintY1Price;
                                lineItemVarSO.NokiaCPQ_Unitary_IRP__c = totalExtendedMaintY1Price;
                            }
                            else
                            {
                                lineItemVarSO.NokiaCPQ_Unitary_IRP__c = totalExtendedMaintY1Price;
                            }

                            lineItemVarSO.LineSequence = 997;
                            lineItemVarSO.UpdatePrice(pricingHelper);
                        }
                        if (maintenanceLinesMap.ContainsKey("Year2"))
                        {
                            lineItemVarSO = maintenanceLinesMap["Year2"];

                            if (!string.IsNullOrWhiteSpace(proposal.NokiaCPQ_No_Of_Years__c) && Convert.ToDecimal(proposal.NokiaCPQ_No_Of_Years__c) > 1)
                            {
                                if (lineItemVarSO.PriceListId == lineItemVarSO.Apttus_Config2__PriceListItemId__r_Apttus_Config2__PriceListId__c)
                                {
                                    lineItemVarSO.BasePriceOverride = totalExtendedMaintY2Price;
                                    lineItemVarSO.NokiaCPQ_Unitary_IRP__c = totalExtendedMaintY2Price;
                                }
                                else
                                {
                                    lineItemVarSO.NokiaCPQ_Unitary_IRP__c = totalExtendedMaintY2Price;
                                }
                            }
                            else
                            {
                                lineItemVarSO.BasePriceOverride = 0;
                                lineItemVarSO.NokiaCPQ_Unitary_IRP__c = 0;
                            }
                            lineItemVarSO.LineSequence = 998;
                            lineItemVarSO.UpdatePrice(pricingHelper);
                        }
                    }
                }

                Dictionary<decimal?, List<decimal?>> BundleCareSRSPriceMap = new Dictionary<decimal?, List<decimal?>>();
                List<decimal?> careSRSList = new List<decimal?>();

                if (!proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING))
                {
                    foreach (var item in productServiceMap.Values)
                    {
                        //if (!item.ChargeType.equalsIgnoreCase(Constants.STANDARD))
                        //{
                        //    if (linenumberQuantity.Count > 0 && linenumberQuantity.ContainsKey(item.GetLineNumber()))
                        //    {
                        //        item.Quantity = linenumberQuantity[item.GetLineNumber()];
                        //    }
                        //}
                        //Care & SRS calculation for NSW
                        if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase("Nokia Software"))
                        {
                            if (item.ChargeType.equalsIgnoreCase(Constants.STANDARD) && lineItemIRPMapDirect.ContainsKey(item.GetLineNumber() + "_" + item.ChargeType))
                            {
                                careSRSList = careSRSCalculationForNSW(item);
                                if (!careSRSList.isEmpty())
                                {
                                    BundleCareSRSPriceMap.Add(item.GetLineNumber(), careSRSList);
                                }
                            }
                        }
                    }
                }

                //Stamp prices to Care & SRS
                List<string> careProActiveList = Labels.NokiaCPQ_Care_Proactive;
                List<string> careAdvanceList = Labels.NokiaCPQ_Care_Advance;
                HashSet<string> careProactiveSet = new HashSet<string>(careProActiveList);
                HashSet<string> careAdvanceSet = new HashSet<string>(careAdvanceList);

                if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase("Nokia Software"))
                {
                    foreach (var item in careSRSOptionMap.Values)
                    {
                        if (item.ChargeType.equalsIgnoreCase(Constants.NOKIA_YEAR1_MAINTENANCE))
                        {
                            var productCode = item.Apttus_Config2__OptionId__r_ProductCode;

                            if (item.OptionId != null && careAdvanceSet.Contains(productCode) && BundleCareSRSPriceMap.ContainsKey(item.GetLineNumber()) &&
                                BundleCareSRSPriceMap[item.GetLineNumber()].Count > 0 &&
                                BundleCareSRSPriceMap.ContainsKey(item.GetLineNumber()) && BundleCareSRSPriceMap[item.GetLineNumber()][0] != null &&
                                item.NokiaCPQ_CareSRSBasePrice__c != pricingHelper.ApplyRounding((BundleCareSRSPriceMap[item.GetLineNumber()][0] * 0.10m), 2, RoundingMode.HALF_UP))
                            {
                                var careSRSBasePrice = pricingHelper.ApplyRounding((BundleCareSRSPriceMap[item.GetLineNumber()][0] * 0.10m), 2, RoundingMode.HALF_UP);

                                item.BasePriceOverride = careSRSBasePrice;
                                item.NokiaCPQ_CareSRSBasePrice__c = careSRSBasePrice;
                                item.PricingStatus = "Pending";
                            }
                            else if (item.OptionId != null && careProactiveSet.Contains(productCode) &&
                                BundleCareSRSPriceMap.ContainsKey(item.GetLineNumber()) && BundleCareSRSPriceMap[item.GetLineNumber()].Count > 0 &&
                                BundleCareSRSPriceMap.ContainsKey(item.GetLineNumber()) && BundleCareSRSPriceMap[item.GetLineNumber()][0] != null &&
                                item.NokiaCPQ_CareSRSBasePrice__c != pricingHelper.ApplyRounding((BundleCareSRSPriceMap[item.GetLineNumber()][0] * 0.15m), 2, RoundingMode.HALF_UP))
                            {
                                var careSRSBasePrice = pricingHelper.ApplyRounding((BundleCareSRSPriceMap[item.GetLineNumber()][0] * 0.15m), 2, RoundingMode.HALF_UP);

                                item.BasePriceOverride = careSRSBasePrice;
                                item.NokiaCPQ_CareSRSBasePrice__c = careSRSBasePrice;
                                item.PricingStatus = "Pending";
                            }
                        }
                        else if (item.ChargeType.equalsIgnoreCase(Constants.NOKIA_SRS))
                        {
                            if (BundleCareSRSPriceMap.ContainsKey(item.GetLineNumber()) && BundleCareSRSPriceMap[item.GetLineNumber()].Count > 1 &&
                                BundleCareSRSPriceMap.ContainsKey(item.GetLineNumber()) && BundleCareSRSPriceMap[item.GetLineNumber()][1] != null &&
                                item.NokiaCPQ_SRSBasePrice__c != pricingHelper.ApplyRounding((BundleCareSRSPriceMap[item.GetLineNumber()][1] * 0.15m), 2, RoundingMode.HALF_UP))
                            {
                                var srsBasePrice = pricingHelper.ApplyRounding((BundleCareSRSPriceMap[item.GetLineNumber()][1] * 0.15m), 2, RoundingMode.HALF_UP);

                                item.BasePriceOverride = srsBasePrice;
                                item.NokiaCPQ_SRSBasePrice__c = srsBasePrice;
                                item.PricingStatus = "Pending";
                            }
                        }
                    }
                }
            }

            await Task.CompletedTask;
        }

        public async Task AfterPricingCartAdjustmentAsync(AggregateCartRequest aggregateCartRequest)
        {
            if (proposal.Quote_Type__c.equalsIgnoreCase("Direct DS") || proposal.Quote_Type__c.equalsIgnoreCase("Indirect DS"))
            {
                await calculateEquivalentPrice();
            }

            if (Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Quote_Type__c))
            {
                Dictionary<string, List<LineItemModel>> primaryNoLineItemList1 = new Dictionary<string, List<LineItemModel>>();
                foreach (var item in cartLineItems)
                {
                    int quantityBundle = 1;
                    if (item.GetLineType() == LineType.Option)
                    {
                        var key = Constants.NOKIA_PRODUCT_SERVICES + Constants.NOKIA_UNDERSCORE + item.GetLineNumber();
                        if (lineItemObjectMap.ContainsKey(key) && lineItemObjectMap[key] != null)
                        {
                            if (this.lineItemObjectMap[key].Quantity != null)
                            {
                                quantityBundle = Convert.ToInt32(Math.Ceiling(lineItemObjectMap[key].GetQuantity()));

                                if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING))
                                {
                                    item.Total_Option_Quantity__c = quantityBundle * item.GetExtendedQuantity();
                                }
                                else
                                {
                                    item.Total_Option_Quantity__c = quantityBundle * item.GetQuantity();
                                }
                            }
                        }
                    }
                }
            }
        }

        public async Task OnCartPricingCompleteAsync(AggregateCartRequest aggregateCartRequest)
        {
            //defaultExchangeRate

            decimal? pricingGuidanceSettingThresold = null;
            List<DirectPortfolioGeneralSettingQueryModel> portfolioSettingList = new List<DirectPortfolioGeneralSettingQueryModel>();
            List<DirectCareCostPercentageQueryModel> careCostPercentList = new List<DirectCareCostPercentageQueryModel>();
            Dictionary<string, decimal?> unitaryCostMap = new Dictionary<string, decimal?>();

            if (UsePricingGuidanceSettingThresold())
            {
                var pricingGuidanceSettingQuery = QueryHelper.GetPricingGuidanceSettingQuery(proposal.NokiaCPQ_Portfolio__c);
                pricingGuidanceSettingThresold = (await dBHelper.FindAsync<PricingGuidanceSettingQueryModel>(pricingGuidanceSettingQuery)).FirstOrDefault()?.Threshold__c;
            }

            if (Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Quote_Type__c))
            {
                var directPortfolioGeneralSettingQuery = QueryHelper.GetDirectPortfolioGeneralSettingQuery(proposal.NokiaCPQ_Portfolio__c);
                portfolioSettingList = await dBHelper.FindAsync<DirectPortfolioGeneralSettingQueryModel>(directPortfolioGeneralSettingQuery);

                var directCareCostPercentageQuery = QueryHelper.GetDirectCareCostPercentageQuery(proposal.Account_Market__c);
                careCostPercentList = await dBHelper.FindAsync<DirectCareCostPercentageQueryModel>(directCareCostPercentageQuery);
            }

            //GP:finish method start
            if (Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Quote_Type__c))
            {
                Dictionary<string, List<LineItem>> primaryNoLineItemList1 = new Dictionary<string, List<LineItem>>();
                foreach (var item in cartLineItems)
                {
                    string partNumber = getPartNumber(item);
                    string configType = getConfigType(item);

                    if (configType.equalsIgnoreCase(Constants.BUNDLE) && !proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING))
                    {
                        var key = item.PrimaryLineNumber + Constants.NOKIA_UNDERSCORE + item.ChargeType;
                        if (primaryNoLineItemList.ContainsKey(key))
                        {
                            if (item.GetLineType() != LineType.ProductService)
                            {
                                item.NokiaCPQ_Unitary_Cost__c = 0;
                                foreach (var optionItem in primaryNoLineItemList[key])
                                {
                                    //Stamping Cost at Arcadia level
                                    if (optionItem.NokiaCPQ_Unitary_Cost__c != null)
                                        item.NokiaCPQ_Unitary_Cost__c =
                                           pricingHelper.ApplyRounding((item.NokiaCPQ_Unitary_Cost__c + optionItem.NokiaCPQ_Unitary_Cost__c * optionItem.GetQuantity()), 2, RoundingMode.HALF_UP);
                                }
                            }

                            //stamping arcadia and other direct options prices to the main bundle - was done to resolve sequencing issue
                            if (item.GetLineType() == LineType.ProductService)
                            {
                                if (lineItemIRPMapDirect != null && lineItemIRPMapDirect.ContainsKey(key))
                                {
                                    item.NokiaCPQ_Unitary_IRP__c = 0;
                                    item.NokiaCPQ_Extended_CUP__c = 0;
                                    item.NCPQ_Unitary_CLP__c = 0;

                                    foreach (var optionLineItem in lineItemIRPMapDirect[key])
                                    {
                                        if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase("IP Routing") || proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase("Optics") ||
                                            proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase("Nuage") || proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase("Optics - Wavelite"))
                                        {
                                            if (optionLineItem.GetLineType() != LineType.ProductService)
                                            {
                                                item.NokiaCPQ_Unitary_IRP__c =
                                                    pricingHelper.ApplyRounding(item.NokiaCPQ_Unitary_IRP__c + optionLineItem.NokiaCPQ_Unitary_IRP__c * optionLineItem.GetQuantity(), 2, RoundingMode.HALF_UP);

                                                item.NokiaCPQ_Extended_CUP__c = item.NokiaCPQ_Extended_CUP__c + optionLineItem.AdjustedPrice;
                                            }

                                            if (optionLineItem.BasePriceOverride != null)
                                            {
                                                item.NCPQ_Unitary_CLP__c = pricingHelper.ApplyRounding(item.NCPQ_Unitary_CLP__c
                                                    + pricingHelper.ApplyRounding((optionLineItem.BasePriceOverride), 2, RoundingMode.HALF_UP)
                                                    * optionLineItem.GetQuantity(), 2, RoundingMode.HALF_UP);
                                            }
                                            else if (optionLineItem.BasePrice != null)
                                            {
                                                item.NCPQ_Unitary_CLP__c = pricingHelper.ApplyRounding(item.NCPQ_Unitary_CLP__c
                                                    + pricingHelper.ApplyRounding((optionLineItem.BasePrice), 2, RoundingMode.HALF_UP)
                                                    * optionLineItem.GetQuantity(), 2, RoundingMode.HALF_UP);
                                            }
                                        }
                                        else if (optionLineItem.ParentBundleNumber == item.PrimaryLineNumber)
                                        {
                                            item.NokiaCPQ_Unitary_IRP__c =
                                                pricingHelper.ApplyRounding(item.NokiaCPQ_Unitary_IRP__c
                                                + optionLineItem.NokiaCPQ_Unitary_IRP__c
                                                * optionLineItem.GetQuantity(), 2, RoundingMode.HALF_UP);

                                            if (optionLineItem.BasePriceOverride != null)
                                            {
                                                item.NCPQ_Unitary_CLP__c =
                                                    pricingHelper.ApplyRounding(item.NCPQ_Unitary_CLP__c
                                                    + pricingHelper.ApplyRounding(optionLineItem.BasePriceOverride, 2, RoundingMode.HALF_UP)
                                                    * optionLineItem.GetQuantity(), 2, RoundingMode.HALF_UP);
                                            }
                                            else if (optionLineItem.BasePrice != null)
                                            {
                                                item.NCPQ_Unitary_CLP__c =
                                                    pricingHelper.ApplyRounding(item.NCPQ_Unitary_CLP__c
                                                    + pricingHelper.ApplyRounding(optionLineItem.BasePrice, 2, RoundingMode.HALF_UP)
                                                    * optionLineItem.GetQuantity(), 2, RoundingMode.HALF_UP);
                                            }

                                            item.NokiaCPQ_Extended_CUP__c = item.NokiaCPQ_Extended_CUP__c + optionLineItem.AdjustedPrice;
                                        }
                                    }
                                }
                            }

                            //Stamping Unitary CLP for Software Arcadia items
                            if (item.GetLineType() != LineType.ProductService && item.BasePriceOverride != null)
                            {
                                item.NCPQ_Unitary_CLP__c = pricingHelper.ApplyRounding(item.BasePriceOverride, 2, RoundingMode.HALF_UP);
                            }
                            else if (item.GetLineType() != LineType.ProductService)
                            {
                                item.NCPQ_Unitary_CLP__c = pricingHelper.ApplyRounding(item.BasePrice, 2, RoundingMode.HALF_UP);
                            }

                            if (item.GetLineType() != LineType.ProductService)
                            {
                                item.NokiaCPQ_Extended_CUP__c = item.AdjustedPrice;
                                item.NokiaCPQ_Extended_CNP__c = item.NetPrice;
                            }

                            if (item.ChargeType != null && !item.ChargeType.equalsIgnoreCase("Standard Price"))
                            {
                                item.NokiaCPQ_Extended_IRP2__c = pricingHelper.ApplyRounding(item.NCPQ_Unitary_CLP__c * item.GetQuantity(), 2, RoundingMode.HALF_UP);
                            }
                            else if (item.ChargeType != null)
                            {
                                item.NokiaCPQ_Extended_IRP2__c = pricingHelper.ApplyRounding(item.NokiaCPQ_Unitary_IRP__c * item.GetQuantity(), 2, RoundingMode.HALF_UP);
                            }

                            item.NokiaCPQ_Extended_CLP_2__c = pricingHelper.ApplyRounding(item.NCPQ_Unitary_CLP__c * item.GetQuantity(), 2, RoundingMode.HALF_UP);

                            if (item.GetLineType() == LineType.ProductService)
                            {
                                if (item.NokiaCPQ_AdvancePricing_CUP__c != null)
                                {
                                    item.NokiaCPQ_Extended_CUP_2__c = pricingHelper.ApplyRounding(item.NokiaCPQ_AdvancePricing_CUP__c * item.GetQuantity(), 2, RoundingMode.HALF_UP);
                                }
                                else
                                {
                                    item.NokiaCPQ_Extended_CUP_2__c = pricingHelper.ApplyRounding(item.NokiaCPQ_Extended_CUP__c * item.GetQuantity(), 2, RoundingMode.HALF_UP);
                                }
                            }
                            else
                            {
                                if (item.NokiaCPQ_AdvancePricing_CUP__c != null)
                                {
                                    item.NokiaCPQ_Extended_CUP_2__c = item.NokiaCPQ_AdvancePricing_CUP__c;
                                }
                                else
                                {
                                    item.NokiaCPQ_Extended_CUP_2__c = item.NokiaCPQ_Extended_CUP__c;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING) && partNumber != null
                                && (partNumber.Contains(Constants.MAINTY1CODE) || partNumber.Contains(Constants.MAINTY2CODE)))
                        {
                            item.NokiaCPQ_Extended_IRP2__c = item.NokiaCPQ_Unitary_IRP__c;

                            if (item.BasePriceOverride != null)
                            {
                                item.NokiaCPQ_Extended_CLP_2__c = pricingHelper.ApplyRounding(item.BasePriceOverride, 2, RoundingMode.HALF_UP);
                            }
                            else if (item.BasePrice != null)
                            {
                                item.NokiaCPQ_Extended_CLP_2__c = pricingHelper.ApplyRounding(item.BasePrice, 2, RoundingMode.HALF_UP);
                            }
                            if (item.NokiaCPQ_AdvancePricing_CUP__c != null)
                            {
                                item.NokiaCPQ_Extended_CUP_2__c = item.NokiaCPQ_AdvancePricing_CUP__c;
                            }
                            else
                            {
                                item.NokiaCPQ_Extended_CUP_2__c = item.AdjustedPrice;
                            }
                        }
                        else
                        {
                            item.NokiaCPQ_Extended_IRP2__c = item.NokiaCPQ_Extended_IRP__c;
                            item.NokiaCPQ_Extended_CLP_2__c = item.NokiaCPQ_Extended_CLP__c;
                            if (item.NokiaCPQ_AdvancePricing_CUP__c != null)
                            {
                                item.NokiaCPQ_Extended_CUP_2__c = item.NokiaCPQ_AdvancePricing_CUP__c;
                            }
                            else
                            {
                                item.NokiaCPQ_Extended_CUP_2__c = item.NokiaCPQ_ExtendedPrice_CUP__c;
                            }
                        }
                    }

                    //added by priyanka to calculate sales margin
                    //IN Rolldown mode Extended CNP value is incorrect so using Net Price
                    if (item.NokiaCPQ_AdvancePricing_NP__c > 0 && item.NokiaCPQ_Extended_Cost__c != null)
                    {
                        item.Sales_Margin__c = ((item.NokiaCPQ_AdvancePricing_NP__c - item.NokiaCPQ_Extended_Cost__c) / (item.NokiaCPQ_AdvancePricing_NP__c)) * 100;
                    }
                    else if (item.NokiaCPQ_ExtendedPrice_CNP__c > 0 && item.NokiaCPQ_Extended_Cost__c != null && proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING))
                    {
                        item.Sales_Margin__c = ((item.NokiaCPQ_ExtendedPrice_CNP__c - item.NokiaCPQ_Extended_Cost__c) / (item.NokiaCPQ_ExtendedPrice_CNP__c)) * 100;
                    }
                    else if (item.NetPrice > 0 && item.NokiaCPQ_Extended_Cost__c != null)
                    {
                        item.Sales_Margin__c = ((item.NetPrice - item.NokiaCPQ_Extended_Cost__c) / (item.NetPrice)) * 100;
                    }
                    else
                    {
                        item.Sales_Margin__c = 0;
                    }

                    //Calculating IRP Discount.IN Rolldown mode Extended CNP value is incorrect so using Net Price
                    if (item.NokiaCPQ_Extended_IRP2__c != null && item.NokiaCPQ_Extended_IRP2__c != 0)
                    {
                        if (item.NokiaCPQ_AdvancePricing_NP__c > 0)
                        {
                            item.NokiaCPQ_IRP_Discount__c = ((item.NokiaCPQ_Extended_IRP2__c - item.NokiaCPQ_AdvancePricing_NP__c) / item.NokiaCPQ_Extended_IRP2__c) * 100;

                        }
                        else
                        {
                            if (item.NokiaCPQ_ExtendedPrice_CNP__c != null && proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING))
                            {

                                item.NokiaCPQ_IRP_Discount__c = ((item.NokiaCPQ_Extended_IRP2__c - item.NokiaCPQ_ExtendedPrice_CNP__c) / item.NokiaCPQ_Extended_IRP2__c) * 100;
                            }
                            else
                            {
                                item.NokiaCPQ_IRP_Discount__c = ((item.NokiaCPQ_Extended_IRP2__c - item.NetPrice) / item.NokiaCPQ_Extended_IRP2__c) * 100;
                            }
                        }
                    }
                    else
                    {
                        item.NokiaCPQ_IRP_Discount__c = 0;
                    }
                    item.NokiaCPQ_IRP_Discount__c = pricingHelper.ApplyRounding(item.NokiaCPQ_IRP_Discount__c, 2, RoundingMode.HALF_UP);

                    //CPQ Requirement : Traffic Light calculations For MN Direct, NSW, Enterprise, QTC
                    deal_Guidance_Calculator(item, configType, pricingGuidanceSettingThresold);

                    //Care Cost
                    if (item.Advanced_Pricing_Done__c == false && !portfolioSettingList.isEmpty() && portfolioSettingList[0].Cost_Calculation_In_PCB__c == false && item.ChargeType != null && item.ChargeType.equalsIgnoreCase(Constants.NOKIA_YEAR1_MAINTENANCE) && item.GetLineType() != LineType.ProductService)
                    {
                        if (!careCostPercentList.isEmpty() && careCostPercentList[0].Care_Cost__c != null && item.NokiaCPQ_ExtendedPrice_CNP__c != null)
                        {
                            item.NokiaCPQ_Unitary_Cost__c = pricingHelper.ApplyRounding((item.NokiaCPQ_ExtendedPrice_CNP__c * (careCostPercentList[0].Care_Cost__c / 100)), 2, RoundingMode.HALF_UP);
                        }
                    }

                    if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING))
                    {
                        if (item.GetLineType() != LineType.ProductService)
                        {
                            if (primaryNoLineItemList1.ContainsKey(item.ParentBundleNumber + Constants.NOKIA_UNDERSCORE + item.ChargeType))
                            {
                                primaryNoLineItemList1[item.ParentBundleNumber + Constants.NOKIA_UNDERSCORE + item.ChargeType].Add(item);
                            }
                            else
                            {
                                primaryNoLineItemList1.Add(item.ParentBundleNumber + Constants.NOKIA_UNDERSCORE + item.ChargeType, new List<LineItem> { item });
                            }
                        }
                    }
                }

                if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING))
                {
                    foreach (var item in cartLineItems)
                    {
                        string configType = getConfigType(item);
                        //Piyush Tawari Start
                        //Req-6228 MN Airscale wifi - Price for the groups to be aggregated from SI
                        //For Direct MN Airscale Wifi
                        //checking if its Group
                        if (item.ChargeType != null && item.GetLineType() == LineType.Option &&
                             Constants.BUNDLE.equalsIgnoreCase(configType))
                        {
                            item.NokiaCPQ_Unitary_Cost__c = 0;
                            item.NCPQ_Unitary_CLP__c = 0;
                            item.NokiaCPQ_Unitary_IRP__c = 0;
                            item.NokiaCPQ_Extended_CUP__c = 0;
                            item.NokiaCPQ_Extended_CNP__c = 0;

                            var key = item.PrimaryLineNumber + '_' + item.ChargeType;
                            if (primaryNoLineItemList1.ContainsKey(key))
                            {
                                foreach (var optionItem in primaryNoLineItemList1[key])
                                {
                                    //Stamping IRP at Group Level
                                    if (optionItem.NokiaCPQ_Unitary_IRP__c != null && optionItem.Quantity != null)
                                        item.NokiaCPQ_Unitary_IRP__c = pricingHelper.ApplyRounding((item.NokiaCPQ_Unitary_IRP__c + optionItem.NokiaCPQ_Unitary_IRP__c * optionItem.GetQuantity()), 2, RoundingMode.HALF_UP);

                                    //stamping CLP at Group level
                                    if (optionItem.BasePriceOverride != null)
                                    {
                                        item.NCPQ_Unitary_CLP__c = pricingHelper.ApplyRounding((item.NCPQ_Unitary_CLP__c
                                            + pricingHelper.ApplyRounding((optionItem.BasePriceOverride), 2, RoundingMode.HALF_UP)
                                            * optionItem.GetQuantity()), 2, RoundingMode.HALF_UP);
                                    }
                                    else if (optionItem.BasePrice != null)
                                    {
                                        item.NCPQ_Unitary_CLP__c =
                                            pricingHelper.ApplyRounding((item.NCPQ_Unitary_CLP__c + pricingHelper.ApplyRounding((optionItem.BasePrice), 2, RoundingMode.HALF_UP) * optionItem.GetQuantity()), 2, RoundingMode.HALF_UP);
                                    }
                                    //Stamping CUP at Group level
                                    item.NokiaCPQ_Extended_CUP__c = item.NokiaCPQ_Extended_CUP__c + optionItem.AdjustedPrice;
                                    //stamping CNP at Group Level
                                    item.NokiaCPQ_Extended_CNP__c = item.NokiaCPQ_Extended_CNP__c + optionItem.NetPrice;
                                    item.NokiaCPQ_Unitary_Cost__c = pricingHelper.ApplyRounding((item.NokiaCPQ_Unitary_Cost__c + pricingHelper.ApplyRounding((optionItem.NokiaCPQ_Unitary_Cost__c), 2, RoundingMode.HALF_UP) * optionItem.GetQuantity()), 2, RoundingMode.HALF_UP);
                                }
                            }

                            if (item.AdjustmentType == Constants.DISCOUNT_AMOUNT)
                            {
                                item.BasePriceOverride = item.AdjustmentAmount;
                            }
                            else
                            {
                                item.BasePriceOverride = 0;
                            }
                        }//Piyush Tawari End
                         //Logic for calculating Cost for main bundle
                        if (item.GetLineType() != LineType.ProductService && !IsOptionLineFromSubBundle(item))
                        {
                            var key = item.ParentBundleNumber + Constants.NOKIA_UNDERSCORE + item.ChargeType;
                            if (unitaryCostMap.ContainsKey(key))
                            {
                                decimal? itemCost = 0;
                                if (configType.equalsIgnoreCase("Bundle") && item.NokiaCPQ_Unitary_Cost__c != null)
                                {
                                    itemCost = unitaryCostMap[key] + item.NokiaCPQ_Unitary_Cost__c;
                                    unitaryCostMap[key] = itemCost;
                                }
                                else if (item.NokiaCPQ_Unitary_Cost__c != null)
                                {
                                    itemCost = unitaryCostMap[key] + pricingHelper.ApplyRounding((item.NokiaCPQ_Unitary_Cost__c * item.GetQuantity()), 2, RoundingMode.HALF_UP);
                                    unitaryCostMap[key] = itemCost;
                                }
                            }
                            else
                            {
                                if (configType.equalsIgnoreCase("Bundle") && item.NokiaCPQ_Unitary_Cost__c != null)
                                {
                                    unitaryCostMap.Add(key, item.NokiaCPQ_Unitary_Cost__c);
                                }
                                else if (item.NokiaCPQ_Unitary_Cost__c != null)
                                {
                                    unitaryCostMap.Add(key, pricingHelper.ApplyRounding((item.NokiaCPQ_Unitary_Cost__c * item.GetQuantity()), 2, RoundingMode.HALF_UP));
                                }
                            }
                        }
                    }
                }

                Dictionary<decimal?, decimal?> linenumberCareDisPercentage = new Dictionary<decimal?, decimal?>();
                Dictionary<decimal?, decimal?> linenumberSRSDisPercentage = new Dictionary<decimal?, decimal?>();
                Dictionary<decimal?, decimal?> linenumberCarePrice = new Dictionary<decimal?, decimal?>();
                Dictionary<decimal?, decimal?> linenumberSRSPrice = new Dictionary<decimal?, decimal?>();
                Dictionary<decimal?, List<decimal?>> linenumberCareCLPCNP = new Dictionary<decimal?, List<decimal?>>();
                Dictionary<decimal?, List<decimal?>> linenumberSRSCLPCNP = new Dictionary<decimal?, List<decimal?>>();

                //Start: creating map to limit for loop iterations
                Dictionary<string, LineItem> mainBundleMap = new Dictionary<string, LineItem>();
                //Stop
                //R-6508 
                decimal? totalExtendedSSPPrice = 0;
                decimal? totalExtendedSRSPrice = 0;

                //R-6508 End
                if (!proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING))
                {
                    Dictionary<string, LineItem> ssp_srsLinesMap_EP = new Dictionary<string, LineItem>();
                    foreach (LineItem item in cartLineItems)
                    {
                        string configType = getConfigType(item);
                        decimal? CareDiscCalc = 0;
                        decimal? SRSDiscCalc = 0;

                        if (item.ChargeType != null && item.ChargeType.equalsIgnoreCase(Constants.STANDARD) && item.GetLineType() == LineType.ProductService && proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase("Nokia Software"))
                        {
                            var key = item.LineNumber + Constants.NOKIA_UNDERSCORE + item.ChargeType;
                            if (lineItemIRPMapDirect.ContainsKey(item.LineNumber + Constants.NOKIA_UNDERSCORE + item.ChargeType))
                            {
                                decimal? careExtendedCLP = item.NokiaCPQ_Extended_CLP_2__c;
                                decimal? careNetCNP = item.NetPrice;
                                decimal? srsExtendedCLP = item.NokiaCPQ_Extended_CLP_2__c;
                                decimal? srsNetCNP = item.NetPrice;

                                foreach (var optionLineItem in lineItemIRPMapDirect[key])
                                {
                                    if (optionLineItem.ParentBundleNumber == item.PrimaryLineNumber)
                                    {
                                        string classification = getClassification(optionLineItem);
                                        string itemType = getItemType(optionLineItem);

                                        if (item.ChargeType == "Standard Price")
                                        {
                                            if (classification != null && classification == "Professional Services")
                                            {
                                                if (optionLineItem.NokiaCPQ_Extended_CLP_2__c != null)
                                                {
                                                    careExtendedCLP = careExtendedCLP - optionLineItem.NokiaCPQ_Extended_CLP_2__c;
                                                    srsExtendedCLP = srsExtendedCLP - optionLineItem.NokiaCPQ_Extended_CLP_2__c;
                                                }
                                                careNetCNP = careNetCNP - optionLineItem.NetPrice;
                                                srsNetCNP = srsNetCNP - optionLineItem.NetPrice;
                                            }

                                            if (itemType != null && itemType == "Hardware")
                                            {
                                                srsExtendedCLP = srsExtendedCLP - optionLineItem.NokiaCPQ_Extended_CLP_2__c;
                                                srsNetCNP = srsNetCNP - optionLineItem.NetPrice;
                                            }
                                        }
                                    }
                                }

                                if (item.ChargeType == "Standard Price")
                                {
                                    List<decimal?> listCareCLPCNP = new List<decimal?>();
                                    if (careExtendedCLP != null)
                                    {
                                        listCareCLPCNP.Add(careExtendedCLP);
                                    }
                                    if (careNetCNP != null)
                                    {
                                        listCareCLPCNP.Add(careNetCNP);
                                    }
                                    List<decimal?> listSRSCLPCNP = new List<decimal?>();
                                    if (srsExtendedCLP != null)
                                    {
                                        listSRSCLPCNP.Add(srsExtendedCLP);
                                    }
                                    if (srsNetCNP != null)
                                    {
                                        listSRSCLPCNP.Add(srsNetCNP);
                                    }
                                    if (!listCareCLPCNP.isEmpty())
                                    {
                                        linenumberCareCLPCNP.Add(item.LineNumber, listCareCLPCNP);
                                    }
                                    if (!listSRSCLPCNP.isEmpty())
                                    {
                                        linenumberSRSCLPCNP.Add(item.LineNumber, listSRSCLPCNP);
                                    }
                                }
                            }

                            if (linenumberCareCLPCNP.ContainsKey(item.LineNumber) && linenumberCareCLPCNP[item.LineNumber] != null && linenumberSRSCLPCNP.ContainsKey(item.LineNumber) && linenumberSRSCLPCNP[item.LineNumber] != null)
                            {
                                if (!linenumberCareCLPCNP[item.LineNumber].isEmpty() && linenumberCareCLPCNP[item.LineNumber][0] != 0)
                                {
                                    CareDiscCalc = (1 - ((linenumberCareCLPCNP[item.LineNumber][0] - linenumberCareCLPCNP[item.LineNumber][1]) / linenumberCareCLPCNP[item.LineNumber][0]));
                                }
                                if (!linenumberSRSCLPCNP[item.LineNumber].isEmpty() && linenumberSRSCLPCNP[item.LineNumber][0] != 0)
                                {
                                    SRSDiscCalc = (1 - ((linenumberSRSCLPCNP[item.LineNumber][0] - linenumberSRSCLPCNP[item.LineNumber][1]) / linenumberSRSCLPCNP[item.LineNumber][0]));
                                }

                                if (CareDiscCalc != null)
                                {
                                    linenumberCareDisPercentage.Add(item.LineNumber, pricingHelper.ApplyRounding(CareDiscCalc, 2, RoundingMode.HALF_UP));
                                }
                                if (SRSDiscCalc != null)
                                {
                                    linenumberSRSDisPercentage.Add(item.LineNumber, pricingHelper.ApplyRounding(SRSDiscCalc, 2, RoundingMode.HALF_UP));
                                }
                            }
                        }

                        //R-6508 --ssp and SRS calculation for Enterprise
                        int quantityBundle = 1;
                        string partNumber = getPartNumber(item);

                        if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_IP_ROUTING) && proposal.Is_List_Price_Only__c == false)
                        {
                            if (partNumber != null &&
                                !partNumber.Contains(Constants.MAINTY1CODE) &&
                                !partNumber.Contains(Constants.MAINTY2CODE) &&
                               !partNumber.Contains(Constants.SSPCODE) &&
                               !partNumber.Contains(Constants.SRS) &&
                               item.Is_List_Price_Only__c == false)
                            {
                                if (item.GetLineType() == LineType.Option)
                                {
                                    if (this.lineItemObjectMap[Constants.NOKIA_PRODUCT_SERVICES + Constants.NOKIA_UNDERSCORE + item.LineNumber] != null)
                                    {
                                        if (this.lineItemObjectMap[Constants.NOKIA_PRODUCT_SERVICES + Constants.NOKIA_UNDERSCORE + item.LineNumber].Quantity != null)
                                        {
                                            quantityBundle = Convert.ToInt32(Math.Ceiling(lineItemObjectMap[Constants.NOKIA_PRODUCT_SERVICES + Constants.NOKIA_UNDERSCORE + item.LineNumber].Quantity.Value));
                                        }
                                    }
                                }

                                if (item.NetPrice != null && item.NokiaCPQ_SSP_Rate__c != null && item.Apttus_Config2__ProductId__r_IsSSP__c == false)
                                {
                                    item.Nokia_SSP_Base_Extended_Price__c = pricingHelper.ApplyRounding((item.NetPrice * item.NokiaCPQ_SSP_Rate__c * 0.01m), 2, RoundingMode.HALF_UP) * quantityBundle;
                                }

                                if (item.NetPrice != null && item.NokiaCPQ_SRS_Rate__c != null && item.Apttus_Config2__ProductId__r_IsSSP__c == true)
                                {
                                    item.Nokia_SRS_Base_Extended_Price__c = pricingHelper.ApplyRounding((item.NetPrice * item.NokiaCPQ_SRS_Rate__c * 0.01m), 2, RoundingMode.HALF_UP) * quantityBundle;
                                }

                                totalExtendedSSPPrice = totalExtendedSSPPrice + pricingHelper.ApplyRounding((item.Nokia_SSP_Base_Extended_Price__c), 2, RoundingMode.HALF_UP);
                                totalExtendedSRSPrice = totalExtendedSRSPrice + pricingHelper.ApplyRounding((item.Nokia_SRS_Base_Extended_Price__c), 2, RoundingMode.HALF_UP);
                            }

                            if (partNumber != null && partNumber.Contains(Constants.SSPCODE))
                            {
                                ssp_srsLinesMap_EP.Add("SSP", item);
                            }
                            else if (partNumber != null && partNumber.Contains(Constants.SRS))
                            {
                                ssp_srsLinesMap_EP.Add("SRS", item);
                            }
                        }
                        //R-6508 End

                        //Logic for calculating Cost for main bundle

                        if (item.GetLineType() != LineType.ProductService && !IsOptionLineFromSubBundle(item))
                        {
                            if (unitaryCostMap.ContainsKey(item.ParentBundleNumber + Constants.NOKIA_UNDERSCORE + item.ChargeType))
                            {
                                decimal? itemCost = 0;
                                if ((configType.equalsIgnoreCase(Constants.BUNDLE) || item.NokiaCPQ_IsArcadiaBundle__c == true) && item.NokiaCPQ_Unitary_Cost__c != null)
                                {
                                    itemCost = unitaryCostMap[item.ParentBundleNumber + Constants.NOKIA_UNDERSCORE + item.ChargeType] + item.NokiaCPQ_Unitary_Cost__c;
                                    unitaryCostMap[item.ParentBundleNumber + Constants.NOKIA_UNDERSCORE + item.ChargeType] = itemCost;
                                }
                                else if (item.NokiaCPQ_Unitary_Cost__c != null)
                                {
                                    itemCost = unitaryCostMap[item.ParentBundleNumber + Constants.NOKIA_UNDERSCORE + item.ChargeType] +
                                        pricingHelper.ApplyRounding((item.NokiaCPQ_Unitary_Cost__c * item.Quantity), 2, RoundingMode.HALF_UP);

                                    unitaryCostMap[item.ParentBundleNumber + Constants.NOKIA_UNDERSCORE + item.ChargeType] = itemCost;
                                }
                            }
                            else
                            {
                                if ((configType.equalsIgnoreCase(Constants.BUNDLE) || item.NokiaCPQ_IsArcadiaBundle__c == true) && item.NokiaCPQ_Unitary_Cost__c != null)
                                {
                                    unitaryCostMap.Add(item.ParentBundleNumber + Constants.NOKIA_UNDERSCORE + item.ChargeType, item.NokiaCPQ_Unitary_Cost__c);
                                }
                                else if (item.NokiaCPQ_Unitary_Cost__c != null)
                                {
                                    unitaryCostMap.Add(item.ParentBundleNumber + Constants.NOKIA_UNDERSCORE + item.ChargeType, pricingHelper.ApplyRounding((item.NokiaCPQ_Unitary_Cost__c * item.Quantity), 2, RoundingMode.HALF_UP));
                                }
                            }
                        }
                    }
                    // 6508  
                    LineItem lineItemVarSO;
                    bool? isUpdateSSP = false;
                    if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_IP_ROUTING) && ssp_srsLinesMap_EP.Count > 0)
                    {
                        if (ssp_srsLinesMap_EP.ContainsKey("SSP"))
                        {
                            lineItemVarSO = ssp_srsLinesMap_EP["SSP"];

                            if (lineItemVarSO.BasePriceOverride != totalExtendedSSPPrice)
                            {
                                lineItemVarSO.BasePriceOverride = totalExtendedSSPPrice;
                                lineItemVarSO.NokiaCPQ_Unitary_IRP__c = totalExtendedSSPPrice;
                                lineItemVarSO.LineSequence = 998;
                                isUpdateSSP = true;
                            }

                            if (isUpdateSSP == true)
                            {
                                lineItemVarSO.UpdatePrice(pricingHelper);
                                isUpdateSSP = false;
                            }
                        }
                        if (ssp_srsLinesMap_EP.ContainsKey("SRS"))
                        {
                            lineItemVarSO = ssp_srsLinesMap_EP["SRS"];

                            if (lineItemVarSO.BasePriceOverride != totalExtendedSRSPrice)
                            {
                                lineItemVarSO.BasePriceOverride = totalExtendedSRSPrice;
                                lineItemVarSO.NokiaCPQ_Unitary_IRP__c = totalExtendedSRSPrice;
                                lineItemVarSO.LineSequence = 999;
                                isUpdateSSP = true;
                            }

                            if (isUpdateSSP == true)
                            {
                                lineItemVarSO.UpdatePrice(pricingHelper);
                                isUpdateSSP = false;
                            }
                        }
                    }
                    //6508 End
                }

                Dictionary<decimal?, decimal?> mainBundleToGroupIRPMap = new Dictionary<decimal?, decimal?>();
                Dictionary<decimal?, decimal?> mainBundleToGroupCLPMap = new Dictionary<decimal?, decimal?>();
                Dictionary<decimal?, decimal?> mainBundleToGroupCUPMap = new Dictionary<decimal?, decimal?>();
                foreach (var item in cartLineItems)
                {
                    string configType = getConfigType(item);
                    if (item.ChargeType != null && item.ChargeType.equalsIgnoreCase(Constants.NOKIA_YEAR1_MAINTENANCE) && item.GetLineType() != LineType.ProductService && proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase("Nokia Software"))
                    {
                        if (linenumberCareDisPercentage.Count > 0 && linenumberCareDisPercentage.ContainsKey(item.LineNumber))
                        {
                            if (linenumberCareDisPercentage[item.LineNumber] > 0)
                            {
                                decimal? calculatedPrice = pricingHelper.ApplyRounding(((item.NokiaCPQ_CareSRSBasePrice__c / conversionRate) * proposal.Exchange_Rate__c) * linenumberCareDisPercentage[item.LineNumber], 2, RoundingMode.HALF_UP);

                                if (item.BasePriceOverride != calculatedPrice)
                                {
                                    item.BasePriceOverride = pricingHelper.ApplyRounding(((item.NokiaCPQ_CareSRSBasePrice__c / conversionRate) * proposal.Exchange_Rate__c) * linenumberCareDisPercentage[item.LineNumber], 2, RoundingMode.HALF_UP);
                                }
                            }
                            linenumberCarePrice.Add(item.LineNumber, pricingHelper.ApplyRounding(item.BasePriceOverride, 2, RoundingMode.HALF_UP));
                        }
                    }
                    if (item.ChargeType != null && item.ChargeType.equalsIgnoreCase(Constants.NOKIA_SRS) && item.GetLineType() != LineType.ProductService && proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase("Nokia Software"))
                    {
                        if (linenumberSRSDisPercentage.Count > 0 && linenumberSRSDisPercentage.ContainsKey(item.LineNumber))
                        {
                            if (linenumberSRSDisPercentage[item.LineNumber] > 0)
                            {
                                decimal? calculatedSRSPrice = pricingHelper.ApplyRounding(((item.NokiaCPQ_SRSBasePrice__c / conversionRate) * proposal.Exchange_Rate__c) * linenumberSRSDisPercentage[item.LineNumber], 2, RoundingMode.HALF_UP);

                                if (item.BasePriceOverride != calculatedSRSPrice)
                                {
                                    item.BasePriceOverride = pricingHelper.ApplyRounding(((item.NokiaCPQ_SRSBasePrice__c / conversionRate) * proposal.Exchange_Rate__c) * linenumberSRSDisPercentage[item.LineNumber], 2, RoundingMode.HALF_UP);
                                }
                            }

                            linenumberSRSPrice.Add(item.LineNumber, pricingHelper.ApplyRounding(item.BasePriceOverride, 2, RoundingMode.HALF_UP));
                        }
                    }
                    //Map for Stamping IRP,CLP,CUP for main bundle Airscale wifi Direct MN
                    if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING) && item.GetLineType() == LineType.Option &&
                        Constants.BUNDLE.equalsIgnoreCase(configType))
                    {
                        if (mainBundleToGroupIRPMap.ContainsKey(item.LineNumber))
                        {
                            if (item.NokiaCPQ_Unitary_IRP__c != null)
                                mainBundleToGroupIRPMap[item.LineNumber] = pricingHelper.ApplyRounding((mainBundleToGroupIRPMap[item.LineNumber] + item.NokiaCPQ_Unitary_IRP__c * item.Quantity), 2, RoundingMode.HALF_UP);
                        }
                        else
                        {
                            if (item.NokiaCPQ_Unitary_IRP__c != null)
                                mainBundleToGroupIRPMap.Add(item.LineNumber, pricingHelper.ApplyRounding((item.NokiaCPQ_Unitary_IRP__c * item.Quantity), 2, RoundingMode.HALF_UP));
                        }

                        if (mainBundleToGroupCLPMap.ContainsKey(item.LineNumber))
                        {
                            if (item.NCPQ_Unitary_CLP__c != null)
                                mainBundleToGroupCLPMap[item.LineNumber] = pricingHelper.ApplyRounding((mainBundleToGroupCLPMap[item.LineNumber] + item.NCPQ_Unitary_CLP__c * item.Quantity), 2, RoundingMode.HALF_UP);
                        }
                        else
                        {
                            if (item.NCPQ_Unitary_CLP__c != null)
                                mainBundleToGroupCLPMap.Add(item.LineNumber, pricingHelper.ApplyRounding((item.NCPQ_Unitary_CLP__c * item.Quantity), 2, RoundingMode.HALF_UP));
                        }

                        if (mainBundleToGroupCUPMap.ContainsKey(item.LineNumber))
                        {
                            if (item.NokiaCPQ_Extended_CUP__c != null)
                                mainBundleToGroupCUPMap[item.LineNumber] = pricingHelper.ApplyRounding((mainBundleToGroupCUPMap[item.LineNumber] + item.NokiaCPQ_Extended_CUP__c), 2, RoundingMode.HALF_UP);
                        }
                        else
                        {
                            if (item.NokiaCPQ_Extended_CUP__c != null)
                                mainBundleToGroupCUPMap.Add(item.LineNumber, pricingHelper.ApplyRounding((item.NokiaCPQ_Extended_CUP__c), 2, RoundingMode.HALF_UP));
                        }
                    }
                    //Start: create map to limit for loop iterations
                    if (item.ChargeType != null && item.GetLineType() == LineType.ProductService)
                    {
                        mainBundleMap.Add(item.Id, item);
                    }
                    //Stop

                    if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase("Nokia Software"))
                        item.UpdatePrice(pricingHelper);
                }

                foreach (var item in mainBundleMap.Values)
                {
                    string configType = getConfigType(item);
                    string partNumber = getPartNumber(item);
                    if (item.ChargeType != null && item.ChargeType.equalsIgnoreCase(Constants.NOKIA_YEAR1_MAINTENANCE) && !proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING))
                    {
                        if (linenumberCarePrice.Count > 0 && linenumberCarePrice.ContainsKey(item.LineNumber) && item.Quantity > 0 && linenumberCarePrice[item.LineNumber] > 0 && linenumberCarePrice[item.LineNumber] != (item.NetPrice / item.Quantity))
                        {
                            item.PricingStatus = "Pending";
                        }
                    }
                    if (item.ChargeType != null && item.ChargeType.equalsIgnoreCase(Constants.NOKIA_SRS) && !proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING))
                    {
                        if (linenumberSRSPrice.Count > 0 && linenumberSRSPrice.ContainsKey(item.LineNumber) && item.Quantity > 0 && linenumberSRSPrice[item.LineNumber] > 0 && linenumberSRSPrice[item.LineNumber] != (item.NetPrice / item.Quantity))
                        {
                            item.PricingStatus = "Pending";
                        }
                    }
                    //Stamping at main bundle
                    if (unitaryCostMap.ContainsKey(item.PrimaryLineNumber + Constants.NOKIA_UNDERSCORE + item.ChargeType))
                    {
                        item.NokiaCPQ_Unitary_Cost__c = unitaryCostMap[item.PrimaryLineNumber + Constants.NOKIA_UNDERSCORE + item.ChargeType];
                    }
                    //Stamping at main IRP, CLP, CUP bundle for Airscale wifi
                    if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING) && Constants.BUNDLE.equalsIgnoreCase(configType))
                    {
                        if (mainBundleToGroupIRPMap.Count > 0 && mainBundleToGroupIRPMap.ContainsKey(item.LineNumber))
                        {
                            item.NokiaCPQ_Unitary_IRP__c = mainBundleToGroupIRPMap[item.LineNumber];
                        }
                        if (mainBundleToGroupCLPMap.Count > 0 && mainBundleToGroupCLPMap.ContainsKey(item.LineNumber))
                        {
                            item.NCPQ_Unitary_CLP__c = mainBundleToGroupCLPMap[item.LineNumber];
                        }
                        if (mainBundleToGroupCUPMap.Count > 0 && mainBundleToGroupCUPMap.ContainsKey(item.LineNumber))
                        {
                            item.NokiaCPQ_Extended_CUP__c = mainBundleToGroupCUPMap[item.LineNumber];
                        }
                        if (item.NokiaCPQ_Unitary_IRP__c != null && item.Quantity != null)
                            item.NokiaCPQ_Extended_IRP2__c = pricingHelper.ApplyRounding((item.NokiaCPQ_Unitary_IRP__c * item.Quantity), 2, RoundingMode.HALF_UP);
                        if (item.NCPQ_Unitary_CLP__c != null && item.Quantity != null)
                            item.NokiaCPQ_Extended_CLP_2__c = pricingHelper.ApplyRounding((item.NCPQ_Unitary_CLP__c * item.Quantity), 2, RoundingMode.HALF_UP);
                        if (item.NokiaCPQ_Extended_CUP__c != null && item.Quantity != null)
                            item.NokiaCPQ_Extended_CUP_2__c = pricingHelper.ApplyRounding((item.NokiaCPQ_Extended_CUP__c * item.Quantity), 2, RoundingMode.HALF_UP);
                    }

                    //For Enterprise
                    if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_IP_ROUTING) && proposal.Is_List_Price_Only__c == false)
                    {
                        //system.debug('enterprise 2nd reprice for SSP/SRS' + item.NokiaCPQ_Extended_IRP2__c + '  ' + item.Apttus_Config2__BasePriceOverride__c*item.Apttus_Config2__Quantity__c);
                        if (partNumber != null && (partNumber.Contains(Constants.SSPCODE) || partNumber.Contains(Constants.SRS)) && item.Quantity != null &&
                            item.NokiaCPQ_Extended_IRP2__c != pricingHelper.ApplyRounding((item.BasePriceOverride * item.Quantity), 2, RoundingMode.HALF_UP))
                        {
                            item.PricingStatus = "Pending";
                        }
                    }
                }
            }

            await Task.CompletedTask;
        }

        private bool IsLeo()
        {
            return string.Compare(proposal.Quote_Type__c, Constants.QUOTE_TYPE_INDIRECTCPQ, true) == 0
                && string.Compare(proposal.NokiaCPQ_No_Of_Years__c, Constants.NOKIA_1YEAR, true) == 0
                && proposal.NokiaCPQ_LEO_Discount__c == true;
        }

        private bool UsePricingGuidanceSettingThresold()
        {
            return string.Compare(proposal.Quote_Type__c, Constants.QUOTE_TYPE_DIRECTCPQ, true) == 0 &&
                    (string.Compare(proposal.NokiaCPQ_Portfolio__c, Constants.QTC, true) == 0 ||
                    (string.Compare(proposal.NokiaCPQ_Portfolio__c, Constants.NOKIA_IP_ROUTING, true) == 0 &&
                    proposal.Is_List_Price_Only__c == false));
        }

        private string GetConfigType(LineItem lineItem)
        {
            string configType = string.Empty;

            if (lineItem.GetLineType() == LineType.ProductService)
            {
                configType = lineItem.Apttus_Config2__ProductId__r_Apttus_Config2__ConfigurationType__c;
            }
            else
            {
                configType = lineItem.Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c;
            }

            return configType;
        }

        private bool IsOptionLineFromSubBundle(LineItem lineItem)
        {
            if (lineItem.GetLineType() != LineType.Option || lineItem.GetRootParentLineItem() == null)
                return false;

            return lineItem.ParentBundleNumber != lineItem.GetRootParentLineItem().GetPrimaryLineNumber();
        }

        private string getPartNumber(LineItem lineItem)
        {
            string partNumber = string.Empty;
            if (lineItem.Is_Custom_Product__c == true)
            {
                partNumber = lineItem.Custom_Product_Code__c;
            }
            else
            {
                if (lineItem.GetLineType() == LineType.ProductService)
                {
                    partNumber = lineItem.Apttus_Config2__ProductId__r_ProductCode;
                }
                else
                {
                    partNumber = lineItem.Apttus_Config2__OptionId__r_ProductCode;
                }
            }

            return partNumber;
        }

        string getConfigType(LineItem lineItem)
        {
            string configType = string.Empty;

            if (lineItem.GetLineType() == LineType.ProductService)
            {
                configType = lineItem.Apttus_Config2__ProductId__r_Apttus_Config2__ConfigurationType__c;
                //configType = String.valueOf(item.Apttus_Config2__ProductId__r.Apttus_Config2__ConfigurationType__c);
            }
            else
            {
                configType = lineItem.Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c;
                //configType = String.valueOf(item.Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c);
            }

            return configType;
        }

        private string getProductDiscountCategory(LineItem lineItemModel)
        {
            string productDiscountCat = string.Empty;

            if (lineItemModel.GetLineType() == LineType.ProductService)
            {
                productDiscountCat = lineItemModel.Apttus_Config2__ProductId__r_NokiaCPQ_Product_Discount_Category__c;
            }
            else
            {
                productDiscountCat = lineItemModel.Apttus_Config2__OptionId__r_NokiaCPQ_Product_Discount_Category__c;
            }

            return productDiscountCat;
        }

        //R-6508
        /* Method Name   : calculateMaintenance_EP_Direct
        * Developer	  : Accenture
        * Description	:  The method calculates Maintenance per line item for Direct Enterprise Quotes */
        private Dictionary<string, decimal?> calculateMaintenance_EP_Direct(LineItem item, decimal? totalExtendedMaintY1Price, decimal? totalExtendedMaintY2Price, string partNumber)
        {
            Dictionary<string, decimal?> maintenanceValueMap = new Dictionary<string, decimal?>();
            string itemName = item.ChargeType;

            decimal? ExtendedMaintY1Price = totalExtendedMaintY1Price;
            decimal? ExtendedMaintY2Price = totalExtendedMaintY2Price;
            int quantityBundle = 1;

            if (partNumber != null && !partNumber.Contains(Constants.MAINTY1CODE) &&
                !partNumber.Contains(Constants.MAINTY2CODE) &&
                !partNumber.Contains(Constants.SSPCODE) &&
                !partNumber.Contains(Constants.SRS))
            {
                if (item.GetLineType() == LineType.Option)
                {
                    var key = Constants.NOKIA_PRODUCT_SERVICES + Constants.NOKIA_UNDERSCORE + item.GetLineNumber();
                    if (lineItemObjectMap.ContainsKey(key) && lineItemObjectMap[key] != null)
                    {
                        if (lineItemObjectMap[key].Quantity != null)
                        {
                            quantityBundle = Convert.ToInt32(Math.Ceiling(lineItemObjectMap[key].Quantity.Value));
                        }
                    }
                }
            }

            if (item.NokiaCPQ_Extended_IRP__c != null && item.Nokia_Maint_Y1_Per__c != null &&
                item.Nokia_Maint_Y2_Per__c != null)
            {
                item.NokiaCPQ_Maint_Yr1_Extended_Price__c = pricingHelper.ApplyRounding(item.NokiaCPQ_Extended_IRP__c * item.Nokia_Maint_Y1_Per__c * 0.01m, 2, RoundingMode.HALF_UP) * quantityBundle;
                item.NokiaCPQ_Maint_Yr2_Extended_Price__c = pricingHelper.ApplyRounding(item.NokiaCPQ_Extended_IRP__c * item.Nokia_Maint_Y2_Per__c * 0.01m, 2, RoundingMode.HALF_UP) * quantityBundle;
            }

            ExtendedMaintY1Price = ExtendedMaintY1Price + pricingHelper.ApplyRounding(item.NokiaCPQ_Maint_Yr1_Extended_Price__c, 2, RoundingMode.HALF_UP);
            ExtendedMaintY2Price = ExtendedMaintY2Price + pricingHelper.ApplyRounding(item.NokiaCPQ_Maint_Yr2_Extended_Price__c, 2, RoundingMode.HALF_UP);

            maintenanceValueMap.Add("ExtendedMaintY1Price", ExtendedMaintY1Price);
            maintenanceValueMap.Add("ExtendedMaintY2Price", ExtendedMaintY2Price);

            return maintenanceValueMap;

        }

        private Dictionary<string, decimal?> calculateMaintenance_MN_Direct(LineItem item, decimal? totalExtendedMaintY1Price, decimal? totalExtendedMaintY2Price, string partNumber, string configType)
        {
            Dictionary<string, decimal?> maintenanceValueMap = new Dictionary<string, decimal?>();
            decimal? ExtendedMaintY1Price = totalExtendedMaintY1Price;
            decimal? ExtendedMaintY2Price = totalExtendedMaintY2Price;
            decimal? groupQuantity = 1;
            decimal? mainBundleQuantity = 1;

            if (item.NokiaCPQ_Spare__c == false)
            {
                string itemType = getItemType(item);
                if (item.GetLineType() == LineType.Option &&
                Constants.NOKIA_STANDALONE.equalsIgnoreCase(configType))
                {
                    if (lineItemObjectMap.ContainsKey(Constants.NOKIA_PRODUCT_SERVICES + Constants.NOKIA_UNDERSCORE + item.GetLineNumber()))
                    {
                        mainBundleQuantity = lineItemObjectMap[Constants.NOKIA_PRODUCT_SERVICES + Constants.NOKIA_UNDERSCORE + item.GetLineNumber()].Quantity;
                    }

                    if (item.ExtendedQuantity != null && item.Quantity != 0)
                    {
                        groupQuantity = item.ExtendedQuantity / item.Quantity;
                    }
                }

                if (itemType != null && itemType.equalsIgnoreCase("Software"))
                {
                    if (item.NokiaCPQ_Product_Type__c != null && item.NokiaCPQ_Product_Type__c.equalsIgnoreCase("Controller"))
                    {
                        decimal? basicYear1 = 0;
                        decimal? basicYear2 = 0;
                        decimal? enhanceYear1 = 0;
                        decimal? enhanceYear2 = 0;
                        decimal? enhanceEmergencyYear1 = 0;
                        decimal? enhanceEmergencyYear2 = 0;

                        //Software Maintenance Basic = Controller SW IRP * 25%  
                        //Software Maintenance Enhanced = Software Maintenance Basic Price +25%
                        //Software Maintenance Enhanced Emergency = Software Maintenance Enhanced + 25%
                        //multiplication by no of years

                        if (proposal.NokiaCPQ_No_Of_Years__c != null && proposal.NokiaCPQ_No_Of_Years__c.equalsIgnoreCase("1"))
                        {
                            basicYear1 = pricingHelper.ApplyRounding((item.NokiaCPQ_Extended_IRP__c * 0.25m), 2, RoundingMode.HALF_UP);
                            basicYear2 = 0;
                            enhanceYear1 = pricingHelper.ApplyRounding((basicYear1 + (basicYear1 * 0.25m)), 2, RoundingMode.HALF_UP);
                            enhanceYear2 = 0;
                            enhanceEmergencyYear1 = pricingHelper.ApplyRounding((enhanceYear1 + (enhanceYear1 * 0.25m)), 2, RoundingMode.HALF_UP);
                            enhanceEmergencyYear2 = 0;
                        }

                        if (proposal.NokiaCPQ_No_Of_Years__c != null && (proposal.NokiaCPQ_No_Of_Years__c.equalsIgnoreCase("2") || proposal.NokiaCPQ_No_Of_Years__c.equalsIgnoreCase("3")))
                        {
                            basicYear1 = pricingHelper.ApplyRounding((item.NokiaCPQ_Extended_IRP__c * 0.25m), 2, RoundingMode.HALF_UP);
                            basicYear2 = pricingHelper.ApplyRounding(((basicYear1 * (Convert.ToDecimal(proposal.NokiaCPQ_No_Of_Years__c) * 0.85m)) - basicYear1), 2, RoundingMode.HALF_UP);
                            enhanceYear1 = pricingHelper.ApplyRounding((basicYear1 + (basicYear1 * 0.25m)), 2, RoundingMode.HALF_UP);
                            enhanceYear2 = pricingHelper.ApplyRounding((((basicYear1 + (basicYear1 * 0.25m)) * (Convert.ToDecimal(proposal.NokiaCPQ_No_Of_Years__c) * 0.85m)) - enhanceYear1), 2, RoundingMode.HALF_UP);
                            enhanceEmergencyYear1 = pricingHelper.ApplyRounding((enhanceYear1 + (enhanceYear1 * 0.25m)), 2, RoundingMode.HALF_UP);
                            enhanceEmergencyYear2 = pricingHelper.ApplyRounding((((enhanceYear1 + (enhanceYear1 * 0.25m)) * (Convert.ToDecimal(proposal.NokiaCPQ_No_Of_Years__c) * 0.85m)) - enhanceEmergencyYear1), 2, RoundingMode.HALF_UP);
                        }

                        else if (proposal.NokiaCPQ_No_Of_Years__c != null && (proposal.NokiaCPQ_No_Of_Years__c.equalsIgnoreCase("4") || proposal.NokiaCPQ_No_Of_Years__c.equalsIgnoreCase("5")))
                        {
                            basicYear1 = pricingHelper.ApplyRounding((item.NokiaCPQ_Extended_IRP__c * 0.25m), 2, RoundingMode.HALF_UP);
                            basicYear2 = pricingHelper.ApplyRounding(((basicYear1 * (Convert.ToDecimal(proposal.NokiaCPQ_No_Of_Years__c) * 0.70m)) - basicYear1), 2, RoundingMode.HALF_UP);
                            enhanceYear1 = pricingHelper.ApplyRounding((basicYear1 + (basicYear1 * 0.25m)), 2, RoundingMode.HALF_UP);
                            enhanceYear2 = pricingHelper.ApplyRounding((((basicYear1 + (basicYear1 * 0.25m)) * (Convert.ToDecimal(proposal.NokiaCPQ_No_Of_Years__c) * 0.70m)) - enhanceYear1), 2, RoundingMode.HALF_UP);
                            enhanceEmergencyYear1 = pricingHelper.ApplyRounding((enhanceYear1 + (enhanceYear1 * 0.25m)), 2, RoundingMode.HALF_UP);
                            enhanceEmergencyYear2 = pricingHelper.ApplyRounding((((enhanceYear1 + (enhanceYear1 * 0.25m)) * (Convert.ToDecimal(proposal.NokiaCPQ_No_Of_Years__c) * 0.70m)) - enhanceEmergencyYear1), 2, RoundingMode.HALF_UP);
                        }

                        if (proposal.NokiaCPQ_Maintenance_Type__c != null && proposal.NokiaCPQ_Maintenance_Type__c.equalsIgnoreCase("MN GS TSS Basic"))
                        {
                            item.NokiaCPQ_Maint_Yr1_Extended_Price__c = basicYear1;
                            item.NokiaCPQ_Maint_Yr2_Extended_Price__c = basicYear2;
                        }
                        else if (proposal.NokiaCPQ_Maintenance_Type__c != null && proposal.NokiaCPQ_Maintenance_Type__c.equalsIgnoreCase("MN GS TSS Enhanced"))
                        {
                            item.NokiaCPQ_Maint_Yr1_Extended_Price__c = enhanceYear1;
                            item.NokiaCPQ_Maint_Yr2_Extended_Price__c = enhanceYear2;
                        }
                        else if (proposal.NokiaCPQ_Maintenance_Type__c != null && proposal.NokiaCPQ_Maintenance_Type__c.equalsIgnoreCase("MN GS TSS Enhanced Emergency"))
                        {
                            item.NokiaCPQ_Maint_Yr1_Extended_Price__c = enhanceEmergencyYear1;
                            item.NokiaCPQ_Maint_Yr2_Extended_Price__c = enhanceEmergencyYear2;
                        }
                    }
                }

                else if (itemType != null && itemType.equalsIgnoreCase("Hardware"))
                {
                    if (item.NokiaCPQ_Product_Type__c != null && item.NokiaCPQ_Product_Type__c.equalsIgnoreCase("Access Point"))
                    {
                        //multiplication by no of years
                        if (proposal.NokiaCPQ_No_Of_Years__c != null && proposal.NokiaCPQ_No_Of_Years__c.equalsIgnoreCase("1"))
                        {
                            var extendedIRP = item.NokiaCPQ_Extended_IRP__c;
                            item.NokiaCPQ_Maint_Yr1_Extended_Price__c = pricingHelper.ApplyRounding((extendedIRP * 0.02m) + (extendedIRP * 0.053m), 2, RoundingMode.HALF_UP);
                            item.NokiaCPQ_Maint_Yr2_Extended_Price__c = 0;
                        }

                        else if (proposal.NokiaCPQ_No_Of_Years__c != null && (proposal.NokiaCPQ_No_Of_Years__c.equalsIgnoreCase("2") || proposal.NokiaCPQ_No_Of_Years__c.equalsIgnoreCase("3")))
                        {
                            var extendedIRP = item.NokiaCPQ_Extended_IRP__c;
                            item.NokiaCPQ_Maint_Yr1_Extended_Price__c = pricingHelper.ApplyRounding((extendedIRP * 0.02m) + (extendedIRP * 0.053m), 2, RoundingMode.HALF_UP);
                            item.NokiaCPQ_Maint_Yr2_Extended_Price__c = pricingHelper.ApplyRounding(((extendedIRP * 0.02m * ((Convert.ToDecimal(proposal.NokiaCPQ_No_Of_Years__c) * 1.00m) - 1)) + (extendedIRP * 0.053m * ((Convert.ToDecimal(proposal.NokiaCPQ_No_Of_Years__c) * 0.85m) - 1))), 2, RoundingMode.HALF_UP);
                        }

                        else if (proposal.NokiaCPQ_No_Of_Years__c != null && (proposal.NokiaCPQ_No_Of_Years__c.equalsIgnoreCase("4") || proposal.NokiaCPQ_No_Of_Years__c.equalsIgnoreCase("5")))
                        {
                            var extendedIRP = item.NokiaCPQ_Extended_IRP__c;
                            item.NokiaCPQ_Maint_Yr1_Extended_Price__c = pricingHelper.ApplyRounding(((extendedIRP * 0.02m) + (extendedIRP * 0.053m)), 2, RoundingMode.HALF_UP);
                            item.NokiaCPQ_Maint_Yr2_Extended_Price__c = pricingHelper.ApplyRounding(((extendedIRP * 0.02m * ((Convert.ToDecimal(proposal.NokiaCPQ_No_Of_Years__c) * 0.85m) - 1)) + (extendedIRP * 0.053m * ((Convert.ToDecimal(proposal.NokiaCPQ_No_Of_Years__c) * 0.70m) - 1))), 2, RoundingMode.HALF_UP);
                        }
                    }
                    else if (item.NokiaCPQ_Product_Type__c != null && item.NokiaCPQ_Product_Type__c.equalsIgnoreCase("Controller"))
                    {
                        if (proposal.NokiaCPQ_No_Of_Years__c != null && proposal.NokiaCPQ_No_Of_Years__c.equalsIgnoreCase("1"))
                        {
                            var extendedIRP = item.NokiaCPQ_Extended_IRP__c;
                            item.NokiaCPQ_Maint_Yr1_Extended_Price__c = pricingHelper.ApplyRounding((extendedIRP * 0.02m), 2, RoundingMode.HALF_UP);
                            item.NokiaCPQ_Maint_Yr2_Extended_Price__c = 0;
                        }

                        else if (proposal.NokiaCPQ_No_Of_Years__c != null && (proposal.NokiaCPQ_No_Of_Years__c.equalsIgnoreCase("2") || proposal.NokiaCPQ_No_Of_Years__c.equalsIgnoreCase("3")))
                        {
                            var extendedIRP = item.NokiaCPQ_Extended_IRP__c;
                            item.NokiaCPQ_Maint_Yr1_Extended_Price__c = pricingHelper.ApplyRounding(extendedIRP * 0.02m, 2, RoundingMode.HALF_UP);
                            item.NokiaCPQ_Maint_Yr2_Extended_Price__c = pricingHelper.ApplyRounding(extendedIRP * 0.02m * (Convert.ToDecimal(proposal.NokiaCPQ_No_Of_Years__c) - 1), 2, RoundingMode.HALF_UP);
                        }

                        else if (proposal.NokiaCPQ_No_Of_Years__c != null && (proposal.NokiaCPQ_No_Of_Years__c.equalsIgnoreCase("4") || proposal.NokiaCPQ_No_Of_Years__c.equalsIgnoreCase("5")))
                        {
                            var extendedIRP = item.NokiaCPQ_Extended_IRP__c;
                            item.NokiaCPQ_Maint_Yr1_Extended_Price__c = pricingHelper.ApplyRounding((extendedIRP * 0.02m), 2, RoundingMode.HALF_UP);
                            item.NokiaCPQ_Maint_Yr2_Extended_Price__c = pricingHelper.ApplyRounding(extendedIRP * 0.02m * ((Convert.ToDecimal(proposal.NokiaCPQ_No_Of_Years__c) * 0.85m) - 1), 2, RoundingMode.HALF_UP);
                        }
                    }
                }

                if (item.NokiaCPQ_Product_Type__c != null && item.NokiaCPQ_Product_Type__c.equalsIgnoreCase("Third Party Wavespot"))
                {
                    if (!string.IsNullOrWhiteSpace(partNumber))
                    {
                        var extendedCLP = item.NokiaCPQ_Extended_CLP__c;
                        item.NokiaCPQ_Maint_Yr1_Extended_Price__c = pricingHelper.ApplyRounding((extendedCLP * 0.18m * 0.85m), 2, RoundingMode.HALF_UP);
                        item.NokiaCPQ_Maint_Yr2_Extended_Price__c = pricingHelper.ApplyRounding(extendedCLP * 0.18m * 0.85m * (Convert.ToDecimal(proposal.NokiaCPQ_No_Of_Years__c) - 1), 2, RoundingMode.HALF_UP);
                    }
                }
            }
            else
            {
                item.NokiaCPQ_Maint_Yr1_Extended_Price__c = 0;
                item.NokiaCPQ_Maint_Yr2_Extended_Price__c = 0;
            }

            ExtendedMaintY1Price = ExtendedMaintY1Price + pricingHelper.ApplyRounding((item.NokiaCPQ_Maint_Yr1_Extended_Price__c * groupQuantity * mainBundleQuantity), 2, RoundingMode.HALF_UP);
            ExtendedMaintY2Price = ExtendedMaintY2Price + pricingHelper.ApplyRounding((item.NokiaCPQ_Maint_Yr2_Extended_Price__c * groupQuantity * mainBundleQuantity), 2, RoundingMode.HALF_UP);

            maintenanceValueMap.Add("ExtendedMaintY1Price", ExtendedMaintY1Price);
            maintenanceValueMap.Add("ExtendedMaintY2Price", ExtendedMaintY2Price);

            return maintenanceValueMap;
        }

        private string getItemType(LineItem item)
        {
            string itemType = string.Empty;

            if (item.GetLineType() == LineType.ProductService)
            {
                itemType = item.Apttus_Config2__ProductId__r_NokiaCPQ_Item_Type__c;
            }
            else
            {
                itemType = item.Apttus_Config2__OptionId__r_NokiaCPQ_Item_Type__c;
            }

            return itemType;
        }

        /*  Method Name : careSRSCalculationForNSW
	        Description : The method calculates Care/SRS for NSW quotes
	        */
        List<decimal?> careSRSCalculationForNSW(LineItem item)
        {
            List<decimal?> careSRSList = new List<decimal?>();
            decimal? sumOfCareItemsRTUOEM = 0;
            decimal? sumOfCareItemsOEMSubscription = 0;
            decimal? sumOfRTUForSSP = 0;

            foreach (var optionLineItem in lineItemIRPMapDirect[item.GetLineNumber() + "_" + item.ChargeType])
            {
                string classification = getClassification(optionLineItem);
                string itemType = getItemType(optionLineItem);
                string licenseUsage = getLicenseUsage(optionLineItem);
                if (itemType == "Hardware" && classification == "Base")
                {
                    if (optionLineItem.BasePriceOverride != null && optionLineItem.Quantity != null)
                    {
                        sumOfCareItemsRTUOEM = sumOfCareItemsRTUOEM + pricingHelper.ApplyRounding((optionLineItem.BasePriceOverride * optionLineItem.Quantity), 2, RoundingMode.HALF_UP);
                    }
                    else if (optionLineItem.BasePrice != null && optionLineItem.Quantity != null)
                    {
                        sumOfCareItemsRTUOEM = sumOfCareItemsRTUOEM + pricingHelper.ApplyRounding((optionLineItem.BasePrice * optionLineItem.Quantity), 2, RoundingMode.HALF_UP);
                    }
                }
                else if (((itemType == "Software" || itemType == "Software SI") &&
                         (classification == "Standard SW (STD)" || classification == "High Value SW (HVF)" || classification == "Customer Specific Software (CSS)") &&
                         (licenseUsage == "Commercial Licence" || licenseUsage == "Testbed License" || licenseUsage == "Trial License")) || (itemType == "Service" && classification == "Customisation Services"))
                {

                    if (optionLineItem.BasePriceOverride != null && optionLineItem.Quantity != null)
                    {
                        sumOfCareItemsOEMSubscription = sumOfCareItemsOEMSubscription + pricingHelper.ApplyRounding((optionLineItem.BasePriceOverride * optionLineItem.Quantity), 2, RoundingMode.HALF_UP);
                    }
                    else if (optionLineItem.BasePrice != null && optionLineItem.Quantity != null)
                    {
                        sumOfCareItemsOEMSubscription = sumOfCareItemsOEMSubscription + pricingHelper.ApplyRounding((optionLineItem.BasePrice * optionLineItem.Quantity), 2, RoundingMode.HALF_UP);
                    }
                }
                else if ((itemType == "Software" || itemType == "Software SI") && licenseUsage == "Commercial Term License" && (classification == "Standard SW (STD)" || classification == "High Value SW (HVF)"))
                {
                    if (optionLineItem.BasePriceOverride != null && optionLineItem.Quantity != null)
                    {
                        sumOfRTUForSSP = sumOfRTUForSSP + pricingHelper.ApplyRounding((optionLineItem.BasePriceOverride * optionLineItem.Quantity), 2, RoundingMode.HALF_UP);
                    }
                    else if (optionLineItem.BasePrice != null && optionLineItem.Quantity != null)
                    {
                        sumOfRTUForSSP = sumOfRTUForSSP + pricingHelper.ApplyRounding((optionLineItem.BasePrice * optionLineItem.Quantity), 2, RoundingMode.HALF_UP);
                    }
                }
            }
            decimal? carePrice = (sumOfCareItemsRTUOEM + sumOfCareItemsOEMSubscription + sumOfRTUForSSP * 3);
            decimal? srsPrice = (sumOfCareItemsOEMSubscription + sumOfRTUForSSP * 3);
            careSRSList.Add(carePrice);
            careSRSList.Add(srsPrice);

            return careSRSList;
        }

        private string getClassification(LineItem item)
        {
            string classification = string.Empty;

            if (item.GetLineType() == LineType.ProductService)
            {
                classification = item.Apttus_Config2__ProductId__r_NokiaCPQ_Classification2__c;
            }
            else
            {
                classification = item.Apttus_Config2__OptionId__r_NokiaCPQ_Classification2__c;
            }

            return classification;
        }

        private string getLicenseUsage(LineItem item)
        {
            string licenseUsage = string.Empty;

            if (item.GetLineType() == LineType.ProductService)
            {
                licenseUsage = item.Apttus_Config2__ProductId__r_NokiaCPQ_License_Usage__c;
            }
            else
            {
                licenseUsage = item.Apttus_Config2__OptionId__r_NokiaCPQ_License_Usage__c;
            }

            return licenseUsage;
        }


        private async Task calculateEquivalentPrice()
        {
            var allLineItems = cartLineItems;
            string market = proposal.Account_Market__c;

            Dictionary<string, decimal?> mapcategoryDiscount = new Dictionary<string, decimal?>();
            Dictionary<string, string> mapLineCategory = new Dictionary<string, string>();
            List<string> discountCatgories = new List<string>();
            Dictionary<int, List<LineItem>> mapBundlelineOption = new Dictionary<int, List<LineItem>>();
            Dictionary<decimal?, decimal?> mapBundlelineNContractPrice = new Dictionary<decimal?, decimal?>();
            Dictionary<decimal?, decimal?> mapBundlelineNReferencePrice = new Dictionary<decimal?, decimal?>();
            List<LineItem> nonContractedLines = new List<LineItem>();
            List<ProductDiscountQueryModel> productDisc = new List<ProductDiscountQueryModel>();
            HashSet<string> pricelistItemId = new HashSet<string>();
            HashSet<string> ContractedPriceItems = new HashSet<string>();
            Dictionary<string, decimal?> mapPLIToContractprice = new Dictionary<string, decimal?>();
            decimal? marketdisc = null;

            foreach (var lineitem in cartLineItems)
            {
                pricelistItemId.Add(lineitem.PriceListItemId);
            }

            var priceListItems = cartLineItems.Select(c => c.GetPriceListItem()).Distinct();

            if (priceListItems.Any())
            {
                foreach (var priceItem in priceListItems)
                {
                    if (priceItem.Entity.ContractPrice != null && priceItem.Entity.ContractPrice != 0)
                    {
                        ContractedPriceItems.Add(priceItem.Entity.Id);
                        mapPLIToContractprice.Add(priceItem.Entity.Id, priceItem.Entity.ContractPrice);
                    }
                }
            }

            foreach (var lineitem in cartLineItems)
            {
                String productCatDiscount = getProductDiscountCategory(lineitem);

                if (ContractedPriceItems.Contains(lineitem.PriceListItemId))
                {
                    lineitem.Reference_Price__c = mapPLIToContractprice[lineitem.PriceListItemId];
                }
                else
                {
                    discountCatgories.Add(productCatDiscount);
                    mapLineCategory.Add(lineitem.Id, productCatDiscount);
                    nonContractedLines.Add(lineitem);
                }

                List<LineItem> optionlinelist = new List<LineItem>();

                if (mapBundlelineOption.ContainsKey(lineitem.GetLineNumber()))
                {
                    optionlinelist = mapBundlelineOption[lineitem.GetLineNumber()];
                }
                else
                {
                    mapBundlelineOption.Add(lineitem.GetLineNumber(), optionlinelist);
                }

                optionlinelist.Add(lineitem);
                mapBundlelineOption[lineitem.GetLineNumber()] = optionlinelist;
            }

            productDisc = await QueryHelper.ExecuteProductDiscountQuery(dBHelper, market, discountCatgories);

            foreach (var categorydiscount in productDisc)
            {
                if (categorydiscount.Product_Discount_Category__c == null)
                {
                    marketdisc = categorydiscount.Discount__c;
                }
                mapcategoryDiscount.Add(categorydiscount.Product_Discount_Category__c ?? Constants.NULL_PRODUCT_DISCOUNT_CATEGORY_KEY, categorydiscount.Discount__c);
            }

            foreach (var lineitem in nonContractedLines)
            {
                //GP: Can LineType be Bundle ????
                //if (!(lineitem.Apttus_Config2__LineType__c.equalsIgnoreCase('Bundle')))
                //{
                if (mapLineCategory.ContainsKey(lineitem.Id))
                {
                    if (mapcategoryDiscount.ContainsKey(mapLineCategory[lineitem.Id] ?? Constants.NULL_PRODUCT_DISCOUNT_CATEGORY_KEY))
                    {
                        decimal? discountPerc = mapcategoryDiscount[mapLineCategory[lineitem.Id]];
                        lineitem.Reference_Price__c = (lineitem.ListPrice - (lineitem.ListPrice * (discountPerc / 100)));
                    }
                    else
                    {
                        lineitem.Reference_Price__c = (lineitem.ListPrice - (lineitem.ListPrice * (marketdisc / 100)));
                    }
                }
                else
                {
                    lineitem.Reference_Price__c = lineitem.ListPrice;
                }
                //}
            }

            foreach (var lineitem in nonContractedLines)
            {
                string configType = getConfigType(lineitem);
                decimal? bundleRefPrice = 0;
                if (mapBundlelineOption.ContainsKey(lineitem.GetLineNumber()) && configType.equalsIgnoreCase("Bundle"))
                {
                    foreach (var option in mapBundlelineOption[lineitem.GetLineNumber()])
                    {
                        if (option.GetLineType() == LineType.Option)
                        {
                            bundleRefPrice = bundleRefPrice + (option.Reference_Price__c * option.GetQuantity());
                        }
                    }

                    lineitem.Reference_Price__c = bundleRefPrice;
                    mapBundlelineNReferencePrice.Add(lineitem.GetLineNumber(), lineitem.Reference_Price__c);
                    mapBundlelineNContractPrice.Add(lineitem.GetLineNumber(), mapPLIToContractprice.GetValueOrDefault(lineitem.PriceListItemId));
                }
            }

            foreach (var lineitem in nonContractedLines)
            {
                decimal? bundleRefPrice = 0;
                decimal? BundleContractprice = 0;
                if (mapBundlelineOption.ContainsKey(lineitem.GetLineNumber()) &&
                   lineitem.GetLineType() == LineType.Option)
                {
                    if (mapBundlelineNReferencePrice.ContainsKey(lineitem.GetLineNumber()))
                    {
                        bundleRefPrice = mapBundlelineNReferencePrice[lineitem.GetLineNumber()];
                    }
                    if (mapBundlelineNContractPrice.ContainsKey(lineitem.GetLineNumber()))
                    {
                        BundleContractprice = mapBundlelineNContractPrice[lineitem.GetLineNumber()];
                    }

                    lineitem.Equivalent_Price__c = (lineitem.Reference_Price__c / bundleRefPrice) * BundleContractprice;
                }
            }
        }

        private void deal_Guidance_Calculator(LineItem item, string configType, decimal? pricingGuidanceSettingThresold)
        {
            //system.debug('In Deal guidance');
            //CPQ Requirement : Traffic Light calculations For MN Direct
            if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING) && !configType.equalsIgnoreCase(Constants.BUNDLE))
            {
                bool? contractedPL = getCLP(item);
                bool? isOEM = getOEM(item);
                decimal? maxIRPDiscount = getMaximumIRPDiscount(item);
                string itemType = getItemType(item);

                if (item.ChargeType != Constants.STANDARD_PRICE && item.GetLineType() == LineType.ProductService)
                {
                    if (item.NetAdjustmentPercent < 0 || (contractedPL == false && item.NokiaCPQ_Extended_IRP__c != item.NokiaCPQ_Extended_CLP__c))
                    {
                        item.NokiaCPQ_Light_Color__c = Constants.RED;
                    }
                    else
                    {
                        item.NokiaCPQ_Light_Color__c = Constants.GREEN;
                    }
                }
                else
                {
                    if (contractedPL == false &&
                    ((isOEM == false && (itemType == Constants.HARDWARE_STRING || itemType == Constants.SOFTWARE_STRING)) ||
                        (isOEM == true && (itemType == Constants.HARDWARE_STRING || itemType == Constants.SOFTWARE_STRING || itemType == Constants.SERVICE_STRING))))
                    {
                        if (item.NokiaCPQ_IRP_Discount__c <= maxIRPDiscount)
                        {
                            item.NokiaCPQ_Light_Color__c = Constants.GREEN;
                        }
                        else
                        {
                            item.NokiaCPQ_Light_Color__c = Constants.RED;
                        }
                    }

                    if (contractedPL == true &&
                    ((isOEM == false && (itemType == Constants.HARDWARE_STRING || itemType == Constants.SOFTWARE_STRING)) ||
                        (isOEM == true && (itemType == Constants.HARDWARE_STRING || itemType == Constants.SOFTWARE_STRING || itemType == Constants.SERVICE_STRING))))
                    {
                        if (item.NetAdjustmentPercent != 0)
                        {
                            item.NokiaCPQ_Light_Color__c = Constants.RED;
                        }
                        else
                        {
                            item.NokiaCPQ_Light_Color__c = Constants.GREEN;
                        }
                    }

                    if (isOEM == false && itemType == Constants.SERVICE_STRING)
                    {
                        if (item.NetAdjustmentPercent < 0 || item.NokiaCPQ_ExtendedPrice_CNP__c == 0 ||
                     (contractedPL == false && item.NokiaCPQ_Extended_IRP__c != item.NokiaCPQ_Extended_CLP__c))
                        {
                            item.NokiaCPQ_Light_Color__c = Constants.RED;
                        }
                        else
                            item.NokiaCPQ_Light_Color__c = Constants.GREEN;
                    }
                }
            }
            //CPQ Requirement : Traffic Light calculations For Direct NSW
            else if (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_SOFTWARE) && !configType.equalsIgnoreCase(Constants.BUNDLE))
            {
                bool? contractedPL = getCLP(item);
                decimal? maxIRPDiscount = getMaximumIRPDiscount(item);
                decimal? minSalesMargin = getMinimumSalesMargin(item);

                if (item.NokiaCPQ_Is_Direct_Option__c == true && ((item.NetAdjustmentPercent != 0 && item.NetAdjustmentPercent != null) ||
                    ((item.NetAdjustmentPercent == null || item.NetAdjustmentPercent == 0) && contractedPL == false)) && item.Item_Type_From_CAT__c == "PS")
                {
                    if (item.Sales_Margin__c >= minSalesMargin)
                    {
                        item.NokiaCPQ_Light_Color__c = Constants.GREEN;
                    }
                    else
                    {
                        item.NokiaCPQ_Light_Color__c = Constants.RED;
                    }
                }

                if ((item.NetAdjustmentPercent == 0 || item.NetAdjustmentPercent == null) && (item.NokiaCPQ_Is_Direct_Option__c == true && contractedPL == true))
                {
                    item.NokiaCPQ_Light_Color__c = Constants.GREEN;
                }

                if (item.NokiaCPQ_Is_Direct_Option__c == true && ((item.NetAdjustmentPercent != null && item.NetAdjustmentPercent != 0) ||
                    ((item.NetAdjustmentPercent == 0 || item.NetAdjustmentPercent == null) && contractedPL == false)) && item.Item_Type_From_CAT__c != "PS")
                {
                    if (item.NokiaCPQ_IRP_Discount__c <= maxIRPDiscount)
                    {
                        item.NokiaCPQ_Light_Color__c = Constants.GREEN;
                    }
                    else
                    {
                        item.NokiaCPQ_Light_Color__c = Constants.RED;
                    }
                }
            }
            //Traffic Light calculations For QTC
            else if (proposal.NokiaCPQ_Portfolio__c != null && proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase("QTC"))
            {
                //system.debug('Entered guidance for QTC');
                if (item.NokiaCPQ_Floor_Price__c == null)
                {
                    item.NokiaCPQ_Light_Color__c = Constants.RED;
                }
                else if ((item.NetPrice < (item.Quantity * item.NokiaCPQ_Floor_Price__c)) || item.NokiaCPQ_Custom_Bid__c == true)
                {
                    item.NokiaCPQ_Light_Color__c = Constants.RED;
                }
                else if ((item.NetPrice >= (item.Quantity * item.NokiaCPQ_Floor_Price__c)) &&
                       (item.NetPrice < (item.Quantity * item.NokiaCPQ_Floor_Price__c * ((100 + pricingGuidanceSettingThresold) / 100))))
                {
                    item.NokiaCPQ_Light_Color__c = Constants.YELLOW;
                }
                else
                {
                    item.NokiaCPQ_Light_Color__c = Constants.GREEN;
                }
            }
            //Traffic Light calculations For Enterprise
            else if (proposal.NokiaCPQ_Portfolio__c != null &&
        (proposal.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Constants.NOKIA_IP_ROUTING)
        && proposal.Is_List_Price_Only__c == false && !configType.equalsIgnoreCase(Constants.BUNDLE)
        && item.ChargeType == Constants.STANDARD_PRICE
        && item.Is_Custom_Product__c == false))
            {
                //system.debug('Entered guidance for Enterprise');
                bool? contractedPL = getCLP(item);
                if (contractedPL == true && item.NetAdjustmentPercent == 0)
                {
                    item.NokiaCPQ_Light_Color__c = Constants.GREEN;
                }
                else
                {
                    if (item.NokiaCPQ_Floor_Price__c == null || item.NokiaCPQ_Custom_Bid__c == true)
                    {
                        item.NokiaCPQ_Light_Color__c = Constants.RED;
                    }
                    else if (item.NetPrice < (item.Quantity * item.NokiaCPQ_Floor_Price__c))
                    {
                        item.NokiaCPQ_Light_Color__c = Constants.RED;
                    }
                    else if ((item.NetPrice >= (item.Quantity * item.NokiaCPQ_Floor_Price__c)) &&
                (item.NetPrice < (item.Quantity * item.NokiaCPQ_Floor_Price__c * ((100 + pricingGuidanceSettingThresold) / 100))))
                    {
                        item.NokiaCPQ_Light_Color__c = Constants.YELLOW;
                    }
                    else
                    {
                        item.NokiaCPQ_Light_Color__c = Constants.GREEN;
                    }
                }
            }
        }

        private bool? getCLP(LineItem item)
        {
            bool? clp = false;
            PriceListItemModel priceListItem = item.GetPriceListItem();

            if (item.PriceListId != priceListItem.Entity.PriceListId &&
                    item.Apttus_Config2__PriceListItemId__r_Contracted__c == Constants.YES_STRING)
            {
                clp = true;
            }
            else
            {
                clp = false;
            }

            return clp;
        }

        /*Start: Methods for replacing usage of formula fields for improving performance*/
        private bool? getOEM(LineItem item)
        {
            bool? oem = false;
            if (item.GetLineType() == LineType.ProductService)
            {
                oem = item.Apttus_Config2__ProductId__r_NokiaCPQ_OEM__c;
            }
            else
            {
                oem = item.Apttus_Config2__OptionId__r_NokiaCPQ_OEM__c;
            }
            return oem;
        }

        private decimal? getMaximumIRPDiscount(LineItem item)
        {
            decimal? maximumIRPDiscount = 0;
            if (item.GetLineType() == LineType.ProductService)
            {
                maximumIRPDiscount = item.Apttus_Config2__ProductId__r_NokiaCPQ_Maximum_IRP_Discount__c;
            }
            else
            {
                maximumIRPDiscount = item.Apttus_Config2__OptionId__r_NokiaCPQ_Maximum_IRP_Discount__c;
            }

            if (maximumIRPDiscount == null)
            {
                maximumIRPDiscount = 0;
            }

            return maximumIRPDiscount;
        }

        private decimal? getMinimumSalesMargin(LineItem item)
        {
            decimal? salesMargin = 0;
            if (item.NokiaCPQ_Account_Region__c == "RG_NAM")
                salesMargin = item.Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_North_America__c;
            else if (item.NokiaCPQ_Account_Region__c == "RG_LAM")
                salesMargin = item.Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Latin_America__c;
            else if (item.NokiaCPQ_Account_Region__c == "RG_MEA")
                salesMargin = item.Apttus_Config2__OptionId__r_NokiaCPQMin_SM_Middle_East_and_Africa__c;
            else if (item.NokiaCPQ_Account_Region__c == "RG_ASIA")
            {
                if (proposal.Apttus_Proposal__Account__r_CountryCode__c == "IN" || proposal.Apttus_Proposal__Account__r_CountryCode__c == "BT" || proposal.Apttus_Proposal__Account__r_CountryCode__c == "NP")
                {
                    salesMargin = item.Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_India__c;
                }
                else
                {
                    salesMargin = item.Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Asia_Pacific_Japan__c;
                }
            }
            else if (item.NokiaCPQ_Account_Region__c == "RG_CHINA")
                salesMargin = item.Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Greater_China__c;
            else if (item.NokiaCPQ_Account_Region__c == "RG_EUROPE")
                salesMargin = item.Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Europe__c;
            else
                salesMargin = 0;

            if (salesMargin == null)
            {
                salesMargin = 0;
            }

            return salesMargin;
        }
    }
}

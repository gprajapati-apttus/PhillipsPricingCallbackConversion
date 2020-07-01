using Apttus.Lightsaber.Extensibility.Framework.Library.Extension;
using Apttus.Lightsaber.Pricing.Common.Constants;
using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Pricing.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Phillips.Totalling
{
    public class LineItem
    {
        private readonly LineItemModel lineItemModel;

        public LineItem(LineItemModel lineItemModel)
        {
            this.lineItemModel = lineItemModel;
        }

        public string Id { get { return lineItemModel.Entity.Id; } set { lineItemModel.Entity.Id = value; } }
        public string Name { get { return lineItemModel.Entity.Name; } set { lineItemModel.Entity.Name = value; } }
        public string CurrencyIsoCode { get { return lineItemModel.Entity.CurrencyIsoCode; } set { lineItemModel.Entity.CurrencyIsoCode = value; } }
        public string AdHocGroupId { get { return lineItemModel.Entity.AdHocGroupId; } set { lineItemModel.Entity.AdHocGroupId = value; } }
        public decimal? AdjustedPrice { get { return lineItemModel.Entity.AdjustedPrice; } set { lineItemModel.Entity.AdjustedPrice = value; } }
        public decimal? AdjustmentAmount { get { return lineItemModel.Entity.AdjustmentAmount; } set { lineItemModel.Entity.AdjustmentAmount = value; } }
        public string AdjustmentType { get { return lineItemModel.Entity.AdjustmentType; } set { lineItemModel.Entity.AdjustmentType = value; } }
        public bool? AllocateGroupAdjustment { get { return lineItemModel.Entity.AllocateGroupAdjustment; } set { lineItemModel.Entity.AllocateGroupAdjustment = value; } }
        public string AllowableAction { get { return lineItemModel.Entity.AllowableAction; } set { lineItemModel.Entity.AllowableAction = value; } }
        public bool? AllowManualAdjustment { get { return lineItemModel.Entity.AllowManualAdjustment; } set { lineItemModel.Entity.AllowManualAdjustment = value; } }
        public bool? AllowProration { get { return lineItemModel.Entity.AllowProration; } set { lineItemModel.Entity.AllowProration = value; } }
        public string ApprovalStatus { get { return lineItemModel.Entity.ApprovalStatus; } set { lineItemModel.Entity.ApprovalStatus = value; } }
        public string AssetLineItemId { get { return lineItemModel.Entity.AssetLineItemId; } set { lineItemModel.Entity.AssetLineItemId = value; } }
        public decimal? AssetQuantity { get { return lineItemModel.Entity.AssetQuantity; } set { lineItemModel.Entity.AssetQuantity = value; } }
        public string AttributeValueId { get { return lineItemModel.Entity.AttributeValueId; } set { lineItemModel.Entity.AttributeValueId = value; } }
        public bool? AutoRenew { get { return lineItemModel.Entity.AutoRenew; } set { lineItemModel.Entity.AutoRenew = value; } }
        public int? AutoRenewalTerm { get { return lineItemModel.Entity.AutoRenewalTerm; } set { lineItemModel.Entity.AutoRenewalTerm = value; } }
        public string AutoRenewalType { get { return lineItemModel.Entity.AutoRenewalType; } set { lineItemModel.Entity.AutoRenewalType = value; } }
        public decimal? BaseCost { get { return lineItemModel.Entity.BaseCost; } set { lineItemModel.Entity.BaseCost = value; } }
        public decimal? BaseCostOverride { get { return lineItemModel.Entity.BaseCostOverride; } set { lineItemModel.Entity.BaseCostOverride = value; } }
        public decimal? BaseExtendedCost { get { return lineItemModel.Entity.BaseExtendedCost; } set { lineItemModel.Entity.BaseExtendedCost = value; } }
        public decimal? BaseExtendedPrice { get { return lineItemModel.Entity.BaseExtendedPrice; } set { lineItemModel.Entity.BaseExtendedPrice = value; } }
        public decimal? BasePrice { get { return lineItemModel.Entity.BasePrice; } set { lineItemModel.Entity.BasePrice = value; } }
        public decimal? BasePriceOverride { get { return lineItemModel.Entity.BasePriceOverride; } set { lineItemModel.Entity.BasePriceOverride = value; } }
        public string BasePriceMethod { get { return lineItemModel.Entity.BasePriceMethod; } set { lineItemModel.Entity.BasePriceMethod = value; } }
        public string BillingFrequency { get { return lineItemModel.Entity.BillingFrequency; } set { lineItemModel.Entity.BillingFrequency = value; } }
        public string BillingPreferenceId { get { return lineItemModel.Entity.BillingPreferenceId; } set { lineItemModel.Entity.BillingPreferenceId = value; } }
        public string BillingRule { get { return lineItemModel.Entity.BillingRule; } set { lineItemModel.Entity.BillingRule = value; } }
        public string BillToAccountId { get { return lineItemModel.Entity.BillToAccountId; } set { lineItemModel.Entity.BillToAccountId = value; } }
        public string ChargeType { get { return lineItemModel.Entity.ChargeType; } set { lineItemModel.Entity.ChargeType = value; } }
        public string ClassificationHierarchy { get { return lineItemModel.Entity.ClassificationHierarchy; } set { lineItemModel.Entity.ClassificationHierarchy = value; } }
        public string ClassificationHierarchyInfo { get { return lineItemModel.Entity.ClassificationHierarchyInfo; } set { lineItemModel.Entity.ClassificationHierarchyInfo = value; } }
        public string ClassificationId { get { return lineItemModel.Entity.ClassificationId; } set { lineItemModel.Entity.ClassificationId = value; } }
        public string Comments { get { return lineItemModel.Entity.Comments; } set { lineItemModel.Entity.Comments = value; } }
        public string ConfigurationId { get { return lineItemModel.Entity.ConfigurationId; } set { lineItemModel.Entity.ConfigurationId = value; } }
        public string ContractNumbers { get { return lineItemModel.Entity.ContractNumbers; } set { lineItemModel.Entity.ContractNumbers = value; } }
        public decimal? Cost { get { return lineItemModel.Entity.Cost; } set { lineItemModel.Entity.Cost = value; } }
        public string CouponCode { get { return lineItemModel.Entity.CouponCode; } set { lineItemModel.Entity.CouponCode = value; } }
        public bool? Customizable { get { return lineItemModel.Entity.Customizable; } set { lineItemModel.Entity.Customizable = value; } }
        public decimal? DeltaPrice { get { return lineItemModel.Entity.DeltaPrice; } set { lineItemModel.Entity.DeltaPrice = value; } }
        public decimal? DeltaQuantity { get { return lineItemModel.Entity.DeltaQuantity; } set { lineItemModel.Entity.DeltaQuantity = value; } }
        public DateTime? EndDate { get { return lineItemModel.Entity.EndDate; } set { lineItemModel.Entity.EndDate = value; } }
        public decimal? ExtendedCost { get { return lineItemModel.Entity.ExtendedCost; } set { lineItemModel.Entity.ExtendedCost = value; } }
        public decimal? ExtendedPrice { get { return lineItemModel.Entity.ExtendedPrice; } set { lineItemModel.Entity.ExtendedPrice = value; } }
        public decimal? ExtendedQuantity { get { return lineItemModel.Entity.ExtendedQuantity; } set { lineItemModel.Entity.ExtendedQuantity = value; } }
        public decimal? FlatOptionPrice { get { return lineItemModel.Entity.FlatOptionPrice; } set { lineItemModel.Entity.FlatOptionPrice = value; } }
        public string Frequency { get { return lineItemModel.Entity.Frequency; } set { lineItemModel.Entity.Frequency = value; } }
        public decimal? GroupAdjustmentPercent { get { return lineItemModel.Entity.GroupAdjustmentPercent; } set { lineItemModel.Entity.GroupAdjustmentPercent = value; } }
        public string Guidance { get { return lineItemModel.Entity.Guidance; } set { lineItemModel.Entity.Guidance = value; } }
        public bool? HasAttributes { get { return lineItemModel.Entity.HasAttributes; } set { lineItemModel.Entity.HasAttributes = value; } }
        public bool? HasBaseProduct { get { return lineItemModel.Entity.HasBaseProduct; } set { lineItemModel.Entity.HasBaseProduct = value; } }
        public bool? HasDefaults { get { return lineItemModel.Entity.HasDefaults; } set { lineItemModel.Entity.HasDefaults = value; } }
        public bool HasIncentives { get { return lineItemModel.Entity.HasIncentives; } set { lineItemModel.Entity.HasIncentives = value; } }
        public bool? HasOptions { get { return lineItemModel.Entity.HasOptions; } set { lineItemModel.Entity.HasOptions = value; } }
        public bool? HasTieredPrice { get { return lineItemModel.Entity.HasTieredPrice; } set { lineItemModel.Entity.HasTieredPrice = value; } }
        public string IncentiveId { get { return lineItemModel.Entity.IncentiveId; } set { lineItemModel.Entity.IncentiveId = value; } }
        public decimal? IncentiveAdjustmentAmount { get { return lineItemModel.Entity.IncentiveAdjustmentAmount; } set { lineItemModel.Entity.IncentiveAdjustmentAmount = value; } }
        public decimal? IncentiveBasePrice { get { return lineItemModel.Entity.IncentiveBasePrice; } set { lineItemModel.Entity.IncentiveBasePrice = value; } }
        public string IncentiveCode { get { return lineItemModel.Entity.IncentiveCode; } set { lineItemModel.Entity.IncentiveCode = value; } }
        public decimal? IncentiveExtendedPrice { get { return lineItemModel.Entity.IncentiveExtendedPrice; } set { lineItemModel.Entity.IncentiveExtendedPrice = value; } }
        public string IncentiveType { get { return lineItemModel.Entity.IncentiveType; } set { lineItemModel.Entity.IncentiveType = value; } }
        public bool? IsAssetPricing { get { return lineItemModel.Entity.IsAssetPricing; } set { lineItemModel.Entity.IsAssetPricing = value; } }
        public bool? IsCustomPricing { get { return lineItemModel.Entity.IsCustomPricing; } set { lineItemModel.Entity.IsCustomPricing = value; } }
        public bool? IsOptional { get { return lineItemModel.Entity.IsOptional; } set { lineItemModel.Entity.IsOptional = value; } }
        public bool? IsOptionRollupLine { get { return lineItemModel.Entity.IsOptionRollupLine; } set { lineItemModel.Entity.IsOptionRollupLine = value; } }
        public bool? IsPrimaryLine { get { return lineItemModel.Entity.IsPrimaryLine; } set { lineItemModel.Entity.IsPrimaryLine = value; } }
        public bool? IsPrimaryRampLine { get { return lineItemModel.Entity.IsPrimaryRampLine; } set { lineItemModel.Entity.IsPrimaryRampLine = value; } }
        public bool? IsQuantityModifiable { get { return lineItemModel.Entity.IsQuantityModifiable; } set { lineItemModel.Entity.IsQuantityModifiable = value; } }
        public bool? IsSellingTermReadOnly { get { return lineItemModel.Entity.IsSellingTermReadOnly; } set { lineItemModel.Entity.IsSellingTermReadOnly = value; } }
        public bool? IsUsageTierModifiable { get { return lineItemModel.Entity.IsUsageTierModifiable; } set { lineItemModel.Entity.IsUsageTierModifiable = value; } }
        public int ItemSequence { get { return lineItemModel.Entity.ItemSequence; } set { lineItemModel.Entity.ItemSequence = value; } }
        public int LineNumber { get { return lineItemModel.Entity.LineNumber; } set { lineItemModel.Entity.LineNumber = value; } }
        public int? LineSequence { get { return lineItemModel.Entity.LineSequence; } set { lineItemModel.Entity.LineSequence = value; } }
        public string LineStatus { get { return lineItemModel.Entity.LineStatus; } set { lineItemModel.Entity.LineStatus = value; } }
        public string LineType { get { return lineItemModel.Entity.LineType; } set { lineItemModel.Entity.LineType = value; } }
        public decimal? ListPrice { get { return lineItemModel.Entity.ListPrice; } set { lineItemModel.Entity.ListPrice = value; } }
        public decimal? MaxPrice { get { return lineItemModel.Entity.MaxPrice; } set { lineItemModel.Entity.MaxPrice = value; } }
        public decimal? MaxUsageQuantity { get { return lineItemModel.Entity.MaxUsageQuantity; } set { lineItemModel.Entity.MaxUsageQuantity = value; } }
        public string MinMaxPriceAppliesTo { get { return lineItemModel.Entity.MinMaxPriceAppliesTo; } set { lineItemModel.Entity.MinMaxPriceAppliesTo = value; } }
        public decimal? MinPrice { get { return lineItemModel.Entity.MinPrice; } set { lineItemModel.Entity.MinPrice = value; } }
        public decimal? MinUsageQuantity { get { return lineItemModel.Entity.MinUsageQuantity; } set { lineItemModel.Entity.MinUsageQuantity = value; } }
        public decimal? NetAdjustmentPercent { get { return lineItemModel.Entity.NetAdjustmentPercent; } set { lineItemModel.Entity.NetAdjustmentPercent = value; } }
        public decimal? NetPrice { get { return lineItemModel.Entity.NetPrice; } set { lineItemModel.Entity.NetPrice = value; } }
        public decimal? NetUnitPrice { get { return lineItemModel.Entity.NetUnitPrice; } set { lineItemModel.Entity.NetUnitPrice = value; } }
        public string OptionId { get { return lineItemModel.Entity.OptionId; } set { lineItemModel.Entity.OptionId = value; } }
        public decimal? OptionCost { get { return lineItemModel.Entity.OptionCost; } set { lineItemModel.Entity.OptionCost = value; } }
        public decimal? OptionPrice { get { return lineItemModel.Entity.OptionPrice; } set { lineItemModel.Entity.OptionPrice = value; } }
        public int? OptionSequence { get { return lineItemModel.Entity.OptionSequence; } set { lineItemModel.Entity.OptionSequence = value; } }
        public int? ParentBundleNumber { get { return lineItemModel.Entity.ParentBundleNumber; } set { lineItemModel.Entity.ParentBundleNumber = value; } }
        public string PaymentTermId { get { return lineItemModel.Entity.PaymentTermId; } set { lineItemModel.Entity.PaymentTermId = value; } }
        public decimal? PriceAdjustment { get { return lineItemModel.Entity.PriceAdjustment; } set { lineItemModel.Entity.PriceAdjustment = value; } }
        public decimal? PriceAdjustmentAmount { get { return lineItemModel.Entity.PriceAdjustmentAmount; } set { lineItemModel.Entity.PriceAdjustmentAmount = value; } }
        public string PriceAdjustmentAppliesTo { get { return lineItemModel.Entity.PriceAdjustmentAppliesTo; } set { lineItemModel.Entity.PriceAdjustmentAppliesTo = value; } }
        public string PriceAdjustmentType { get { return lineItemModel.Entity.PriceAdjustmentType; } set { lineItemModel.Entity.PriceAdjustmentType = value; } }
        public string PriceGroup { get { return lineItemModel.Entity.PriceGroup; } set { lineItemModel.Entity.PriceGroup = value; } }
        public bool? PriceIncludedInBundle { get { return lineItemModel.Entity.PriceIncludedInBundle; } set { lineItemModel.Entity.PriceIncludedInBundle = value; } }
        public string PriceListId { get { return lineItemModel.Entity.PriceListId; } set { lineItemModel.Entity.PriceListId = value; } }
        public string PriceListItemId { get { return lineItemModel.Entity.PriceListItemId; } set { lineItemModel.Entity.PriceListItemId = value; } }
        public string PriceMethod { get { return lineItemModel.Entity.PriceMethod; } set { lineItemModel.Entity.PriceMethod = value; } }
        public string PriceType { get { return lineItemModel.Entity.PriceType; } set { lineItemModel.Entity.PriceType = value; } }
        public string PriceUom { get { return lineItemModel.Entity.PriceUom; } set { lineItemModel.Entity.PriceUom = value; } }
        public DateTime? PricingDate { get { return lineItemModel.Entity.PricingDate; } set { lineItemModel.Entity.PricingDate = value; } }
        public string PricingGuidance { get { return lineItemModel.Entity.PricingGuidance; } set { lineItemModel.Entity.PricingGuidance = value; } }
        public string PricingStatus { get { return lineItemModel.Entity.PricingStatus; } set { lineItemModel.Entity.PricingStatus = value; } }
        public string PricingSteps { get { return lineItemModel.Entity.PricingSteps; } set { lineItemModel.Entity.PricingSteps = value; } }
        public string LocationId { get { return lineItemModel.Entity.LocationId; } set { lineItemModel.Entity.LocationId = value; } }
        public int PrimaryLineNumber { get { return lineItemModel.Entity.PrimaryLineNumber; } set { lineItemModel.Entity.PrimaryLineNumber = value; } }
        public string ProductId { get { return lineItemModel.Entity.ProductId; } set { lineItemModel.Entity.ProductId = value; } }
        public string ProductOptionId { get { return lineItemModel.Entity.ProductOptionId; } set { lineItemModel.Entity.ProductOptionId = value; } }
        public decimal? RelatedAdjustmentAmount { get { return lineItemModel.Entity.RelatedAdjustmentAmount; } set { lineItemModel.Entity.RelatedAdjustmentAmount = value; } }
        public string RelatedAdjustmentAppliesTo { get { return lineItemModel.Entity.RelatedAdjustmentAppliesTo; } set { lineItemModel.Entity.RelatedAdjustmentAppliesTo = value; } }
        public string RelatedAdjustmentType { get { return lineItemModel.Entity.RelatedAdjustmentType; } set { lineItemModel.Entity.RelatedAdjustmentType = value; } }
        public string RelatedItemId { get { return lineItemModel.Entity.RelatedItemId; } set { lineItemModel.Entity.RelatedItemId = value; } }
        public decimal? RelatedPercent { get { return lineItemModel.Entity.RelatedPercent; } set { lineItemModel.Entity.RelatedPercent = value; } }
        public string RelatedPercentAppliesTo { get { return lineItemModel.Entity.RelatedPercentAppliesTo; } set { lineItemModel.Entity.RelatedPercentAppliesTo = value; } }
        public decimal? Quantity { get { return lineItemModel.Entity.Quantity; } set { lineItemModel.Entity.Quantity = value; } }
        public decimal? RenewalAdjustmentAmount { get { return lineItemModel.Entity.RenewalAdjustmentAmount; } set { lineItemModel.Entity.RenewalAdjustmentAmount = value; } }
        public string RenewalAdjustmentType { get { return lineItemModel.Entity.RenewalAdjustmentType; } set { lineItemModel.Entity.RenewalAdjustmentType = value; } }
        public string RollupPriceMethod { get { return lineItemModel.Entity.RollupPriceMethod; } set { lineItemModel.Entity.RollupPriceMethod = value; } }
        public bool? RollupPriceToBundle { get { return lineItemModel.Entity.RollupPriceToBundle; } set { lineItemModel.Entity.RollupPriceToBundle = value; } }
        public string ShipToAccountId { get { return lineItemModel.Entity.ShipToAccountId; } set { lineItemModel.Entity.ShipToAccountId = value; } }
        public DateTime? StartDate { get { return lineItemModel.Entity.StartDate; } set { lineItemModel.Entity.StartDate = value; } }
        public string SellingFrequency { get { return lineItemModel.Entity.SellingFrequency; } set { lineItemModel.Entity.SellingFrequency = value; } }
        public decimal? SellingTerm { get { return lineItemModel.Entity.SellingTerm; } set { lineItemModel.Entity.SellingTerm = value; } }
        public string SellingUom { get { return lineItemModel.Entity.SellingUom; } set { lineItemModel.Entity.SellingUom = value; } }
        public string SummaryGroupId { get { return lineItemModel.Entity.SummaryGroupId; } set { lineItemModel.Entity.SummaryGroupId = value; } }
        public bool? Taxable { get { return lineItemModel.Entity.Taxable; } set { lineItemModel.Entity.Taxable = value; } }
        public string TaxCodeId { get { return lineItemModel.Entity.TaxCodeId; } set { lineItemModel.Entity.TaxCodeId = value; } }
        public bool? TaxInclusive { get { return lineItemModel.Entity.TaxInclusive; } set { lineItemModel.Entity.TaxInclusive = value; } }
        public decimal? Term { get { return lineItemModel.Entity.Term; } set { lineItemModel.Entity.Term = value; } }
        public string TransferPriceLineItemId { get { return lineItemModel.Entity.TransferPriceLineItemId; } set { lineItemModel.Entity.TransferPriceLineItemId = value; } }
        public decimal? TotalQuantity { get { return lineItemModel.Entity.TotalQuantity; } set { lineItemModel.Entity.TotalQuantity = value; } }
        public decimal? UnitCostAdjustment { get { return lineItemModel.Entity.UnitCostAdjustment; } set { lineItemModel.Entity.UnitCostAdjustment = value; } }
        public decimal? UnitPriceAdjustmentAuto { get { return lineItemModel.Entity.UnitPriceAdjustmentAuto; } set { lineItemModel.Entity.UnitPriceAdjustmentAuto = value; } }
        public decimal? UnitPriceAdjustmentManual { get { return lineItemModel.Entity.UnitPriceAdjustmentManual; } set { lineItemModel.Entity.UnitPriceAdjustmentManual = value; } }
        public string Uom { get { return lineItemModel.Entity.Uom; } set { lineItemModel.Entity.Uom = value; } }

        public PriceListItemModel GetPriceListItem()
        {
            return lineItemModel.GetPriceListItem();
        }

        public decimal GetQuantity()
        {
            return lineItemModel.GetQuantity();
        }

        public bool IsOptionLine()
        {
            return lineItemModel.IsOptionLine();
        }

        public ProductLineItemModel GetRootParentLineItem()
        {
            return lineItemModel.GetRootParentLineItem();
        }

        public string GetProductOrOptionId()
        {
            return lineItemModel.GetProductOrOptionId();
        }

        public int GetLineNumber()
        {
            return lineItemModel.GetLineNumber();
        }

        public decimal GetValuetOrDefault(string fieldName, decimal defaultValue)
        {
            return lineItemModel.GetValuetOrDefault(fieldName, defaultValue);
        }

        public LineType GetLineType()
        {
            return lineItemModel.GetLineType();
        }

        #region Custom & Not available Standard LineItem Fields

        public string Apttus_Config2__Description__c { get { return lineItemModel.Get<string>(LineItemStandardField.Apttus_Config2__Description__c); } set { lineItemModel.Set(LineItemStandardField.Apttus_Config2__Description__c, value); } }

        public string APTS_Local_Bundle_Header__c
        {
            get
            {
                return lineItemModel.Get<string>(LineItemCustomField.APTS_Local_Bundle_Header__c);
            }
            set
            {
                lineItemModel.Set(LineItemCustomField.APTS_Local_Bundle_Header__c, value);
            }
        }

        public string APTS_Local_Bundle_Component__c
        {
            get
            {
                return lineItemModel.Get<string>(LineItemCustomField.APTS_Local_Bundle_Component__c);
            }
            set
            {
                lineItemModel.Set(LineItemCustomField.APTS_Local_Bundle_Component__c, value);
            }
        }

        public decimal? APTS_Extended_Quantity__c
        {
            get
            {
                return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Extended_Quantity__c);
            }
            set
            {
                lineItemModel.Set(LineItemCustomField.APTS_Extended_Quantity__c, value);
            }
        }

        public decimal? APTS_Bundle_Quantity__c
        {
            get
            {
                return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Bundle_Quantity__c);
            }
            set
            {
                lineItemModel.Set(LineItemCustomField.APTS_Bundle_Quantity__c, value);
            }
        }

        public decimal? APTS_Extended_List_Price__c
        {
            get
            {
                return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c);
            }
            set
            {
                lineItemModel.Set(LineItemCustomField.APTS_Extended_List_Price__c, value);
            }
        }

        public decimal? APTS_Option_Unit_Price__c
        {
            get
            {
                return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Option_Unit_Price__c);
            }
            set
            {
                lineItemModel.Set(LineItemCustomField.APTS_Option_Unit_Price__c, value);
            }
        }

        public decimal? APTS_Target_Price_SPOO__c
        {
            get
            {
                return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Target_Price_SPOO__c);
            }
            set
            {
                lineItemModel.Set(LineItemCustomField.APTS_Target_Price_SPOO__c, value);
            }
        }

        public decimal? APTS_Escalation_Price_SPOO__c
        {
            get
            {
                return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Escalation_Price_SPOO__c);
            }
            set
            {
                lineItemModel.Set(LineItemCustomField.APTS_Escalation_Price_SPOO__c, value);
            }
        }

        public string APTS_Product_ID_SPOO__c
        {
            get
            {
                return lineItemModel.Get<string>(LineItemCustomField.APTS_Product_ID_SPOO__c);
            }
            set
            {
                lineItemModel.Set(LineItemCustomField.APTS_Product_ID_SPOO__c, value);
            }
        }

        public string APTS_System_Type__c
        {
            get
            {
                return lineItemModel.Get<string>(LineItemCustomField.APTS_System_Type__c);
            }
            set
            {
                lineItemModel.Set(LineItemCustomField.APTS_System_Type__c, value);
            }
        }

        public string APTS_Valuation_Class__c
        {
            get
            {
                return lineItemModel.Get<string>(LineItemCustomField.APTS_Valuation_Class__c);
            }
            set
            {
                lineItemModel.Set(LineItemCustomField.APTS_Valuation_Class__c, value);
            }
        }

        public decimal? APTS_Service_List_Price__c
        {
            get
            {
                return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Service_List_Price__c);
            }
            set
            {
                lineItemModel.Set(LineItemCustomField.APTS_Service_List_Price__c, value);
            }
        }

        public decimal? APTS_Minimum_Price_Service__c
        {
            get
            {
                return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Minimum_Price_Service__c);
            }
            set
            {
                lineItemModel.Set(LineItemCustomField.APTS_Minimum_Price_Service__c, value);
            }
        }

        public decimal? APTS_Cost_Service__c
        {
            get
            {
                return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Cost_Service__c);
            }
            set
            {
                lineItemModel.Set(LineItemCustomField.APTS_Cost_Service__c, value);
            }
        }

        public string APTS_VolumeTier__c
        {
            get
            {
                return lineItemModel.Get<string>(LineItemCustomField.APTS_VolumeTier__c);
            }
            set
            {
                lineItemModel.Set(LineItemCustomField.APTS_VolumeTier__c, value);
            }
        }

        public decimal? APTS_ContractDiscount__c
        {
            get
            {
                return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_ContractDiscount__c);
            }
            set
            {
                lineItemModel.Set(LineItemCustomField.APTS_ContractDiscount__c, value);
            }
        }

        public bool? APTS_Local_Bundle_Component_Flag__c { get { return lineItemModel.Get<bool?>(LineItemCustomField.APTS_Local_Bundle_Component_Flag__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Local_Bundle_Component_Flag__c, value); } }
        public decimal? APTS_Unit_Incentive_Adj_Amount__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Unit_Incentive_Adj_Amount__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Unit_Incentive_Adj_Amount__c, value); } }
        public decimal? APTS_Promotion_Discount_Amount_c__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Promotion_Discount_Amount_c__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Promotion_Discount_Amount_c__c, value); } }
        public decimal? APTS_Solution_Offered_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Solution_Offered_Price__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Solution_Offered_Price__c, value); } }
        public decimal? APTS_Offered_Price_c__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Offered_Price_c__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Offered_Price_c__c, value); } }
        public decimal? APTS_Item_List_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Item_List_Price__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Item_List_Price__c, value); } }
        public decimal? APTS_Solution_list_Price_c__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Solution_list_Price_c__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Solution_list_Price_c__c, value); } }
        public decimal? APTS_Philips_List_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Philips_List_Price__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Philips_List_Price__c, value); } }
        public decimal? APTS_Net_Adjustment_Percent_c__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Net_Adjustment_Percent_c__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Net_Adjustment_Percent_c__c, value); } }
        public decimal? APTS_Contract_Discount_Amount__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Contract_Discount_Amount__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Contract_Discount_Amount__c, value); } }
        public string APTS_Billing_Plan__c { get { return lineItemModel.Get<string>(LineItemCustomField.APTS_Billing_Plan__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Billing_Plan__c, value); } }
        public decimal? APTS_Unit_Strategic_Discount_Amount__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Unit_Strategic_Discount_Amount__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Unit_Strategic_Discount_Amount__c, value); } }
        public decimal? APTS_Strategic_Discount_Amount_c__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Strategic_Discount_Amount_c__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Strategic_Discount_Amount_c__c, value); } }
        public decimal? APTS_SAP_List_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_SAP_List_Price__c); } set { lineItemModel.Set(LineItemCustomField.APTS_SAP_List_Price__c, value); } }
        public decimal? APTS_Target_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Target_Price__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Target_Price__c, value); } }
        public decimal? APTS_Minimum_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Minimum_Price__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Minimum_Price__c, value); } }
        public decimal? APTS_Escalation_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Escalation_Price__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Escalation_Price__c, value); } }
        public decimal? APTS_Contract_Net_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Contract_Net_Price__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Contract_Net_Price__c, value); } }
        public decimal? APTS_Escalation_Price_Attainment_c__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Escalation_Price_Attainment_c__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Escalation_Price_Attainment_c__c, value); } }
        public decimal? APTS_Target_Price_Attainment__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Target_Price_Attainment__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Target_Price_Attainment__c, value); } }
        public decimal? APTS_Minimum_Price_Attainment_c__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Minimum_Price_Attainment_c__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Minimum_Price_Attainment_c__c, value); } }
        public string APTS_Price_Attainment_Color__c { get { return lineItemModel.Get<string>(LineItemCustomField.APTS_Price_Attainment_Color__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Price_Attainment_Color__c, value); } }
        public decimal? APTS_Minimum_Price_Bundle__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Minimum_Price_Bundle__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Minimum_Price_Bundle__c, value); } }
        public decimal? APTS_Escalation_Price_Bundle__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Escalation_Price_Bundle__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Escalation_Price_Bundle__c, value); } }
        public decimal? APTS_Country_Target_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Country_Target_Price__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Country_Target_Price__c, value); } }
        public decimal? APTS_Target_Price_Bundle__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Target_Price_Bundle__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Target_Price_Bundle__c, value); } }
        public decimal? APTS_Option_List_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Option_List_Price__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Option_List_Price__c, value); } }
        public decimal? APTS_Solution_Contract_Discount_Amount__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Solution_Contract_Discount_Amount__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Solution_Contract_Discount_Amount__c, value); } }
        public decimal? APTS_Incentive_Adjustment_Amount_Bundle__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Incentive_Adjustment_Amount_Bundle__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Incentive_Adjustment_Amount_Bundle__c, value); } }
        public decimal? APTS_Solution_Unit_Incentive_Adj_Amount__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Solution_Unit_Incentive_Adj_Amount__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Solution_Unit_Incentive_Adj_Amount__c, value); } }
        public decimal? APTS_Solution_Offered_Price_c__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Solution_Offered_Price_c__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Solution_Offered_Price_c__c, value); } }
        public decimal? APTS_Solution_List_Price_c__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Solution_List_Price_c__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Solution_List_Price_c__c, value); } }
        public decimal? APTS_Total_Resulting_Discount_Amount__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Total_Resulting_Discount_Amount__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Total_Resulting_Discount_Amount__c, value); } }
        public string APTS_Solution_Price_Attainment_Color__c { get { return lineItemModel.Get<string>(LineItemCustomField.APTS_Solution_Price_Attainment_Color__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Solution_Price_Attainment_Color__c, value); } }
        public string APTS_Payment_Term__c { get { return lineItemModel.Get<string>(LineItemCustomField.APTS_Payment_Term__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Payment_Term__c, value); } }
        public string APTS_Inco_Term__c { get { return lineItemModel.Get<string>(LineItemCustomField.APTS_Inco_Term__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Inco_Term__c, value); } }
        public decimal? APTS_Escalation_Price_Attainment__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Escalation_Price_Attainment__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Escalation_Price_Attainment__c, value); } }
        public decimal? APTS_Minimum_Price_Attainment__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Minimum_Price_Attainment__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Minimum_Price_Attainment__c, value); } }
        public string APTS_MAG__c { get { return lineItemModel.Get<string>(LineItemCustomField.APTS_MAG__c); } set { lineItemModel.Set(LineItemCustomField.APTS_MAG__c, value); } }
        public string APTS_Business_Unit__c { get { return lineItemModel.Get<string>(LineItemCustomField.APTS_Business_Unit__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Business_Unit__c, value); } }
        public bool? APTS_Is_Escalation_Price_Attained__c { get { return lineItemModel.Get<bool?>(LineItemCustomField.APTS_Is_Escalation_Price_Attained__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Is_Escalation_Price_Attained__c, value); } }
        public bool? APTS_Is_Minimum_Price_Attained__c { get { return lineItemModel.Get<bool?>(LineItemCustomField.APTS_Is_Minimum_Price_Attained__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Is_Minimum_Price_Attained__c, value); } }
        public bool? APTS_Procurement_approval_needed__c { get { return lineItemModel.Get<bool?>(LineItemCustomField.APTS_Procurement_approval_needed__c); } set { lineItemModel.Set(LineItemCustomField.APTS_Procurement_approval_needed__c, value); } }

        #endregion

        #region Relationship LineItem Fields

        public string Apttus_Config2__ProductId__r_ProductCode { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_ProductCode); }  }
        public string Apttus_Config2__ProductId__r_Apttus_Config2__ProductType__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Apttus_Config2__ProductType__c); }  }
        public string Apttus_Config2__ProductId__r_APTS_SPOO_Type__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_APTS_SPOO_Type__c); } }
        public decimal? Apttus_Config2__AttributeValueId__r_APTS_Cost_Price__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__AttributeValueId__r_APTS_Cost_Price__c); } }
        public decimal? Apttus_Config2__AttributeValueId__r_APTS_Fair_Market_Value__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__AttributeValueId__r_APTS_Fair_Market_Value__c); } }
        public decimal? Apttus_Config2__AttributeValueId__r_APTS_List_Price__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__AttributeValueId__r_APTS_List_Price__c); } }
        public string Apttus_Config2__AttributeValueId__r_APTS_Service_Plan_Name__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__AttributeValueId__r_APTS_Service_Plan_Name__c); } }
        public string Apttus_Config2__AttributeValueId__r_APTS_Product_Name__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__AttributeValueId__r_APTS_Product_Name__c); } }
        public string Apttus_Config2__ProductId__r_APTS_Valuation_Class__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_APTS_Valuation_Class__c); } }
        public string Apttus_Config2__OptionId__r_APTS_Valuation_Class__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_APTS_Valuation_Class__c); } }
        public bool Apttus_Config2__OptionId__r_APTS_Local_Bundle__c { get { return lineItemModel.GetLookupValue<bool>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_APTS_Local_Bundle__c); } }
        public string Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c); } }
        public string Apttus_Config2__OptionId__r_Main_Article_Group_ID__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Main_Article_Group_ID__c); } }
        public string Apttus_Config2__OptionId__r_Business_Unit_ID__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Business_Unit_ID__c); } }
        public string Apttus_Config2__ProductId__r_Main_Article_Group_ID__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Main_Article_Group_ID__c); } }
        public string Apttus_Config2__ProductId__r_Business_Unit_ID__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Business_Unit_ID__c); } }
        public string Apttus_Config2__ProductId__r_APTS_Type__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_APTS_Type__c); } }
        public string Apttus_Config2__ProductId__r_APTS_CLOGS__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_APTS_CLOGS__c); } }

        #endregion
    }
}

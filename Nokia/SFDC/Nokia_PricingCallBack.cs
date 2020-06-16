/************************************************************************************************************
* Apex Class Name   : Nokia_PricingCallBack.cls
* Version	 : 1.0 
* Created Date	  : 14 Aug 2017
* Function	: Pricing  calculation on cart to calculation maintenance, SSP and SRS product
* Modification Log  :
* Developer	   Date		Description
* -----------------------------------------------------------------------------------------------------------
* Rupesh Sharma	  08/14/2017	 Pricing  calculation on cart to calculation maintenance, SSP and SRS product
* Varsha Mahalwala			08/22/2018	 Changes in Release 5 for introducing new Portfolio
************************************************************************************************************/
class Nokia_PricingCallBack
{
    private Apttus_Config2.ProductConfiguration cart = null;
    private Apttus_Config2.CustomClass.PricingMode mode = null;
    private List<Accreditation__c> allAccreditationlist = new List<Accreditation__c>();
    private String maintenanceType = '';
    private boolean isUpdate = false;
    private boolean isION = false;
    private boolean isFN = false;
    private boolean isLEO = false;
    private boolean is1Year = false;
    private Apttus_Proposal__Proposal__c proposalSO;
    private Apttus_Config2__ProductConfiguration__c configSO;
    private List<NokiaCPQ_Maintenance_and_SSP_Rules__c> maintenanceSSPRule = new List<NokiaCPQ_Maintenance_and_SSP_Rules__c>();
    private List<NokiaCPQ_Maintenance_and_SSP_Rules__c> maintenanceSSPRulePMA = new List<NokiaCPQ_Maintenance_and_SSP_Rules__c>();
    private List<Tier_Discount_Detail__c> tierDiscountDetails = new List<Tier_Discount_Detail__c>();
    private List<Apttus_Config2__LineItem__c> lineItemObject = new List<Apttus_Config2__LineItem__c>();
    private Map<String, Apttus_Config2__LineItem__c> lineItemObjectMap = new Map<String, Apttus_Config2__LineItem__c>();
    private Map<double, Apttus_Config2__LineItem__c> lineItemObjectMapDirect = new Map<double, Apttus_Config2__LineItem__c>();

    private Map<String, double> productPriceMap = new Map<String, double>();
    private Map<String, double> productCostMap = new Map<String, double>();
    private List<CurrencyType> defaultExchangeRate = new List<CurrencyType>(); //Req 4188
    private List<Apttus_Config2__LineItem__c> csvLineItem = new List<Apttus_Config2__LineItem__c>();
    private List<Apttus_Config2__LineItem__c> mainLineItem = new List<Apttus_Config2__LineItem__c>();
    private List<Product2> prod = new List<Product2>();
    private Double minMaintIONContract;
    private Double minMaintFNContract;
    private Double rollUpPrice;
    private Double rollUpPriceIRP;
    private Double finalRollUpPrice;
    private Double mainBundleRollUp;
    private Shipping_Location__c shippingLoc;
    private String isIONExistingContract = Nokia_CPQ_Constants.BLANK_STRING_WITHOUT_SPACE;
    private String mainIONType = Nokia_CPQ_Constants.BLANK_STRING_WITHOUT_SPACE;
    private String mainFNType = Nokia_CPQ_Constants.BLANK_STRING_WITHOUT_SPACE;
    private String isFNExistingContract = Nokia_CPQ_Constants.BLANK_STRING_WITHOUT_SPACE;
    private List<Apttus_Config2__PriceListItem__c> priceListItem = new List<Apttus_Config2__PriceListItem__c>();
    private Map<String, List<String>> productAccredMap = new Map<String, List<String>>();
    private Double minMaintPrice = 0;
    Integer methodSequence = 0;
    Integer count = 0;
    private String maintPricinglevel;
    private Double renewalDist = 0;
    private Double attachmentDist = 0;
    private Double perfDist = 0;
    private Double multiYr = 0;
    private Map<String, List<NokiaCPQ_Maintenance_and_SSP_Rules__c>> maintenanceSSPRuleMap = new Map<String, List<NokiaCPQ_Maintenance_and_SSP_Rules__c>>();
    public Map<Decimal, Decimal> linenumberQuantity = new Map<Decimal, Decimal>();
    private Map<String, List<Double>> tierDiscountRuleMap = new Map<String, List<Double>>();
    private String debugTest = Nokia_CPQ_Constants.BLANK_STRING_WITHOUT_SPACE;
    private Set<String> nokiaSoftwareProductCodes = new Set<String>();
    List<Nokia_CPQ_SSP_SRS_Default_Values__mdt> sspSRSDefaultsList = new List<Nokia_CPQ_SSP_SRS_Default_Values__mdt>();
    List<String> pdcList = new List<String>();
    List<String> productList = new List<String>();
    Map<String, List<Apttus_Config2__LineItem__c>> primaryNoLineItemList = new Map<String, List<Apttus_Config2__LineItem__c>>();
    Map<String, List<Apttus_Config2__LineItem__c>> lineItemIRPMapDirect = new Map<String, List<Apttus_Config2__LineItem__c>>();
    List<String> mainBundleList = new List<String>();
    Map<String, Double> unitaryCostMap = new Map<String, Double>();
    Map<id, string> mapPliType = new Map<id, string>();
    List<Direct_Portfolio_General_Setting__mdt> portfolioSettingList = new List<Direct_Portfolio_General_Setting__mdt>();
    List<Direct_Care_Cost_Percentage__mdt> careCostPercentList = new List<Direct_Care_Cost_Percentage__mdt>();

    private List<NokiaCPQ_MN_Direct_Product_Map__mdt> MN_Direct_Products_List = new List<NokiaCPQ_MN_Direct_Product_Map__mdt>();
    //R-6508
    private Map<String, CPQ_Maintenance_and_SSP_Rule__c> maintenanceSSPRuleMap_EP = new Map<String, CPQ_Maintenance_and_SSP_Rule__c>();
    private Shipping_Location__c minMaintenance_EP;
    private Double minMaintPrice_EP = 0;
    private String isIONExistingContract_EP = Nokia_CPQ_Constants.BLANK_STRING_WITHOUT_SPACE;
    //R-6508 End
    //R-6500
    private List<Pricing_Guidance_Setting__c> pricingGuidanceSetting = new List<Pricing_Guidance_Setting__c>();
    Map<Id, Product_Extension__c> Product_extensitonmap = new Map<Id, Product_Extension__c>();
    Set<Id> ProductId_set = new Set<Id>();

    void start(Apttus_Config2.ProductConfiguration prodConfig)
    {
        this.cart = prodConfig;
        this.configSO = cart.getConfigSO();

        if (this.proposalSO == null && configSO.Apttus_Config2__BusinessObjectType__c != Nokia_CPQ_Constants.AGREEMENT)
        {
            this.proposalSO = [SELECT Id, NokiaCPQ_LEO_Discount__c, NokiaCPQ_Maintenance_Level__c, Apttus_Proposal__Account__r.GEOLevel1ID__c, Is_List_Price_Only__c, Warranty_credit__c, NokiaCPQ_Is_Maintenance_Quote__c, CurrencyIsoCode, Name,
                NokiaCPQ_Existing_IONMaint_Contract__c,
           NokiaCPQ_SSP_Level__c, NokiaCPQ_SRS_Level__c, NokiaCPQ_Maintenance_Accreditation__r.Portfolio__c, NokiaCPQ_Maintenance_Accreditation__r.Pricing_Cluster__c,
           NokiaCPQ_Portfolio__c, NokiaCPQ_Maintenance_Type__c, NokiaCPQ_No_of_Years__c, NokiaProductAccreditation__r.Portfolio__c, NokiaProductAccreditation__r.Pricing_Cluster__c,
           NokiaCPQ_IsPMA__c, NokiaProductAccreditation__r.Pricing_Accreditation__c, NokiaCPQ_Maintenance_Accreditation__r.Pricing_Accreditation__c, Quote_Type__c,
           exchange_rate__c, Apttus_Proposal__Account__r.Name, Apttus_Proposal__Account__r.Id, Apttus_Proposal__Account__r.Partner_Program__c,
          Apttus_Proposal__Account__r.Partner_Type__c,
           Apttus_Proposal__Account__r.NokiaCPQ_Renewal__c, Apttus_Proposal__Account__r.NokiaCPQ_Performance__c, Apttus_Proposal__Account__r.NokiaCPQ_Attachment__c,
           Apttus_Proposal__Account__r.CountryCode__c,
           NokiaCPQ_SRS_Per__c, NokiaCPQ_OpportunityLeadBG__c, Account_Market__c, Apttus_Proposal__Account__r.L7Name__c FROM Apttus_Proposal__Proposal__c WHERE Id =: configSO.Apttus_QPConfig__Proposald__c and Apttus_Proposal__Account__c != null LIMIT 1];
        }

        if (this.proposalSO != null && this.proposalSO.Quote_Type__c.equalsIgnoreCase(Nokia_CPQ_Constants.QUOTE_TYPE_DIRECTCPQ))
        {
            defaultExchangeRate = [SELECT ConversionRate FROM CurrencyType WHERE IsoCode =: this.proposalSO.CurrencyIsoCode LIMIT: (Limits.getLimitQueryRows() - Limits.getQueryRows())];
        }
        if (this.proposalSO != null && this.proposalSO.Quote_Type__c.equalsIgnoreCase(Nokia_CPQ_Constants.QUOTE_TYPE_INDIRECTCPQ) && this.proposalSO.NokiaCPQ_No_of_Years__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_1YEAR) && this.proposalSO.NokiaCPQ_LEO_Discount__c == true)
        {
            //GP: Used in beforepricing-adjustment and onpriceitemset method. Can be used as local variable
            isLEO = true;
        }

        if (this.proposalSO != null && this.proposalSO.Quote_Type__c.equalsIgnoreCase(Nokia_CPQ_Constants.QUOTE_TYPE_INDIRECTCPQ) && this.proposalSO.NokiaCPQ_No_of_Years__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_1YEAR))
        {
            //GP: Used in onpriceitemset method. Can be used as local variable
            is1Year = true;
        }

        if (this.proposalSO != null && this.proposalSO.Quote_Type__c.equalsIgnoreCase(Nokia_CPQ_Constants.QUOTE_TYPE_DIRECTCPQ) &&
            (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase('QTC') ||
    (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_IP_ROUTING) &&
     this.proposalSO.Is_List_Price_Only__c == false)))
        {
            //GP: This can be fetched once pricing comleted to call deal guidance method. Can be used as local variable
            pricingGuidanceSetting = [SELECT Name, Threshold__c FROM Pricing_Guidance_Setting__c
                          WHERE Name =: this.proposalSO.NokiaCPQ_Portfolio__c
                          LIMIT 1];
        }

        this.mainLineItem = [SELECT CustomProductValue__c, NokiaCPQ_Is_SI__c, Source__c, Total_ONT_Quantity__c, Total_ONT_Quantity_FBA__c, Total_ONT_Quantity_P2P__c
        , is_Custom_Product__c, NCPQ_Unitary_CLP__c, Apttus_Config2__BasePrice__c, NokiaCPQ_Spare__c, NokiaCPQ_Product_Type__c, NokiaCPQ_Extended_CLP_2__c,
        Apttus_Config2__ListPrice__c, Apttus_Config2__AdjustmentAmount__c, Apttus_Config2__AdjustmentType__c,
        NokiaCPQ_Maint_Yr1_Extended_Price__c, NokiaCPQ_Maint_Yr2_Extended_Price__c,
        Apttus_Config2__ProductId__r.NokiaCPQ_Item_Type__c, Nokia_SRS_Base_Extended_Price__c,
        Nokia_SSP_Base_Extended_Price__c, Apttus_Config2__PrimaryLineNumber__c,
        Apttus_Config2__ParentBundleNumber__c, Apttus_Config2__ProductId__c, NokiaCPQ_Unitary_IRP__c,
        Apttus_Config2__ChargeType__c, Apttus_Config2__Quantity__c, Apttus_Config2__OptionId__c,
        Apttus_Config2__LineType__c, Apttus_Config2__BasePriceOverride__c,
        Apttus_Config2__LineNumber__c, Apttus_Config2__ExtendedPrice__c, NokiaCPQ_Extended_CNP__c, NokiaCPQ_AdvancePricing_CUP__c,
        NokiaCPQAdv_Net_Price__c, NokiaCPQ_ExtendedPrice_CNP__c, NokiaCPQ_ExtendedAdvance_NP__c, Advanced_pricing_done__c, NokiaCPQ_AdvancePricing_NP__c,
        Apttus_Config2__AdjustedPrice__c, Apttus_Config2__NetPrice__c, NokiaCPQ_Extended_CUP__c,
        NokiaCPQ_Unitary_Cost__c, NokiaCPQ_Extended_CLP__c,
        NokiaCPQ_Extended_IRP__c, NokiaCPQ_ExtendedPrice_CUP__c, Apttus_Config2__ProductId__r.Business_Group__c,
        NokiaCPQ_Is_CLP__c, OEM__c, NokiaCPQ_Light_Color__c, NokiaCPQ_Maximum_IRP_Discount__c, NokiaCPQ_IRP_Discount__c, Apttus_Config2__ProductId__r.NokiaCPQ_Product_Discount_Category__c, 
        Apttus_Config2__OptionId__r.NokiaCPQ_Product_Discount_Category__c, Apttus_Config2__ProductId__r.Apttus_Config2__ConfigurationType__c, Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c,
        Apttus_Config2__ProductId__r.NokiaCPQ_Classification2__c, Apttus_Config2__OptionId__r.NokiaCPQ_Classification2__c, Apttus_Config2__OptionId__r.NokiaCPQ_Item_Type__c, 
        Apttus_Config2__ProductId__r.NokiaCPQ_License_Usage__c, Apttus_Config2__OptionId__r.NokiaCPQ_License_Usage__c,
        Apttus_Config2__ProductId__r.Id, Apttus_Config2__OptionId__r.Id
        FROM Apttus_Config2__LineItem__c
        where Apttus_Config2__ConfigurationId__c = :configSO.Id];

        for (Apttus_Config2__LineItem__c lineItem : this.mainLineItem)
        {
            if (lineItem.Apttus_Config2__LineType__c == Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES)
            {
                //GP: This map may not be required as we can directly get root/parent bundle from the optione line item.
                lineItemObjectMap.put(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES + Nokia_CPQ_Constants.NOKIA_UNDERSCORE + lineItem.Apttus_Config2__LineNumber__c, lineItem);
            }
        }
        if (this.proposalSO != null && this.proposalSO.Quote_Type__c.equalsIgnoreCase(Nokia_CPQ_Constants.QUOTE_TYPE_DIRECTCPQ))
        {
            for (Apttus_Config2__LineItem__c item : this.mainLineItem)
            {
                String configType = getConfigType(item);

                //GP: lineItemIRPMapDirect need to be created using all cart line items.
                if (lineItemIRPMapDirect.containsKey(item.Apttus_Config2__LineNumber__c + '_' + item.Apttus_Config2__ChargeType__c))
                {
                    lineItemIRPMapDirect.get(item.Apttus_Config2__LineNumber__c + '_' + item.Apttus_Config2__ChargeType__c).add(item);
                }
                else
                {
                    lineItemIRPMapDirect.put(item.Apttus_Config2__LineNumber__c + '_' + item.Apttus_Config2__ChargeType__c, new List<Apttus_Config2__LineItem__c> { item });
                }

                if (item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES))
                {
                    productList.add(item.Apttus_Config2__ProductId__c);
                }
                else
                {
                    productList.add(item.Apttus_Config2__OptionId__c);
                }

                if (!item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES))
                {
                    //GP: primaryNoLineItemList need to be created using all cart line items.
                    if (primaryNoLineItemList.containsKey(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c))
                    {
                        primaryNoLineItemList.get(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c).add(item);
                    }
                    else
                    {
                        primaryNoLineItemList.put(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c, new List<Apttus_Config2__LineItem__c> { item });
                    }
                }

                if (configType.equalsIgnoreCase('Bundle'))
                {
                    //GP: lineItemObjectMapDirect need to be created using all cart line items.
                    lineItemObjectMapDirect.put(item.Apttus_Config2__PrimaryLineNumber__c, item);
                }

                if (item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES))
                {
                    mainBundleList.add(String.valueOf(item.Apttus_Config2__PrimaryLineNumber__c));
                }

                if (this.proposalSO != null && (this.proposalSO.NokiaCPQ_Portfolio__c == 'QTC' ||
                   (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_IP_ROUTING)
                     && this.proposalSO.Is_List_Price_Only__c == false)))
                {
                    if (item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES))
                    {
                        ProductId_set.add(item.Apttus_Config2__ProductId__r.Id);
                    }
                    else
                    {
                        ProductId_set.add(item.Apttus_Config2__OptionId__r.Id);
                    }
                }
            }

            if (!ProductId_set.isEmpty())
            {
                List<Product_Extension__c> Prod_extList = QTC_DataExtract.getrelatedproductextensions(ProductId_set, this.ProposalSO.CurrencyIsoCode);
                If(!Prod_extList.isEmpty()){
                    for (Product_Extension__c Prod_ext: Prod_extList)
                    {
                        Product_extensitonmap.put(Prod_ext.Product__c, Prod_ext);
                    }
                }
            }

            this.priceListItem = [SELECT Apttus_Config2__ListPrice__c, Apttus_Config2__ProductId__c, Apttus_Config2__ProductActive__c, Apttus_Config2__ProductId__r.Portfolio__c, Apttus_Config2__Cost__c from Apttus_Config2__PriceListItem__c where Apttus_Config2__PriceListId__r.PriceList_Type__c = 'CPQ' AND Apttus_Config2__ProductId__c in :productList AND Apttus_Config2__PriceListId__r.Apttus_Config2__BasedOnPriceListId__c = Null  AND CurrencyIsoCode = 'EUR'];

            for (Apttus_Config2__PriceListItem__c pli : this.priceListItem)
            {
                productPriceMap.put(pli.Apttus_Config2__ProductId__c, pli.Apttus_Config2__ListPrice__c);
                productCostMap.put(pli.Apttus_Config2__ProductId__c, pli.Apttus_Config2__Cost__c);
            }

            //fetching the MN Direct Product Map custom Metadata
            if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING))
            {
                MN_Direct_Products_List = [Select Id, NokiaCPQ_Product_Code__c, NokiaCPQ_Product_Type__c from NokiaCPQ_MN_Direct_Product_Map__mdt];
            }

            this.portfolioSettingList = [SELECT Portfolio__c, Cost_Calculation_In_PCB__c FROM Direct_Portfolio_General_Setting__mdt where Portfolio__c =: this.proposalSO.NokiaCPQ_Portfolio__c LIMIT 1];

            this.careCostPercentList = [SELECT Market__c, Care_Cost__c FROM Direct_Care_Cost_Percentage__mdt WHERE Market__c =: this.proposalSO.Account_Market__c];

            //R-6508

            if (Nokia_CPQ_Constants.NOKIA_IP_ROUTING.equalsIgnoreCase(this.proposalSO.NokiaCPQ_Portfolio__c) && !this.proposalSO.Is_List_Price_Only__c)
            {

                for (CPQ_Maintenance_and_SSP_Rule__c main_rule : [Select Region__c, Maintenance_Level__c, Maintenance_Type__c, Maintenance_Category__c,
                              Service_Rate_Y1__c, Service_Rate_Y2__c, Biennial_SSP_Discount__c, Unlimited_SSP_Discount__c,
                             Biennial_SRS_Discount__c, Unlimited_SRS_Discount__c
                             from CPQ_Maintenance_and_SSP_Rule__c
                             Where Region__c =: this.proposalSO.Apttus_Proposal__Account__r.GEOLevel1ID__c and Maintenance_Type__c =: this.proposalSO.NokiaCPQ_Maintenance_Type__c LIMIT: (Limits.getLimitQueryRows() - Limits.getQueryRows())]){
                    if (main_rule.Maintenance_Type__c != Null && main_rule.Maintenance_Category__c != Null)
                    {
                        maintenanceSSPRuleMap_EP.put(main_rule.Maintenance_Type__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + main_rule.Maintenance_Category__c, main_rule);
                    }
                }

                List<Shipping_Location__c> minMaintPriceList_EP = new List<Shipping_Location__c>();

                minMaintPriceList_EP =   [Select Id, Min_Maint_EUR__c, Min_Maint_USD__c, Quote_Type__c, Maintenance_Type__c, Portfolio__c
                        From Shipping_Location__c
                        where Quote_Type__c =: Nokia_CPQ_Constants.QUOTE_TYPE_DIRECTCPQ and Maintenance_Type__c =: this.proposalSO.NokiaCPQ_Maintenance_Type__c and Portfolio__c =:this.proposalSO.NokiaCPQ_Portfolio__c LIMIT 1];

                if (!minMaintPriceList_EP.isEmpty() && minMaintPriceList_EP != Null)
                {
                    this.minMaintenance_EP = minMaintPriceList_EP[0];
                }
                if (minMaintenance_EP != null && (proposalSO.CurrencyIsoCode).equals(Nokia_CPQ_Constants.USDCURRENCY))
                {
                    minMaintPrice_EP = minMaintenance_EP.Min_Maint_USD__c;
                }
                else if (minMaintenance_EP != null && (proposalSO.CurrencyIsoCode).equals(Nokia_CPQ_Constants.EUR_CURR))
                {
                    minMaintPrice_EP = minMaintenance_EP.Min_Maint_EUR__c;
                }
                //logic for Existing maintenance Contract
                if (this.proposalSO.NokiaCPQ_Existing_IONMaint_Contract__c != null)
                {
                    isIONExistingContract_EP = String.valueOf(this.proposalSO.NokiaCPQ_Existing_IONMaint_Contract__c);
                }
            }
            //R-6508 END
        }

        if (this.proposalSO != null && this.proposalSO.Quote_Type__c.equalsIgnoreCase(Nokia_CPQ_Constants.QUOTE_TYPE_INDIRECTCPQ))
        {
            List<Shipping_Location__c> shippingLocList = new List<Shipping_Location__c>();
            if (this.proposalSO.NokiaCPQ_Maintenance_Accreditation__r.Portfolio__c != null && this.proposalSO.NokiaCPQ_Maintenance_Accreditation__r.Pricing_Cluster__c != null)
            {
                shippingLocList = [SELECT Id, Min_Maint_EUR__c, LEO_Mini_Maint_EUR__c, LEO_Mini_Maint_USD__c, Min_Maint_USD__c, Portfolio__c, Pricing_Cluster__c From Shipping_Location__c where Portfolio__c =: this.proposalSO.NokiaCPQ_Maintenance_Accreditation__r.Portfolio__c and Pricing_Cluster__c =: this.proposalSO.NokiaCPQ_Maintenance_Accreditation__r.Pricing_Cluster__c LIMIT 1];
                if (!shippingLocList.isEmpty())
                {
                    this.shippingLoc = shippingLocList[0];
                }
            }
            //to reduce heap size
            shippingLocList = null;

            if (this.proposalSO.NokiaCPQ_Existing_IONMaint_Contract__c != null)
            {
                isIONExistingContract = String.valueOf(this.proposalSO.NokiaCPQ_Existing_IONMaint_Contract__c);
            }

            /*Req 3478 Start*/
            if (shippingLoc != null && (proposalSO.CurrencyIsoCode).equals(Nokia_CPQ_Constants.USDCURRENCY))
            {
                if (proposalSO.NokiaCPQ_LEO_Discount__c)
                {
                    minMaintPrice = shippingLoc.LEO_Mini_Maint_USD__c;
                }
                else
                {
                    minMaintPrice = shippingLoc.Min_Maint_USD__c;
                }
            }
            else if (shippingLoc != null && !((proposalSO.CurrencyIsoCode).equals(Nokia_CPQ_Constants.USDCURRENCY)))
            {
                if (proposalSO.NokiaCPQ_LEO_Discount__c)
                {
                    minMaintPrice = shippingLoc.LEO_Mini_Maint_EUR__c;
                }
                else
                {
                    minMaintPrice = shippingLoc.Min_Maint_EUR__c;
                }
            }
            /*Req 3478 End*/
            //Heema : Req 6593 start: 
            if (proposalSO.NokiaCPQ_LEO_Discount__c && !proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase('Nuage'))
            {
                maintenanceType = proposalSO.NokiaCPQ_Maintenance_Type__c;
            }
            else
            {
                maintenanceType = Nokia_CPQ_Constants.MAINT_TYPE_DEFAULT_VALUE;
            }
            //defect 14394 start
            if (this.proposalSO.NokiaCPQ_Maintenance_Level__c != Nokia_CPQ_Constants.NOKIA_YES)
                maintPricinglevel = this.proposalSO.NokiaCPQ_Maintenance_Accreditation__r.Pricing_Accreditation__c;
            else
                maintPricinglevel = Nokia_CPQ_Constants.Nokia_Brand;
            //defect 14394 END
            //Start: Varsha - Changes in R5 for Req 4143

            if (!proposalSO.NokiaCPQ_IsPMA__c && !proposalSO.NokiaCPQ_LEO_Discount__c)
            {
                this.maintenanceSSPRule = [SELECT NokiaCPQ_withPMA__c, NokiaCPQ_Pricing_Cluster__c, NokiaCPQ_Product_Discount_Category__c, NokiaCPQ_Product_Discount_Category_per__c, NokiaCPQ_Unlimited_SSP_Discount__c, NokiaCPQ_Biennial_SSP_Discount__c, NokiaCPQ_Maintenance_Level__c, NokiaCPQ_Maintenance_Type__c, NokiaCPQ_Service_Rate_Y1__c, NokiaCPQ_Service_Rate_Y2__c FROM NokiaCPQ_Maintenance_and_SSP_Rules__c WHERE NokiaCPQ_withPMA__c = FALSE AND Partner_Program__c =: proposalSO.Apttus_Proposal__Account__r.Partner_Program__c];

            }
            else if (proposalSO.NokiaCPQ_LEO_Discount__c)
            {
                this.maintenanceSSPRule = [SELECT NokiaCPQ_withPMA__c, NokiaCPQ_Pricing_Cluster__c, NokiaCPQ_Product_Discount_Category__c, NokiaCPQ_Product_Discount_Category_per__c, NokiaCPQ_Unlimited_SSP_Discount__c, NokiaCPQ_Biennial_SSP_Discount__c, NokiaCPQ_Maintenance_Level__c, NokiaCPQ_Maintenance_Type__c, NokiaCPQ_Service_Rate_Y1__c, NokiaCPQ_Service_Rate_Y2__c FROM NokiaCPQ_Maintenance_and_SSP_Rules__c WHERE NokiaCPQ_withPMA__c = FALSE and NokiaCPQ_Maintenance_Type__c =: maintenanceType and NokiaCPQ_Maintenance_Level__c = '' and Partner_Program__c =: proposalSO.Apttus_Proposal__Account__r.Partner_Program__c];
            }
            else if (proposalSO.NokiaCPQ_IsPMA__c)
            {
                this.maintenanceSSPRule = [SELECT NokiaCPQ_withPMA__c, NokiaCPQ_Pricing_Cluster__c, NokiaCPQ_Product_Discount_Category__c, NokiaCPQ_Product_Discount_Category_per__c, NokiaCPQ_Unlimited_SSP_Discount__c, NokiaCPQ_Biennial_SSP_Discount__c, NokiaCPQ_Maintenance_Level__c, NokiaCPQ_Maintenance_Type__c, NokiaCPQ_Service_Rate_Y1__c, NokiaCPQ_Service_Rate_Y2__c FROM NokiaCPQ_Maintenance_and_SSP_Rules__c WHERE NokiaCPQ_withPMA__c = TRUE and NokiaCPQ_Maintenance_Type__c =: proposalSO.NokiaCPQ_Maintenance_Type__c and NokiaCPQ_Maintenance_Level__c =:maintPricinglevel and Partner_Program__c =: proposalSO.Apttus_Proposal__Account__r.Partner_Program__c];
            }
            else { }
            //Heema : Req 6593 start:
            //End: Varsha - Changes in R5 for Req 4143

            if (!this.maintenanceSSPRule.isEmpty())
            {
                for (Integer maintSSPSize = 0; maintSSPSize < this.maintenanceSSPRule.size(); maintSSPSize++)
                {
                    if (this.maintenanceSSPRule.get(maintSSPSize).NokiaCPQ_Pricing_Cluster__c.equalsIgnoreCase(proposalSO.NokiaCPQ_Maintenance_Accreditation__r.Pricing_Cluster__c) || this.maintenanceSSPRule.get(maintSSPSize).NokiaCPQ_Pricing_Cluster__c.equalsIgnoreCase(proposalSO.NokiaProductAccreditation__r.Pricing_Cluster__c))
                    {
                        if (maintenanceSSPRuleMap.containsKey(this.maintenanceSSPRule.get(maintSSPSize).NokiaCPQ_Pricing_Cluster__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.maintenanceSSPRule.get(maintSSPSize).NokiaCPQ_Product_Discount_Category__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.maintenanceSSPRule.get(maintSSPSize).NokiaCPQ_withPMA__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.maintenanceSSPRule.get(maintSSPSize).NokiaCPQ_Maintenance_Type__c))
                        {
                            maintenanceSSPRuleMap.get(this.maintenanceSSPRule.get(maintSSPSize).NokiaCPQ_Pricing_Cluster__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.maintenanceSSPRule.get(maintSSPSize).NokiaCPQ_Product_Discount_Category__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.maintenanceSSPRule.get(maintSSPSize).NokiaCPQ_withPMA__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.maintenanceSSPRule.get(maintSSPSize).NokiaCPQ_Maintenance_Type__c).add(this.maintenanceSSPRule.get(maintSSPSize));
                        }
                        else
                        {
                            maintenanceSSPRuleMap.put(this.maintenanceSSPRule.get(maintSSPSize).NokiaCPQ_Pricing_Cluster__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.maintenanceSSPRule.get(maintSSPSize).NokiaCPQ_Product_Discount_Category__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.maintenanceSSPRule.get(maintSSPSize).NokiaCPQ_withPMA__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.maintenanceSSPRule.get(maintSSPSize).NokiaCPQ_Maintenance_Type__c, new List<NokiaCPQ_Maintenance_and_SSP_Rules__c> {
            this.maintenanceSSPRule.get(maintSSPSize)
                });
                        }
                    }
                }
            }
            //to reduce heap size
            this.maintenanceSSPRule = null;

            this.tierDiscountDetails = [SELECT NokiaCPQ_Tier_Type__c, NokiaCPQ_Partner_Type__c, NokiaCPQ_Pricing_Tier__c, NokiaCPQ_Tier_Discount__c, Nokia_CPQ_Partner_Program__c FROM Tier_Discount_Detail__c where Nokia_CPQ_Partner_Program__c =: this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c and NokiaCPQ_Partner_Type__c =: this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c LIMIT: (Limits.getLimitQueryRows() - Limits.getQueryRows())];
            if (!this.tierDiscountDetails.isEmpty())
            {
                for (Integer tierDiscSize = 0; tierDiscSize < this.tierDiscountDetails.size(); tierDiscSize++)
                {
                    List<Double> tierDiscountRuleList = new List<Double>();
                    tierDiscountRuleList.add(Double.valueOf(this.tierDiscountDetails.get(tierDiscSize).NokiaCPQ_Tier_Discount__c));
                    tierDiscountRuleMap.put(this.tierDiscountDetails.get(tierDiscSize).NokiaCPQ_Tier_Type__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.tierDiscountDetails.get(tierDiscSize).NokiaCPQ_Partner_Type__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.tierDiscountDetails.get(tierDiscSize).NokiaCPQ_Pricing_Tier__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.tierDiscountDetails.get(tierDiscSize).Nokia_CPQ_Partner_Program__c, tierDiscountRuleList);

                }
            }

            //to reduce heap size
            this.tierDiscountDetails = null;

            String accredACCID = String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Id).substring(0, 15);

            //Varsha: Start: Changes for Req #4920
            this.sspSRSDefaultsList = [Select Portfolio__c, SSP_Visible__c, SRS_Visible__c, SRS_Percentage__c, Tier_Discount_Applicable__c, AccountLevel_Discount_Applicable__c, Multi_Year_Discount_Applicable__c From Nokia_CPQ_SSP_SRS_Default_Values__mdt where Portfolio__c =: this.proposalSO.NokiaCPQ_Portfolio__c LIMIT 1];
            //Varsha: End: Changes for Req #4920

            String pdcs = Label.SRSPDC;
            if (String.isNotBlank(pdcs))
            {
                pdcList.addAll(pdcs.split(Nokia_CPQ_Constants.SEMICOLON_STRING));
            }

            // Req : 5260
            if ((this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_FIXED_ACCESS_POL) || this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_FIXED_ACCESS_FBA)) || this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.Nokia_FASTMILE))
            {
                for (Apttus_Config2__PriceList__c objPl : [select id, PriceList_Type__c from Apttus_Config2__PriceList__c where PriceList_Type__c= 'Indirect Market']){
                    mapPliType.put(objPl.id, objPl.PriceList_Type__c);
                }
            }
        }
    }

    void setMode(Apttus_Config2.CustomClass.PricingMode mode)
    {
        this.mode = mode;
    }

    /*Start: Methods for replacing usage of formula fields for improving performance*/
    String getPartNumber(Apttus_Config2__LineItem__c item)
    {
        String partNumber = '';
        if (item.is_Custom_Product__c)
        {
            partNumber = item.Custom_Product_Code__c;
        }
        else
        {
            if ('Product/Service'.equalsIgnoreCase(item.Apttus_Config2__LineType__c))
            {
                partNumber = item.Apttus_Config2__ProductId__r.ProductCode;
            }
            else
            {
                partNumber = item.Apttus_Config2__OptionId__r.ProductCode;
            }
        }

        return partNumber;
    }

    String getPortfolio(Apttus_Config2__LineItem__c item)
    {

        String portfolio = '';
        if (item.Apttus_Config2__ProductId__c != null && item.Apttus_Config2__ProductId__r.Portfolio__c != Null)
        {
            portfolio = item.Apttus_Config2__ProductId__r.Portfolio__c;
        }

        return portfolio;
    }

    static String getProductDiscountCategory(Apttus_Config2__LineItem__c item)
    {
        String productDiscountCat = '';

        if ('Product/Service'.equalsIgnoreCase(item.Apttus_Config2__LineType__c))
        {
            productDiscountCat = String.valueOf(item.Apttus_Config2__ProductId__r.NokiaCPQ_Product_Discount_Category__c);
        }
        else
        {
            productDiscountCat = String.valueOf(item.Apttus_Config2__OptionId__r.NokiaCPQ_Product_Discount_Category__c);
        }

        return productDiscountCat;
    }

    static String getConfigType(Apttus_Config2__LineItem__c item)
    {
        String configType = '';

        if ('Product/Service'.equalsIgnoreCase(item.Apttus_Config2__LineType__c))
        {
            configType = String.valueOf(item.Apttus_Config2__ProductId__r.Apttus_Config2__ConfigurationType__c);
        }
        else
        {
            configType = String.valueOf(item.Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c);
        }

        return configType;
    }

    static String getClassification(Apttus_Config2__LineItem__c item)
    {
        String classification = '';

        if ('Product/Service'.equalsIgnoreCase(item.Apttus_Config2__LineType__c))
        {
            classification = String.valueOf(item.Apttus_Config2__ProductId__r.NokiaCPQ_Classification2__c);
        }
        else
        {
            classification = String.valueOf(item.Apttus_Config2__OptionId__r.NokiaCPQ_Classification2__c);
        }

        return classification;
    }

    static String getItemType(Apttus_Config2__LineItem__c item)
    {
        String itemType = '';

        if ('Product/Service'.equalsIgnoreCase(item.Apttus_Config2__LineType__c))
        {
            itemType = String.valueOf(item.Apttus_Config2__ProductId__r.NokiaCPQ_Item_Type__c);
        }
        else
        {
            itemType = String.valueOf(item.Apttus_Config2__OptionId__r.NokiaCPQ_Item_Type__c);
        }

        return itemType;
    }

    static String getLicenseUsage(Apttus_Config2__LineItem__c item)
    {
        String licenseUsage = '';

        if ('Product/Service'.equalsIgnoreCase(item.Apttus_Config2__LineType__c))
        {
            licenseUsage = String.valueOf(item.Apttus_Config2__ProductId__r.NokiaCPQ_License_Usage__c);
        }
        else
        {
            licenseUsage = String.valueOf(item.Apttus_Config2__OptionId__r.NokiaCPQ_License_Usage__c);
        }

        return licenseUsage;
    }

    /*End: Methods for replacing usage of formula fields for improving performance  */
    /* Method Name : beforePricing
    * Developer   : Apttus
    * Description : OOTB method override with pricing calculation for Nokia */
    void beforePricing(Apttus_Config2.ProductConfiguration.LineItemColl itemColl)
    {
        List<String> sspFNList = System.label.FN_SSP_Product.Split(';');
        set<String> sspFNSet = new set<String>();
        sspFNSet.addAll(sspFNList);

        try
        {
            if (this.proposalSO != null && this.proposalSO.Quote_Type__c.equalsIgnoreCase(Nokia_CPQ_Constants.QUOTE_TYPE_INDIRECTCPQ))
            {
                if (mode == Apttus_Config2.CustomClass.PricingMode.basePrice)
                {
                    for (Apttus_Config2.LineItem allLineItems: itemColl.getAllLineItems())
                    {
                        Apttus_Config2__LineItem__c item = allLineItems.getLineItemSO();

                        if (item.Apttus_Config2__BasePrice__c != null && !item.Apttus_Config2__PriceIncludedInBundle__c && item.Apttus_Config2__BasePrice__c != item.Apttus_Config2__BasePrice__c.setScale(2, RoundingMode.HALF_UP))
                        {
                            //First Time
                            item.Apttus_Config2__BasePriceOverride__c = item.Apttus_Config2__BasePrice__c.setScale(2, RoundingMode.HALF_UP);
                            item.Apttus_Config2__BasePrice__c = item.Apttus_Config2__BasePrice__c.setScale(2, RoundingMode.HALF_UP);
                        }

                        String partNumber = getPartNumber(item);

                        if (partNumber != null && partNumber.equalsIgnoreCase(Nokia_CPQ_Constants.MAINTY2CODE))
                        {
                            item.Apttus_Config2__LineSequence__c = 997;
                        }
                        else if (partNumber != null && partNumber.equalsIgnoreCase(Nokia_CPQ_Constants.MAINTY1CODE))
                        {
                            item.Apttus_Config2__LineSequence__c = 996;
                        }
                        else if (partNumber != null && partNumber.equalsIgnoreCase(Nokia_CPQ_Constants.SSPCODE))
                        {
                            item.Apttus_Config2__LineSequence__c = 998;
                        }
                        else if (partNumber != null && partNumber.equalsIgnoreCase(Nokia_CPQ_Constants.SRS))
                        {
                            item.Apttus_Config2__LineSequence__c = 999;
                        }
                    }
                }

                if (mode == Apttus_Config2.CustomClass.PricingMode.ADJUSTMENT && Nokia_CPQ_Constants.PCBBEFOREPRICINGINADJ.equalsIgnoreCase(Nokia_CPQ_Constants.FALSE_CONSTANT))
                {
                    Double totalOntQuantity = 0.0;
                    Double totalFBAONTQty = 0.00;
                    Double totalFBAP2PQty = 0.00;
                    for (Apttus_Config2.LineItem allLineItems: this.cart.getLineItems())
                    {
                        Apttus_Config2__LineItem__c item = allLineItems.getLineItemSO();
                        if (item.Apttus_Config2__BasePrice__c != null && !item.Apttus_Config2__PriceIncludedInBundle__c && (item.Apttus_Config2__BasePriceOverride__c == null || (item.Apttus_Config2__BasePriceOverride__c != null && item.Apttus_Config2__BasePriceOverride__c != item.Apttus_Config2__BasePrice__c.setScale(2, RoundingMode.HALF_UP))))
                        {
                            //Second time
                            item.Apttus_Config2__BasePriceOverride__c = item.Apttus_Config2__BasePrice__c.setScale(2, RoundingMode.HALF_UP);
                            item.Apttus_Config2__PricingStatus__c = 'Pending';
                        }
                        String partNumber = getPartNumber(item);
                        if (partNumber != null && partNumber.equalsIgnoreCase(Nokia_CPQ_Constants.MAINTY2CODE))
                        {
                            item.Apttus_Config2__LineSequence__c = 997;
                        }
                        else if (partNumber != null && partNumber.equalsIgnoreCase(Nokia_CPQ_Constants.MAINTY1CODE))
                        {
                            item.Apttus_Config2__LineSequence__c = 996;

                        }
                        else if (partNumber != null && partNumber.equalsIgnoreCase(Nokia_CPQ_Constants.SSPCODE))
                        {
                            item.Apttus_Config2__LineSequence__c = 998;
                        }
                        else if (partNumber != null && partNumber.equalsIgnoreCase(Nokia_CPQ_Constants.SRS))
                        {
                            item.Apttus_Config2__LineSequence__c = 999;
                        }
                    }
                }

                methodSequence++;
                if (mode == Apttus_Config2.CustomClass.PricingMode.ADJUSTMENT && Nokia_CPQ_Constants.PCBBEFOREPRICINGINADJ.equalsIgnoreCase(Nokia_CPQ_Constants.FALSE_CONSTANT))
                {
                    Double totalExtendedMaintY1Price = 0.00;
                    Double totalExtendedMaintY2Price = 0.00;
                    Double totalExtendedSSPPrice = 0.00;
                    Double totalExtendedSRSPrice = 0.00;

                    Double totalOntQuantity = 0.0;
                    Double totalFBAONTQty = 0.00;
                    Double totalFBAP2PQty = 0.00;

                    for (Apttus_Config2__LineItem__c item: this.mainLineItem)
                    {
                        String productDiscountCat = getProductDiscountCategory(item);
                        if ((item.Apttus_Config2__BasePrice__c != null && Double.valueOf(item.Apttus_Config2__BasePrice__c) > 0.00 && item.NokiaCPQ_Spare__c == False) || (item.Apttus_Config2__IsHidden__c = true && item.Apttus_Config2__BasePrice__c != null && item.Apttus_Config2__BasePrice__c == 0.00))
                        {
                            if (item.NokiaCPQ_Maint_Yr1_Extended_Price__c != null)
                            {
                                totalExtendedMaintY1Price = totalExtendedMaintY1Price + Double.valueOf(item.NokiaCPQ_Maint_Yr1_Extended_Price__c);
                            }
                            if (item.NokiaCPQ_Maint_Yr2_Extended_Price__c != null)
                            {
                                totalExtendedMaintY2Price = totalExtendedMaintY2Price + Double.valueOf(item.NokiaCPQ_Maint_Yr2_Extended_Price__c);
                            }

                            //Replace item.Portfolio_from_Quote_Line_Item__c formula field with 'this.proposalSO.NokiaCPQ_Portfolio__c', NokiaCPQ_Product_Discount_Category__c with value fetched from method
                            if (((productDiscountCat != null && !pdcList.isEmpty() && pdcList.contains(productDiscountCat)) || (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_NUAGE) && Nokia_CPQ_Constants.PRODUCTITEMTYPESOFTWARE.equalsIgnoreCase(item.Apttus_Config2__ProductId__r.NokiaCPQ_Item_Type__c))) || item.is_Custom_Product__c == true)
                            {
                                if (item.Nokia_SRS_Base_Extended_Price__c != null)
                                {
                                    totalExtendedSRSPrice = totalExtendedSRSPrice + item.Nokia_SRS_Base_Extended_Price__c;
                                }
                            }
                        }

                        if ((productDiscountCat != null && !productDiscountCat.contains(Nokia_CPQ_Constants.NOKIA_NFM_P) && item.NokiaCPQ_Spare__c == False) || item.is_Custom_Product__c == true)
                        {
                            if (item.Nokia_SSP_Base_Extended_Price__c != null)
                            {
                                totalExtendedSSPPrice = totalExtendedSSPPrice + Double.valueOf(item.Nokia_SSP_Base_Extended_Price__c);
                            }
                        }

                        if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase('Fixed Access - POL') && item.Total_ONT_Quantity__c != null)
                        {
                            totalOntQuantity = totalOntQuantity + item.Total_ONT_Quantity__c;
                        }
                        if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase('Fixed Access - FBA'))
                        {
                            if (item.Total_ONT_Quantity_FBA__c != null)
                            {
                                totalFBAONTQty = totalFBAONTQty + item.Total_ONT_Quantity_FBA__c;
                            }
                            else if (item.Total_ONT_Quantity_P2P__c != null)
                            {
                                totalFBAP2PQty = totalFBAP2PQty + item.Total_ONT_Quantity_P2P__c;
                            }
                        }
                    }

                    for (Apttus_Config2.LineItem configLineItem: this.cart.getLineItems())
                    {
                        Apttus_Config2__LineItem__c item = configLineItem.getLineItemSO();
                        String partNumber = getPartNumber(item);
                        isUpdate = false;

                        //Execute third time only for Product/Service lines to complete price calculations after quantity update
                        if (item.Apttus_Config2__BasePrice__c != null && !item.Apttus_Config2__PriceIncludedInBundle__c && item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES))
                        {
                            isUpdate = true;
                        }

                        if (item.is_Custom_Product__c == false && item.Is_Contract_Pricing_2__c == true && item.Apttus_Config2__PriceListItemId__c != null)
                        {
                            item.Apttus_Config2__BasePriceOverride__c = item.Apttus_Config2__PriceListItemId__r.Partner_Price__c;
                            item.Apttus_Config2__BasePrice__c = item.Apttus_Config2__PriceListItemId__r.Partner_Price__c;
                            isUpdate = true;
                        }

                        //Varsha: End: Changes in Sprint 7 for Req 3354
                        if (partNumber != null && partNumber.contains(Nokia_CPQ_Constants.MAINTY1CODE))
                        {
                            if (this.proposalSO != null && totalExtendedMaintY1Price > 0)
                            {
                                if (Nokia_CPQ_Constants.Nokia_NO.equalsIgnoreCase(isIONExistingContract) && totalExtendedMaintY1Price < minMaintPrice)
                                {
                                    totalExtendedMaintY1Price = minMaintPrice;
                                }
                            }
                            if (item.Apttus_Config2__ConfigurationId__r.Apttus_QPConfig__Proposald__r.Maintenance_Y1__c != null)
                            {
                                item.Apttus_Config2__BasePriceOverride__c = item.Apttus_Config2__ConfigurationId__r.Apttus_QPConfig__Proposald__r.Maintenance_Y1__c;
                                item.Apttus_Config2__BasePrice__c = item.Apttus_Config2__ConfigurationId__r.Apttus_QPConfig__Proposald__r.Maintenance_Y1__c;
                            }
                            else
                            {
                                item.Apttus_Config2__BasePriceOverride__c = totalExtendedMaintY1Price;
                                item.Apttus_Config2__BasePrice__c = totalExtendedMaintY1Price;
                            }
                            isUpdate = true;
                        }
                        else if (partNumber != null && partNumber.contains(Nokia_CPQ_Constants.MAINTY2CODE))
                        {
                            if (item.Apttus_Config2__ConfigurationId__r.Apttus_QPConfig__Proposald__r.Maintenance_Y2__c != null)
                            {
                                item.Apttus_Config2__BasePriceOverride__c = item.Apttus_Config2__ConfigurationId__r.Apttus_QPConfig__Proposald__r.Maintenance_Y2__c;
                                item.Apttus_Config2__BasePrice__c = item.Apttus_Config2__ConfigurationId__r.Apttus_QPConfig__Proposald__r.Maintenance_Y2__c;
                            }
                            else
                            {
                                item.Apttus_Config2__BasePriceOverride__c = totalExtendedMaintY2Price;
                                item.Apttus_Config2__BasePrice__c = totalExtendedMaintY2Price;
                            }
                            isUpdate = true;
                        }
                        else if (item.Apttus_Config2__ChargeType__c != null && !String.isBlank(String.valueOf(item.Apttus_Config2__ChargeType__c)) && String.valueOf(item.Apttus_Config2__ChargeType__c).contains(Nokia_CPQ_Constants.NOKIA_PRODUCT_NAME_SSP))
                        {
                            if (item.Apttus_Config2__ConfigurationId__r.Apttus_QPConfig__Proposald__r.SSP__c != null)
                            {
                                item.Apttus_Config2__BasePriceOverride__c = item.Apttus_Config2__ConfigurationId__r.Apttus_QPConfig__Proposald__r.SSP__c;
                                item.Apttus_Config2__BasePrice__c = item.Apttus_Config2__ConfigurationId__r.Apttus_QPConfig__Proposald__r.SSP__c;
                            }
                            else if (isLEO)
                            {
                                item.Apttus_Config2__BasePriceOverride__c = 0.00;
                                item.Apttus_Config2__BasePrice__c = 0.00;

                            }
                            else
                            {
                                item.Apttus_Config2__BasePriceOverride__c = totalExtendedSSPPrice;
                                item.Apttus_Config2__BasePrice__c = totalExtendedSSPPrice;
                            }

                            isUpdate = true;
                        }
                        else if (item.Apttus_Config2__ChargeType__c != null && !String.isBlank(String.valueOf(item.Apttus_Config2__ChargeType__c)) && String.valueOf(item.Apttus_Config2__ChargeType__c).contains(Nokia_CPQ_Constants.NOKIA_PRODUCT_NAME_SRS))
                        {
                            if (item.Apttus_Config2__ConfigurationId__r.Apttus_QPConfig__Proposald__r.SRS__c != null)
                            {
                                item.Apttus_Config2__BasePriceOverride__c = item.Apttus_Config2__ConfigurationId__r.Apttus_QPConfig__Proposald__r.SRS__c;
                                item.Apttus_Config2__BasePrice__c = item.Apttus_Config2__ConfigurationId__r.Apttus_QPConfig__Proposald__r.SRS__c;
                            }
                            else if (isLEO)
                            {
                                item.Apttus_Config2__BasePriceOverride__c = 0.00;
                                item.Apttus_Config2__BasePrice__c = 0.00;

                            }
                            else
                            {
                                item.Apttus_Config2__BasePriceOverride__c = totalExtendedSRSPrice;
                                item.Apttus_Config2__BasePrice__c = totalExtendedSRSPrice;
                            }
                            isUpdate = true;
                        }
                        if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase('Fixed Access - POL') && sspFNSet.contains(partNumber))
                        {
                            item.Apttus_Config2__Quantity__c = integer.valueof(this.proposalSO.NokiaCPQ_No_of_Years__c) * totalOntQuantity;
                            isUpdate = true;
                        }
                        if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase('Fixed Access - FBA') && sspFNSet.contains(partNumber))
                        {
                            item.Apttus_Config2__Quantity__c = integer.valueof(this.proposalSO.NokiaCPQ_No_of_Years__c) * totalFBAONTQty + integer.valueof(this.proposalSO.NokiaCPQ_No_of_Years__c) * totalFBAP2PQty;
                            isUpdate = true;
                        }
                        if (isUpdate)
                        {
                            configLineItem.updatePrice();
                        }

                    }
                    Nokia_CPQ_Constants.PCBBEFOREPRICINGINADJ = Nokia_CPQ_Constants.TRUE_CONSTANT;
                }
            }
            else if (this.proposalSO != null && this.proposalSO.Quote_Type__c.equalsIgnoreCase(Nokia_CPQ_Constants.QUOTE_TYPE_DIRECTCPQ))
            {
                //GP: Q-Why are we executing below logic for baseprice mode ? Condition on adjustment mode is not enough ?
                if (mode == Apttus_Config2.CustomClass.PricingMode.basePrice || (mode == Apttus_Config2.CustomClass.PricingMode.ADJUSTMENT && Nokia_CPQ_Constants.PCBBEFOREPRICINGINADJ.equalsIgnoreCase(Nokia_CPQ_Constants.FALSE_CONSTANT)))
                {
                    system.debug('mode: ' + mode);
                    //initialize the maintenance price
                    Double totalExtendedMaintY1Price = 0.00;
                    Double totalExtendedMaintY2Price = 0.00;
                    Map<String, Apttus_Config2.LineItem> maintenanceLinesMap = new Map<String, Apttus_Config2.LineItem>();
                    //R-6508 
                    Map<String, Apttus_Config2.LineItem> maintenanceLinesMap_EP = new Map<String, Apttus_Config2.LineItem>();
                    //R-6508 END

                    //Start: using maps to limit for loop iterations
                    Map<String, Apttus_Config2__LineItem__c> productServiceMap = new Map<String, Apttus_Config2__LineItem__c>();
                    Map<String, Apttus_Config2__LineItem__c> careSRSOptionMap = new Map<String, Apttus_Config2__LineItem__c>();
                    //End
                    for (Apttus_Config2.LineItem allLineItems: this.cart.getLineItems())
                    {
                        Apttus_Config2__LineItem__c item = allLineItems.getLineItemSO();
                        String partNumber = getPartNumber(item);
                        if (mode == Apttus_Config2.CustomClass.PricingMode.ADJUSTMENT)
                        {
                            //Logic from Workflow: Enable Manual Adjustment For Options
                            if ((this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_SOFTWARE) || (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_IP_ROUTING) && !this.proposalSO.Is_List_Price_Only__c)) && !item.Apttus_Config2__ChargeType__c.equalsIgnoreCase(Nokia_CPQ_Constants.STANDARD))
                            {
                                item.Apttus_Config2__AllowManualAdjustment__c = false;
                            }
                            if (item.Apttus_Config2__BasePrice__c != null && item.Apttus_Config2__BasePrice__c > 0 && item.Apttus_Config2__ChargeType__c != null && item.Apttus_Config2__ChargeType__c.equalsIgnoreCase('Standard Price') && !(item.Source__c == 'BOMXAE' && this.proposalSO.NokiaCPQ_Portfolio__c == 'QTC'))
                            {
                                if (item.Apttus_Config2__PriceListId__c == item.Apttus_Config2__PriceListItemId__r.Apttus_Config2__PriceListId__c && item.Apttus_Config2__BasePriceOverride__c != ((item.Apttus_Config2__BasePrice__c / defaultExchangeRate.get(0).ConversionRate) * (this.proposalSO.exchange_rate__c)).setScale(2, RoundingMode.HALF_UP))
                                {
                                    item.Apttus_Config2__BasePriceOverride__c = ((item.Apttus_Config2__BasePrice__c / defaultExchangeRate.get(0).ConversionRate) * (this.proposalSO.exchange_rate__c)).setScale(5, RoundingMode.HALF_UP);
                                    item.Apttus_Config2__BasePriceOverride__c = ((item.Apttus_Config2__BasePrice__c / defaultExchangeRate.get(0).ConversionRate) * (this.proposalSO.exchange_rate__c)).setScale(2, RoundingMode.HALF_UP);
                                    item.Apttus_Config2__PricingStatus__c = 'Pending';
                                }
                                else if (item.Apttus_Config2__PriceListId__c != item.Apttus_Config2__PriceListItemId__r.Apttus_Config2__PriceListId__c && item.Apttus_Config2__BasePriceOverride__c != item.Apttus_Config2__BasePrice__c.setScale(2, RoundingMode.HALF_UP))
                                {
                                    item.Apttus_Config2__BasePriceOverride__c = item.Apttus_Config2__BasePrice__c.setScale(5, RoundingMode.HALF_UP);
                                    item.Apttus_Config2__BasePriceOverride__c = item.Apttus_Config2__BasePrice__c.setScale(2, RoundingMode.HALF_UP);
                                    item.Apttus_Config2__PricingStatus__c = 'Pending';
                                }
                            }
                        }
                        //To set quantities for other charge types on main bundle
                        if (item != null && item.Apttus_Config2__LineType__c != NULL && item.Apttus_Config2__ChargeType__c != NULL && item.Apttus_Config2__ChargeType__c.equalsIgnoreCase(Nokia_CPQ_Constants.STANDARD) && item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES))
                        {
                            linenumberQuantity.put(item.Apttus_Config2__LineNumber__c, item.Apttus_Config2__Quantity__c);
                        }
                        //Map of Airscale wifi Maintenance lines
                        if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING))
                        {
                            if (partNumber != null && partNumber.contains(Nokia_CPQ_Constants.MAINTY1CODE))
                            {
                                maintenanceLinesMap.put('Year1', allLineItems);
                            }
                            else if (partNumber != null && partNumber.contains(Nokia_CPQ_Constants.MAINTY2CODE))
                            {
                                maintenanceLinesMap.put('Year2', allLineItems);
                            }
                        }
                        //Start: creating maps to limit for loop iterations
                        if (item != null && item.Apttus_Config2__LineType__c != NULL && item.Apttus_Config2__ChargeType__c != NULL && item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES))
                        {
                            productServiceMap.put(item.Id, item);
                        }
                        if (item != null && item.Apttus_Config2__LineType__c != NULL && item.Apttus_Config2__ChargeType__c != NULL && (item.Apttus_Config2__ChargeType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_YEAR1_MAINTENANCE) || item.Apttus_Config2__ChargeType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_SRS)) && !item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES))
                        {
                            careSRSOptionMap.put(item.Id, item);
                        }
                        //End
                    }

                    //R-6508
                    if (Nokia_CPQ_Constants.NOKIA_IP_ROUTING.equalsIgnoreCase(this.proposalSO.NokiaCPQ_Portfolio__c) && !this.proposalSO.Is_List_Price_Only__c)
                    {
                        for (Apttus_Config2.LineItem allLineItems: this.cart.getLineItems())
                        {
                            Apttus_Config2__LineItem__c item = allLineItems.getLineItemSO();
                            String partNumber = getPartNumber(item);

                            //Piece of code written below calculates Maintenance for each line item and saves it on line item for Direct Enterprise
                            if (partNumber != null &&
                      !partNumber.contains(Nokia_CPQ_Constants.MAINTY1CODE) &&
                      !partNumber.contains(Nokia_CPQ_Constants.MAINTY2CODE) &&
                      !partNumber.contains(Nokia_CPQ_Constants.SSPCODE) &&
                      !partNumber.contains(Nokia_CPQ_Constants.SRS) &&
                      !item.Is_List_Price_Only__c)
                            {
                                Map<String, double> maintenanceValueMap = calculateMaintenance_EP_Direct(item, totalExtendedMaintY1Price, totalExtendedMaintY2Price, partNumber);
                                totalExtendedMaintY1Price = maintenanceValueMap.get('ExtendedMaintY1Price');
                                totalExtendedMaintY2Price = maintenanceValueMap.get('ExtendedMaintY2Price');
                            }

                            //Map Of Miantenance Line items for IP routing- Enterprise
                            if (partNumber != null && partNumber.contains(Nokia_CPQ_Constants.MAINTY1CODE))
                            {
                                maintenanceLinesMap_EP.put('Year1', allLineItems);
                            }
                            else if (partNumber != null && partNumber.contains(Nokia_CPQ_Constants.MAINTY2CODE))
                            {
                                maintenanceLinesMap_EP.put('Year2', allLineItems);
                            }

                        }

                        Apttus_Config2__LineItem__c lineItemVarSO;
                        Boolean isUpdate = False;
                        if (maintenanceLinesMap_EP.Size() > 0)
                        {
                            Apttus_Config2.LineItem lineItemVar;
                            lineItemVarSO = new Apttus_Config2__LineItem__c();

                            if (maintenanceLinesMap_EP.containsKey('Year1'))
                            {
                                lineItemVar = maintenanceLinesMap_EP.get('Year1');
                                lineItemVarSO = lineItemVar.getLineItemSO();

                                if (Nokia_CPQ_Constants.NOKIA_NO.equalsIgnoreCase(isIONExistingContract_EP) && minMaintPrice_EP != null && minMaintPrice_EP > totalExtendedMaintY1Price)
                                {
                                    if (lineItemVarSO.Apttus_Config2__BasePriceOverride__c != minMaintPrice_EP)
                                    {
                                        lineItemVarSO.Apttus_Config2__BasePriceOverride__c = minMaintPrice_EP;
                                        lineItemVarSO.NokiaCPQ_Unitary_IRP__c = minMaintPrice_EP;
                                        lineItemVarSO.Apttus_Config2__LineSequence__c = 996;
                                        isUpdate = True;
                                    }
                                }
                                else
                                {
                                    if (lineItemVarSO.Apttus_Config2__BasePriceOverride__c != totalExtendedMaintY1Price)
                                    {
                                        lineItemVarSO.Apttus_Config2__BasePriceOverride__c = totalExtendedMaintY1Price;
                                        lineItemVarSO.NokiaCPQ_Unitary_IRP__c = totalExtendedMaintY1Price;
                                        lineItemVarSO.Apttus_Config2__LineSequence__c = 996;
                                        isUpdate = True;
                                    }
                                }

                                if (isUpdate)
                                {
                                    System.debug('checkcounty1>>>');
                                    lineItemVar.updatePrice();
                                    isUpdate = False;
                                }
                            }
                            if (maintenanceLinesMap_EP.containsKey('Year2'))
                            {
                                lineItemVar = maintenanceLinesMap_EP.get('Year2');
                                lineItemVarSO = lineItemVar.getLineItemSO();

                                if (lineItemVarSO.Apttus_Config2__BasePriceOverride__c != totalExtendedMaintY2Price)
                                {
                                    lineItemVarSO.Apttus_Config2__BasePriceOverride__c = totalExtendedMaintY2Price;
                                    lineItemVarSO.NokiaCPQ_Unitary_IRP__c = totalExtendedMaintY2Price;
                                    lineItemVarSO.Apttus_Config2__LineSequence__c = 997;
                                    isUpdate = True;
                                }

                                if (isUpdate)
                                {
                                    System.debug('checkcounty2>>>');
                                    lineItemVar.updatePrice();
                                    isUpdate = False;
                                }
                            }
                        }
                    }
                    //R-6508 End

                    //Piyush Tawari Req 6229 Airscale Wifi Direct
                    //Copy Discounts from Groups to SIs
                    if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING))
                    {
                        for (Apttus_Config2.LineItem allLineItems: this.cart.getLineItems())
                        {
                            Apttus_Config2__LineItem__c item = allLineItems.getLineItemSO();

                            String partNumber = getPartNumber(item);
                            String configType = getConfigType(item);
                            if (!configType.equalsIgnoreCase('Bundle') && item.Apttus_Config2__LineType__c.equalsIgnoreCase('Option')
                        && lineItemObjectMapDirect != null && !item.Advanced_pricing_done__c)
                            {
                                if (lineItemObjectMapDirect.get(item.Apttus_Config2__ParentBundleNumber__c) != null && item.Apttus_Config2__ParentBundleNumber__c != item.Apttus_Config2__LineNumber__c)
                                {
                                    if (lineItemObjectMapDirect.get(item.Apttus_Config2__ParentBundleNumber__c).Apttus_Config2__AdjustmentType__c == Nokia_CPQ_Constants.DISCOUNT_PERCENT)
                                    {

                                        item.Apttus_Config2__AdjustmentType__c = lineItemObjectMapDirect.get(item.Apttus_Config2__ParentBundleNumber__c).Apttus_Config2__AdjustmentType__c;
                                        item.Apttus_Config2__AdjustmentAmount__c = lineItemObjectMapDirect.get(item.Apttus_Config2__ParentBundleNumber__c).Apttus_Config2__AdjustmentAmount__c;
                                    }
                                    else if (lineItemObjectMapDirect.get(item.Apttus_Config2__ParentBundleNumber__c).Apttus_Config2__AdjustmentType__c == Nokia_CPQ_Constants.DISCOUNT_AMOUNT)
                                    {
                                        if ((lineItemObjectMapDirect.get(item.Apttus_Config2__ParentBundleNumber__c).NokiaCPQ_Extended_CLP__c != 0 || lineItemObjectMapDirect.get(item.Apttus_Config2__ParentBundleNumber__c).NokiaCPQ_Extended_CLP__c != Null) && lineItemObjectMapDirect.get(item.Apttus_Config2__ParentBundleNumber__c).Apttus_Config2__AdjustmentAmount__c != Null)
                                        {
                                            double disPer = 0.00;
                                            disPer = (lineItemObjectMapDirect.get(item.Apttus_Config2__ParentBundleNumber__c).Apttus_Config2__AdjustmentAmount__c / lineItemObjectMapDirect.get(item.Apttus_Config2__ParentBundleNumber__c).NokiaCPQ_Extended_CLP__c) * 100;

                                            item.Apttus_Config2__AdjustmentType__c = Nokia_CPQ_Constants.DISCOUNT_PERCENT;
                                            item.Apttus_Config2__AdjustmentAmount__c = disPer;
                                        }
                                    }
                                    else if (lineItemObjectMapDirect.get(item.Apttus_Config2__ParentBundleNumber__c).Apttus_Config2__AdjustmentType__c == Null)
                                    {
                                        item.Apttus_Config2__AdjustmentType__c = Null;
                                        item.Apttus_Config2__AdjustmentAmount__c = Null;
                                    }
                                }
                            }
                            //Piece of cde written below calculates Maintenance for each line item and saves it on line item for Airscale Wifi
                            if (partNumber != null && !partNumber.contains(Nokia_CPQ_Constants.MAINTY1CODE) && !partNumber.contains(Nokia_CPQ_Constants.MAINTY2CODE))
                            {
                                Map<String, double> maintenanceValueMap = calculateMaintenance_MN_Direct(item, totalExtendedMaintY1Price, totalExtendedMaintY2Price, partNumber, configType);

                                totalExtendedMaintY1Price = maintenanceValueMap.get('ExtendedMaintY1Price');
                                totalExtendedMaintY2Price = maintenanceValueMap.get('ExtendedMaintY2Price');

                            }
                            if (item != null && item.Apttus_Config2__LineType__c != NULL && item.Apttus_Config2__ChargeType__c != NULL && !item.Apttus_Config2__ChargeType__c.equalsIgnoreCase(Nokia_CPQ_Constants.STANDARD) && item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES))
                            {
                                if (linenumberQuantity.size() > 0 && linenumberQuantity.containsKey(item.Apttus_Config2__LineNumber__c))
                                {
                                    item.Apttus_Config2__Quantity__c = linenumberQuantity.get(item.Apttus_Config2__LineNumber__c);

                                }
                            }
                        }

                        for (Apttus_Config2.LineItem allLineItems: this.cart.getLineItems())
                        {
                            Apttus_Config2__LineItem__c item = allLineItems.getLineItemSO();

                            String configType = getConfigType(item);

                            /**Piyush Tawari Start**/
                            //Req-6228 MN Airscale wifi - Price for the groups to be aggregated from SI
                            //For Direct MN Airscale Wifi
                            //checking if its Group
                            if (item != null && item.Apttus_Config2__LineType__c != NULL && item.Apttus_Config2__ChargeType__c != NULL && Nokia_CPQ_Constants.NOKIA_OPTION.equalsIgnoreCase(item.Apttus_Config2__LineType__c) && Nokia_CPQ_Constants.BUNDLE.equalsIgnoreCase(configType))
                            {
                                item.NokiaCPQ_Unitary_Cost__c = 0.00;
                                item.NCPQ_Unitary_CLP__c = 0.00;
                                item.NokiaCPQ_Unitary_IRP__c = 0.00;
                                item.NokiaCPQ_Extended_CUP__c = 0.00;
                                item.NokiaCPQ_Extended_CNP__c = 0.00;
                                if (primaryNoLineItemList.containsKey(item.Apttus_Config2__PrimaryLineNumber__c + '_' + item.Apttus_Config2__ChargeType__c))
                                {
                                    for (Apttus_Config2__LineItem__c optionItem : primaryNoLineItemList.get(item.Apttus_Config2__PrimaryLineNumber__c + '_' + item.Apttus_Config2__ChargeType__c))
                                    {
                                        //Stamping IRP at Group Level
                                        if (optionItem.NokiaCPQ_Unitary_IRP__c != Null && optionItem.Apttus_Config2__Quantity__c != Null)
                                            item.NokiaCPQ_Unitary_IRP__c = (item.NokiaCPQ_Unitary_IRP__c + optionItem.NokiaCPQ_Unitary_IRP__c * optionItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                                        //stamping CLP at Group level
                                        if (optionItem.Apttus_Config2__BasePriceOverride__c != null)
                                        {
                                            item.NCPQ_Unitary_CLP__c = (item.NCPQ_Unitary_CLP__c + (optionItem.Apttus_Config2__BasePriceOverride__c).setScale(2, RoundingMode.HALF_UP) * optionItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                                        }
                                        else if (optionItem.Apttus_Config2__BasePrice__c != null)
                                        {
                                            item.NCPQ_Unitary_CLP__c = (item.NCPQ_Unitary_CLP__c + (optionItem.Apttus_Config2__BasePrice__c).setScale(2, RoundingMode.HALF_UP) * optionItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                                        }
                                        //Stamping CUP at Group level
                                        item.NokiaCPQ_Extended_CUP__c = item.NokiaCPQ_Extended_CUP__c + optionItem.Apttus_Config2__AdjustedPrice__c;
                                        //stamping CNP at Group Level
                                        item.NokiaCPQ_Extended_CNP__c = item.NokiaCPQ_Extended_CNP__c + optionItem.Apttus_Config2__NetPrice__c;
                                        if (optionItem.NokiaCPQ_Unitary_Cost__c != Null && optionItem.Apttus_Config2__Quantity__c != Null)
                                            item.NokiaCPQ_Unitary_Cost__c = (item.NokiaCPQ_Unitary_Cost__c + (optionItem.NokiaCPQ_Unitary_Cost__c).setScale(2, RoundingMode.HALF_UP) * optionItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                                    }
                                }
                                if (item.Apttus_Config2__AdjustmentType__c == Nokia_CPQ_Constants.DISCOUNT_AMOUNT)
                                    item.Apttus_Config2__BasePriceOverride__c = item.Apttus_Config2__AdjustmentAmount__c;
                                else
                                    item.Apttus_Config2__BasePriceOverride__c = 0;
                            }/**Piyush Tawari End**/
                        }

                        //Calculate MaintY1 & MaintY2 for MN Direct(Airscale wifi)
                        if (mode == Apttus_Config2.CustomClass.PricingMode.ADJUSTMENT)
                        {
                            Apttus_Config2__LineItem__c lineItemVarSO;
                            if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING) && maintenanceLinesMap.size() > 0)
                            {
                                Apttus_Config2.LineItem lineItemVar;
                                lineItemVarSO = new Apttus_Config2__LineItem__c();

                                if (maintenanceLinesMap.containsKey('Year1'))
                                {
                                    lineItemVar = maintenanceLinesMap.get('Year1');
                                    lineItemVarSO = lineItemVar.getLineItemSO();
                                    if (lineItemVarSO.Apttus_Config2__PriceListId__c == lineItemVarSO.Apttus_Config2__PriceListItemId__r.Apttus_Config2__PriceListId__c)
                                    {
                                        lineItemVarSO.Apttus_Config2__BasePriceOverride__c = totalExtendedMaintY1Price;
                                        lineItemVarSO.NokiaCPQ_Unitary_IRP__c = totalExtendedMaintY1Price;
                                    }
                                    else
                                    {
                                        lineItemVarSO.NokiaCPQ_Unitary_IRP__c = totalExtendedMaintY1Price;
                                    }
                                    lineItemVarSO.Apttus_Config2__LineSequence__c = 997;
                                    lineItemVar.updatePrice();
                                }
                                if (maintenanceLinesMap.containsKey('Year2'))
                                {
                                    lineItemVar = maintenanceLinesMap.get('Year2');
                                    lineItemVarSO = lineItemVar.getLineItemSO();
                                    if (proposalSO.NokiaCPQ_No_of_Years__c != Null && !String.isBlank(proposalSO.NokiaCPQ_No_of_Years__c) &&
                              Decimal.valueOf(proposalSO.NokiaCPQ_No_of_Years__c) > 1)
                                    {
                                        if (lineItemVarSO.Apttus_Config2__PriceListId__c == lineItemVarSO.Apttus_Config2__PriceListItemId__r.Apttus_Config2__PriceListId__c)
                                        {
                                            lineItemVarSO.Apttus_Config2__BasePriceOverride__c = totalExtendedMaintY2Price;
                                            lineItemVarSO.NokiaCPQ_Unitary_IRP__c = totalExtendedMaintY2Price;
                                        }
                                        else
                                        {
                                            lineItemVarSO.NokiaCPQ_Unitary_IRP__c = totalExtendedMaintY2Price;
                                        }
                                    }
                                    else
                                    {
                                        system.debug('INBASEPRICEOVER');
                                        lineItemVarSO.Apttus_Config2__BasePriceOverride__c = 0.00;
                                        lineItemVarSO.NokiaCPQ_Unitary_IRP__c = 0.00;
                                    }
                                    lineItemVarSO.Apttus_Config2__LineSequence__c = 998;
                                    lineItemVar.updatePrice();
                                }
                            }
                        }
                    }



                    Map<Decimal, List<Double>> BundleCareSRSPriceMap = new Map<Decimal, List<Double>>();
                    List<Double> careSRSList = new List<Double>();
                    if (!this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING))
                    {
                        for (Apttus_Config2__LineItem__c item : productServiceMap.values())
                        {
                            if (!item.Apttus_Config2__ChargeType__c.equalsIgnoreCase(Nokia_CPQ_Constants.STANDARD))
                            {
                                if (linenumberQuantity.size() > 0 && linenumberQuantity.containsKey(item.Apttus_Config2__LineNumber__c))
                                {
                                    item.Apttus_Config2__Quantity__c = linenumberQuantity.get(item.Apttus_Config2__LineNumber__c);
                                }
                            }
                            //Care & SRS calculation for NSW
                            if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase('Nokia Software'))
                            {
                                if (mode == Apttus_Config2.CustomClass.PricingMode.ADJUSTMENT)
                                {
                                    if (item.Apttus_Config2__ChargeType__c.equalsIgnoreCase(Nokia_CPQ_Constants.STANDARD) && lineItemIRPMapDirect.containsKey(item.Apttus_Config2__LineNumber__c + '_' + item.Apttus_Config2__ChargeType__c))
                                    {
                                        careSRSList = careSRSCalculationForNSW(item);
                                        if (!careSRSList.isEmpty())
                                        {
                                            BundleCareSRSPriceMap.put(item.Apttus_Config2__LineNumber__c, careSRSList);
                                        }
                                    }
                                }
                            }
                        }

                    }

                    //Stamp prices to Care & SRS
                    List<String> careProActiveList = System.label.NokiaCPQ_Care_Proactive.Split(',');
                    List<String> careAdvanceList = System.label.NokiaCPQ_Care_Advance.Split(',');
                    set<String> careProactiveSet = new set<String>();
                    set<String> careAdvanceSet = new set<String>();
                    careProactiveSet.addAll(careProActiveList);
                    careAdvanceSet.addAll(careAdvanceList);

                    if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase('Nokia Software') && mode == Apttus_Config2.CustomClass.PricingMode.ADJUSTMENT)
                    {
                        for (Apttus_Config2__LineItem__c item : careSRSOptionMap.values())
                        {
                            if (item.Apttus_Config2__ChargeType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_YEAR1_MAINTENANCE))
                            {

                                if (item.Apttus_Config2__OptionId__c != null && careAdvanceSet.contains(item.Apttus_Config2__OptionId__r.ProductCode) && BundleCareSRSPriceMap.containsKey(item.Apttus_Config2__LineNumber__c) && BundleCareSRSPriceMap.get(item.Apttus_Config2__LineNumber__c).get(0) != null && item.NokiaCPQ_CareSRSBasePrice__c != (BundleCareSRSPriceMap.get(item.Apttus_Config2__LineNumber__c).get(0) * 0.10).setScale(2, RoundingMode.HALF_UP))
                                {
                                    item.Apttus_Config2__BasePriceOverride__c = (BundleCareSRSPriceMap.get(item.Apttus_Config2__LineNumber__c).get(0) * 0.10).setScale(2, RoundingMode.HALF_UP);
                                    item.NokiaCPQ_CareSRSBasePrice__c = (BundleCareSRSPriceMap.get(item.Apttus_Config2__LineNumber__c).get(0) * 0.10).setScale(2, RoundingMode.HALF_UP);
                                    item.Apttus_Config2__PricingStatus__c = 'Pending';
                                }
                                else if (item.Apttus_Config2__OptionId__c != null && careProactiveSet.contains(item.Apttus_Config2__OptionId__r.ProductCode) && BundleCareSRSPriceMap.containsKey(item.Apttus_Config2__LineNumber__c) && BundleCareSRSPriceMap.get(item.Apttus_Config2__LineNumber__c).get(0) != null && item.NokiaCPQ_CareSRSBasePrice__c != (BundleCareSRSPriceMap.get(item.Apttus_Config2__LineNumber__c).get(0) * 0.15).setScale(2, RoundingMode.HALF_UP))
                                {
                                    item.Apttus_Config2__BasePriceOverride__c = (BundleCareSRSPriceMap.get(item.Apttus_Config2__LineNumber__c).get(0) * 0.15).setScale(2, RoundingMode.HALF_UP);
                                    item.NokiaCPQ_CareSRSBasePrice__c = (BundleCareSRSPriceMap.get(item.Apttus_Config2__LineNumber__c).get(0) * 0.15).setScale(2, RoundingMode.HALF_UP);
                                    item.Apttus_Config2__PricingStatus__c = 'Pending';
                                }
                            }
                            else if (item.Apttus_Config2__ChargeType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_SRS))
                            {

                                if (BundleCareSRSPriceMap.containsKey(item.Apttus_Config2__LineNumber__c) && BundleCareSRSPriceMap.get(item.Apttus_Config2__LineNumber__c).get(1) != null && item.NokiaCPQ_SRSBasePrice__c != (BundleCareSRSPriceMap.get(item.Apttus_Config2__LineNumber__c).get(1) * 0.15).setScale(2, RoundingMode.HALF_UP))
                                {
                                    item.Apttus_Config2__BasePriceOverride__c = (BundleCareSRSPriceMap.get(item.Apttus_Config2__LineNumber__c).get(1) * 0.15).setScale(2, RoundingMode.HALF_UP);
                                    item.NokiaCPQ_SRSBasePrice__c = (BundleCareSRSPriceMap.get(item.Apttus_Config2__LineNumber__c).get(1) * 0.15).setScale(2, RoundingMode.HALF_UP);
                                    item.Apttus_Config2__PricingStatus__c = 'Pending';
                                }
                            }
                        }
                    }
                    if (mode == Apttus_Config2.CustomClass.PricingMode.ADJUSTMENT)
                    {
                        Nokia_CPQ_Constants.PCBBEFOREPRICINGINADJ = Nokia_CPQ_Constants.TRUE_CONSTANT;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            //ExceptionHandler.addException(ex, Nokia_PricingCallBack.class.getName(),'BeforePricing Method and line number is'+ex.getLineNumber());
        }
    }

    /*  Method Name : careSRSCalculationForNSW
	Description : The method calculates Care/SRS for NSW quotes
	*/
    List<Double> careSRSCalculationForNSW(Apttus_Config2__LineItem__c item)
    {
        List<Double> careSRSList = new List<Double>();
        Double sumOfCareItemsRTUOEM = 0.00;
        Double sumOfCareItemsOEMSubscription = 0.00;
        Double sumOfRTUForSSP = 0.00;
        for (Apttus_Config2__LineItem__c optionLineItem :lineItemIRPMapDirect.get(item.Apttus_Config2__LineNumber__c + '_' + item.Apttus_Config2__ChargeType__c))
        {
            String classification = getClassification(optionLineItem);
            String itemType = getItemType(optionLineItem);
            String licenseUsage = getLicenseUsage(optionLineItem);
            if (itemType == 'Hardware' && classification == 'Base')
            {
                if (optionLineItem.Apttus_Config2__BasePriceOverride__c != null && optionLineItem.Apttus_Config2__Quantity__c != null)
                {
                    sumOfCareItemsRTUOEM = sumOfCareItemsRTUOEM + (optionLineItem.Apttus_Config2__BasePriceOverride__c * optionLineItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                }
                else if (optionLineItem.Apttus_Config2__BasePrice__c != null && optionLineItem.Apttus_Config2__Quantity__c != null)
                {
                    sumOfCareItemsRTUOEM = sumOfCareItemsRTUOEM + (optionLineItem.Apttus_Config2__BasePrice__c * optionLineItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                }
            }
            else if (((itemType == 'Software' || itemType == 'Software SI') &&
                     (classification == 'Standard SW (STD)' || classification == 'High Value SW (HVF)' || classification == 'Customer Specific Software (CSS)') &&
                     (licenseUsage == 'Commercial Licence' || licenseUsage == 'Testbed License' || licenseUsage == 'Trial License')) || (itemType == 'Service' && classification == 'Customisation Services'))
            {

                if (optionLineItem.Apttus_Config2__BasePriceOverride__c != null && optionLineItem.Apttus_Config2__Quantity__c != null)
                {
                    sumOfCareItemsOEMSubscription = sumOfCareItemsOEMSubscription + (optionLineItem.Apttus_Config2__BasePriceOverride__c * optionLineItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                }
                else if (optionLineItem.Apttus_Config2__BasePrice__c != null && optionLineItem.Apttus_Config2__Quantity__c != null)
                {
                    sumOfCareItemsOEMSubscription = sumOfCareItemsOEMSubscription + (optionLineItem.Apttus_Config2__BasePrice__c * optionLineItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                }
            }
            else if ((itemType == 'Software' || itemType == 'Software SI') && licenseUsage == 'Commercial Term License' && (classification == 'Standard SW (STD)' || classification == 'High Value SW (HVF)'))
            {
                if (optionLineItem.Apttus_Config2__BasePriceOverride__c != null && optionLineItem.Apttus_Config2__Quantity__c != null)
                {
                    sumOfRTUForSSP = sumOfRTUForSSP + (optionLineItem.Apttus_Config2__BasePriceOverride__c * optionLineItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                }
                else if (optionLineItem.Apttus_Config2__BasePrice__c != null && optionLineItem.Apttus_Config2__Quantity__c != null)
                {
                    sumOfRTUForSSP = sumOfRTUForSSP + (optionLineItem.Apttus_Config2__BasePrice__c * optionLineItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                }
            }
        }
        Double carePrice = (sumOfCareItemsRTUOEM + sumOfCareItemsOEMSubscription + sumOfRTUForSSP * 3);
        Double srsPrice = (sumOfCareItemsOEMSubscription + sumOfRTUForSSP * 3);
        careSRSList.add(carePrice);
        careSRSList.add(srsPrice);

        return careSRSList;
    }

    /*  Method Name : calculateMaintenance_MN_Direct
    Developer : Piyush Potdar
    Description : The method calculates Maintenance per line item for MN direct quotes
    */
    Map<String, double> calculateMaintenance_MN_Direct(Apttus_Config2__LineItem__c item, double totalExtendedMaintY1Price, double totalExtendedMaintY2Price, String partNumber, String configType)
    {
        Map<String, double> maintenanceValueMap = new Map<String, double>();
        double ExtendedMaintY1Price = totalExtendedMaintY1Price;
        double ExtendedMaintY2Price = totalExtendedMaintY2Price;
        double groupQuantity = 1;
        double mainBundleQuantity = 1;

        if (!item.NokiaCPQ_Spare__c)
        {
            String itemType = getItemType(item);
            if (Nokia_CPQ_Constants.NOKIA_OPTION.equalsIgnoreCase(item.Apttus_Config2__LineType__c) &&
            Nokia_CPQ_Constants.NOKIA_STANDALONE.equalsIgnoreCase(configType))
            {
                if (lineItemObjectMap.containsKey(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES + Nokia_CPQ_Constants.NOKIA_UNDERSCORE + item.Apttus_Config2__LineNumber__c))
                    mainBundleQuantity = lineItemObjectMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES + Nokia_CPQ_Constants.NOKIA_UNDERSCORE + item.Apttus_Config2__LineNumber__c).Apttus_Config2__Quantity__c;
                if (item.Apttus_Config2__ExtendedQuantity__c != Null && item.Apttus_Config2__Quantity__c != 0)
                    groupQuantity = item.Apttus_Config2__ExtendedQuantity__c / item.Apttus_Config2__Quantity__c;
            }

            if (itemType != null && itemType.equalsIgnoreCase('Software'))
            {
                if (item.NokiaCPQ_Product_Type__c != null && item.NokiaCPQ_Product_Type__c.equalsIgnoreCase('Controller'))
                {
                    Double basicYear1 = 0.00;
                    Double basicYear2 = 0.00;
                    Double enhanceYear1 = 0.00;
                    Double enhanceYear2 = 0.00;
                    Double enhanceEmergencyYear1 = 0.00;
                    Double enhanceEmergencyYear2 = 0.00;

                    //Software Maintenance Basic = Controller SW IRP * 25%  
                    //Software Maintenance Enhanced = Software Maintenance Basic Price +25%
                    //Software Maintenance Enhanced Emergency = Software Maintenance Enhanced + 25%
                    //multiplication by no of years

                    if (proposalSO.NokiaCPQ_No_of_Years__c != null && String.valueOf(proposalSO.NokiaCPQ_No_of_Years__c).equalsIgnoreCase('1'))
                    {
                        basicYear1 = (item.NokiaCPQ_Extended_IRP__c * 0.25).setScale(2, RoundingMode.HALF_UP);
                        basicYear2 = 0.00;
                        enhanceYear1 = (basicYear1 + (basicYear1 * 0.25)).setScale(2, RoundingMode.HALF_UP);
                        enhanceYear2 = 0.00;
                        enhanceEmergencyYear1 = (enhanceYear1 + (enhanceYear1 * 0.25)).setScale(2, RoundingMode.HALF_UP);
                        enhanceEmergencyYear2 = 0.00;
                    }

                    if (proposalSO.NokiaCPQ_No_of_Years__c != null && (String.valueOf(proposalSO.NokiaCPQ_No_of_Years__c).equalsIgnoreCase('2') || String.valueOf(proposalSO.NokiaCPQ_No_of_Years__c).equalsIgnoreCase('3')))
                    {
                        basicYear1 = (item.NokiaCPQ_Extended_IRP__c * 0.25).setScale(2, RoundingMode.HALF_UP);
                        basicYear2 = ((basicYear1 * (decimal.valueOf(proposalSO.NokiaCPQ_No_of_Years__c) * 0.85)) - basicYear1).setScale(2, RoundingMode.HALF_UP);
                        enhanceYear1 = (basicYear1 + (basicYear1 * 0.25)).setScale(2, RoundingMode.HALF_UP);
                        enhanceYear2 = (((basicYear1 + (basicYear1 * 0.25)) * (decimal.valueOf(proposalSO.NokiaCPQ_No_of_Years__c) * 0.85)) - enhanceYear1).setScale(2, RoundingMode.HALF_UP);
                        enhanceEmergencyYear1 = (enhanceYear1 + (enhanceYear1 * 0.25)).setScale(2, RoundingMode.HALF_UP);
                        enhanceEmergencyYear2 = (((enhanceYear1 + (enhanceYear1 * 0.25)) * (decimal.valueOf(proposalSO.NokiaCPQ_No_of_Years__c) * 0.85)) - enhanceEmergencyYear1).setScale(2, RoundingMode.HALF_UP);
                    }

                    else if (proposalSO.NokiaCPQ_No_of_Years__c != null && (String.valueOf(proposalSO.NokiaCPQ_No_of_Years__c).equalsIgnoreCase('4') || String.valueOf(proposalSO.NokiaCPQ_No_of_Years__c).equalsIgnoreCase('5')))
                    {
                        basicYear1 = (item.NokiaCPQ_Extended_IRP__c * 0.25).setScale(2, RoundingMode.HALF_UP);
                        basicYear2 = ((basicYear1 * (decimal.valueOf(proposalSO.NokiaCPQ_No_of_Years__c) * 0.70)) - basicYear1).setScale(2, RoundingMode.HALF_UP);
                        enhanceYear1 = (basicYear1 + (basicYear1 * 0.25)).setScale(2, RoundingMode.HALF_UP);
                        enhanceYear2 = (((basicYear1 + (basicYear1 * 0.25)) * (decimal.valueOf(proposalSO.NokiaCPQ_No_of_Years__c) * 0.70)) - enhanceYear1).setScale(2, RoundingMode.HALF_UP);
                        enhanceEmergencyYear1 = (enhanceYear1 + (enhanceYear1 * 0.25)).setScale(2, RoundingMode.HALF_UP);
                        enhanceEmergencyYear2 = (((enhanceYear1 + (enhanceYear1 * 0.25)) * (decimal.valueOf(proposalSO.NokiaCPQ_No_of_Years__c) * 0.70)) - enhanceEmergencyYear1).setScale(2, RoundingMode.HALF_UP);
                    }

                    if (proposalSO.NokiaCPQ_Maintenance_Type__c != null && proposalSO.NokiaCPQ_Maintenance_Type__c.equalsIgnoreCase('MN GS TSS Basic'))
                    {
                        item.NokiaCPQ_Maint_Yr1_Extended_Price__c = basicYear1;
                        item.NokiaCPQ_Maint_Yr2_Extended_Price__c = basicYear2;
                    }
                    else if (proposalSO.NokiaCPQ_Maintenance_Type__c != null && proposalSO.NokiaCPQ_Maintenance_Type__c.equalsIgnoreCase('MN GS TSS Enhanced'))
                    {
                        item.NokiaCPQ_Maint_Yr1_Extended_Price__c = enhanceYear1;
                        item.NokiaCPQ_Maint_Yr2_Extended_Price__c = enhanceYear2;
                    }
                    else if (proposalSO.NokiaCPQ_Maintenance_Type__c != null && proposalSO.NokiaCPQ_Maintenance_Type__c.equalsIgnoreCase('MN GS TSS Enhanced Emergency'))
                    {
                        item.NokiaCPQ_Maint_Yr1_Extended_Price__c = enhanceEmergencyYear1;
                        item.NokiaCPQ_Maint_Yr2_Extended_Price__c = enhanceEmergencyYear2;
                    }
                }
            }

            else if (itemType != null && itemType.equalsIgnoreCase('Hardware'))
            {
                if (item.NokiaCPQ_Product_Type__c != null && item.NokiaCPQ_Product_Type__c.equalsIgnoreCase('Access Point'))
                {
                    //multiplication by no of years
                    if (proposalSO.NokiaCPQ_No_of_Years__c != null && String.valueOf(proposalSO.NokiaCPQ_No_of_Years__c).equalsIgnoreCase('1'))
                    {
                        item.NokiaCPQ_Maint_Yr1_Extended_Price__c = ((item.NokiaCPQ_Extended_IRP__c * 0.02) + (item.NokiaCPQ_Extended_IRP__c * 0.053)).setScale(2, RoundingMode.HALF_UP);
                        item.NokiaCPQ_Maint_Yr2_Extended_Price__c = 0.00;
                    }

                    else if (proposalSO.NokiaCPQ_No_of_Years__c != null && (String.valueOf(proposalSO.NokiaCPQ_No_of_Years__c).equalsIgnoreCase('2') ||
                                     String.valueOf(proposalSO.NokiaCPQ_No_of_Years__c).equalsIgnoreCase('3')))
                    {
                        item.NokiaCPQ_Maint_Yr1_Extended_Price__c = ((item.NokiaCPQ_Extended_IRP__c * 0.02) + (item.NokiaCPQ_Extended_IRP__c * 0.053)).setScale(2, RoundingMode.HALF_UP);
                        item.NokiaCPQ_Maint_Yr2_Extended_Price__c = ((item.NokiaCPQ_Extended_IRP__c * 0.02 * ((decimal.valueOf(proposalSO.NokiaCPQ_No_of_Years__c) * 1.00) - 1)) + (item.NokiaCPQ_Extended_IRP__c * 0.053 * ((decimal.valueOf(proposalSO.NokiaCPQ_No_of_Years__c) * 0.85) - 1))).setScale(2, RoundingMode.HALF_UP);
                    }

                    else if (proposalSO.NokiaCPQ_No_of_Years__c != null && (String.valueOf(proposalSO.NokiaCPQ_No_of_Years__c).equalsIgnoreCase('4') ||
                                     String.valueOf(proposalSO.NokiaCPQ_No_of_Years__c).equalsIgnoreCase('5')))
                    {
                        item.NokiaCPQ_Maint_Yr1_Extended_Price__c = ((item.NokiaCPQ_Extended_IRP__c * 0.02) + (item.NokiaCPQ_Extended_IRP__c * 0.053)).setScale(2, RoundingMode.HALF_UP);
                        item.NokiaCPQ_Maint_Yr2_Extended_Price__c = ((item.NokiaCPQ_Extended_IRP__c * 0.02 * ((decimal.valueOf(proposalSO.NokiaCPQ_No_of_Years__c) * 0.85) - 1)) + (item.NokiaCPQ_Extended_IRP__c * 0.053 * ((decimal.valueOf(proposalSO.NokiaCPQ_No_of_Years__c) * 0.70) - 1))).setScale(2, RoundingMode.HALF_UP);
                    }
                }

                else if (item.NokiaCPQ_Product_Type__c != null && item.NokiaCPQ_Product_Type__c.equalsIgnoreCase('Controller'))
                {
                    //multiplication by no of years
                    if (proposalSO.NokiaCPQ_No_of_Years__c != null && String.valueOf(proposalSO.NokiaCPQ_No_of_Years__c).equalsIgnoreCase('1'))
                    {
                        item.NokiaCPQ_Maint_Yr1_Extended_Price__c = (item.NokiaCPQ_Extended_IRP__c * 0.02).setScale(2, RoundingMode.HALF_UP);
                        item.NokiaCPQ_Maint_Yr2_Extended_Price__c = 0.00;
                    }

                    else if (proposalSO.NokiaCPQ_No_of_Years__c != null && (String.valueOf(proposalSO.NokiaCPQ_No_of_Years__c).equalsIgnoreCase('2') ||
                                String.valueOf(proposalSO.NokiaCPQ_No_of_Years__c).equalsIgnoreCase('3')))
                    {
                        item.NokiaCPQ_Maint_Yr1_Extended_Price__c = (item.NokiaCPQ_Extended_IRP__c * 0.02).setScale(2, RoundingMode.HALF_UP);
                        item.NokiaCPQ_Maint_Yr2_Extended_Price__c = (item.NokiaCPQ_Extended_IRP__c * 0.02 * (decimal.valueOf(proposalSO.NokiaCPQ_No_of_Years__c) - 1)).setScale(2, RoundingMode.HALF_UP);
                    }

                    else if (proposalSO.NokiaCPQ_No_of_Years__c != null && (String.valueOf(proposalSO.NokiaCPQ_No_of_Years__c).equalsIgnoreCase('4') ||
                                String.valueOf(proposalSO.NokiaCPQ_No_of_Years__c).equalsIgnoreCase('5')))
                    {
                        item.NokiaCPQ_Maint_Yr1_Extended_Price__c = (item.NokiaCPQ_Extended_IRP__c * 0.02).setScale(2, RoundingMode.HALF_UP);
                        item.NokiaCPQ_Maint_Yr2_Extended_Price__c = (item.NokiaCPQ_Extended_IRP__c * 0.02 * ((decimal.valueOf(proposalSO.NokiaCPQ_No_of_Years__c) * 0.85) - 1)).setScale(2, RoundingMode.HALF_UP);
                    }
                }

            }

            if (item.NokiaCPQ_Product_Type__c != null && item.NokiaCPQ_Product_Type__c.equalsIgnoreCase('Third Party Wavespot'))
            {
                if (String.isNotBlank(partNumber))
                {
                    item.NokiaCPQ_Maint_Yr1_Extended_Price__c = (item.NokiaCPQ_Extended_CLP__c * 0.18 * 0.85).setScale(2, RoundingMode.HALF_UP);
                    item.NokiaCPQ_Maint_Yr2_Extended_Price__c = (item.NokiaCPQ_Extended_CLP__c * 0.18 * 0.85 * (decimal.valueOf(proposalSO.NokiaCPQ_No_of_Years__c) - 1)).setScale(2, RoundingMode.HALF_UP);
                }
            }
        }

        else
        {
            item.NokiaCPQ_Maint_Yr1_Extended_Price__c = 0.00;
            item.NokiaCPQ_Maint_Yr2_Extended_Price__c = 0.00;
        }

        ExtendedMaintY1Price = ExtendedMaintY1Price + (item.NokiaCPQ_Maint_Yr1_Extended_Price__c * groupQuantity * mainBundleQuantity).setScale(2, RoundingMode.HALF_UP);
        ExtendedMaintY2Price = ExtendedMaintY2Price + (item.NokiaCPQ_Maint_Yr2_Extended_Price__c * groupQuantity * mainBundleQuantity).setScale(2, RoundingMode.HALF_UP);

        maintenanceValueMap.put('ExtendedMaintY1Price', ExtendedMaintY1Price);
        maintenanceValueMap.put('ExtendedMaintY2Price', ExtendedMaintY2Price);

        return maintenanceValueMap;
    }

    //R-6508
    /* Method Name   : calculateMaintenance_EP_Direct
    * Developer	  : Accenture
    * Description	:  The method calculates Maintenance per line item for Direct Enterprise Quotes */
    Map<String, double> calculateMaintenance_EP_Direct(Apttus_Config2__LineItem__c item, double totalExtendedMaintY1Price, double totalExtendedMaintY2Price, String partNumber)
    {
        Map<String, double> maintenanceValueMap = new Map<String, double>();
        CPQ_Maintenance_and_SSP_Rule__c nokiaSSPSRSProdDiscount_EP = new CPQ_Maintenance_and_SSP_Rule__c();
        String itemName = String.valueOf(item.Apttus_Config2__ChargeType__c);

        double ExtendedMaintY1Price = totalExtendedMaintY1Price;
        double ExtendedMaintY2Price = totalExtendedMaintY2Price;
        Integer quantityBundle = 1;

        if (partNumber != null && !partNumber.contains(Nokia_CPQ_Constants.MAINTY1CODE) &&
    !partNumber.contains(Nokia_CPQ_Constants.MAINTY2CODE) &&
    !partNumber.contains(Nokia_CPQ_Constants.SSPCODE) &&
    !partNumber.contains(Nokia_CPQ_Constants.SRS))
        {
            if (item.Apttus_Config2__LineType__c != null && Nokia_CPQ_Constants.NOKIA_OPTION.equals(String.valueOf(item.Apttus_Config2__LineType__c)))
            {
                if (this.lineItemObjectMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES + Nokia_CPQ_Constants.NOKIA_UNDERSCORE + item.Apttus_Config2__LineNumber__c) != null)
                {
                    if (this.lineItemObjectMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES + Nokia_CPQ_Constants.NOKIA_UNDERSCORE + item.Apttus_Config2__LineNumber__c).Apttus_Config2__Quantity__c != null)
                    {
                        quantityBundle = Integer.valueOf(this.lineItemObjectMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES + Nokia_CPQ_Constants.NOKIA_UNDERSCORE + item.Apttus_Config2__LineNumber__c).Apttus_Config2__Quantity__c.round(System.RoundingMode.CEILING));
                    }
                }
            }
        }

        if (item.NokiaCPQ_Extended_IRP__c != null && item.Nokia_Maint_Y1_Per__c != null && item.Nokia_Maint_Y2_Per__c != null)
        {
            item.NokiaCPQ_Maint_Yr1_Extended_Price__c = ((item.NokiaCPQ_Extended_IRP__c * item.Nokia_Maint_Y1_Per__c * 0.01).setScale(2, RoundingMode.HALF_UP)) * quantityBundle;
            item.NokiaCPQ_Maint_Yr2_Extended_Price__c = ((item.NokiaCPQ_Extended_IRP__c * item.Nokia_Maint_Y2_Per__c * 0.01).setScale(2, RoundingMode.HALF_UP)) * quantityBundle;
        }

        ExtendedMaintY1Price = ExtendedMaintY1Price + (item.NokiaCPQ_Maint_Yr1_Extended_Price__c).setScale(2, RoundingMode.HALF_UP);
        ExtendedMaintY2Price = ExtendedMaintY2Price + (item.NokiaCPQ_Maint_Yr2_Extended_Price__c).setScale(2, RoundingMode.HALF_UP);

        maintenanceValueMap.put('ExtendedMaintY1Price', ExtendedMaintY1Price);
        maintenanceValueMap.put('ExtendedMaintY2Price', ExtendedMaintY2Price);
        return maintenanceValueMap;

    }

    /* Method Name   : onPriceItemSet
    * Developer	  : Apttus
    * Description	: OOTB method, overridden and put nokia pricing logic */
    void onPriceItemSet(Apttus_Config2__PriceListItem__c itemSO, Apttus_Config2.LineItem lineItemMO)
    {
        try
        {
            methodSequence++;

            Apttus_Config2__LineItem__c item = lineItemMO.getLineItemSO();

            String partNumber = getPartNumber(item);
            String productDiscountCat = getProductDiscountCategory(item);
            String configType = getConfigType(item);

            //Logic from Workflow: Account Market Update
            if (this.proposalSO != null && (this.proposalSO.Quote_Type__c.equalsIgnoreCase(Nokia_CPQ_Constants.QUOTE_TYPE_DIRECTCPQ) || this.proposalSO.Quote_Type__c.equalsIgnoreCase(Nokia_CPQ_Constants.QUOTE_TYPE_INDIRECTCPQ)))
            {
                item.NokiaCPQ_Account_Region__c = this.proposalSO.Apttus_Proposal__Account__r.GEOLevel1ID__c;
            }

            if (this.proposalSO != null && this.proposalSO.Quote_Type__c.equalsIgnoreCase(Nokia_CPQ_Constants.QUOTE_TYPE_DIRECTCPQ))
            {
                //Replacing item.Portfolio_from_Quote_Line_Item__c with 'this.proposalSO.NokiaCPQ_Portfolio__c'
                if (this.proposalSO.NokiaCPQ_Portfolio__c == Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING
                   && configType == Nokia_CPQ_Constants.NOKIA_STANDALONE &&
                   item.Apttus_Config2__LineType__c == Nokia_CPQ_Constants.NOKIA_OPTION)
                {
                    item.NokiaCPQ_Is_SI__c = true;
                }

                //R-6456,6667 update QTC Line Item with Price point info from Product Extension and BG, BU info from Product2
                if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase('QTC'))
                {
                    item.NokiaCPQ_Org__c = this.proposalSO.Apttus_Proposal__Account__r.L7Name__c;
                    item.NokiaCPQ_BU__c = item.Apttus_Config2__ProductId__r.Family;
                    item.NokiaCPQ_BG__c = item.Apttus_Config2__ProductId__r.Business_Group__c;

                    if (Product_extensitonmap.get(item.Apttus_Config2__ProductId__c) != null)
                    {
                        item.NokiaCPQ_Custom_Bid__c = Product_extensitonmap.get(item.Apttus_Config2__ProductId__c).Custom_Bid__c;
                        item.NokiaCPQ_Floor_Price__c = Product_extensitonmap.get(item.Apttus_Config2__ProductId__c).Floor_Price__c == null ? null : Product_extensitonmap.get(item.Apttus_Config2__ProductId__c).Floor_Price__c;
                        item.NokiaCPQ_Market_Price__c = Product_extensitonmap.get(item.Apttus_Config2__ProductId__c).Market_Price__c == null ? null : Product_extensitonmap.get(item.Apttus_Config2__ProductId__c).Market_Price__c;
                        item.Product_Extension__c = Product_extensitonmap.get(item.Apttus_Config2__ProductId__c).Id;
                    }
                    else
                    {
                        item.NokiaCPQ_Floor_Price__c = null;
                        item.NokiaCPQ_Market_Price__c = null;
                    }

                }

                //R-6508,6510 Logic to stamp Maint Y1, Y2, SSP, SRS rates onto line item
                ////R-6500 update Enterprise Line Item with FLoor Price info from Product Extension
                if (this.proposalSO.Is_List_Price_Only__c != null && this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_IP_ROUTING))
                {
                    item.Is_List_Price_Only__c = this.proposalSO.Is_List_Price_Only__c;
                    if (this.proposalSO.Is_List_Price_Only__c == false)
                    {
                        if (item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES)
                    && Product_extensitonmap.get(item.Apttus_Config2__ProductId__r.Id) != null &&
                     Product_extensitonmap.get(item.Apttus_Config2__ProductId__r.Id).Floor_Price__c != Null)
                        {
                            item.NokiaCPQ_Floor_Price__c = Product_extensitonmap.get(item.Apttus_Config2__ProductId__r.Id).Floor_Price__c == null ? null : Product_extensitonmap.get(item.Apttus_Config2__ProductId__r.Id).Floor_Price__c;
                        }
                        else if (item.Apttus_Config2__LineType__c.equalsIgnoreCase('Option')
                    && Product_extensitonmap.get(item.Apttus_Config2__OptionId__r.Id) != null &&
                     Product_extensitonmap.get(item.Apttus_Config2__OptionId__r.Id).Floor_Price__c != Null)
                        {
                            item.NokiaCPQ_Floor_Price__c = Product_extensitonmap.get(item.Apttus_Config2__OptionId__r.Id).Floor_Price__c == null ? null : Product_extensitonmap.get(item.Apttus_Config2__OptionId__r.Id).Floor_Price__c;
                        }
                        else
                        {
                            item.NokiaCPQ_Floor_Price__c = null;
                        }
                    }
                }

                CPQ_Maintenance_and_SSP_Rule__c nokiaSSPSRSProdDiscount_EP = new CPQ_Maintenance_and_SSP_Rule__c();

                Double unlimitedSSP = 0.00;
                Double biennialSSP = 0.00;
                Double unlimitedSRS = 0.00;
                Double biennialSRS = 0.00;
                Double serviceRateY1 = 0.00;
                Double serviceRateY2 = 0.00;

                if (Nokia_CPQ_Constants.NOKIA_IP_ROUTING.equalsIgnoreCase(this.proposalSO.NokiaCPQ_Portfolio__c) && !this.proposalSO.Is_List_Price_Only__c && !item.is_Custom_Product__c &&
                    partNumber != null && !partNumber.contains(Nokia_CPQ_Constants.MAINTY1CODE) &&
                    !partNumber.contains(Nokia_CPQ_Constants.MAINTY2CODE) &&
                    !partNumber.contains(Nokia_CPQ_Constants.SSPCODE) &&
                    !partNumber.contains(Nokia_CPQ_Constants.SRS))
                {


                    if (maintenanceSSPRuleMap_EP != null && maintenanceSSPRuleMap_EP.get(this.proposalSO.NokiaCPQ_Maintenance_Type__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + productDiscountCat) != null)
                    {

                        nokiaSSPSRSProdDiscount_EP = maintenanceSSPRuleMap_EP.get(this.proposalSO.NokiaCPQ_Maintenance_Type__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + productDiscountCat);
                        // System.debug('nokiaSSPSRSProdDiscount_EP'+nokiaSSPSRSProdDiscount_EP);

                        if (nokiaSSPSRSProdDiscount_EP != null)
                        {
                            //SSP Rate assignment
                            if (nokiaSSPSRSProdDiscount_EP.Unlimited_SSP_Discount__c == NULL)
                            {
                                unlimitedSSP = 0.0;
                            }
                            else
                            {
                                unlimitedSSP = nokiaSSPSRSProdDiscount_EP.Unlimited_SSP_Discount__c;
                            }
                            if (nokiaSSPSRSProdDiscount_EP.Biennial_SSP_Discount__c == NULL)
                            {
                                biennialSSP = 0.0;
                            }
                            else
                            {
                                biennialSSP = nokiaSSPSRSProdDiscount_EP.Biennial_SSP_Discount__c;
                            }
                            // SRS Rate assignment 
                            if (nokiaSSPSRSProdDiscount_EP.Unlimited_SRS_Discount__c == NULL)
                            {
                                unlimitedSRS = 0.0;
                            }
                            else
                            {
                                unlimitedSRS = nokiaSSPSRSProdDiscount_EP.Unlimited_SRS_Discount__c;
                            }
                            if (nokiaSSPSRSProdDiscount_EP.Biennial_SRS_Discount__c == NULL)
                            {
                                biennialSRS = 0.0;
                            }
                            else
                            {
                                biennialSRS = nokiaSSPSRSProdDiscount_EP.Biennial_SRS_Discount__c;
                            }

                            // Year1, Year 2 Rate assignment
                            if (nokiaSSPSRSProdDiscount_EP.Maintenance_Category__c == NULL)
                            {
                                serviceRateY1 = 0.0;
                            }
                            else
                            {
                                serviceRateY1 = nokiaSSPSRSProdDiscount_EP.Service_Rate_Y1__c;
                            }
                            if (nokiaSSPSRSProdDiscount_EP.Maintenance_Category__c == NULL)
                            {
                                serviceRateY2 = 0.0;
                            }
                            else
                            {
                                serviceRateY2 = nokiaSSPSRSProdDiscount_EP.Service_Rate_Y2__c;
                            }

                        }
                    }


                    if (this.proposalSO.NokiaCPQ_Maintenance_Type__c != null)
                    {
                        item.Nokia_Maint_Y1_Per__c = serviceRateY1 * 100;
                        item.Nokia_Maint_Y2_Per__c = serviceRateY2 * 100;
                    }
                    //  System.debug('isSSP>>>>'+item.Apttus_Config2__ProductId__r.IsSSP__c);
                    if (!item.Apttus_Config2__ProductId__r.IsSSP__c && this.proposalSO.NokiaCPQ_Maintenance_Type__c != null && this.proposalSO.NokiaCPQ_SSP_Level__c != null && Nokia_CPQ_Constants.NOKIA_UNLIMITED.equalsIgnoreCase(this.proposalSO.NokiaCPQ_SSP_Level__c))
                    {
                        item.NokiaCPQ_SSP_Rate__c = unlimitedSSP * 100;
                    }
                    else if (!item.Apttus_Config2__ProductId__r.IsSSP__c && this.proposalSO.NokiaCPQ_Maintenance_Type__c != null && this.proposalSO.NokiaCPQ_SSP_Level__c != null && Nokia_CPQ_Constants.NOKIA_BIENNIAL.equalsIgnoreCase(this.proposalSO.NokiaCPQ_SSP_Level__c))
                    {
                        item.NokiaCPQ_SSP_Rate__c = biennialSSP * 100;
                    }

                    if (item.Apttus_Config2__ProductId__r.IsSSP__c && this.proposalSO.NokiaCPQ_Maintenance_Type__c != null && this.proposalSO.NokiaCPQ_SRS_Level__c != null && Nokia_CPQ_Constants.NOKIA_UNLIMITED.equalsIgnoreCase(this.proposalSO.NokiaCPQ_SRS_Level__c))
                    {
                        item.NokiaCPQ_SRS_Rate__c = unlimitedSRS * 100;
                    }
                    else if (item.Apttus_Config2__ProductId__r.IsSSP__c && this.proposalSO.NokiaCPQ_Maintenance_Type__c != null && this.proposalSO.NokiaCPQ_SRS_Level__c != null && Nokia_CPQ_Constants.NOKIA_BIENNIAL.equalsIgnoreCase(this.proposalSO.NokiaCPQ_SRS_Level__c))
                    {
                        item.NokiaCPQ_SRS_Rate__c = biennialSRS * 100;
                    }


                }
                //6508 End

                if (mode == Apttus_Config2.CustomClass.PricingMode.BASEPRICE)
                {
                    if (item.Apttus_Config2__PriceListId__c == item.Apttus_Config2__PriceListItemId__r.Apttus_Config2__PriceListId__c)
                    {
                        if (item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_OPTION) && this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_SOFTWARE) && configType.equalsIgnoreCase('Standalone') && !mainBundleList.isEmpty() && !mainBundleList.contains(String.valueOf(item.Apttus_Config2__ParentBundleNumber__c)))
                        {
                            item.NokiaCPQ_Unitary_IRP__c = 0.00;
                        }
                        else
                        {
                            if (!item.is_Custom_Product__c)
                            {
                                item.NokiaCPQ_Unitary_IRP__c = (itemSO.Apttus_Config2__ListPrice__c * (this.proposalSO.exchange_rate__c)).setScale(5, RoundingMode.HALF_UP);
                            }
                        }

                        //Setting the Cost
                        if (!portfolioSettingList.isEmpty() && portfolioSettingList[0].Cost_Calculation_In_PCB__c == true)
                        {
                            item.NokiaCPQ_Unitary_Cost__c = 0.00;
                            //ADDED BY PRIYANKA
                            if (itemSO.Apttus_Config2__Cost__c != null)
                            {
                                if (!item.Advanced_pricing_done__c)
                                {
                                    item.NokiaCPQ_Unitary_Cost__c = ((itemSO.Apttus_Config2__Cost__c / defaultExchangeRate.get(0).ConversionRate) * (this.proposalSO.exchange_rate__c)).setScale(5, RoundingMode.HALF_UP);
                                }
                                else if (item.Advanced_pricing_done__c && item.NokiaCPQ_Unitary_Cost__c != null)
                                {
                                    item.NokiaCPQ_Unitary_Cost__c = ((item.NokiaCPQ_Unitary_Cost__c / defaultExchangeRate.get(0).ConversionRate) * (this.proposalSO.exchange_rate__c)).setScale(5, RoundingMode.HALF_UP);
                                }
                            }
                        }
                        else if (!portfolioSettingList.isEmpty() && item.NokiaCPQ_Unitary_Cost_Initial__c != null)
                        {
                            item.NokiaCPQ_Unitary_Cost__c = ((item.NokiaCPQ_Unitary_Cost_Initial__c / defaultExchangeRate.get(0).ConversionRate) * (this.proposalSO.exchange_rate__c)).setScale(5, RoundingMode.HALF_UP);
                        }
                    }
                    else if (productPriceMap != null)
                    {
                        if (item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_OPTION) && this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_SOFTWARE) && configType.equalsIgnoreCase('Standalone') && !mainBundleList.isEmpty() && !mainBundleList.contains(String.valueOf(item.Apttus_Config2__ParentBundleNumber__c)))
                        {
                            item.NokiaCPQ_Unitary_IRP__c = 0.00;
                        }
                        else
                        {
                            if (item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES))
                            {
                                item.NokiaCPQ_Unitary_IRP__c = (productPriceMap.get(item.Apttus_Config2__ProductId__c) * (this.proposalSO.exchange_rate__c)).setScale(5, RoundingMode.HALF_UP);
                            }
                            else
                            {
                                item.NokiaCPQ_Unitary_IRP__c = (productPriceMap.get(item.Apttus_Config2__OptionId__c) * (this.proposalSO.exchange_rate__c)).setScale(5, RoundingMode.HALF_UP);
                            }
                        }

                        //Setting the Cost
                        if (!portfolioSettingList.isEmpty() && portfolioSettingList[0].Cost_Calculation_In_PCB__c == true)
                        {
                            item.NokiaCPQ_Unitary_Cost__c = 0.00;
                            if (item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES) && productCostMap.get(item.Apttus_Config2__ProductId__c) != null)
                            {
                                //ADDED BY PRIYANKA 
                                if (item.Advanced_pricing_done__c)
                                {
                                    item.NokiaCPQ_Unitary_Cost__c = ((item.NokiaCPQ_Unitary_Cost__c / defaultExchangeRate.get(0).ConversionRate) * (this.proposalSO.exchange_rate__c)).setScale(5, RoundingMode.HALF_UP);
                                }
                                else
                                {
                                    item.NokiaCPQ_Unitary_Cost__c = (productCostMap.get(item.Apttus_Config2__ProductId__c) * (this.proposalSO.exchange_rate__c)).setScale(5, RoundingMode.HALF_UP);
                                }
                            }
                            else if (!item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES) && productCostMap.get(item.Apttus_Config2__OptionId__c) != null)
                            {
                                //ADDED BY PRIYANKA 
                                if (item.Advanced_pricing_done__c)
                                {
                                    item.NokiaCPQ_Unitary_Cost__c = ((item.NokiaCPQ_Unitary_Cost__c / defaultExchangeRate.get(0).ConversionRate) * (this.proposalSO.exchange_rate__c)).setScale(5, RoundingMode.HALF_UP);
                                }
                                else
                                {
                                    item.NokiaCPQ_Unitary_Cost__c = (productCostMap.get(item.Apttus_Config2__OptionId__c) * (this.proposalSO.exchange_rate__c)).setScale(5, RoundingMode.HALF_UP);
                                }
                            }
                        }
                        else if (!portfolioSettingList.isEmpty() && item.NokiaCPQ_Unitary_Cost_Initial__c != null)
                        {
                            item.NokiaCPQ_Unitary_Cost__c = ((item.NokiaCPQ_Unitary_Cost_Initial__c / defaultExchangeRate.get(0).ConversionRate) * (this.proposalSO.exchange_rate__c)).setScale(5, RoundingMode.HALF_UP);
                        }
                    }

                    item.NokiaCPQ_Unitary_IRP__c = item.NokiaCPQ_Unitary_IRP__c.setScale(2, RoundingMode.HALF_UP);
                    if (item.NokiaCPQ_Unitary_Cost__c != null)
                        item.NokiaCPQ_Unitary_Cost__c = item.NokiaCPQ_Unitary_Cost__c.setScale(2, RoundingMode.HALF_UP);

                    if (!mainBundleList.isEmpty() && mainBundleList.contains(String.valueOf(item.Apttus_Config2__ParentBundleNumber__c)))
                    {
                        item.NokiaCPQ_Is_Direct_Option__c = true;
                    }

                    //The piece of code mentioned below is used fro addingm Maintenance line on MN Direct quotes
                    if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING))
                    {
                        for (NokiaCPQ_MN_Direct_Product_Map__mdt MN_Direct_rec : MN_Direct_Products_List)
                        {
                            if (MN_Direct_rec.NokiaCPQ_Product_Code__c.contains(partNumber))
                            {
                                item.NokiaCPQ_Product_Type__c = MN_Direct_rec.NokiaCPQ_Product_Type__c;
                            }
                        }
                    }
                }
            }

            //DSI-811 for DS Team Option Quantity to be calculated from the bundle
            if (this.proposalSO != null && this.proposalSO.Quote_Type__c.equalsIgnoreCase(CSWXGlobalConstant.Direct_DS))
            {
                Integer quantityBundle = 1;
                if (item.Apttus_Config2__LineType__c != null && Nokia_CPQ_Constants.NOKIA_OPTION.equals(String.valueOf(item.Apttus_Config2__LineType__c)))
                {
                    if (this.lineItemObjectMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES + Nokia_CPQ_Constants.NOKIA_UNDERSCORE + item.Apttus_Config2__LineNumber__c) != null)
                    {
                        if (this.lineItemObjectMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES + Nokia_CPQ_Constants.NOKIA_UNDERSCORE + item.Apttus_Config2__LineNumber__c).Apttus_Config2__Quantity__c != null)
                        {
                            quantityBundle = Integer.valueOf(this.lineItemObjectMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES + Nokia_CPQ_Constants.NOKIA_UNDERSCORE + item.Apttus_Config2__LineNumber__c).Apttus_Config2__Quantity__c.round(System.RoundingMode.CEILING));
                            item.Total_Option_Quantity__c = quantityBundle * item.Apttus_Config2__Quantity__c;

                        }
                    }
                }
            }

            //4188 ends

            if (this.proposalSO != null && this.proposalSO.Quote_Type__c.equalsIgnoreCase(Nokia_CPQ_Constants.QUOTE_TYPE_INDIRECTCPQ))
            {
                if (mode == Apttus_Config2.CustomClass.PricingMode.BASEPRICE)
                {
                    if (item.is_Custom_Product__c == true)
                    {
                        String[] arrbasrValues = item.CustomProductValue__c.split(';');
                        if (arrbasrValues.size() > 0)
                        {
                            item.Apttus_Config2__BasePriceOverride__c = decimal.valueof(arrbasrValues[2]);
                            item.Apttus_Config2__BasePrice__c = decimal.valueof(arrbasrValues[1]);
                            item.Apttus_Config2__ListPrice__c = decimal.valueof(arrbasrValues[0]);
                            itemSO.Apttus_Config2__ListPrice__c = decimal.valueof(arrbasrValues[0]);
                        }
                    }

                    /*Start Performance Formula Fields*/
                    String dummyBundleLI = '';
                    if ('Product/Service'.equalsIgnoreCase(item.Apttus_Config2__LineType__c))
                    {
                        dummyBundleLI = String.valueOf(item.Apttus_Config2__ProductId__r.Is_Dummy_Bundle_CPQ__c);
                    }
                    else
                    {
                        dummyBundleLI = String.valueOf(item.Apttus_Config2__OptionId__r.Is_Dummy_Bundle_CPQ__c);
                    }
                    /*End Performance Formula Fields*/

                    if (Nokia_CPQ_Constants.DEFAULTPENDING.equals(String.valueOf(item.Apttus_Config2__ConfigStatus__c)) && Nokia_CPQ_Constants.NOKIA_YES.equals(dummyBundleLI) && Nokia_CPQ_Constants.NOKIA_OPTION.equals(String.valueOf(item.Apttus_Config2__LineType__c)))
                    {
                        item.Apttus_Config2__ConfigStatus__c = Nokia_CPQ_Constants.COMPLETE_MSG;
                    }
                    if (item != null && item.Source__c != null && (item.Source__c.equalsignorecase(Nokia_CPQ_Constants.NOKIA_EPT) || item.Source__c.equalsignorecase(GlobalConstants.WAVELITESOURCE)))
                    { //Added by RG for Wavelite check
                        item.Apttus_Config2__IsReadOnly__c = True;
                    }

                    //Req-5817
                    if (item.Apttus_Config2__ConfigurationId__r.Apttus_QPConfig__Proposald__r.NokiaProductAccreditation__r.NokiaCPQ_Incoterm_Percentage__c != null)
                    {
                        item.NokiaCPQ_IncotermNew__c = item.Apttus_Config2__ConfigurationId__r.Apttus_QPConfig__Proposald__r.NokiaProductAccreditation__r.NokiaCPQ_Incoterm_Percentage__c;
                    }

                    //Req : 5260
                    if (item.Apttus_Config2__PriceListId__c != itemSO.Apttus_Config2__PriceListId__c)
                    {
                        if (mapPliType.get(itemSO.Apttus_Config2__PriceListId__c) == 'Indirect Market')
                            item.Is_Contract_Pricing_2__c = false;
                        else
                            item.Is_Contract_Pricing_2__c = true;
                    }

                    String itemName = String.valueOf(item.Apttus_Config2__ChargeType__c);
                    if (!(itemName.contains(Nokia_CPQ_Constants.NOKIA_ACCRED_TYPE_MAINTENANCE) || itemName.contains(Nokia_CPQ_Constants.NOKIA_PRODUCT_NAME_SSP) || itemName.contains(Nokia_CPQ_Constants.NOKIA_PRODUCT_NAME_SRS)))
                    {
                        Integer quantityBundle = 1;
                        if (item.Apttus_Config2__LineType__c != null && Nokia_CPQ_Constants.NOKIA_OPTION.equals(String.valueOf(item.Apttus_Config2__LineType__c)))
                        {
                            if (this.lineItemObjectMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES + Nokia_CPQ_Constants.NOKIA_UNDERSCORE + item.Apttus_Config2__LineNumber__c) != null)
                            {

                                if (this.lineItemObjectMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES + Nokia_CPQ_Constants.NOKIA_UNDERSCORE + item.Apttus_Config2__LineNumber__c).Apttus_Config2__Quantity__c != null)
                                {
                                    quantityBundle = Integer.valueOf(this.lineItemObjectMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES + Nokia_CPQ_Constants.NOKIA_UNDERSCORE + item.Apttus_Config2__LineNumber__c).Apttus_Config2__Quantity__c.round(System.RoundingMode.CEILING));
                                    item.Total_Option_Quantity__c = quantityBundle * item.Apttus_Config2__Quantity__c;
                                }
                            }
                        }
                        if (this.proposalSO.NokiaProductAccreditation__c != null)
                        {
                            item.NokiaCPQAccreditationType__c = this.proposalSO.NokiaProductAccreditation__r.Pricing_Accreditation__c;
                            item.Nokia_Pricing_Cluster__c = this.proposalSO.NokiaProductAccreditation__r.Pricing_Cluster__c;
                        }

                        if (this.proposalSO.NokiaCPQ_Maintenance_Accreditation__c != null)
                        {
                            if (this.proposalSO.NokiaCPQ_LEO_Discount__c)
                            {
                                item.Nokia_Maintenance_Level__c = Nokia_CPQ_Constants.NOKIA_LEO;
                            }
                            //Heema Change for Defect 14394 start
                            else if (this.proposalSO.NokiaCPQ_Maintenance_Level__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_YES))
                            {
                                item.Nokia_Maintenance_Level__c = Nokia_CPQ_Constants.Nokia_Brand;
                            }
                            //Heema Change : Defect 14394 End
                            else
                            {
                                item.Nokia_Maintenance_Level__c = this.proposalSO.NokiaCPQ_Maintenance_Accreditation__r.Pricing_Accreditation__c;
                            }
                            item.Nokia_Maint_Pricing_Cluster__c = this.proposalSO.NokiaCPQ_Maintenance_Accreditation__r.Pricing_Cluster__c;
                        }

                        //Varsha: start: Changes for req #4920 : added check for Tier Discount applicable
                        if (!tierDiscountRuleMap.isEmpty() && tierDiscountRuleMap.get(Nokia_CPQ_Constants.NOKIA_MAINTENANCE_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + String.valueOf(item.Nokia_Maintenance_Level__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c)) != null &&
                        tierDiscountRuleMap.get(Nokia_CPQ_Constants.NOKIA_MAINTENANCE_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + String.valueOf(item.Nokia_Maintenance_Level__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c)).size() > 0 && !this.sspSRSDefaultsList.isEmpty() && sspSRSDefaultsList[0].Tier_Discount_Applicable__c == true)
                        {
                            item.NokiaCPQ_Maint_Accreditation_Discount__c = tierDiscountRuleMap.get(Nokia_CPQ_Constants.NOKIA_MAINTENANCE_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + String.valueOf(item.Nokia_Maintenance_Level__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c)).get(0);
                        }
                        else
                        {
                            item.NokiaCPQ_Maint_Accreditation_Discount__c = 0;
                        }

                        if (!tierDiscountRuleMap.isEmpty() && tierDiscountRuleMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + String.valueOf(item.NokiaCPQAccreditationType__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c)) != null &&
                        tierDiscountRuleMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + String.valueOf(item.NokiaCPQAccreditationType__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c)).size() > 0 && !this.sspSRSDefaultsList.isEmpty() && sspSRSDefaultsList[0].Tier_Discount_Applicable__c == true)
                        {
                            item.NokiaCPQ_Accreditation_Discount__c = tierDiscountRuleMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + String.valueOf(item.NokiaCPQAccreditationType__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c)).get(0);
                        }
                        else
                        {
                            item.NokiaCPQ_Accreditation_Discount__c = 0;
                        }
                        //Varsha: end: Changes for req #4920 : added check for Tier Discount applicable

                        if (proposalSO.NokiaCPQ_IsPMA__c && !this.sspSRSDefaultsList.isEmpty() && sspSRSDefaultsList[0].AccountLevel_Discount_Applicable__c == true)
                        {
                            if (this.proposalSO.Apttus_Proposal__Account__r.NokiaCPQ_Renewal__c)
                            {
                                if (!tierDiscountRuleMap.isEmpty() && tierDiscountRuleMap.get(Nokia_CPQ_Constants.INCENTIVES_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + Nokia_CPQ_Constants.RENEWAL_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c)) != null && tierDiscountRuleMap.get(Nokia_CPQ_Constants.INCENTIVES_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + Nokia_CPQ_Constants.RENEWAL_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c)).size() > 0)
                                {
                                    item.NokiaCPQ_Renewal_Per__c = tierDiscountRuleMap.get(Nokia_CPQ_Constants.INCENTIVES_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + Nokia_CPQ_Constants.RENEWAL_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c)).get(0);
                                }
                            }
                            if (this.proposalSO.Apttus_Proposal__Account__r.NokiaCPQ_Attachment__c)
                            {
                                if (!tierDiscountRuleMap.isEmpty() && tierDiscountRuleMap.get(Nokia_CPQ_Constants.INCENTIVES_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + Nokia_CPQ_Constants.ATTACHMENT_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c)) != null && tierDiscountRuleMap.get(Nokia_CPQ_Constants.INCENTIVES_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + Nokia_CPQ_Constants.ATTACHMENT_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c)).size() > 0)
                                {
                                    item.NokiaCPQ_Attachment_Per__c = tierDiscountRuleMap.get(Nokia_CPQ_Constants.INCENTIVES_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + Nokia_CPQ_Constants.ATTACHMENT_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c)).get(0);
                                }
                            }
                            if (this.proposalSO.Apttus_Proposal__Account__r.NokiaCPQ_Performance__c)
                            {
                                if (!tierDiscountRuleMap.isEmpty() && tierDiscountRuleMap.get(Nokia_CPQ_Constants.INCENTIVES_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + Nokia_CPQ_Constants.ATTACHMENT_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c)) != null && tierDiscountRuleMap.get(Nokia_CPQ_Constants.INCENTIVES_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + Nokia_CPQ_Constants.ATTACHMENT_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c)).size() > 0)
                                {
                                    item.NokiaCPQ_Performance_Per__c = tierDiscountRuleMap.get(Nokia_CPQ_Constants.INCENTIVES_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + Nokia_CPQ_Constants.PERFORMANCE_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c)).get(0);
                                }
                            }
                        }

                        if (Integer.Valueof(this.proposalSO.NokiaCPQ_No_of_Years__c) >= 3 && !this.sspSRSDefaultsList.isEmpty() && sspSRSDefaultsList[0].Multi_Year_Discount_Applicable__c == true)
                        {
                            if (!tierDiscountRuleMap.isEmpty() && tierDiscountRuleMap.get(Nokia_CPQ_Constants.INCENTIVES_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + Nokia_CPQ_Constants.MULTIYR_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c)) != null && tierDiscountRuleMap.get(Nokia_CPQ_Constants.INCENTIVES_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + Nokia_CPQ_Constants.MULTIYR_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c)).size() > 0)
                            {
                                item.NokiaCPQ_Multi_Yr_Per__c = tierDiscountRuleMap.get(Nokia_CPQ_Constants.INCENTIVES_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Type__c) + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + Nokia_CPQ_Constants.MULTIYR_STRING_APPENDER + String.valueOf(this.proposalSO.Apttus_Proposal__Account__r.Partner_Program__c)).get(0);
                            }
                        }

                        if (item.NokiaCPQ_Attachment_Per__c == null)
                        {
                            item.NokiaCPQ_Attachment_Per__c = 0.00;
                        }
                        if (item.NokiaCPQ_Renewal_Per__c == null)
                        {
                            item.NokiaCPQ_Renewal_Per__c = 0.00;
                        }
                        if (item.NokiaCPQ_Performance_Per__c == null)
                        {
                            item.NokiaCPQ_Performance_Per__c = 0.00;
                        }
                        if (item.NokiaCPQ_Multi_Yr_Per__c == null && Integer.Valueof(this.proposalSO.NokiaCPQ_No_of_Years__c) < 3)
                        {
                            item.NokiaCPQ_Multi_Yr_Per__c = 0.00;
                        }

                        item.NokiaCPQ_Total_Maintenance_Discount__c = item.NokiaCPQ_Maint_Accreditation_Discount__c + item.NokiaCPQ_Attachment_Per__c + item.NokiaCPQ_Renewal_Per__c + item.NokiaCPQ_Performance_Per__c + item.NokiaCPQ_Multi_Yr_Per__c;

                        NokiaCPQ_Maintenance_and_SSP_Rules__c nokiaSSPSRSProdDiscount = new NokiaCPQ_Maintenance_and_SSP_Rules__c();
                        Double productCatDiscount = 0.00;
                        Double unlimitedSSP = 0.00;
                        Double biennialSSP = 0.00;
                        Double serviceRateY1 = 0.00;
                        Double serviceRateY2 = 0.00;
                        if (maintenanceSSPRuleMap != null && maintenanceSSPRuleMap.get(item.Nokia_Pricing_Cluster__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + productDiscountCat + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.proposalSO.NokiaCPQ_IsPMA__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.proposalSO.NokiaCPQ_Maintenance_Type__c) != null && maintenanceSSPRuleMap.get(item.Nokia_Pricing_Cluster__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + productDiscountCat + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.proposalSO.NokiaCPQ_IsPMA__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.proposalSO.NokiaCPQ_Maintenance_Type__c).size() > 0)
                        {
                            nokiaSSPSRSProdDiscount = maintenanceSSPRuleMap.get(item.Nokia_Pricing_Cluster__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + productDiscountCat + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.proposalSO.NokiaCPQ_IsPMA__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.proposalSO.NokiaCPQ_Maintenance_Type__c).get(0);
                            if (nokiaSSPSRSProdDiscount != null)
                            {
                                if (nokiaSSPSRSProdDiscount.NokiaCPQ_Product_Discount_Category_per__c == NULL)
                                {
                                    productCatDiscount = 0.0;
                                }
                                else
                                {
                                    productCatDiscount = nokiaSSPSRSProdDiscount.NokiaCPQ_Product_Discount_Category_per__c;
                                }
                                if (nokiaSSPSRSProdDiscount.NokiaCPQ_Unlimited_SSP_Discount__c == NULL)
                                {
                                    unlimitedSSP = 0.0;
                                }
                                else
                                {
                                    unlimitedSSP = nokiaSSPSRSProdDiscount.NokiaCPQ_Unlimited_SSP_Discount__c;
                                }
                                if (nokiaSSPSRSProdDiscount.NokiaCPQ_Biennial_SSP_Discount__c == NULL)
                                {
                                    biennialSSP = 0.0;
                                }
                                else
                                {
                                    biennialSSP = nokiaSSPSRSProdDiscount.NokiaCPQ_Biennial_SSP_Discount__c;
                                }
                            }
                        }

                        NokiaCPQ_Maintenance_and_SSP_Rules__c nokiaMaintenanceProdDiscount;
                        if (maintenanceSSPRuleMap != null && maintenanceSSPRuleMap.get(item.Nokia_Maint_Pricing_Cluster__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + productDiscountCat + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.proposalSO.NokiaCPQ_IsPMA__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.proposalSO.NokiaCPQ_Maintenance_Type__c) != null && maintenanceSSPRuleMap.get(item.Nokia_Maint_Pricing_Cluster__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + productDiscountCat + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.proposalSO.NokiaCPQ_IsPMA__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.proposalSO.NokiaCPQ_Maintenance_Type__c).size() > 0)
                        {
                            nokiaMaintenanceProdDiscount = maintenanceSSPRuleMap.get(item.Nokia_Maint_Pricing_Cluster__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + productDiscountCat + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.proposalSO.NokiaCPQ_IsPMA__c + Nokia_CPQ_Constants.NOKIA_STRING_APPENDER + this.proposalSO.NokiaCPQ_Maintenance_Type__c).get(0);
                            if (nokiaMaintenanceProdDiscount != null)
                            {
                                if (nokiaMaintenanceProdDiscount.NokiaCPQ_Service_Rate_Y1__c == NULL || item.NokiaCPQ_Spare__c == true || (proposalSO.NokiaCPQ_Is_Maintenance_Quote__c == true && proposalSO.Warranty_credit__c != null && proposalSO.Warranty_credit__c.equalsIgnoreCase(Nokia_CPQ_Constants.Nokia_NO)) || (item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_OPTION) && item.NokiaCPQ_Static_Bundle_Option__c == true && (proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_FIXED_ACCESS_POL) || proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_FIXED_ACCESS_FBA))))
                                {
                                    serviceRateY1 = 0.0;
                                }
                                else
                                {
                                    serviceRateY1 = nokiaMaintenanceProdDiscount.NokiaCPQ_Service_Rate_Y1__c;
                                }
                                if (nokiaMaintenanceProdDiscount.NokiaCPQ_Service_Rate_Y2__c == NULL || item.NokiaCPQ_Spare__c == true || (is1Year == true) || (item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_OPTION) && item.NokiaCPQ_Static_Bundle_Option__c == true && (proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_FIXED_ACCESS_POL) || proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_FIXED_ACCESS_FBA))))
                                {
                                    serviceRateY2 = 0.0;
                                }
                                else
                                {
                                    serviceRateY2 = nokiaMaintenanceProdDiscount.NokiaCPQ_Service_Rate_Y2__c;
                                }

                            }
                        }
                        //Heema : Req 6593 start:
                        if (proposalSO.NokiaCPQ_LEO_Discount__c == true && (proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_FIXED_ACCESS_POL) || proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_FIXED_ACCESS_FBA)))
                        {

                            serviceRateY1 = 0.05;
                            if (is1Year)
                            {
                                serviceRateY2 = 0.0;
                            }
                            else
                            {
                                serviceRateY2 = 0.05;
                            }
                        }
                        //Heema : Req 6593 End:
                        Double maintY1Percent = 0.00;
                        Double maintY2Percent = 0.00;

                        //Varsha: start: Changes for Req #4920
                        //Removed item.portfolio formula field and used proposalSO.NokiaCPQ_Portfolio__c
                        String portfolioFromProduct = getPortfolio(item);
                        if (!this.sspSRSDefaultsList.isEmpty() && String.isNotBlank(portfolioFromProduct) && sspSRSDefaultsList[0].Portfolio__c.equalsIgnoreCase(portfolioFromProduct))
                        {

                            if (this.proposalSO.NokiaCPQ_Maintenance_Type__c != null && item.is_custom_product__c == false)
                            {
                                item.Nokia_Maint_Y1_Per__c = serviceRateY1 * 100;
                                item.Nokia_Maint_Y2_Per__c = serviceRateY2 * 100;

                            }

                            if (item.is_custom_product__c == false)
                                item.Nokia_CPQ_Maint_Prod_Cat_Disc__c = productCatDiscount * 100;

                            if (item.NokiaCPQ_Accreditation_Discount__c == null)
                            {
                                item.NokiaCPQ_Accreditation_Discount__c = 0.00;
                            }
                            if (item.Nokia_CPQ_Maint_Prod_Cat_Disc__c == null)
                            {
                                item.Nokia_CPQ_Maint_Prod_Cat_Disc__c = 0.00;
                            }

                            //For portfolios eligible for SSP
                            if (sspSRSDefaultsList[0].SSP_Visible__c)
                            {
                                if (item.is_custom_Product__c == false)
                                {

                                    if (this.proposalSO.NokiaCPQ_SSP_Level__c != null && Nokia_CPQ_Constants.NOKIA_UNLIMITED.equalsIgnoreCase(this.proposalSO.NokiaCPQ_SSP_Level__c) && !isLEO)
                                    {
                                        item.NokiaCPQ_SSP_Rate__c = unlimitedSSP * 100;
                                    }
                                    else if (this.proposalSO.NokiaCPQ_SSP_Level__c != null && Nokia_CPQ_Constants.NOKIA_BIENNIAL.equalsIgnoreCase(this.proposalSO.NokiaCPQ_SSP_Level__c) && !isLEO)
                                    {
                                        item.NokiaCPQ_SSP_Rate__c = biennialSSP * 100;
                                    }

                                    if (item.NokiaCPQ_SSP_Rate__c == null || isLEO)
                                    {
                                        item.NokiaCPQ_SSP_Rate__c = 0.00;

                                    }
                                }
                                if ((productDiscountCat != null && !productDiscountCat.contains(Nokia_CPQ_Constants.NOKIA_NFM_P)) || item.is_Custom_Product__c == true)
                                {
                                    if (itemSO.Apttus_Config2__ListPrice__c != null && item.NokiaCPQ_SSP_Rate__c != null)
                                    {
                                        //00209932 D-14423 Start Rounding issue
                                        item.Nokia_SSP_List_Price__c = (Double.valueOf(itemSO.Apttus_Config2__ListPrice__c) * Double.valueOf(item.NokiaCPQ_SSP_Rate__c) * .01).setScale(2, RoundingMode.HALF_UP);
                                        //00209932 D-14423 End Rounding issue
                                    }
                                    if (item.Nokia_SSP_List_Price__c != null && item.NokiaCPQ_SSP_Rate__c != null && item.Nokia_CPQ_Maint_Prod_Cat_Disc__c != null)
                                    {
                                        item.Nokia_SSP_Base_Price__c = item.Nokia_SSP_List_Price__c * (1 - item.Nokia_CPQ_Maint_Prod_Cat_Disc__c * .01) * (1 - item.NokiaCPQ_Accreditation_Discount__c * .01);
                                        item.Nokia_SSP_Base_Price__c = item.Nokia_SSP_Base_Price__c.setScale(2, RoundingMode.HALF_UP);
                                    }
                                    if (item.Nokia_SSP_Base_Price__c != null && item.Apttus_Config2__Quantity__c != null)
                                    {
                                        item.Nokia_SSP_Base_Extended_Price__c = Double.valueOf(item.Nokia_SSP_Base_Price__c) * item.Apttus_Config2__Quantity__c * quantityBundle;
                                    }
                                }
                            }

                            //For portfolios eligible for SRS
                            if (sspSRSDefaultsList[0].SRS_Visible__c)
                            {
                                // Replacing item.Portfolio_from_Quote_Line_Item__c with this.proposalSO.NokiaCPQ_Portfolio__c
                                if (((productDiscountCat != null && !pdcList.isEmpty() && pdcList.contains(productDiscountCat) || (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_NUAGE) && item.Apttus_Config2__ProductId__c != null && Nokia_CPQ_Constants.PRODUCTITEMTYPESOFTWARE.equalsIgnoreCase(item.Apttus_Config2__ProductId__r.NokiaCPQ_Item_Type__c))) && !isLEO) || item.is_Custom_Product__c == true)
                                {
                                    //system.debug('SRS calculation');
                                    if (itemSO.Apttus_Config2__ListPrice__c != null && sspSRSDefaultsList[0].SRS_Percentage__c != null)
                                    {
                                        //00209932 D-14423 Start Rounding issue
                                        item.Nokia_SRS_List_Price__c = (Double.valueOf(itemSO.Apttus_Config2__ListPrice__c) * sspSRSDefaultsList[0].SRS_Percentage__c).setScale(2, RoundingMode.HALF_UP);
                                        //00209932 D-14423 End Rounding issue
                                    }
                                    // system.debug('SRS calculation' +item.Nokia_SRS_List_Price__c);
                                    if (item.Nokia_SRS_List_Price__c != null && item.Nokia_CPQ_Maint_Prod_Cat_Disc__c != null)
                                    {
                                        item.Nokia_SRS_Base_Price__c = item.Nokia_SRS_List_Price__c * (1 - item.Nokia_CPQ_Maint_Prod_Cat_Disc__c * .01) * (1 - item.NokiaCPQ_Accreditation_Discount__c * .01);
                                        item.Nokia_SRS_Base_Price__c = item.Nokia_SRS_Base_Price__c.setScale(2, RoundingMode.HALF_UP);
                                    }
                                    if (item.Nokia_SRS_Base_Price__c != null && item.Apttus_Config2__Quantity__c != null)
                                    {
                                        item.Nokia_SRS_Base_Extended_Price__c = item.Nokia_SRS_Base_Price__c * item.Apttus_Config2__Quantity__c * quantityBundle;
                                    }
                                }
                            }
                        }
                        //Varsha: end: Changes for Req #4920

                        if (item.NokiaCPQ_Total_Maintenance_Discount__c == null)
                        {
                            item.NokiaCPQ_Total_Maintenance_Discount__c = 0.00;
                        }

                        if (itemSO.Apttus_Config2__ListPrice__c != null && item.Nokia_Maint_Y1_Per__c != null)
                        {
                            //00209932 D-14423 Start 
                            item.NokiaCPQ_Maint_Y1_List_Price__c = (itemSO.Apttus_Config2__ListPrice__c * item.Nokia_Maint_Y1_Per__c * .01).setScale(2, RoundingMode.HALF_UP);
                            item.Nokia_Maint_Y1_Extended_List_Price__c = (item.NokiaCPQ_Maint_Y1_List_Price__c * item.Apttus_Config2__Quantity__c * quantityBundle).setScale(2, RoundingMode.HALF_UP);
                            //00209932 D-14423 End 
                        }
                        if (itemSO.Apttus_Config2__ListPrice__c != null && item.Nokia_Maint_Y2_Per__c != null)
                        {
                            //00209932 D-14423 Start 
                            item.NokiaCPQ_Maint_Yr2_List_Price__c = (itemSO.Apttus_Config2__ListPrice__c * item.Nokia_Maint_Y2_Per__c * .01).setScale(2, RoundingMode.HALF_UP);
                            item.NokiaCPQ_Maint_Yr2_Extended_List_Price__c = (item.NokiaCPQ_Maint_Yr2_List_Price__c * item.Apttus_Config2__Quantity__c * quantityBundle).setScale(2, RoundingMode.HALF_UP);
                            //00209932 D-14423 End
                        }


                        if (item.NokiaCPQ_Maint_Y1_List_Price__c != null)
                        {

                            // system.debug('NokiaCPQ_Total_Maintenance_Discount__c'+item.NokiaCPQ_Total_Maintenance_Discount__c+' '+item.NokiaCPQ_Maint_Y1_List_Price__c);
                            item.NokiaCPQ_Maint_Yr1_Base_Price__c = item.NokiaCPQ_Maint_Y1_List_Price__c * (1 - Double.valueOf(item.NokiaCPQ_Total_Maintenance_Discount__c) * .01);
                            item.NokiaCPQ_Maint_Yr1_Base_Price__c = item.NokiaCPQ_Maint_Yr1_Base_Price__c.setScale(2, RoundingMode.HALF_UP);

                        }

                        if (item.NokiaCPQ_Maint_Yr2_List_Price__c != null)
                        {
                            item.NokiaCPQ_Maint_Yr2_Base_Price__c = item.NokiaCPQ_Maint_Yr2_List_Price__c * (1 - Double.valueOf(item.NokiaCPQ_Total_Maintenance_Discount__c) * .01);
                            item.NokiaCPQ_Maint_Yr2_Base_Price__c = item.NokiaCPQ_Maint_Yr2_Base_Price__c.setScale(2, RoundingMode.HALF_UP);
                        }

                        if (item.NokiaCPQ_Maint_Yr1_Base_Price__c != null && item.Apttus_Config2__Quantity__c != null)
                        {
                            item.NokiaCPQ_Maint_Yr1_Extended_Price__c = Double.valueOf(item.NokiaCPQ_Maint_Yr1_Base_Price__c) * Double.valueOf(item.Apttus_Config2__Quantity__c) * quantityBundle;
                        }
                        if (item.NokiaCPQ_Maint_Yr2_Base_Price__c != null && item.Apttus_Config2__Quantity__c != null)
                        {
                            item.NokiaCPQ_Maint_Yr2_Extended_Price__c = Double.valueOf(item.NokiaCPQ_Maint_Yr2_Base_Price__c) * Double.valueOf(item.Apttus_Config2__Quantity__c) * quantityBundle;
                        }

                        // End SSP for fn
                        if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_FIXED_ACCESS_POL) && itemSO.Apttus_Config2__ListPrice__c > 0 && item.NokiaCPQ_Category__c != Nokia_CPQ_Constants.NOKIA_PTP && item.Product_Number_Of_Ports__c > 0)
                        {

                            if (item.Nokia_Pricing_Cluster__c.contains(Nokia_CPQ_Constants.NOKIA_NAMCLUSTER))
                            {
                                if (item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_OPTION))
                                {
                                    item.Total_ONT_Quantity__c = item.Total_Option_Quantity__c;
                                }
                                else
                                {
                                    item.Total_ONT_Quantity__c = item.Apttus_Config2__Quantity__c;
                                }
                            }
                            else
                            {
                                if (item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_OPTION))
                                {
                                    item.Total_ONT_Quantity__c = item.Total_Option_Quantity__c * item.Apttus_Config2__OptionId__r.Number_of_GE_Ports__c;
                                }
                                else
                                {
                                    item.Total_ONT_Quantity__c = item.Apttus_Config2__Quantity__c * item.Apttus_Config2__ProductId__r.Number_of_GE_Ports__c;
                                }

                            }

                        }
                        if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_FIXED_ACCESS_FBA))
                        {
                            if (item.Apttus_Config2__ListPrice__c > 0 && item.NokiaCPQ_Category__c == Nokia_CPQ_Constants.NOKIA_PTP && item.Product_Number_Of_Ports__c > 0)
                            {
                                item.Is_P2P__c = true;
                                if (item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_OPTION))
                                {
                                    item.Total_ONT_Quantity_P2P__c = item.Total_Option_Quantity__c * item.Apttus_Config2__OptionId__r.Number_of_GE_Ports__c;
                                }
                                else
                                {
                                    item.Total_ONT_Quantity_P2P__c = item.Apttus_Config2__Quantity__c * item.Apttus_Config2__ProductId__r.Number_of_GE_Ports__c;
                                }
                            }
                            if (item.Apttus_Config2__ListPrice__c > 0 && item.NokiaCPQ_Category__c == Nokia_CPQ_Constants.NOKIA_ONT)
                            {
                                item.Is_FBA__c = true;
                                if (item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_OPTION))
                                {
                                    item.Total_ONT_Quantity_FBA__c = item.Total_Option_Quantity__c;
                                }
                                else
                                {
                                    item.Total_ONT_Quantity_FBA__c = item.Apttus_Config2__Quantity__c;
                                }

                            }
                        }
                        // End SSP for fn
                    }
                }
            }
        }
        catch (Exception ex)
        {
            //system.debug('exception in PriceItemSet**' + ex);
            //ExceptionHandler.addException(ex, Nokia_PricingCallBack.class.getName(),'OnPriceItemSet Method and line number is'+ex.getLineNumber());
        }
    }

    /* Method Name   : beforePricingLineItem
	* Developer	  : Apttus
	* Description	: OOTB */
    void beforePricingLineItem(Apttus_Config2.ProductConfiguration.LineItemColl itemColl, Apttus_Config2.LineItem lineItemMO)
    {

    }

    /* Method Name   : afterPricing
    * Developer	  : Apttus
    * Description	: OOTB */
    void afterPricing(Apttus_Config2.ProductConfiguration.LineItemColl itemColl)
    {

    }

    /* Method Name   : beforePricingLineItem
    * Developer	  : Apttus
    * Description	: OOTB */
    void afterPricingLineItem(Apttus_Config2.ProductConfiguration.LineItemColl itemColl, Apttus_Config2.LineItem lineItemMO)
    {

    }

    /*****************************
    *Method Name: calculateEquivalentPrice
    *Description: To calculat equivalent price
    *Parameters: List of Configuration lin items from cart
    *******************************/

    public static void calculateEquivalentPrice(list<Apttus_Config2__LineItem__c> allLineItems, string market)
    {
        try
        {
            map<string, decimal> mapcategoryDiscount = new map<string, decimal>();
            map<id, string> mapLineCategory = new map<id, string>();
            list<String> discountCatgory = new List<String>();
            map<Decimal, list<Apttus_Config2__LineItem__c>> mapBundlelineOption = new map<Decimal, list<Apttus_Config2__LineItem__c>>();
            map<Decimal, Decimal> mapBundlelineNContractPrice = new map<Decimal, Decimal>();
            map<Decimal, Decimal> mapBundlelineNReferencePrice = new map<Decimal, Decimal>();
            list<Apttus_Config2__LineItem__c> nonContractedLines = new List<Apttus_Config2__LineItem__c>();
            list<Product_Discount__c> productDisc = new list<Product_Discount__c>();
            set<ID> pricelistItemId = new set<ID>();
            list<Apttus_Config2__PriceListItem__c> priceitems = new list<Apttus_Config2__PriceListItem__c>();
            set<ID> ContractedPriceItems = new set<ID>();
            map<ID, Decimal> mapPLIToContractprice = new map<ID, Decimal>();
            decimal marketdisc;
            for (Apttus_Config2__LineItem__c lineitem : allLineItems)
            {
                pricelistItemId.add(lineitem.Apttus_Config2__PriceListItemId__c);
            }

            priceitems = [select id, Apttus_Config2__ContractPrice__c from Apttus_Config2__PriceListItem__c where id in: pricelistItemId];
            
            if (!priceitems.isEmpty())
            {
                for (Apttus_Config2__PriceListItem__c priceItem : priceitems)
                {
                    if (priceItem.Apttus_Config2__ContractPrice__c != null && priceItem.Apttus_Config2__ContractPrice__c != 0.00)
                    {
                        ContractedPriceItems.add(priceItem.id);
                        mapPLIToContractprice.put(priceItem.id, priceItem.Apttus_Config2__ContractPrice__c);
                    }
                }
            }
            
            for (Apttus_Config2__LineItem__c lineitem : allLineItems)
            {
                String productCatDiscount = getProductDiscountCategory(lineitem);

                if(ContractedPriceItems.contains(lineitem.Apttus_Config2__PriceListItemId__c)){
                    if(!(lineitem.Apttus_Config2__LineType__c.equalsIgnoreCase('Bundle'))){
                        lineitem.Reference_Price__c = mapPLIToContractprice.get(lineitem.Apttus_Config2__PriceListItemId__c);
                    }
                }

        else
                {
                    discountCatgory.add(productCatDiscount);
                    mapLineCategory.put(lineitem.id, productCatDiscount);
                    nonContractedLines.add(lineitem);
                }
                list<Apttus_Config2__LineItem__c> optionlinelist = new list<Apttus_Config2__LineItem__c>();
                if (mapBundlelineOption.containsKey(lineitem.Apttus_Config2__LineNumber__c))
                {
                    optionlinelist = mapBundlelineOption.get(lineitem.Apttus_Config2__LineNumber__c);
                }
                optionlinelist.add(lineitem);
                mapBundlelineOption.put(lineitem.Apttus_Config2__LineNumber__c, optionlinelist);
            }

            productDisc = [select id, name, Product_Discount_Category__c, Market__c, Discount__c from Product_Discount__c where Market__c =: market AND(Product_Discount_Category__c in: discountCatgory OR Product_Discount_Category__c = null)];
            if (!productDisc.IsEmpty())
            {
                for (Product_Discount__c categorydiscount : productDisc)
                {
                    if (categorydiscount.Product_Discount_Category__c == null)
                    {
                        marketdisc = categorydiscount.Discount__c;
                    }
                    mapcategoryDiscount.put(categorydiscount.Product_Discount_Category__c, categorydiscount.Discount__c);
                }
            }
            if (!nonContractedLines.IsEmpty())
            {
                for (Apttus_Config2__LineItem__c lineitem : nonContractedLines)
                {
                    if (!(lineitem.Apttus_Config2__LineType__c.equalsIgnoreCase('Bundle')))
                    {
                        if (mapLineCategory.containsKey(lineitem.id))
                        {
                            if (mapcategoryDiscount.containsKey(mapLineCategory.get(lineitem.id)))
                            {
                                decimal discountPerc = mapcategoryDiscount.get(mapLineCategory.get(lineitem.id));
                                lineitem.Reference_Price__c = (lineitem.Apttus_Config2__ListPrice__c - (lineitem.Apttus_Config2__ListPrice__c * (discountPerc / 100)));
                            }
                            else
                            {
                                lineitem.Reference_Price__c = (lineitem.Apttus_Config2__ListPrice__c - (lineitem.Apttus_Config2__ListPrice__c * (marketdisc / 100)));
                            }
                        }
                        else
                        {
                            lineitem.Reference_Price__c = lineitem.Apttus_Config2__ListPrice__c;
                        }
                    }
                }
            }
            for (Apttus_Config2__LineItem__c lineitem : allLineItems)
            {
                String configType = getConfigType(lineitem);
                decimal bundleRefPrice = 0.00;
                if (mapBundlelineOption.containskey(lineitem.Apttus_Config2__LineNumber__c) && configType.equalsIgnoreCase('Bundle'))
                {

                    for (Apttus_Config2__LineItem__c option : mapBundlelineOption.get(lineitem.Apttus_Config2__LineNumber__c))
                    {
                        if (option.Apttus_Config2__LineType__c.equalsIgnoreCase('Option'))
                        {

                            bundleRefPrice = bundleRefPrice + (option.Reference_Price__c * option.Apttus_Config2__Quantity__c);
                        }
                    }
                    lineitem.Reference_Price__c = bundleRefPrice;
                    mapBundlelineNReferencePrice.put(lineitem.Apttus_Config2__LineNumber__c, lineitem.Reference_Price__c);
                    mapBundlelineNContractPrice.put(lineitem.Apttus_Config2__LineNumber__c, mapPLIToContractprice.get(lineitem.Apttus_Config2__PriceListItemId__c));
                }
            }
            for (Apttus_Config2__LineItem__c lineitem : allLineItems)
            {
                decimal bundleRefPrice = 0.00;
                decimal BundleContractprice = 0.00;
                if (mapBundlelineOption.containskey(lineitem.Apttus_Config2__LineNumber__c) &&
                   lineitem.Apttus_Config2__LineType__c.equalsIgnoreCase('Option'))
                {
                    if (mapBundlelineNReferencePrice.containsKey(lineitem.Apttus_Config2__LineNumber__c))
                    {
                        BundleRefprice = mapBundlelineNReferencePrice.get(lineitem.Apttus_Config2__LineNumber__c);
                    }
                    if (mapBundlelineNContractPrice.containsKey(lineitem.Apttus_Config2__LineNumber__c))
                    {
                        BundleContractprice = mapBundlelineNContractPrice.get(lineitem.Apttus_Config2__LineNumber__c);
                    }

                    lineitem.Equivalent_Price__c = (lineitem.Reference_Price__c / bundleRefPrice) * BundleContractprice;
                }
            }
        }
        catch (Exception ex)
        {
            //ExceptionHandler.addException(ex, 'calculateEquivalentPrice', 'calculateEquivalentPrice and line number is' + ex.getLineNumber());
        }
    }

    /* Method Name   : finish
    * Developer	  : Apttus
    * Description	: OOTB */
    void finish()
    {
        try
        {
            system.debug(' finishmethode: ');

            if (this.proposalSO != null &&
    (this.proposalSO.Quote_Type__c.equalsIgnoreCase('Direct DS') ||
     (this.proposalSO.Quote_Type__c.equalsIgnoreCase('Indirect DS'))))
            {
                List<Apttus_Config2.LineItem> listLineitems = cart.getLineItems();
                string Market = this.proposalSO.Account_Market__c;
                List<Apttus_Config2__LineItem__c> allLineitem = new List<Apttus_Config2__LineItem__c>();
                if (!listLineitems.isempty())
                {
                    for (Apttus_Config2.LineItem LineItems: listLineitems)
                    {
                        allLineitem.add(LineItems.getLineItemSO());
                    }
                }
                if (!allLineitem.isempty())
                {
                    calculateEquivalentPrice(allLineitem, Market);
                }
            }
            if (this.proposalSO != null && this.proposalSO.Quote_Type__c.equalsIgnoreCase(Nokia_CPQ_Constants.QUOTE_TYPE_DIRECTCPQ))
            {
                system.debug('mode in Finish***' + mode);
                if (mode == Apttus_Config2.CustomClass.PricingMode.ADJUSTMENT || mode == Apttus_Config2.CustomClass.PricingMode.ROLLDOWN)
                {
                    List<Apttus_Config2.LineItem> listLineitem = cart.getLineItems();
                    Map<String, List<Apttus_Config2__LineItem__c>> primaryNoLineItemList1 = new Map<String, List<Apttus_Config2__LineItem__c>>();
                    for (Apttus_Config2.LineItem allLineItems: listLineitem)
                    {
                        Apttus_Config2__LineItem__c item = allLineItems.getLineItemSO();
                        String partNumber = getPartNumber(item);
                        String configType = getConfigType(item);

                        //Calculate quantity of options to be used in exports
                        Integer quantityBundle = 1;
                        if (mode == Apttus_Config2.CustomClass.PricingMode.ADJUSTMENT)
                        {
                            if (item.Apttus_Config2__LineType__c != null && Nokia_CPQ_Constants.NOKIA_OPTION.equals(String.valueOf(item.Apttus_Config2__LineType__c)))
                            {
                                if (this.lineItemObjectMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES + Nokia_CPQ_Constants.NOKIA_UNDERSCORE + item.Apttus_Config2__LineNumber__c) != null)
                                {
                                    if (this.lineItemObjectMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES + Nokia_CPQ_Constants.NOKIA_UNDERSCORE + item.Apttus_Config2__LineNumber__c).Apttus_Config2__Quantity__c != null)
                                    {
                                        quantityBundle = Integer.valueOf(this.lineItemObjectMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES + Nokia_CPQ_Constants.NOKIA_UNDERSCORE + item.Apttus_Config2__LineNumber__c).Apttus_Config2__Quantity__c.round(System.RoundingMode.CEILING));
                                        if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING))
                                            item.Total_Option_Quantity__c = quantityBundle * item.Apttus_Config2__ExtendedQuantity__c;
                                        else
                                            item.Total_Option_Quantity__c = quantityBundle * item.Apttus_Config2__Quantity__c;
                                    }
                                }
                            }
                        }

                        if (configType.equalsIgnoreCase('Bundle') && !this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING))
                        {
                            if (primaryNoLineItemList.containsKey(item.Apttus_Config2__PrimaryLineNumber__c + '_' + item.Apttus_Config2__ChargeType__c))
                            {
                                if (!Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES.equalsIgnoreCase(item.Apttus_Config2__LineType__c))
                                {
                                    item.NokiaCPQ_Unitary_Cost__c = 0.00;
                                    for (Apttus_Config2__LineItem__c optionItem : primaryNoLineItemList.get(item.Apttus_Config2__PrimaryLineNumber__c + '_' + item.Apttus_Config2__ChargeType__c))
                                    {
                                        //Stamping Cost at Arcadia level
                                        if (optionItem.NokiaCPQ_Unitary_Cost__c != null)
                                            item.NokiaCPQ_Unitary_Cost__c = (item.NokiaCPQ_Unitary_Cost__c + optionItem.NokiaCPQ_Unitary_Cost__c * optionItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                                    }
                                }

                                //stamping arcadia and other direct options prices to the main bundle - was done to resolve sequencing issue
                                if (Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES.equalsIgnoreCase(item.Apttus_Config2__LineType__c))
                                {
                                    if (lineItemIRPMapDirect != null && lineItemIRPMapDirect.containsKey(item.Apttus_Config2__LineNumber__c + '_' + item.Apttus_Config2__ChargeType__c))
                                    {
                                        item.NokiaCPQ_Unitary_IRP__c = 0.0;
                                        item.NokiaCPQ_Extended_CUP__c = 0.0;
                                        item.NCPQ_Unitary_CLP__c = 0.0;

                                        for (Apttus_Config2__LineItem__c optionLineItem :lineItemIRPMapDirect.get(item.Apttus_Config2__LineNumber__c + '_' + item.Apttus_Config2__ChargeType__c))
                                        {
                                            if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase('IP Routing') || this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase('Optics') || this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase('Nuage') || this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase('Optics - Wavelite'))
                                            {
                                                if (!optionLineItem.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES))
                                                    item.NokiaCPQ_Unitary_IRP__c = (item.NokiaCPQ_Unitary_IRP__c + optionLineItem.NokiaCPQ_Unitary_IRP__c * optionLineItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);

                                                if (optionLineItem.Apttus_Config2__BasePriceOverride__c != null)
                                                {
                                                    item.NCPQ_Unitary_CLP__c = (item.NCPQ_Unitary_CLP__c + (optionLineItem.Apttus_Config2__BasePriceOverride__c).setScale(2, RoundingMode.HALF_UP) * optionLineItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                                                }
                                                else if (optionLineItem.Apttus_Config2__BasePrice__c != null)
                                                {
                                                    item.NCPQ_Unitary_CLP__c = (item.NCPQ_Unitary_CLP__c + (optionLineItem.Apttus_Config2__BasePrice__c).setScale(2, RoundingMode.HALF_UP) * optionLineItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                                                }

                                                if (!optionLineItem.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES))
                                                    item.NokiaCPQ_Extended_CUP__c = item.NokiaCPQ_Extended_CUP__c + optionLineItem.Apttus_Config2__AdjustedPrice__c;
                                            }
                                            else if (optionLineItem.Apttus_Config2__ParentBundleNumber__c == item.Apttus_Config2__PrimaryLineNumber__c)
                                            {
                                                item.NokiaCPQ_Unitary_IRP__c = (item.NokiaCPQ_Unitary_IRP__c + optionLineItem.NokiaCPQ_Unitary_IRP__c * optionLineItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);

                                                if (optionLineItem.Apttus_Config2__BasePriceOverride__c != null)
                                                {
                                                    item.NCPQ_Unitary_CLP__c = (item.NCPQ_Unitary_CLP__c + (optionLineItem.Apttus_Config2__BasePriceOverride__c).setScale(2, RoundingMode.HALF_UP) * optionLineItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                                                }
                                                else if (optionLineItem.Apttus_Config2__BasePrice__c != null)
                                                {
                                                    item.NCPQ_Unitary_CLP__c = (item.NCPQ_Unitary_CLP__c + (optionLineItem.Apttus_Config2__BasePrice__c).setScale(2, RoundingMode.HALF_UP) * optionLineItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                                                }

                                                item.NokiaCPQ_Extended_CUP__c = item.NokiaCPQ_Extended_CUP__c + optionLineItem.Apttus_Config2__AdjustedPrice__c;
                                            }
                                        }
                                    }

                                }
                                //Stamping Unitary CLP for Software Arcadia items
                                if (!item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES) && item.Apttus_Config2__BasePriceOverride__c != null)
                                {
                                    item.NCPQ_Unitary_CLP__c = item.Apttus_Config2__BasePriceOverride__c.setScale(2, RoundingMode.HALF_UP);
                                }
                                else if (!item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES))
                                {
                                    item.NCPQ_Unitary_CLP__c = item.Apttus_Config2__BasePrice__c.setScale(2, RoundingMode.HALF_UP);
                                }

                                if (!item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES))
                                {
                                    item.NokiaCPQ_Extended_CUP__c = item.Apttus_Config2__AdjustedPrice__c;
                                    item.NokiaCPQ_Extended_CNP__c = item.Apttus_Config2__NetPrice__c;
                                }

                                if (item.Apttus_Config2__ChargeType__c != null && !item.Apttus_Config2__ChargeType__c.equalsIgnoreCase('Standard Price'))
                                {
                                    item.NokiaCPQ_Extended_IRP2__c = (item.NCPQ_Unitary_CLP__c * item.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                                }
                                else if (item.Apttus_Config2__ChargeType__c != null)
                                {
                                    item.NokiaCPQ_Extended_IRP2__c = (item.NokiaCPQ_Unitary_IRP__c * item.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                                }

                                item.NokiaCPQ_Extended_CLP_2__c = (item.NCPQ_Unitary_CLP__c * item.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);

                                if (item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES))
                                {
                                    if (item.NokiaCPQ_AdvancePricing_CUP__c != null)
                                    {
                                        item.NokiaCPQ_Extended_CUP_2__c = (item.NokiaCPQ_AdvancePricing_CUP__c * item.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                                    }
                                    else
                                    {
                                        item.NokiaCPQ_Extended_CUP_2__c = (item.NokiaCPQ_Extended_CUP__c * item.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
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
                            if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING) && partNumber != null
                      && (partNumber.contains(Nokia_CPQ_Constants.MAINTY1CODE) || partNumber.contains(Nokia_CPQ_Constants.MAINTY2CODE)))
                            {

                                item.NokiaCPQ_Extended_IRP2__c = item.NokiaCPQ_Unitary_IRP__c;

                                if (item.Apttus_Config2__BasePriceOverride__c != null)
                                {
                                    item.NokiaCPQ_Extended_CLP_2__c = item.Apttus_Config2__BasePriceOverride__c.setScale(2, RoundingMode.HALF_UP);
                                }
                                else if (item.Apttus_Config2__BasePrice__c != null)
                                {
                                    item.NokiaCPQ_Extended_CLP_2__c = item.Apttus_Config2__BasePrice__c.setScale(2, RoundingMode.HALF_UP);
                                }
                                if (item.NokiaCPQ_AdvancePricing_CUP__c != null)
                                {
                                    item.NokiaCPQ_Extended_CUP_2__c = item.NokiaCPQ_AdvancePricing_CUP__c;
                                }
                                else
                                {
                                    item.NokiaCPQ_Extended_CUP_2__c = item.Apttus_Config2__AdjustedPrice__c;
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
                        else if (item.NokiaCPQ_ExtendedPrice_CNP__c > 0 && item.NokiaCPQ_Extended_Cost__c != null && this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING))
                        {
                            item.Sales_Margin__c = ((item.NokiaCPQ_ExtendedPrice_CNP__c - item.NokiaCPQ_Extended_Cost__c) / (item.NokiaCPQ_ExtendedPrice_CNP__c)) * 100;
                        }
                        else if (item.Apttus_Config2__NetPrice__c > 0 && item.NokiaCPQ_Extended_Cost__c != null)
                        {
                            item.Sales_Margin__c = ((item.Apttus_Config2__NetPrice__c - item.NokiaCPQ_Extended_Cost__c) / (item.Apttus_Config2__NetPrice__c)) * 100;
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
                                if (item.NokiaCPQ_ExtendedPrice_CNP__c != null && this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING))
                                {

                                    item.NokiaCPQ_IRP_Discount__c = ((item.NokiaCPQ_Extended_IRP2__c - item.NokiaCPQ_ExtendedPrice_CNP__c) / item.NokiaCPQ_Extended_IRP2__c) * 100;
                                }
                                else
                                {
                                    item.NokiaCPQ_IRP_Discount__c = ((item.NokiaCPQ_Extended_IRP2__c - item.Apttus_Config2__NetPrice__c) / item.NokiaCPQ_Extended_IRP2__c) * 100;
                                }
                            }
                        }
                        else
                        {
                            item.NokiaCPQ_IRP_Discount__c = 0.00;
                        }
                        item.NokiaCPQ_IRP_Discount__c = item.NokiaCPQ_IRP_Discount__c.setScale(2, RoundingMode.HALF_UP);
                        //CPQ Requirement : Traffic Light calculations For MN Direct, NSW, Enterprise, QTC
                        deal_Guidance_Calculator(item, configType);

                        //Care Cost
                        if (item.Advanced_pricing_done__c == false && !portfolioSettingList.isEmpty() && portfolioSettingList[0].Cost_Calculation_In_PCB__c == false && item.Apttus_Config2__ChargeType__c != null && item.Apttus_Config2__ChargeType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_YEAR1_MAINTENANCE) && !item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES))
                        {
                            if (!careCostPercentList.isEmpty() && careCostPercentList[0].Care_Cost__c != null && item.NokiaCPQ_ExtendedPrice_CNP__c != null)
                            {
                                item.NokiaCPQ_Unitary_Cost__c = (item.NokiaCPQ_ExtendedPrice_CNP__c * (careCostPercentList[0].Care_Cost__c / 100)).setScale(2, RoundingMode.HALF_UP);
                            }
                        }

                        if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING))
                        {
                            if (!item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES))
                            {
                                if (primaryNoLineItemList1.containsKey(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c))
                                {
                                    primaryNoLineItemList1.get(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c).add(item);
                                }
                                else
                                {
                                    primaryNoLineItemList1.put(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c, new List<Apttus_Config2__LineItem__c> { item });
                                }
                            }
                        }
                    }

                    if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING))
                    {
                        for (Apttus_Config2.LineItem allLineItems: this.cart.getLineItems())
                        {
                            Apttus_Config2__LineItem__c item = allLineItems.getLineItemSO();
                            String configType = getConfigType(item);
                            //Piyush Tawari Start
                            //Req-6228 MN Airscale wifi - Price for the groups to be aggregated from SI
                            //For Direct MN Airscale Wifi
                            //checking if its Group
                            if (item != null && item.Apttus_Config2__LineType__c != NULL && item.Apttus_Config2__ChargeType__c != NULL &&
                            Nokia_CPQ_Constants.NOKIA_OPTION.equalsIgnoreCase(item.Apttus_Config2__LineType__c) &&
                                 Nokia_CPQ_Constants.BUNDLE.equalsIgnoreCase(configType))
                            {
                                item.NokiaCPQ_Unitary_Cost__c = 0.00;
                                item.NCPQ_Unitary_CLP__c = 0.0;
                                item.NokiaCPQ_Unitary_IRP__c = 0.0;
                                item.NokiaCPQ_Extended_CUP__c = 0.00;
                                item.NokiaCPQ_Extended_CNP__c = 0.00;
                                if (primaryNoLineItemList1.containsKey(item.Apttus_Config2__PrimaryLineNumber__c + '_' + item.Apttus_Config2__ChargeType__c))
                                {
                                    for (Apttus_Config2__LineItem__c optionItem : primaryNoLineItemList1.get(item.Apttus_Config2__PrimaryLineNumber__c + '_' + item.Apttus_Config2__ChargeType__c))
                                    {
                                        //Stamping IRP at Group Level
                                        if (optionItem.NokiaCPQ_Unitary_IRP__c != Null && optionItem.Apttus_Config2__Quantity__c != Null)
                                            item.NokiaCPQ_Unitary_IRP__c = (item.NokiaCPQ_Unitary_IRP__c + optionItem.NokiaCPQ_Unitary_IRP__c * optionItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                                        //stamping CLP at Group level
                                        if (optionItem.Apttus_Config2__BasePriceOverride__c != null)
                                        {
                                            item.NCPQ_Unitary_CLP__c = (item.NCPQ_Unitary_CLP__c + (optionItem.Apttus_Config2__BasePriceOverride__c).setScale(2, RoundingMode.HALF_UP) * optionItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                                        }
                                        else if (optionItem.Apttus_Config2__BasePrice__c != null)
                                        {
                                            item.NCPQ_Unitary_CLP__c = (item.NCPQ_Unitary_CLP__c + (optionItem.Apttus_Config2__BasePrice__c).setScale(2, RoundingMode.HALF_UP) * optionItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                                        }
                                        //Stamping CUP at Group level
                                        item.NokiaCPQ_Extended_CUP__c = item.NokiaCPQ_Extended_CUP__c + optionItem.Apttus_Config2__AdjustedPrice__c;
                                        //stamping CNP at Group Level
                                        item.NokiaCPQ_Extended_CNP__c = item.NokiaCPQ_Extended_CNP__c + optionItem.Apttus_Config2__NetPrice__c;
                                        item.NokiaCPQ_Unitary_Cost__c = (item.NokiaCPQ_Unitary_Cost__c + (optionItem.NokiaCPQ_Unitary_Cost__c).setScale(2, RoundingMode.HALF_UP) * optionItem.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                                    }
                                }
                                if (item.Apttus_Config2__AdjustmentType__c == Nokia_CPQ_Constants.DISCOUNT_AMOUNT)
                                    item.Apttus_Config2__BasePriceOverride__c = item.Apttus_Config2__AdjustmentAmount__c;
                                else
                                    item.Apttus_Config2__BasePriceOverride__c = 0;
                            }//Piyush Tawari End
                             //Logic for calculating Cost for main bundle

                            if (!mainBundleList.isEmpty() && !item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES) && mainBundleList.contains(String.valueOf(item.Apttus_Config2__ParentBundleNumber__c)))
                            {
                                if (unitaryCostMap.containsKey(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c))
                                {
                                    Double itemCost = 0.0;
                                    if (configType.equalsIgnoreCase('Bundle') && item.NokiaCPQ_Unitary_Cost__c != null)
                                    {
                                        itemCost = unitaryCostMap.get(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c) + item.NokiaCPQ_Unitary_Cost__c;
                                        unitaryCostMap.put(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c, itemCost);
                                    }
                                    else if (item.NokiaCPQ_Unitary_Cost__c != null)
                                    {
                                        itemCost = unitaryCostMap.get(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c) + (item.NokiaCPQ_Unitary_Cost__c * item.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                                        unitaryCostMap.put(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c, itemCost);
                                    }
                                }
                                else
                                {
                                    if (configType.equalsIgnoreCase('Bundle') && item.NokiaCPQ_Unitary_Cost__c != null)
                                    {
                                        unitaryCostMap.put(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c, item.NokiaCPQ_Unitary_Cost__c);
                                    }
                                    else if (item.NokiaCPQ_Unitary_Cost__c != null)
                                    {
                                        unitaryCostMap.put(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c, (item.NokiaCPQ_Unitary_Cost__c * item.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP));
                                    }
                                }
                            }
                        }
                    }

                    Map<Decimal, Decimal> linenumberCareDisPercentage = new Map<Decimal, Decimal>();
                    Map<Decimal, Decimal> linenumberSRSDisPercentage = new Map<Decimal, Decimal>();
                    Map<Decimal, Double> linenumberCarePrice = new Map<Decimal, Double>();
                    Map<Decimal, Double> linenumberSRSPrice = new Map<Decimal, Double>();
                    Map<Decimal, List<Double>> linenumberCareCLPCNP = new Map<Decimal, List<Double>>();
                    Map<Decimal, List<Double>> linenumberSRSCLPCNP = new Map<Decimal, List<Double>>();
                    //Start: creating map to limit for loop iterations
                    Map<String, Apttus_Config2__LineItem__c> mainBundleMap = new Map<String, Apttus_Config2__LineItem__c>();
                    //Stop
                    //R-6508 
                    Double totalExtendedSSPPrice = 0.00;
                    Double totalExtendedSRSPrice = 0.00;
                    //R-6508 End
                    if (!this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING))
                    {
                        Map<String, Apttus_Config2.LineItem> ssp_srsLinesMap_EP = new Map<String, Apttus_Config2.LineItem>();
                        for (Apttus_Config2.LineItem allLineItems: this.cart.getLineItems())
                        {
                            Apttus_Config2__LineItem__c item = allLineItems.getLineItemSO();
                            String configType = getConfigType(item);
                            Double CareDiscCalc = 0.0;
                            Double SRSDiscCalc = 0.0;
                            if (item != null && item.Apttus_Config2__LineType__c != null && item.Apttus_Config2__ChargeType__c != null && item.Apttus_Config2__ChargeType__c.equalsIgnoreCase(Nokia_CPQ_Constants.STANDARD) && item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES) && this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase('Nokia Software'))
                            {
                                if (lineItemIRPMapDirect.containsKey(item.Apttus_Config2__LineNumber__c + '_' + item.Apttus_Config2__ChargeType__c))
                                {
                                    Double careExtendedCLP = item.NokiaCPQ_Extended_CLP_2__c;
                                    Double careNetCNP = item.Apttus_Config2__NetPrice__c;
                                    Double srsExtendedCLP = item.NokiaCPQ_Extended_CLP_2__c;
                                    Double srsNetCNP = item.Apttus_Config2__NetPrice__c;

                                    for (Apttus_Config2__LineItem__c optionLineItem :lineItemIRPMapDirect.get(item.Apttus_Config2__LineNumber__c + '_' + item.Apttus_Config2__ChargeType__c))
                                    {
                                        if (optionLineItem.Apttus_Config2__ParentBundleNumber__c == item.Apttus_Config2__PrimaryLineNumber__c)
                                        {
                                            String classification = getClassification(optionLineItem);
                                            String itemType = getItemType(optionLineItem);

                                            if ('Standard Price'.equals(item.Apttus_Config2__ChargeType__c))
                                            {
                                                if (classification != null && 'Professional Services'.equals(classification))
                                                {
                                                    if (optionLineItem.NokiaCPQ_Extended_CLP_2__c != null)
                                                    {
                                                        careExtendedCLP = careExtendedCLP - optionLineItem.NokiaCPQ_Extended_CLP_2__c;
                                                        srsExtendedCLP = srsExtendedCLP - optionLineItem.NokiaCPQ_Extended_CLP_2__c;
                                                    }
                                                    careNetCNP = careNetCNP - optionLineItem.Apttus_Config2__NetPrice__c;
                                                    srsNetCNP = srsNetCNP - optionLineItem.Apttus_Config2__NetPrice__c;


                                                }

                                                if (itemType != null && 'Hardware'.equals(itemType))
                                                {
                                                    srsExtendedCLP = srsExtendedCLP - optionLineItem.NokiaCPQ_Extended_CLP_2__c;
                                                    srsNetCNP = srsNetCNP - optionLineItem.Apttus_Config2__NetPrice__c;

                                                }

                                            }
                                        }
                                    }

                                    if ('Standard Price'.equals(item.Apttus_Config2__ChargeType__c))
                                    {
                                        List<Double> listCareCLPCNP = new List<Double>();
                                        if (careExtendedCLP != null)
                                        {
                                            listCareCLPCNP.add(careExtendedCLP);
                                        }
                                        if (careNetCNP != null)
                                        {
                                            listCareCLPCNP.add(careNetCNP);
                                        }
                                        List<Double> listSRSCLPCNP = new List<Double>();
                                        if (srsExtendedCLP != null)
                                        {
                                            listSRSCLPCNP.add(srsExtendedCLP);
                                        }
                                        if (srsNetCNP != null)
                                        {
                                            listSRSCLPCNP.add(srsNetCNP);
                                        }
                                        if (!listCareCLPCNP.isEmpty())
                                        {
                                            linenumberCareCLPCNP.put(item.Apttus_Config2__LineNumber__c, listCareCLPCNP);
                                        }
                                        if (!listSRSCLPCNP.isEmpty())
                                        {
                                            linenumberSRSCLPCNP.put(item.Apttus_Config2__LineNumber__c, listSRSCLPCNP);
                                        }
                                    }
                                }

                                if (linenumberCareCLPCNP.get(item.Apttus_Config2__LineNumber__c) != null && linenumberSRSCLPCNP.get(item.Apttus_Config2__LineNumber__c) != null)
                                {
                                    if (!linenumberCareCLPCNP.get(item.Apttus_Config2__LineNumber__c).isEmpty() && linenumberCareCLPCNP.get(item.Apttus_Config2__LineNumber__c).get(0) != 0)
                                    {
                                        CareDiscCalc = (1 - ((linenumberCareCLPCNP.get(item.Apttus_Config2__LineNumber__c).get(0) - linenumberCareCLPCNP.get(item.Apttus_Config2__LineNumber__c).get(1)) / linenumberCareCLPCNP.get(item.Apttus_Config2__LineNumber__c).get(0)));
                                    }
                                    if (!linenumberSRSCLPCNP.get(item.Apttus_Config2__LineNumber__c).isEmpty() && linenumberSRSCLPCNP.get(item.Apttus_Config2__LineNumber__c).get(0) != 0)
                                    {
                                        SRSDiscCalc = (1 - ((linenumberSRSCLPCNP.get(item.Apttus_Config2__LineNumber__c).get(0) - linenumberSRSCLPCNP.get(item.Apttus_Config2__LineNumber__c).get(1)) / linenumberSRSCLPCNP.get(item.Apttus_Config2__LineNumber__c).get(0)));
                                    }

                                    if (CareDiscCalc != null)
                                    {
                                        linenumberCareDisPercentage.put(item.Apttus_Config2__LineNumber__c, Decimal.valueOf(CareDiscCalc).setScale(2, RoundingMode.HALF_UP));
                                    }
                                    if (SRSDiscCalc != null)
                                    {
                                        linenumberSRSDisPercentage.put(item.Apttus_Config2__LineNumber__c, Decimal.valueof(SRSDiscCalc).setScale(2, RoundingMode.HALF_UP));
                                    }
                                }
                            }

                            //R-6508 --ssp and SRS calculation for Enterprise
                            Integer quantityBundle = 1;
                            String partNumber = getPartNumber(item);

                            if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_IP_ROUTING) && !this.proposalSO.Is_List_Price_Only__c)
                            {
                                if (partNumber != null &&
                                    !partNumber.contains(Nokia_CPQ_Constants.MAINTY1CODE) &&
                                    !partNumber.contains(Nokia_CPQ_Constants.MAINTY2CODE) &&
                                   !partNumber.contains(Nokia_CPQ_Constants.SSPCODE) &&
                                   !partNumber.contains(Nokia_CPQ_Constants.SRS) &&
                                   !item.Is_List_Price_Only__c)
                                {
                                    if (item.Apttus_Config2__LineType__c != null && Nokia_CPQ_Constants.NOKIA_OPTION.equals(String.valueOf(item.Apttus_Config2__LineType__c)))
                                    {
                                        if (this.lineItemObjectMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES + Nokia_CPQ_Constants.NOKIA_UNDERSCORE + item.Apttus_Config2__LineNumber__c) != null)
                                        {
                                            if (this.lineItemObjectMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES + Nokia_CPQ_Constants.NOKIA_UNDERSCORE + item.Apttus_Config2__LineNumber__c).Apttus_Config2__Quantity__c != null)
                                            {
                                                quantityBundle = Integer.valueOf(this.lineItemObjectMap.get(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES + Nokia_CPQ_Constants.NOKIA_UNDERSCORE + item.Apttus_Config2__LineNumber__c).Apttus_Config2__Quantity__c.round(System.RoundingMode.CEILING));
                                            }
                                        }
                                    }

                                    if (item.Apttus_Config2__NetPrice__c != null && item.NokiaCPQ_SSP_Rate__c != null && !item.Apttus_Config2__ProductId__r.IsSSP__c)
                                    {
                                        item.Nokia_SSP_Base_Extended_Price__c = ((item.Apttus_Config2__NetPrice__c * item.NokiaCPQ_SSP_Rate__c * 0.01).setScale(2, RoundingMode.HALF_UP)) * quantityBundle;
                                    }

                                    if (item.Apttus_Config2__NetPrice__c != null && item.NokiaCPQ_SRS_Rate__c != null && item.Apttus_Config2__ProductId__r.IsSSP__c)
                                    {
                                        item.Nokia_SRS_Base_Extended_Price__c = ((item.Apttus_Config2__NetPrice__c * item.NokiaCPQ_SRS_Rate__c * 0.01).setScale(2, RoundingMode.HALF_UP)) * quantityBundle;
                                    }

                                    totalExtendedSSPPrice = totalExtendedSSPPrice + (item.Nokia_SSP_Base_Extended_Price__c).setScale(2, RoundingMode.HALF_UP);
                                    totalExtendedSRSPrice = totalExtendedSRSPrice + (item.Nokia_SRS_Base_Extended_Price__c).setScale(2, RoundingMode.HALF_UP);
                                }

                                if (partNumber != null && partNumber.contains(Nokia_CPQ_Constants.SSPCODE))
                                {
                                    ssp_srsLinesMap_EP.put('SSP', allLineItems);
                                }
                                else if (partNumber != null && partNumber.contains(Nokia_CPQ_Constants.SRS))
                                {
                                    ssp_srsLinesMap_EP.put('SRS', allLineItems);
                                }
                            }
                            //R-6508 End

                            //Logic for calculating Cost for main bundle

                            if (!mainBundleList.isEmpty() && !item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES) && mainBundleList.contains(String.valueOf(item.Apttus_Config2__ParentBundleNumber__c)))
                            {
                                if (unitaryCostMap.containsKey(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c))
                                {
                                    Double itemCost = 0.0;
                                    if ((configType.equalsIgnoreCase('Bundle') || item.NokiaCPQ_IsArcadiaBundle__c == true) && item.NokiaCPQ_Unitary_Cost__c != null)
                                    {
                                        itemCost = unitaryCostMap.get(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c) + item.NokiaCPQ_Unitary_Cost__c;
                                        unitaryCostMap.put(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c, itemCost);
                                    }
                                    else if (item.NokiaCPQ_Unitary_Cost__c != null)
                                    {
                                        itemCost = unitaryCostMap.get(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c) + (item.NokiaCPQ_Unitary_Cost__c * item.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                                        unitaryCostMap.put(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c, itemCost);
                                    }
                                }
                                else
                                {
                                    if ((configType.equalsIgnoreCase('Bundle') || item.NokiaCPQ_IsArcadiaBundle__c == true) && item.NokiaCPQ_Unitary_Cost__c != null)
                                    {
                                        unitaryCostMap.put(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c, item.NokiaCPQ_Unitary_Cost__c);
                                    }
                                    else if (item.NokiaCPQ_Unitary_Cost__c != null)
                                    {
                                        unitaryCostMap.put(item.Apttus_Config2__ParentBundleNumber__c + '_' + item.Apttus_Config2__ChargeType__c, (item.NokiaCPQ_Unitary_Cost__c * item.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP));
                                    }
                                }
                            }
                        }
                        // 6508  
                        Apttus_Config2__LineItem__c lineItemVarSO;
                        Boolean isUpdateSSP = False;
                        if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_IP_ROUTING) && ssp_srsLinesMap_EP.Size() > 0)
                        {
                            Apttus_Config2.LineItem lineItemVar;
                            lineItemVarSO = new Apttus_Config2__LineItem__c();

                            if (ssp_srsLinesMap_EP.containsKey('SSP'))
                            {
                                lineItemVar = ssp_srsLinesMap_EP.get('SSP');
                                lineItemVarSO = lineItemVar.getLineItemSO();

                                if (lineItemVarSO.Apttus_Config2__BasePriceOverride__c != totalExtendedSSPPrice)
                                {
                                    lineItemVarSO.Apttus_Config2__BasePriceOverride__c = totalExtendedSSPPrice;
                                    lineItemVarSO.NokiaCPQ_Unitary_IRP__c = totalExtendedSSPPrice;
                                    lineItemVarSO.Apttus_Config2__LineSequence__c = 998;
                                    isUpdateSSP = True;
                                }

                                if (isUpdateSSP)
                                {
                                    System.debug('checkcountSSP>>>');
                                    lineItemVar.updatePrice();
                                    isUpdateSSP = False;
                                }
                            }
                            if (ssp_srsLinesMap_EP.containsKey('SRS'))
                            {
                                lineItemVar = ssp_srsLinesMap_EP.get('SRS');
                                lineItemVarSO = lineItemVar.getLineItemSO();

                                if (lineItemVarSO.Apttus_Config2__BasePriceOverride__c != totalExtendedSRSPrice)
                                {
                                    lineItemVarSO.Apttus_Config2__BasePriceOverride__c = totalExtendedSRSPrice;
                                    lineItemVarSO.NokiaCPQ_Unitary_IRP__c = totalExtendedSRSPrice;
                                    lineItemVarSO.Apttus_Config2__LineSequence__c = 999;
                                    isUpdateSSP = True;
                                }

                                if (isUpdateSSP)
                                {
                                    System.debug('checkcountSRS>>>');
                                    lineItemVar.updatePrice();
                                    isUpdateSSP = False;
                                }
                            }
                        }
                        //6508 End
                    }
                    Map<Decimal, Decimal> mainBundleToGroupIRPMap = new Map<Decimal, Decimal>();
                    Map<Decimal, Decimal> mainBundleToGroupCLPMap = new Map<Decimal, Decimal>();
                    Map<Decimal, Decimal> mainBundleToGroupCUPMap = new Map<Decimal, Decimal>();
                    for (Apttus_Config2.LineItem allLineItems: this.cart.getLineItems())
                    {
                        Apttus_Config2__LineItem__c item = allLineItems.getLineItemSO();
                        String configType = getConfigType(item);
                        system.debug('insideForLoop');
                        if (item != null && item.Apttus_Config2__LineType__c != NULL && item.Apttus_Config2__ChargeType__c != NULL && item.Apttus_Config2__ChargeType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_YEAR1_MAINTENANCE) && !item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES) && this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase('Nokia Software'))
                        {
                            if (linenumberCareDisPercentage.size() > 0 && linenumberCareDisPercentage.containsKey(item.Apttus_Config2__LineNumber__c))
                            {
                                if (linenumberCareDisPercentage.get(item.Apttus_Config2__LineNumber__c) > 0)
                                {
                                    Decimal calculatedPrice = (((item.NokiaCPQ_CareSRSBasePrice__c / defaultExchangeRate.get(0).ConversionRate) * this.proposalSO.exchange_rate__c) * linenumberCareDisPercentage.get(item.Apttus_Config2__LineNumber__c)).setScale(2, RoundingMode.HALF_UP);

                                    if (item.Apttus_Config2__BasePriceOverride__c != calculatedPrice)
                                    {
                                        item.Apttus_Config2__BasePriceOverride__c = (((item.NokiaCPQ_CareSRSBasePrice__c / defaultExchangeRate.get(0).ConversionRate) * this.proposalSO.exchange_rate__c) * linenumberCareDisPercentage.get(item.Apttus_Config2__LineNumber__c)).setScale(2, RoundingMode.HALF_UP);
                                    }
                                }
                                linenumberCarePrice.put(item.Apttus_Config2__LineNumber__c, item.Apttus_Config2__BasePriceOverride__c.setScale(2, RoundingMode.HALF_UP));
                            }
                        }
                        if (item != null && item.Apttus_Config2__LineType__c != NULL && item.Apttus_Config2__ChargeType__c != NULL && item.Apttus_Config2__ChargeType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_SRS) && !item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES) && this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase('Nokia Software'))
                        {
                            if (linenumberSRSDisPercentage.size() > 0 && linenumberSRSDisPercentage.containsKey(item.Apttus_Config2__LineNumber__c))
                            {
                                if (linenumberSRSDisPercentage.get(item.Apttus_Config2__LineNumber__c) > 0)
                                {
                                    Decimal calculatedSRSPrice = (((item.NokiaCPQ_SRSBasePrice__c / defaultExchangeRate.get(0).ConversionRate) * this.proposalSO.exchange_rate__c) * linenumberSRSDisPercentage.get(item.Apttus_Config2__LineNumber__c)).setScale(2, RoundingMode.HALF_UP);

                                    if (item.Apttus_Config2__BasePriceOverride__c != calculatedSRSPrice)
                                    {
                                        item.Apttus_Config2__BasePriceOverride__c = (((item.NokiaCPQ_SRSBasePrice__c / defaultExchangeRate.get(0).ConversionRate) * this.proposalSO.exchange_rate__c) * linenumberSRSDisPercentage.get(item.Apttus_Config2__LineNumber__c)).setScale(2, RoundingMode.HALF_UP);
                                    }
                                }

                                linenumberSRSPrice.put(item.Apttus_Config2__LineNumber__c, item.Apttus_Config2__BasePriceOverride__c.setScale(2, RoundingMode.HALF_UP));
                            }
                        }
                        //Map for Stamping IRP,CLP,CUP for main bundle Airscale wifi Direct MN
                        if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING) && item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_OPTION) &&
                     Nokia_CPQ_Constants.Bundle.equalsIgnoreCase(configType))
                        {
                            if (mainBundleToGroupIRPMap.containsKey(item.Apttus_Config2__LineNumber__c))
                            {
                                if (item.NokiaCPQ_Unitary_IRP__c != Null)
                                    mainBundleToGroupIRPMap.put(item.Apttus_Config2__LineNumber__c, (mainBundleToGroupIRPMap.get(item.Apttus_Config2__LineNumber__c) + item.NokiaCPQ_Unitary_IRP__c * item.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP));
                            }
                            else
                            {
                                if (item.NokiaCPQ_Unitary_IRP__c != Null)
                                    mainBundleToGroupIRPMap.put(item.Apttus_Config2__LineNumber__c, (item.NokiaCPQ_Unitary_IRP__c * item.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP));
                            }

                            if (mainBundleToGroupCLPMap.containsKey(item.Apttus_Config2__LineNumber__c))
                            {
                                if (item.NCPQ_Unitary_CLP__c != Null)
                                    mainBundleToGroupCLPMap.put(item.Apttus_Config2__LineNumber__c, (mainBundleToGroupCLPMap.get(item.Apttus_Config2__LineNumber__c) + item.NCPQ_Unitary_CLP__c * item.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP));
                            }
                            else
                            {
                                if (item.NCPQ_Unitary_CLP__c != Null)
                                    mainBundleToGroupCLPMap.put(item.Apttus_Config2__LineNumber__c, (item.NCPQ_Unitary_CLP__c * item.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP));
                            }

                            if (mainBundleToGroupCUPMap.containsKey(item.Apttus_Config2__LineNumber__c))
                            {
                                if (item.NokiaCPQ_Extended_CUP__c != Null)
                                    mainBundleToGroupCUPMap.put(item.Apttus_Config2__LineNumber__c, (mainBundleToGroupCUPMap.get(item.Apttus_Config2__LineNumber__c) + item.NokiaCPQ_Extended_CUP__c).setScale(2, RoundingMode.HALF_UP));
                            }
                            else
                            {
                                if (item.NokiaCPQ_Extended_CUP__c != Null)
                                    mainBundleToGroupCUPMap.put(item.Apttus_Config2__LineNumber__c, (item.NokiaCPQ_Extended_CUP__c).setScale(2, RoundingMode.HALF_UP));
                            }
                        }
                        //Start: create map to limit for loop iterations
                        if (item.Apttus_Config2__LineType__c != NULL && item.Apttus_Config2__ChargeType__c != NULL
                         && item.Apttus_Config2__LineType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES))
                        {
                            mainBundleMap.put(item.Id, item);
                        }
                        //Stop

                        if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase('Nokia Software'))
                            allLineItems.updatePrice();
                    }

                    for (Apttus_Config2__LineItem__c item: mainBundleMap.values())
                    {
                        String configType = getConfigType(item);
                        String partNumber = getPartNumber(item);
                        if (item.Apttus_Config2__ChargeType__c != NULL && item.Apttus_Config2__ChargeType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_YEAR1_MAINTENANCE) && !this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING))
                        {
                            if (linenumberCarePrice.size() > 0 && linenumberCarePrice.containsKey(item.Apttus_Config2__LineNumber__c) && item.Apttus_Config2__Quantity__c > 0 && linenumberCarePrice.get(item.Apttus_Config2__LineNumber__c) > 0 && linenumberCarePrice.get(item.Apttus_Config2__LineNumber__c) != (item.Apttus_Config2__NetPrice__c / item.Apttus_Config2__Quantity__c))
                            {
                                item.Apttus_Config2__PricingStatus__c = 'Pending';
                            }
                        }
                        if (item.Apttus_Config2__ChargeType__c != NULL && item.Apttus_Config2__ChargeType__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_SRS) && !this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING))
                        {
                            if (linenumberSRSPrice.size() > 0 && linenumberSRSPrice.containsKey(item.Apttus_Config2__LineNumber__c) && item.Apttus_Config2__Quantity__c > 0 && linenumberSRSPrice.get(item.Apttus_Config2__LineNumber__c) > 0 && linenumberSRSPrice.get(item.Apttus_Config2__LineNumber__c) != (item.Apttus_Config2__NetPrice__c / item.Apttus_Config2__Quantity__c))
                            {
                                item.Apttus_Config2__PricingStatus__c = 'Pending';
                            }
                        }
                        //Stamping at main bundle
                        if (unitaryCostMap.containsKey(item.Apttus_Config2__PrimaryLineNumber__c + '_' + item.Apttus_Config2__ChargeType__c))
                        {
                            item.NokiaCPQ_Unitary_Cost__c = unitaryCostMap.get(item.Apttus_Config2__PrimaryLineNumber__c + '_' + item.Apttus_Config2__ChargeType__c);
                        }
                        //Stamping at main IRP, CLP, CUP bundle for Airscale wifi
                        if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING) &&
                     Nokia_CPQ_Constants.Bundle.equalsIgnoreCase(configType))
                        {
                            if (mainBundleToGroupIRPMap.size() > 0 && mainBundleToGroupIRPMap.containsKey(item.Apttus_Config2__LineNumber__c))
                            {
                                item.NokiaCPQ_Unitary_IRP__c = mainBundleToGroupIRPMap.get(item.Apttus_Config2__LineNumber__c);
                            }
                            if (mainBundleToGroupCLPMap.size() > 0 && mainBundleToGroupCLPMap.containsKey(item.Apttus_Config2__LineNumber__c))
                            {
                                item.NCPQ_Unitary_CLP__c = mainBundleToGroupCLPMap.get(item.Apttus_Config2__LineNumber__c);
                            }
                            if (mainBundleToGroupCUPMap.size() > 0 && mainBundleToGroupCUPMap.containsKey(item.Apttus_Config2__LineNumber__c))
                            {
                                item.NokiaCPQ_Extended_CUP__c = mainBundleToGroupCUPMap.get(item.Apttus_Config2__LineNumber__c);
                            }
                            if (item.NokiaCPQ_Unitary_IRP__c != Null && item.Apttus_Config2__Quantity__c != Null)
                                item.NokiaCPQ_Extended_IRP2__c = (item.NokiaCPQ_Unitary_IRP__c * item.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                            if (item.NCPQ_Unitary_CLP__c != Null && item.Apttus_Config2__Quantity__c != Null)
                                item.NokiaCPQ_Extended_CLP_2__c = (item.NCPQ_Unitary_CLP__c * item.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                            if (item.NokiaCPQ_Extended_CUP__c != Null && item.Apttus_Config2__Quantity__c != Null)
                                item.NokiaCPQ_Extended_CUP_2__c = (item.NokiaCPQ_Extended_CUP__c * item.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP);
                        }

                        //For Enterprise
                        if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_IP_ROUTING) && !this.proposalSO.Is_List_Price_Only__c)
                        {
                            //system.debug('enterprise 2nd reprice for SSP/SRS' + item.NokiaCPQ_Extended_IRP2__c + '  ' + item.Apttus_Config2__BasePriceOverride__c*item.Apttus_Config2__Quantity__c);
                            if (partNumber != null && (partNumber.contains(Nokia_CPQ_Constants.SSPCODE) || partNumber.contains(Nokia_CPQ_Constants.SRS)) && item.Apttus_Config2__Quantity__c != null && item.NokiaCPQ_Extended_IRP2__c != (item.Apttus_Config2__BasePriceOverride__c * item.Apttus_Config2__Quantity__c).setScale(2, RoundingMode.HALF_UP))
                            {
                                item.Apttus_Config2__PricingStatus__c = 'Pending';
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            //ExceptionHandler.addException(ex, Nokia_PricingCallBack.class.getName(),'Finish Method and line number is'+ex.getLineNumber());
        }
    }

    /******************************************************
	Method Name: Deal_Guidance_Calculator
	Date: 03/10/2019
	Description: This method calculates how good deal is
	and shows guidance based on this calculations(Req 6670,6671,6500)
	Author: Piyush Tawari, Kamlesh Jandu
	Parameters: Apttus_Config2__LineItem__c
	******************************************************/
    public void deal_Guidance_Calculator(Apttus_Config2__LineItem__c item, String configType)
    {
        //system.debug('In Deal guidance');
        //CPQ Requirement : Traffic Light calculations For MN Direct
        if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.AIRSCALE_WIFI_STRING) && !configType.equalsIgnoreCase('Bundle'))
        {
            Boolean contractedPL = getCLP(item);
            Boolean isOEM = getOEM(item);
            Decimal maxIRPDiscount = getMaximumIRPDiscount(item);
            String itemType = getItemType(item);
            if (item.Apttus_Config2__ChargeType__c != Nokia_CPQ_Constants.STANDARD_PRICE && item.Apttus_Config2__LineType__c == Nokia_CPQ_Constants.NOKIA_PRODUCT_SERVICES)
            {
                if (item.Apttus_Config2__NetAdjustmentPercent__c < 0 || (!contractedPL && item.NokiaCPQ_Extended_IRP__c != item.NokiaCPQ_Extended_CLP__c))
                {
                    item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.RED;
                }
                else
                    item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.GREEN;
            }
            else
            {
                if (!contractedPL &&
                ((!isOEM && (itemType == Nokia_CPQ_Constants.HARDWARE_STRING || itemType == Nokia_CPQ_Constants.SOFTWARE_STRING)) ||
                    (isOEM && (itemType == Nokia_CPQ_Constants.HARDWARE_STRING || itemType == Nokia_CPQ_Constants.SOFTWARE_STRING || itemType == Nokia_CPQ_Constants.SERVICE_STRING))))
                {
                    if (item.NokiaCPQ_IRP_Discount__c <= maxIRPDiscount)
                    {
                        item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.GREEN;
                    }
                    else
                        item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.RED;
                }

                if (contractedPL &&
                ((!isOEM && (itemType == Nokia_CPQ_Constants.HARDWARE_STRING || itemType == Nokia_CPQ_Constants.SOFTWARE_STRING)) ||
                    (isOEM && (itemType == Nokia_CPQ_Constants.HARDWARE_STRING || itemType == Nokia_CPQ_Constants.SOFTWARE_STRING || itemType == Nokia_CPQ_Constants.SERVICE_STRING))))
                {
                    if (item.Apttus_Config2__NetAdjustmentPercent__c != 0)
                    {
                        item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.RED;
                    }
                    else
                        item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.GREEN;
                }

                if (!isOEM && itemType == Nokia_CPQ_Constants.SERVICE_STRING)
                {
                    if (item.Apttus_Config2__NetAdjustmentPercent__c < 0 || item.NokiaCPQ_ExtendedPrice_CNP__c == 0 ||
                 (!contractedPL && item.NokiaCPQ_Extended_IRP__c != item.NokiaCPQ_Extended_CLP__c))
                    {
                        item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.RED;
                    }
                    else
                        item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.GREEN;
                }
            }
        }
        //CPQ Requirement : Traffic Light calculations For Direct NSW
        else if (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_SOFTWARE) && !configType.equalsIgnoreCase('Bundle'))
        {
            Boolean contractedPL = getCLP(item);
            Decimal maxIRPDiscount = getMaximumIRPDiscount(item);
            Decimal minSalesMargin = getMinimumSalesMargin(item);
            if (item.NokiaCPQ_Is_Direct_Option__c == true && ((item.Apttus_Config2__NetAdjustmentPercent__c != 0 && item.Apttus_Config2__NetAdjustmentPercent__c != null) ||
                ((item.Apttus_Config2__NetAdjustmentPercent__c == null || item.Apttus_Config2__NetAdjustmentPercent__c == 0) && !contractedPL)) && item.Item_Type_From_CAT__c == 'PS')
            {
                if (item.Sales_Margin__c >= minSalesMargin)
                {
                    item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.GREEN;
                }
                else
                {
                    item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.RED;
                }
            }

            if ((item.Apttus_Config2__NetAdjustmentPercent__c == 0 || item.Apttus_Config2__NetAdjustmentPercent__c == null) && (item.NokiaCPQ_Is_Direct_Option__c == true && contractedPL))
            {
                item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.GREEN;
            }

            if (item.NokiaCPQ_Is_Direct_Option__c == true && ((item.Apttus_Config2__NetAdjustmentPercent__c != null && item.Apttus_Config2__NetAdjustmentPercent__c != 0) ||
                ((item.Apttus_Config2__NetAdjustmentPercent__c == 0 || item.Apttus_Config2__NetAdjustmentPercent__c == null) && !contractedPL)) && item.Item_Type_From_CAT__c != 'PS')
            {
                if (item.NokiaCPQ_IRP_Discount__c <= maxIRPDiscount)
                {
                    item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.GREEN;
                }
                else
                {
                    item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.RED;
                }
            }
        }
        //Traffic Light calculations For QTC
        else if (this.proposalSO.NokiaCPQ_Portfolio__c != null &&
    this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase('QTC'))
        {
            //system.debug('Entered guidance for QTC');
            if (item.NokiaCPQ_Floor_Price__c == null)
            {
                item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.RED;
            }
            else if ((item.Apttus_Config2__NetPrice__c < (item.Apttus_Config2__Quantity__c * item.NokiaCPQ_Floor_Price__c)) || item.NokiaCPQ_Custom_Bid__c)
            {
                item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.RED;
            }
            else if ((item.Apttus_Config2__NetPrice__c >= (item.Apttus_Config2__Quantity__c * item.NokiaCPQ_Floor_Price__c)) &&
                   (item.Apttus_Config2__NetPrice__c < (item.Apttus_Config2__Quantity__c * item.NokiaCPQ_Floor_Price__c * ((100 + pricingGuidanceSetting[0].Threshold__c) / 100))))
            {
                item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.YELLOW;
            }
            else
            {
                item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.GREEN;
            }
        }
        //Traffic Light calculations For Enterprise
        else if (this.proposalSO.NokiaCPQ_Portfolio__c != null &&
    (this.proposalSO.NokiaCPQ_Portfolio__c.equalsIgnoreCase(Nokia_CPQ_Constants.NOKIA_IP_ROUTING)
    && this.proposalSO.Is_List_Price_Only__c == false && !configType.equalsIgnoreCase('Bundle')
    && item.Apttus_Config2__ChargeType__c == Nokia_CPQ_Constants.STANDARD_PRICE
    && !item.is_Custom_Product__c))
        {
            //system.debug('Entered guidance for Enterprise');
            Boolean contractedPL = getCLP(item);
            if (contractedPL && item.Apttus_Config2__NetAdjustmentPercent__c == 0)
            {
                item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.GREEN;
            }
            else
            {
                if (item.NokiaCPQ_Floor_Price__c == null || item.NokiaCPQ_Custom_Bid__c)
                {
                    item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.RED;
                }
                else if (item.Apttus_Config2__NetPrice__c < (item.Apttus_Config2__Quantity__c * item.NokiaCPQ_Floor_Price__c))
                {
                    item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.RED;
                }
                else if ((item.Apttus_Config2__NetPrice__c >= (item.Apttus_Config2__Quantity__c * item.NokiaCPQ_Floor_Price__c)) &&
            (item.Apttus_Config2__NetPrice__c < (item.Apttus_Config2__Quantity__c * item.NokiaCPQ_Floor_Price__c * ((100 + pricingGuidanceSetting[0].Threshold__c) / 100))))
                {
                    item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.YELLOW;
                }
                else
                {
                    item.NokiaCPQ_Light_Color__c = Nokia_CPQ_Constants.GREEN;
                }
            }
        }

    }

    /*Start: Methods for replacing usage of formula fields for improving performance*/
    Boolean getOEM(Apttus_Config2__LineItem__c item)
    {
        Boolean oem = false;
        if ('Product/Service'.equalsIgnoreCase(item.Apttus_Config2__LineType__c))
        {
            oem = item.Apttus_Config2__ProductId__r.NokiaCPQ_OEM__c;
        }
        else
        {
            oem = item.Apttus_Config2__OptionId__r.NokiaCPQ_OEM__c;
        }
        return oem;
    }

    Decimal getMaximumIRPDiscount(Apttus_Config2__LineItem__c item)
    {
        Decimal maximumIRPDiscount = 0.00;
        if ('Product/Service'.equalsIgnoreCase(item.Apttus_Config2__LineType__c))
        {
            maximumIRPDiscount = item.Apttus_Config2__ProductId__r.NokiaCPQ_Maximum_IRP_Discount__c;
        }
        else
        {
            maximumIRPDiscount = item.Apttus_Config2__OptionId__r.NokiaCPQ_Maximum_IRP_Discount__c;
        }

        if (maximumIRPDiscount == null)
        {
            maximumIRPDiscount = 0.00;
        }

        return maximumIRPDiscount;
    }

    Boolean getCLP(Apttus_Config2__LineItem__c item)
    {
        Boolean clp = false;
        if (item.Apttus_Config2__PriceListId__c != item.Apttus_Config2__PriceListItemId__r.Apttus_Config2__PriceListId__c &&
    item.Apttus_Config2__PriceListItemId__r.Contracted__c == 'Yes')
        {
            clp = true;
        }
        else
        {
            clp = false;
        }
        return clp;
    }

    Decimal getMinimumSalesMargin(Apttus_Config2__LineItem__c item)
    {
        Decimal salesMargin = 0;
        if (item.NokiaCPQ_Account_Region__c == 'RG_NAM')
            salesMargin = item.Apttus_Config2__OptionId__r.NokiaCPQ_Min_SM_North_America__c;
        else if (item.NokiaCPQ_Account_Region__c == 'RG_LAM')
            salesMargin = item.Apttus_Config2__OptionId__r.NokiaCPQ_Min_SM_Latin_America__c;
        else if (item.NokiaCPQ_Account_Region__c == 'RG_MEA')
            salesMargin = item.Apttus_Config2__OptionId__r.NokiaCPQMin_SM_Middle_East_and_Africa__c;
        else if (item.NokiaCPQ_Account_Region__c == 'RG_ASIA')
        {
            if (this.proposalSO.Apttus_Proposal__Account__r.CountryCode__c == 'IN' || this.proposalSO.Apttus_Proposal__Account__r.CountryCode__c == 'BT' || this.proposalSO.Apttus_Proposal__Account__r.CountryCode__c == 'NP')
            {
                salesMargin = item.Apttus_Config2__OptionId__r.NokiaCPQ_Min_SM_India__c;
            }
            else
            {
                salesMargin = item.Apttus_Config2__OptionId__r.NokiaCPQ_Min_SM_Asia_Pacific_Japan__c;
            }
        }
        else if (item.NokiaCPQ_Account_Region__c == 'RG_CHINA')
            salesMargin = item.Apttus_Config2__OptionId__r.NokiaCPQ_Min_SM_Greater_China__c;
        else if (item.NokiaCPQ_Account_Region__c == 'RG_EUROPE')
            salesMargin = item.Apttus_Config2__OptionId__r.NokiaCPQ_Min_SM_Europe__c;
        else
            salesMargin = 0;

        if (salesMargin == null)
        {
            salesMargin = 0;
        }

        return salesMargin;
    }
    /*End: Methods for replacing usage of formula fields for improving performance*/
}
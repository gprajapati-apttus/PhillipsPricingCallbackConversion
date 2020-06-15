public class APTS_CustomPricingCallBackHelper_Ultra
{
    public final string TIER_1 = 'Tier 1';
    public final string TIER_2 = 'Tier 2';
    public final string TIER_3 = 'Tier 3';
    public final string TIER_4 = 'Tier 4';

    public Apttus_Proposal__Proposal__c proposalSO;
    public String pricingMode = null;

    public final List<String> ListTradeSpoo = new List<String> { 'Trade-In', 'Trade-In PO', 'Trade-In Return' }; 
    public final String SYSTEM_TYPE_DEMO = 'Demo';
    public final String SYSTEM_TYPE_3RD_PARTY = '3rd Party';
    public final String SYSTEM_TYPE_PHILIPS = 'Philips';
    public final String SYSTEM_TYPE_SERVICE = 'Service';
    public Map<Id, Apttus_Config2__PriceListItem__c> mapPLiSOSPOO = new Map<Id, Apttus_Config2__PriceListItem__c>();

    //Variables for Contract Pricing
    public Map<Id, Apttus_Config2__PriceListItem__c> mapPliSO = new Map<Id, Apttus_Config2__PriceListItem__c>();
    public String strVolumeTier;
    public Map<Id, String> mapMemberTiers = new Map<Id, String>();
    public Set<Id> setPliRelatedAgreement = new Set<Id>();

    public APTS_CustomPricingCallBackHelper_Ultra(Apttus_Proposal__Proposal__c proposal)
    {
        proposalSO = proposal;
    }

    public void populateMapPliAgreementContractsBeforePricing(List<Apttus_Config2__LineItem__c> listLiSO)
    {
        Set<Id> prodIds = new Set<Id>();
        Set<String> contractNumberSet = new Set<String>();

        for (Apttus_Config2__LineItem__c liSO : listLiSO)
        {
            //populate Map when below if condition satisfy
            //As part of US520672
            if (liSO.Apttus_Config2__ContractNumbers__c != null && liSO.Apttus_Config2__PricingStatus__c == 'Pending')
            {  
                if (liSO.Apttus_Config2__OptionId__c != null)
                    prodIds.add(liSO.Apttus_Config2__OptionId__c);
                else
                    prodIds.add(liSO.Apttus_Config2__ProductId__c);
                contractNumberSet.add(liSO.Apttus_Config2__ContractNumbers__c);//As part of US520672
            }
        }

        if (proposalSO.Account_Sold_to__c != null)
        {
            for (Apttus_Config2__PriceListItem__c pli : [SELECT Id, APTS_Country_Pricelist_List_Price__c, APTS_Agreement_Group__c, Apttus_Config2__ProductId__c,
                    APTS_Tier_1_List_Price__c, APTS_Tier_2_List_Price__c, APTS_Tier_3_List_Price__c, APTS_Tier_4_List_Price__c,
                    APTS_Tier_1_Discount__c, APTS_Tier_2_Discount__c, APTS_Tier_3_Discount__c, APTS_Tier_4_Discount__c,
                    APTS_Tier_1_Pre_Escalation_Price__c, APTS_Tier_2_Pre_Escalation_Price__c, APTS_Tier_3_Pre_Escalation_Price__c, APTS_Tier_4_Pre_Escalation_Price__c,
                    APTS_Tier_1_Target_Price__c, APTS_Tier_2_Target_Price__c, APTS_Tier_3_Target_Price__c, APTS_Tier_4_Target_Price__c,
                    Apttus_Config2__MinPrice__c, APTS_Tier_1_Minimum_Price__c, APTS_Tier_2_Minimum_Price__c, APTS_Tier_3_Minimum_Price__c, APTS_Tier_4_Minimum_Price__c,
                    Apttus_Config2__ContractNumber__c, Apttus_Config2__PriceListId__r.APTS_Related_Agreement__c
                    FROM Apttus_Config2__PriceListItem__c WHERE Apttus_Config2__ProductId__c IN: prodIds AND Apttus_Config2__ContractNumber__c IN: contractNumberSet]) {
                mapPliSO.put(pli.Id, pli);

                if (pli.Apttus_Config2__PriceListId__r.APTS_Related_Agreement__c != null)
                    setPliRelatedAgreement.add(pli.Apttus_Config2__PriceListId__r.APTS_Related_Agreement__c);
            }
        }

        try
        {
            for (APTS_Account_Contract__c oAccountContract : [SELECT Id, APTS_Member__c,
                    APTS_Agreement_Group__c, APTS_Volume_Tier__c, APTS_Related_Agreement__c,
                    APTS_Related_Agreement__r.APTS_Default_Volume_Tier__c

                    FROM APTS_Account_Contract__c

                    WHERE APTS_Member__c = :proposalSO.Account_Sold_to__c
                                           AND APTS_Related_Agreement__c IN: setPliRelatedAgreement
                                           AND(APTS_End_Date__c >= TODAY OR(APTS_End_Date__c = null AND APTS_Related_Agreement__r.Apttus__Contract_End_Date__c >= TODAY))
                                           AND(APTS_Start_Date__c <= TODAY OR(APTS_Start_Date__c = null AND  APTS_Related_Agreement__r.Apttus__Contract_Start_Date__c < = TODAY))]) {

                if (oAccountContract != null)
                {
                    if (oAccountContract.APTS_Agreement_Group__c != null)
                    {
                        if (oAccountContract.APTS_Volume_Tier__c != null)
                            strVolumeTier = oAccountContract.APTS_Volume_Tier__c;
                        mapMemberTiers.put(oAccountContract.APTS_Agreement_Group__c, strVolumeTier);
                    }
                }
            }
        }
        catch (Exception e)
        {
            System.debug('*****Error querying APTS_Account_Contract__c in finish method, PCB: ' + e.getMessage());
        }
    }

    //GP: This will be removed in a month...
    /* Author: Bogdan Botcharov, August 10, 2018
     * Purpose: DE58299/DE58523/DE58574/DE58697 - this corrects adjustments with 'Enable Adjustment Spread' set to true in Config System Properties
     */
    public void setDiscountWithAdjustmentSpread(List<Apttus_Config2__LineItem__c> allLines)
    {
        Map<Decimal, Apttus_Config2__LineItem__c> mapBundleDiscounts = new Map<Decimal, Apttus_Config2__LineItem__c>();

        for (Apttus_Config2__LineItem__c liSO : allLines)
        {

            liSO.APTS_Promotion_Discount_Amount_c__c = liSO.Apttus_Config2__IncentiveAdjustmentAmount__c != null ? formatPrecisionCeiling(liSO.Apttus_Config2__IncentiveAdjustmentAmount__c) * -1 : null;

            if (liSO.Apttus_Config2__OptionId__c == null /*&& liSO.Apttus_Config2__AdjustmentType__c != null && liSO.Apttus_Config2__AdjustmentType__c == 'Discount %'*/)
                mapBundleDiscounts.put(liSO.Apttus_Config2__LineNumber__c, liSO);
            Decimal adjustmentsAmount = mapBundleDiscounts.get(liSO.Apttus_Config2__LineNumber__c).Apttus_Config2__AdjustmentAmount__c;
            adjustmentsAmount = formatPrecisionCeiling(adjustmentsAmount);


            if (liSO.Apttus_Config2__OptionId__c != null && liSO.Apttus_Config2__AllocateGroupAdjustment__c == true &&
                       mapBundleDiscounts.containsKey(liSO.Apttus_Config2__LineNumber__c) &&
                       mapBundleDiscounts.get(liSO.Apttus_Config2__LineNumber__c).Apttus_Config2__AdjustmentAmount__c != null &&
                       mapBundleDiscounts.get(liSO.Apttus_Config2__LineNumber__c).Apttus_Config2__AdjustmentType__c != null)
            {
                if (mapBundleDiscounts.containsKey(liSO.Apttus_Config2__LineNumber__c))
                {

                    String bundleAdjTYpe = mapBundleDiscounts.get(liSO.Apttus_Config2__LineNumber__c).Apttus_Config2__AdjustmentType__c;
                    if (bundleAdjTYpe != 'Price Override' && bundleAdjTYpe != 'Discount Amount')
                    {
                        liSO.Apttus_Config2__AdjustmentType__c = bundleAdjTYpe;
                        liSO.Apttus_Config2__AdjustmentAmount__c = adjustmentsAmount;
                    }

                }
            }
            else if (mapBundleDiscounts.containsKey(liSO.Apttus_Config2__LineNumber__c) && liSO.Apttus_Config2__OptionId__c != null && liSO.Apttus_Config2__AllocateGroupAdjustment__c
                     && liSO.Apttus_Config2__AdjustmentType__c == null)
            {
                liSO.Apttus_Config2__AdjustmentAmount__c = null;
                liSO.Apttus_Config2__AdjustmentType__c = null;
            }
        }
    }

    //DE56939
    public void SetRollupsAndThresholdFlags(Apttus_Config2__ProductConfiguration__c prodConfigSO, List<Apttus_Config2__LineItem__c> lineItems)
    {
        Map<String, Double> mapClogToThresoldValue = new Map<String, Double>();
        list<Threshold_Approvers__c> lstCountryApprTH = [select APTS_3rd_Party_Threshold__c, APTS_PM_Percentage_Threshold__c,
                                     APTS_Installation_Threshold__c, APTS_Quote_Total_Threshold__c, APTS_Turnover_Threshold__c, APTS_FSE_Threshold__c
                                     from Threshold_Approvers__c where APTS_Sales_Organization__c = : prodConfigSO.APTS_Sales_Organization__c limit 1];

        List<APTS_Procurement_Approval__c> listProcurementApp = [select APTS_Threshold_Value__c, APTS_CLOGS__c
                                                                        FROM APTS_Procurement_Approval__c
                                                                        WHERE APTS_Sales_Organization__c = : prodConfigSO.APTS_Sales_Organization__c];

        for (APTS_Procurement_Approval__c proc : listProcurementApp)
        {
            mapClogToThresoldValue.put(proc.APTS_CLOGS__c, proc.APTS_Threshold_Value__c);
        }
        prodConfigSO.APTS_3rd_Party_Total_Required__c = false;
        prodConfigSO.APTS_Installation_Cost_Required__c = false;
        prodConfigSO.APTS_Quote_Total_Required__c = false;
        prodConfigSO.APTS_3rd_Party_Totals_PCB__c = 0;
        prodConfigSO.APTS_Total_Net_Price_PCB__c = 0;
        prodConfigSO.APTS_PM_Approval_Required__c = false;
        prodConfigSO.APTS_Finance_Required__c = false;
        double CalculatedThreshold_3rdP = 0;
        double CalculatedThreshold_QT = 0;
        double CalculatedThreshold_Ins = 0;
        double CalculatedThreshold_prjmgr = 0;

        CalculatedThreshold_prjmgr = prodConfigSO.APTS_MCOS__c + prodConfigSO.APTS_DSE__c;
        Set<String> setValuationclass = new Set<String>();

        for (APTS_Valuation_Class_Mapping__c valClassSetting : [SELECT Name, APTS_Column__c FROM APTS_Valuation_Class_Mapping__c
                WHERE APTS_Column__c = '3rd party turnover']) {
            setValuationclass.add(valClassSetting.Name);
        }

        for (Apttus_Config2__LineItem__c lineItem : lineItems)
        {
            //Mithilesh Maurya :  adding null check for prod issue
            if (lineItem.Apttus_Config2__ProductId__r.APTS_Type__c == 'SPOO' && lineItem.Apttus_Config2__ProductId__r.APTS_SPOO_Type__c == '3rd Party' && lineitem.Apttus_Config2__NetPrice__c != null)
                CalculatedThreshold_3rdP = CalculatedThreshold_3rdP + formatPrecisionCeiling(lineitem.Apttus_Config2__NetPrice__c);

            if (lineItem.Apttus_Config2__LineType__c == 'Product/Service' && lineitem.Apttus_Config2__NetPrice__c != null)
                CalculatedThreshold_QT = CalculatedThreshold_QT + formatPrecisionCeiling(lineitem.Apttus_Config2__NetPrice__c);

            //Added by Mithilesh Maurya for Story#259996
            if (mapClogToThresoldValue.containsKey(lineItem.Apttus_Config2__ProductId__r.APTS_CLOGS__c)
                        && lineitem.APTS_Extended_List_Price__c >= mapClogToThresoldValue.get(lineItem.Apttus_Config2__ProductId__r.APTS_CLOGS__c))
            {
                lineItem.APTS_Procurement_approval_needed__c = true;
            }
            else
            {
                lineItem.APTS_Procurement_approval_needed__c = false;
            }
        }
        prodConfigSO.APTS_3rd_Party_Totals_PCB__c = CalculatedThreshold_3rdP;
        prodConfigSO.APTS_Total_Net_Price_PCB__c = CalculatedThreshold_QT;

        if (!lstCountryApprTH.isempty())
        {
            Threshold_Approvers__c objApprTH = lstCountryApprTH[0];

            if (CalculatedThreshold_3rdP >= objApprTH.APTS_3rd_Party_Threshold__c)
                prodConfigSO.APTS_3rd_Party_Total_Required__c = true;

            if (CalculatedThreshold_QT >= objApprTH.APTS_Quote_Total_Threshold__c)
                prodConfigSO.APTS_Quote_Total_Required__c = true;

            if (prodConfigSO.APTS_Turnover__c > objApprTH.APTS_Turnover_Threshold__c || prodConfigSO.APTS_FSE__c > objApprTH.APTS_FSE_Threshold__c)
                prodConfigSO.APTS_Finance_Required__c = true;

            Decimal Install_Threshold = 0;
            Decimal PM_Percentage_Threshold = 0;
            if (objApprTH.APTS_Installation_Threshold__c != NULL)
                Install_Threshold = objApprTH.APTS_Installation_Threshold__c;

            if (objApprTH.APTS_PM_Percentage_Threshold__c != null)
                PM_Percentage_Threshold = objApprTH.APTS_PM_Percentage_Threshold__c;

            Decimal thresholdmgrvalue = 0;
            if (prodConfigSO.APTS_Offer_Price__c != null && prodConfigSO.APTS_Offer_Price__c > 0)
            {
                thresholdmgrvalue = CalculatedThreshold_prjmgr / prodConfigSO.APTS_Offer_Price__c;
            }

            if (CalculatedThreshold_prjmgr > Install_Threshold || thresholdmgrvalue > PM_Percentage_Threshold)
            {
                prodConfigSO.APTS_PM_Approval_Required__c = true;
            }
        }
    }


    public void populateExtendedQuantity(List<Apttus_Config2__LineItem__c> listLiSO)
    {
        Map<String, decimal> bundleLineQtyMap = new Map<String, decimal>();
        Map<String, String> bundleLineAdjTYpeMap = new Map<String, String>();
        Map<String, Boolean> isOptionalLineQtyMap = new Map<String, Boolean>();

        for (Apttus_Config2__LineItem__c liSO : listLiSO)
        {
            //Added by Sridhar to populate extended quantity
            liSO.Apts_Local_Bundle_Header__c = null;
            liSO.Apts_Local_Bundle_Component__c = null;
            if (liSO.Apttus_COnfig2__LineType__c == 'Product/Service')
            {
                bundleLineQtyMap.put(liSO.Apttus_COnfig2__LineNumber__c + '' + liSO.Apttus_COnfig2__ProductId__c, liSO.Apttus_COnfig2__Quantity__c);
                if (liSO.Apttus_Config2__IsOptional__c)
                    isOptionalLineQtyMap.put(liSO.Apttus_COnfig2__LineNumber__c + '' + liSO.Apttus_COnfig2__ProductId__c, liSO.Apttus_Config2__IsOptional__c);
            }
        }
        //Added by Sridhar to populate extended quantity
        for (Apttus_Config2__LineItem__c liSO : listLiSO)
        {
            if (liSO.Apttus_Config2__LineType__c == 'Option')
            {
                Decimal bundleQty = bundleLineQtyMap.get(liSO.Apttus_Config2__LineNumber__c + '' + liSO.Apttus_Config2__ProductId__c);
                liSO.APTS_Extended_Quantity__c = bundleQty != null ? bundleQty * liSO.Apttus_Config2__Quantity__c : liSO.Apttus_Config2__Quantity__c;
                liSO.APTS_Bundle_Quantity__c = bundleQty != null ? bundleQty : liSO.Apttus_Config2__Quantity__c;
                Boolean isOptionalLine = isOptionalLineQtyMap.get(liSO.Apttus_Config2__LineNumber__c + '' + liSO.Apttus_Config2__ProductId__c);
                if (isOptionalLine != null)
                {
                    liSO.Apttus_COnfig2__IsOptional__c = isOptionalLine;
                }
            }
            else
            {
                liSO.APTS_Extended_Quantity__c = liSO.Apttus_Config2__Quantity__c;
                liSO.APTS_Bundle_Quantity__c = liSO.Apttus_Config2__Quantity__c;
            }
        }

    }

    //462170 - Added by Bogdan Botcharov [November 4 2019]
    public void incentiveAdjustmentUnitRounding(List<Apttus_Config2__LineItem__c> listLiSO)
    {
        for (Apttus_Config2__LineItem__c liSO : listLiSO)
        {
            if (liSO.Apttus_Config2__IncentiveBasePrice__c != null && liSO.Apttus_Config2__IncentiveBasePrice__c != 0
&& liSO.Apttus_Config2__BasePrice__c != null && liSO.Apttus_Config2__BasePrice__c != 0)
            {

                System.debug('*****ADJUSTMENT mode Apttus_Config2__IncentiveAdjustmentAmount__c: ' + liSO.Apttus_Config2__IncentiveAdjustmentAmount__c);
                System.debug('*****ADJUSTMENT mode Apttus_Config2__IncentiveBasePrice__c: ' + liSO.Apttus_Config2__IncentiveBasePrice__c);
                System.debug('*****ADJUSTMENT mode Apttus_Config2__BasePrice__c: ' + liSO.Apttus_Config2__BasePrice__c);

                Decimal sellingTerm = liSO.Apttus_Config2__SellingTerm__c != null && liSO.Apttus_Config2__SellingTerm__c != 0 ? liSO.Apttus_Config2__SellingTerm__c : 1;
                Decimal lineItemQ = liSO.Apttus_Config2__Quantity__c != null && liSO.Apttus_Config2__Quantity__c != 0 ? liSO.Apttus_Config2__Quantity__c : 1;

                Decimal unitIncentiveAmount = liSO.Apttus_Config2__BasePrice__c - liSO.Apttus_Config2__IncentiveBasePrice__c;
                unitIncentiveAmount = unitIncentiveAmount.setScale(2);

                liSO.APTS_Unit_Incentive_Adj_Amount__c = unitIncentiveAmount;
                liSO.Apttus_Config2__IncentiveBasePrice__c = liSO.Apttus_Config2__BasePrice__c - unitIncentiveAmount;
                liSO.Apttus_Config2__IncentiveAdjustmentAmount__c = unitIncentiveAmount * lineItemQ * sellingTerm * -1;//498878 [Nov 18 2019]
                System.debug('*****after rounding Apttus_Config2__IncentiveAdjustmentAmount__c: ' + liSO.Apttus_Config2__IncentiveAdjustmentAmount__c);
                System.debug('*****after rounding Apttus_Config2__IncentiveBasePrice__c: ' + liSO.Apttus_Config2__IncentiveBasePrice__c);
            }
        }
    }

    /* Author: Bogdan Botcharov, August 2, 2018
     * Purpose: F22944
     */
    public void populateMapSPOOPLi(List<Apttus_Config2__LineItem__c> listLiSO)
    {
        Set<Id> spooProdIds = new Set<Id>();
        for (Apttus_Config2__LineItem__c liSO : listLiSO)
            if (String.isNotBlank(liSO.Apttus_Config2__ProductId__r.APTS_SPOO_Type__c))
                spooProdIds.add(liSO.Apttus_Config2__ProductId__c);

        if (!spooProdIds.isEmpty())
        {
            for (Apttus_Config2__PriceListItem__c pli : [SELECT Id, Apttus_Config2__ProductId__c, APTS_List_Price_Multiplier__c, APTS_Target_Price_Multiplier__c,
                    APTS_Escalation_Price_Multiplier__c, APTS_Minimum_Price_Multiplier__c
                    FROM Apttus_Config2__PriceListItem__c WHERE Apttus_Config2__ProductId__c IN: spooProdIds])
                mapPLiSOSPOO.put(pli.Id, pli);
        }
    }

    public void calculateSPOOPricing(Apttus_Config2__PriceListItem__c itemSO, Apttus_Config2__LineItem__c lineItem)
    {
        //Check Product2 Field for SPOO - only then proceed
        if (String.isNotBlank(lineItem.Apttus_Config2__ProductId__r.APTS_SPOO_Type__c))
        {
            //Cost Variables
            Decimal costPrice, listPriceMultiplier, minPriceMultiplier, targetPriceMultiplier, escPriceMultiplier, listPrice, minPrice, targetPrice, escPrice, fairMarketValue;//netPriceResale,
            //Text variables
            String systemType, description, spooCategory, valuationClass;

            //Apttus_Config2__PriceListItem__c variables
            listPriceMultiplier = mapPLiSOSPOO.get(itemSO.Id).APTS_List_Price_Multiplier__c != null ? mapPLiSOSPOO.get(itemSO.Id).APTS_List_Price_Multiplier__c : 1;
            minPriceMultiplier = mapPLiSOSPOO.get(itemSO.Id).APTS_Minimum_Price_Multiplier__c != null ? mapPLiSOSPOO.get(itemSO.Id).APTS_Minimum_Price_Multiplier__c : 1;
            targetPriceMultiplier = mapPLiSOSPOO.get(itemSO.Id).APTS_Target_Price_Multiplier__c != null ? mapPLiSOSPOO.get(itemSO.Id).APTS_Target_Price_Multiplier__c : 1;
            escPriceMultiplier = mapPLiSOSPOO.get(itemSO.Id).APTS_Escalation_Price_Multiplier__c != null ? mapPLiSOSPOO.get(itemSO.Id).APTS_Escalation_Price_Multiplier__c : 1;

            //Apttus_Config2__AttributeValue__c variables
            costPrice = lineItem.Apttus_Config2__AttributeValueId__r.APTS_Cost_Price__c != null ? lineItem.Apttus_Config2__AttributeValueId__r.APTS_Cost_Price__c : 0;
            fairMarketValue = lineItem.Apttus_Config2__AttributeValueId__r.APTS_Fair_Market_Value__c != null ? lineItem.Apttus_Config2__AttributeValueId__r.APTS_Fair_Market_Value__c : 0;
            listPrice = lineItem.Apttus_Config2__AttributeValueId__r.APTS_List_Price__c != null ? lineItem.Apttus_Config2__AttributeValueId__r.APTS_List_Price__c : 0;
            //Changed by Yoncho Yonchev for DE59261 on 17 August 2018. SPOO description.
            //description             = lineItem.Apttus_Config2__AttributeValueId__r.APTS_Description__c;
            if (lineItem.Apttus_Config2__ProductId__r.APTS_SPOO_Type__c == SYSTEM_TYPE_SERVICE)
            {
                description = lineItem.Apttus_Config2__AttributeValueId__r.APTS_Service_Plan_Name__c;
            }
            else
            {
                description = lineItem.Apttus_Config2__AttributeValueId__r.APTS_Product_Name__c;
            }

            //Product2 variables
            systemType = lineItem.Apttus_Config2__ProductId__r.APTS_SPOO_Type__c;
            spooCategory = lineItem.Apttus_Config2__ProductId__r.ProductCode;
            valuationClass = lineItem.Apttus_Config2__ProductId__r.APTS_Valuation_Class__c;

            if (systemType == SYSTEM_TYPE_3RD_PARTY || systemType == SYSTEM_TYPE_PHILIPS || systemType == SYSTEM_TYPE_DEMO)
            {
                itemSO.Apttus_Config2__ListPrice__c = costPrice * listPriceMultiplier;
            }
            else if (systemType == SYSTEM_TYPE_SERVICE)
            {
                itemSO.Apttus_Config2__ListPrice__c = listPrice;
            }
            else if (ListTradeSpoo.contains(systemType))
            {  //[US #519734]-Changed for SPOO Trade products
                //SR Updated to calculate List Price for trade-ins from List Price and not from Fair Market Value. DE61480
                itemSO.Apttus_Config2__ListPrice__c = listPrice * -1;
            }
            //F23266 - Added by Bogdan
            Decimal sellingTerm = lineItem.Apttus_Config2__SellingTerm__c != null && lineItem.Apttus_Config2__SellingTerm__c != 0 ? lineItem.Apttus_Config2__SellingTerm__c : 1;
            lineItem.APTS_Extended_List_Price__c = itemSO.Apttus_Config2__ListPrice__c != null ? formatPrecisionCeiling(itemSO.Apttus_Config2__ListPrice__c) * lineItem.APTS_Extended_Quantity__c * sellingTerm : 0;
            //SR: For Option Unit Price: Option List Price * Bundle Quantity
            lineItem.APTS_Option_Unit_Price__c = itemSO.Apttus_Config2__ListPrice__c != null ? (formatPrecisionCeiling(itemSO.Apttus_Config2__ListPrice__c) * lineItem.APTS_Extended_Quantity__c) / lineItem.Apttus_Config2__Quantity__c * sellingTerm : 0;

            //DE59203
            if (ListTradeSpoo.contains(systemType))
            { //[US #519734]-Changed for SPOO Trade products
                //DE59104 - Requirement is to display negative values in alignment with negative value of Trade-In
                lineItem.APTS_Target_Price_SPOO__c = fairMarketValue * targetPriceMultiplier * -1;
                //DE60483
                lineItem.Apttus_Config2__MinPrice__c = fairMarketValue * minPriceMultiplier * -1;
                lineItem.APTS_Escalation_Price_SPOO__c = fairMarketValue * escPriceMultiplier * -1;
                //lineItem.APTS_Fair_Market_Value__c = fairMarketValue ;                      Removed by Mario Yordanov for US-368604 on 11.9.2019
            }
            else
            {
                lineItem.APTS_Target_Price_SPOO__c = costPrice * targetPriceMultiplier;
                lineItem.Apttus_Config2__MinPrice__c = costPrice * minPriceMultiplier;//DE59103 - Updated field, previous: APTS_Minimum_Price_SPOO__c
                lineItem.APTS_Escalation_Price_SPOO__c = costPrice * escPriceMultiplier;
            }

            if (spooCategory != null)
                lineItem.APTS_Product_ID_SPOO__c = spooCategory;

            if (systemType != null)
                lineItem.APTS_System_Type__c = systemType;

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
    //FF22944 End

    //Added by Yoncho Yonchev for DE59198 on 19 August 2018
    public void computeNetAdjustment(List<Apttus_Config2.LineItem> lineItems)
    {
        //F254432 - Added by Bogdan Botcharov
        Map<String, Decimal> mapBundleAdjustments = new Map<String, Decimal>();
        //Mithilesh Maurya 15 Feb 2020, compute solution List Price (fix)
        Map<String, pricePointsWrapper> mapPricePoints = new Map<String, pricePointsWrapper>();
        for (Apttus_Config2.LineItem lineItemMO : lineItems)
        {
            Apttus_Config2__LineItem__c liSO = lineItemMO.getLineItemSO();
            String uniqIdentifier = String.valueOf(liSO.Apttus_Config2__LineNumber__c) + liSO.Apttus_Config2__ChargeType__c;
            if (liSO.Apttus_Config2__OptionId__c == null)
            {
                if (!mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__PrimaryLineNumber__c))
                    mapPricePoints.put(uniqIdentifier, new pricePointsWrapper(liSO));
            }
            else if (liSO.Apttus_Config2__OptionId__c != null && liSO.Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c == 'Bundle')
            {
                if (!mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__PrimaryLineNumber__c))
                    mapPricePoints.put(uniqIdentifier + liSO.Apttus_Config2__PrimaryLineNumber__c, new pricePointsWrapper(liSO));
            }
            else
            {
                if (liSO.APTS_Extended_List_Price__c != null && !liSO.Apttus_Config2__IsOptional__c)
                {
                    if (mapPricePoints.containsKey(uniqIdentifier))
                        mapPricePoints.get(uniqIdentifier).listPrice += liSO.APTS_Extended_List_Price__c;
                    if (mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c))
                        mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).listPrice += liSO.APTS_Extended_List_Price__c;
                }
            }
        }
        for (Apttus_Config2.LineItem lineItemMO : lineItems)
        {
            Apttus_Config2__LineItem__c lineItemSO = lineItemMO.getLineItemSO();
            String uniqIdentifier = String.valueOf(lineItemSO.Apttus_Config2__LineNumber__c) + lineItemSO.Apttus_Config2__ChargeType__c;
            if (lineItemSO.Apttus_Config2__OptionId__c == null && lineItemSO.Apttus_Config2__AdjustmentType__c != null &&
                    lineItemSO.Apttus_Config2__AdjustmentAmount__c != null && lineItemSO.Apttus_Config2__AdjustmentType__c == 'Discount Amount')
            {
                Decimal netAdjPercentage = 0;
                Decimal extendedListPrice = lineItemSO.APTS_Extended_List_Price__c != null && lineItemSO.Apttus_Config2__IsOptional__c ? 0 : lineItemSO.APTS_Extended_List_Price__c;
                //Mithilesh - calculated Sol List Price
                lineItemSO.APTS_Solution_list_Price_c__c = mapPricePoints.containsKey(uniqIdentifier + lineItemSO.Apttus_Config2__PrimaryLineNumber__c) ? mapPricePoints.get(uniqIdentifier + lineItemSO.Apttus_Config2__PrimaryLineNumber__c).listPrice + extendedListPrice : mapPricePoints.get(uniqIdentifier).listPrice + extendedListPrice;
                if (lineItemSO.APTS_Solution_List_Price__c != null)
                {
                    netAdjPercentage = -1 * formatPrecisionCeiling(lineItemSO.Apttus_Config2__AdjustmentAmount__c) / formatPrecisionCeiling(lineItemSO.APTS_Solution_List_Price_c__c) * 100;
                }
                mapBundleAdjustments.put(lineItemSO.Apttus_Config2__LineNumber__c + lineItemSO.Apttus_Config2__ChargeType__c, netAdjPercentage);
            }
        }

        for (Apttus_Config2.LineItem lineItemMO : lineItems)
        {
            Apttus_Config2__LineItem__c lineItemSO = lineItemMO.getLineItemSO();
            if (lineItemSO.Apttus_Config2__AdjustmentType__c != null &&
                    lineItemSO.Apttus_Config2__AdjustmentAmount__c != null)
            { 
                if (lineItemSO.Apttus_Config2__AdjustmentType__c == 'Discount %')
                {
                    lineItemSO.Apttus_Config2__NetAdjustmentPercent__c = -1 * formatPrecisionCeiling(lineItemSO.Apttus_Config2__AdjustmentAmount__c);

                }
                else if (lineItemSO.Apttus_Config2__AdjustmentType__c == 'Discount Amount')
                {
                    if (!mapBundleAdjustments.isEmpty() && mapBundleAdjustments.containsKey(lineItemSO.Apttus_Config2__LineNumber__c + lineItemSO.Apttus_Config2__ChargeType__c)
                       && lineItemSO.Apttus_Config2__AllocateGroupAdjustment__c == true)
                    {
                        lineItemSO.Apttus_Config2__NetAdjustmentPercent__c = mapBundleAdjustments.get(lineItemSO.Apttus_Config2__LineNumber__c + lineItemSO.Apttus_Config2__ChargeType__c);
                        if (lineItemSO.Apttus_Config2__OptionId__c != null)
                            lineItemSO.Apttus_Config2__AdjustmentAmount__c = 0;
                    }
                    else if (lineItemSO.APTS_Philips_List_Price__c != null && lineItemSO.APTS_Philips_List_Price__c != 0)
                    {
                        lineItemSO.Apttus_Config2__NetAdjustmentPercent__c = -1 * formatPrecisionCeiling(lineItemSO.Apttus_Config2__AdjustmentAmount__c) / formatPrecisionCeiling(lineItemSO.APTS_Philips_List_Price__c) * 100;
                    }
                }
                //F254432 - Added by Bogdan Botcharov
            }
            else if (!mapBundleAdjustments.isEmpty() && mapBundleAdjustments.containsKey(lineItemSO.Apttus_Config2__LineNumber__c + lineItemSO.Apttus_Config2__ChargeType__c)
                     && lineItemSO.APTS_Philips_List_Price__c != null && lineItemSO.APTS_Philips_List_Price__c > 0
                     && lineItemSO.Apttus_Config2__AllocateGroupAdjustment__c == true)
            {
                lineItemSO.Apttus_Config2__NetAdjustmentPercent__c = mapBundleAdjustments.get(lineItemSO.Apttus_Config2__LineNumber__c + lineItemSO.Apttus_Config2__ChargeType__c);
            }
            //End F254432
            lineItemSO.APTS_Net_Adjustment_Percent_c__c = lineItemSO.Apttus_Config2__NetAdjustmentPercent__c;
        }
    }

    /*
    @Author: Bogdan Botcharov
    @Description: Fix for DE55958. Calculating Price Points for Bundle, sum of all Options based  on Charge Types
    @Date: July 2, 2018
    */
    public void calculatePricePointsForBundle(List<Apttus_Config2.LineItem> allLines)
    {
        Map<String, pricePointsWrapper> mapPricePoints = new Map<String, pricePointsWrapper>();
        String bundleAdjustmentType = '';
        String uniqIdentifier_li = '';
        Map<String, String> mapBundleAdjustment = new Map<String, String>();

        for (Apttus_Config2.LineItem lineItemMO : allLines)
        {
            Apttus_Config2__LineItem__c liSO = lineItemMO.getLineItemSO();
            String uniqIdentifier = String.valueOf(liSO.Apttus_Config2__LineNumber__c) + liSO.Apttus_Config2__ChargeType__c;

            //GBS:US-520662 To set the Unique identifier to populate map for bundle adjustment type       
            if (liSO.Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c == 'Solution Subscription' && (liSO.Apttus_Config2__LineType__c == 'Product/Service' || liSO.Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c == 'Bundle'))
                uniqIdentifier_li = String.valueOf(liSO.Apttus_Config2__LineNumber__c) + liSO.Apttus_Config2__PrimaryLineNumber__c + liSO.Apttus_Config2__ChargeType__c;
            else if (liSO.Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c == 'Solution Subscription' &&
                liSO.Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c == 'Option')
                uniqIdentifier_li = String.valueOf(liSO.Apttus_Config2__LineNumber__c) + liSO.Apttus_Config2__ParentBundleNumber__c + liSO.Apttus_Config2__ChargeType__c;
            else
                uniqIdentifier_li = String.valueOf(liSO.Apttus_Config2__LineNumber__c) + liSO.Apttus_Config2__ChargeType__c;

            //GP:  Checking and adding different key value, why ?
            if (liSO.Apttus_Config2__OptionId__c == null)
            {
                if (!mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__PrimaryLineNumber__c))
                    mapPricePoints.put(uniqIdentifier, new pricePointsWrapper(liSO));
                mapBundleAdjustment.put(uniqIdentifier_li, liSO.Apttus_Config2__AdjustmentType__c);
            }
            else if (liSO.Apttus_Config2__OptionId__c != null && liSO.Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c == 'Bundle' && liSO.Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c == 'Solution Subscription')
            {
                if (!mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__PrimaryLineNumber__c))
                    mapPricePoints.put(uniqIdentifier + liSO.Apttus_Config2__PrimaryLineNumber__c, new pricePointsWrapper(liSO));
                mapBundleAdjustment.put(uniqIdentifier_li, liSO.Apttus_Config2__AdjustmentType__c);
            }
            else
            {
                if (mapPricePoints.containsKey(uniqIdentifier/* + liSO.Apttus_Config2__ParentBundleNumber__c*/))
                {
                    Decimal targetPrice = 0;
                    Decimal minPrice = 0;
                    Decimal optEscPrice = 0;
                    Decimal extQty2 = liSO.APTS_Extended_Quantity__c;
                    Decimal optionOfferedPrice = 0;
                    Decimal sellingTerm = liSO.Apttus_Config2__SellingTerm__c != null && liSO.Apttus_Config2__SellingTerm__c != 0 ? liSO.Apttus_Config2__SellingTerm__c : 1;
                    Decimal extQty = liSO.APTS_Extended_Quantity__c != null && liSO.APTS_Extended_Quantity__c != 0 ? liSO.APTS_Extended_Quantity__c : 1;
                    Decimal lineQ = liSO.Apttus_Config2__Quantity__c != null && liSO.Apttus_Config2__Quantity__c != 0 ? liSO.Apttus_Config2__Quantity__c : 1;

                    bundleAdjustmentType = mapBundleAdjustment.get(uniqIdentifier_li) != null ? mapBundleAdjustment.get(uniqIdentifier_li) : '';//GBS:US-520662          
                    //DE57083 - Optional Lines must not be included in Total
                    //DE59190 - Added by Bogdan Botcharov - now referencing Bundle quantity from mapPricePoints.get(uniqIdentifier).qty
                    Decimal optionQty = liSO.Apttus_Config2__Quantity__c != null ? liSO.Apttus_Config2__Quantity__c : 1;
                    //Accuring the Extended List Price to set it to the
                    if (liSO.APTS_Extended_List_Price__c != null && !liSO.Apttus_Config2__IsOptional__c)
                    {
                        mapPricePoints.get(uniqIdentifier/* + liSO.Apttus_Config2__ParentBundleNumber__c*/).listPrice += liSO.APTS_Extended_List_Price__c;
                        if (mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c))
                            mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).listPrice += liSO.APTS_Extended_List_Price__c;
                    }
                    Decimal listPrice = liSO.APTS_Extended_List_Price__c != null ? liSO.APTS_Extended_List_Price__c : 0;
                    Decimal contractDiscount = liSO.APTS_ContractDiscount__c != null ? liSO.APTS_ContractDiscount__c : 0;
                    Decimal contractAmt = 0;
                    if (contractDiscount != null && listPrice != null)
                    {
                        contractAmt = listPrice * contractDiscount / 100;
                    }

                    //GP: Are we supporting 'Price Override' adjustment type ? 
                    liSO.APTS_Contract_Discount_Amount__c = contractDiscount != null ? contractAmt : null;
                    Decimal netAdjPercent = liSO.Apttus_Config2__NetAdjustmentPercent__c;

                    Decimal stdAdj = getStrategicDiscount(netAdjPercent, liSO.Apttus_Config2__AdjustmentType__c, listPrice, contractAmt);
                    Decimal incAdjAmt = liSO.Apttus_Config2__IncentiveAdjustmentAmount__c != null ? formatPrecisionCeiling(liSO.Apttus_Config2__IncentiveAdjustmentAmount__c) : 0;

                    Decimal unitStrategicDiscountAmount = (listPrice / extQty / sellingTerm) * stdAdj / 100;
                    Decimal strategicDiscountAmount = listPrice * stdAdj / 100;
                    String productType = liSO.Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c;
                    if (productType != null && productType.contains('Solution'))
                    {
                        if (liSO.APTS_Billing_Plan__c != null && liSO.APTS_Billing_Plan__c == 'Annual')
                            liSO.APTS_Unit_Strategic_Discount_Amount__c = (unitStrategicDiscountAmount * 12).setScale(2);
                        else
                            liSO.APTS_Unit_Strategic_Discount_Amount__c = (unitStrategicDiscountAmount).setScale(2);
                    }
                    else if (liSO.Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c != null && liSO.Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c == 'Service')
                    {
                        liSO.APTS_Unit_Strategic_Discount_Amount__c = unitStrategicDiscountAmount;
                    }
                    else
                    {
                        liSO.APTS_Unit_Strategic_Discount_Amount__c = unitStrategicDiscountAmount.setScale(2);
                    }

                    if (liSO.Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c != null && liSO.Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c == 'Service')
                    {
                        liSO.APTS_Strategic_Discount_Amount_c__c = strategicDiscountAmount;
                    }
                    else
                    {
                        liSO.APTS_Strategic_Discount_Amount_c__c = unitStrategicDiscountAmount.setScale(2) * sellingTerm * extQty;
                    }
                    
                    //462170 - Added by Bogdan Botcharov [11 November 2019]
                    if (productType != null && productType.contains('Solution'))
                    {
                        if (mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c))
                        {
                            if (liSO.APTS_SAP_List_Price__c != null)
                            {
                                mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).sapListPrice += liSO.APTS_SAP_List_Price__c;
                            }
                            if (liSO.APTS_Unit_Strategic_Discount_Amount__c != null)
                            {
                                mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).unitStrategicDiscountAmount += liSO.APTS_Unit_Strategic_Discount_Amount__c;
                            }
                        }
                    }

                    if (liSO.APTS_Target_Price__c != null)
                    {
                        targetPrice = formatPrecisionCeiling(liSO.APTS_Target_Price__c);
                        if (liSO.Apttus_Config2__IsOptional__c != true && targetPrice != null)
                        {
                            mapPricePoints.get(uniqIdentifier/* + liSO.Apttus_Config2__ParentBundleNumber__c*/).targetPrice += targetPrice;
                            if (mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c))
                                mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).targetPrice += targetPrice;
                        }
                    }

                    if (liSO.APTS_Minimum_Price__c != null)
                    {
                        minPrice = formatPrecisionCeiling(liSO.APTS_Minimum_Price__c);
                        if (liSO.Apttus_Config2__IsOptional__c != true)
                        {
                            mapPricePoints.get(uniqIdentifier/* + liSO.Apttus_Config2__ParentBundleNumber__c*/).minPrice += minPrice;
                            if (mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c))
                                mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).minPrice += minPrice;
                        }
                    }

                    if (liSO.APTS_Escalation_Price__c != null)
                    {
                        optEscPrice = formatPrecisionCeiling(liSO.APTS_Escalation_Price__c);
                        if (liSO.Apttus_Config2__IsOptional__c != true)
                        {
                            mapPricePoints.get(uniqIdentifier/* + liSO.Apttus_Config2__ParentBundleNumber__c*/).preEscalationPrice += optEscPrice;
                            if (mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c))
                                mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).preEscalationPrice += optEscPrice;
                        }
                    }

                    if (liSO.Apttus_Config2__ListPrice__c != null && (liSO.APTS_Local_Bundle_Component_Flag__c == null || liSO.APTS_Local_Bundle_Component_Flag__c == false))
                    {
                        Decimal qty = liSO.Apttus_Config2__Quantity__c != null ? liSO.Apttus_Config2__Quantity__c : 1;
                        // Modified by Medea Gugushvili for DE57652 on [26.07.2018]
                        if (liSO.Apttus_Config2__IsOptional__c != true)
                        {
                            mapPricePoints.get(uniqIdentifier/* + liSO.Apttus_Config2__ParentBundleNumber__c*/).optionPrice += (liSO.Apttus_Config2__PriceListItemId__r.APTS_Country_Pricelist_List_Price__c == null) ? formatPrecisionCeiling(liSO.Apttus_Config2__ListPrice__c) * qty : liSO.Apttus_Config2__PriceListItemId__r.APTS_Country_Pricelist_List_Price__c * qty;
                            if (mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c))
                                mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).optionPrice += (liSO.Apttus_Config2__PriceListItemId__r.APTS_Country_Pricelist_List_Price__c == null) ? formatPrecisionCeiling(liSO.Apttus_Config2__ListPrice__c) * qty : liSO.Apttus_Config2__PriceListItemId__r.APTS_Country_Pricelist_List_Price__c * qty;
                        }
                    }

                    if (liSO.APTS_Contract_Net_Price__c != null && !liSO.Apttus_Config2__IsOptional__c)
                    {
                        mapPricePoints.get(uniqIdentifier/* + liSO.Apttus_Config2__ParentBundleNumber__c*/).contractNetPrice += formatPrecisionCeiling(liSO.APTS_Contract_Net_Price__c);
                        if (mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c))
                            mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).contractNetPrice += formatPrecisionCeiling(liSO.APTS_Contract_Net_Price__c);
                    }

                    if (liSO.Apttus_Config2__ExtendedPrice__c != null && !liSO.Apttus_Config2__IsOptional__c)
                    {
                        mapPricePoints.get(uniqIdentifier/* + liSO.Apttus_Config2__ParentBundleNumber__c*/).bundleExtendedPrice += mapPricePoints.get(uniqIdentifier/* + liSO.Apttus_Config2__ParentBundleNumber__c*/).qty != null ? formatPrecisionCeiling(liSO.Apttus_Config2__ExtendedPrice__c) * mapPricePoints.get(uniqIdentifier/* + liSO.Apttus_Config2__ParentBundleNumber__c*/).qty : formatPrecisionCeiling(liSO.Apttus_Config2__ExtendedPrice__c);
                        if (mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c))
                            mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).bundleExtendedPrice += mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).qty != null ? formatPrecisionCeiling(liSO.Apttus_Config2__ExtendedPrice__c) * mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).qty : formatPrecisionCeiling(liSO.Apttus_Config2__ExtendedPrice__c);
                    }

                    if (liSO.APTS_ContractDiscount__c != null && liSO.Apttus_Config2__PriceListItemId__r.Apttus_Config2__ListPrice__c != null)
                    {
                        pricePointsWrapper points = mapPricePoints.get(uniqIdentifier/* + liSO.Apttus_Config2__ParentBundleNumber__c*/);
                        if (points.solutionContractDiscountAmount == null)
                        {
                            points.solutionContractDiscountAmount = 0;
                        }
                        if (!liSO.Apttus_Config2__IsOptional__c)
                        {
                            mapPricePoints.get(uniqIdentifier/* + liSO.Apttus_Config2__ParentBundleNumber__c*/).solutionContractDiscountAmount += contractAmt;
                            if (mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c))
                                mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).solutionContractDiscountAmount += contractAmt;
                        }
                    }

                    if (liSO.Apttus_Config2__IncentiveAdjustmentAmount__c != null && liSO.Apttus_Config2__IncentiveAdjustmentAmount__c < 0 && !liSO.Apttus_Config2__IsOptional__c)
                    {
                        mapPricePoints.get(uniqIdentifier).incentiveAdjAmount += (formatPrecisionCeiling(liSO.Apttus_Config2__IncentiveAdjustmentAmount__c) * -1);

                        if (liSO.APTS_Unit_Incentive_Adj_Amount__c != null)
                            mapPricePoints.get(uniqIdentifier).solutionUnitIncentiveAmount += liSO.APTS_Unit_Incentive_Adj_Amount__c * -1;

                        if (mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c))
                        {
                            mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).incentiveAdjAmount += (formatPrecisionCeiling(liSO.Apttus_Config2__IncentiveAdjustmentAmount__c) * -1);

                            if (liSO.APTS_Unit_Incentive_Adj_Amount__c != null)
                                mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).solutionUnitIncentiveAmount += liSO.APTS_Unit_Incentive_Adj_Amount__c * -1;
                        }
                    }
                    else if (liSO.Apttus_Config2__IncentiveAdjustmentAmount__c != null && liSO.Apttus_Config2__IncentiveAdjustmentAmount__c > 0 && !liSO.Apttus_Config2__IsOptional__c)
                    {
                        mapPricePoints.get(uniqIdentifier).incentiveAdjAmount += (formatPrecisionCeiling(liSO.Apttus_Config2__IncentiveAdjustmentAmount__c));

                        if (liSO.APTS_Unit_Incentive_Adj_Amount__c != null)
                            mapPricePoints.get(uniqIdentifier).solutionUnitIncentiveAmount += liSO.APTS_Unit_Incentive_Adj_Amount__c;

                        if (mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c))
                        {
                            mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).incentiveAdjAmount += (formatPrecisionCeiling(liSO.Apttus_Config2__IncentiveAdjustmentAmount__c));

                            if (liSO.APTS_Unit_Incentive_Adj_Amount__c != null)
                                mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).solutionUnitIncentiveAmount += liSO.APTS_Unit_Incentive_Adj_Amount__c;
                        }
                    }

                    if (liSO.APTS_Offered_Price_c__c != null)
                    {
                        optionOfferedPrice = liSO.APTS_Extended_List_Price__c != null ? liSO.APTS_Extended_List_Price__c : 0;
                        Decimal totalDiscounts = 0;
                        if (contractAmt != null)
                        {
                            totalDiscounts = totalDiscounts - contractAmt;
                        }
                        if (liSO.Apttus_Config2__IncentiveAdjustmentAmount__c != null)
                        {
                            Decimal incAdjAmt1 = formatPrecisionCeiling(liSO.Apttus_Config2__IncentiveAdjustmentAmount__c);
                            totalDiscounts = totalDiscounts + incAdjAmt1;
                        }
                        if (liSO.Apttus_Config2__NetAdjustmentPercent__c != null)
                        {
                            totalDiscounts = totalDiscounts - liSO.APTS_Strategic_Discount_Amount_c__c;
                        }
                        optionOfferedPrice = optionOfferedPrice + totalDiscounts;

                        if (!liSO.Apttus_Config2__IsOptional__c && optionOfferedPrice != null)
                        {
                            mapPricePoints.get(uniqIdentifier/* + liSO.Apttus_Config2__ParentBundleNumber__c*/).offeredPrice += optionOfferedPrice;
                            if (mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c))
                                mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).offeredPrice += optionOfferedPrice;
                        }
                    }

                    if (optEscPrice != null && optEscPrice != 0)
                        liSO.APTS_Escalation_Price_Attainment_c__c = (optionOfferedPrice / formatPrecisionCeiling(liSO.APTS_Escalation_Price__c)) * 100;
                    if (targetPrice != null && targetPrice != 0)
                        liSO.APTS_Target_Price_Attainment__c = (optionOfferedPrice / (formatPrecisionCeiling(liSO.APTS_Target_Price__c))) * 100;
                    if (minPrice != null && minPrice != 0)
                        liSO.APTS_Minimum_Price_Attainment_c__c = (optionOfferedPrice / (formatPrecisionCeiling(liSO.APTS_Minimum_Price__c))) * 100;

                    //DE60436 - Added by Bogdan Botcharov
                    liSO.APTS_Price_Attainment_Color__c = returnColor(optionOfferedPrice, optEscPrice, minPrice, liSO.APTS_Extended_List_Price__c); //US#368606 Removed by Mario Yordanov on 15.10.2019
                }
            }
        }

        for (Apttus_Config2.LineItem lineItemMO : allLines)
        {
            Apttus_Config2__LineItem__c liSO = lineItemMO.getLineItemSO();
            Decimal sellingTerm = liSO.Apttus_Config2__SellingTerm__c != null && liSO.Apttus_Config2__SellingTerm__c != 0 ? liSO.Apttus_Config2__SellingTerm__c : 1;
            Decimal extQty = liSO.APTS_Extended_Quantity__c != null ? liSO.APTS_Extended_Quantity__c : 1;

            String uniqIdentifier = String.valueOf(liSO.Apttus_Config2__LineNumber__c) + liSO.Apttus_Config2__ChargeType__c/* + liSO.Apttus_Config2__PrimaryLineNumber__c*/;

            //GBS:US-520662        
            if (liSO.Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c == 'Solution Subscription' && (liSO.Apttus_Config2__LineType__c == 'Product/Service' || liSO.Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c == 'Bundle'))
                uniqIdentifier_li = String.valueOf(liSO.Apttus_Config2__LineNumber__c) + liSO.Apttus_Config2__PrimaryLineNumber__c + liSO.Apttus_Config2__ChargeType__c;
            else if (liSO.Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c == 'Solution Subscription' && liSO.Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c == 'Option')
                uniqIdentifier_li = String.valueOf(liSO.Apttus_Config2__LineNumber__c) + liSO.Apttus_Config2__ParentBundleNumber__c + liSO.Apttus_Config2__ChargeType__c;
            else
                uniqIdentifier_li = String.valueOf(liSO.Apttus_Config2__LineNumber__c) + liSO.Apttus_Config2__ChargeType__c;

            bundleAdjustmentType = mapBundleAdjustment.get(uniqIdentifier_li) != null ? mapBundleAdjustment.get(uniqIdentifier_li) : '';

            if (liSO.Apttus_Config2__OptionId__c != null && liSO.Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c == 'Bundle')
                uniqIdentifier += String.valueOf(liSO.Apttus_Config2__PrimaryLineNumber__c);
            
            if (mapPricePoints.containsKey(uniqIdentifier))
            {
                if (liSO.Apttus_Config2__OptionId__c == null || liSO.Apttus_Config2__OptionId__c != null && liSO.Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c == 'Bundle')
                {
                    Decimal extendedListPrice = liSO.APTS_Extended_List_Price__c != null && liSO.Apttus_Config2__IsOptional__c ? 0 : liSO.APTS_Extended_List_Price__c;
                    if (liSO.APTS_Extended_List_Price__c != null)
                    {
                        liSO.APTS_Solution_list_Price_c__c = mapPricePoints.get(uniqIdentifier).listPrice + extendedListPrice;
                    }

                    Decimal listPrice = liSO.APTS_Extended_List_Price__c != null ? liSO.APTS_Extended_List_Price__c : 0;
                    Decimal bundleOfferedPrice = liSO.APTS_Extended_List_Price__c != null ? liSO.APTS_Extended_List_Price__c : 0;
                    Decimal totalDiscounts = 0;
                    Decimal contractAmt = 0;
                    Decimal unitContractAmt = 0;
                    //525556 - Modified by Bogdan Botcharov
                    if (liSO.APTS_ContractDiscount__c != null)
                    {
                        Decimal contractDiscount = liSO.APTS_ContractDiscount__c != null ? liSO.APTS_ContractDiscount__c : 0;
                        unitContractAmt = ((bundleOfferedPrice / extQty / sellingTerm) * contractDiscount / 100).setScale(2);
                        contractAmt = unitContractAmt * extQty * sellingTerm;
                        totalDiscounts = totalDiscounts - contractAmt;
                    }
                    if (liSO.Apttus_Config2__IncentiveAdjustmentAmount__c != null)
                    {
                        totalDiscounts = totalDiscounts + formatPrecisionCeiling(liSO.Apttus_Config2__IncentiveAdjustmentAmount__c);
                    }
                    if (liSO.Apttus_Config2__NetAdjustmentPercent__c != null)
                    {
                        Decimal stdAdj = getStrategicDiscount(liSO.Apttus_Config2__NetAdjustmentPercent__c, liSO.Apttus_Config2__AdjustmentType__c, listPrice, contractAmt);
                        Decimal incAdjAmt = liSO.Apttus_Config2__IncentiveAdjustmentAmount__c != null ? formatPrecisionCeiling(liSO.Apttus_Config2__IncentiveAdjustmentAmount__c) : 0;

                        //System.debug('pj 1352*****liSO.APTS_Strategic_Discount_Amount__c: '+liSO.APTS_Strategic_Discount_Amount__c);
                        //471758 - Added by Bogdan Botcharov on October 24 2019 - Calculate unit strategic discount to 2 decimal places and then multiply by Selling Term and Quantity
                        Decimal unitStrategicDiscountAmount = (listPrice / extQty / sellingTerm) * stdAdj / 100;
                        Decimal strategicDiscountAmount = listPrice * stdAdj / 100;
                        if (liSO.Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c == 'Service')
                        {
                            liSO.APTS_Unit_Strategic_Discount_Amount__c = unitStrategicDiscountAmount;
                            liSO.APTS_Strategic_Discount_Amount_c__c = strategicDiscountAmount;
                        }
                        else
                        {
                            liSO.APTS_Unit_Strategic_Discount_Amount__c = unitStrategicDiscountAmount.setScale(2);
                            liSO.APTS_Strategic_Discount_Amount_c__c = unitStrategicDiscountAmount.setScale(2) * sellingTerm * extQty;
                        }

                        totalDiscounts = totalDiscounts - liSO.APTS_Strategic_Discount_Amount_c__c;
                    }
                    bundleOfferedPrice = bundleOfferedPrice + totalDiscounts;

                    //462170 - Added by Bogdan Botcharov [11 November 2019]
                    String productType = liSO.Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c;
                    if (productType != null && productType.contains('Solution') && liSO.Apttus_Config2__OptionId__c != null && liSO.Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c == 'Bundle')
                    {
                        if (liSO.APTS_Billing_Plan__c != null &&
                            (liSO.APTS_Billing_Plan__c == 'Monthly' || liSO.APTS_Billing_Plan__c == 'Quarterly' || liSO.APTS_Billing_Plan__c == 'Annual'))
                        {
                            liSO.APTS_SAP_List_Price__c = mapPricePoints.get(uniqIdentifier).sapListPrice;
                        }
                        liSO.APTS_Unit_Strategic_Discount_Amount__c = mapPricePoints.get(uniqIdentifier).unitStrategicDiscountAmount;
                    }

                    Decimal optEscPrice = formatPrecisionCeiling(liSO.APTS_Escalation_Price__c);
                    if (optEscPrice != null && optEscPrice != 0)
                    {
                        liSO.APTS_Escalation_Price_Attainment_c__c = (!ListTradeSpoo.contains(liSO.Apttus_Config2__ProductId__r.APTS_SPOO_Type__c)) ? (bundleOfferedPrice / optEscPrice) * 100 : bundleOfferedPrice != 0 ? (optEscPrice / bundleOfferedPrice) * 100 : 0;  //[US #519734]-Changed for SPOO Trade products
                    }
                    Decimal minPrice = formatPrecisionCeiling(liSO.APTS_Minimum_Price__c);
                    if (minPrice != null && minPrice != 0)
                    {
                        liSO.APTS_Minimum_Price_Attainment_c__c = (!ListTradeSpoo.contains(liSO.Apttus_Config2__ProductId__r.APTS_SPOO_Type__c)) ? (bundleOfferedPrice / minPrice) * 100 : bundleOfferedPrice != 0 ? (minPrice / bundleOfferedPrice) * 100 : 0;  //[US #519734]-Changed for SPOO Trade products
                    }
                    Decimal targetPrice = formatPrecisionCeiling(liSO.APTS_Target_Price__c);
                    if (targetPrice != null && targetPrice != 0)
                    {
                        liSO.APTS_Target_Price_Attainment__c = (!ListTradeSpoo.contains(liSO.Apttus_Config2__ProductId__r.APTS_SPOO_Type__c)) ? (bundleOfferedPrice / targetPrice) * 100 : bundleOfferedPrice != 0 ? (targetPrice / bundleOfferedPrice) * 100 : 0;  //[US #519734]-Changed for SPOO Trade products
                    }

                    //DE60436 - Added by Bogdan Botcharov
                    liSO.APTS_Price_Attainment_Color__c = returnColor(bundleOfferedPrice, optEscPrice, minPrice, liSO.APTS_Extended_List_Price__c); //US#368606 Removed by Mario Yordanov on 15.10.2019

                    Decimal solMinPrice = liSO.Apttus_Config2__IsOptional__c ? mapPricePoints.get(uniqIdentifier).minPrice : mapPricePoints.get(uniqIdentifier).minPrice + formatPrecisionCeiling(liSO.APTS_Minimum_Price__c);
                    liSO.APTS_Minimum_Price_Bundle__c = solMinPrice;
                    Decimal solEscPrice = liSO.Apttus_Config2__IsOptional__c ? mapPricePoints.get(uniqIdentifier).preEscalationPrice : mapPricePoints.get(uniqIdentifier).preEscalationPrice + optEscPrice;
                    liSO.APTS_Escalation_Price_Bundle__c = solEscPrice;
                    if (liSO.APTS_Country_Target_Price__c != null)
                    {
                        liSO.APTS_Target_Price_Bundle__c = liSO.Apttus_Config2__IsOptional__c ? mapPricePoints.get(uniqIdentifier).targetPrice : mapPricePoints.get(uniqIdentifier).targetPrice + formatPrecisionCeiling(liSO.APTS_Target_Price__c);
                    }
                    liSO.APTS_Option_List_Price__c = mapPricePoints.get(uniqIdentifier).optionPrice; //US368606 Removed by Mario Yordanov on 15.10.2019

                    liSO.APTS_Contract_Discount_Amount__c = mapPricePoints.get(uniqIdentifier).contractDiscountAmount;
                    liSO.APTS_Solution_Contract_Discount_Amount__c = mapPricePoints.get(uniqIdentifier).solutionContractDiscountAmount;
                    liSO.APTS_Incentive_Adjustment_Amount_Bundle__c = mapPricePoints.get(uniqIdentifier).incentiveAdjAmount;
                    liSO.APTS_Solution_Unit_Incentive_Adj_Amount__c = mapPricePoints.get(uniqIdentifier).solutionUnitIncentiveAmount;

                    Decimal solOfferedPrice = mapPricePoints.get(uniqIdentifier).offeredPrice;
                    liSO.APTS_Solution_Offered_Price_c__c = solOfferedPrice;
                    liSO.Apttus_Config2__NetPrice__c = solOfferedPrice + bundleOfferedPrice;

                    if (liSO.APTS_Solution_List_Price_c__c != null && liSO.APTS_Solution_Offered_Price__c != null)
                    { 
                        liSO.APTS_Total_Resulting_Discount_Amount__c = liSO.APTS_Solution_List_Price_c__c - (liSO.APTS_Solution_Offered_Price_c__c + bundleOfferedPrice);
                    }

                    liSO.APTS_Solution_Price_Attainment_Color__c = returnColor(solOfferedPrice + bundleOfferedPrice, solEscPrice, solMinPrice, liSO.APTS_Extended_List_Price__c);
                }
            }
        }
    }

    //Populate Tier for pricing Only for Contract Pricing
    public void populateTierOnPriceItemSet(Apttus_Config2.LineItem lineItemMO, Apttus_Config2__PriceListItem__c itemSO)
    {
        Apttus_Config2__LineItem__c liSO = lineItemMO.getLineItemSO();

        if (!mapPliSO.isEmpty())
        {
            if (mapPliSO.containsKey(itemSO.Id) && (mapMemberTiers.get(mapPliSO.get(itemSO.Id).APTS_Agreement_Group__c) != null))
            {
                if (mapMemberTiers.get(mapPliSO.get(itemSO.Id).APTS_Agreement_Group__c) != null)
                    strVolumeTier = mapMemberTiers.get(mapPliSO.get(itemSO.Id).APTS_Agreement_Group__c);

                liSO.APTS_VolumeTier__c = strVolumeTier;

                //To set Apttus_Config2__ListPrice__c on line item, we temporarily set value of corresponding Price List Item
                // Added because of removal of matrix generation in CLM
                itemSO.Apttus_Config2__ListPrice__c = TIER_1 == strVolumeTier ? mapPliSO.get(itemSO.Id).APTS_Tier_1_List_Price__c
                                                      : TIER_2 == strVolumeTier ? mapPliSO.get(itemSO.Id).APTS_Tier_2_List_Price__c
                                                      : TIER_3 == strVolumeTier ? mapPliSO.get(itemSO.Id).APTS_Tier_3_List_Price__c
                                                      : TIER_4 == strVolumeTier ? mapPliSO.get(itemSO.Id).APTS_Tier_4_List_Price__c
                                                      : null;

                liSO.APTS_ContractDiscount__c = TIER_1 == strVolumeTier ? mapPliSO.get(itemSO.Id).APTS_Tier_1_Discount__c
                                                : TIER_2 == strVolumeTier ? mapPliSO.get(itemSO.Id).APTS_Tier_2_Discount__c
                                                : TIER_3 == strVolumeTier ? mapPliSO.get(itemSO.Id).APTS_Tier_3_Discount__c
                                                : TIER_4 == strVolumeTier ? mapPliSO.get(itemSO.Id).APTS_Tier_4_Discount__c
                                                : null;
            }
        }
        else
        {
            clearContractRelatedField(liSO);
        }
    }

    //Added by Medea Gugushvili on 17-AUG-2018 for [DE58371]
    private void clearContractRelatedField(Apttus_Config2__LineItem__c liSO)
    {
        liSO.APTS_VolumeTier__c = null;
        liSO.APTS_ContractDiscount__c = null;
    }

    //GP: This logic will be moved to the Finish method.
    public void populateCustomFields(List<Apttus_Config2.LineItem> lineItems)
    {
        APTS_Country_Payment_Term__c countryPayment = APTS_Country_Payment_Term__c.getInstance(proposalSO.APTS_Country_code__c);

        for (Apttus_Config2.LineItem lineItemMO : lineItems)
        {
            Apttus_Config2__LineItem__c liSO = lineItemMO.getLineItemSO();
            Apttus_Config2__PriceListItem__c pli = liSO.Apttus_Config2__PriceListItemId__r;
            if (liSO.Apttus_Config2__PriceListItemId__r.APTS_Country_Pricelist_List_Price__c != null)
            {

                liSO.APTS_Payment_Term__c = pli.Apttus_Config2__PriceListId__r.APTS_Payment_Term_Credit_Terms__c;
                liSO.APTS_Inco_Term__c = pli.Apttus_Config2__PriceListId__r.APTS_Inco_Terms__c;

            }
            else
            {
                liSO.APTS_Payment_Term__c = proposalSO.Apttus_Proposal__Payment_Term__c;
                liSO.APTS_Inco_Term__c = proposalSO.APTS_Inco_Term__c;
            }

            //DE60369 - Added by Bogdan Botcharov
            if (liSO.Apttus_Config2__LineType__c == 'Option')
            {
                if (liSO.Apttus_Config2__OptionId__r.Main_Article_Group_ID__c != null)
                    liSO.APTS_MAG__c = liSO.Apttus_Config2__OptionId__r.Main_Article_Group_ID__c;
                if (liSO.Apttus_Config2__OptionId__r.Business_Unit_ID__c != null)
                    liSO.APTS_Business_Unit__c = liSO.Apttus_Config2__OptionId__r.Business_Unit_ID__c;
            }
            else
            {
                if (liSO.Apttus_Config2__ProductId__r.Main_Article_Group_ID__c != null)
                    liSO.APTS_MAG__c = liSO.Apttus_Config2__ProductId__r.Main_Article_Group_ID__c;
                if (liSO.Apttus_Config2__ProductId__r.Business_Unit_ID__c != null)
                    liSO.APTS_Business_Unit__c = liSO.Apttus_Config2__ProductId__r.Business_Unit_ID__c;
            }

            if (liSO.APTS_Escalation_Price_Attainment__c != NULL)
            {
                liSO.APTS_Is_Escalation_Price_Attained__c = !((liSO.APTS_Escalation_Price_Attainment__c < 100 && liSO.Apttus_Config2__NetPrice__c > 0) || (liSO.APTS_Escalation_Price_Attainment__c > 100 && liSO.Apttus_Config2__NetPrice__c < 0));
            }
            if (liSO.APTS_Minimum_Price_Attainment__c != NULL)
            {
                liSO.APTS_Is_Minimum_Price_Attained__c = !((liSO.APTS_Minimum_Price_Attainment__c < 100 && liSO.Apttus_Config2__NetPrice__c > 0) || (liSO.APTS_Minimum_Price_Attainment__c > 100 && liSO.Apttus_Config2__NetPrice__c < 0));
            }
        }
    }
    // End Added by Medea Gugushvili for DE57121

    public class pricePointsWrapper
    {
        public Decimal targetPrice;
        public Decimal minPrice;
        public Decimal listPrice;
        public Decimal preEscalationPrice;
        public Decimal optionPrice;
        public Decimal bundleExtendedPrice;
        public Decimal contractNetPrice;
        public Decimal offeredPrice;
        public Decimal qty;
        public Decimal bundleBaseExtendedPrice;
        public Decimal sellingTerm;
        public Decimal incentiveAdjAmount;
        public Decimal solutionUnitIncentiveAmount;
        public Decimal sapListPrice;
        public Decimal unitStrategicDiscountAmount = 0;
        public Decimal pliCountryPriceListPrice;
        public Decimal philipsListPrice;
        public Decimal contractDiscountAmount;
        public Decimal solutionContractDiscountAmount;

        public pricePointsWrapper(Apttus_Config2__LineItem__c liSO)
        {
            listPrice = 0;
            solutionUnitIncentiveAmount = 0;
            //Check Selling Term for Option
            if (liSO.Apttus_Config2__Frequency__c != null && liSO.Apttus_Config2__Frequency__c != 'One Time')
            {
                if (liSO.Apttus_Config2__StartDate__c != null && liSO.Apttus_Config2__EndDate__c != null)
                    liSO.Apttus_Config2__SellingTerm__c = Apttus_Config2.CPQWebservice.computeTerm(liSO.Apttus_Config2__StartDate__c, liSO.Apttus_Config2__EndDate__c, liSO.Apttus_Config2__Frequency__c);
                sellingTerm = liSO.Apttus_Config2__SellingTerm__c != null && liSO.Apttus_Config2__SellingTerm__c != 0 ? liSO.Apttus_Config2__SellingTerm__c : 1;
            }
            else
            {
                liSO.Apttus_Config2__SellingTerm__c = 1;
                sellingTerm = 1;
            }
            preEscalationPrice = 0;
            targetPrice = 0;
            minPrice = 0;
            if (liSO.Apttus_Config2__PriceListId__r.Apttus_Config2__ContractNumber__c != null)
            {
                contractDiscountAmount = liSO.APTS_ContractDiscount__c != null && liSO.Apttus_Config2__PriceListItemId__r.Apttus_Config2__ListPrice__c != null && liSO.Apttus_Config2__PriceListItemId__r.Apttus_Config2__ListPrice__c != 0 ? (liSO.APTS_ContractDiscount__c / 100) * liSO.Apttus_Config2__PriceListItemId__r.Apttus_Config2__ListPrice__c * liSO.Apttus_Config2__Quantity__c : 0;
                solutionContractDiscountAmount = !liSO.Apttus_Config2__IsOptional__c && liSO.APTS_ContractDiscount__c != null && liSO.Apttus_Config2__PriceListItemId__r.Apttus_Config2__ListPrice__c != null ? (liSO.APTS_ContractDiscount__c / 100) * liSO.Apttus_Config2__PriceListItemId__r.Apttus_Config2__ListPrice__c * liSO.Apttus_Config2__Quantity__c : 0;
            }

            pliCountryPriceListPrice = liSO.Apttus_Config2__PriceListItemId__r.APTS_Country_Pricelist_List_Price__c != null && liSO.Apttus_Config2__PriceListItemId__r.APTS_Country_Pricelist_List_Price__c > 0 ? liSO.Apttus_Config2__PriceListItemId__r.APTS_Country_Pricelist_List_Price__c : 0;
            philipsListPrice = liSO.APTS_Item_List_Price__c != null ? liSO.APTS_Item_List_Price__c.setscale(2, System.RoundingMode.CEILING) : 0;
            sapListPrice = philipsListPrice;
            qty = liSO.Apttus_Config2__Quantity__c != null ? liSO.Apttus_Config2__Quantity__c : 1;
            if (philipsListPrice != null && (pliCountryPriceListPrice != null && pliCountryPriceListPrice > 0))
                contractNetPrice = liSO.Apttus_Config2__ListPrice__c != null ? qty * liSO.Apttus_Config2__ListPrice__c.setscale(2, System.RoundingMode.CEILING) : 0;
            else
                contractNetPrice = 0;

            optionPrice = 0;
            offeredPrice = 0;
            bundleBaseExtendedPrice = liSO.Apttus_Config2__BasePrice__c != null ? liSO.Apttus_Config2__BasePrice__c.setscale(2, System.RoundingMode.CEILING) * qty * sellingTerm : 0;

            bundleExtendedPrice = bundleBaseExtendedPrice;

            if (solutionContractDiscountAmount == null) solutionContractDiscountAmount = 0;
            incentiveAdjAmount = liSO.Apttus_Config2__IncentiveAdjustmentAmount__c != null && !liSO.Apttus_Config2__IsOptional__c ? liSO.Apttus_Config2__IncentiveAdjustmentAmount__c.setscale(2, System.RoundingMode.CEILING) * -1 : 0;
            if (incentiveAdjAmount < 0)
            {
                incentiveAdjAmount = incentiveAdjAmount * -1;
                if (liSO.APTS_Unit_Incentive_Adj_Amount__c != null && liSO.APTS_Unit_Incentive_Adj_Amount__c > 0)
                    solutionUnitIncentiveAmount = liSO.APTS_Unit_Incentive_Adj_Amount__c * -1;
            }
        }
    }

    public String returnColor(Decimal offeredPrice, Decimal escPrice, Decimal minPrice, Decimal listPrice)
    {
        String color = null;
        if (offeredPrice != null && escPrice != null && minPrice != null)
        {
            if (offeredPrice != null && offeredPrice == 0 && (listPrice == null || listPrice == 0))
            {
                //Price = 0
                color = null;
            }
            else if (offeredPrice >= escPrice)
            {
                color = 'White';
            }
            else if (offeredPrice < escPrice && offeredPrice > minPrice)
            {
                //(Yellow)
                color = '#FFFF00';
            }
            else if (offeredPrice <= minPrice)
            {
                //(RED)
                color = '#FF0000';
            }
        }
        return color;
    }

    public Decimal getStrategicDiscount(Decimal netAdj, String adjType, Decimal listPrice, Decimal contractAmt)
    {
        Decimal stdAdj = netAdj * -1;
        IF(listPrice < 0 && adjType == 'Discount %') {
            stdAdj = netAdj;
        }
        if (listPrice < 0 && adjType == 'Discount Amount')
        {
            stdAdj = netAdj;
        }

        return stdAdj;
    }

    // NZahariev & KKunev 14.May.2019 format LI currency & number field values
    public Decimal formatPrecisionCeiling(Decimal fieldValue)
    {
        if (fieldValue != null)
        {
            return fieldValue.setscale(2, System.RoundingMode.CEILING);
        }
        else
        {
            return fieldValue;
        }
    }
}
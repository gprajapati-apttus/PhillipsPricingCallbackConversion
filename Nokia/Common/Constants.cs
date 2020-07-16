using System.Collections.Generic;

namespace Apttus.Lightsaber.Nokia.Common
{
    class Constants
    {
        public const string PROPOSAL = "Apttus_Proposal__Proposal__c";

        public const string NOKIA_PRODUCT_NAME_SSP = "SSP";
        public const string NOKIA_PARTNER_PROFILE_NAME = "PRM Partner Community User Login";
        public const string NOKIA_EMPTY = "";
        public const string NOKIA_PARTNER_RELATIONSHIP_MANAGER = "PRM Partner Relationship Manager Login";
        public const string NOKIA_PRODUCT_NAME_SRS = "SRS";
        public const string NOKIA_ACCRED_TYPE_PRODUCT = "Product";
        public const string NOKIA_ACCRED_TYPE_MAINTENANCE = "Maintenance";
        public const string NOKIA_IP_ROUTING = "IP Routing";
        public const string NOKIA_FIXED_ACCESS_POL = "Fixed Access - POL";
        public const string NOKIA_FIXED_ACCESS_FBA = "Fixed Access - FBA";
        public const string NOKIA_NUAGE = "Nuage";
        public const string NOKIA_NFM_P = "NFM-P";
        public const string NOKIA_MAINT_Y1 = "Maintenance Year1";
        public const string NOKIA_MAINT_Y2 = "Maintenance Year2+";
        public const string NOKIA_UNLIMITED = "Unlimited";
        public const string NOKIA_BIENNIAL = "Biennial";
        public const string NOKIA_GOLD = "Gold";
        public const string NOKIA_BRONZE = "Bronze";
        public const string NOKIA_NO = "No";
        public const string NOKIA_YES = "Yes";
        public const string NOKIA_BOM = "BOM";
        public const string NOKIA_BOM_APP = "BOM_";
        public const string NOKIA_Q2C = "PPA";
        public const string NOKIA_ORDERGENERATION = "Order Generation";
        public const string NOKIA_PRICECATALOGEXPORT = "PriceCatalogExport";
        public const string NOKIA_QUOTESUMMARY = "Quote Summary";
        public const string NOKIA_EXPORTQUOTE = "Export Quote";
        public const string NOKIA_BASICEXPORTQUOTE = "Basic Quote-Indirect"; //Added by RG for Req 6611
        public const string NOKIA_DIRECTEXPORTQUOTE = "Direct Export Quote";
        public const string NOKIA_CSPEXPORT = "CSP Export";
        public const string NOKIA_PROPOSAL = "Proposal";
        public const string NOKIA_CHARGE_TYPE_ERROR_MSG = "Competitive Additional Discount can only be entered at Total (One Time) level. Please remove CAD Amount from other Sub Totals and click Reprice.";
        public const string NOKIA_CAD_ERROR_MSG = "Competitive Additional Discount can not be entered more than Total (One Time) SubTotal. Please correct CAD Amount and click Reprice.";
        public const string NOKIA_STRING_APPENDER = "~~";
        public const string NOKIA_MAINTENANCE_STRING_APPENDER = "Maintenance~~";
        public const string NOKIA_PRODUCT_STRING_APPENDER = "Product~~";
        public const string NOKIA_OPTION = "Option";
        public const string ERRORMSG2 = "The Quote cannot be finalized as it contains products that do not belong to the portfolio you selected on the quote. Please remove the following items:";
        public const string ERRORMSG1 = "The Quote cannot be validated as it contains products that are no longer available for purchase. Please remove the following items:";
        public const string NOKIA_PRODUCT_SERVICES = "Product/Service";
        public const string NOKIA_UNDERSCORE = "_";
        public const string NOKIA_MAINTENANCE_CATALOGUE = "Maintenance Catalogue_";
        public const string NOKIA_PPA = "PPA-";
        public const string NOKIA_URL_PART1 = "/apex/Apttus_XApps__EditInExcelLaunch?selectedRecordId=";
        public const string NOKIA_URL_PART2 = "&appName=";
        public const string NOKIA_URL_PART3 = "&mode=touchless&outputType=EXCEL";
        public const string NOKIA_URL_PART4 = "&mode=touchless&outputType=None";
        public const string NOKIA_APPNAME = "appName";
        public const string NOKIA_SELECTED_RECORD_ID = "selectedRecordId";

        public const string NOKIA_PRM = "PRM";
        public const string NOKIA_PRODUCTID_SITE_APPENDER = "--";
        public const string NOKIA_PRODUCTCODE_ERROR_MSG = "Either the Product is inactive or does not exist.";
        public const string NOKIA_NULL = "null";
        public const string NOKIA_PRODUCT_LOADED_MSG = "Products loaded successfully..";
        public const string NOKIA_STRING_ID = "id";
        public const string NOKIA_COMMA = ",";
        public const string NOKIA_NEW_LINE = "\n";
        public const string NOKIA_NULL_APPENDER = "--null";
        public const string NOKIA_PRODUCT_NOTEXIST_MSG = "Product codes do not exist";
        public const string NOIKA_FILE_NOTSELECT_MSG = "No File Selected";
        public const string NOKIA_CSVFILE_ERROR_MSG = "CSV File Error";
        public const string NOKIA_PRODUCT_UPLOADERROR_MSG = "Products Upload Error";
        public const string NOKIA_PRODUCT_LOADED_MSG2 = "Product loaded....";
        public const string NOKIA_RETURN_TOCORT_URL1 = "/apex/Apttus_QPConfig__ProposalConfiguration?id=";
        public const string NOKIA_RETURN_TOCORT_URL2 = "&flow=NewUIPartner";
        public const string NOKIA_PRODUCT_VERSION = "Apttus_Config2__ProductVersion__c";
        public const string SOURCE = "Source__c";
        public const string CSV = "CSV";
        public const string NOKIA_YEAR1_MAINTENANCE = "Year 1 Maintenance";
        public const string NOKIA_YEAR2_BEYOND = "Maintenance for Y2 and Beyond";
        public const string NOKIA_YEAR2_BEYOND2 = "Maintenance for Y2 & Beyond";
        public const string NOKIA_SSP = "SSP";
        public const string NOKIA_SRS = "SRS";
        public const string DEPLOY_SERVICES = "Deploy Services";

        //O2Q Constants Start
        public const string FINALIZED = "Finalized";
        public const string FINALIZEDERRORMESSAGE = "Please finalize the cart";
        public const string CREATEOFFERSTAGE = "Create offer (bid)";
        public const string WINTHECASESTAGE = "Win the Case (Negotiate)";
        public const string PLEASECREATEOFFER = "Please create an offer with LoA Level to proceed";
        public const string PLEASECONFIGUREPRODUCTS = "Please configure products in the quote to proceed";
        public const string LOABYPASS = "LoA Bypass";
        public const string QUOTECANNOTBEVALIDATED = "Quote cannot be validated at the cuurent opportunity stage";
        public const string SUBMITTEDG4APPROVAL = "Submitted for G4 Approval";
        public const string OPPORTUNITYUNDERAPPROVAL = "Opportunity is already pending approval";
        public const string QUOTEDRAFT = "Draft";
        public const string QUOTEINREVIEW = "In Review";
        public const string QUOTEAPPROVED = "Approved";
        public const string QUOTEEXPIRED = "Expired";
        public const string QUOTEREJECTED = "Rejected";
        public const string QUOTECLOSED = "Closed(not won)";
        public const string OPTION = "Option";
        public const string QUOTEAPPROVEDSTATUS = "Quote Approved";
        public const string INREVIEWMESSAGEPARTNER = "Quote is In Review. You will be notified when the Quote is approved.";
        public const string INREVIEWMESSAGEPSM = "Quote is In Review, please submit the opportunity for G4 approval to complete the quote approval.";
        public const string INVALIDOFFERPARTNER = "Quote cannot be validated. Please contact your Partner Sales Manager (Invalid Offer condition).";
        public const string INVALIDOFFERPSM = "Quote cannot be validated against the Active Offer. Please create a new Active Offer with the required conditions.";
        public const string SLASH = "/";
        public const string OPPORTUNITY = "Opportunity";
        public const string OPPORTUNITYCANNOTBEVALIDATED = "Quote cannot be validated.Opportunity should be in Create offer(bid) stage.";
        public const string VALIDATECLASS = "NokiaCPQ_Validate_Quote_Ctrlr";
        public const string FUTURE = "Future";
        public const string APPROVALMETHODNAME = "submitforApproval";
        public const string QUOTEIDPARAMETER = "quoteId";
        public const string OPPIDPARAMETER = "oppId";
        public const string USDCURRENCY = "USD";
        public const string OPPORTUNITYSIDE = "NokiaCPQ_Opportunity_Site__c";
        public const string PRODUCTCATALOG = "Product Catalogue";

        public const string MAINTY1CODE = "MT001";
        public const string MAINTY2CODE = "MT002";
        public const string SSPCODE = "SSP002";
        public const string SRS = "SRS001";
        public const string INCENTIVES_STRING_APPENDER = "Incentives~~";
        public const string RENEWAL_STRING_APPENDER = "Renewal~~";
        public const string ATTACHMENT_STRING_APPENDER = "Attachment~~";
        public const string MULTIYR_STRING_APPENDER = "Multi-Year~~";
        public const string PERFORMANCE_STRING_APPENDER = "Performance~~";
        public const string OPTICS = "Optics";

        // O2Q Constant END
        //static variable to handle recursion
        public const string ProductAfterTriggerExecute = "False";
        public const string ProposalBeforeUpdateTriggerExecute = "False";
        public const string ProposalAfterUpdateTriggerExecute = "False";
        public const string PCBBEFOREPRICINGINADJ = "False";
        public const string ProposalAfterUpdate = "True";
        public const string ProposalBeforeUpdate = "True";
        public const string ProductConfigAfterUpdate = "True";


        public const string QUOTE_TYPE_INDIRECTCPQ = "Indirect CPQ";
        public const string QUOTE_TYPE_DIRECTCPQ = "Direct CPQ";
        public const string EUR_CURR = "EUR";
        public const string PLTYPE_DIRECT = "Direct";
        public const string PLTYPE_CPQ = "CPQ";
        public const string OBJECT_ACCOUNT = "Account";
        public const string QUOTE_TYPE_DS = "DS";

        //Maintenance Type Lightnimg component constants
        public const string Maintenance_Type_List_Values = "Bronze (Return for Exchange);Gold (Return for Exchange);Gold (Return for Repair);Gold & Advanced Exchange Next Business Day";
        public const string PROPOSAL_OBJECT = "Apttus_Proposal__Proposal__c";
        public const string FAIL_STRING = "FAIL";
        public const string SUCCESS_STRING = "SUCCESS";
        public const string IP_ROUTING_STRING = "IP Routing";
        public const string AIRSCALE_WIFI_STRING = "Airscale Wifi";
        public const string SSP_DEFAULT_VALUE = "Unlimited";
        public const string SRS_DEFAULT_VALUE = "Unlimited";
        public const string NO_STRING = "No";
        public const string YES_STRING = "Yes";
        public const string ONE_YEAR_STRING = "1";
        public const string TWO_YEAR_STRING = "2";
        public const string THREE_YEAR_STRING = "3";
        public const string FIVE_YEAR_STRING = "5";
        public const string MAINT_TYPE_DEFAULT_VALUE = "Gold (Return for Exchange)";
        public const string PRICING_LEVEL_ALL_OTHERS = "All Others";
        public const string BRAND_STRING = "Brand";
        public const string AED_STRING = "AED";
        public const string STRING_EXISTING_MAINT_CNT = "NokiaCPQ_Existing_IONMaint_Contract__c";
        public const string STRING_SSP_API_NAME = "NokiaCPQ_SSP_Level__c";
        public const string STRING_SRS_API_NAME = "NokiaCPQ_SRS_Level__c";
        public const string STRING_NO_OF_YEARS = "NokiaCPQ_No_of_Years__c";
        public const string NONE_STRING = "--None--";
        public const string IN_REVIEW_STRING = "In Review";
        public const string APPROVED_STRING = "Approved";
        public const string EXPIRED_STRING = "Expired";
        public const string CLOSED_NOT_WON_STRING = "Closed(not won)";
        public const string ACCEPTED_STRING = "Accepted";
        public const string BLANK_STRING = " ";
        public const string BLANK_STRING_WITHOUT_SPACE = "";
        public const string SEMICOLON_STRING = ";";
        public const string UPDATE_MAINTENANCE_TYPE_VALUE = "updateMaintenanceTypeValue()";
        public const string ERROR_MSD = "Error occurred";
        public const string AirScaleWiFi = "AirScaleWiFi";

        //NokiaCPQquoteActionsController
        public const string PRM_QUOTE_BUTTON = "Internal_Detailed_Quote_Export_for_PrM__c";
        public const string LOA_QUOTE_BUTTON = "LOA_Export__c";
        //proposal trigger
        public const string TRUE_STRING = "True";
        public const string QUOTE_NOT_FOUND = "Quote Not found";

        //reprice functionality
        public const string BATCHAPEX_STRING = "BatchApex";
        public const string PROCCESSING_STRING = "Processing";
        public const string PREPARING_STRING = "Preparing";
        public const string HOLDING_STRING = "Holding";
        public const string QUEUED_STRING = "Queued";
        public const string QUOTEID_STRING = "quoteId";
        public const string PENDING_STRING = "Pending";
        public const string REPRICING_WAIT_MSG = "Repricing.... Please Wait";
        public const string REPRICING_COMPLETE_MSG = "Reprice Complete.Finalizing the cart......Please Wait";
        public const string FINALIZE_MSG = "Finalize Complete";
        public const string COMPLETE_MSG = "Complete";

        //collaboration functionality
        public const string COLLABORATION_REQUEST = "Collaboration Request";
        public const string ACCEPTED = "Accepted";

        //toast messages
        public const string REPRICENOTREQ = "Reprice is not required at the moment. Please note that quotes with collaboration in progress can only be repriced after completion of collaboration.";
        public const string REPRICEBATCHSUBMITTED = "Request to reprice quotes was submitted successfully. You will be notified via e-mail & chatter once the quote reprice is completed. Please note that quotes with collaboration in progress will not be repriced.";
        public const string REPRICEBATCHOVERLOAD = "Request to reprice cannot be submitted at this time due to system overload.Please try again after some time";
        public const string SINGLEREPRICENOTREQ = "Reprice not required at the moment";
        public const string SINGLEREPRICEINCOLLAB = "Repricing cannot be initiated until quote collaboration is completed.";
        public const string CONFIGUREFIRST = "Please configure the products in the quote before repricing";

        //DirectToastMessage Cons.
        public const string CLASS_NAME_STRING = "Class Name : ";
        public const string FUNCTION_NAME_STRING = "Function Name : ";
        public const string FUNCTION_CHECK_CONFIG_LINE_ITEMS = "checkExistingConfigurationLineItems()";

        //NokiaCPQ_Validate_Quote_Ctrlr
        public const string JobType = "Future";
        public const string productConfig = "Finalized";
        public const string profilename_contains = "Partner";
        public const string lin_Apttus_QPConfig = "Product/Service";
        public const string lin_Apttus_QPConfig_if_else = "option";
        public const string opportunity_sname = "Create offer bid";
        public const string quote_Nokia_CPQ_EATC = "PRM";
        public const string Job_Status = "Completed";
        public const string ExceptionHandle = "Product Configuration Not present";
        public const string addmessage = "Please finalize the cart";
        public const string ErrorNoLOA = "The offer condition of the related opportunity is set to LoA bypass: No LoA. For successful quote validation this condition requires:<br/>1. No manual discounts<br/>2. All CNP based on contracted CLP<br/>3. Quote total below 5 million Euro<br/>Please make sure to fulfill the above criteria or update the offer condition.";
        public const string ErrorPreApproved = "The offer condition of the related opportunity is set to LoA bypass: Pre-approved for Account Manager. For successful quote validation this condition requires:<br/>1. No manual discounts<br/>2. All CNP based on contracted CLP<br/>3. Quote total below 5 million<br/>Please make sure to fulfill the above criteria or update the offer condition.";
        public const string ErrorAuthorized = "The offer condition of the related opportunity is set to LoA bypass: Authorized for Pricing Manager. For successful quote validation this condition requires:<br/>1. All discounts within the green area<br/>2. Quote total below 5 million<br/>Please make sure to fulfill the above criteria or update the offer condition.";


        public const string DOWNLOAD_OFFER = "Download Offer";

        //proposalTriggerHelper
        public const string CANCEL_ACTION = "Cancel Action";
        public const string DIRECT_REC_TYPE = "Direct_Record_Type";
        public const string RECALL_ACTION = "Recall Action";
        public const string ERROR_ONE = "Please select a valid portfolio value, you are not authorized for ";
        public const string ERROR_TWO = "The quotation cannot be saved. Please select values for all required fields.";
        public const string INSIDE_QUOTE = "Inside quote";

        //ProcessLineItemBatch
        public const string Finish = "Finished Reprice";
        public const string Repricing = "The quote repricing request for Opportunity \"";
        public const string Nokia_Partner = "Nokia Partner Portal";
        public const string Refer = "\" is complete. Please refer below link for more details- \n\n";
        public const string Query = "Select Id, Apttus_Config2__ConfigurationId__c From Apttus_Config2__LineItem__c where Id IN: lineItemIdList";

        //Nokia_PricingCallBack
        public const string STANDARD = "Standard price";
        public const string AFTERPRICING = "afterPricing";
        public const string DEFAULTPENDING = "Default Pending";
        public const string NOKIA_EPT = "EPT";
        public const string CALCULATE_QUANTITY = "calculateTotalQuantityForSRSDataRef";
        public const string AGREEMENT = "Agreement";

        //NokiaCPQ_AttachmentHandler
        public const string Edit = "Edit";
        public const string xlsm = ".xlsm";
        public const string text_csv = "text/csv";
        public const string None = "None";
        public const string PRMEXPORT = "Internal Detailed Quote Export for PrM";
        public const string LOAEXPORT = "LOA_file_";
        public const string CSPEXPORT = "P20 (CSP) Export_";

        public const string STANDARD_PRICE = "Standard Price";
        public const string BUNDLE = "Bundle";
        public const string FALSE_CONSTANT = "False";
        public const string TRUE_CONSTANT = "true";

        // Added by Hardik Shah
        // for Support Ticket : 12088
        public const string DATA_REFINERY = "Data Refinery";
        public const string FASTER_MIND = "Fastermind";
        public const string DATA_REFINERY_SRS = "Data Refinery TL SRS";
        public const string FASTER_MIND_SRS = "Fastermind TL SRS";

        //APTS_CustomActionCallback
        public const string Errormessage3 = "Copy is not allowed on EPT Products: ";

        //NeedsRepriceQuoteRepricePriceList
        public const string QUERY1 = "\",\"";
        public const string QUERY2 = "\")";
        public const string NOKIA_SOFTWARE = "Nokia Software";
        public const string BASIC85 = "Basic (8x5)";
        public const string SUBSCR = "Subscription";
        public const string GOLD247 = "Gold (24x7)";
        public const string PLAT_SLA = "Platinum (higher SLA)";

        //AccreditationHandler
        public const string ACCRD_HANDLER = "AccreditationHandler";

        //AccreditationTrigger
        public const string AccredAfterTriggerExecute = "False";

        //AccreditationTriggerHelper
        public const string PRICING_TEAM = "Pricing Team";
        public const string CHATTER1 = "\n A new Accreditation record has been added or the Current Pricing Level has been changed. Please see the information below. \n \n Partner Name:";
        public const string CHATTER2 = "\n Accreditation Type : ";
        public const string CHATTER3 = "\n Portfolio : ";
        public const string CHATTER4 = "\n Accreditation Level : ";
        public const string CHATTER5 = "\n Reason for Status Change : ";
        public const string CHATTER6 = "\n Pricing Level Override  : ";
        public const string CHATTER7 = "\n Pricing Override Expiration : ";
        public const string CHATTER8 = "\n Current Pricing Level : ";
        public const string ACCR_TRIGGER_NAME = "AccreditationTrigger";

        //Values From PCB
        public const string PRODUCTITEMTYPESOFTWARE = "Software";

        //Quote Actions
        public const string SCRM = "sCRM";

        //NokiaCPQ_CSP_Export_Generator
        public const string SOFTWARE_STRING = "Software";
        public const string HARDWARE_STRING = "Hardware";
        public const string SERVICE_STRING = "Service";
        public const string SERVICES_STRING = "Services";
        public const string GS_Deploy_STRING = "GS Deploy Services Items";
        public const string Deploy_Services_STRING = "Deploy Services";
        public const string Attribute_STRING = "ATTRIBUTE";
        public const double double_zero = 0.0;
        // req. 5676
        public const bool RUN_AFTER_UPDATE_QUOTE_TRIGGER = true;
        public const string QUOTE_EXTERNAL_TARGET_SYSTEM_ALLIANCE = "Alliance";
        public const string SLI_PREFIX_FOR_ALLIANCE_EXPORT = "NOK:";
        public const string ALLIANCE_EXPORT_PRODUCT_TYPE_SLI = "SLI";
        public const string ALLIANCE_EXPORT_PRODUCT_TYPE_CO = "CO";
        public const string ALLIANCE_EXPORT_PRODUCT_LEVEL_TWO = "2";
        public const string ALLIANCE_EXPORT_PRODUCT_LEVEL_ONE = "1";
        public const string ALLIANCE_EXPORT_PRODUCT_CATEGORY_CDE = "CDE";
        public const string ALLIANCE_EXPORT_PRODUCT_CATEGORY_SSP_SRS = "SSP/SRS";
        public const string ALLIANCE_EXPORT_PRODUCT_CATEGORY_CARES = "CAREs";
        public const string ALLIANCE_EXPORT_PRODUCT_CATEGORY_HW = "HW";
        public const string ALLIANCE_EXPORT_PRODUCT_CATEGORY_SW = "SW";
        public const string ALLIANCE_EXPORT_PRODUCT_CATEGORY_CS = "CS";
        public const string ALLIANCE_EXPORT_ITEM_CODE_FOR_HW = "FP_MF02_MAT_EXTER";
        public const string ALLIANCE_EXPORT_ITEM_CODE_FOR_SW = "FP_AF08_SW_LICENCE";
        public const string ALLIANCE_EXPORT_ITEM_CODE_FOR_OHER_SERVICES = "FP_SF03_NE_S_INTEG";
        public const string ALLIANCE_EXPORT_SUCCESS_STRING = "SUCCESS";
        public const string ALLIANCE_EXPORT_ERROR_STRING = "ERROR: ";
        public const string ALLIANCE_EXPORT_ERROR_MESSAGE_STRING = "ERROR: There is no Proposal Line Items associated with current Quote!";
        public const string QTC_FILE_NAME_PREFIX = "QTC (Surround) Export_<";
        public const string LESS_THAN_SYMBOL = ">";
        public const string ENGLISH_ALPHABET_S = "S";
        public const string ENGLISH_ALPHABET_V = "V";
        public const string TXT_FILE_EXTENSION = ".txt";
        public const string QTC_FILE_COLUMN_HEADER = "#Services|item_code|quantity|price_class|description|adjust_amt";
        public const string QTC_FILE_NEW_LINE = "\r\n";
        public const string QTC_FILE_LINE_1 = "Tool|Version||||";
        public const string QTC_FILE_LINE_2 = "VIPRE|smartCPQ||||";
        public const string QTC_FILE_COLUMN_SEPERATOR = "|";
        public const string QTC_FILE_WORD_SERVICE = "#Services";
        public const string NOKIA_STANDALONE = "Standalone";
        public const string NOKIA_SCPQ = "sCPQ";
        public const string SUMMARY_LINE = "Standard Price (One Time)";

        //req. 4623
        public const bool TrueValue = true;

        //Req 6000
        public const string ZERO = "0";
        public const string NEGATIVE_DISCOUNT = "-100";

        //Req 5258
        public const string Saved = "Saved";

        //Req 6238 
        public const string Split = "Split";
        public const string SplitLevelOne = "1";
        public const string Main = "Main";
        public const string Table = "Table";

        //Req 6225
        public const string DISCOUNT_PERCENT = "% Discount";
        public const string DISCOUNT_AMOUNT = "Discount Amount";
        public const string PRICE_OVEERIDE = "Price Override";
        //Req 6337
        public const string AdvacncePricing_error = "Please relaunch the Advanced Pricing app to update the Net Price correctly and click on Reprice in the cart afterwards.";

        // req. 6316	
        public const string QTC_SITE_FILE_NAME_PREFIX = "QTC (SITE) Export_<";
        public const string ITEM_TYPE_OFFER = "Offer";
        public const string VALID = "VALID";
        public const string INDIA_V_SELECT = "IndivSelect";
        public const string SALES_ORDER_NUMBER = "12345678";
        public const string SUBORDER_NUMBER = "123";
        public const string ENGLISH_ALPHABET_X = "X";
        public const string ENGLISH_ALPHABET_R = "R";
        public const string ENGLISH_ALPHABET_E = "E";
        public const string CONFIGURATOR_OUTPUT = "CONFIGURATOR OUTPUT";
        public const string ERROR_EXPORT_FILE_CREATION = "An Error occurred during \"Export File\" creation. Kindly contact your system administrator!";
        public const string NO_SALES_ITEM_FOR_QTC_SITE_EXPORT = "No Sales Item found for QTC (SITE) Export.";
        public const string QTC_SIT_EXPORT_TOP_LEVEL_XML_TAGS = "TITLE:ORDER_DATA:CONFIG_TOOL:ORDERABLE_ITEM";
        public const string QTC_SIT_EXPORT_ORDER_DATA_XML_TAGS = "SALES_ORDER:SUBORDER:SEGMENT:VERSION:OPERATION:OMS:LOGIN_ID";
        public const string QTC_SIT_EXPORT_CONFIG_TOOL_XML_TAGS = "TOOL_NAME:TOOL_STATUS:CONFIG_ID:VINTAGE";
        public const string QTC_SIT_EXPORT_ORDERABLE_ITEMS_XML_TAGS = "OI_ID:OI_TITLE:OI_QTY:CONFIG_DESC";
        public const string QTC_SIT_EXPORT_ROOT_XML_TAG = "CONFIG_DATA";
        public const string QTC_SIT_EXPORT_IST = "IST";
        public const string DOT = ".";
        // Req. 5934
        public const string EXTERNAL = "External";
        public const string SERVICE_PCI = "Service PCI";
        public const string EQUIPMENT_PCI = "Equipment PCI";
        public const string HYPHEN = "-";
        public const string DUMMY = "Dummy";
        public const string ENGLISH_ALPHABET_Y = "Y";
        public const string ENGLISH_ALPHABET_N = "N";
        public const string ALL = "ALL";
        public const string PCI_SEMICOLON = "PCI;";
        public const bool isCloneRun = false;

        public const string GS_DEPLOY = "GS Deploy Service Product";

        //Req 6383

        public const string WARRANTYCREDIT = "Warranty Credit Applicable For Maintenance Only Quote";
        public const string CONTRACTSTARTDATE = "Contract Start Date Applicable For Maintenance Only Quote";
        public const string WARRANTYCONTRACTSTARTDATE = "Warranty Credit & Contract Start Date Applicable For Maintenance Only Quote";

        //QTC Constants

        public const string FlexibleGroupController = "FlexibleGroupController";
        public const string getlineitems = "getlineitems";
        public const string updatemarketmodel = "updatemarketmodel";
        public const string RemoveLineItems = "RemoveLineItems";
        public const string getbundledata = "getbundledata";
        public const string QTC_PORTFOLIO = "QTC";
        public const string SITE_STRING = "Site";
        //DS Update Quote Approval Stage + ION Direct Is List Price Only
        public const string CPQ_GSS_User = "CPQ_GSS_User";

        //CPQ Req 6624
        public const string RED = "RED";
        public const string GREEN = "GREEN";
        public const string CPQ_SALES_USER = "CPQ_Sales_User";
        public const string PRICING_MANAGER = "PricingManager";
        public const string YELLOW = "YELLOW";

        //CPQ indirect 
        public const string NOKIA_LEO_WARNING_MSG = "Leo Quote amount is less than EUR 500. Please verify";

        //Performance Improvement
        public const string SUBTOTAL_ONE_TIME = "Subtotal - Standard Price (One Time)";
        public const string TOTAL_ONE_TIME = "Total (One Time)";
        public const bool flagOnSummary = true;
        public const bool isCloneLineItem = false;

        //CartService Nova 
        public const string NokiaOrder = "Order";
        public const string Nokia_CART_Interface = "CART Interface";
        public const string Nokia_Upsert = "Upsert";
        public const string Nokia_Cart_service_Interface = "CART Service Interface - CartId: ";
        public const string Nokia_ConfigId = " ConfigId: ";
        public const string NokiaCPQ_Unitary_Cost = "NokiaCPQ_Unitary_Cost__c";
        public const string Nokia_Item_Type_From_CAT = "Item_Type_From_CAT__c";
        public const string Nokia_PCI_Code = "PCI_Code__c";
        public const string NokiaCPQ_IsArcadiaBundle = "NokiaCPQ_IsArcadiaBundle__c";
        public const string NokiaCPQ_Unitary_Cost_Initial = "NokiaCPQ_Unitary_Cost_Initial__c";
        public const string Nokia_Insert_Delivery_Sales_Items = "Insert Delivery Sales Items";
        public const string Nokia_CARTService = "CARTService";
        public const string Nokia_createInterfaceLog = "createInterfaceLog";
        public const string Nokia_Required_Data_Missing = "Required Data Missing";
        public const string Nokia_Missing_configuration_data = "Missing configuration data";
        public const string Nokia_Missing_BOM_Data = "Missing BOM Data";
        public const string Nokia_Missing_Main_Bundle_Line_Item_Id = "Missing Main Bundle Line Item Id";
        public const string Nokia_Missing_Cart_Id = "Missing Cart Id";
        public const string NokiaCPQ_ZID = "-ZID";
        public const string NokiaCPQ_ZID1 = "ZID";
        public const string NokiaCPQ_ZNAME = "-ZNAME";
        public const string NokiaCPQ_ZNAME1 = "ZNAME";
        public const string NokiaCPQ_CHARC = "-CHARC";
        public const string NokiaCPQ_CHARC1 = "CHARC";
        public const string NokiaCPQ_CHARC_TXT = "-CHARC_TXT";
        public const string NokiaCPQ_CHARC_TXT1 = "CHARC_TXT";
        public const string NokiaCPQ_VALUE = "-VALUE";
        public const string NokiaCPQ_VALUE1 = "VALUE";
        public const string NokiaCPQ_VALUE_TXT = "-VALUE_TXT";
        public const string NokiaCPQ_VALUE_TXT1 = "VALUE_TXT";
        public const string NokiaCPQ_digits = "&#124;";
        public const string NokiaCPQ_Hyphen = "-";
        public const string NokiaCPQ_Colon = ":";
        public const string NokiaCPQ_errorMessage = "These Products are selected in CAT but not available in Apttus CPQ- ";
        public const string NokiaCPQ_txt = "text/plain";
        public const string NokiaCPQ_cfgdata = "cfg_data.txt";
        public const string NokiaCPQ_Success = "Success";
        public const string NokiaCPQ_spaceComma = " ; ";
        public const string NokiaCPQ_CartService = "CartService";
        public const string NokiaCPQ_insertAttachment = "insertAttachment";
        public const string NokiaCPQ_checkArcadiaItems = "checkArcadiaItems";
        public const string NokiaCPQ_addSelectedOptions = "addSelectedOptions";
        public const string NokiaCPQ_addArcadiaItems = "addArcadiaItems";
        public const string NokiaCPQ_parseConfigString = "parseConfigString";
        // ProductConfigTriggerHelper- NovaSuite
        public const int Two = 2;
        public const string directDS = "Direct DS";
        public const int minusOne = -1;
        public const int Three = 3;
        public const string dotZero = ".00";
        public const string updateFieldsOnProdConfig = "updateFieldsOnProdConfig";
        public const string ProductConfigTriggerHelper = "ProductConfigTriggerHelper";

        //NovaSuiteFixes
        public const string pricingManagerPS = "Nokia_CPQ_Pricing_Manager";
        public const string adminPS = "CPQ_Admin";
        public const string sendAddQuoteMessageToEAImethod = "sendAddQuoteMessageToEAI";
        public const string Error_String = "Error";
        public const string Closed_String = "Closed";
        public const string Fail = "Fail";
        public const string getQuoteDeliverySalesItemsForArcadiamethod = "getQuoteDeliverySalesItemsForArcadia";
        public const string generateParentToChildOptionsHierarchymethod = "generateParentToChildOptionsHierarchy";
        public const string FN = "FN";
        public const string Primary = "Primary";
        public const string Activated = "Activated";
        public const string ApprovalRequired = "Approval Required";
        public const string Generated = "Generated";
        public const string Denied = "Denied";
        public const string Closed_Not_Won = "Closed (Not Won)";
        public const string Validate = "Validate";
        public const string Submit = "Submit";
        public const string Optics_Wavelite = "Optics - Wavelite";
        public const string Error = "Please contact your PSM to check your Partner Certification";
        public const string sObjectType = "sObjectType";
        public const string sObjectId = "sObjectId";
        public const string Pricing_Manager1 = "Pricing Manager";
        public const string Approved_with_DOD = "Approved with DOD";
        public const string Line_number = "Line number ";
        public const string Qword = "Q";
        public const string Distributor = "Distributor";
        public const string PRODSTATUS = "CPQ_ProductStatusCheckController";
        public const string GETPRODSTATUS = "getProductStatusResult";
        public const string PRODNAME = "{ProdName}";
        public const string PRODCODE = "{ProdCode}";
        public const string NokiaCPQ_PRICE = "Price";
        public const string PendingApproval = "Pending Approval";
        public const string QTC = "QTC";
        public const string Nokia_New = "New";
        public const string Nokia_Brand = "Nokia Brand of Service";
        public const string Nokia_Maintenance_level_api = "NokiaCPQ_Maintenance_Level__c";
        public const string Working = "working";

        public const string Nokia_FASTMILE = "FastMile";
        public const string NOKIA_1YEAR = "1";
        public const string NOKIA_LEO = "LEO";
        public const string NOKIA_NAMCLUSTER = "NAM";
        public const string NOKIA_ONT = "ONT";
        public const string NOKIA_PTP = "PTP";
        public const string Configure = "Configure";
        public const string Manual = "Manual";
        public const string NokiaCPQShareProposalsWithOTM = "NokiaCPQShareProposalsWithOTM";
        public const string unshareProposalRecordAfterOTMTriggerDelete = "unshareProposalRecordAfterOTMTriggerDelete";
        public const string CUSTPROD001 = "CUSTPROD001";
        public const string Direct_DS = "Direct DS";
        public const string WAVELITESOURCE = "Wavelite";
        public static List<string> pdcList = new List<string>() { "NFM - P IPR", "5620 SAM IPR" };
        public const string NULL_PRODUCT_DISCOUNT_CATEGORY_KEY = "NULL_PRODUCT_DISCOUNT_CATEGORY_KEY";
    }

    class Labels
    {
        public static List<string> SRSPDC = new List<string>() { "NFM - P IPR", "5620 SAM IPR" };
        public static List<string> FN_SSP_Product = new List<string>() { "301049607", "3FE30998BA" };
        public static List<string> NokiaCPQ_Care_Proactive = new List<string>() { "785500098210", "784600098210", "784800098210" };
        public static List<string> NokiaCPQ_Care_Advance = new List<string>() { "785500098209", "784600098209", "784800098209" };
    }
}
